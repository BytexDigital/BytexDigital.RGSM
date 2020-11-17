using System;

namespace BytexDigital.RGSM.Domain.Interfaces
{
    public interface IHasCreationTimestamp
    {
        public DateTimeOffset TimeCreated { get; set; }
    }
}
