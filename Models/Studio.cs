namespace IIS.Models;


public class Studio
{
    public int Id { get; set; } 
    public string Name { get; set; }

    public ICollection<User> UsersAssigned { get; set; }
    public ICollection<Equipment> OwnedEquipment { get; set; }
}