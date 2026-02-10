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
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            // Admin user
            const string adminEmail = "admin@iam.local";
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

                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, Roles.Admin);

                // Admin permissions
                await userManager.AddClaimAsync(admin, new Claim("perm", Permissions.DocumentsRead));
                await userManager.AddClaimAsync(admin, new Claim("perm", Permissions.DocumentsUpload));
                await userManager.AddClaimAsync(admin, new Claim("perm", Permissions.DocumentsDelete));
                await userManager.AddClaimAsync(admin, new Claim("perm", Permissions.AdminPanel));
            }
        }
    }
}
