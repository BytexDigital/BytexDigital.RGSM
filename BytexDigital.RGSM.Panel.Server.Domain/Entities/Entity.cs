using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using BytexDigital.RGSM.Panel.Server.Domain.Interfaces;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public abstract class Entity : IHasCreationTimestamp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public DateTimeOffset TimeCreated { get; set; }
    }
}
