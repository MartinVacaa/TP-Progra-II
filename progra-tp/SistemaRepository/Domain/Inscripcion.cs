using Microsoft.EntityFrameworkCore;
using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepositroy.Domain
{
    [Table("Inscripciones")]
    [Index(nameof(AlumnoId), nameof(CarreraId), IsUnique = true)] // un alumno no se inscribe dos veces a la misma carrera
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AlumnoId { get; set; }

        [Required]
        public int CarreraId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        // Navegación
        [ForeignKey(nameof(AlumnoId))]
        [JsonIgnore]
        public Alumno? Alumno { get; set; }

        [ForeignKey(nameof(CarreraId))]
        
        public Carrera? Carrera { get; set; }
    }
}