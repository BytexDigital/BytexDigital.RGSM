using System.IO;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Helpers;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.FileSystem.Commands
{
    public class GetFileContentQuery : IRequest<GetFileContentQuery.Response>
    {
        public string Id { get; set; }
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetFileContentQuery, Response>
        {
            private readonly FileSystemService _fileSystemService;
            private readonly ServerService _serversService;

            public Handler(FileSystemService fileSystemService, ServerService serversService)
            {
                _fileSystemService = fileSystemService;
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                var accessedPath = System.IO.Path.Combine(server.Directory, request.Path);

                if (!accessedPath.IsSubdirectoryOfOrMatches(server.Directory))
                {
                    throw new ServiceException()
                        .AddServiceError()
                        .WithField(nameof(request.Path))
                        .WithDescription("The path cannot be accessed because it resides outside the server directory.");
                }

                if (!File.Exists(accessedPath))
                {
                    throw new ServiceException()
                        .AddServiceError()
                        .WithField(nameof(request.Path))
                        .WithDescription("File does not exist.");
                }

                return new Response
                {
                    Content = await _fileSystemService.ReadFileAsStringAsync(request.Path, cancellationToken),
                    Extension = System.IO.Path.GetExtension(request.Path)
                };
            }
        }

        public class Response
        {
            public string Extension { get; set; }
            public byte[] Content { get; set; }
        }

        public class Validator : AbstractValidator<GetFileContentQuery>
        {
            public Validator()
            {
                RuleFor(x => x.Path)
                    .Cascade(CascadeMode.Stop)

                    .NotNull()

                    .Must(path => File.Exists(path))
                    .WithMessage("File does not exist.")

                    .Must(path => !System.IO.Path.IsPathRooted(path))
                    .WithMessage("Path may not be rooted.");
            }
        }
    }
}
