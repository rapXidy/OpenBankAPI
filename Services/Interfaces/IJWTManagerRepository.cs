using BankAPI.Models;

namespace BankAPI.Services.Interfaces
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(AuthenticateModel users);
    }
}
