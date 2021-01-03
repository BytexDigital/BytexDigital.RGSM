using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.ServerLogs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs.Commands
{
    public class GetLogSourcesQuery : IRequest<GetLogSourcesQuery.Response>
    {
        public string ServerId { get; set; }

        public class Handler : IRequestHandler<GetLogSourcesQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetLogSourcesQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IServerLogs logState) throw new ServerDoesNotSupportFeatureException<IServerLogs>();

                var sources = await logState.GetLogSourcesAsync(cancellationToken);

                return new Response
                {
                    Sources = sources
                };
            }
        }

        public class Response
        {
            public List<LogSource> Sources { get; set; }
        }
    }
}
