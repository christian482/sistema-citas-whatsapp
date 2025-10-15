using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace citas.Models
{
    public class Client
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El teléfono del cliente es requerido")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email del cliente es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;
        
        // Propiedad de navegación - ignorar durante escritura si es null para evitar referencias circulares
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
