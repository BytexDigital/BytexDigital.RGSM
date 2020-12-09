
using BytexDigital.Common.Errors.Exceptions;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerDoesNotSupportFeatureException<T> : ServiceException
    {
        public ServerDoesNotSupportFeatureException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(ServerDoesNotSupportFeatureException<T>))
                .WithMessage($"Server does not support {typeof(T).Name}.")
                .Build().Errors;
        }
    }
}
