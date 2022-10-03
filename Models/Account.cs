using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankAPI.Models
{
    [Table("Accounts")]
    public class Account
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
        
        //we'll also store the hash and salt of the account transaction pin
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        //now to generate an account number, we do that in the constructor
        //first, lets create a random object
        Random rand = new Random();
        public Account()
        {
            AccountNumberGenerated = Convert.ToString((long)Math.Floor(rand.NextDouble() * 9_000_000_000L + 1_000_000_000L));
            //we did 9_000_000_000L so we could get a 10 digit random number
            //also, AccountName property = FirstName+LastName
            AccountName = $"{FirstName} {LastName}"; //eg John Doe
        }

    }

    public enum AccountType
    {
        Savings,
        Current,
        Corporate,
        Government
    }
}
