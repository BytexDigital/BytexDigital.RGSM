using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Services
{
    public class SteamCredentialsService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SteamCredentialsService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<SteamCredential> GetCredentials(string id)
        {
            return _applicationDbContext.SteamCredentials.Where(x => x.Id == id);
        }

        public IQueryable<SteamCredential> GetCredentialsForApp(long appId, bool requireWorkshopAccess = false)
        {
            if (requireWorkshopAccess)
            {
                return _applicationDbContext.SteamCredentials.Where(x => x.SteamCredentialSupportedApps.Any(supportedApp => supportedApp.AppId == appId && supportedApp.SupportsWorkshop));
            }

            return _applicationDbContext.SteamCredentials.Where(x => x.SteamCredentialSupportedApps.Any(supportedApp => supportedApp.AppId == appId));
        }

        public async Task AddSteamCredentialAsync(SteamCredential steamCredential)
        {
            var entry = _applicationDbContext.CreateEntity(x => x.SteamCredentials);

            entry.Username = steamCredential.Username;
            entry.Password = steamCredential.Password;
            entry.LoginKey = steamCredential.LoginKey;
            entry.Sentry = steamCredential.Sentry;

            _applicationDbContext.SteamCredentials.Add(entry);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task AddSupportedAppAsync(SteamCredential steamCredential, long appId, bool supportsWorkshop)
        {
            steamCredential.SteamCredentialSupportedApps.Add(new SteamCredentialSupportedApp
            {
                AppId = appId,
                SupportsWorkshop = supportsWorkshop
            });

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task RemoveSupportedAppAsync(SteamCredential steamCredential, long appId)
        {
            var toRemove = steamCredential.SteamCredentialSupportedApps.FirstOrDefault(x => x.AppId == appId);

            if (toRemove == null) return;

            steamCredential.SteamCredentialSupportedApps.Remove(toRemove);

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
