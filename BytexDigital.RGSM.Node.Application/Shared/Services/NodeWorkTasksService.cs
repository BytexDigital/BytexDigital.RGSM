using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class NodeWorkTasksService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public NodeWorkTasksService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IQueryable<WorkTask>> CreateWorkTaskAsync(Server server, string code, string description = "No description provided.")
        {
            var task = _applicationDbContext.CreateEntity(x => x.Tasks);

            task.Code = code;
            task.ServerId = server.Id;
            task.Description = description;
            task.Status = RGSM.Domain.Enumerations.WorkTaskStatus.Running;

            _applicationDbContext.Tasks.Add(task);
            await _applicationDbContext.SaveChangesAsync();

            return _applicationDbContext.Tasks.Where(x => x.Id == task.Id);
        }

        public async Task<bool> TaskIsRunningAsync(Server server, string code) =>
            await _applicationDbContext.Tasks.AnyAsync(x => x.ServerId == server.Id && x.Code == code && x.Status == RGSM.Domain.Enumerations.WorkTaskStatus.Running);
    }
}
