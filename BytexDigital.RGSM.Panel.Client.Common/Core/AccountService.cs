using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string GetIdentifier(ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

        public async Task<List<GroupDto>> GetAssignedGroupsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GroupDto>>("/API/Accounts/GetSessionApplicationUsersGroups");
        }

        public async Task<List<ApplicationUserDto>> GetApplicationUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ApplicationUserDto>>("/API/Accounts/GetApplicationUsers");
        }

        public async Task<ApplicationUserDto> CreateApplicationUserAsync(ApplicationUserDto applicationUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/API/Accounts/CreateApplicationUser", applicationUserDto);

            await response.ThrowIfInvalidAsync();

            return await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
        }

        public async Task DeleteApplicationUserAsync(ApplicationUserDto applicationUserDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/API/Accounts/DeleteApplicationUser", applicationUserDto);

            await response.ThrowIfInvalidAsync();
        }

        public async Task UpdateApplicationUserGroupAffinityAsync(ApplicationUserDto applicationUserDto, GroupDto groupDto, bool isInGroup)
        {
            var response = await _httpClient.PostAsJsonAsync($"/API/Accounts/UpdateApplicationUserGroupAffinity?isInGroup={isInGroup}", new ApplicationUserGroupDto
            {
                ApplicationUserId = applicationUserDto.Id,
                GroupId = groupDto.Id
            });

            await response.ThrowIfInvalidAsync();
        }
    }
}
