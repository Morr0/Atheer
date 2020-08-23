using AtheerCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AtheerBackend.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            BlogPost post = new BlogPost { Title = "Hi" };
            return Ok(post);
        }
    }
}
