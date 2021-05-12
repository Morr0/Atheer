using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Authentication.Models
{
    public class LoginViewModel
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
        public int AttemptsLeft { get; set; }
    }
}