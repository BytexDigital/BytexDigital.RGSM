using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Models;
using BytexDigital.RGSM.Shared;

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

        public async Task<List<ApplicationUserGroupDto>> GetGroupsOfUserAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"/API/Groups/GetUsersGroups?userId={userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
                }

                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadFromJsonAsync<List<ApplicationUserGroupDto>>();
        }

        public async Task<ApiKeyDetailsDto> GetApiKeyValidityAsync(string key)
        {
            var response = await _httpClient.GetAsync($"/API/Authentication/GetApiKeyValidity?key={key}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
                }

                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadFromJsonAsync<ApiKeyDetailsDto>();
        }
    }
}
