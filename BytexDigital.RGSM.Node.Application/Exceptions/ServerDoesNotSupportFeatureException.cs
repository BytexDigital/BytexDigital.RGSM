using BytexDigital.ErrorHandling.Shared;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerDoesNotSupportFeatureException<T> : ServiceException
    {
        public ServerDoesNotSupportFeatureException()
        {
            FailureDetails = new ServiceException()
                .AddApplicationError()
                .WithCode(nameof(ServerDoesNotSupportFeatureException<T>))
                .WithDescription($"Server does not support {typeof(T).Name}.")
                .ToBuilder()
                .Build()
                .FailureDetails;
        }
    }
}
