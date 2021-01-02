using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Arma3;
using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Shared;
using BytexDigital.RGSM.Shared.Enumerations;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ServerIntegrityService
    {
        private readonly NodeDbContext _nodeDbContext;
        private readonly ArmaServerService _armaServerService;

        public ServerIntegrityService(NodeDbContext nodeDbContext, ArmaServerService armaServerService)
        {
            _nodeDbContext = nodeDbContext;
            _armaServerService = armaServerService;
        }

        public async Task EnsureCorrectSetupAllAsync()
        {
            foreach (var server in _nodeDbContext.Servers)
            {
                await EnsureCorrectSetupAsync(server);
            }
        }

        public async Task EnsureCorrectSetupAsync(Server server)
        {
            // Default scheduler plan entity exists
            if (server.SchedulerPlan == null)
            {
                server.SchedulerPlan = _nodeDbContext.CreateEntity(x => x.SchedulerPlans);
                server.SchedulerPlan.IsEnabled = false;
            }

            await _nodeDbContext.SaveChangesAsync();

            // Permissions exist
            await EnsurePermissionsUpdatedAsync(server);

            // Game specific checks
            switch (server.Type)
            {
                case ServerType.Arma3:
                {
                    await _armaServerService.EnsureCorrectSetupAsync(server);
                }
                break;

                case ServerType.DayZ:
                    break;

                default:
                    break;
            }
        }

        public async Task EnsurePermissionsUpdatedAsync(Server server)
        {
            var permissions = PermissionConstants.Permissions.GetValueOrDefault(server.Type);

            // Delete permissions that don't exist anymore
            server.Permissions.Where(x => !permissions.Keys.Contains(x.Name)).ToList().ForEach(x => server.Permissions.Remove(x));

            foreach (var permissionConfig in permissions)
            {
                // Key = name
                var name = permissionConfig.Key;

                // Value = description
                var description = permissionConfig.Value;

                var permission = server.Permissions.FirstOrDefault(x => x.Name == name);

                if (permission == null)
                {
                    permission = _nodeDbContext.CreateEntity(x => x.Permissions); ;
                    server.Permissions.Add(permission);
                }

                permission.Name = name;
                permission.Description = description;
            }

            await _nodeDbContext.SaveChangesAsync();
        }
    }
}
