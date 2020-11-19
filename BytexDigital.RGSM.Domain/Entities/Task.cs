using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class Task : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
