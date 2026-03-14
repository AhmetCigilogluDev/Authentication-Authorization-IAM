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
    }







}
