using System;

namespace AtheerBackend.Models
{
    public class Contact
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ContacterEmail { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        // Was it initiated from a form, article
        public string InitiatedFrom { get; set; }
        public string CreationDate { get; set; } = DateTime.UtcNow.ToString();
        
        public string IPAddressWhenContacted { get; set; }
        public string CountryWhenContacted { get; set; }
        
        public bool Answered { get; set; }
        
        // If `InitiatedFrom` == "Post"
        public int PostCreatedYear { get; set; }
        public string PostTitleShrinked { get; set; }
    }
}