using AtheerBackend.Controllers.Headers;
using AtheerBackend.Controllers.Queries;
using AtheerBackend.Extensions;
using AtheerBackend.Services;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

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

    #region Reading
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BlogsQuery query, 
            [FromHeader(Name = nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Year))] string? hyear,
            [FromHeader(Name = nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Title))] string? htitle)
        {
            // Gets created from headers
            PostsPaginationPrimaryKey paginationHeader = new PostsPaginationPrimaryKey
            {
                X_AthBlog_Last_Year = hyear,
                X_AthBlog_Last_Title = htitle
            };
            var repoResponse = await _blogRepo.Get(query.Size, paginationHeader);
            
            // Insert into headers the pagination stuff
            if (repoResponse.PaginationHeader != null && !repoResponse.PaginationHeader.Empty())
            {
                repoResponse.PaginationHeader.AddHeaders(Response.Headers);
            }

            return Ok(_mapper.Map<List<BlogPostReadDTO>>(repoResponse.Posts));
        }

        [HttpGet("{year}")]
        public async Task<IActionResult> GetManyByYear([FromRoute] int year,
            [FromQuery] BlogsQuery query,
            [FromHeader(Name = nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Year))] string? hyear,
            [FromHeader(Name = nameof(PostsPaginationPrimaryKey.X_AthBlog_Last_Title))] string? htitle)
        {
            // Gets created from headers
            PostsPaginationPrimaryKey paginationHeader = new PostsPaginationPrimaryKey
            {
                X_AthBlog_Last_Year = hyear,
                X_AthBlog_Last_Title = htitle
            };
            BlogRepositoryBlogResponse response = await _blogRepo.GetByYear(year, query.Size, paginationHeader);
            if (response.Posts.Count == 0)
                return NotFound();

            // Insert into headers the pagination stuff
            if (response.PaginationHeader != null && !response.PaginationHeader.Empty())
            {
                response.PaginationHeader.AddHeaders(Response.Headers);
            }

            IEnumerable<BlogPostReadDTO> postsReadDTO = _mapper.Map<List<BlogPostReadDTO>>(response.Posts);
            return Ok(postsReadDTO);
        }

        [HttpGet("{year}/{title}")]
        public async Task<IActionResult> GetOne([FromRoute] int year, [FromRoute] string title)
        {
            BlogPost post = await _blogRepo.Get(year, title);
            if (post == null)
                return NotFound();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }
        #endregion

        #region Liking and Sharing
        
        [HttpPost("like/{year}/{title}")]
        public async Task<IActionResult> Like(int year, string title)
        {
            BlogPost post = await _blogRepo.Like(year, title);
            if (post == null)
                return BadRequest();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }
        
        [HttpPost("share/{year}/{title}")]
        public async Task<IActionResult> Share(int year, string title)
        {
            BlogPost post = await _blogRepo.Share(year, title);
            if (post == null)
                return BadRequest();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }

        #endregion
    }
} 
