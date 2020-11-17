using System.Collections.Generic;

namespace BytexDigital.RGSM.Shared.Interfaces
{
    public interface IErrorCollection
    {
        public IReadOnlyCollection<IError> Errors { get; }
    }
}
