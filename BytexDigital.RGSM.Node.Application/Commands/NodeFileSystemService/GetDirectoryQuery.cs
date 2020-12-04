using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Services.NodeFileSystemService;

using FluentValidation;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Commands.NodeFileSystemService
{

    public class GetDirectoryQuery : IRequest<GetDirectoryQuery.Response>
    {
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetDirectoryQuery, Response>
        {
            private readonly Shared.Services.NodeFileSystemService _nodeFileSystemService;

            public Handler(Shared.Services.NodeFileSystemService nodeFileSystemService)
            {
                _nodeFileSystemService = nodeFileSystemService;
            }

            public Task<Response> Handle(GetDirectoryQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return Task.FromResult(new Response { Directory = _nodeFileSystemService.GetDirectory(request.Path) });

                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new ServiceException().WithCode(nameof(UnauthorizedAccessException)).WithMessage(ex.Message).Build();
                }
            }
        }

        public class Response
        {
            public Directory Directory { get; set; }
        }

        public class Validator : AbstractValidator<GetDirectoryQuery>
        {
            public Validator()
            {
                RuleFor(x => x.Path)
                    .NotEmpty();
            }
        }
    }
}
