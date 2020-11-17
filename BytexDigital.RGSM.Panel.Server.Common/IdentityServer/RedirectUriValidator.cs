using System;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Http;

namespace BytexDigital.RGSM.Panel.Server.Common.IdentityServer
{
    public class RedirectUriValidator : IRedirectUriValidator
    {
        private readonly HttpContext _httpContext;

        public RedirectUriValidator(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (requestedUri.StartsWith("http"))
            {
                try
                {
                    var requestedUriParsed = new Uri(requestedUri);

                    if (requestedUriParsed.Host == _httpContext.Request.Host.Host && requestedUriParsed.Port == _httpContext.Request.Host.Port)
                    {
                        return Task.FromResult(true);
                    }
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (requestedUri.StartsWith("http"))
            {
                try
                {
                    var requestedUriParsed = new Uri(requestedUri);

                    if (requestedUriParsed.Host == _httpContext.Request.Host.Host && requestedUriParsed.Port == _httpContext.Request.Host.Port)
                    {
                        return Task.FromResult(true);
                    }
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
