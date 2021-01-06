using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class PermissionService
    {
        private readonly HttpClient _httpClient;

        public PermissionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PermissionDto>> GetPermissionsAsync(NodeDto node, ServerDto server)
        {
            var response = await _httpClient.GetAsync($"{node.BaseUri}/API/Permissions/{server.Id}/GetPermissions");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            return await response.Content.ReadFromJsonAsync<List<PermissionDto>>();
        }

        public async Task SavePermissionAsync(NodeDto nodeDto, ServerDto serverDto, GroupDto groupDto, PermissionDto permissionDto, bool value)
        {
            var parameters = $"?permissionName={permissionDto.Name}&groupId={groupDto.Id}&addOrRemove={value}";
            var response = await _httpClient.PostAsync($"{nodeDto.BaseUri}/API/Permissions/{serverDto.Id}/AddOrRemoveGroupFromPermission{parameters}", null);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
