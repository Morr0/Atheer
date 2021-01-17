using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class LoginViewModel
    {
        [Required] 
        [DisplayName("Email/Username")]
        public string EmailOrUsername { get; set; }
        [Required] public string Password { get; set; }
    }
}