using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Entities;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class NodeSetting : Entity
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
