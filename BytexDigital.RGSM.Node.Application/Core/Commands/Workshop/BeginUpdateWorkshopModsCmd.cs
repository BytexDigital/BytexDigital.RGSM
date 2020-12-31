using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Workshop
{
    public class BeginUpdateWorkshopModsCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<BeginUpdateWorkshopModsCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(BeginUpdateWorkshopModsCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                var canUpdate = await workshopState.CanUpdateWorkshopModsAsync(cancellationToken);

                if (!canUpdate)
                    throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription($"The server cannot begin updating workshop mods. Reason: {canUpdate.FailureReason}");

                await workshopState.BeginUpdatingWorkshopModsAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
