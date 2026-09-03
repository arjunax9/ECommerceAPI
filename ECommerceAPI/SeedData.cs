using ECommerce.Business.Services;
using ECommerce.Data.Data;
using ECommerce.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<ApplicationDbContext>();

        // apply pending migrations (safe in dev)
        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
            // ignore migration errors in some envs
        }

        // Ensure Role column exists on Users table for older DBs that predate the Role property.
        try
        {
            var sql = @"IF COL_LENGTH('dbo.Users','Role') IS NULL
                        BEGIN
                            ALTER TABLE [Users] ADD [Role] nvarchar(max) NOT NULL CONSTRAINT DF_Users_Role DEFAULT('Customer')
                        END";

            await context.Database.ExecuteSqlRawAsync(sql);
        }
        catch
        {
            // ignore if unable to alter (migration approach preferred)
        }

        // Seed a demo product
        if (!await context.Products.AnyAsync())
        {
            var product = new Product
            {
                Name = "Demo Product",
                Description = "A seeded demo product",
                Price = 49.99m,
                Category = "demo"
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            // seed inventory
            context.Inventories.Add(new Inventory { ProductId = product.Id, Quantity = 20 });
            await context.SaveChangesAsync();
        }

        // Seed a demo user
        if (!await context.Users.AnyAsync())
        {
            var user = new User
            {
                Name = "Demo User",
                Email = "demo@local",
                Password = PasswordHasher.Hash("Password123")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Seed an admin user if not present
        if (!await context.Users.AnyAsync(u => u.Email == "admin@local"))
        {
            var admin = new User
            {
                Name = "Administrator",
                Email = "admin@local",
                Password = PasswordHasher.Hash("Admin123!"),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
