using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities.Scheduling
{
    public class ScheduleGroup : Entity
    {
        [Required]
        public string SchedulerPlanId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string CronExpression { get; set; }

        [Required]
        public int Priority { get; set; }

        public virtual SchedulerPlan SchedulerPlan { get; set; }
        public virtual ICollection<ScheduleAction> ScheduleActions { get; set; }
    }
}
