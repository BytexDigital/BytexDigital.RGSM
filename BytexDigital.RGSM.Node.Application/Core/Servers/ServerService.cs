using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Shared.Enumerations;

namespace BytexDigital.RGSM.Node.Application.Core.Servers
{
    public class ServerService
    {
        private readonly NodeDbContext _nodeDbContext;

        public ServerService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<Server> GetServers() => _nodeDbContext.Servers;
        public IQueryable<Server> GetServer(string id) => _nodeDbContext.Servers.Where(x => x.Id == id);
        public IQueryable<Permission> GetServerPermissions(Server server)
            => _nodeDbContext.Permissions.Where(x => x.ServerId == server.Id);

        public async Task<IQueryable<Server>> CreateServerAsync(string displayName, string directory, ServerType serverType)
        {
            System.IO.Directory.CreateDirectory(directory);

            var server = _nodeDbContext.CreateEntity(x => x.Servers);

            server.DisplayName = displayName;
            server.Type = serverType;
            server.Directory = directory;
            server.SchedulerPlan = _nodeDbContext.CreateEntity(x => x.SchedulerPlans);
            server.SchedulerPlan.IsEnabled = false;

            _nodeDbContext.Servers.Add(server);
            await _nodeDbContext.SaveChangesAsync();

            switch (serverType)
            {
                case ServerType.Arma3:
                {
                    //await CreateArmaServerAsync(server, displayName);
                }
                break;

                default:
                    throw new NotImplementedException();
            }

            return GetServer(server.Id);
        }

        //private async Task CreateArmaServerAsync(Server server, string displayName)
        //{
        //    var a3Server = _nodeDbContext.CreateEntity(x => x.Arma3Server);

        //    a3Server.IsInstalled = false;
        //    a3Server.Port = 2302;

        //    a3Server.RconIp = "0.0.0.0";
        //    a3Server.RconPort = a3Server.Port + 10;
        //    a3Server.RconPassword = Guid.NewGuid().ToString();

        //    server.Arma3Server = a3Server;

        //    await _nodeDbContext.SaveChangesAsync();
        //}

        public async Task DeleteServerAsync(Server server, bool deleteAllFiles)
        {
            _nodeDbContext.Servers.Remove(server);
            await _nodeDbContext.SaveChangesAsync();

            if (!Directory.Exists(server.Directory)) return;

            var files = Directory.GetFiles(server.Directory);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }

            try
            {
                Directory.Delete(server.Directory, true);
            }
            catch { }
        }
    }
}
