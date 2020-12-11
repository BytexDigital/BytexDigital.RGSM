using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthenticationController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiKeyDetailsDto>> GetApiKeyValidityAsync([FromQuery] string key)
        {
            return _mapper.Map<ApiKeyDetailsDto>((await _mediator.Send(new GetApiKeyValidityQuery { KeyValue = key })));
        }
    }
}
