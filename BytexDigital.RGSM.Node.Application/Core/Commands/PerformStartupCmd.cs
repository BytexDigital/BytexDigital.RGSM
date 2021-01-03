using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.Scheduling;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Steam;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class PerformStartupCmd : IRequest
    {
        public class Handler : IRequestHandler<PerformStartupCmd>
        {
            private readonly ServerIntegrityService _serverIntegrityService;
            private readonly ServerStateRegister _serverStateRegister;
            private readonly SteamDownloadService _steamDownloadService;
            private readonly SchedulerHandler _schedulerHandler;

            public Handler(
                ServerIntegrityService serverIntegrityService,
                ServerStateRegister serverStateRegister,
                SteamDownloadService steamDownloadService,
                SchedulerHandler schedulerHandler)
            {
                _serverIntegrityService = serverIntegrityService;
                _serverStateRegister = serverStateRegister;
                _steamDownloadService = steamDownloadService;
                _schedulerHandler = schedulerHandler;
            }

            public async Task<Unit> Handle(PerformStartupCmd request, CancellationToken cancellationToken)
            {
                await _serverStateRegister.InitializeAsync();
                await _serverIntegrityService.EnsureCorrectSetupAllAsync();
                await _steamDownloadService.InitializeAsync();
                await _schedulerHandler.InitializeAsync();

                return Unit.Value;
            }
        }
    }
}
