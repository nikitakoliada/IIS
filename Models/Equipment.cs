namespace IIS.Models;

public class Equipment
{
    public int Id { get; set; } 
    public int CreationYear { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Image { get; set; }
    public TimeSpan MaxRentalTime { get; set; }

    public int StudioId { get; set; }
    public Studio Studio { get; set; } 
    public int EquipmentTypeId { get; set; }
    public EquipmentType EquipmentType { get; set; }
    
    public ICollection<Borrow> Borrows { get; set; }
    public ICollection<User> UsersForbiddenToBorrow { get; set; }
    public ICollection<RentalDayInterval> RentalDayIntervals { get; set; }
}
