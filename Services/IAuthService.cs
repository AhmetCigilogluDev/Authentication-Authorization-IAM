using Authentication_Authorization_Platform___IAM.Models;

namespace Authentication_Authorization_Platform___IAM.Services
{
    public interface IAuthService
    {

        Task<AuthResponse> RegisterAsync(RegisterRequest req);
        Task<AuthResponse> LoginAsync(LoginRequest req);
    }
}
