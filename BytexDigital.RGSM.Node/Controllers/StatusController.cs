using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.TransferObjects.Models.Status;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StatusController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ServerStatusDto>> StatusAsync([FromRoute] string serverId)
        {
            return _mapper.Map<ServerStatusDto>((await _mediator.Send(new GetServerStatusQuery())).Status);
        }

        [HttpPost]
        public async Task<ActionResult> StartAsync([FromRoute] string serverId)
        {
            await _mediator.Send(new ChangeServerStatusCmd { Id = serverId, StartOrStop = true });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> StopAsync([FromRoute] string serverId)
        {
            await _mediator.Send(new ChangeServerStatusCmd { Id = serverId, StartOrStop = false });

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ServerInstallationStatusDto>> InstallationStatusAsync([FromRoute] string serverId)
        {
            var response = await _mediator.Send(new GetServerInstallationStatusQuery { Id = serverId });

            return _mapper.Map<ServerInstallationStatusDto>(response.InstallationStatus);
        }

        [HttpPost]
        public async Task<ActionResult> InstallOrUpdateAsync([FromRoute] string serverId)
        {
            await _mediator.Send(new InstallIOrUpdateServerCmd { Id = serverId });

            return Ok();
        }
    }
}
