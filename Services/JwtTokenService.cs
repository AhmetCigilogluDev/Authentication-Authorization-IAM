namespace Authentication_Authorization_Platform___IAM.Services
{
    using Authentication_Authorization_Platform___IAM.Models;

    
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        // Making the dependecy injection to pulling the config & userManager object.
        public JwtTokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // Pulling the roles from the userManager.
            var roles = await _userManager.GetRolesAsync(user);

            // Pulling the user claims from the userManager

            var userClaims = await _userManager.GetClaimsAsync(user);

            // Starting to the PAYLOAD

            var payload = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("fullName", user.FullName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // User roles adding into the Payload

            payload.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Claims/permissions is adding into the Payload

            payload.AddRange(userClaims);

            // JWT Settings is setting with the _config object

            var jwtKey = _config["Jwt:Key"]!;
            var jwtIssuer = _config["Jwt:Issuer"];
            var jwtAudience = _config["Jwt:Audience"];
            var expiresMinutes = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "60");


            // Starting to the HEADER

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Obtain the JWT OBJECT ---- FINALLY obtaining the jwt payload + config is will be mapping

            var token = new JwtSecurityToken(

                issuer: jwtIssuer,
                audience : jwtAudience,
                claims: payload,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds


                );


            // SIGNATURE -- signing of the JWT OBJECT

            return new JwtSecurityTokenHandler().WriteToken(token);

           

           
            

            
        }
    }

}
