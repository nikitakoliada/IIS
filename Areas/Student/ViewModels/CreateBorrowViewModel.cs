namespace IIS.Areas.Student.ViewModels;

public class CreateBorrowViewModel
{
    public int EquipmentId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}