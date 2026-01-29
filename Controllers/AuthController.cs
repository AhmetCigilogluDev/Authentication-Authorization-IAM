namespace Authentication_Authorization_Platform___IAM.Controllers
{
    using Authentication_Authorization_Platform___IAM.Models;
    using Authentication_Authorization_Platform___IAM.Services;
  
    using Microsoft.AspNetCore.Mvc;
    using System.Net.WebSockets;

    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var result = await _auth.RegisterAsync(req);

            if (!result.Success)
                return BadRequest(result);

            // Token dönüyorsan 200 OK genelde daha pratik, "Created" da olabilir.
            return Ok(result);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {

            var result = await _auth.LoginAsync(req);
           if (!result.Success) return Unauthorized(result);
           return Ok(result);
        }
    }
}
