using System.Threading;
using System.Threading.Tasks;

using Autofac;

using BytexDigital.RGSM.Node.Application.Core;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Mediator
{
    public class ScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ScopeService _scopeService;
        private readonly ILifetimeScope _lifetimeScope;

        public ScopeBehavior(ScopeService scopeService, ILifetimeScope lifetimeScope)
        {
            _scopeService = scopeService;
            _lifetimeScope = lifetimeScope;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            //return await _scopeService.RunScoped(() => next.Invoke());
            if (_lifetimeScope.Tag == (object)"Internal") return await next.Invoke();

            using (var scope = _lifetimeScope.BeginLifetimeScope((object)"Internal"))
            {
                return (TResponse)await scope.Resolve<IMediator>().Send(request, cancellationToken);
            }
        }
    }
}
