using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Shared;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Groups.Commands
{
    public class UpdateGroupCmd : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public class Handler : IRequestHandler<UpdateGroupCmd>
        {
            private readonly GroupService _groupsService;

            public Handler(GroupService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Unit> Handle(UpdateGroupCmd request, CancellationToken cancellationToken)
            {
                var group = await _groupsService.GetGroupById(request.Id).FirstOrDefaultAsync();

                if (group == null) throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription("Group does not exist.");

                if (group.Id == GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_ID)
                {
                    throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription("Administrator group cannot be changed.");
                }

                await _groupsService.UpdateGroupAsync(group, new Domain.Entities.Group
                {
                    DisplayName = request.DisplayName,
                    Name = request.Name
                });

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<UpdateGroupCmd>
        {
            public Validator(GroupService groupsService)
            {
                RuleFor(x => x.DisplayName)
                    .NotEmpty();

                RuleFor(x => x.Name)
                    .Cascade(CascadeMode.Stop)

                    .NotEmpty()

                    .Must(name => name?.ToLower() != GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME.ToLower())
                    .WithMessage("Group name is reserved.")

                    .MustAsync(async (model, name, token) =>
                    {
                        return !await groupsService.GetGroupByName(name).Where(x => x.Id != model.Id).AnyAsync();
                    })
                    .WithMessage("Name is not unique.");
            }
        }
    }
}
