using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Workshop
{
    public class UpdateWorkshopModLoadStatusCmd : IRequest
    {
        public string ServerId { get; set; }
        public ulong PublishedFileId { get; set; }
        public bool Load { get; set; }

        public class Handler : IRequestHandler<UpdateWorkshopModLoadStatusCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;
            private readonly ServersService _serversService;
            private readonly WorkshopManagerService _workshopService;

            public Handler(ServerStateRegister serverStateRegister, ServersService serversService, WorkshopManagerService workshopService)
            {
                _serverStateRegister = serverStateRegister;
                _serversService = serversService;
                _workshopService = workshopService;
            }

            public async Task<Unit> Handle(UpdateWorkshopModLoadStatusCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();
                if (!(state is IWorkshopSupport workshopState)) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                var server = await _serversService.GetServer(state.Id).FirstAsync();
                var trackedMod = await _workshopService.GetTrackedWorkshopMods(server).FirstOrDefaultAsync(x => x.PublishedFileId == request.PublishedFileId);

                if (trackedMod == null) throw ServiceException.ServiceError("Mod is not being tracked.").WithField(nameof(request.PublishedFileId));

                await _workshopService.UpdateTrackedWorkshopItemAsync(trackedMod, request.Load);

                return Unit.Value;
            }
        }
    }
}
