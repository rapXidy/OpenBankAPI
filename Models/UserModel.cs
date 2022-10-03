using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankAPI.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }
   
        public string Role { get; set; }
        
        [Required, RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$", ErrorMessage = "Please enter valid 10 digit Account Number")]
        public string AccountNumber { get; set; }
        
        [Required]
        public string Password { get; set; }

        [JsonIgnore]
        public DateTime DateSignedUp { get; set; } = DateTime.Now;
    }

}
