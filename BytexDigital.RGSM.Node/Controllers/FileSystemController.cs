using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands.FileSystem;
using BytexDigital.RGSM.Node.TransferObjects.Models.FileSystem;
using BytexDigital.RGSM.Shared;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class FileSystemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public FileSystemController(IMediator mediator, IMapper mapper, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<DirectoryContentDetailsDto>> GetDirectoryContentDetailsAsync([FromRoute] string serverId, [FromQuery, Required] string path)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.FILEBROWSER_READ
            })).Succeeded)
            {
                return Unauthorized();
            }

            return _mapper.Map<DirectoryContentDetailsDto>((await _mediator.Send(new GetDirectoryContentQuery { Path = path, Id = serverId })).DirectoryContentDetails);
        }

        [HttpGet]
        public async Task<ActionResult<FileContentResult>> GetFileContentAsync([FromRoute] string serverId, [FromQuery, Required] string path)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.FILEBROWSER_READ
            })).Succeeded)
            {
                return Unauthorized();
            }

            var response = await _mediator.Send(new GetFileContentQuery { Path = path, Id = serverId });
            var fileName = Path.GetFileName(path);

            return File(response.Content, "application/octet-stream", fileName);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetFileContentAsStringAsync([FromRoute] string serverId, [FromQuery, Required] string path)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.FILEBROWSER_READ
            })).Succeeded)
            {
                return Unauthorized();
            }

            var response = await _mediator.Send(new GetFileContentQuery { Path = path, Id = serverId });

            return Encoding.UTF8.GetString(response.Content);
        }
    }
}
