using Atheer.Models;
using Atheer.Services.ArticlesService;

namespace Atheer.Controllers.ViewModels
{
    public class UserPageViewModel
    {
        public User User { get; set; }
        public ArticlesResponse Articles { get; set; }
    }
}