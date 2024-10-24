using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IIS.Models;

public class EquipmentType
{
    public int Id { get; set; }
    [DisplayName("Equipment Type")]
    [StringLength(100)]
    public required string Name { get; set; }

    public ICollection<Equipment> EquipmentItems { get; set; } = new List<Equipment>();
}