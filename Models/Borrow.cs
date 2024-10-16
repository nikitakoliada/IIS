using IIS.Enums;

namespace IIS.Models;

public class Borrow
{
    public int Id { get; set; }
    public required DateTime FromDate { get; set; }
    public required DateTime ToDate { get; set; }
    public required BorrowState State { get; set; }
    
    public required string UserId { get; set; }
    public required User User { get; set; } = null!;
    public required int EquipmentId { get; set; }
    public required Equipment Equipment { get; set; } = null!;
}