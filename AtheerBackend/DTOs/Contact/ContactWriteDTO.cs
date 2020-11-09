using System.ComponentModel.DataAnnotations;

namespace AtheerBackend.DTOs.Contact
{
    public class ContactWriteDTO
    {
        [Required]
        [MinLength(5)]
        public string ContacterEmail { get; set; }
        [Required]
        [MinLength(10)]
        public string Title { get; set; }
        [Required]
        [MinLength(20)]
        public string Content { get; set; }
        // Was it initiated from a form, article
        [Required]
        public string InitiatedFrom { get; set; }
    }
}