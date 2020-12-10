using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Node.Domain.Enumerations;

namespace BytexDigital.RGSM.Node.Domain.Entities.Scheduling
{
    public class ScheduleAction : Entity
    {
        [Required]
        public string ScheduleEventId { get; set; }

        [Required]
        public ScheduleActionType ActionType { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool ContinueOnError { get; set; }

        public virtual ICollection<KeyValue> KeyValues { get; set; }
        public virtual ScheduleGroup ScheduleGroups { get; set; }
    }
}
