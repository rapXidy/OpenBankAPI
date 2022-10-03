using BankAPI.DAL;
using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankAPIDBContext _dbContext;

        public AccountService(BankAPIDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            //lets make authenticate
            //does account exists for that number?
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;
            //ok so we have a match
            //verify pinHash
            if(!VerifyPinHash(Pin,account.PinHash,account.PinSalt))
                return null;

            //ok so Authenticate is passed
            return account;
        }

        private static bool VerifyPinHash(string pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentNullException("pin");
            //now lets verify the pin
            using(var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
                for(int i = 0; i < computedPinHash.Length; i++)
                {
                    if(computedPinHash[i] != pinHash[i])
                        return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            //this is to create a new account
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) 
                throw new ApplicationException("An account already exists with this email");
            //validate pin
            if (!Pin.Equals(ConfirmPin))
                throw new ApplicationException("Pins do not match!");

            //now all validation passes, lets create account,
            //we are hashing/encrypting pin first
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt); // lets create this crypto method

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            account.DateLastUpdated = DateTime.Now;
            account.DateCreated = DateTime.Now;

            //all good add new account to db
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return account;
        }

        private void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if(account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account=_dbContext.Accounts.Where(x=>x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null)
                return null;

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x=>x.Id==Id).FirstOrDefault();
            if (account == null)
                return null;

            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            //update is more tasky

            var accounToBeUpdated = _dbContext.Accounts.Where(x=>x.Email==account.Email).SingleOrDefault();// we can actually use Id to look it up instead of email...
            if (accounToBeUpdated == null)
                throw new ApplicationException("Account does not exist");
            //if it exists, lets listen for user wanting to change any of his properties
            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                //this means that user wishes to change his/her email
                //check if the one he's changing to, is'nt already taken
                if(_dbContext.Accounts.Any(x=>x.Email == account.Email))
                    throw new ApplicationException("This email "+ account.Email+" already exists!");
                //else change email for him

                accounToBeUpdated.Email = account.Email;
            }
            //actually we want to allow the user to be able to change only email and phone number and pin
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                //this means that user wishes to change his/her phone
                //check if the one he's changing to, is'nt already taken
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber))
                    throw new ApplicationException("This phone " + account.PhoneNumber + " already exists!");
                //else change phone for him

                accounToBeUpdated.PhoneNumber = account.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(Pin))
            {
                //this means that user wishes to change his/her pin
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accounToBeUpdated.PinHash = pinHash;
                accounToBeUpdated.PinSalt = pinSalt;

            }
            accounToBeUpdated.DateLastUpdated= DateTime.Now;
            ///now persist this update to db
            _dbContext.Accounts.Update(accounToBeUpdated);
            _dbContext.SaveChanges();
        }
    }
}
