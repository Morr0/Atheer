using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atheer.Utilities.Config.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atheer.Controllers.ViewModels
{
    public class RegisterViewModel : IValidatableObject
    {
        public const string NamePattern = @"^.*[a-z ,.'-]+$";
        public const string NameErrorMessage = "The name must be alphabet with ( ,.'-) only";
        
        [Required, RegularExpression(NamePattern, ErrorMessage = NameErrorMessage), MinLength(2), MaxLength(32)] 
        public string FirstName { get; set; }
        [Required, RegularExpression(NamePattern, ErrorMessage = NameErrorMessage), MinLength(2), MaxLength(32)] 
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Bio { get; set; }
        [Required, EmailAddress, MaxLength(128)] 
        public string Email { get; set; }

        private const string PasswordError = "The password must be between 8 and 32 characters";
        [Required(ErrorMessage = PasswordError), MinLength(8, ErrorMessage = PasswordError), MaxLength(32, ErrorMessage = PasswordError)] 
        public string Password { get; set; }
        
        [BindProperty(Name = "g-recaptcha-response")]
        public string RecaptchaResponse { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            ValidateRecaptchaResponseIfAppropriate(validationContext, ref errors);
            
            return errors;
        }

        private void ValidateRecaptchaResponseIfAppropriate(ValidationContext validationContext, ref List<ValidationResult> errors)
        {
            var recaptchaConfig = validationContext.GetService<IOptions<Recaptcha>>();
            if (!recaptchaConfig.Value.Enabled) return;
            
            if (string.IsNullOrEmpty(RecaptchaResponse))
                errors.Add(new ValidationResult("You must verify with the reCaptcha"));
        }
    }
}