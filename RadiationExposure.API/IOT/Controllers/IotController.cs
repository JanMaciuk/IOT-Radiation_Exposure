using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IOT.Data;
using IOT.Dto;
using IOT.Models;

namespace IOT.Controllers;

record EmployeeStats
{
    public DateTime EntranceTime { get; init; }
    public DateTime ExitTime { get; init; }
    public double Radiation { get; init; }
}

record DashboardStats
{
    public int TotalEmployees { get; init; }
    public int TotalZones { get; init; }
    public int EntrancesToday { get; init; }
    public int WorkersAboveRadiationLimit { get; init; }
}

[ApiController]
public class IotController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly DatabaseBackupService _databaseBackupService;

    public IotController(AppDbContext context, DatabaseBackupService databaseBackupService)
    {
        _context = context;
        _databaseBackupService = databaseBackupService;
    }
    
    [HttpPost("db-backup")]
    [Tags("Management")]
    public async Task<ActionResult> CreateDbBackup()
    {
        try
        {
            await _databaseBackupService.CreateDbBackupAsync();
        }
        catch
        {
            return BadRequest("Failed to create database backup!");
        }
        return Ok("Database backup created successfully!");
    } 

    [HttpGet("last-month-entrances")]
    [Tags("Dashboard")]
    public async Task<ActionResult> GetLastMonthEntrances()
    {
        var startDate = DateTime.UtcNow.AddMonths(-1).Date;
        var endDate = DateTime.UtcNow.Date;

        var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(offset => startDate.AddDays(offset))
            .ToList();

        var entrances = await _context.EmployeeEntrance
            .Where(e => e.EntranceTime >= startDate)
            .GroupBy(e => e.EntranceTime.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count(),
            })
            .ToListAsync();

        var result = dateRange.GroupJoin(
                entrances,
                date => date,
                entrance => entrance.Date,
                (date, entranceGroup) => new
                {
                    Date = date,
                    Count = entranceGroup.Sum(e => e.Count)
                })
            .ToList();

        return Ok(result);
    }

    [HttpGet("dashboard")]
    [Tags("Dashboard")]
    public async Task<ActionResult> GetDashboardStats()
    {
        var workersRadiationData = await _context.EmployeeEntrance
            .Where(e => e.ExitTime != null)
            .Select(e => new
            {
                Employee = e.Employee,
                Radiation = e.Zone.Radiation,
                Duration = CalculateDuration(e.EntranceTime, e.ExitTime),
            })
            .GroupBy(e => e.Employee)
            .ToArrayAsync();

        var workersSumRadiation = workersRadiationData
            .Select(w => w.Sum(e => CalculateRadiationDose(e.Radiation, e.Duration)))
            .ToArray();

        var stats = new DashboardStats
        {
            TotalEmployees = await _context.Employees.CountAsync(),
            TotalZones = await _context.Zones.CountAsync(),
            EntrancesToday = await _context.EmployeeEntrance
                .CountAsync(e => e.EntranceTime.Date == DateTime.UtcNow.Date),
            WorkersAboveRadiationLimit = workersSumRadiation.Count(r => r > 5),
        };

        return Ok(stats);
    }

    [HttpGet("zones")]
    [Tags("Zones")]
    public async Task<ActionResult<Zone>> GetAllZones()
    {
        var zones = await _context.Zones
            .Select(z => new GetZoneResponse
            {
                Id = z.Id,
                Radiation = Convert.ToDecimal(z.Radiation),
                ZoneName = z.Info,
                LastEntrance = z.Entrances
                    .OrderByDescending(e => e.EntranceTime)
                    .Select(e => e.EntranceTime)
                    .FirstOrDefault(),
                EmployeesInsideNow = z.Entrances.Count(e => e.ExitTime == null),
            })
            .ToListAsync();

        return Ok(zones);
    }

    [HttpGet("zones/{id}")]
    [Tags("Zones")]
    public async Task<ActionResult<Zone>> GetZoneById(int id)
    {
        var zone = await _context.Zones
            .Include(z => z.Entrances)
            .FirstOrDefaultAsync(z => z.Id == id);
        if (zone == null)
        {
            return NotFound();
        }

        return Ok(zone);
    }

    [HttpGet("zones/{id}/entrances")]
    [Tags("Zones")]
    public async Task<ActionResult<List<GetEntranceResponse>>> GetEntrancesForZone(int id)
    {
        var entrances = await _context.EmployeeEntrance.Where(er => er.ZoneId == id)
            .Select(er => new GetEntranceResponse
            {
                Id = er.Id,
                ZoneId = er.ZoneId,
                ZoneName = er.Zone.Info,
                EmployeeName = string.Join(' ', er.Employee.Name, er.Employee.Surname),
                EntryTime = er.EntranceTime,
                ExitTime = er.ExitTime,
                Duration = CalculateDuration(er.EntranceTime, er.ExitTime),
                RadiationDose =
                    CalculateRadiationDose(er.Zone.Radiation, CalculateDuration(er.EntranceTime, er.ExitTime)),
            })
            .OrderBy(er => er.ExitTime)
            .ToListAsync();

        return Ok(entrances);
    }

    [HttpGet("employees")]
    [Tags("Employees")]
    public async Task<ActionResult<GetEmployeeResponse>> GetAllEmployees()
    {
        var employeeLastEntrancesDict = await _context.EmployeeEntrance
            .Include(e => e.Zone)
            .Where(e => e.EntranceTime == _context.EmployeeEntrance
                .Where(e2 => e2.EmployeeId == e.EmployeeId)
                .Max(e2 => e2.EntranceTime))
            .ToDictionaryAsync(e => e.EmployeeId, e => new EmployeeLastEntrance(e.EntranceTime, e.ZoneId, e.Zone.Info));

        var weekAgoDate = DateTime.UtcNow.AddDays(-7);
        var monthAgoDate = DateTime.UtcNow.AddMonths(-1);

        var employees = await _context.Employees.ToListAsync();

        var employeeResponses = employees.Select(e =>
            new GetEmployeeResponse
            {
                Id = e.Id,
                Name = e.Name,
                Surname = e.Surname,
                LastEntranceDate = employeeLastEntrancesDict.ContainsKey(e.Id)
                    ? employeeLastEntrancesDict[e.Id].LastEntranceDate
                    : null,
                LastZoneId = employeeLastEntrancesDict.ContainsKey(e.Id)
                    ? employeeLastEntrancesDict[e.Id].LastZoneId
                    : null,
                LastZoneName = employeeLastEntrancesDict.ContainsKey(e.Id)
                    ? employeeLastEntrancesDict[e.Id].LastZoneName
                    : null,
                RadiationDoseInLastMonth = e.Entrances
                    .Where(ee => ee.EntranceTime >= monthAgoDate && ee.ExitTime != null)
                    .Select(ee => new EmployeeStats
                    {
                        Radiation = ee.Zone.Radiation,
                        ExitTime = ee.ExitTime.Value,
                        EntranceTime = ee.EntranceTime,
                    })
                    .Sum(ee => CalculateRadiationDose(ee.Radiation, CalculateDuration(ee.EntranceTime, ee.ExitTime))),
                RadiationDoseInLastWeek = e.Entrances
                    .Where(ee => ee.EntranceTime >= weekAgoDate && ee.ExitTime != null)
                    .Select(ee => new EmployeeStats
                    {
                        Radiation = ee.Zone.Radiation,
                        ExitTime = ee.ExitTime.Value,
                        EntranceTime = ee.EntranceTime,
                    })
                    .Sum(ee => CalculateRadiationDose(ee.Radiation, CalculateDuration(ee.EntranceTime, ee.ExitTime))),
            }).ToList();

        return Ok(employeeResponses);
    }

    [HttpGet("employees/{id}")]
    [Tags("Employees")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpGet("entrances/{id}")]
    [Tags("Entrances")]
    public async Task<ActionResult<GetEntranceResponse>> GetEmployeeRadiation(int id, [FromQuery] DateTime? date)
    {
        var query = _context.EmployeeEntrance
            .Where(e => e.EmployeeId == id);

        if (date.HasValue)
        {
            query = query.Where(e => e.EntranceTime.Date == date.Value.Date);
        }

        var employee = await query
            .Select(er => new GetEntranceResponse
            {
                Id = er.Id,
                ZoneId = er.ZoneId,
                ZoneName = er.Zone.Info,
                EntryTime = er.EntranceTime,
                ExitTime = er.ExitTime,
                Duration = CalculateDuration(er.EntranceTime, er.ExitTime),
                RadiationDose =
                    CalculateRadiationDose(er.Zone.Radiation, CalculateDuration(er.EntranceTime, er.ExitTime)),
                EmployeeName = string.Join(' ', er.Employee.Name, er.Employee.Surname),
            })
            .ToListAsync();

        return Ok(employee);
    }

    private static double CalculateDuration(DateTime entranceTime, DateTime? exitTime)
    {
        DateTime referenceTime = exitTime ?? DateTime.UtcNow;
        double duration = (referenceTime - entranceTime).TotalMinutes;
        return Math.Round(duration, 1, MidpointRounding.AwayFromZero);
    }

    private static double CalculateRadiationDose(double radiation, double duration)
    {
        return double.Round(radiation * (duration / 60), 3, MidpointRounding.AwayFromZero);
    }
}