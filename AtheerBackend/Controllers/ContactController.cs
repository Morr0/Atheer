using System.Threading.Tasks;
using AtheerBackend.DTOs.Contact;
using AtheerBackend.Services.ContactService;
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
            Contact contact = _mapper.Map<Contact>(writeDto);
            await _contactService.Contact(contact);

            return Ok();
        }
    }
}