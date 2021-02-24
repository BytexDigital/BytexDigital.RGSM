using System;
using System.IO;
using System.Threading.Tasks;

using MediatR;

using Serilog;
using Serilog.Core;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public abstract class ServerStateBase : IAsyncDisposable
    {
        public IMediator Mediator { get; }
        public string Id { get; }
        public string BaseDirectory { get; }
        public Logger Logger { get; set; }

        public ServerStateBase(IMediator mediator, string id, string directory)
        {
            Mediator = mediator;
            Id = id;
            BaseDirectory = directory;

            Logger = new LoggerConfiguration()
                .WriteTo.Logger(logger => logger.WriteTo.File(Path.Combine(BaseDirectory, ".rgsm", "logs", "server.log"), rollingInterval: RollingInterval.Day))
                .CreateLogger();
        }



        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task ShutdownAsync()
        {
            await DisposeAsync();
        }

        public virtual async Task PreserveConfigurationAsync()
        {
            await Task.CompletedTask;
        }

        public virtual ValueTask DisposeAsync()
        {
            Logger.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}
