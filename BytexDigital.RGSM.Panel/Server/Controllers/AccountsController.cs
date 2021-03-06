﻿using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AccountService _accountsService;

        public AccountsController(IMapper mapper, AccountService accountsService)
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
