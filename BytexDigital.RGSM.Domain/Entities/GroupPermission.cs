using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class GroupPermission : Entity
    {
        [Required]
        public string GroupId { get; set; }

        [Required]
        public string PermissionId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
