
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]/[action]")]
    [ApiController]
    public class ConnectivityController : ControllerBase
    {
        [HttpGet]
        public ActionResult Ping()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public ActionResult AuthenticatedPing()
        {
            return Ok();
        }
    }
}
