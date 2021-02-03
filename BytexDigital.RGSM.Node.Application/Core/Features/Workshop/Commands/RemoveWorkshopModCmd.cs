using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.Steam.Core.Structs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class RemoveWorkshopModCmd : IRequest
    {
        public string Id { get; set; }
        public PublishedFileId PublishedFileId { get; set; }

        public class Handler : IRequestHandler<RemoveWorkshopModCmd>
        {
            private readonly ServerService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(RemoveWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                await workshopState.RemoveWorkshopModAsync(request.PublishedFileId);

                return Unit.Value;
            }
        }
    }
}
