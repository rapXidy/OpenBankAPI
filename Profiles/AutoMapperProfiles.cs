using AutoMapper;
using BankAPI.DTOs;
using BankAPI.Models;

namespace BankAPI.Profiles
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterNewAccountModel, Account>();

            CreateMap<UpdateAccountModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<UserModel, UserReadDTO>();
            CreateMap<TransactionRequestDTO, Transaction>();

            //I'll create these DTO classes also

        }
    }
}
