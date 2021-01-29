using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Steam.Commands;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class SteamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SteamController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SteamLoginDto>>> GetLoginsAsync()
        {
            return _mapper.Map<List<SteamLoginDto>>((await _mediator.Send(new GetSteamLoginsQuery())).Credentials);
        }
    }
}
