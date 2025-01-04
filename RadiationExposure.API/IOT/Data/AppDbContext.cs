using Microsoft.EntityFrameworkCore; 

namespace IOT.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Employee> Employees { get; set; }
        public DbSet<Models.Zone> Zones { get; set; }
        public DbSet<Models.EmployeesRadiation> EmployeesRadiations { get; set; }

    }
}
