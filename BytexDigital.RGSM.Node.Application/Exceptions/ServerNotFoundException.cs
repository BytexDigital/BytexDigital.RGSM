using BytexDigital.ErrorHandling.Shared;


namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class ServerNotFoundException : ServiceException
    {
        public ServerNotFoundException()
        {
            FailureDetails = new ServiceExceptionBuilder()
                .AddApplicationError()
                .WithCode(nameof(ServerNotFoundException))
                .WithDescription("Server not found.")
                .ToBuilder()
                .Build()
                .FailureDetails;
        }
    }
}
