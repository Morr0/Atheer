using System;
using System.Collections.Generic;
using Atheer.Repositories.Junctions;
using Newtonsoft.Json;
using NpgsqlTypes;

namespace Atheer.Models
{
    // WHENEVER YOU UPDATE HERE, UPDATE THE MAPPER AS WELL AS THE MAPPING METHODS
    public class Article
    {
        public int CreatedYear { get; set; }
        public string TitleShrinked { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }
        
        public bool Likeable { get; set; }

        public int Likes { get; set; }

        public bool Shareable { get; set; }
        
        public int Shares { get; set; }

        // The article is not done
        public bool Draft { get; set; }

        // Not to be listed however can be accessed
        public bool Unlisted { get; set; }

        public bool Scheduled { get; set; }

        [JsonIgnore]
        public IList<TagArticle> Tags { get; set; }
        
        // Postgresql specific
        [JsonIgnore]
        public NpgsqlTsVector SearchVector { get; set; }

        public int? SeriesId { get; set; }
        public ArticleSeries Series { get; set; }

        public bool Narratable { get; set; }
        public string NarrationMp3Url { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
