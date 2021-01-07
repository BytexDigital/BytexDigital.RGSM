
using System.Linq;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Authentication.ApiKeys
{
    public class ApiKeyService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ApiKeyService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<ApiKey> GetApiKey(string value)
        {
            return _applicationDbContext.ApiKeys.Where(x => x.Value == value);
        }
    }
}
