using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class Group : Entity
    {
        public const string DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME = "system_administrator";
        public const string DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_DISPLAYNAME = "System Administrator";


        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public virtual ICollection<ApplicationUserGroup> Users { get; set; }
    }
}
