using Authentication_Authorization_Platform___IAM.Models.DTOs;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Authentication_Authorization_Platform___IAM.Services
{
    public interface IAdminService
    {

        Task<List<AdminUserListItemDto>> GetUserAsync();
        Task<AdminActionResultDto> AssignRoleAsync(string actorUserId, string actorEmail, AssignRoleRequest request);
        Task<AdminActionResultDto> RemoveRoleAsync(string actorUserId, string actorEmail, RemoveRoleRequest request);

        Task<AdminActionResultDto> AddPermissionAsync(string actorUserId, string actorEmail, AddPermissionRequest request);
        Task<AdminActionResultDto> RemovePermissionAsync(string actorUserId, string actorEmail, RemovePermissionRequest request);


    }
}
