using IIS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IIS.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        
        context.Users.RemoveRange(context.Users); // Remove all users
        
        // Seed Equipment Types
        if (!context.EquipmentTypes.Any())
        {
            context.EquipmentTypes.AddRange(
                new EquipmentType { Name = "Laptop" },
                new EquipmentType { Name = "Mouse" },
                new EquipmentType { Name = "Keyboard" }
            );

            await context.SaveChangesAsync();
        }

        // Seed Studios
        if (!context.Studios.Any())
        {
            context.Studios.AddRange(
                new Studio { Name = "Studio A" },
                new Studio { Name = "Studio B" },
                new Studio { Name = "Studio C" }
            );

            await context.SaveChangesAsync();
        }

        // Seed Equipments
        if (!context.Equipments.Any())
        {
            context.Equipments.AddRange(
                new Equipment
                {
                    Name = "Dell XPS 13",
                    ManufactureYear = 2021,
                    PurchaseDate = new DateTime(2021, 1, 1),
                    StudioId = context.Studios.First().Id,
                    EquipmentTypeId = context.EquipmentTypes.First().Id
                },
                new Equipment
                {
                    Name = "Logitech G502",
                    ManufactureYear = 2020,
                    PurchaseDate = new DateTime(2020, 1, 1),
                    StudioId = context.Studios.First().Id,
                    EquipmentTypeId = context.EquipmentTypes.Skip(1).First().Id
                },
                // ... (other equipment entries)
                new Equipment
                {
                    Name = "Razer Ornata",
                    ManufactureYear = 2021,
                    PurchaseDate = new DateTime(2021, 1, 1),
                    StudioId = context.Studios.Skip(2).First().Id,
                    EquipmentTypeId = context.EquipmentTypes.Skip(2).First().Id
                }
            );

            await context.SaveChangesAsync();
        }

        // Seed Rental Day Intervals
        if (!context.RentalDayIntervals.Any())
        {
            context.RentalDayIntervals.AddRange(
                new RentalDayInterval
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(16, 0),
                    Place = "Room 101",
                    EquipmentId = context.Equipments.First().Id
                },
                // ... (other rental day interval entries)
                new RentalDayInterval
                {
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(19, 0),
                    Place = "Room 101",
                    EquipmentId = context.Equipments.First().Id
                }
            );

            await context.SaveChangesAsync();
        }

        // Seed Users
        if (!context.Users.Any())
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var admin = new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                UserName = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Address = "123 Main St",
                BirthDate = DateTime.Today.AddYears(-18),
                EmailConfirmed = true,
            };

            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, "qwerty678");

            await userManager.CreateAsync(admin);
            await userManager.AddToRoleAsync(admin, "Admin"); // Assign Admin role

            // Add other users
            var john = new User
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                UserName = "johndoe@example.com",
                NormalizedEmail = "JOHNDOE@EXAMPLE.COM",
                NormalizedUserName = "JOHNDOE@EXAMPLE.COM",
                Address = "123 Main St",
                BirthDate = DateTime.Today.AddYears(-20),
            };

            var jane = new User
            {
                Name = "Jane Doe",
                Email = "janedoe@example.com",
                UserName = "janedoe@example.com",
                NormalizedEmail = "JANEDOE@EXAMPLE.COM",
                NormalizedUserName = "JANEDOE@EXAMPLE.COM",
                Address = "456 Main St",
                BirthDate = DateTime.Today.AddYears(-22)
            };

            await userManager.CreateAsync(john, "Password123!"); // Assign a password for John
            await userManager.CreateAsync(jane, "Password123!"); // Assign a password for Jane
            
            await context.SaveChangesAsync();
        }
    }
}
