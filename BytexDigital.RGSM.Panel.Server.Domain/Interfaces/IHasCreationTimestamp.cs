using System;

namespace BytexDigital.RGSM.Panel.Server.Domain.Interfaces
{
    public interface IHasCreationTimestamp
    {
        public DateTimeOffset TimeCreated { get; set; }
    }
}
