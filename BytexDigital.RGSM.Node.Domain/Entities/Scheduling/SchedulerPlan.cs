using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities.Scheduling
{
    public class SchedulerPlan : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        public virtual Server Server { get; set; }
        public virtual ICollection<ScheduleGroup> ScheduleGroups { get; set; }
    }
}
