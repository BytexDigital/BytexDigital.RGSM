using System.IO;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Node.Application.Shared.Services;

namespace BytexDigital.RGSM.Node.Application.Games.Arma3.Services
{
    public class StatusService
    {
        public const string TASK_SERVER_UPDATING = nameof(TASK_SERVER_UPDATING);

        private readonly NodeWorkTasksService _nodeWorkTasksService;

        public StatusService(NodeWorkTasksService nodeWorkTasksService)
        {
            _nodeWorkTasksService = nodeWorkTasksService;
        }

        public async Task<bool> CanServerChangeStatusAsync(Server server)
        {
            if (!File.Exists(Path.Combine(server.Directory, "arma3server.exe")) && !File.Exists(Path.Combine(server.Directory, "arma3server_x64.exe")))
                return false;

            if (await _nodeWorkTasksService.TaskIsRunningAsync(server, TASK_SERVER_UPDATING)) return false;

            return true;
        }

        public bool CanServerUpdate(Server server)
        {
            return server.Status == RGSM.Domain.Enumerations.ServerStatus.Stopped;
        }
    }
}
