namespace IOT.Models;

public class Zone 
{
    public int Id { get; set; }
    public required string Info { get; set; }
    public required float Radiation { get; set; }

    public ICollection<EmployeeEntrance> Entrances { get; set; } = [];
}