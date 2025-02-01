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
            new Zone { Info = "A", Radiation = 0.1f },
            new Zone { Info = "B", Radiation = 0.2f },
            new Zone { Info = "C", Radiation = 0.3f },
            new Zone { Info = "D", Radiation = 0.4f },
            new Zone { Info = "E", Radiation = 0.5f },
            new Zone { Info = "F", Radiation = 0.6f },
            new Zone { Info = "G", Radiation = 0.7f },
            new Zone { Info = "H", Radiation = 0.8f },
            new Zone { Info = "I", Radiation = 0.9f },
            new Zone { Info = "J", Radiation = 1.0f },
        };
        _context.Zones.AddRange(zones);

        var employeeFaker = new Faker<Employee>("pl")
            .RuleFor(e => e.Name, f => f.Name.FirstName())
            .RuleFor(e => e.Surname, f => f.Name.LastName())
            .RuleFor(e => e.Card, f => f.Random.Long(000_000_000, 999_999_999).ToString());

        //var employees = employeeFaker.Generate(15);
        var employees = new List<Employee>
        {
            new Employee {Name ="Jan", Surname = "Maciuk", Card = "111214734182"},
            new Employee {Name ="Jan", Surname = "Kowalski", Card = "25118719434160"},
            new Employee {Name ="Alicja", Surname = "Maciuk", Card = "111214734183"},
            new Employee {Name ="Anna", Surname = "Maciuk", Card = "111214734184"},
        };
        _context.Employees.AddRange(employees);
        _context.SaveChanges();

        var entranceFaker = new Faker<EmployeeEntrance>()
            .RuleFor(e => e.EmployeeId, f => f.PickRandom(employees).Id)
            .RuleFor(e => e.ZoneId, f => f.PickRandom(zones).Id)
            .RuleFor(e => e.EntranceTime, f => f.Date.Recent(30))
            .RuleFor(e => e.ExitTime, (f, er) => er.EntranceTime.AddMinutes(f.Random.Double(1, 60)));

        var entrances = entranceFaker.Generate(300);
        _context.EmployeeEntrance.AddRange(entrances);

        _context.SaveChanges();
    }
}