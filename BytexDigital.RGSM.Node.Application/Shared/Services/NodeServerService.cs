using BytexDigital.RGSM.Persistence;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class NodeServerService
    {
        private readonly ApplicationDbContext _storage;

        public NodeServerService(ApplicationDbContext storage)
        {
            _storage = storage;
        }


    }
}
