using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Authentication
{
    public class NodeAuthenticationHandler : AuthenticationHandler<NodeAuthenticationOptions>
    {
        public const string HEADER_NAME = "Node-Api-Key";
        private readonly IMediator _mediator;
        private readonly AuthenticationService _authenticationService;

        public NodeAuthenticationHandler(
            IOptionsMonitor<NodeAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMediator mediator,
            AuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            _mediator = mediator;
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HEADER_NAME, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            var node = await _authenticationService.GetNodeByApiKey(providedApiKey).FirstOrDefaultAsync();

            if (node == null)
            {
                return AuthenticateResult.NoResult();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, node.Id),
                new Claim("scope", "rgsm.app"),
                new Claim("scope", "rgsm.node")
            };

            var identity = new ClaimsIdentity(claims, NodeAuthenticationOptions.NODE_AUTHENTICATION_SCHEME);
            var principal = new ClaimsPrincipal(new[] { identity });
            var ticket = new AuthenticationTicket(principal, NodeAuthenticationOptions.NODE_AUTHENTICATION_SCHEME);

            return AuthenticateResult.Success(ticket);
        }
    }
}
