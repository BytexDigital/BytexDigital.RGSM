using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Features.Installable;
using BytexDigital.RGSM.Node.Application.Core.Features.Runnable;
using BytexDigital.RGSM.Node.Application.Core.Features.Workshop;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Servers.Commands
{
    public class DeleteServerCmd : IRequest
    {
        public string ServerId { get; set; }
        public bool DeleteAllFiles { get; set; }

        public class Handler : IRequestHandler<DeleteServerCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;
            private readonly ServerService _serversService;

            public Handler(ServerStateRegister serverStateRegister, ServerService serversService)
            {
                _serverStateRegister = serverStateRegister;
                _serversService = serversService;
            }

            public async Task<Unit> Handle(DeleteServerCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);

                if (state == null) throw new ServerNotFoundException();

                var server = await _serversService.GetServer(state.Id).FirstOrDefaultAsync();

                if (state is IRunnable runnable)
                {
                    if (await runnable.IsRunningAsync(cancellationToken)) throw ServiceException.ServiceError("Server cannot be deleted because it is running.");
                }

                if (state is IWorkshopSupport workshopSupport)
                {
                    if ((await workshopSupport.GetWorkshopModStatesAsync(cancellationToken)).Any(x => x.IsUpdating))
                    {
                        throw ServiceException.ServiceError("Server cannot be deleted because it is updating workshop mods.");
                    }
                }

                if (state is IInstallAndUpdatable updatable)
                {
                    if ((await updatable.GetInstallationStatusAsync(cancellationToken)).IsUpdating)
                    {
                        throw ServiceException.ServiceError("Server cannot be deleted because it is updating.");
                 
                    }
                }

                await state.ShutdownAsync();
                await _serversService.DeleteServerAsync(server, request.DeleteAllFiles);

                return Unit.Value;
            }
        }
    }
}
