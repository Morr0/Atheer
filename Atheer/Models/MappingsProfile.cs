using Atheer.Controllers.Dtos;
using AutoMapper;

namespace Atheer.Models
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateUserMappings();
        }

        private void CreateUserMappings()
        {
            CreateMap<RegisterViewModel, User>();
        }
    }
}