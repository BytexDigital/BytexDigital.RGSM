using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]")]
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
        public async Task<ActionResult<ServerDto>> CreateServerAsync([FromBody, Required] ServerDto serverDto)
        {
            var inputServer = _mapper.Map<Server>(serverDto);

            var result = await _mediator.Send(new CreateServerCmd
            {
                DisplayName = inputServer.DisplayName,
                ServerType = inputServer.Type
            });

            return _mapper.Map<ServerDto>(result.Server);
        }

        [HttpGet]
        public async Task<ActionResult<List<ServerDto>>> GetServersAsync()
        {
            return _mapper.Map<List<ServerDto>>((await _mediator.Send(new GetServersQuery())).Servers);
        }
    }
}
