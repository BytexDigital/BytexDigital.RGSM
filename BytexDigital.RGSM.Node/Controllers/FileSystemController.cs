using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Commands.FileSystem;
using BytexDigital.RGSM.Node.TransferObjects.Models.FileSystem;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FileSystemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public FileSystemController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<DirectoryContentDetailsDto> GetDirectoryContentDetailsAsync([FromQuery, Required] string path)
        {
            return _mapper.Map<DirectoryContentDetailsDto>((await _mediator.Send(new GetDirectoryContentQuery { Path = path })).DirectoryContentDetails);
        }

        [HttpGet]
        public async Task<FileContentResult> GetFileContentAsync([FromQuery, Required] string path)
        {
            var response = await _mediator.Send(new GetFileContentQuery { Path = path });
            var fileName = Path.GetFileName(path);

            return File(response.Content, "application/octet-stream", fileName);
        }

        [HttpGet]
        public async Task<string> GetFileContentAsStringAsync([FromQuery, Required] string path)
        {
            var response = await _mediator.Send(new GetFileContentQuery { Path = path });

            return Encoding.UTF8.GetString(response.Content);
        }
    }
}
