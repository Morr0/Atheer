using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class LocalDateViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string model)
        {
            return View("Default", model);
        }
    }
}