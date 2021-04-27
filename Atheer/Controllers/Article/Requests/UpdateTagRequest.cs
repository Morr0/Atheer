using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Article.Requests
{
    public class UpdateTagRequest : IValidatableObject
    {
        [Required] public string OriginalTitle { get; set; }
        [Required] public string NewTitle { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OriginalTitle == NewTitle) yield return new ValidationResult("You must provide a new title not the same old");
        }
    }
}