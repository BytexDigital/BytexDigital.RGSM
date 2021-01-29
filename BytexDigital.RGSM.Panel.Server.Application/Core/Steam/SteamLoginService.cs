using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Steam
{
    public class SteamLoginService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SteamLoginService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<SteamLogin> GetLogins() => _applicationDbContext.SteamLogins;

        public IQueryable<SteamLogin> GetCredentials(string id)
        {
            return _applicationDbContext.SteamLogins.Where(x => x.Id == id);
        }

        public IQueryable<SteamLogin> GetLoginsForApp(long appId, bool requireWorkshopAccess = false)
        {
            if (requireWorkshopAccess)
            {
                return _applicationDbContext.SteamLogins.Where(x => x.SteamLoginSupportedApps.Any(supportedApp => supportedApp.AppId == appId && supportedApp.SupportsWorkshop));
            }

            return _applicationDbContext.SteamLogins.Where(x => x.SteamLoginSupportedApps.Any(supportedApp => supportedApp.AppId == appId));
        }

        public async Task AddSteamLoginAsync(SteamLogin steamCredential)
        {
            var entry = _applicationDbContext.CreateEntity(x => x.SteamLogins);

            entry.Username = steamCredential.Username;
            entry.Password = steamCredential.Password;
            entry.LoginKey = steamCredential.LoginKey;
            entry.Sentry = steamCredential.Sentry;

            _applicationDbContext.SteamLogins.Add(entry);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task AddSupportedAppAsync(SteamLogin steamCredential, long appId, bool supportsWorkshop)
        {
            steamCredential.SteamLoginSupportedApps.Add(new SteamLoginSupportedApp
            {
                AppId = appId,
                SupportsWorkshop = supportsWorkshop
            });

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task RemoveSupportedAppAsync(SteamLogin steamCredential, long appId)
        {
            var toRemove = steamCredential.SteamLoginSupportedApps.FirstOrDefault(x => x.AppId == appId);

            if (toRemove == null) return;

            steamCredential.SteamLoginSupportedApps.Remove(toRemove);

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
