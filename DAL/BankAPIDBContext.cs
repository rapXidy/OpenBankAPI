using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.DAL
{
    public class BankAPIDBContext:DbContext
    {
        public BankAPIDBContext(DbContextOptions<BankAPIDBContext> options): base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserModel> UserModels { get; set; }
    }
}
