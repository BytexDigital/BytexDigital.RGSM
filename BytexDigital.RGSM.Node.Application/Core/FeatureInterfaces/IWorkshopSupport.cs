using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.Workshop;

namespace BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces
{
    public interface IWorkshopSupport
    {
        Task<List<WorkshopItem>> GetWorkshopModsAsync(CancellationToken cancellationToken = default);
        Task<CanResult> CanUpdateWorkshopModsAsync(CancellationToken cancellationToken = default);
        Task BeginUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default);
        Task CancelUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default);
    }
}
