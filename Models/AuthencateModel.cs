using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    public class AuthencateModel
    {
        [Required] //lets validate that the accountno is 10 digits using regex attribute
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$")]
        public string AccountNumber { get; set; }
        [Required]
        public string Pin { get; set; }
    }
}
