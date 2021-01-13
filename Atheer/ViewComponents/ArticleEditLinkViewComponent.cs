using System.Threading.Tasks;
using Atheer.Services;
using Atheer.Services.BlogService;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.ViewComponents
{
    public class ArticleEditLinkViewComponent : ViewComponent
    {
        private readonly IBlogPostService _postService;

        public ArticleEditLinkViewComponent(IBlogPostService postService)
        {
            _postService = postService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(ArticleEditLinkModel model)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return Content(string.Empty);
            }

            var key = new BlogPostPrimaryKey(model.CreatedYear, model.TitleShrinked);
            if (!(await _postService.AuthorizedFor(key, model.UserId).ConfigureAwait(false)))
            {
                return Content(string.Empty);
            }
            
            return View("Default", model);
        }
    }
}