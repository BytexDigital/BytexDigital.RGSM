using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Commands;

using MediatR;

using Microsoft.Extensions.Hosting;

namespace BytexDigital.RGSM.Node.Application.Core.Infrastructure
{
    public class ConfigurationPreservationService : IHostedService
    {
        private Timer _timer;
        private readonly IMediator _mediator;

        public ConfigurationPreservationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public async void RunAsync(object state)
        {
            try
            {
                await _mediator.Send(new PerformConfigurationPreservationCmd());
            }
            catch
            {
                // Logging?
            }
        }
    }
}
