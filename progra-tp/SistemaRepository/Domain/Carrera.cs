using SistemaRepository.Domain;
using SistemaRepositroy.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepository.Domain
{
    [Table("Carreras")]
    public class Carrera
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 10, ErrorMessage = "La duración debe estar entre 1 y 10 años")]
        public byte DuracionAnios { get; set; }   // TINYINT en SQL Server

        // Navegación
        [JsonIgnore]
        public ICollection<Materia> Materias { get; set; } = new List<Materia>();

        [JsonIgnore]
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}