using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs.Commands;
using BytexDigital.RGSM.Node.TransferObjects.Models.ServerLogs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class ServerLogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ServerLogsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<LogSourceDto>>> GetLogSourcesAsync([FromRoute] string serverId)
        {
            return _mapper.Map<List<LogSourceDto>>((await _mediator.Send(new GetLogSourcesQuery { ServerId = serverId })).Sources);
        }

        [HttpGet]
        public async Task<ActionResult<LogSourceDto>> GetPrimaryLogSourceAsync([FromRoute] string serverId)
        {
            return _mapper.Map<LogSourceDto>((await _mediator.Send(new GetPrimaryLogSourceQuery { ServerId = serverId })).Source);
        }

        [HttpGet]
        public async Task<ActionResult<LogContentDto>> GetLogContentAsync(
            [FromRoute] string serverId,
            [FromQuery, Required] string sourceName,
            [FromQuery] int limitLines = default)
        {
            return _mapper.Map<LogContentDto>((await _mediator.Send(new GetLogContentQuery
            {
                ServerId = serverId,
                SourceName = sourceName,
                LimitLines = limitLines
            })).Content);
        }
    }
}
