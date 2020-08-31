using System;

namespace AtheerCore.Models
{
    // WHENEVER YOU UPDATE HERE, UPDATE THE MAPPER AS WELL AS THE MAPPING METHODS
    public class BlogPost
    {
        // Partition key
        public int CreatedYear { get; set; } = DateTime.UtcNow.Year;

        // Range key, e.g. this-is-a-title <- replace spaces with - and lower case everything
        public string TitleShrinked { get; set; }

        // Although is not a key, use it for internal stuff
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        // This and update date, are seperate from the key above. These are for info only.
        public string CreationDate { get; set; }

        public string LastUpdatedDate { get; set; }

        public string Topic { get; set; }

        public int Likes { get; set; }

        // The post is not done
        public bool Draft { get; set; }

        // Not to be listed however can be accessed
        public bool Unlisted { get; set; }
    }
}
