using System.ComponentModel.DataAnnotations;

namespace IIS.Models;

public class EquipmentType
{
    public int Id { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; }

    public ICollection<Equipment> EquipmentItems { get; set; } = new List<Equipment>();
}