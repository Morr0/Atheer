using System.Threading.Tasks;
using Atheer.Controllers.Article.Requests;
using Atheer.Extensions;
using Atheer.Services.TagService;
using Atheer.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers.Article
{
    [Route("Tag")]
    [Authorize(Roles = UserRoles.AdminRole)]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }
        
        [HttpGet("Update/{tagId}")]
        [Authorize(UserRoles.AdminRole)]
        public async Task<IActionResult> UpdateTagView([FromRoute] string tagId)
        {
            var tag = await _tagService.Get(tagId).CAF();
            if (tag is null) return NotFound();

            return View("UpdateTag", tag.Title);
        }

        [HttpPost("Update/{tagId}")]
        [Authorize(UserRoles.AdminRole)]
        public async Task<IActionResult> UpdateTagPost([FromRoute] string tagId, [FromForm] UpdateTagRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateTag", request.OriginalTitle);
            }

            string userId = this.GetViewerUserId();
            await _tagService.Update(tagId, request.NewTitle).CAF();
            
            _logger.LogInformation("User: {UserId} Updated tag with id: {TagId} from original title {OriginalTitle} to {NewTitle}", 
                userId, tagId, request.OriginalTitle, request.NewTitle);

            return Redirect("/");
        }
        
    }
}