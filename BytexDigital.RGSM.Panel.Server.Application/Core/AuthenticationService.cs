
using System.Linq;

using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core
{
    public class AuthenticationService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthenticationService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<Domain.Entities.Node> GetNodeByApiKey(string key)
        {
            return _applicationDbContext.Nodes.Where(x => x.ApiKey == key);
        }
    }
}
