using AutoMapper;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;
using BytexDigital.RGSM.Node.Domain.Models.Console;
using BytexDigital.RGSM.Node.Domain.Models.FileSystem;
using BytexDigital.RGSM.Node.Domain.Models.Status;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Node.TransferObjects.Models.BattlEye;
using BytexDigital.RGSM.Node.TransferObjects.Models.Console;
using BytexDigital.RGSM.Node.TransferObjects.Models.FileSystem;
using BytexDigital.RGSM.Node.TransferObjects.Models.Status;
using BytexDigital.RGSM.Node.TransferObjects.Models.Workshop;

namespace BytexDigital.RGSM.Node.Application.Mappings
{
    public class NodeProfile : Profile
    {
        public NodeProfile()
        {
            CreateMap<Server, ServerDto>().ReverseMap();
            //CreateMap<Arma3Server, Arma3ServerDto>().ReverseMap();
            CreateMap<ServerStatus, ServerStatusDto>().ReverseMap();
            CreateMap<ServerInstallationStatus, ServerInstallationStatusDto>().ReverseMap();

            CreateMap<DirectoryContentDetails, DirectoryContentDetailsDto>().ReverseMap();
            CreateMap<FileDetails, FileDetailsDto>().ReverseMap();

            CreateMap<ConsoleOutputContent, ConsoleOutputContentDto>().ReverseMap();

            CreateMap<BeRconPlayer, BeRconPlayerDto>().ReverseMap();
            CreateMap<BeRconMessage, BeRconMessageDto>().ReverseMap();
            CreateMap<BeRconStatus, BeRconStatusDto>().ReverseMap();

            CreateMap<WorkshopItem, WorkshopItemDto>().ReverseMap();
        }
    }
}
