using System.Collections.Generic;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using Atheer.Services.ArticlesService;

namespace Atheer.Services.ArticlesService
{
    // A wrapper on what will the service return
    // This is a cleaner choice to tuples
    public class ArticlesResponse
    {
        public int CurrentPage { get; set; }
        public IList<StrippedArticleViewModel> Articles { get; set; }
        public bool AnyNext { get; set; }
        public bool AnyPrevious { get; set; }
        public string TagId { get; set; }
        public string TagTitle { get; set; }
        public int Year { get; set; }
        public bool SpecificYear => Year != 0;
        public string UserId { get; set; }
    }
}
