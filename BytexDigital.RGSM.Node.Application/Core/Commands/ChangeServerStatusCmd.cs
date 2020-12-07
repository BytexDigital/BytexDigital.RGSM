﻿using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{

    public class ChangeServerStatusCmd : IRequest
    {
        public string Id { get; set; }
        public bool StartOrStop { get; set; }

        public class Handler : IRequestHandler<ChangeServerStatusCmd>
        {
            private readonly ServersService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServersService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(ChangeServerStatusCmd request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServiceException().WithField(nameof(request.Id)).WithMessage("Server not found.");

                var state = _serverStateRegister.GetServerState(request.Id);

                if (state is not IRunnable runnableState)
                    throw new ServerNotRunnableException();

                // Start
                if (request.StartOrStop)
                {
                    if (!await runnableState.CanStartAsync())
                        throw new ServiceException().WithField(nameof(request.Id)).WithMessage("Server cannot be started at the time.");

                    await runnableState.StartAsync();
                }
                // Stop
                else
                {
                    if (!await runnableState.CanStopAsync())
                        throw new ServiceException().WithField(nameof(request.Id)).WithMessage("Server cannot be stopped at the time.");

                    await runnableState.StopAsync();
                }

                return Unit.Value;
            }
        }
    }
}
