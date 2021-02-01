﻿using System.ComponentModel.DataAnnotations;

namespace Atheer.Controllers.Queries
{
    public class ArticlesQuery
    {
        [Range(1, int.MaxValue)]
        public int Year { get; set; }
    }
}