using System.ComponentModel.DataAnnotations;

namespace bankaccounts.Models
{
    public class LoginUser
    {
        [EmailAddress]
        [Required]
        public string Email {get;set;}

        [Required]
        [MinLength(8, ErrorMessage = "Your password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}