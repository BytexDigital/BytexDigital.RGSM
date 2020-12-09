using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class TrackedDepot : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public long DepotId { get; set; }

        public virtual Server Server { get; set; }
    }
}
