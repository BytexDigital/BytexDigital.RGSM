using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Shared;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class PermissionsService
    {
        private readonly NodeDbContext _nodeDbContext;

        public PermissionsService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<Permission> GetPermission(Server server, string name) =>
            _nodeDbContext.Permissions.Where(x => x.ServerId == server.Id && x.Name == name);

        public IQueryable<Permission> GetPermissionsOfGroups(IEnumerable<string> groupIds) =>
            _nodeDbContext.Permissions.Where(x => x.GroupReferences.Any(g => groupIds.Contains(g.GroupId)));

        public IQueryable<Permission> GetPermissionsOfGroup(string groupId)
        {
            if (groupId == MasterApiService.SYSTEM_ADMINISTRATOR_GROUP_ID)
            {
                return _nodeDbContext.Permissions;
            }
            else
            {
                return _nodeDbContext.Permissions.Where(x => x.GroupReferences.Any(g => g.GroupId == groupId));
            }
        }

        public IQueryable<Permission> GetPermissionsOfGroup(Server server, string groupId)
        {
            if (groupId == MasterApiService.SYSTEM_ADMINISTRATOR_GROUP_ID)
            {
                return _nodeDbContext.Permissions.Where(x => x.ServerId == server.Id);
            }
            else
            {
                return _nodeDbContext.Permissions.Where(x => x.ServerId == server.Id && x.GroupReferences.Any(g => g.GroupId == groupId));
            }
        }

        public async Task AddOrRemoveGroupFromPermissionAsync(Permission permission, bool addOrRemove, string groupId)
        {
            if (addOrRemove)
            {
                if (permission.GroupReferences.Any(x => x.GroupId == groupId)) return;

                var groupRef = _nodeDbContext.CreateEntity(x => x.GroupReferences);
                groupRef.GroupId = groupId;

                permission.GroupReferences.Add(groupRef);

                await _nodeDbContext.SaveChangesAsync();
            }
            else
            {
                var groupRef = permission.GroupReferences.FirstOrDefault(x => x.GroupId == groupId);

                if (groupRef == null) return;

                permission.GroupReferences.Remove(groupRef);

                await _nodeDbContext.SaveChangesAsync();
            }
        }
    }
}
