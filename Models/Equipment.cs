namespace IIS.Models;

public class Equipment
{
    public int Id { get; set; } 
    public int CreationYear { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Image { get; set; }
    public string RentalPlace { get; set; }
    public DateTime RentalDate { get; set; }

    public int StudioId { get; set; }
    public Studio Studio { get; set; } 
    public int EquipmentTypeId { get; set; }
    public EquipmentType EquipmentType { get; set; } 
}
