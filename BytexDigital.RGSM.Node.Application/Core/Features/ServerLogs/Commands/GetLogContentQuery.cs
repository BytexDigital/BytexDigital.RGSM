using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.ServerLogs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs.Commands
{
    public class GetLogContentQuery : IRequest<GetLogContentQuery.Response>
    {
        public string ServerId { get; set; }
        public string SourceName { get; set; }
        public int LimitLines { get; set; }

        public class Handler : IRequestHandler<GetLogContentQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetLogContentQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IServerLogs logState) throw new ServerDoesNotSupportFeatureException<IServerLogs>();

                var content = await logState.GetLogContentOrDefaultAsync(request.SourceName, request.LimitLines, cancellationToken);

                if (content == null) throw ServiceException.ServiceError("Source was not found or the associated log has been deleted.").WithField(nameof(request.SourceName));

                return new Response
                {
                    Content = content
                };
            }
        }

        public class Response
        {
            public LogContent Content { get; set; }
        }
    }
}
