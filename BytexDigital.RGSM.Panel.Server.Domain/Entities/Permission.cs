using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class Permission : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ServerId { get; set; }

        public virtual ICollection<GroupPermission> Groups { get; set; }
    }
}
