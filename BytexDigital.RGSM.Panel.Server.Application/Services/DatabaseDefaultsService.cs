using System;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BytexDigital.RGSM.Panel.Server.Application.Services
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

        public async Task<DatabaseDefaultsService> EnsureRootAccountExistsAsync()
        {
            var account = await _userManager.FindByNameAsync(ROOT_DEFAULT_USERNAME);

            if (account != null) return this;

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

            return this;
        }
    }
}
