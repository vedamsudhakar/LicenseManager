using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace LicenseManager.Authentication
{
    public static class SeedData
    {
        public static async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            // Retrieve IConfiguration
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Resolve services
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define default roles
            string[] roleNames = { UserRoles.Admin, UserRoles.User };

            // Create roles if they don't already exist
            foreach (var roleName in roleNames)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {roleName}");
                    }
                }
            }

            // Optionally, create a default admin user:
            var adminEmail    = configuration["AdminUser:Email"] ?? "admin@domain.com";
            var adminUserName = configuration["AdminUser:UserName"] ?? "admin";
            var adminPassword = configuration["AdminUser:Password"] ?? "Test@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var userResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create admin user.");
                }
            }
        }
    }
}
