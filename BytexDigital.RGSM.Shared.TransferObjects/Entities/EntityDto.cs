using System;

namespace BytexDigital.RGSM.Shared.TransferObjects.Entities
{
    public abstract class EntityDto
    {
        public string Id { get; set; }
        public DateTimeOffset TimeCreated { get; set; }
    }
}
