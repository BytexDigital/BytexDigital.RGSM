
using BytexDigital.Common.Errors.Exceptions;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerStateNotFoundException : ServiceException
    {
        public ServerStateNotFoundException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(ServerStateNotFoundException))
                .WithMessage("Server not found.")
                .Build().Errors;
        }
    }
}
