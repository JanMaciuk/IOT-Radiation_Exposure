using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOT.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname{ get; set; }
        [Required]
        public string Card { get; set; }
    }

    public class Zone 
    {
        public int Id { get; set; }
        [Required]
        public string Info { get; set; }
        [Required]
        public float Radiation { get; set; }
    }
    public class EmployeesRadiation
    {
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int FkEmployee { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("Zone")]
        public int FkZone { get; set; }
        public Zone Zone { get; set; }

        [Required]
        public DateTime EntranceTime { get; set; }

        public DateTime? ExitTime { get; set; }
    }
}
