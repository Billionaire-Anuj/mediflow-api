using mediflow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IConfiguration config)
    {
        // Base roles
        await EnsureRoleAsync(db, "Admin");
        await EnsureRoleAsync(db, "Doctor");
        await EnsureRoleAsync(db, "Patient");

        var adminEmail = config["Seed:AdminEmail"] ?? "admin@mediflow.local";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin@12345";
        var adminFullName = config["Seed:AdminFullName"] ?? "System Admin";

        var admin = await db.Users.SingleOrDefaultAsync(u => u.Email == adminEmail);
        if (admin == null)
        {
            admin = new User
            {
                FullName = adminFullName,
                Email = adminEmail,
                Phone = null,
                IsActive = true,
                PointsBalance = 0
            };

            var hasher = new PasswordHasher<User>();
            admin.PasswordHash = hasher.HashPassword(admin, adminPassword);

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }

        var adminRoleId = await db.Roles
            .Where(r => r.Name == "Admin")
            .Select(r => r.Id)
            .SingleAsync();

        var alreadyHasRole = await db.UserRoles.AnyAsync(ur => ur.UserId == admin.Id && ur.RoleId == adminRoleId);
        if (!alreadyHasRole)
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRoleId
            });

            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureRoleAsync(AppDbContext db, string roleName)
    {
        var exists = await db.Roles.AnyAsync(r => r.Name == roleName);
        if (!exists)
        {
            db.Roles.Add(new Role { Name = roleName });
            await db.SaveChangesAsync();
        }
    }
}