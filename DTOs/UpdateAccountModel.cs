using System;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.DTOs
{
    public class UpdateAccountModel
    {

        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;//here customer will receive OTP and account no.
                                                         //lets add Regular expression for pin. must be 4 digits.
        [Required]
        [RegularExpression(@"^[0-9]/d{4}$", ErrorMessage = "Pin must not be more than 4 digits")]
        public string Pin { get; set; }
        [Required]
        [Compare("Pin", ErrorMessage = "Pins do not match")]
        public string ConfirmPin { get; set; } //we want to compare both of them...

        public DateTime DateLastUpdated { get; set; }


    }
}
