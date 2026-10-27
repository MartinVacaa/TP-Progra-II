using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepository.Domain
{
    [Table("Materias")]
    public class Materia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarreraId { get; set; }

        public int? ProfesorId { get; set; }   // puede no tener profesor asignado

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 10, ErrorMessage = "El año debe estar entre 1 y 10")]
        public byte Anio { get; set; }   // TINYINT en SQL Server

        // Navegación
        [ForeignKey(nameof(CarreraId))]
        [JsonIgnore]
        public Carrera? Carrera { get; set; }

        [ForeignKey(nameof(ProfesorId))]
        [JsonIgnore]
        public Profesor? Profesor { get; set; }

        [JsonIgnore]
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}