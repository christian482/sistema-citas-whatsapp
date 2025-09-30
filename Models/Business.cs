using System.Collections.Generic;

namespace citas.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WorkingHours { get; set; } // Ejemplo: "09:00-18:00"
        public string WorkingDays { get; set; } // Ejemplo: "Lunes-Viernes"
        public ICollection<Service> Services { get; set; }
    }
}
