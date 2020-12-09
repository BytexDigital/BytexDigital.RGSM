using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class SteamProvidesNoBattlEyeRconException : ServiceException
    {
        public SteamProvidesNoBattlEyeRconException()
        {
            Errors = new ServiceException()
                .WithCode(nameof(SteamProvidesNoBattlEyeRconException))
                .WithMessage($"Server does not the implemenet {nameof(IBattlEyeRcon)} interface.")
                .Build().Errors;
        }
    }
}
