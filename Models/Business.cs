using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace citas.Models
{
    public class Business
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del negocio es requerido")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Las horas de trabajo son requeridas")]
        public string WorkingHours { get; set; } = string.Empty; // Ejemplo: "09:00-18:00"
        
        [Required(ErrorMessage = "Los días de trabajo son requeridos")]
        public string WorkingDays { get; set; } = string.Empty; // Ejemplo: "Lunes-Viernes"
        
        // Propiedad de navegación - ignorar durante escritura si es null para evitar referencias circulares
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<Service>? Services { get; set; }
    }
}
