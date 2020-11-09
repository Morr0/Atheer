using AutoMapper;

namespace AtheerBackend.DTOs.Contact
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<ContactWriteDTO, AtheerCore.Models.Contact.Contact>();
        }
    }
}