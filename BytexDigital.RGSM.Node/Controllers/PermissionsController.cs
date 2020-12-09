using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public PermissionsController(IMediator mediator, IMapper mapper, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<List<PermissionDto>> GetPermissionsAsync([FromRoute] string serverId)
        {
            return _mapper.Map<List<PermissionDto>>((await _mediator.Send(new GetServerPermissionsQuery { Id = serverId })).Permissions);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrRemoveGroupFromPermissionAsync(
            [FromRoute] string serverId,
            [FromQuery, Required] string permissionName,
            [FromQuery, Required] string groupId,
            [FromQuery, Required] bool addOrRemove)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new SystemAdministratorRequirement())).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new ChangeGroupPermissionCmd { Id = serverId, AddOrRemove = addOrRemove, GroupId = groupId, Name = permissionName });

            return Ok();
        }
    }
}
