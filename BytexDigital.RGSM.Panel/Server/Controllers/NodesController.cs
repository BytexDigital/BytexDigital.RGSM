using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Nodes;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    [Authorize]
    public class NodesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public NodesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<NodeDto>> RegisterNodeAsync([FromBody, Required] NodeDto nodeDto)
        {
            var result = await _mediator.Send(new RegisterNodeCmd
            {
                BaseUri = nodeDto.BaseUri,
                Name = nodeDto.Name,
                DisplayName = nodeDto.DisplayName
            });

            return _mapper.Map<NodeDto>(result.Node);
        }
    }
}
