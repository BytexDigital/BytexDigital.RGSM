using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Enumerations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class Server : Entity
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public ServerType Type { get; set; }

        [Required]
        public ServerStatus Status { get; set; }

        public string Directory { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
