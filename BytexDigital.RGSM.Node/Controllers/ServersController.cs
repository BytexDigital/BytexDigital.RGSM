using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Node.Application.Commands.Servers;
using BytexDigital.RGSM.Shared.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ServersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ServersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ServerDto> CreateServerAsync([FromBody, Required] ServerDto inputDto)
        {
            var input = _mapper.Map<Server>(inputDto);

            var response = await _mediator.Send(new CreateServerCmd
            {
                ServerType = input.Type,
                DisplayName = input.DisplayName,
                Directory = input.Directory
            });

            return _mapper.Map<ServerDto>(response.Server);
        }
    }
}
