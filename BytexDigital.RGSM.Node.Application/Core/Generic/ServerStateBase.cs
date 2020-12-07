using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public abstract class ServerStateBase
    {
        public IMediator Mediator { get; }
        public string Id { get; }
        public string Directory { get; }

        public ServerStateBase(IMediator mediator, string id, string directory)
        {
            Mediator = mediator;
            Id = id;
            Directory = directory;
        }
    }
}
