using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Scheduling;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;

using Cronos;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Scheduling
{
    public class UpdateSchedulerCmd : IRequest
    {
        public string ServerId { get; set; }
        public SchedulerPlan ChangedSchedulerPlan { get; set; }

        public class Handler : IRequestHandler<UpdateSchedulerCmd>
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

            public async Task<Unit> Handle(UpdateSchedulerCmd request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.ServerId).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                var schedulerPlan = await _schedulersService.GetSchedulerPlan(server).FirstOrDefaultAsync();

                await _schedulersService.ChangeSchedulerAsync(schedulerPlan, request.ChangedSchedulerPlan);
                await _schedulerHandler.NotifySchedulerOfNewPlanAsync(schedulerPlan);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<UpdateSchedulerCmd>
        {
            public Validator()
            {
                RuleFor(x => x.ChangedSchedulerPlan)
                    .NotNull()
                    .DependentRules(() =>
                    {
                        RuleForEach(x => x.ChangedSchedulerPlan.ScheduleGroups)
                            .ChildRules(g =>
                            {
                                g.RuleFor(x => x.CronExpression)
                                    .Must(x =>
                                    {
                                        try
                                        {
                                            _ = CronExpression.Parse(x);
                                            return true;
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    })
                                    .WithMessage("Invalid cron expression.");
                            });
                    });
            }
        }
    }
}
