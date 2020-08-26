using AtheerBackend.DTO;
using AtheerBackend.Services;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get()
        {
            List<BlogPost> posts = await _blogRepo.Get(10);
            return Ok(_mapper.Map<List<BlogPostReadDTO>>(posts));
        }
    }
}
