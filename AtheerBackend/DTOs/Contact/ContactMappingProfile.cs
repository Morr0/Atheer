using AutoMapper;
using AtheerBackend.Models;

namespace AtheerBackend.DTOs.Contact
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<ContactWriteDTO, Models.Contact>();
        }
    }
}