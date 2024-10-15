using Microsoft.Build.Framework;

namespace IIS.Models;

public class EquipmentType
{
    public required int Id { get; set; } 
    
    public required string Name { get; set; }

    public ICollection<Equipment> EquipmentItems { get; set; } =
        new List<Equipment>(); // Equipments of this type (probably could be an enum as well)
}