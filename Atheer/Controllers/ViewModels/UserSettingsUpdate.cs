using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class UserSettingsUpdate
    {
        [Required, MinLength(2), MaxLength(32), RegularExpression(RegisterViewModel.NamePattern, ErrorMessage = RegisterViewModel.NameErrorMessage)] 
        public string FirstName { get; set; }
        [Required, MinLength(2), MaxLength(32), RegularExpression(RegisterViewModel.NamePattern, ErrorMessage = RegisterViewModel.NameErrorMessage)] 
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Bio { get; set; }

        public string DateCreated { get; set; }
    }
}