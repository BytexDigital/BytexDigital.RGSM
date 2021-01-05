using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

namespace BytexDigital.RGSM.Panel.Server.Application.Core
{
    public class GroupService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GroupService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<Group> GetGroups()
        {
            return _applicationDbContext.Groups;
        }

        public IQueryable<Group> GetGroup(string id)
        {
            return _applicationDbContext.Groups.Where(x => x.Id == id);
        }

        public IQueryable<Group> GetGroupByName(string name)
        {
            return _applicationDbContext.Groups.Where(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<IQueryable<Group>> CreateGroupAsync(Group group)
        {
            var newGroup = _applicationDbContext.CreateEntity(x => x.Groups);
            newGroup.DisplayName = group.DisplayName;
            newGroup.Name = group.Name;

            _applicationDbContext.Groups.Add(newGroup);

            await _applicationDbContext.SaveChangesAsync();

            return GetGroup(newGroup.Id);
        }

        public async Task UpdateGroupAsync(Group group, Group updatedFields)
        {
            group.DisplayName = updatedFields.DisplayName;
            group.Name = updatedFields.Name;

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(Group group)
        {
            _applicationDbContext.Groups.Remove(group);

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
