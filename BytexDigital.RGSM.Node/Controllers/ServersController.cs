using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServersController : ControllerBase
    {

    }
}
