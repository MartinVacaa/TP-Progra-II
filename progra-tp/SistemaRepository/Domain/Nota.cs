using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepository.Domain
{
    [Table("Notas")]
    public class Nota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AlumnoId { get; set; }

        [Required]
        public int MateriaId { get; set; }

        [Required(ErrorMessage = "El tipo de nota es obligatorio")]
        [StringLength(20)]
        [RegularExpression("^(PARCIAL 1|PARCIAL 2|RECUPERATORIO|FINAL)$",
            ErrorMessage = "Tipo inválido: PARCIAL 1, PARCIAL 2, RECUPERATORIO o FINAL")]
        public string Tipo { get; set; } = string.Empty;

        // En C# una propiedad no puede llamarse igual que su clase,
        // por eso se llama Valor y se mapea a la columna "Nota".
        [Required]
        [Range(1, 10, ErrorMessage = "La nota debe estar entre 1 y 10")]
        [Column("Nota", TypeName = "decimal(4,2)")]
        public decimal Valor { get; set; }

        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        // Navegación
        [ForeignKey(nameof(AlumnoId))]
        [JsonIgnore]
        public Alumno? Alumno { get; set; }

        [ForeignKey(nameof(MateriaId))]
        [JsonIgnore]
        public Materia? Materia { get; set; }
    }
}