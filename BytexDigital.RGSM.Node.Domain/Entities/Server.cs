using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;
using BytexDigital.RGSM.Node.Domain.Models.Arma;
using BytexDigital.RGSM.Shared.Enumerations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class Server : Entity
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public ServerType Type { get; set; }

        [Required]
        public string Directory { get; set; }

        public virtual SchedulerPlan SchedulerPlan { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
