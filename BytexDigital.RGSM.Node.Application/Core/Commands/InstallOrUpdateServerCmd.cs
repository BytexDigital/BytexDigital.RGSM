using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{

    public class InstallOrUpdateServerCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<InstallOrUpdateServerCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(InstallOrUpdateServerCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();

                if (state is not IRunnable runnableState) throw new ServerDoesNotSupportFeatureException<IRunnable>();

                var canUpdate = await runnableState.CanInstallOrUpdateAsync();

                if (!canUpdate)
                    throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription($"The server cannot install or update at the moment. Reason: {canUpdate.FailureReason}");

                await runnableState.BeginInstallationOrUpdateAsync();

                return Unit.Value;
            }
        }
    }
}
