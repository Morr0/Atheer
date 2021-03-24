using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Admin.Models
{
    public class NavItemAdd
    {
        [Required, MinLength(1), MaxLength(32)]
        public string Name { get; set; }
        [Required, Url]
        public string Url { get; set; }
    }
}