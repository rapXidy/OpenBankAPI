using BankAPI.Models;
using System.Collections.Generic;

namespace BankAPI.Services
{
    public interface IAccountService
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        void Update(Account account, string Pin = null);
        void Delete(int Id);
        Account GetById(int Id);
        Account GetByAccountNumber(string AccountNumber);
    }
}
