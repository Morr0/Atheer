using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Admin.Models
{
    public class NavItemRemove
    {
        [Required, Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}