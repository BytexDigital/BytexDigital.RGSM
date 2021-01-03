using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Scheduling.Commands
{
    public class UpdateSchedulerEnabledCmd : IRequest
    {
        public string ServerId { get; set; }
        public bool Enable { get; set; }

        public class Handler : IRequestHandler<UpdateSchedulerEnabledCmd>
        {
            private readonly ServersService _serversService;
            private readonly SchedulersService _schedulersService;
            private readonly SchedulerHandler _schedulerHandler;

            public Handler(ServersService serversService, SchedulersService schedulersService, SchedulerHandler schedulerHandler)
            {
                _serversService = serversService;
                _schedulersService = schedulersService;
                _schedulerHandler = schedulerHandler;
            }

            public async Task<Unit> Handle(UpdateSchedulerEnabledCmd request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.ServerId).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                var schedulerPlan = await _schedulersService.GetSchedulerPlan(server).FirstOrDefaultAsync();

                await _schedulersService.EnableSchedulerAsync(schedulerPlan, request.Enable);
                await _schedulerHandler.NotifySchedulerOfNewPlanAsync(schedulerPlan);

                return Unit.Value;
            }
        }
    }
}
