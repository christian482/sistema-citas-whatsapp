using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace citas.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del doctor es requerido")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La especialidad del doctor es requerida")]
        public string Specialty { get; set; } = string.Empty;
        
        // Propiedad de navegación - ignorar durante escritura si es null para evitar referencias circulares
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
