
using BankAPI.Models;
using System;

namespace BankAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Response CreateNewTransaction(Transaction transaction);
        Response FindTransactionByDate(DateTime date);
        Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin);
        Response MakeFundsTransfer(string FromAccount, string ToAccount,decimal Amount, string TransactionPin);
    }
}
