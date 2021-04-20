using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.User.Models
{
    public class UserChangePassword : IValidatableObject
    {
        [Required, MinLength(8), MaxLength(32)] public string OldPassword { get; set; }
        [Required, MinLength(8), MaxLength(32)] public string NewPassword { get; set; }
        [Required, MinLength(8), MaxLength(32)] public string NewPasswordSecondTime { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewPassword != NewPasswordSecondTime)
            {
                yield return new ValidationResult("One of the 2 new password fields don't match");
            }
        }
    }
}