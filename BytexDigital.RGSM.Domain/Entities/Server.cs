using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Enumerations;
using BytexDigital.RGSM.Domain.Games.Arma3.Entities;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class Server : Entity
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string NodeId { get; set; }

        [Required]
        public ServerType Type { get; set; }

        [Required]
        public ServerStatus Status { get; set; }

        public string Directory { get; set; }


        public virtual Arma3Server Arma3Server { get; set; }

        public virtual Node Node { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<WorkTask> Tasks { get; set; }
    }
}
