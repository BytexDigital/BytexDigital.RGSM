using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Mediator
{
    public class ScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ScopeService _scopeService;

        public ScopeBehavior(ScopeService scopeService)
        {
            _scopeService = scopeService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return await _scopeService.RunScoped(() => next.Invoke());
        }
    }
}
