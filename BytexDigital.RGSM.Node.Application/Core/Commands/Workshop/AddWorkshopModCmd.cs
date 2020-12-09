using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.Steam.Core.Structs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Workshop
{
    public class AddWorkshopModCmd : IRequest
    {
        public string Id { get; set; }
        public PublishedFileId PublishedFileId { get; set; }

        public class Handler : IRequestHandler<AddWorkshopModCmd>
        {
            private readonly ServersService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServersService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(AddWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                if (server.TrackedWorkshopMods.Any(x => x.PublishedFileId == request.PublishedFileId))
                    throw new ServiceException().WithField(nameof(request.PublishedFileId)).WithMessage("Workshop item has already been added.");

                await _serversService.AddTrackedWorkshopItemAsync(server, request.PublishedFileId);

                return Unit.Value;
            }
        }
    }
}
