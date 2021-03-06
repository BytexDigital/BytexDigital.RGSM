﻿using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class Node : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string BaseUri { get; set; }

        public virtual ApiKey ApiKey { get; set; }
    }
}
