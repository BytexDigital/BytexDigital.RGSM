using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Core.Steam
{
    public class WorkshopManagerService
    {
        private readonly NodeDbContext _nodeDbContext;

        public WorkshopManagerService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public async Task AddTrackedWorkshopItemAsync(Server server, PublishedFileId publishedFileId)
        {
            var trackedMod = _nodeDbContext.CreateEntity(x => x.TrackedWorkshopMods);
            trackedMod.PublishedFileId = publishedFileId.Id;
            trackedMod.Load = false;

            server.TrackedWorkshopMods.Add(trackedMod);

            await _nodeDbContext.SaveChangesAsync();
        }

        public async Task RemoveTrackedWorkshopItemAsync(Server server, PublishedFileId publishedFileId)
        {
            var trackedMod = server.TrackedWorkshopMods.FirstOrDefault(x => x.PublishedFileId == publishedFileId);

            if (trackedMod != null)
            {
                server.TrackedWorkshopMods.Remove(trackedMod);
                await _nodeDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateTrackedWorkshopItemAsync(TrackedWorkshopMod trackedWorkshopMod, bool load)
        {
            trackedWorkshopMod.Load = load;

            await _nodeDbContext.SaveChangesAsync();
        }

        public IQueryable<TrackedWorkshopMod> GetTrackedWorkshopMods(Server server)
            => _nodeDbContext.TrackedWorkshopMods.Where(x => x.ServerId == server.Id);
    }
}
