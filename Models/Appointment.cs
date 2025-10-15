using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace citas.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El título de la cita es requerido")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de la cita es requerida")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "El ID del servicio es requerido")]
        public int ServiceId { get; set; }
        
        // Propiedades de navegación - ignorar durante escritura si son null para evitar referencias circulares
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Service? Service { get; set; }
        
        [Required(ErrorMessage = "El ID del cliente es requerido")]
        public int ClientId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Client? Client { get; set; }
        
        [Required(ErrorMessage = "El ID del doctor es requerido")]
        public int DoctorId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Doctor? Doctor { get; set; }
        
        [Required(ErrorMessage = "El ID del negocio es requerido")]
        public int BusinessId { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Business? Business { get; set; }
        
        public bool IsCancelled { get; set; }
    }
}
