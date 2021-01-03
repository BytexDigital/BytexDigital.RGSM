using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Features.BattlEye.Commands;
using BytexDigital.RGSM.Node.TransferObjects.Models.BattlEye;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class BeRconController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public BeRconController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<BeRconStatusDto>> GetStatusAsync([FromRoute] string serverId)
        {
            return _mapper.Map<BeRconStatusDto>((await _mediator.Send(new GetRconStatusQuery { Id = serverId })).Status);
        }

        [HttpGet]
        public async Task<ActionResult<List<BeRconMessageDto>>> GetMessagesAsync([FromRoute] string serverId, [FromQuery] int limit = 0)
        {
            return _mapper.Map<List<BeRconMessageDto>>((await _mediator.Send(new GetRconMessagesQuery { Id = serverId, Limit = limit })).Messages);
        }

        [HttpGet]
        public async Task<ActionResult<List<BeRconPlayerDto>>> GetPlayersAsync([FromRoute] string serverId)
        {
            return _mapper.Map<List<BeRconPlayerDto>>((await _mediator.Send(new GetRconPlayersQuery { Id = serverId })).Players);
        }

        [HttpPost]
        public async Task<ActionResult> SendMessageAsync([FromRoute] string serverId, [FromQuery] string message)
        {
            await _mediator.Send(new SendRconMessageCmd { Id = serverId, Message = message });

            return Ok();
        }
    }
}
