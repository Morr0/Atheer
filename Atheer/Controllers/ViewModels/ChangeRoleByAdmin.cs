using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atheer.Services.UsersService;

namespace Atheer.Controllers.ViewModels
{
    public class ChangeRoleByAdmin : IValidatableObject
    {
        [Required, MinLength(1)]
        public string UserId { get; set; }
        [Required]
        public string NewRole { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (string.IsNullOrEmpty(NewRole) || (NewRole != UserRoles.BasicRole && NewRole != UserRoles.EditorRole))
                results.Add(new ValidationResult(""));

            return results;
        }
    }
}