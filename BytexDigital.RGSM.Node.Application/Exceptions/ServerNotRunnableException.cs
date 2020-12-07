
using BytexDigital.Common.Errors.Exceptions;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerNotRunnableException : ServiceException
    {
        public ServerNotRunnableException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(ServerNotRunnableException))
                .WithMessage("Server does not the implemenet IRunnable interface.")
                .Build().Errors;
        }
    }
}
