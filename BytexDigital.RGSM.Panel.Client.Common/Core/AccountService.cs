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
            return await _httpClient.GetFromJsonAsync<List<GroupDto>>("/API/Accounts/GetAssignedGroups");
        }
    }
}
