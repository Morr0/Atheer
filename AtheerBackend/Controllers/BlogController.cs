using AtheerBackend.DTO;
using AtheerCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AtheerBackend.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IMapper _mapper;

        public BlogController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            BlogPost post = new BlogPost { Title = "Hi" };
            return Ok(_mapper.Map<BlogPostReadDTO>(post));
        }
    }
}
