﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IOT.Data;
using IOT.Dto;
using IOT.Models;

namespace IOT.Controllers;

[ApiController]
public class IotController : ControllerBase
{
    private readonly AppDbContext _context;

    public IotController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("zones")]
    [Tags("Zones")]
    public async Task<ActionResult<Zone>> GetAllZones()
    {
        var zones = await _context.Zones.ToListAsync();
        return Ok(zones);
    }

    [HttpGet("zones/{id}")]
    [Tags("Zones")]
    public async Task<ActionResult<Zone>> GetZoneById(int id)
    {
        var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == id);
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
                EmployeeName = er.Employee.Name,
                EntryTime = er.EntranceTime,
                ExitTime = er.ExitTime,
                Duration = CalculateDuration(er.EntranceTime, er.ExitTime),
                RadiationDose = CalculateRadiationDose(er.Zone.Radiation, CalculateDuration(er.EntranceTime, er.ExitTime)),
            })
            .OrderBy(er => er.ExitTime)
            .ToListAsync();

        return Ok(entrances);
    }

    [HttpGet("employees")]
    [Tags("Employees")]
    public async Task<ActionResult<EmployeeEntrance>> GetAllEmployees()
    {
        var employee = await _context.EmployeeEntrance.ToListAsync();
        return Ok(employee);
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
                RadiationDose = CalculateRadiationDose(er.Zone.Radiation, CalculateDuration(er.EntranceTime, er.ExitTime)),
                EmployeeName = string.Join(' ', er.Employee.Name, er.Employee.Surname),
            })
            .ToListAsync();

        return Ok(employee);
    }
    
    private static double CalculateDuration(DateTime entranceTime, DateTime? exitTime)
    {
        return exitTime.HasValue
            ? (exitTime.Value - entranceTime).TotalMinutes
            : (DateTime.UtcNow - entranceTime).TotalMinutes;
    }
    
    private static double CalculateRadiationDose(double radiation, double duration)
    {
        return radiation * (duration / 60);
    }
}