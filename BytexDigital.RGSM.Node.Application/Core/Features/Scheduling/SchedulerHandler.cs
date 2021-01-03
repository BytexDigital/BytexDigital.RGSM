using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.BattlEye.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.Installable;
using BytexDigital.RGSM.Node.Application.Core.Features.Installable.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.Scheduling.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;

using Cronos;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Nito.AsyncEx;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Scheduling
{
    public class SchedulerHandler : IHostedService, IAsyncDisposable
    {
        private ConcurrentDictionary<string, (CancellationTokenSource CancellationTokenSource, Task Task, AsyncManualResetEvent NewSchedulerEvent)> _schedulerTasks;
        private readonly IMediator _mediator;

        public SchedulerHandler(IMediator mediator)
        {
            _schedulerTasks = new ConcurrentDictionary<string, (CancellationTokenSource, Task, AsyncManualResetEvent)>();
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
            AsyncManualResetEvent newSchedulerEvent = new AsyncManualResetEvent();
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

        public async Task NotifySchedulerOfNewPlanAsync(SchedulerPlan schedulerPlan)
        {
            if (_schedulerTasks.TryGetValue(schedulerPlan.Id, out var data))
            {
                if (!data.Task.IsCompleted)
                {
                    data.NewSchedulerEvent.Set();
                }
                else
                {
                    await StartSchedulerAsync(schedulerPlan);
                }
            }
            else
            {
                await StartSchedulerAsync(schedulerPlan);
            }
        }

        private async Task WorkAsync(SchedulerPlan schedulerPlan, AsyncManualResetEvent newSchedulerEvent, CancellationToken cancellationToken)
        {
            bool firstRun = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (firstRun || newSchedulerEvent.IsSet)
                {
                    firstRun = false;

                    schedulerPlan = (await _mediator.Send(new GetSchedulerQuery
                    {
                        ServerId = schedulerPlan.ServerId,
                        Query = query => query.Include(x => x.ScheduleGroups).ThenInclude(x => x.ScheduleActions).ThenInclude(x => x.KeyValues)
                    })).SchedulerPlan;

                    newSchedulerEvent.Reset();

                    if (!schedulerPlan.IsEnabled)
                    {
                        return;
                    }
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
                    // No next group to execute found. Wait until a new scheduler event arrives or we are asked to cancel the worker.
                    await newSchedulerEvent.WaitAsync(cancellationToken);
                    continue;
                }

                var nextGroup = nextCombinedGroups.OrderBy(x => x.Group.Priority).FirstOrDefault();

                // Wait until the group should execute or wait for a signal that the scheduler got updated
                var cancellationTokenSource = new CancellationTokenSource();
                var combinedCancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);

                var delayTask = Task.Delay(nextGroup.NextTime - DateTime.UtcNow, combinedCancelToken.Token);
                var newSchedulerTask = newSchedulerEvent.WaitAsync(combinedCancelToken.Token);

                var endingTask = await Task.WhenAny(delayTask, newSchedulerTask);

                cancellationTokenSource.Cancel();

                if (endingTask == newSchedulerTask)
                {
                    continue;
                }

                try
                {
                    await RunGroupAsync(schedulerPlan, nextGroup.Group, cancellationToken);
                }
                catch (Exception ex)
                {
                    // TODO: Add logging
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
                        case Domain.Enumerations.ScheduleActionType.ExecutionDelay:
                        {
                            var secondsStr = action.KeyValues.FirstOrDefault(x => x.Key == "message")?.Value ?? "0";
                            var seconds = int.Parse(secondsStr);

                            await Task.Delay(seconds, cancellationToken);
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.RunExecutable:
                        {
                            var executablePath = action.KeyValues.FirstOrDefault(x => x.Key == "path")?.Value;
                            var arguments = action.KeyValues.FirstOrDefault(x => x.Key == "arguments")?.Value;

                            if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath))
                            {
                                var psi = new ProcessStartInfo();
                                psi.Arguments = arguments;
                                psi.FileName = executablePath;

                                var process = Process.Start(psi);

                                var processCancelToken = CancellationTokenSource.CreateLinkedTokenSource(
                                    cancellationToken,
                                    new CancellationTokenSource(TimeSpan.FromMinutes(15)).Token);

                                await process.WaitForExitAsync(processCancelToken.Token);
                            }
                        }
                        break;

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
                            await InstallOrUpdateWorkshopModificationsAsync(schedulerPlan, scheduleGroup, cancellationToken);
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.UpdateGameserver:
                        {
                            await InstallOrUpdateServerAsync(schedulerPlan, scheduleGroup, cancellationToken);
                        }
                        break;

                        case Domain.Enumerations.ScheduleActionType.BeRcon_SendMessage:
                        {
                            var message = action.KeyValues.FirstOrDefault(x => x.Key == "message")?.Value;

                            await _mediator.Send(new SendRconMessageCmd { Id = schedulerPlan.ServerId, Message = message });
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

        private async Task InstallOrUpdateServerAsync(SchedulerPlan schedulerPlan, ScheduleGroup scheduleGroup, CancellationToken cancellationToken)
        {
            await _mediator.Send(new InstallOrUpdateServerCmd { Id = schedulerPlan.ServerId });

            // Continuously query for the status
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                var installationResponse = await _mediator.Send(new GetServerInstallationStatusQuery { Id = schedulerPlan.ServerId }, cancellationToken);

                if (!installationResponse.InstallationStatus.IsUpdating)
                {
                    if (installationResponse.InstallationStatus.FailureReason != default)
                    {
                        throw new Exception($"Server update failed: {installationResponse.InstallationStatus.FailureReason}");
                    }

                    break;
                }
            }
        }

        private async Task InstallOrUpdateWorkshopModificationsAsync(SchedulerPlan schedulerPlan, ScheduleGroup scheduleGroup, CancellationToken cancellationToken)
        {
            await _mediator.Send(new BeginUpdateWorkshopModsCmd { Id = schedulerPlan.ServerId });

            // Continuously query for the status
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                var workshopItemsResponse = await _mediator.Send(new GetWorkshopItemStatusQuery { Id = schedulerPlan.ServerId }, cancellationToken);

                if (workshopItemsResponse.WorkshopItems.All(x => !x.IsUpdating))
                {
                    var faultedItems = workshopItemsResponse.WorkshopItems.Where(x => x.UpdateFailureReason != default);

                    var exceptions = new List<Exception>();

                    foreach (var faultedItem in faultedItems)
                    {
                        exceptions.Add(new Exception($"Workshop item update failed: {faultedItem.UpdateFailureReason}"));
                    }

                    throw new AggregateException(exceptions);
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
