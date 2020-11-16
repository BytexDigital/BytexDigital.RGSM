using System.Collections.Generic;

using IdentityServer4.Extensions;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        private readonly ILogger<OidcConfigurationController> _logger;

        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger<OidcConfigurationController> logger)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            _logger = logger;
        }

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);

            //return Ok(new Dictionary<string, string>
            //{
            //    { "authority", HttpContext.GetIdentityServerIssuerUri() },
            //    { "client_id", clientId },
            //    { "redirect_uri", "/authentication/login-callback" },
            //    { "post_logout_redirect_uri", "/authentication/logout-callback" },
            //    { "response_type", "code" },
            //    { "scope", "openid profile rgsm" }
            //}); ;
        }
    }
}
