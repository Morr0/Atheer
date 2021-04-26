using Atheer.Services.ArticlesService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticlesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ArticlesResponse model)
        {
            return View("Default", model);
        }
    }
}