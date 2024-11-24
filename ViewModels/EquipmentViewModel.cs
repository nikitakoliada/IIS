using System.ComponentModel;
using IIS.Models;

namespace IIS.ViewModels;

public class EquipmentViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [DisplayName ("Manufacture year")]
    public int ManufactureYear { get; set; }
    
    [DisplayName ("Purchase date")]

    public DateTime PurchaseDate { get; set; }

    public string? Image { get; set; }
    
    [DisplayName ("Max Rental Days")]
    public int? MaxRentalDays { get; set; }

    [DisplayName ("Studio")]
    public int StudioId { get; set; }

    [DisplayName ("Equipment Type")]
    public int EquipmentTypeId { get; set; }

    public List<RentalDayIntervalViewModel> RentalDayIntervals { get; set; }

    public List<UserViewModel>? UsersForbiddenToBorrow { get; set; }
}