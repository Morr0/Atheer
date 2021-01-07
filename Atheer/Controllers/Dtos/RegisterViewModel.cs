using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Dtos
{
    public class RegisterViewModel
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Bio { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}