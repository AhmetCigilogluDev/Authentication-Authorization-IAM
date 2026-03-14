using Authentication_Authorization_Platform___IAM.Models.DTOs;

namespace Authentication_Authorization_Platform___IAM.Services
{
    public interface IAdminService
    {

        Task<List<AdminUserListItemDto>> GetUserAsync();
    }
}
