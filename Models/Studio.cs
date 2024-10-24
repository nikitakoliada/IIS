using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IIS.Models;

public class Studio
{
    public int Id { get; set; }
    [DisplayName("Studio Name")]
    [StringLength(200)]
    public required string Name { get; set; }

    public ICollection<User> UsersAssigned { get; set; } = new List<User>();
    public ICollection<Equipment> OwnedEquipment { get; set; } = new List<Equipment>();
}