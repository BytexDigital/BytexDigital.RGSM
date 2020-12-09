using BytexDigital.Common.Errors.Exceptions;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerProvidesNoConsoleOutputException : ServiceException
    {
        public ServerProvidesNoConsoleOutputException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(ServerProvidesNoConsoleOutputException))
                .WithMessage("Server does not the implemenet IProvidesConsoleOutput interface.")
                .Build().Errors;
        }
    }
}
