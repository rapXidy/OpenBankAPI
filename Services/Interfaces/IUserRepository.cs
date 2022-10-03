using BankAPI.DTOs;
using BankAPI.Models;

namespace BankAPI.Services.Interfaces
{
    public interface IUserRepository
    {
        UserModel GetUser(string username, string password);
        UserModel RegisterNewUser(UserModel user);
    }

}
