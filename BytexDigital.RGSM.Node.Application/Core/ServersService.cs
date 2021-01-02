using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Shared.Enumerations;
using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ServersService
    {
        private readonly NodeDbContext _nodeDbContext;

        public ServersService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<Server> GetServers() => _nodeDbContext.Servers;
        public IQueryable<Server> GetServer(string id) => _nodeDbContext.Servers.Where(x => x.Id == id);
        public IQueryable<Arma3Server> GetArmaServer(string id) => _nodeDbContext.Arma3Server.Where(x => x.Server.Id == id);
        public IQueryable<Permission> GetServerPermissions(Server server)
            => _nodeDbContext.Permissions.Where(x => x.ServerId == server.Id);

        public async Task AddTrackedWorkshopItemAsync(Server server, PublishedFileId publishedFileId)
        {
            var trackedMod = _nodeDbContext.CreateEntity(x => x.TrackedWorkshopMods);
            trackedMod.PublishedFileId = publishedFileId.Id;
            trackedMod.IsLoaded = false;

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

        public IQueryable<TrackedWorkshopMod> GetTrackedWorkshopMods(Server server)
            => _nodeDbContext.TrackedWorkshopMods.Where(x => x.ServerId == server.Id);
    }
}
