using AtheerBackend.Controllers.Headers;
using AtheerBackend.Controllers.Queries;
using AtheerBackend.DTO;
using AtheerBackend.Extensions;
using AtheerBackend.Services;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtheerBackend.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IMapper _mapper;
        private IBlogRepository _blogRepo;

        public BlogController(IMapper mapper, IBlogRepository blogRepo)
        {
            _mapper = mapper;
            _blogRepo = blogRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BlogsQuery query, 
            [FromHeader(Name = nameof(PostsPaginationHeader.X_AthBlog_Last_Year))] string? hyear,
            [FromHeader(Name = nameof(PostsPaginationHeader.X_AthBlog_Last_Title))] string? htitle)
        {
            PostsPaginationHeader paginationHeader = new PostsPaginationHeader
            {
                X_AthBlog_Last_Year = hyear,
                X_AthBlog_Last_Title = htitle
            };

            var repoResponse = await _blogRepo.Get(query.Size, paginationHeader);
            
            // Insert into headers the pagination stuff
            if (repoResponse.PaginationHeader != null && !repoResponse.PaginationHeader.Empty())
            {
                Console.WriteLine("Not null");
                repoResponse.PaginationHeader.AddHeaders(Response.Headers);
            }

            return Ok(_mapper.Map<List<BlogPostReadDTO>>(repoResponse.Posts));
        }
    }
}
