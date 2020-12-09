using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public interface IWorkshopStorage
    {
        Task<string> GetWorkshopModPathAsync(TrackedWorkshopMod trackedWorkshopMod, CancellationToken cancellationToken);
    }
}
