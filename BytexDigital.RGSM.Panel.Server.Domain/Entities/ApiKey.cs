using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class ApiKey : Entity
    {
        public string NodeId { get; set; }

        [Required]
        public string Value { get; set; }

        public virtual Node Node { get; set; }
    }
}
