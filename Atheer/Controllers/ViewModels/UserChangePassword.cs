using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.ViewModels
{
    public class UserChangePassword
    {
        [Required, MinLength(8), MaxLength(32)] public string OldPassword { get; set; }
        [Required, MinLength(8), MaxLength(32)] public string NewPassword { get; set; }
        [Required, MinLength(8), MaxLength(32)] public string NewPasswordSecondTime { get; set; }
    }
}