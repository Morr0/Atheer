using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.User.Models
{
    public class UserRemoveImage
    {
        [Required, MinLength(1)]
        public string UserId { get; set; }
    }
}