using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Helpers;
using BytexDigital.RGSM.Node.Domain.Models.FileSystem;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.FileSystem.Commands
{
    public class GetDirectoryContentQuery : IRequest<GetDirectoryContentQuery.Response>
    {
        public string Id { get; set; }
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetDirectoryContentQuery, Response>
        {
            private readonly FileSystemService _fileSystemService;
            private readonly ServerService _serversService;

            public Handler(FileSystemService fileSystemService, ServerService serversService)
            {
                _fileSystemService = fileSystemService;
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetDirectoryContentQuery request, CancellationToken cancellationToken)
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

                if (!System.IO.Directory.Exists(accessedPath))
                {
                    throw new ServiceException()
                        .AddServiceError()
                        .WithField(nameof(request.Path))
                        .WithDescription("Directory does not exist.");
                }

                return new Response
                {
                    DirectoryContentDetails = await _fileSystemService.GetDirectoryContentAsync(accessedPath, cancellationToken)
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
                    .Cascade(CascadeMode.Stop)

                    .NotNull()

                    .Must(path => !System.IO.Path.IsPathRooted(path))
                    .WithMessage("Path may not be rooted.");
            }
        }
    }
}