using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class Permission : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual Server Server { get; set; }
        public virtual ICollection<GroupReference> GroupReferences { get; set; }
    }
}
