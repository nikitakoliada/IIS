namespace IIS.Models;

public class Borrow
{
    public int Id { get; set; } 
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string State { get; set; }  // could be an enum as well

    public string UserId { get; set; }
    public User User { get; set; } 
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
}