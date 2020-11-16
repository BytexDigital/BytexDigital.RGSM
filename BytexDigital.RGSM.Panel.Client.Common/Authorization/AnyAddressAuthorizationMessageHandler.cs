using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BytexDigital.RGSM.Panel.Client.Common.Authorization
{
    public class AnyAddressAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _provider;
        private readonly NavigationManager _navigation;
        private AccessToken _lastToken;
        private AuthenticationHeaderValue _cachedHeader;
        private AccessTokenRequestOptions _tokenOptions;

        public AnyAddressAuthorizationMessageHandler(
            IAccessTokenProvider provider,
            NavigationManager navigation)
        {
            _provider = provider;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;

            if (_lastToken == null || now >= _lastToken.Expires.AddMinutes(-5))
            {
                var tokenResult = _tokenOptions != null ?
                    await _provider.RequestAccessToken(_tokenOptions) :
                    await _provider.RequestAccessToken();

                if (tokenResult.TryGetToken(out var token))
                {
                    _lastToken = token;
                    _cachedHeader = new AuthenticationHeaderValue("Bearer", _lastToken.Value);
                }
                else
                {
                    throw new AccessTokenNotAvailableException(_navigation, tokenResult, _tokenOptions?.Scopes);
                }
            }

            request.Headers.Authorization = _cachedHeader;

            return await base.SendAsync(request, cancellationToken);
        }

        public AnyAddressAuthorizationMessageHandler ConfigureHandler(
            IEnumerable<string> authorizedUrls,
            IEnumerable<string> scopes = null,
            string returnUrl = null)
        {
            var scopesList = scopes?.ToArray();
            if (scopesList != null || returnUrl != null)
            {
                _tokenOptions = new AccessTokenRequestOptions
                {
                    Scopes = scopesList,
                    ReturnUrl = returnUrl
                };
            }

            return this;
        }
    }
}
