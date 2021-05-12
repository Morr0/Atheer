using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Authentication.Requests
{
    public class LoginRequest
    {
        [Required, DisplayName("Email/Username"), MaxLength(128)]
        public string EmailOrUsername { get; set; }
        private const string PasswordError = "The password must be between 8 and 32 characters";
        [Required(ErrorMessage = PasswordError), MinLength(8, ErrorMessage = PasswordError), MaxLength(32)] 
        public string Password { get; set; }
    }
}