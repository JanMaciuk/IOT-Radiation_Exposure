namespace IOT.Models;

public class EmployeeEntrance
{
    public int Id { get; set; }

    public required int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public required int ZoneId { get; set; }
    public Zone Zone { get; set; }

    public required DateTime EntranceTime { get; set; }
    public DateTime? ExitTime { get; set; }
}