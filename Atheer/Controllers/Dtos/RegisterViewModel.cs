using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Dtos
{
    public class RegisterViewModel
    {
        [Required] public string Name { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}