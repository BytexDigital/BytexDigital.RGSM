﻿
using System;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core
{
    public class NodeService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public NodeService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IQueryable<Domain.Entities.Node>> RegisterNodeAsync(string baseUri, string name, string displayName)
        {
            var node = _applicationDbContext.CreateEntity(x => x.Nodes);

            node.BaseUri = baseUri;
            node.Name = name;
            node.DisplayName = displayName;
            node.ApiKey = _applicationDbContext.CreateEntity(x => x.ApiKeys);
            node.ApiKey.Value = Guid.NewGuid().ToString();

            _applicationDbContext.Nodes.Add(node);
            await _applicationDbContext.SaveChangesAsync();

            return GetNode(node.Id);
        }

        public async Task UpdateNodeAsync(Domain.Entities.Node node, string name, string displayName, string baseUri)
        {
            node.Name = name;
            node.DisplayName = displayName;
            node.BaseUri = baseUri;

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task UnregisterNodeAsync(Domain.Entities.Node node)
        {
            _applicationDbContext.ApiKeys.Remove(node.ApiKey);
            _applicationDbContext.Nodes.Remove(node);
           
            await _applicationDbContext.SaveChangesAsync();
        }

        public IQueryable<Domain.Entities.Node> GetNode(string id) => _applicationDbContext.Nodes.Where(x => x.Id == id);

        public IQueryable<Domain.Entities.ApiKey> GetNodeKey(string id) => _applicationDbContext.ApiKeys.Where(x => x.Node.Id == id);

        public IQueryable<Domain.Entities.Node> GetNodes() => _applicationDbContext.Nodes;

        public IQueryable<Domain.Entities.Node> GetNodeByName(string name) => _applicationDbContext.Nodes.Where(x => x.Name == name);
    }
}
