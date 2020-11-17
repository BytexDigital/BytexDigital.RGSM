using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class Group : Entity
    {
        [Required]
        public string DisplayName { get; set; }

        public virtual ICollection<ApplicationUserGroup> Users { get; set; }
    }
}
