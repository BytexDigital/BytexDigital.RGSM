
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication;
using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Groups;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<List<ApplicationUserGroupDto>>> GetUsersGroupsAsync([FromQuery, Required] string userId)
        {
            return _mapper.Map<List<ApplicationUserGroupDto>>((await _mediator.Send(new GetUserGroupsQuery { UserId = userId })).GroupLinks);
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetGroupsAsync()
        {
            return _mapper.Map<List<GroupDto>>((await _mediator.Send(new GetGroupsQuery())).Groups);
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<GroupDto>> GetGroupAsync([FromRoute] string groupId)
        {
            return _mapper.Map<GroupDto>((await _mediator.Send(new GetGroupQuery { Id = groupId })).Group);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<GroupDto>> CreateGroupAsync([FromBody] GroupDto groupDto)
        {
            return _mapper.Map<GroupDto>((await _mediator.Send(new CreateGroupCmd
            {
                DisplayName = groupDto.DisplayName,
                Name = groupDto.Name
            })).Group);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateGroupAsync([FromBody] GroupDto groupDto)
        {
            await _mediator.Send(new UpdateGroupCmd
            {
                Id = groupDto.Id,
                DisplayName = groupDto.DisplayName,
                Name = groupDto.Name
            });

            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeleteGroupAsync([FromBody] GroupDto groupDto)
        {
            await _mediator.Send(new DeleteGroupCmd
            {
                Id = groupDto.Id
            });

            return Ok();
        }
    }
}
