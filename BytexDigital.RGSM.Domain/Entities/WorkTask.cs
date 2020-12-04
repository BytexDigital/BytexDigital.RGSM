using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Enumerations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class WorkTask : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public WorkTaskStatus Status { get; set; }

        public string FailureDescription { get; set; }

        [Required]
        public string Description { get; set; }

        public Server Server { get; set; }
    }
}
