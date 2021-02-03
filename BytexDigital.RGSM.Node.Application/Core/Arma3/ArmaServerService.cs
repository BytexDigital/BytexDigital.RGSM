using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Models.Arma;
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

        //public IQueryable<ArmaServer> GetServer(string id) => _nodeDbContext.Arma3Server.AsQueryable().Where(x => x.Server.Id == id);

        public async Task EnsureCorrectSetupAsync(Server server)
        {
            //// Validate depot ids
            //List<long> depotIds = new List<long> { 233781, 233782 };

            //server.TrackedDepots.Where(x => !depotIds.Contains(x.DepotId)).ToList().ForEach(x => server.TrackedDepots.Remove(x));

            //foreach (var depotId in depotIds)
            //{
            //    if (server.TrackedDepots.Any(x => x.DepotId == depotId)) continue;

            //    // Add depots required for SteamCmd support
            //    var contentDepot = _nodeDbContext.CreateEntity(x => x.Depots);
            //    contentDepot.DepotId = depotId;

            //    server.TrackedDepots.Add(contentDepot);
            //}

            //await _nodeDbContext.SaveChangesAsync();
        }

        public async Task MarkAsInstalledAsync(ArmaServer arma3Server)
        {
            arma3Server.IsInstalled = true;

            await _nodeDbContext.SaveChangesAsync();
        }

        public async Task UpdateSettingsAsync(
            ArmaServer armaServer,
            int? appId,
            string branch,
            string executableFileName,
            string profilesPath,
            string battlEyePath,
            string rconIp,
            int rconPort,
            string rconPassword,
            string arguments)
        {
            armaServer.AppId = appId;
            armaServer.Branch = branch;
            armaServer.ExecutableFileName = executableFileName;
            armaServer.ProfilesPath = profilesPath;
            armaServer.BattlEyePath = battlEyePath;
            armaServer.RconIp = rconIp;
            armaServer.RconPort = rconPort;
            armaServer.RconPassword = rconPassword;
            armaServer.AdditionalArguments = arguments;

            await _nodeDbContext.SaveChangesAsync();
        }
    }
}
