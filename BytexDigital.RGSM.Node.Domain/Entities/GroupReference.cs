using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class GroupReference : Entity
    {
        [Required]
        public string PermissionId { get; set; }

        [Required]
        public string GroupId { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
