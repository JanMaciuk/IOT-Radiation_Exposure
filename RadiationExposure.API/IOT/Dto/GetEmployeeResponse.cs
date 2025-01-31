namespace IOT.Dto;

public class GetEmployeeResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required double RadiationDoseInLastMonth { get; set; }
    public required double RadiationDoseInLastWeek { get; set; }
    public required DateTime? LastEntranceDate { get; set; }
    public required int? LastZoneId { get; set; }
    public required string? LastZoneName { get; set; }
}