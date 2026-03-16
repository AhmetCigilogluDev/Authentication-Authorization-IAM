using Authentication_Authorization_Platform___IAM.Models.Auth;
using Authentication_Authorization_Platform___IAM.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Authentication_Authorization_Platform___IAM.Data.Infrastructure
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Roles
            var roles = new[] { Roles.Admin, Roles.Manager, Roles.User };
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

            // Admin user
            const string adminEmail = "admin@iam.local";
            const string adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    FullName = "System Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);

                if (!createResult.Succeeded)
                    throw new Exception("Admin user oluşturulamadı: " +
                        string.Join(" | ", createResult.Errors.Select(e => e.Description)));
            }

            // Admin role garanti altına al
            if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
            {
                var addRoleResult = await userManager.AddToRoleAsync(admin, Roles.Admin);

                if (!addRoleResult.Succeeded)
                    throw new Exception("Admin role eklenemedi: " +
                        string.Join(" | ", addRoleResult.Errors.Select(e => e.Description)));
            }

            // Admin claims garanti altına al
            var existingClaims = await userManager.GetClaimsAsync(admin);

            var requiredPermissions = new[]
            {
                Permissions.DocumentsRead,
                Permissions.DocumentsUpload,
                Permissions.DocumentsDelete,
                Permissions.AdminPanel
            };

            foreach (var permission in requiredPermissions)
            {
                var hasPermission = existingClaims.Any(c => c.Type == "perm" && c.Value == permission);

                if (!hasPermission)
                {
                    var addClaimResult = await userManager.AddClaimAsync(admin, new Claim("perm", permission));

                    if (!addClaimResult.Succeeded)
                        throw new Exception($"Permission eklenemedi: {permission} | " +
                            string.Join(" | ", addClaimResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}