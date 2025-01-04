using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IOT.Data;
using IOT.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IOT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IotController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("zone")]
        public async Task<IActionResult> GetAllZones()
        {
            var zones = await _context.Zones.ToListAsync();
            return Ok(zones);
        }

        [HttpGet("zone/{id}")]
        public async Task<IActionResult> GetZoneById(int id)
        {
            var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == id);
            if (zone == null) { 
                return NotFound();
            }

            return Ok(zone);    
        }

        [HttpGet("zone/{id}/info")]
        public async Task<IActionResult> GetZoneInfoById(int id)
        {
            var logs = await _context.EmployeesRadiations
                .Where(er => er.FkZone == id)
                .Include(er => er.Employee)
                .Select(er => new
                {
                    EmployeeName = $"{er.Employee.Name} {er.Employee.Surname}",
                    EntryTime = er.EntranceTime,
                    ExitTime = er.ExitTime,
                    Duration = er.ExitTime.HasValue
                        ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                        : (DateTime.UtcNow - er.EntranceTime).TotalMinutes,
                    RadiationDose = er.Zone.Radiation * ((er.ExitTime.HasValue
                        ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                        : (DateTime.UtcNow - er.EntranceTime).TotalMinutes) / 60),
                })
                .ToListAsync();

            var totalOccupants = logs.Count;
            return Ok(new { Logs = logs, TotalOccupants = totalOccupants });
        }

        [HttpGet("employee")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var empolyee = await _context.EmployeesRadiations.ToListAsync();

            return Ok(empolyee);
        }

        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var empolyee = await _context.EmployeesRadiations.FirstOrDefaultAsync(e => e.Id == id);

            if (empolyee == null) {
                return NotFound();
            }

            return Ok(empolyee);
        }

        [HttpGet("employee-radiation/{id}")]
        public async Task<IActionResult> GetEmployeeRadiation(int id)
        {
            var empolyee = await _context.EmployeesRadiations
                .Where(e => e.FkEmployee == id)
                .Include(e=> e.Zone)
                .Select(er => new
                {
                    ZoneName = er.Zone.Info,
                    EntryTime = er.EntranceTime,
                    ExitTime = er.ExitTime,
                    Duration = er.ExitTime.HasValue
                                    ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                                    : (DateTime.UtcNow - er.EntranceTime).TotalMinutes,
                    RadiationDose = er.Zone.Radiation * ((er.ExitTime.HasValue
                                    ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                                    : (DateTime.UtcNow - er.EntranceTime).TotalMinutes) / 60)
                })
        .ToListAsync();

            return Ok(empolyee);
        }

        [HttpGet("employee-radiation/{id}today")]
        public async Task<IActionResult> GetEmployeeRadiationToday(int id)
        {
            var today = DateTime.UtcNow.Date;
            var empolyee = await _context.EmployeesRadiations
                .Where(er => er.FkEmployee == id && er.EntranceTime.Date == today)
                .Include(er => er.Zone)
                .Select(er => new
                {
                    ZoneName = er.Zone.Info,
                    EntryTime = er.EntranceTime,
                    ExitTime = er.ExitTime,
                    Duration = er.ExitTime.HasValue
                        ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                        : (DateTime.UtcNow - er.EntranceTime).TotalMinutes,
                    RadiationDose = er.Zone.Radiation * ((er.ExitTime.HasValue
                        ? (er.ExitTime.Value - er.EntranceTime).TotalMinutes
                        : (DateTime.UtcNow - er.EntranceTime).TotalMinutes) / 60)
                })
                .ToListAsync();

            return Ok(empolyee);
        }
    }
}
