using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Atheer.Controllers.ViewModels
{
    public class UserChangeImage
    {
        [Required, MinLength(1)]
        public string UserId { get; set; }
        [Required] public IFormFile File { get; set; }
    }
}