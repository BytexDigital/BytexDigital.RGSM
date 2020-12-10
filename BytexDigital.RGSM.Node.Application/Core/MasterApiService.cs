using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.Common.Errors;
using BytexDigital.RGSM.Node.Application.Helpers;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class MasterApiService
    {
        public const string SYSTEM_ADMINISTRATOR_GROUP_ID = "72056b80-0f35-4b5c-bdac-a143258c0e7c";

        private readonly HttpClient _httpClient;

        public MasterApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResult<List<ApplicationUserGroupDto>>> GetGroupsOfUserAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"/API/Groups/GetUsersGroups?userId={userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    return await response.Content.ReadFromJsonAsync<FailureDetails>();
                }

                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadFromJsonAsync<List<ApplicationUserGroupDto>>();
        }
    }
}
