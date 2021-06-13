using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Atheer.Models
{
    public class ArticleSeries
    {
        [BsonIgnore] public int Id { get; set; }
        [BsonId, BsonElement("id")] public string SeriesId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Finished { get; set; }
        public string AuthorId { get; set; }
        [BsonIgnore] public IList<Article> Articles { get; set; }
        public List<string> ArticlesIds { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}