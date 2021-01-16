using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly AccountService _accountsService;

        public AccountsController(IMediator mediator, IMapper mapper, AccountService accountsService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _accountsService = accountsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetSessionApplicationUsersGroupsAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _mapper.Map<List<GroupDto>>((await _mediator.Send(new GetUsersGroupsQuery { ApplicationUserId = userId })).Groups);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<List<GroupDto>>> GetApplicationUsersGroupsAsync([FromQuery, Required] string applicationUserId)
        {
            return _mapper.Map<List<GroupDto>>((await _mediator.Send(new GetUsersGroupsQuery { ApplicationUserId = applicationUserId })).Groups);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetApplicationUsersAsync()
        {
            return _mapper.Map<List<ApplicationUserDto>>((await _mediator.Send(new GetUsersQuery())).ApplicationUsers);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ApplicationUserDto>> CreateApplicationUserAsync([FromBody, Required] CreateUserModel createUserModel)
        {
            return _mapper.Map<ApplicationUserDto>((await _mediator.Send(new CreateUserCmd
            {
                UserName = createUserModel.UserName,
                Password = createUserModel.Password,
                PasswordRepeat = createUserModel.PasswordRepeat
            })).ApplicationUser);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeleteApplicationUserAsync([FromBody] ApplicationUserDto applicationUserDto)
        {
            await _mediator.Send(new DeleteUserCmd { Id = applicationUserDto.Id });

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateApplicationUserGroupAffinityAsync([FromBody] ApplicationUserGroupDto applicationUserGroupDto, [FromQuery, Required] bool isInGroup)
        {
            await _mediator.Send(new UpdateUserGroupAffinityCmd
            {
                ApplicationUserId = applicationUserGroupDto.ApplicationUserId,
                GroupId = applicationUserGroupDto.GroupId,
                IsInGroup = isInGroup
            });

            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateApplicationUserAsync([FromBody, Required] ApplicationUserDto applicationUserDto)
        {
            await _mediator.Send(new UpdateUserCmd
            {
                Id = applicationUserDto.Id,
                UserName = applicationUserDto.UserName
            });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSessionApplicationUserAsync([FromBody, Required] ApplicationUserDto applicationUserDto)
        {
            await _mediator.Send(new UpdateUserCmd
            {
                Id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName = applicationUserDto.UserName
            });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePasswordAsync([FromBody, Required] UpdatePasswordModel updatePasswordModel)
        {
            await _mediator.Send(new UpdateUserPasswordCmd
            {
                ApplicationUserId = updatePasswordModel.ApplicationUserId,
                CurrentPassword = updatePasswordModel.CurrentPassword,
                Password = updatePasswordModel.Password,
                PasswordRepeat = updatePasswordModel.PasswordRepeat
            });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSessionPasswordAsync([FromBody, Required] UpdatePasswordModel updatePasswordModel)
        {
            await _mediator.Send(new UpdateUserPasswordCmd
            {
                ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                CurrentPassword = updatePasswordModel.CurrentPassword,
                Password = updatePasswordModel.Password,
                PasswordRepeat = updatePasswordModel.PasswordRepeat
            });

            return Ok();
        }
    }
}
