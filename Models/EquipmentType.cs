namespace IIS.Models;

public class EquipmentType
{
    public int Id { get; set; } 
    public string Name { get; set; }

    public ICollection<Equipment> EquipmentItems { get; set; } // Equipments of this type (probably could be an enum as well)
}