using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Persistence;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ServerSetupService
    {
        private readonly NodeDbContext _nodeDbContext;

        public ServerSetupService(NodeDbContext nodeDbContext)
        {
            _nodeDbContext = nodeDbContext;
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
            if (server.SchedulerPlan == null)
            {
                server.SchedulerPlan = _nodeDbContext.CreateEntity(x => x.SchedulerPlans);
                server.SchedulerPlan.IsEnabled = false;
            }

            await _nodeDbContext.SaveChangesAsync();
        }
    }
}
