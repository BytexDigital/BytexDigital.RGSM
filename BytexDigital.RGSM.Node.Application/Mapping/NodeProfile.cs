
using AutoMapper;

using BytexDigital.RGSM.Node.Domain.Models.Services.NodeFileSystemService;
using BytexDigital.RGSM.Shared.TransferObjects.Models.Services.NodeFileSystemService;

namespace BytexDigital.RGSM.Node.Application.Mapping
{
    public class NodeProfile : Profile
    {
        public NodeProfile()
        {
            CreateMap<Directory, DirectoryDto>().ReverseMap();
            CreateMap<File, FileDto>().ReverseMap();
            CreateMap<DirectoryReference, DirectoryReferenceDto>();
        }
    }
}
