
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Panel.Server.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
    }
}
