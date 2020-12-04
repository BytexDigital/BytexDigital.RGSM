using System;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Shared.Options;
using BytexDigital.RGSM.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class NodeService
    {
        private readonly IOptions<NodeOptions> _nodeOptions;
        private readonly NodeSettingsService _nodeSettingsService;
        private readonly ApplicationDbContext _applicationDbContext;

        public NodeService(IOptions<NodeOptions> nodeOptions, NodeSettingsService nodeSettingsService, ApplicationDbContext applicationDbContext)
        {
            _nodeOptions = nodeOptions;
            _nodeSettingsService = nodeSettingsService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task EnsureLocalSettingsCreatedAsync()
        {
            await _nodeSettingsService.EnsureIdExistsAsync();
        }

        public async Task EnsureNodeRegisteredAsync()
        {
            string nodeId = await GetLocalNodeIdAsync();

            if (!await GetNode(nodeId).AnyAsync())
            {
                // Register this node in the shared database so that the panel knows 
                await CreateFromLocalNodeAsync();
            }
        }

        public IQueryable<RGSM.Domain.Entities.Node> GetNode(string id)
        {
            return _applicationDbContext.Nodes.Where(x => x.Id == id);
        }

        public async Task<IQueryable<RGSM.Domain.Entities.Node>> GetLocalNodeAsync() => GetNode(await GetLocalNodeIdAsync());

        public async Task<IQueryable<RGSM.Domain.Entities.Node>> CreateFromLocalNodeAsync()
        {
            string nodeId = await GetLocalNodeIdAsync();

            if (await GetNode(await GetLocalNodeIdAsync()).AnyAsync()) throw new InvalidOperationException("The local node already exists in the shared database.");

            var node = _applicationDbContext.CreateEntity(x => x.Nodes);

            node.Id = nodeId;
            node.Name = $"network-node-{nodeId}";
            node.DisplayName = $"Node {nodeId}";
            node.BaseUri = _nodeOptions.Value.BaseUri;

            _applicationDbContext.Nodes.Add(node);
            await _applicationDbContext.SaveChangesAsync();

            return GetNode(await GetLocalNodeIdAsync());
        }

        public async Task<string> GetLocalNodeIdAsync()
        {
            return await _nodeSettingsService.GetValueAsync<string>(NodeSettingsService.KEY_SETTING_NODEID);
        }
    }
}
