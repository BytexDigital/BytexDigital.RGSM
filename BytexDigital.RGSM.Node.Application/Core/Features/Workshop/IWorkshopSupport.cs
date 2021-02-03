using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop
{
    public interface IWorkshopSupport
    {
        Task AddOrUpdateWorkshopModAsync(PublishedFileId id, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);
        Task RemoveWorkshopModAsync(PublishedFileId id, CancellationToken cancellationToken = default);
        Task EnableOrDisableWorkshopModAsync(PublishedFileId id, bool enabled, CancellationToken cancellationToken = default);
        Task<List<WorkshopModState>> GetWorkshopModStatesAsync(CancellationToken cancellationToken = default);
        Task<CanResult> CanUpdateWorkshopModsAsync(CancellationToken cancellationToken = default);
        Task BeginUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default);
        Task CancelUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default);
    }
}
