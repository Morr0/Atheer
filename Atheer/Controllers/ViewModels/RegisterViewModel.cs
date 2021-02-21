using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class RegisterViewModel
    {
        public const string NamePattern = @"^.*[a-z ,.'-]+$";
        public const string NameErrorMessage = "The name must be alphabet with ( ,.'-) only";
        
        [Required, RegularExpression(NamePattern, ErrorMessage = NameErrorMessage), MinLength(2), MaxLength(32)] 
        public string FirstName { get; set; }
        [Required, RegularExpression(NamePattern, ErrorMessage = NameErrorMessage), MinLength(2), MaxLength(32)] 
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Bio { get; set; }
        [Required, EmailAddress, MaxLength(128)] 
        public string Email { get; set; }

        private const string PasswordError = "The password must be between 8 and 32 characters";
        [Required(ErrorMessage = PasswordError), MinLength(8, ErrorMessage = PasswordError), MaxLength(32, ErrorMessage = PasswordError)] 
        public string Password { get; set; }
    }
}