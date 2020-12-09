using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core;
using BytexDigital.RGSM.Panel.Server.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Nodes;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NodesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly NodesService _nodesService;

        public NodesController(
            IMediator mediator,
            IMapper mapper,
            IAuthorizationService authorizationService,
            NodesService nodesService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _nodesService = nodesService;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<NodeDto>> RegisterNodeAsync([FromBody, Required] NodeDto nodeDto)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new RegisterNodeCmd
            {
                BaseUri = nodeDto.BaseUri,
                Name = nodeDto.Name,
                DisplayName = nodeDto.DisplayName
            });

            return _mapper.Map<NodeDto>(result.Node);
        }

        [HttpGet]
        public async Task<ActionResult<List<NodeDto>>> GetNodesAsync()
        {
            return _mapper.Map<List<NodeDto>>(await _nodesService.GetNodes().ToListAsync());
        }

        [HttpGet("{nodeId}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<NodeKeyDto>> GetNodeKeyAsync([FromRoute, Required] string nodeId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            return _mapper.Map<NodeKeyDto>((await _mediator.Send(new GetNodeKeyQuery { NodeId = nodeId })).NodeKey);
        }

        [HttpDelete("{nodeId}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeleteAsync([FromRoute, Required] string nodeId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new DeleteNodeCmd { Id = nodeId });

            return Ok();
        }

        [HttpPut("{nodeId}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateAsync([FromRoute, Required] string nodeId, [FromBody, Required] NodeDto nodeDto)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new UpdateNodeCmd
            {
                Id = nodeId,
                DisplayName = nodeDto.DisplayName,
                BaseUri = nodeDto.BaseUri,
                Name = nodeDto.Name
            });

            return Ok();
        }
    }
}
