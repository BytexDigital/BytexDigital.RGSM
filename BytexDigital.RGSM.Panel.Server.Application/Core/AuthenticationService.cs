
using System.Linq;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
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

        public IQueryable<ApiKey> GetApiKey(string value)
        {
            return _applicationDbContext.ApiKeys.Where(x => x.Value == value);
        }
    }
}
