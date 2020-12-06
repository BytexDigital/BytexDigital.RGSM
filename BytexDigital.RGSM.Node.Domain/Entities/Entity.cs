using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using BytexDigital.RGSM.Node.Domain.Interfaces;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public abstract class Entity : IHasCreationTimestamp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public DateTimeOffset TimeCreated { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
