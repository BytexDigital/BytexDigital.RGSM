using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class Node : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string BaseUri { get; set; }

        public virtual ICollection<Server> Servers { get; set; }
    }
}
