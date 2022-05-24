using BankAPI.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankAPI.DTOs
{
    /// <summary>
    /// Basically this is a data transfer object. its intended for data hiding
    /// it contains all objects in Account model except a few we 
    /// want to hide from users.
    /// </summary>
    public class RegisterNewAccountModel
    {
        public RegisterNewAccountModel()
        {
            AccountName = $@"{FirstName} {LastName}";
        }

        [JsonIgnore]
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid!!")]
        public string Email { get; set; }//here customer will receive OTP and account no.
        public AccountType AccountType { get; set; }
        //public DateTime DateCreated { get; set; }
        //public DateTime DateLastUpdated { get; set; }
        //lets add Regular expression for pin. must be 4 digits.
        [Required]
        [RegularExpression(@"^[0-9]{4}$",ErrorMessage ="Pin must not be more than 4 digits")]
        public string Pin { get; set; }
        [Required]
        [Compare("Pin",ErrorMessage ="Pins do not match")]
        public string ConfirmPin { get; set; } //we want to compare both of them...

    }
}
