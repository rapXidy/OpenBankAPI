using BankAPI.DAL;
using BankAPI.DTOs;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using System;
using System.Linq;

namespace BankAPI.Services.Implementations
{
    public class UserRepository:IUserRepository
    {
        private readonly BankAPIDBContext _dbContext;

        public UserRepository(BankAPIDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UserModel GetUser(string username, string password)
        {
            var account = _dbContext.UserModels.SingleOrDefault(x => x.UserName == username && x.Password == password);
            
            if (account == null)
                return null;

            return account;
        }

        public UserModel RegisterNewUser(UserModel user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));
            
            if (_dbContext.UserModels.Any(x => x.UserName == user.UserName))
                throw new ApplicationException("An account already exists with this email");
          
            //all good add new account to db
            _dbContext.UserModels.Add(user);
            _dbContext.SaveChanges();

            return user;
    
        }
    }
}
