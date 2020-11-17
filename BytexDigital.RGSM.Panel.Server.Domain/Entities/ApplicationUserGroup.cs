using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class ApplicationUserGroup : Entity
    {
        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
