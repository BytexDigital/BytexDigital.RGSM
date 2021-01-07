using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

using Microsoft.AspNetCore.Identity;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts
{
    public class AccountService
    {
        private readonly ApplicationDbContext _storage;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(ApplicationDbContext storage, UserManager<ApplicationUser> userManager)
        {
            _storage = storage;
            _userManager = userManager;
        }

        public async Task<List<string>> CheckPasswordForRequirementErrorsAsync(string password)
        {
            var errors = new List<string>();

            foreach (var passwordValidator in _userManager.PasswordValidators)
            {
                var validationResult = await passwordValidator.ValidateAsync(_userManager, null, password);

                if (!validationResult.Succeeded)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        errors.Add(error.Description);
                    }
                }
            }

            return errors;
        }

        public async Task<IQueryable<ApplicationUser>> CreateApplicationUserAsync(string username, string password)
        {
            var account = new ApplicationUser
            {
                UserName = username
            };

            var creationResult = await _userManager.CreateAsync(account, password);

            if (!creationResult.Succeeded) throw new InvalidOperationException("Could not create user.");

            return GetApplicationUserById(account.Id);
        }

        public async Task DeleteApplicationUserAsync(ApplicationUser applicationUser)
        {
            var deleteResult = await _userManager.DeleteAsync(applicationUser);

            if (!deleteResult.Succeeded) throw new InvalidOperationException("Could not delete user.");
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

        public IQueryable<ApplicationUser> GetApplicationUsers()
        {
            return _storage.Users;
        }

        public IQueryable<ApplicationUser> GetApplicationUserById(string id)
        {
            return _storage.Users.Where(x => x.Id.ToLower() == id.ToLower());
        }

        public IQueryable<ApplicationUser> GetApplicationUserByUserName(string userName)
        {
            return _storage.Users.Where(x => x.UserName.ToLower() == userName.ToLower());
        }

        public async Task UpdateApplicationUserGroupAffinityAsync(ApplicationUser applicationUser, Group group, bool isInGroup)
        {
            if (isInGroup)
            {
                if (applicationUser.Groups.Any(x => x.Id == group.Id)) return;

                var link = _storage.CreateEntity(x => x.ApplicationUserGroups);

                link.ApplicationUserId = applicationUser.Id;
                link.GroupId = group.Id;

                _storage.ApplicationUserGroups.Add(link);
                await _storage.SaveChangesAsync();
            }
            else
            {
                if (!applicationUser.Groups.Any(x => x.Id == group.Id)) return;

                _storage.ApplicationUserGroups.Remove(applicationUser.Groups.First(x => x.Id == group.Id));
                await _storage.SaveChangesAsync();
            }
        }
    }
}
