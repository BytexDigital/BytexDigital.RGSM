﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Helpers;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.FileSystem
{
    public class GetFileContentQuery : IRequest<GetFileContentQuery.Response>
    {
        public string Id { get; set; }
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetFileContentQuery, Response>
        {
            private readonly FileSystemService _fileSystemService;
            private readonly ServersService _serversService;

            public Handler(FileSystemService fileSystemService, ServersService serversService)
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
                        .WithField(nameof(request.Path))
                        .WithMessage("The path cannot be accessed because it resides outside the server directory.");
                }

                if (!System.IO.File.Exists(accessedPath))
                {
                    throw new ServiceException()
                        .WithField(nameof(request.Path))
                        .WithMessage("File does not exist.");
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
