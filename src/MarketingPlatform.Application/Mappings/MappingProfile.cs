using AutoMapper;
using MarketingPlatform.Application.DTOs.User;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UpdateUserDto, ApplicationUser>();
        }
    }
}
