using System.Collections.Generic;
using Atheer.Models;

namespace Atheer.Controllers.ViewModels
{
    public class ArticlesViewModel
    {
        public IList<Article> Articles { get; set; }
        
        public bool SpecificYear { get; set; }
        public int Year { get; set; }
    }
}