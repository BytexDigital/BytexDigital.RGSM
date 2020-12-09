using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Persistence;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaServerService
    {
        private readonly NodeDbContext _nodeDbContext;

        public ArmaServerService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<Arma3Server> GetServer(string id) => _nodeDbContext.Arma3Server.Where(x => x.Server.Id == id);

        public async Task MarkAsInstalledAsync(Arma3Server arma3Server)
        {
            arma3Server.IsInstalled = true;

            await _nodeDbContext.SaveChangesAsync();
        }
    }
}
