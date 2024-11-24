using System.ComponentModel;
using System.Security.Claims;
using IIS.Models;

namespace IIS.ViewModels;

public class ListEquipmentViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    [DisplayName("Manufacture Year")]
    public int ManufactureYear { get; set; }
    [DisplayName("Purchase Date")]
    public DateTime PurchaseDate { get; set; }
    [DisplayName("Max Rental Time")]
    public string? MaxRentalTime { get; set; } // Human-readable rental time (e.g., "3 days")
    [DisplayName("Studio assigned to")]
    public string StudioName { get; set; }
    [DisplayName("Equipment Type")]
    public string EquipmentTypeName { get; set; }
    public bool CanBorrow { get; set; } // Whether the curre    nt user can borrow this equipment
    public bool CanEditOrDelete { get; set; } // Permission to edit or delete

    public static ListEquipmentViewModel FromEquipmentModel(Equipment model, string userId, ClaimsPrincipal User) =>
        new ListEquipmentViewModel()
        {
            Id = model.Id,
            Name = model.Name,
            ManufactureYear = model.ManufactureYear,
            PurchaseDate = model.PurchaseDate,
            MaxRentalTime = model.MaxRentalTime.HasValue ? $"{model.MaxRentalTime.HasValue} days" : "Unlimited",
            StudioName = model.Studio.Name,
            EquipmentTypeName = model.EquipmentType.Name,
            CanBorrow = model.UsersForbiddenToBorrow.All(u => u.Id != userId),
            CanEditOrDelete = User.IsInRole("Admin") || (User.IsInRole("Teacher") && model.OwnerId == userId)
        };
}