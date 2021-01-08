using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ArticleEditLinkModel model)
        {
            return View("Default", model);
        }
    }
}