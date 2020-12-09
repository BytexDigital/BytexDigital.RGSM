
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GroupsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetUserGroup([FromQuery, Required] string userId)
        {

        }
    }
}
