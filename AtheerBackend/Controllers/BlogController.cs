﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AtheerBackend.Controllers.Queries;
using AtheerBackend.Controllers.Results;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Services.BlogService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

#nullable enable

namespace AtheerBackend.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IMapper _mapper;
        private IBlogPostService _blogRepo;

        public BlogController(IMapper mapper, IBlogPostService blogRepo)
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

            return Ok(new BlogPostsResult
            {
                Posts = _mapper.Map<List<BlogPostReadDTO>>(repoResponse.Posts),
                X_AthBlog_Last_Year = repoResponse?.PaginationHeader?.X_AthBlog_Last_Year,
                X_AthBlog_Last_Title = repoResponse?.PaginationHeader?.X_AthBlog_Last_Title
            }.Posts);
        }

        [HttpGet("bare")]
        public async Task<IActionResult> GetAllBare()
        {
            return Ok(await _blogRepo.GetBare());
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

            IEnumerable<BlogPostReadDTO> postsReadDTO = _mapper.Map<List<BlogPostReadDTO>>(response.Posts);
            return Ok(new BlogPostsResult
            {
                Posts = postsReadDTO,
                X_AthBlog_Last_Year = response?.PaginationHeader?.X_AthBlog_Last_Year,
                X_AthBlog_Last_Title = response?.PaginationHeader?.X_AthBlog_Last_Title
            }.Posts);
        }

        [HttpGet("{year}/{title}")]
        public async Task<IActionResult> GetOne([FromRoute] int year, [FromRoute] string title)
        {
            BlogPostPrimaryKey key = new BlogPostPrimaryKey(year, title);
            BlogPost post = await _blogRepo.GetSpecific(key);
            if (post == null)
                return NotFound();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }
        
        #endregion

        #region Liking and Sharing
        
        [HttpPost("like/{year}/{title}")]
        public async Task<IActionResult> Like(int year, string title)
        {
            var key = new BlogPostPrimaryKey(year, title);
            BlogPost post = await _blogRepo.Like(key);
            if (post == null)
                return BadRequest();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }
        
        [HttpPost("share/{year}/{title}")]
        public async Task<IActionResult> Share(int year, string title)
        {
            var key = new BlogPostPrimaryKey(year, title);
            BlogPost post = await _blogRepo.Share(key);
            if (post == null)
                return BadRequest();

            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }

        #endregion

        #region Deleting and updating

        [HttpDelete("{year}/{titleShrinked}")]
        public async Task<IActionResult> Delete([FromRoute] int year, [FromRoute] string titleShrinked)
        {
            var key = new BlogPostPrimaryKey(year, titleShrinked);
            await _blogRepo.Delete(key);
            return Ok();
        }

        #endregion
    }
} 
