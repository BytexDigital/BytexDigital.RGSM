using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Server.Application.Mappings
{
    public class MasterProfile : Profile
    {
        public MasterProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUserGroup, ApplicationUserGroupDto>().ReverseMap();
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<Domain.Entities.Node, NodeDto>().ReverseMap();
            CreateMap<Domain.Entities.NodeKey, NodeKeyDto>().ReverseMap();
        }
    }
}
