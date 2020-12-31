using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;

using FluentValidation;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.BattlEye
{

    public class SendRconMessageCmd : IRequest
    {
        public string Id { get; set; }
        public string Message { get; set; }

        public class Handler : IRequestHandler<SendRconMessageCmd>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Unit> Handle(SendRconMessageCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IBattlEyeRcon beRconState) throw new ServerDoesNotSupportFeatureException<IBattlEyeRcon>();

                await beRconState.SendBeRconMessageAsync(request.Message, cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<SendRconMessageCmd>
        {
            public Validator()
            {
                RuleFor(x => x.Message)
                    .NotEmpty();
            }
        }
    }
}
