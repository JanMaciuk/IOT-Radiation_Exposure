using IOT.Models;
using Microsoft.EntityFrameworkCore; 

namespace IOT.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<EmployeeEntrance> EmployeeEntrance { get; set; }

}