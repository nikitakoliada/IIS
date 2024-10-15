namespace IIS.Models;

public class EquipmentType
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Equipment> EquipmentItems { get; set; } = new List<Equipment>();
}