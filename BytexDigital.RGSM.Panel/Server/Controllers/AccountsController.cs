using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Application.Core.Commands;
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
        public async Task<ActionResult> GetAssignedGroupsAsync()
        {
            var user = await _accountsService.GetUser(HttpContext.User).FirstOrDefaultAsync();
            var groups = await _accountsService.GetAssignedGroups(user).ToListAsync();

            return Ok(_mapper.Map<List<GroupDto>>(groups));
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetApplicationUsersAsync()
        {
            return _mapper.Map<List<ApplicationUserDto>>(await _mediator.Send(new GetApplicationUsersQuery()));
        }
    }
}
