using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core;
using BytexDigital.RGSM.Shared.TransferObjects.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AccountsService _accountsService;

        public AccountsController(IMapper mapper, AccountsService accountsService)
        {
            _mapper = mapper;
            _accountsService = accountsService;
        }

        [HttpGet("GetAssignedGroups")]
        public async Task<ActionResult> GetAssignedGroupsAsync()
        {
            var user = await _accountsService.GetUser(HttpContext.User).FirstOrDefaultAsync();
            var groups = await _accountsService.GetAssignedGroups(user).ToListAsync();

            return Ok(_mapper.Map<List<GroupDto>>(groups));
        }
    }
}
