using System.Collections.Generic;

namespace AtheerBackend.DTOs
{
    /// <summary>
    /// This is a stripped down version of BlogPost for list admin view
    /// </summary>
    public class BareBlogPostReadDTO
    {
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }
        public string Title { get; set; }
        public List<string> Topics { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool Scheduled { get; set; }
        public string CreationDate { get; set; }
    }
}