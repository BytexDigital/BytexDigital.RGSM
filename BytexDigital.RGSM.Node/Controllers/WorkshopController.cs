using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core;
using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands.Workshop;
using BytexDigital.RGSM.Node.TransferObjects.Models.Workshop;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class WorkshopController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public WorkshopController(IMediator mediator, IMapper mapper, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddModAsync([FromRoute] string serverId, [FromQuery, Required] ulong publishedFileId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.WORKSHOP
            })).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new AddWorkshopModCmd { Id = serverId, PublishedFileId = publishedFileId });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> RemoveModAsync([FromRoute] string serverId, [FromQuery, Required] ulong publishedFileId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.WORKSHOP
            })).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new RemoveWorkshopModCmd { Id = serverId, PublishedFileId = publishedFileId });

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkshopItemDto>>> GetModsAsync([FromRoute] string serverId)
        {
            var response = await _mediator.Send(new GetWorkshopItemStatusQuery { Id = serverId });

            return _mapper.Map<List<WorkshopItemDto>>(response.WorkshopItems);
        }

        [HttpPost]
        public async Task<ActionResult> InstallOrUpdateAllAsync([FromRoute] string serverId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.WORKSHOP
            })).Succeeded)
            {
                return Unauthorized();
            }

            await _mediator.Send(new BeginUpdateWorkshopModsCmd { Id = serverId });

            return Ok();
        }
    }
}
