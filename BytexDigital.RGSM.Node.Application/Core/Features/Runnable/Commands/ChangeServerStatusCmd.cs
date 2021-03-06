﻿using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Features.Runnable;
using BytexDigital.RGSM.Node.Application.Core.Servers;
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
            private readonly ServerService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(ChangeServerStatusCmd request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription("Server not found.");

                var state = _serverStateRegister.GetServerState(request.Id);

                if (state is not IRunnable runnableState)
                    throw new ServerDoesNotSupportFeatureException<IRunnable>();

                // Start
                if (request.StartOrStop)
                {
                    var canStart = await runnableState.CanStartAsync();

                    if (!canStart)
                        throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription($"Server cannot be started at the time. Reason: {canStart.FailureReason}");

                    await runnableState.StartAsync();
                }
                // Stop
                else
                {
                    var canStop = await runnableState.CanStopAsync();

                    if (!canStop)
                        throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription($"Server cannot be stopped at the time. Reason: {canStop.FailureReason}");

                    await runnableState.StopAsync();
                }

                return Unit.Value;
            }
        }
    }
}
