using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtheerBlogWriterBackend.DTOs;
using AtheerBlogWriterBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtheerBlogWriterBackend.Controllers
{
    [Route("api/blogwrite")]
    [ApiController]
    public class BlogWriteController : ControllerBase
    {
        private IBlogEditorService _editorService;

        public BlogWriteController(IBlogEditorService editorService)
        {
            _editorService = editorService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPost([FromBody] BlogPostWriteDTO writeDTO)
        {
            return Ok(await _editorService.AddPost(writeDTO));
        }
    }
}
