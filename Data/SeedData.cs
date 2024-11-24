using IIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IIS.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        context.Users.RemoveRange(context.Users);
        context.Studios.RemoveRange(context.Studios);
        context.EquipmentTypes.RemoveRange(context.EquipmentTypes);
        context.Equipments.RemoveRange(context.Equipments);
        context.RentalDayIntervals.RemoveRange(context.RentalDayIntervals);
        context.Reservations.RemoveRange(context.Reservations);
        
        await context.SaveChangesAsync();

        // Studios
        var studios = new List<Studio>
        {
            new Studio { Name = "Studio Alpha" },
            new Studio { Name = "Studio Beta" },
            new Studio { Name = "Studio Gamma" }
        };
        context.Studios.AddRange(studios);
        await context.SaveChangesAsync();

        // EquipmentTypes
        var equipmentTypes = new List<EquipmentType>
        {
            new EquipmentType { Name = "Laptop" },
            new EquipmentType { Name = "Camera" },
            new EquipmentType { Name = "Microphone" }
        };
        context.EquipmentTypes.AddRange(equipmentTypes);
        await context.SaveChangesAsync();

        // Users
        var teachers = new Dictionary<int, User>();
        foreach (var studio in studios)
        {
            var roles = new[] { "Admin", "StudioAdmin", "Teacher", "Student" };
            foreach (var role in roles)
            {
                var user = new User
                {
                    Name = $"{role ?? "Guest"} User {studio.Name}",
                    Email = $"{role?.ToLower() ?? "guest"}_{studio.Name.Split(" ")[1].ToLower()}@example.com",
                    UserName = $"{role?.ToLower() ?? "guest"}_{studio.Name.Split(" ")[1].ToLower()}@example.com",
                    Address = "Brno",
                    BirthDate = DateTime.Today.AddYears(-20),
                    AssignedStudioId = studio.Id,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "qwerty678")
                };

                await userManager.CreateAsync(user);
                if (role != null) await userManager.AddToRoleAsync(user, role);

                if (role == "Teacher")
                {
                    teachers[studio.Id] = user;
                }
            }
        }

        await context.SaveChangesAsync();

        // Equipments and Rental Day Intervals
        var equipments = new List<Equipment>();
        foreach (var studio in studios)
        {
            foreach (var equipmentType in equipmentTypes)
            {
                for (int i = 0; i < 2; i++) // Two equipments of each type per studio
                {
                    var equipment = new Equipment
                    {
                        Name = $"{equipmentType.Name} {i + 1} - {studio.Name}",
                        ManufactureYear = 2022 - i,
                        PurchaseDate = DateTime.Now.AddYears(-2 - i),
                        StudioId = studio.Id,
                        EquipmentTypeId = equipmentType.Id,
                        OwnerId = teachers[studio.Id].Id,
                    };
                    equipments.Add(equipment);
                }
            }
        }

        context.Equipments.AddRange(equipments);
        await context.SaveChangesAsync();

        foreach (var equipment in equipments)
        {
            context.RentalDayIntervals.AddRange(
                new RentalDayInterval
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(16, 0),
                    Place = "Room 101",
                    EquipmentId = equipment.Id
                },
                new RentalDayInterval
                {
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(17, 0),
                    Place = "Room 102",
                    EquipmentId = equipment.Id
                },
                new RentalDayInterval
                {
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(18, 0),
                    Place = "Room 103",
                    EquipmentId = equipment.Id
                }
            );
        }

        await context.SaveChangesAsync();

        // Borrow
        var borrow = equipments.Take(5).Select(equipment => new Borrow
        {
            UserId = context.Users.First().Id,
            EquipmentId = equipment.Id,
            FromDate = DateTime.Now.AddDays(25),
            ToDate = DateTime.Now.AddDays(30),
            State = Enums.BorrowState.Accepted
        });

        context.Reservations.AddRange(borrow);
        await context.SaveChangesAsync();
    }
}
