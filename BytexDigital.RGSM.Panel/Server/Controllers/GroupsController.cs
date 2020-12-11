
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication;
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
    }
}
