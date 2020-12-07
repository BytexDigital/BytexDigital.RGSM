using System;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Enumerations;
using BytexDigital.RGSM.Node.Persistence;

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

        public async Task<IQueryable<Server>> CreateServerAsync(string displayName, ServerType serverType)
        {
            var server = _nodeDbContext.CreateEntity(x => x.Servers);

            server.DisplayName = displayName;
            server.Type = serverType;

            switch (serverType)
            {
                case ServerType.Arma3:
                {
                    server.Arma3Server = CreateArma3Server();
                }
                break;

                default:
                    throw new NotImplementedException();
            }

            _nodeDbContext.Servers.Add(server);
            await _nodeDbContext.SaveChangesAsync();

            return GetServer(server.Id);
        }

        private Arma3Server CreateArma3Server()
        {
            var server = _nodeDbContext.CreateEntity(x => x.Arma3Server);

            return server;
        }
    }
}
