using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;
using BytexDigital.RGSM.Node.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Scheduling
{
    public class SchedulersService
    {
        private readonly NodeDbContext _nodeDbContext;

        public SchedulersService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<SchedulerPlan> GetSchedulerPlan(Server server)
        {
            Debug.WriteLine($"GetSchedulerPlan: {_nodeDbContext.ContextId}");

            return _nodeDbContext.SchedulerPlans.Where(x => x.ServerId == server.Id).Include(x => x.ScheduleGroups).ThenInclude(x => x.ScheduleActions);
        }

        public IQueryable<SchedulerPlan> GetSchedulerPlans()
            => _nodeDbContext.SchedulerPlans.Include(x => x.ScheduleGroups).ThenInclude(x => x.ScheduleActions);

        public async Task ChangeSchedulerAsync(SchedulerPlan schedulerPlan, SchedulerPlan changedSchedulerPlan)
        {
            schedulerPlan.IsEnabled = changedSchedulerPlan.IsEnabled;

            // Delete not existing entities anymore
            foreach (var group in schedulerPlan.ScheduleGroups.ToList())
            {
                var changedGroup = changedSchedulerPlan.ScheduleGroups.FirstOrDefault(x => x.Id == group.Id);

                if (changedGroup == null)
                {
                    schedulerPlan.ScheduleGroups.Remove(group);
                    _nodeDbContext.ScheduleGroups.Remove(group);
                }
                else
                {
                    foreach (var action in group.ScheduleActions)
                    {
                        var changedAction = changedGroup.ScheduleActions.FirstOrDefault(x => x.Id == action.Id);

                        if (changedAction == null)
                        {
                            group.ScheduleActions.Remove(action);
                            _nodeDbContext.ScheduleActions.Remove(action);
                        }
                    }
                }
            }

            // Merge existing and new
            foreach (var changedGroup in changedSchedulerPlan.ScheduleGroups)
            {
                var group = schedulerPlan.ScheduleGroups.FirstOrDefault(x => x.Id == changedGroup.Id);

                if (group == null)
                {
                    group = _nodeDbContext.CreateEntity(x => x.ScheduleGroups);
                    group.ScheduleActions = new List<ScheduleAction>();

                    schedulerPlan.ScheduleGroups.Add(group);
                }

                group.CronExpression = changedGroup.CronExpression;
                group.DisplayName = changedGroup.DisplayName;
                group.Priority = changedGroup.Priority;

                foreach (var changedAction in changedGroup.ScheduleActions)
                {
                    var action = group.ScheduleActions.FirstOrDefault(x => x.Id == changedAction.Id);

                    if (action == null)
                    {
                        action = _nodeDbContext.CreateEntity(x => x.ScheduleActions);
                        action.KeyValues = new List<KeyValue>();

                        group.ScheduleActions.Add(action);
                    }

                    action.ContinueOnError = changedAction.ContinueOnError;
                    action.ActionType = changedAction.ActionType;
                    action.KeyValues = changedAction.KeyValues;
                    action.Order = changedAction.Order;
                }
            }

            await _nodeDbContext.SaveChangesAsync();
            await _nodeDbContext.Entry(schedulerPlan).ReloadAsync();
        }
    }
}
