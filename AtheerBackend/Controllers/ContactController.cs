using System.Threading.Tasks;
using AtheerBackend.DTOs.Contact;
using AtheerBackend.Services.ContactService;
using AtheerBackend.Services.ContactService.Exceptions;
using AtheerCore.Models.Contact;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AtheerBackend.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : Controller
    {
        private IMapper _mapper;
        private IContactService _contactService;

        public ContactController(IMapper mapper, IContactService contactService)
        {
            _mapper = mapper;
            _contactService = contactService;
        }
        
        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody] ContactWriteDTO writeDto)
        {
            try
            {
                Contact contact = _mapper.Map<Contact>(writeDto);
                contact.IPAddressWhenContacted = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                await _contactService.Contact(contact);

                return Ok();
            }
            catch (BlogPostDoesNotPermitContactException)
            {
                return BadRequest();
            }
        }
    }
}