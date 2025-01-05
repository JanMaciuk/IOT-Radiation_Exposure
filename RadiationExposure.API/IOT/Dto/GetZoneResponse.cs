namespace IOT.Dto;

public class GetZoneResponse
{
    public int Id { get; set; }
    public GetEntranceResponse[] Entrances { get; set; }
}