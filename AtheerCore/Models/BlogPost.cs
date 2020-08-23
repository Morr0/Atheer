using System;

namespace AtheerCore.Models
{
    public class BlogPost
    {
        // Partition key
        public int CreatedYear { get; set; } = DateTime.UtcNow.Year;

        // Range key, e.g. this-is-a-title <- replace spaces with - and lower case everything
        public string TitleShrinked { get; set; }

        // Although is not a key, use it for internal stuff
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        // This and update date, are seperate from the key above. These are for info only.
        public string CreationDate { get; set; }

        public string LastUpdatedDate { get; set; }

        public string Topic { get; set; }
    }
}
