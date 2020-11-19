using BytexDigital.RGSM.Persistence;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class ServerService
    {
        private readonly ApplicationDbContext _storage;

        public ServerService(ApplicationDbContext storage)
        {
            _storage = storage;
        }


    }
}
