using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Games.Arma3;
using BytexDigital.RGSM.Node.Application.Games.Shared;
using BytexDigital.RGSM.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class ServerContainerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, ServerContainerBase> _localContainers;

        public ServerContainerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _localContainers = new ConcurrentDictionary<string, ServerContainerBase>();
        }

        public async Task CreateLocalInstancesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
                var node = await nodeService.GetNode(await nodeService.GetLocalNodeIdAsync()).FirstAsync();
                var servers = node.Servers;

                foreach (var server in servers)
                {
                    if (_localContainers.ContainsKey(server.Id)) continue;

                    ServerContainerBase instance = server.Type switch
                    {
                        RGSM.Domain.Enumerations.ServerType.Arma3 => new Arma3ServerContainer(),
                        RGSM.Domain.Enumerations.ServerType.DayZ => throw new NotImplementedException(),
                        _ => throw new NotImplementedException()
                    };

                    instance.GlobalId = server.Id;
                    instance.Directory = server.Directory;

                    _ = _localContainers.TryAdd(server.Id, instance);
                }
            }
        }

        public T GetInstanceOrDefault<T>(string id, T defaultValue = default) where T : ServerContainerBase
        {
            _localContainers.TryGetValue(id, out var ret);

            return (T)ret ?? defaultValue;
        }
    }
}
