using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.ViewModels
{
    public class ArticlesViewModel
    {
        public IList<BlogPost> Posts { get; set; }
        
        public bool SpecificYear { get; set; }
        public int Year { get; set; }
    }
}