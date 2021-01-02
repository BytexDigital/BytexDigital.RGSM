using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Arma3;
using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Shared.Enumerations;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ServerCreationService
    {
        private readonly NodeDbContext _nodeDbContext;
        private readonly ServersService _serversService;

        public ServerCreationService(NodeDbContext nodeDbContext, ServersService serversService)
        {
            _nodeDbContext = nodeDbContext;
            _serversService = serversService;
        }

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
                    await CreateArmaServerAsync(server, displayName);
                }
                break;

                default:
                    throw new NotImplementedException();
            }

            return _serversService.GetServer(server.Id);
        }

        private async Task CreateArmaServerAsync(Server server, string displayName)
        {
            var a3Server = _nodeDbContext.CreateEntity(x => x.Arma3Server);

            a3Server.IsInstalled = false;
            a3Server.Port = 2302;

            a3Server.RconIp = "0.0.0.0";
            a3Server.RconPort = a3Server.Port + 10;
            a3Server.RconPassword = Guid.NewGuid().ToString();

            server.Arma3Server = a3Server;

            await _nodeDbContext.SaveChangesAsync();
        }
    }
}
