using System.Linq;
using System.Security.Claims;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts
{
    public class AccountService
    {
        private readonly ApplicationDbContext _storage;

        public AccountService(ApplicationDbContext storage)
        {
            _storage = storage;
        }

        public IQueryable<ApplicationUser> GetUser(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            return _storage.Users.Where(x => x.Id == userId);
        }

        public IQueryable<ApplicationUser> GetUser(string id)
            => _storage.Users.Where(x => x.Id == id);

        public IQueryable<Group> GetAssignedGroups(ApplicationUser applicationUser)
        {
            return _storage.Groups.Where(x => x.Users.Any(y => y.ApplicationUserId == applicationUser.Id));
        }

        public IQueryable<ApplicationUserGroup> GetAssignedGroupLinks(ApplicationUser applicationUser)
        {
            return _storage.ApplicationUserGroups.Where(x => x.ApplicationUserId == applicationUser.Id);
        }
    }
}
