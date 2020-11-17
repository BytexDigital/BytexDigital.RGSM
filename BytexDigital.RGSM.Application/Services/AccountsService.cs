using System.Linq;
using System.Security.Claims;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Persistence;

namespace BytexDigital.RGSM.Application.Services
{
    public class AccountsService
    {
        private readonly ApplicationDbContext _storage;

        public AccountsService(ApplicationDbContext storage)
        {
            _storage = storage;
        }

        public IQueryable<ApplicationUser> GetUser(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            return _storage.Users.Where(x => x.Id == userId);
        }

        public IQueryable<Group> GetAssignedGroups(ApplicationUser applicationUser)
        {
            return _storage.Groups.Where(x => x.Users.Any(y => y.ApplicationUserId == applicationUser.Id));
        }
    }
}
