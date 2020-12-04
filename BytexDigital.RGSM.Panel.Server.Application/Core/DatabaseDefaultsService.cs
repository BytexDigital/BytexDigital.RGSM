using System;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BytexDigital.RGSM.Panel.Server.Application.Core
{
    public class DatabaseDefaultsService
    {
        private const string ROOT_DEFAULT_USERNAME = "root";
        private const string ROOT_DEFAULT_PASSWORD = "DefaultRoot!123";

        private readonly ILogger<DatabaseDefaultsService> _logger;
        private readonly ApplicationDbContext _storage;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseDefaultsService(ILogger<DatabaseDefaultsService> logger, ApplicationDbContext storage, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _storage = storage;
            _userManager = userManager;
        }

        public async Task<DatabaseDefaultsService> EnsureSystemAdministratorGroupExistsAsync()
        {
            if (await _storage.Groups.CountAsync(x => x.Name == Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME) > 0) return this;

            var group = _storage.CreateEntity(x => x.Groups);

            group.Name = Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME;
            group.DisplayName = Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_DISPLAYNAME;

            _storage.Groups.Add(group);
            await _storage.SaveChangesAsync();

            return this;
        }

        public async Task<DatabaseDefaultsService> EnsureRootAccountExistsAsync()
        {
            var account = await _userManager.FindByNameAsync(ROOT_DEFAULT_USERNAME);

            if (account != null)
            {
                // Make sure he's in the correct group
                if (!account.Groups.Any(x => x.Group.Name == Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME))
                {
                    await LinkToGroupAsync(account);
                }

                return this;
            }

            account = new ApplicationUser
            {
                UserName = ROOT_DEFAULT_USERNAME
            };

            var creationResult = await _userManager.CreateAsync(account, ROOT_DEFAULT_PASSWORD);

            if (!creationResult.Succeeded)
            {
                _logger.LogError($"The default root account could not be created: {string.Join(Environment.NewLine, creationResult.Errors.Select(x => x.Description))}");

                throw new InvalidOperationException("The default root account could not be created.");
            }

            // Link to group
            await LinkToGroupAsync(account);

            async Task LinkToGroupAsync(ApplicationUser user)
            {
                var groupLink = _storage.CreateEntity(x => x.ApplicationUserGroups);

                groupLink.GroupId = (await _storage.Groups.FirstOrDefaultAsync(x => x.Name == Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME)).Id;
                groupLink.ApplicationUserId = user.Id;

                _storage.ApplicationUserGroups.Add(groupLink);
                await _storage.SaveChangesAsync();
            }

            return this;
        }
    }
}
