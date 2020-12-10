using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class KeyValue : Entity
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public virtual ICollection<ScheduleAction> ScheduleActions { get; set; }
    }
}
