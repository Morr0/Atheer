using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AtheerBackend.Controllers.Queries
{
    public class BlogsQuery
    {
        // Size of page
        private const int MAX_PAGE_SIZE = 10;
        private const int DEFAULT_PAGE_SIZE = MAX_PAGE_SIZE;

        [NotNull]
        [Range(1, MAX_PAGE_SIZE)]
        public int Size { get; set; } = DEFAULT_PAGE_SIZE;
    }
}
