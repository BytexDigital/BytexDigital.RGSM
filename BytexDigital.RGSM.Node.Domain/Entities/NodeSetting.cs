using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class NodeSetting
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
