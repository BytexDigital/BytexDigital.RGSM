using Microsoft.AspNetCore.Authentication;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string NODE_AUTHENTICATION_SCHEME = "Node Api Key";
    }
}
