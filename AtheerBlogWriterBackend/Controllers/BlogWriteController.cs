using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtheerBlogWriterBackend.Exceptions;
using AtheerBlogWriterBackend.Services;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtheerBlogWriterBackend.Controllers
{
    [Route("api/blogwrite")]
    [ApiController]
    public class BlogWriteController : ControllerBase
    {
        private IBlogEditorService _editorService;
        private IMapper _mapper;

        public BlogWriteController(IBlogEditorService editorService, IMapper mapper)
        {
            _editorService = editorService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPost([FromBody] BlogPostWriteDTO writeDTO)
        {
            return Ok(await _editorService.AddPost(writeDTO));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateExistingPost([FromBody] BlogPostUpdateDTO updateDTO)
        {
            try
            {
                BlogPost post = await _editorService.UpdateExistingPost(updateDTO);
                return Ok(_mapper.Map<BlogPostReadDTO>(post));
            } catch (BlogPostNotFoundException e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
    }
}
