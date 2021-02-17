using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class UserSettingsViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string DateCreated { get; set; }
    }
}