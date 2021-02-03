using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class EnableOrDisableWorkshopModCmd : IRequest
    {
        public string ServerId { get; set; }
        public ulong PublishedFileId { get; set; }
        public bool Enabled { get; set; }

        public class Handler : IRequestHandler<EnableOrDisableWorkshopModCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;
            private readonly ServerService _serversService;

            public Handler(ServerStateRegister serverStateRegister, ServerService serversService)
            {
                _serverStateRegister = serverStateRegister;
                _serversService = serversService;
            }

            public async Task<Unit> Handle(EnableOrDisableWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                await workshopState.EnableOrDisableWorkshopModAsync(request.PublishedFileId, request.Enabled, cancellationToken);

                return Unit.Value;
            }
        }
    }
}
