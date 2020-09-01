using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using AtheerBlogWriterBackend.Exceptions;
using AtheerBlogWriterBackend.Services;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThirdParty.Json.LitJson;

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
            // After successful model validation of body, 
            // check that more than 2 elements have been passed in JSON
            Request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(Request.Body))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                string rawJson = await reader.ReadToEndAsync();
                JsonDocument doc = JsonDocument.Parse(rawJson);
                if (doc.RootElement.EnumerateObject().Count() == 2)
                    return BadRequest(new {});
            }

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

        [HttpDelete("{year}/{titleShrinked}")]
        public async Task<IActionResult> DeleteExistingPost([FromRoute] int year, [FromRoute] string titleShrinked)
        {
            bool result = await _editorService.DeleteExistingPost(year, titleShrinked);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
