using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string TagsAsString(IEnumerable<Tag> tags)
        {
            var sb = new StringBuilder(tags.Count());

            foreach (var tag in tags)
            {
                sb.Append($"{tag.Title} ");
            }

            return sb.ToString().TrimEnd();
        }
    }
}