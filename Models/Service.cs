using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace citas.Models
{
    public class Service
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del servicio es requerido")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El ID del negocio es requerido")]
        public int BusinessId { get; set; }
        
        // Propiedad de navegación - ignorar en la deserialización para evitar conflictos
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Business? Business { get; set; }
    }
}
