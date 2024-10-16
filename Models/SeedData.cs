using IIS.Data;
using Microsoft.EntityFrameworkCore;

namespace IIS.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (context.EquipmentTypes.Any())
        {
            return; // DB has been seeded
        }

        context.EquipmentTypes.AddRange(
            new EquipmentType
            {
                Name = "Laptop"
            },
            new EquipmentType
            {
                Name = "Mouse"
            },
            new EquipmentType
            {
                Name = "Keyboard"
            }
        );
        
        context.Studios.AddRange(
            new Studio
            {
                Name = "Studio A"
            },
            new Studio
            {
                Name = "Studio B"
            },
            new Studio
            {
                Name = "Studio C"
            }
        );
        
        context.SaveChanges();
        
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
            new Equipment
            {
                Name = "Razer BlackWidow",
                ManufactureYear = 2019,
                PurchaseDate = new DateTime(2019, 1, 1),
                StudioId = context.Studios.First().Id,
                EquipmentTypeId = context.EquipmentTypes.Skip(2).First().Id
            },
            new Equipment
            {
                Name = "MacBook Pro 16",
                ManufactureYear = 2020,
                PurchaseDate = new DateTime(2020, 1, 1),
                StudioId = context.Studios.Skip(1).First().Id,
                EquipmentTypeId = context.EquipmentTypes.First().Id
            },
            new Equipment
            {
                Name = "Logitech G Pro",
                ManufactureYear = 2021,
                PurchaseDate = new DateTime(2021, 1, 1),
                StudioId = context.Studios.Skip(1).First().Id,
                EquipmentTypeId = context.EquipmentTypes.Skip(1).First().Id
            },
            new Equipment
            {
                Name = "Razer Huntsman",
                ManufactureYear = 2020,
                PurchaseDate = new DateTime(2020, 1, 1),
                StudioId = context.Studios.Skip(1).First().Id,
                EquipmentTypeId = context.EquipmentTypes.Skip(2).First().Id
            },
            new Equipment
            {
                Name = "Dell XPS 15",
                ManufactureYear = 2019,
                PurchaseDate = new DateTime(2019, 1, 1),
                StudioId = context.Studios.Skip(2).First().Id,
                EquipmentTypeId = context.EquipmentTypes.First().Id
            },
            new Equipment
            {
                Name = "Logitech G203",
                ManufactureYear = 2020,
                PurchaseDate = new DateTime(2020, 1, 1),
                StudioId = context.Studios.Skip(2).First().Id,
                EquipmentTypeId = context.EquipmentTypes.Skip(1).First().Id
            },
            new Equipment
            {
                Name = "Razer Ornata",
                ManufactureYear = 2021,
                PurchaseDate = new DateTime(2021, 1, 1),
                StudioId = context.Studios.Skip(2).First().Id,
                EquipmentTypeId = context.EquipmentTypes.Skip(2).First().Id
            }
        );
        
        context.SaveChanges();
        
        context.RentalDayIntervals.AddRange(
            new RentalDayInterval
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(16, 0),
                Place = "Room 101",
                EquipmentId = context.Equipments.First().Id
            },
            new RentalDayInterval
            {
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(16, 0),
                Place = "Room 101",
                EquipmentId = context.Equipments.First().Id
            },
            new RentalDayInterval
            {
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(16, 0),
                Place = "Room 101",
                EquipmentId = context.Equipments.First().Id
            },
            new RentalDayInterval
            {
                DayOfWeek = DayOfWeek.Thursday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(16, 0),
                Place = "Room 101",
                EquipmentId = context.Equipments.First().Id
            },
            new RentalDayInterval
            {
                DayOfWeek = DayOfWeek.Friday,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(19, 0),
                Place = "Room 101",
                EquipmentId = context.Equipments.First().Id
            }
        );

        context.SaveChanges();
    }
}