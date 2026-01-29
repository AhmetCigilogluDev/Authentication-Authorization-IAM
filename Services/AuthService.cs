namespace Authentication_Authorization_Platform___IAM.Services
{
    using Authentication_Authorization_Platform___IAM.Models;
   
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;

    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwt;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenService jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwt = jwt;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
          // is there a user by email
          var user = await _userManager.FindByEmailAsync(req.Email.Trim());
            if (user == null)
                return new AuthResponse {
                    Success = false,
                    Message = "E-mail veya şifre hatalı girilmiştir, düzeltiniz." 
                };
            // password verification
            var signIn = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure:  false);
            if (!signIn.Succeeded)
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email veya sifre hatalı..."
                };

            // token producing
            var token = await _jwt.CreateTokenAsync(user);

            // return to response
            return new AuthResponse
            {
                Success = true,
                Message = "Başarılı bir şekilde giriş yaptınız",
                Token = token
            };
           
          
           

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
