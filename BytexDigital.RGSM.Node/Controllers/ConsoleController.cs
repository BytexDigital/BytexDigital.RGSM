using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Commands.Console;
using BytexDigital.RGSM.Node.TransferObjects.Models.Console;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class ConsoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ConsoleController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ConsoleOutputContentDto>>> GetConsoleOutputAsync(
            [FromRoute, Required] string serverId,
            [FromQuery] int lastNLines = 0,
            [FromQuery] string[] identifiers = default)
        {
            var response = await _mediator.Send(new GetConsoleOutputQuery
            {
                Id = serverId,
                LastNLines = lastNLines,
                Identifiers = identifiers?.ToList()
            });

            return _mapper.Map<List<ConsoleOutputContentDto>>(response.Outputs);
        }
    }
}
