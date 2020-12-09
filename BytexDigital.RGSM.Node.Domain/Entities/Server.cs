using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Enumerations;

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

        public virtual Arma3Server Arma3Server { get; set; }
        public virtual ICollection<TrackedDepot> TrackedDepots { get; set; }
        public virtual ICollection<TrackedWorkshopMod> TrackedWorkshopMods { get; set; }
    }
}
