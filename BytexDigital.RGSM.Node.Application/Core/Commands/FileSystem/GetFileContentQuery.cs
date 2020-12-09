using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.FileSystem
{
    public class GetFileContentQuery : IRequest<GetFileContentQuery.Response>
    {
        public string Path { get; set; }

        public class Handler : IRequestHandler<GetFileContentQuery, Response>
        {
            private readonly FileSystemService _fileSystemService;

            public Handler(FileSystemService fileSystemService)
            {
                _fileSystemService = fileSystemService;
            }

            public async Task<Response> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
            {
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
                    .Must(path => File.Exists(path))
                    .WithMessage("File does not exist.");
            }
        }
    }
}
