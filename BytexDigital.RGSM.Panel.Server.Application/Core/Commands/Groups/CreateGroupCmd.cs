using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Shared;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Groups
{
    public class CreateGroupCmd : IRequest<CreateGroupCmd.Response>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public class Handler : IRequestHandler<CreateGroupCmd, Response>
        {
            private readonly GroupsService _groupsService;

            public Handler(GroupsService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Response> Handle(CreateGroupCmd request, CancellationToken cancellationToken)
            {
                var group = await (await _groupsService.CreateGroupAsync(new Group
                {
                    Name = request.Name,
                    DisplayName = request.DisplayName
                })).FirstAsync();

                return new Response
                {
                    Group = group
                };
            }
        }

        public class Response
        {
            public Group Group { get; set; }
        }

        public class Validator : AbstractValidator<CreateGroupCmd>
        {
            public Validator(GroupsService groupsService)
            {
                RuleFor(x => x.DisplayName)
                    .NotEmpty();

                RuleFor(x => x.Name)
                    .Cascade(CascadeMode.Stop)

                    .NotEmpty()

                    .NotEqual(GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME)
                    .WithMessage("Group name is reserved.")

                    .MustAsync(async (name, token) =>
                    {
                        return await groupsService.GetGroupByName(name).AnyAsync();
                    })
                    .WithMessage("Name is not unique.");
            }
        }
    }
}
