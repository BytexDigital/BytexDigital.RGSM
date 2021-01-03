using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.ServerLogs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs.Commands
{
    public class GetPrimaryLogSourceQuery : IRequest<GetPrimaryLogSourceQuery.Response>
    {
        public string ServerId { get; set; }

        public class Handler : IRequestHandler<GetPrimaryLogSourceQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetPrimaryLogSourceQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IServerLogs logState) throw new ServerDoesNotSupportFeatureException<IServerLogs>();

                var primarySource = await logState.GetPrimaryLogSourceOrDefaultAsync(cancellationToken);

                if (primarySource == default) throw ServiceException.ServiceError("No primary log source found.").WithField(nameof(request.ServerId));

                return new Response
                {
                    Source = primarySource
                };
            }
        }

        public class Response
        {
            public LogSource Source { get; set; }
        }
    }
}
