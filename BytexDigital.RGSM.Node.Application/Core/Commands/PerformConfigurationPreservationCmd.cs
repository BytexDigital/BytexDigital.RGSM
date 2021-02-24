using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class PerformConfigurationPreservationCmd : IRequest
    {
        public class Handler : IRequestHandler<PerformConfigurationPreservationCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(PerformConfigurationPreservationCmd request, CancellationToken cancellationToken)
            {
                await _serverStateRegister.PreserveConfigurationsAsync();

                return Unit.Value;
            }
        }
    }
}
