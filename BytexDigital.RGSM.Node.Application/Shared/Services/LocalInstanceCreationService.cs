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
    public class LocalInstanceCreationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, ServerBase> _localServerInstances;

        public LocalInstanceCreationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _localServerInstances = new ConcurrentDictionary<string, ServerBase>();
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
                    if (_localServerInstances.ContainsKey(server.Id)) continue;

                    ServerBase instance = server.Type switch
                    {
                        RGSM.Domain.Enumerations.ServerType.Arma3 => new LocalArma3Server(),
                        RGSM.Domain.Enumerations.ServerType.DayZ => throw new NotImplementedException(),
                        _ => throw new NotImplementedException()
                    };

                    instance.GlobalId = server.Id;

                    _ = _localServerInstances.TryAdd(server.Id, instance);
                }
            }
        }
    }
}
