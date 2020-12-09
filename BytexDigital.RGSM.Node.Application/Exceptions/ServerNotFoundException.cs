
using BytexDigital.Common.Errors.Exceptions;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerNotFoundException : ServiceException
    {
        public ServerNotFoundException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(ServerNotFoundException))
                .WithMessage("Server not found.")
                .Build().Errors;
        }
    }
}
