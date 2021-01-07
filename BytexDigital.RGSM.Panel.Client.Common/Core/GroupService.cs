using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class GroupService
    {
        private readonly HttpClient _httpClient;

        public GroupService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GroupDto>>($"/API/Groups/GetGroups");
        }

        public async Task<GroupDto> CreateGroupAsync(GroupDto groupDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/API/Groups/CreateGroup", groupDto);

            await response.ThrowIfInvalidAsync();

            return await response.Content.ReadFromJsonAsync<GroupDto>();
        }

        public async Task DeleteGroupAsync(GroupDto groupDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/API/Groups/DeleteGroup", groupDto);

            await response.ThrowIfInvalidAsync();
        }

        public async Task<GroupDto> GetGroupAsync(string groupId)
        {
            var response = await _httpClient.GetAsync($"/API/Groups/GetGroup/{groupId}");

            await response.ThrowIfInvalidAsync();

            return await response.Content.ReadFromJsonAsync<GroupDto>();
        }

        public async Task UpdateGroupAsync(GroupDto groupDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"/API/Groups/UpdateGroup", groupDto);

            await response.ThrowIfInvalidAsync();
        }
    }
}
