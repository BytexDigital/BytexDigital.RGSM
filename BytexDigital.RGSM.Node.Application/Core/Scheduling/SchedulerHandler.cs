using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.Application.Core.Commands.Scheduling;
using BytexDigital.RGSM.Node.Application.Core.Commands.Workshop;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;

using Cronos;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Nito.AsyncEx;

namespace BytexDigital.RGSM.Node.Application.Core.Scheduling
{
    public class SchedulerHandler : IHostedService, IAsyncDisposable
    {
        private ConcurrentDictionary<string, (CancellationTokenSource CancellationTokenSource, Task Task, AsyncAutoResetEvent NewSchedulerEvent)> _schedulerTasks;
        private readonly IMediator _mediator;

        public SchedulerHandler(IMediator mediator)
        {
            _schedulerTasks = new ConcurrentDictionary<string, (CancellationTokenSource, Task, AsyncAutoResetEvent)>();
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var data in _schedulerTasks)
            {
                await ShutdownSchedulerAsync(data.Key);
            }
        }

        public async Task InitializeAsync()
        {
            var schedulerPlans = (await _mediator.Send(new GetSchedulersQuery())).SchedulerPlans;

            foreach (var schedulerPlan in schedulerPlans)
            {
                await StartSchedulerAsync(schedulerPlan);
            }
        }

        public Task StartSchedulerAsync(SchedulerPlan schedulerPlan)
        {
            AsyncAutoResetEvent newSchedulerEvent = new AsyncAutoResetEvent();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task task = Task.Run(async () =>
            {
                await WorkAsync(schedulerPlan, newSchedulerEvent, cancellationTokenSource.Token);
            });

            _schedulerTasks.TryAdd(schedulerPlan.Id, (cancellationTokenSource, task, newSchedulerEvent));

            return Task.CompletedTask;
        }

        public Task ShutdownSchedulerAsync(string schedulerId)
        {
            if (!_schedulerTasks.TryGetValue(schedulerId, out var runtimeInfo))
            {
                return Task.CompletedTask;
            }

            runtimeInfo.CancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

        public Task NotifySchedulerOfNewPlanAsync(SchedulerPlan schedulerPlan)
        {
            if (_schedulerTasks.TryGetValue(schedulerPlan.Id, out var data))
            {
                data.NewSchedulerEvent.Set();
            }

            return Task.CompletedTask;
        }

        private async Task WorkAsync(SchedulerPlan schedulerPlan, AsyncAutoResetEvent newSchedulerEvent, CancellationToken cancellationToken)
        {
            bool firstRun = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (firstRun || newSchedulerEvent.IsSet)
                {
                    if (newSchedulerEvent.IsSet)
                    {
                        await newSchedulerEvent.WaitAsync(cancellationToken);
                    }

                    schedulerPlan = (await _mediator.Send(new GetSchedulerQuery
                    {
                        ServerId = schedulerPlan.ServerId,
                        Query = query => query.Include(x => x.ScheduleGroups).ThenInclude(x => x.ScheduleActions)
                    })).SchedulerPlan;
                }

                List<(ScheduleGroup Group, DateTime NextTime)> nextGroupTimes = new List<(ScheduleGroup, DateTime)>();

                foreach (var group in schedulerPlan.ScheduleGroups)
                {
                    var cronExpression = CronExpression.Parse(group.CronExpression);

                    DateTime? nextTime = cronExpression.GetNextOccurrence(DateTime.UtcNow);

                    if (nextTime != null)
                    {
                        nextGroupTimes.Add((group, nextTime.Value));
                    }
                }

                var nextCombinedGroups = nextGroupTimes.GroupBy(x => x.NextTime).OrderBy(x => x.Key).FirstOrDefault();

                if (nextCombinedGroups == default)
                {
                    await newSchedulerEvent.WaitAsync(cancellationToken);
                    continue;
                }

                var nextGroup = nextCombinedGroups.OrderBy(x => x.Group.Priority).FirstOrDefault();

                try
                {

                }
                catch (Exception ex)
                {
                    await RunGroupAsync(schedulerPlan, nextGroup.Group, cancellationToken);
                }
            }
        }

        private async Task RunGroupAsync(SchedulerPlan schedulerPlan, ScheduleGroup scheduleGroup, CancellationToken cancellationToken)
        {
            foreach (var action in scheduleGroup.ScheduleActions.OrderBy(x => x.Order))
            {
                try
                {
                    switch (action.ActionType)
                    {
                        case Domain.Enumerations.ScheduleActionType.StartServer:
                        {
                            await _mediator.Send(new ChangeServerStatusCmd { Id = schedulerPlan.ServerId, StartOrStop = true });
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.StopServer:
                        {
                            await _mediator.Send(new ChangeServerStatusCmd { Id = schedulerPlan.ServerId, StartOrStop = false });
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.UpdateWorkshopModifications:
                        {
                            await _mediator.Send(new BeginUpdateWorkshopModsCmd { Id = schedulerPlan.ServerId });
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.UpdateGameserver:
                        {
                            await _mediator.Send(new InstallOrUpdateServerCmd { Id = schedulerPlan.ServerId });
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.BeRcon_SendMessage:
                        {
                            var message = action.KeyValues.FirstOrDefault(x => x.Key == "message")?.Value;

                            await _mediator.Send(new Commands.BattlEye.SendRconMessageCmd { Id = schedulerPlan.ServerId, Message = message });
                        }
                        break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (!action.ContinueOnError)
                    {
                        throw;
                    }
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
