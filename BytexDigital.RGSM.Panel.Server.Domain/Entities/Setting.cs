using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class Setting : Entity
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
