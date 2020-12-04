using System;

namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Entities
{
    public abstract class EntityDto
    {
        public string Id { get; set; }
        public DateTimeOffset TimeCreated { get; set; }
    }
}
