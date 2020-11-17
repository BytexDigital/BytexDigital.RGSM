using AutoMapper;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Shared.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Server.Common.MappingProfiles
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUserGroup, ApplicationUserGroupDto>().ReverseMap();
            CreateMap<Group, GroupDto>().ReverseMap();
        }
    }
}
