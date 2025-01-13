namespace IOT.Dto;

public class GetZoneResponse
{
    public required int Id { get; set; }
    public required string ZoneName { get; set; }
    public required decimal Radiation { get; set; }
    public required DateTime? LastEntrance { get; set; }
    public required int EmployeesInsideNow { get; set; }
}