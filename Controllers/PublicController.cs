using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization_Platform___IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { ok = true, message = "public ok" });
    }
}
