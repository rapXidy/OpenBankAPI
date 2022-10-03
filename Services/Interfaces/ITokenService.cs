using BankAPI.DTOs;

namespace BankAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDTO user);
        bool ValidateToken(string key, string issuer, string audience, string token);
    }

}
