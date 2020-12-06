using System;

namespace BytexDigital.RGSM.Node.Domain.Interfaces
{
    public interface IHasCreationTimestamp
    {
        public DateTimeOffset TimeCreated { get; set; }
    }
}
