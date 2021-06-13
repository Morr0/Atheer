using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Atheer.Repositories.Junctions;
using MongoDB.Bson.Serialization.Attributes;

namespace Atheer.Models
{
    public class Tag
    {
        [BsonId] public string Id { get; set; }
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        
        [JsonIgnore, BsonIgnore] 
        public IList<TagArticle> Tags { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}