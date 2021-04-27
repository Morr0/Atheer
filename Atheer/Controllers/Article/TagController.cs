using System.Threading.Tasks;
using Atheer.Controllers.Article.Requests;
using Atheer.Extensions;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atheer.Controllers.Article
{
    [Route("Tag")]
    [Authorize(Roles = UserRoles.AdminRole)]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        
        [HttpGet("Update/{tagId}")]
        public async Task<IActionResult> UpdateTagView([FromRoute] string tagId)
        {
            var tag = await _tagService.Get(tagId).CAF();
            if (tag is null) return NotFound();

            return View("UpdateTag", tag.Title);
        }

        [HttpPost("Update/{tagId}")]
        public async Task<IActionResult> UpdateTagPost([FromRoute] string tagId, [FromForm] UpdateTagRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateTag", request.OriginalTitle);
            }
            
            await _tagService.Update(tagId, request.NewTitle).CAF();

            return Redirect("/");
        }
        
    }
}