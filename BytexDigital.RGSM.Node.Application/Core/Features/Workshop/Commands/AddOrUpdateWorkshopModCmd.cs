using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class AddOrUpdateWorkshopModCmd : IRequest
    {
        public string Id { get; set; }
        public PublishedFileId PublishedFileId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public class Handler : IRequestHandler<AddOrUpdateWorkshopModCmd>
        {
            private readonly ServerService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(AddOrUpdateWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                await workshopState.AddOrUpdateWorkshopModAsync(request.PublishedFileId, request.Metadata);

                return Unit.Value;
            }
        }
    }
}
