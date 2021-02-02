using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class RegisterViewModel
    {
        [Required, RegularExpression("/^[A-Z]+$/i", ErrorMessage = "First Name must be alphabet only"), MinLength(2), MaxLength(32)] 
        public string FirstName { get; set; }
        [Required, RegularExpression("/^[A-Z]+$/i", ErrorMessage = "Last Name must be alphabet only"), MinLength(2), MaxLength(32)] 
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