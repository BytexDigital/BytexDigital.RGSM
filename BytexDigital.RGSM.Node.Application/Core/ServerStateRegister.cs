using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Autofac;

using BytexDigital.RGSM.Node.Application.Core.Arma3;
using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Enumerations;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ServerStateRegister
    {
        private readonly ConcurrentDictionary<string, ServerStateBase> _serverStates;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IMediator _mediator;

        public ServerStateRegister(ILifetimeScope lifetimeScope, IMediator mediator)
        {
            _serverStates = new ConcurrentDictionary<string, ServerStateBase>();
            _lifetimeScope = lifetimeScope;
            _mediator = mediator;
        }

        public async Task InitializeAsync()
        {
            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                var serversService = scope.Resolve<ServersService>();

                foreach (var server in await serversService.GetServers().ToListAsync())
                {
                    Register(server);
                }
            }
        }

        public void Register(Server server)
        {
            var state = server.Type switch
            {
                ServerType.Arma3 => new ArmaServerState(_mediator, server.Id, server.Directory),
                _ => throw new NotImplementedException()
            };

            _ = _serverStates.TryAdd(server.Id, state);
        }

        public void Unregister(Server server)
        {
            _ = _serverStates.TryRemove(server.Id, out _);
        }

        public ServerStateBase GetServerState(string id)
        {
            _ = _serverStates.TryGetValue(id, out var state);

            return state;
        }
    }
}
