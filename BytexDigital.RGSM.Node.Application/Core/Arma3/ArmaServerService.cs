using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Persistence;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaServerService
    {
        private readonly NodeDbContext _nodeDbContext;

        public ArmaServerService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
        }

        public IQueryable<Arma3Server> GetServer(string id) => _nodeDbContext.Arma3Server.Where(x => x.Server.Id == id);

        public async Task MarkAsInstalledAsync(Arma3Server arma3Server)
        {
            arma3Server.IsInstalled = true;

            await _nodeDbContext.SaveChangesAsync();
        }

        public async Task EnsurePermissionsExistAsync(Arma3Server server, CancellationToken cancellationToken)
        {
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.STARSTOP, "Start and stop the server.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.FILEBROWSER_READ, "Read files from disk and download them.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.FILEBROWSER_WRITE, "Write files to disk.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.UPDATE, "Update the server.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.WORKSHOP, "Manage workshop mods.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.SCHEDULER, "Manage scheduler.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.BE_RCON_READ, "Read RCON messages.");
            await CreatePermissionOrUpdateAsync(server.Server, PermissionConstants.BE_RCON_SEND, "Send RCON messages.");

            await _nodeDbContext.SaveChangesAsync();
        }

        private Task CreatePermissionOrUpdateAsync(Server server, string name, string description)
        {
            var permission = server.Permissions.FirstOrDefault(x => x.Name == name);

            if (permission == null)
            {
                permission = _nodeDbContext.CreateEntity(x => x.Permissions); ;
                server.Permissions.Add(permission);
            }

            permission.Name = name;
            permission.Description = description;

            return Task.CompletedTask;
        }
    }
}
