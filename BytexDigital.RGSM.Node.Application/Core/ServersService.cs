using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Enumerations;
using BytexDigital.RGSM.Node.Persistence;
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

        public async Task<IQueryable<Server>> CreateServerAsync(string displayName, string directory, ServerType serverType)
        {
            System.IO.Directory.CreateDirectory(directory);

            var server = _nodeDbContext.CreateEntity(x => x.Servers);

            server.DisplayName = displayName;
            server.Type = serverType;
            server.Directory = directory;

            switch (serverType)
            {
                case ServerType.Arma3:
                {
                    server.Arma3Server = CreateArma3Server(displayName);

                    var contentDepot = _nodeDbContext.CreateEntity(x => x.TrackedDepots);
                    contentDepot.DepotId = 233781;

                    var windowsDepot = _nodeDbContext.CreateEntity(x => x.TrackedDepots);
                    windowsDepot.DepotId = 233782;

                    server.TrackedDepots = new List<TrackedDepot> { contentDepot, windowsDepot };
                }
                break;

                default:
                    throw new NotImplementedException();
            }

            _nodeDbContext.Servers.Add(server);
            await _nodeDbContext.SaveChangesAsync();

            return GetServer(server.Id);
        }

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

        private Arma3Server CreateArma3Server(string displayName)
        {
            var server = _nodeDbContext.CreateEntity(x => x.Arma3Server);

            server.IsInstalled = false;
            server.Port = 2302;

            server.RconIp = "0.0.0.0";
            server.RconPort = server.Port + 10;
            server.RconPassword = Guid.NewGuid().ToString();

            return server;
        }
    }
}
