using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Servers.Commands;
using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ServersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public ServersController(IMediator mediator, IMapper mapper, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<ActionResult<ServerDto>> CreateServerAsync([FromBody, Required] ServerDto serverDto)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            var inputServer = _mapper.Map<Server>(serverDto);

            var result = await _mediator.Send(new CreateServerCmd
            {
                DisplayName = inputServer.DisplayName,
                ServerType = inputServer.Type,
                Directory = inputServer.Directory
            });

            return _mapper.Map<ServerDto>(result.Server);
        }

        [HttpGet]
        public async Task<ActionResult<List<ServerDto>>> GetServersAsync()
        {
            return _mapper.Map<List<ServerDto>>((await _mediator.Send(new GetServersQuery())).Servers);
        }

        [HttpPost("{serverId}")]
        public async Task<ActionResult> DeleteServerAsync([FromRoute] string serverId, [FromQuery] bool deleteAllFiles = true)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new DeleteServerCmd { ServerId = serverId, DeleteAllFiles = deleteAllFiles });

            return Ok();
        }
    }
}
