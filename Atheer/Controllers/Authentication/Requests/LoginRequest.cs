using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Authentication.Requests
{
    public class LoginRequest
    {
        [Required, MaxLength(32)] public string Username { get; set; }
        private const string PasswordError = "The password must be between 8 and 32 characters";
        [Required(ErrorMessage = PasswordError), MinLength(8, ErrorMessage = PasswordError), MaxLength(32)] 
        public string Password { get; set; }
    }
}