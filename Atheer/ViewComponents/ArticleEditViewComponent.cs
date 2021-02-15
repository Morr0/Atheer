using System.Threading.Tasks;
using Atheer.Services;
using Atheer.Services.ArticlesService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticleEditViewComponent : ViewComponent
    {
        private readonly IArticleService _articleService;

        public ArticleEditViewComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(ArticleEditLinkModel model)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return Content(string.Empty);
            }

            var key = new ArticlePrimaryKey(model.CreatedYear, model.TitleShrinked);
            if (!(await _articleService.AuthorizedFor(key, model.UserId).ConfigureAwait(false)))
            {
                if (!User.IsInRole(UserRoles.AdminRole)) return Content(string.Empty);
            }
            
            return View("Default", model);
        }
    }
}