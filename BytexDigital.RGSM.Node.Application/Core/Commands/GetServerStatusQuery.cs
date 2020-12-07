using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Status;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{

    public class GetServerStatusQuery : IRequest<GetServerStatusQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetServerStatusQuery, Response>
        {
            private readonly ServersService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServersService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetServerStatusQuery request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServiceException().WithField(nameof(request.Id)).WithMessage("Server not found.");

                var state = _serverStateRegister.GetServerState(request.Id);

                if (state is not IRunnable runnableState)
                    throw new ServerNotRunnableException();

                return new Response
                {
                    Status = new ServerStatus
                    {
                        IsRunning = await runnableState.IsRunningAsync()
                    }
                };
            }
        }

        public class Response
        {
            public ServerStatus Status { get; set; }
        }
    }
}
