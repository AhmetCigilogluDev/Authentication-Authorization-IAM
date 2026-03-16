using Authentication_Authorization_Platform___IAM.Data;
using Authentication_Authorization_Platform___IAM.Models;
using Authentication_Authorization_Platform___IAM.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Authentication_Authorization_Platform___IAM.Services
{
    public class AdminService : IAdminService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        // Making The Dependency Injection with the userManager object
        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
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
                Detail = $"Role Removed: {request.Role}",
                CreatedAtUtc = DateTime.UtcNow
            };


            // persist to the db
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();

            // returning to the real world with dto
            return Success("Rol has been succesfully removed", reloginRequired: true);




        }



        async Task<AdminActionResultDto> IAdminService.AddPermissionAsync(string actorUserId, string actorEmail, AddPermissionRequest request)
        {
            //find the user
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return Fail("The target user didn't find it");



            //get the user claim
            var claims = await _userManager.GetClaimsAsync(user);
        

            //check! does the user have this permissions
            var havePermission = claims.Any(c => c.Type == "perm" && c.Value == request.Permission);
            if (havePermission)
                return Fail("User is already has this permission");

            // new permission claim is adding
            var result = await _userManager.AddClaimAsync(user, new Claim("perm", request.Permission));
            if (!result.Succeeded)
                return Fail(string.Join("|", result.Errors.Select(e => e.Description)));

            //All operation will writing in to the auditin log
            var auditLog = new AuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                TargetUserId = user.Id,
                TargetEmail = user.Email,
                ActionType = "AddPermission",
                Detail = $"Role Removed: {request.Permission}",
                CreatedAtUtc = DateTime.UtcNow
            };


            // persist to the db
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();

            // returning to the real world with dto
            return Success("Permission has been added", reloginRequired: true);




        }



        async Task<AdminActionResultDto> IAdminService.RemovePermissionAsync(string actorUserId, string actorEmail, RemovePermissionRequest request)
        {
            //find the user
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return Fail("This user is not finding");

            // Get the user claims
            var claims = await _userManager.GetClaimsAsync(user);

            // Find the permission
            var permissionClaim = claims.FirstOrDefault(c => c.Type == "perm" && c.Value == request.Permission);
            if (permissionClaim is null)
                return Fail("User has not this permission");
            // permission is removing from the user
            var result = await _userManager.RemoveClaimAsync(user, permissionClaim);

            if (!result.Succeeded)
                return Fail(string.Join("|", result.Errors.Select(e => e.Description)));



            var auditLog = new AuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                TargetUserId = user.Id,
                TargetEmail = user.Email,
                ActionType = "RemovePermission",
                Detail = $"Role Permission: {request.Permission}",
                CreatedAtUtc = DateTime.UtcNow
            };



            // persist to the db
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();

            // returning to the real world with dto
            return Success("Permission has been removed", reloginRequired: true);





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
