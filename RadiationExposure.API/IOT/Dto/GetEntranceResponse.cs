namespace IOT.Dto;

public class GetEntranceResponse
{
    public required int Id { get; set; }
    public required int ZoneId { get; set; }
    public required string ZoneName { get; set; }
    public required string EmployeeName { get; set; }
    public required DateTime EntryTime { get; set; }
    public required DateTime? ExitTime { get; set; }
    public required double Duration { get; set; }
    public required double RadiationDose { get; set; }
}