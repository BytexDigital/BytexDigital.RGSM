using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class NodeKey : Entity
    {
        [Required]
        public string NodeId { get; set; }

        [Required]
        public string ApiKey { get; set; }

        public virtual Node Node { get; set; }
    }
}
