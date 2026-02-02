using Authentication_Authorization_Platform___IAM.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorization_Platform___IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("panel")]
        [Authorize(Policy = Policies.AdminPanel)]
        public IActionResult Panel() => Ok(new { message = "admin panel ok" });
    }
}
