using BankAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.DTOs
{
    public class GetAccountModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;//here customer will receive OTP and account no.
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }// This will be an enum to show if the account to be created is "Corporate", "savings" or "current"
        public string AccountNumberGenerated { get; set; }//we shall generate account no. here
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }


    }
}
