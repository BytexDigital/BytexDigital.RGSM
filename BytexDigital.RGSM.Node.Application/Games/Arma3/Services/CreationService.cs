using System.Threading.Tasks;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Persistence;

namespace BytexDigital.RGSM.Node.Application.Games.Arma3.Services
{
    public class CreationService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CreationService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Server> CreateServerAsync(RGSM.Domain.Entities.Node node, string displayName, string directory)
        {
            var server = _applicationDbContext.CreateEntity(x => x.Servers);

            server.DisplayName = displayName;
            server.Directory = directory;
            server.NodeId = node.Id;

            server.Arma3Server = _applicationDbContext.CreateEntity(x => x.Arma3Servers);

            server.Arma3Server.Hostname = "Hostname";
            server.Arma3Server.MaxPlayers = 10;
            server.Arma3Server.VerifySignatures = 2;

            _applicationDbContext.Servers.Add(server);
            await _applicationDbContext.SaveChangesAsync();

            return server;
        }
    }
}
