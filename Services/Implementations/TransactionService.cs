using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BankAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private BankAPIDBContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountService _accountService;

        public TransactionService(BankAPIDBContext dbContext, 
            ILogger<TransactionService> logger, 
            IOptions<AppSettings> settings,
            IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            //Create a new transaction
            Response response = new Response();
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully";
            response.Data = null;

            return response;
        }

        public Response FindTransactionByDate(DateTime date)
        {
            //get transaction by date
            Response response = new Response();
            var transaction = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList();//because there are many transactions in a day
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successsfully";
            response.Data = transaction;

            return response;

        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            //make deposit here...
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need to authenticate in UserService, so let's inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            //so validation passes...
            try
            {
                //for deposit, our bankSettlementAcount is the source giving money to the user's account
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _accountService.GetByAccountNumber(AccountNumber);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update
                if((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                   (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful";
                    response.Data = null;

                }
                else
                {
                    //so transaction failed
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }
            //SET OTHER PROPS OF TRANSACTION HERE
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $@"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject
                (transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            //lte's implemement funds transfer here

            //make funds transfer..
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need to authenticate in UserService, so let's inject IUserService here
            var authUser = _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            //so validation passes...
            try
            {
                //for funds transfer, our ToAccountNumber is the destination, getting money from the sender's account
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                   (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful";
                    response.Data = null;

                }
                else
                {
                    //so transaction failed
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }
            //SET OTHER PROPS OF TRANSACTION HERE
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $@"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject
                (transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;


        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            // lets implement withdrawal now...
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need to authenticate in UserService, so let's inject IUserService here
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            //so validation passes...
            try
            {
                //for withdrawal, our bankSettlementAcount is the destination getting money from the user's account
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                   (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful";
                    response.Data = null;

                }
                else
                {
                    //so transaction failed
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }
            //SET OTHER PROPS OF TRANSACTION HERE
            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $@"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject
                (transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject
                (transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }
    }
}
