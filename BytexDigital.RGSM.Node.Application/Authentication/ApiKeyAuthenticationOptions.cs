using Microsoft.AspNetCore.Authentication;

namespace BytexDigital.RGSM.Node.Application.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string NODE_AUTHENTICATION_SCHEME = "Node Api Key";
    }
}
