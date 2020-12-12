
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core;
using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands.Scheduling;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;
using BytexDigital.RGSM.Node.TransferObjects.Entities.Scheduling;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/{serverId}/[action]")]
    [ApiController]
    [Authorize]
    public class SchedulerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public SchedulerController(IMediator mediator, IMapper mapper, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<SchedulerPlanDto>> GetSchedulerPlanAsync([FromRoute] string serverId)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.SCHEDULER
            })).Succeeded)
            {
                return Unauthorized();
            }

            return _mapper.Map<SchedulerPlanDto>((await _mediator.Send(new GetSchedulerQuery { ServerId = serverId })).SchedulerPlan);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSchedulerPlanAsync([FromRoute] string serverId, [FromBody] SchedulerPlanDto schedulerPlanDto)
        {
            if (!(await _authorizationService.AuthorizeAsync(HttpContext.User, null, new PermissionRequirement
            {
                ServerId = serverId,
                Name = PermissionConstants.SCHEDULER
            })).Succeeded)
            {
                return Unauthorized();
            }

            var schedulerPlan = _mapper.Map<SchedulerPlan>(schedulerPlanDto);

            await _mediator.Send(new UpdateSchedulerPlanCmd { ServerId = serverId, ChangedSchedulerPlan = schedulerPlan });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> EnableSchedulerAsync([FromRoute] string serverId)
        {
            await _mediator.Send(new UpdateSchedulerEnabledCmd { ServerId = serverId, Enable = true });

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> StopSchedulerAsync([FromRoute] string serverId)
        {
            await _mediator.Send(new UpdateSchedulerEnabledCmd { ServerId = serverId, Enable = false });

            return Ok();
        }
    }
}
