﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Atheer.Repositories.Junctions;

namespace Atheer.Models
{
    public class Tag
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        
        [JsonIgnore] 
        public IList<TagArticle> Tags { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}