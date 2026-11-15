using Microsoft.EntityFrameworkCore;
using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepositroy.Domain
{
    [Table("Inscripciones")]
    [Index(nameof(Legajo), nameof(CarreraId), IsUnique = true)] // un alumno no se inscribe dos veces a la misma carrera
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        // FK al alumno (por legajo)
        [Required(ErrorMessage = "El legajo es obligatorio")]
        [StringLength(20)]
        public string Legajo { get; set; } = string.Empty;

        [Required]
        public int CarreraId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        // Navegación
        [ForeignKey(nameof(Legajo))]
        [JsonIgnore]
        public Alumno? Alumno { get; set; }

        [ForeignKey(nameof(CarreraId))]
        [JsonIgnore]
        public Carrera? Carrera { get; set; }
    }
}
    
