using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.Steam.Core.Structs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class AddWorkshopModCmd : IRequest
    {
        public string Id { get; set; }
        public PublishedFileId PublishedFileId { get; set; }

        public class Handler : IRequestHandler<AddWorkshopModCmd>
        {
            private readonly ServersService _serversService;
            private readonly WorkshopManagerService _workshopService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServersService serversService, WorkshopManagerService workshopService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _workshopService = workshopService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(AddWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                if (server.TrackedWorkshopMods.Any(x => x.PublishedFileId == request.PublishedFileId))
                    throw new ServiceException().AddServiceError().WithField(nameof(request.PublishedFileId)).WithDescription("Workshop item has already been added.");

                await _workshopService.AddTrackedWorkshopItemAsync(server, request.PublishedFileId);

                return Unit.Value;
            }
        }
    }
}
