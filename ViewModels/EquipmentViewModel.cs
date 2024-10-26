namespace IIS.ViewModels;

public class EquipmentViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ManufactureYear { get; set; }

    public DateTime PurchaseDate { get; set; }

    public string? Image { get; set; }

    public TimeSpan? MaxRentalTime { get; set; }

    public int StudioId { get; set; }

    public int EquipmentTypeId { get; set; }

    public List<RentalDayIntervalViewModel> RentalDayIntervals { get; set; } = new List<RentalDayIntervalViewModel>();

}