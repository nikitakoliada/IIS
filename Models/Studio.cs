namespace IIS.Models;

public class Studio
{
    public required int Id { get; set; } 
    public required string Name { get; set; }

    public ICollection<User> UsersAssigned { get; set; } = new List<User>();
    public ICollection<Equipment> OwnedEquipment { get; set; } = new List<Equipment>();
}