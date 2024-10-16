using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IIS.Models;

public class Equipment
{
    public int Id { get; set; }
    [DisplayName("Name")]
    [StringLength(200)]
    public required string Name { get; set; }
    [DisplayName("Year of Manufacture")]
    public required int ManufactureYear { get; set; }
    [DisplayName("Purchase Date")]
    public required DateTime PurchaseDate { get; set; }
    public string? Image { get; set; }
    [DisplayName("Maximum Rental Time")]
    public TimeSpan? MaxRentalTime { get; set; }

    public required int StudioId { get; set; }
    public Studio Studio { get; set; } = null!;
    public required int EquipmentTypeId { get; set; }
    public EquipmentType EquipmentType { get; set; } = null!;
    
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
    public ICollection<User> UsersForbiddenToBorrow { get; set; } = new List<User>();
    public ICollection<RentalDayInterval> RentalDayIntervals { get; set; } = new List<RentalDayInterval>();
}
