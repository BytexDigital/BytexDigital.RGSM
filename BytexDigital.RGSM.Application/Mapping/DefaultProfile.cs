using AutoMapper;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Domain.Enumerations;
using BytexDigital.RGSM.Shared.TransferObjects.Entities;
using BytexDigital.RGSM.Shared.TransferObjects.Enumerations;

namespace BytexDigital.RGSM.Application.Mapping
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUserGroup, ApplicationUserGroupDto>().ReverseMap();
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<Server, ServerDto>().ReverseMap();
            CreateMap<ServerStatus, ServerStatusDto>().ReverseMap();
            CreateMap<ServerType, ServerTypeDto>().ReverseMap();
        }
    }
}
