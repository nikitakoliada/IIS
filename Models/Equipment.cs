namespace IIS.Models;

public class Equipment
{
    public required int Id { get; set; } 
    public required int CreationYear { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public string? Image { get; set; }
    public TimeSpan? MaxRentalTime { get; set; }

    public required int StudioId { get; set; }
    public required Studio Studio { get; set; } = null!;
    public required int EquipmentTypeId { get; set; }
    public required EquipmentType EquipmentType { get; set; } = null!;
    
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
    public ICollection<User> UsersForbiddenToBorrow { get; set; } = new List<User>();
    public ICollection<RentalDayInterval> RentalDayIntervals { get; set; } = new List<RentalDayInterval>();
}
