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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _adminService.GetUserAsync();

                return Ok(result);
        }


 [HttpGet("panel")]
      
        public IActionResult Panel() => Ok(new { message = "admin panel ok" });
    }
}
