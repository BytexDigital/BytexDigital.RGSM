using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class PerformShutdownCmd : IRequest
    {
        public class Handler : IRequestHandler<PerformShutdownCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(PerformShutdownCmd request, CancellationToken cancellationToken)
            {
                await _serverStateRegister.ShutdownAsync();

                return Unit.Value;
            }
        }
    }
}
