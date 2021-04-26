using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class LocalDateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string model)
        {
            return View("Default", model);
        }
    }
}