using System.IO;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.FileSystem;

using FluentValidation;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.FileSystem
{
    public class GetDirectoryContentQuery : IRequest<GetDirectoryContentQuery.Response>
    {
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetDirectoryContentQuery, Response>
        {
            private readonly FileSystemService _fileSystemService;

            public Handler(FileSystemService fileSystemService)
            {
                _fileSystemService = fileSystemService;
            }

            public async Task<Response> Handle(GetDirectoryContentQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    DirectoryContentDetails = await _fileSystemService.GetDirectoryContentAsync(request.Path, cancellationToken)
                };
            }
        }

        public class Response
        {
            public DirectoryContentDetails DirectoryContentDetails { get; set; }
        }

        public class Validator : AbstractValidator<GetDirectoryContentQuery>
        {
            public Validator()
            {
                RuleFor(x => x.Path)
                    .Must(path => Directory.Exists(path))
                    .WithMessage("Path does not exist.");
            }
        }
    }
}
