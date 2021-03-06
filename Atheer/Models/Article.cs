﻿using System;
using System.Collections.Generic;
using Atheer.Repositories.Junctions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NpgsqlTypes;

namespace Atheer.Models
{
    // WHENEVER YOU UPDATE HERE, UPDATE THE MAPPER AS WELL AS THE MAPPING METHODS
    public class Article
    {
        [BsonId] public string Id { get; set; }
        [BsonIgnore] public int CreatedYear { get; set; }
        [BsonIgnore] public string TitleShrinked { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool Likeable { get; set; }
        public int Likes { get; set; }
        public bool Shareable { get; set; }
        public int Shares { get; set; }
        public bool Draft { get; set; }
        public bool Unlisted { get; set; }
        public bool ForceFullyUnlisted { get; set; }
        [JsonIgnore, BsonIgnore]
        public IList<TagArticle> Tags { get; set; }

        public List<string> TagsIds { get; set; }
        
        // Postgresql specific
        [JsonIgnore, BsonIgnore]
        public NpgsqlTsVector SearchVector { get; set; }

        public int? SeriesId { get; set; }
        [BsonIgnore] public ArticleSeries Series { get; set; }
        public bool Narratable { get; set; }
        public string NarrationMp3Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int Version { get; set; }
        public bool EverPublished { get; set; }

        public string GetId() => $"{CreatedYear.ToString()}-{TitleShrinked}";
    }
}
