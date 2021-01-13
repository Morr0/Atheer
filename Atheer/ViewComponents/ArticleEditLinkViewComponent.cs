using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ArticleEditLinkModel model)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return Content(string.Empty);
            }
            
            return View("Default", model);
        }
    }
}