using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class UserSettingsUpdate
    {
        [Required, MinLength(1), MaxLength(32)] 
        public string FirstName { get; set; }
        [Required, MinLength(1), MaxLength(32)] 
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Bio { get; set; }
    }
}