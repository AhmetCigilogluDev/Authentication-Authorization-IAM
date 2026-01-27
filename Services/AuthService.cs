namespace Authentication_Authorization_Platform___IAM.Services
{
    using Authentication_Authorization_Platform___IAM.Models;
   
    using Microsoft.AspNetCore.Identity;

    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwt;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenService jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            // 1) Email unique mi kontrol et.
            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
                return new AuthResponse { Success = false, Message = "Bu email zaten kayıtlı." };

            // 2) Identity user objesini üret.
            var user = new ApplicationUser
            {
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim(),
                UserName = req.Email.Trim() // pratik standart: username=email
            };

            // 3) Password hash + user insert işlemini Identity’e yaptır.
            var createResult = await _userManager.CreateAsync(user, req.Password);
            if (!createResult.Succeeded)
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(" | ", createResult.Errors.Select(e => e.Description))
                };

            // 4) Default rol yoksa oluştur.
            const string defaultRole = "User";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
                await _roleManager.CreateAsync(new IdentityRole(defaultRole));

            // 5) Kullanıcıya default rol ata.
            await _userManager.AddToRoleAsync(user, defaultRole);

            // 6) İstersen token üretip response’a koy.
            var token = await _jwt.CreateTokenAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Kayıt başarılı.",
                Token = token
            };
        }
    }
}
