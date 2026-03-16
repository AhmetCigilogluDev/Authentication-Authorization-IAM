using Authentication_Authorization_Platform___IAM.Models.Auth;
using Authentication_Authorization_Platform___IAM.Models.DTOs;
using Authentication_Authorization_Platform___IAM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authentication_Authorization_Platform___IAM.Controllers
{
    [Route("api/admin")]
    [Authorize(Policy = Policies.AdminPanel)]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;

        // Admin Service injecting with Depency Injection
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        //I will define a private function; the user ID and email will be fetched directly.
        private (string UserId, string email) GetActor()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "unknown";
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "unknown";

            return (userId, email);

        }

        // FETCH THE GETUSERASYNC FUNCTION

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            //user list is fetching from the admin service
            var userList = await _adminService.GetUserAsync();

            return Ok(userList);
        }


        // Fetching the assignRoleAsync function

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleRequest request)
        {
            // get actor information from jwt
            var actor = GetActor();


            // get servis function
            var result = await _adminService.AssignRoleAsync(actor.UserId, actor.email, request);

            // make reponse before make validation
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);

        }


        // Remove the role from user
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
        {
            // get actor information from jwt
            var actor = GetActor();

            // get servis function
            var result = await _adminService.RemoveRoleAsync(actor.UserId, actor.email, request);

            // make response before make validation
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("add-permission")]
        public async Task<IActionResult> AddPermission([FromBody] AddPermissionRequest request)
        {
            // get actor information from jwt
            var actor = GetActor();
            // get service function
            var result = await _adminService.AddPermissionAsync(actor.UserId, actor.email, request);

            // make return before make validation
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("remove-permission")]
        public async Task<IActionResult> RemovePermission([FromBody] RemovePermissionRequest request)
        {
            // get actor information from jwt
            var actor = GetActor();
            // get servis function
            var result = await _adminService.RemovePermissionAsync(actor.UserId, actor.email, request);

            // return before make validation
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }



    }
}