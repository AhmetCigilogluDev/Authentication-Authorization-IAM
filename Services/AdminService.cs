using Authentication_Authorization_Platform___IAM.Data;
using Authentication_Authorization_Platform___IAM.Models;
using Authentication_Authorization_Platform___IAM.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Authentication_Authorization_Platform___IAM.Services
{
    public class AdminService : IAdminService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly AppDbContext _db;

        // Making The Dependency Injection with the userManager object
        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationUser> roleManager,
                            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
             
        // GettingUserAsync Function -- for listing filtering users

     public async   Task<List<AdminUserListItemDto>> GetUserAsync()
        {
            // getting all users from db
            var users = await _userManager.Users.ToListAsync();

            // DTO List to return to the world
            var result = new List<AdminUserListItemDto>();

            // Pulling the ecery users roles and claims

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);


                // Filtering claims for to get the permissionsClaims

                var permissions = claims.Where(c => c.Type == "perm")
                                        .Select(c => c.Value).ToList();

                // Collected users is mapping the DTO

                result.Add(new AdminUserListItemDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList(),
                    Permissions = permissions
                });

            }
             return result;

        }


        async Task<AdminActionResultDto> IAdminService.RemoveRoleAsync(string actorUserId, string actorEmail, RemoveRoleRequest request)
        {
            // finding user to remove the role
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Fail("target user didn't find");

            // check user already has the role?
            var isInRole = await _userManager.IsInRoleAsync(user, request.Role);

            if (isInRole)
                return Fail("Target user already has this role");


            // rol has removed from the user
            var result = await _userManager.RemoveFromRoleAsync(user, request.Role);

            if(!result.Succeeded)
                return Fail(string.Join("|", result.Errors.Select(e => e.Description)));

            // All operation is writing onto the AuditLog
            var auditLog = new AuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                TargetUserId = user.Id,
                TargetEmail = user.Email,
                ActionType = "RemoveRole",
                Detail = ${ "Role Removed:{ request.Role} " };
            }






        }


        async Task<AdminActionResultDto> IAdminService.AssignRoleAsync(string actorUserId, string actorEmail, AssignRoleRequest request)
        {
            // Find the user to give role
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return Fail("Rol didn't find");

            // Is rol in the system, check it
            var roles = await _roleManager.RoleExistsAsync(request.Role);

            // Is user already have this role, check it
            var alreadyInRole = await _userManager.IsInRoleAsync(user, request.Role);

            if (alreadyInRole)
                return Fail("User is already have this role.");

            // adding the role to user
            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (!result.Succeeded)
                return Fail(string.Join("|", result.Errors.Select(e => e.Description)));

            // All Operation is writing in the auditlog system 

            var auditLog = new AuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                TargetUserId = user.Id,
                TargetEmail = user.Email ?? string.Empty,
                ActionType = "AssignRole",
                Detail = $"Role Assigned: {request.Role}",
                CreatedAtUtc = DateTime.UtcNow
            };

            // persisting to the db

            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();

            // returning to the real world
            return Success("Rol is assigned succesfully", reloginRequired: true);


        }

        Task<AdminActionResultDto> IAdminService.AddPermissionAsync(string actorUserId, string actorEmail, AddPermissionRequest request)
        {
            // finding the user to give permission

        }

     

        Task<AdminActionResultDto> IAdminService.RemovePermissionAsync(string actorUserId, string actorEmail, RemoveRoleRequest request)
        {
            throw new NotImplementedException();
        }


        private static AdminActionResultDto Success(string message, bool reloginRequired)
        {
            return new AdminActionResultDto
            {
                Success = true,
                Message = message,
                ReloginRequired = reloginRequired
            };
        }

        private static AdminActionResultDto Fail(string message)
        {
            return new AdminActionResultDto
            {
                Success = false,
                Message = message,
                ReloginRequired = false
            };
        }
    }







}
