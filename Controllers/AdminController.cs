using Authentication_Authorization_Platform___IAM.Models.Auth;
using Authentication_Authorization_Platform___IAM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Authentication_Authorization_Platform___IAM.Controllers
{
    [Route("api/admin")]
    [Authorize(Policy = Policies.AdminPanel)]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;






        [HttpGet("panel")]
      
        public IActionResult Panel() => Ok(new { message = "admin panel ok" });
    }
}
