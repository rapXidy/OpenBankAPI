using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankAPI.DTOs
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage ="Passwords do not match!")]
        public string ConfirmPassword { get; set; }
        [JsonIgnore]
        public string Role { get; set; }
    }

}
