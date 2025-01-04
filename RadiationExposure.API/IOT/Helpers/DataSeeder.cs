using Bogus;
using IOT.Data;
using IOT.Models;

namespace IOT.Helpers;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Zones.Any())
        {
            return;
        }

        var zones = new List<Zone>
        {
            new Zone { Info = "Zone 32b", Radiation = 0.1f },
            new Zone { Info = "Zone 36z", Radiation = 0.35f },
            new Zone { Info = "Zone 12a", Radiation = 0.2f },
        };
        _context.Zones.AddRange(zones);

        var employeeFaker = new Faker<Employee>()
            .RuleFor(e => e.Name, f => f.Name.FullName())
            .RuleFor(e => e.Surname, f => f.Name.LastName())
            .RuleFor(e => e.Card, f => f.Random.Long(000_000_000, 999_999_999).ToString());

        var employees = employeeFaker.Generate(15);
        _context.Employees.AddRange(employees);
        _context.SaveChanges();

        var entranceFaker = new Faker<EmployeesRadiation>()
            .RuleFor(e => e.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(e => e.ZoneId, f => f.PickRandom(zones).Id)
            .RuleFor(e => e.EntranceTime, f => f.Date.Recent(30))
            .RuleFor(e => e.ExitTime, (f, er) => er.EntranceTime.AddMinutes(f.Random.Double(1, 60)));

        var entrances = entranceFaker.Generate(300);
        _context.EmployeesRadiations.AddRange(entrances);

        _context.SaveChanges();
    }
}