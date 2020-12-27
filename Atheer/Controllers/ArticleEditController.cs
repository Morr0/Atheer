﻿using System.Threading.Tasks;
using Atheer.Controllers.Queries;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Services.BlogService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atheer.Controllers
{
    // Used for both editing/adding new posts
    [Route("Article/Edit")]
    public class ArticleEditController : Controller
    {
        private ILogger<ArticleEditController> _logger;
        private IBlogPostService _service;

        public ArticleEditController(ILogger<ArticleEditController> logger, IBlogPostService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] BlogPostEditQuery editQuery)
        {
            BlogPost post = null;
            if (IsNewPost(editQuery.TitleShrinked))
            {
                post = new BlogPost();
            }
            else
            {
                post =
                    await _service.GetSpecific(new BlogPostPrimaryKey(editQuery.CreatedYear, editQuery.TitleShrinked))
                        .ConfigureAwait(false);
                if (post is null) return Redirect("/");
            }

            return View("ArticleEdit", post);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromForm] BlogPost post)
        {
            BlogPost checkedOutPost = null;
            if (IsNewPost(post.TitleShrinked))
            {
                checkedOutPost = await _service.AddPost(post).ConfigureAwait(false);
                return RedirectToAction("Index", "Article", post);
            }
            else
            {
                checkedOutPost = await _service.UpdatePost(post).ConfigureAwait(false);
                // Refresh as GET
                return RedirectToAction("Index");
            }
        }

        private bool IsNewPost(string titleShrinked)
        {
            return string.IsNullOrEmpty(titleShrinked);
        }
    }
}