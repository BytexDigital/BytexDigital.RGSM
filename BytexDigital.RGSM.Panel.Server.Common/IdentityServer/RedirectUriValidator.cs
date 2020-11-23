using System;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BytexDigital.RGSM.Panel.Server.Common.IdentityServer
{
    public class RedirectUriValidator : IRedirectUriValidator
    {
        private readonly HttpContext _httpContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RedirectUriValidator(IHttpContextAccessor httpContext, IWebHostEnvironment webHostEnvironment)
        {
            _httpContext = httpContext.HttpContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (_webHostEnvironment.IsDevelopment()) return Task.FromResult(true);

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
            if (_webHostEnvironment.IsDevelopment()) return Task.FromResult(true);

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
