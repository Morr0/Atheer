using System.Threading.Tasks;
using Atheer.Services.ArticlesService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticlesViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ArticlesResponse model)
        {
            return View("Default", model);
        }
    }
}