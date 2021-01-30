using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Atheer.Models
{
    public class Tag
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string DateCreated { get; set; }
        /// <summary>
        /// When is the last time this tag had an article linked to
        /// </summary>
        public string DateLastAddedTo { get; set; }
        
        [JsonIgnore] 
        public IList<TagArticle> Tags { get; set; }
    }
}