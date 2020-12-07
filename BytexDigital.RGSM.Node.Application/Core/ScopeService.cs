using System;

using Autofac;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ScopeService
    {
        public const string LAYER_SCOPE_NAME = "InternalScope";

        private readonly ILifetimeScope _lifetimeScope;

        public ScopeService(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void RunScoped(Action action)
        {
            if (_lifetimeScope.Tag == (object)LAYER_SCOPE_NAME) action.Invoke();

            using (var scope = _lifetimeScope.BeginLifetimeScope(LAYER_SCOPE_NAME))
            {
                action.Invoke();
            }
        }

        public T RunScoped<T>(Func<T> action)
        {
            if (_lifetimeScope.Tag == (object)LAYER_SCOPE_NAME) return action.Invoke();

            using (var scope = _lifetimeScope.BeginLifetimeScope(LAYER_SCOPE_NAME))
            {
                return action.Invoke();
            }
        }
    }
}
