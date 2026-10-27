using Microsoft.EntityFrameworkCore;
using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepository.Domain
{
    [Table("Profesores")]
    [Index(nameof(Dni), IsUnique = true)]
    public class Profesor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(15)]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(60)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(60)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(120)]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string? Email { get; set; }

        // Navegación: materias que dicta
        [JsonIgnore]
        public ICollection<Materia> Materias { get; set; } = new List<Materia>();
    }
}
