using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class SharedSetting
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
