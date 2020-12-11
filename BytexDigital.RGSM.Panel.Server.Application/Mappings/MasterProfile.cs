using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Domain.Models;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Models;

namespace BytexDigital.RGSM.Panel.Server.Application.Mappings
{
    public class MasterProfile : Profile
    {
        public MasterProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUserGroup, ApplicationUserGroupDto>().ReverseMap();
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<Node, NodeDto>().ReverseMap();
            CreateMap<ApiKey, NodeKeyDto>().ReverseMap();
            CreateMap<ApiKeyDetails, ApiKeyDetailsDto>().ReverseMap();
        }
    }
}
