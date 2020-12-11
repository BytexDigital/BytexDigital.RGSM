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
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public const string HEADER_NAME = "X-API-Key";
        private readonly IMediator _mediator;
        private readonly AuthenticationService _authenticationService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
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

            var keyValue = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(keyValue))
            {
                return AuthenticateResult.NoResult();
            }

            var apiKey = await _authenticationService.GetApiKey(keyValue).FirstOrDefaultAsync();

            if (apiKey == null)
            {
                return AuthenticateResult.NoResult();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey.Id),
                new Claim("scope", "rgsm.app")
            };

            var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.NODE_AUTHENTICATION_SCHEME);
            var principal = new ClaimsPrincipal(new[] { identity });
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.NODE_AUTHENTICATION_SCHEME);

            return AuthenticateResult.Success(ticket);
        }
    }
}
