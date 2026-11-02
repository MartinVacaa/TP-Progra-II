using Microsoft.EntityFrameworkCore;
using SistemaRepository.Domain;
using SistemaRepositroy.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaRepository.Domain
{
    [Table("Alumnos")]
    [Index(nameof(UsuarioId), IsUnique = true)]
    [Index(nameof(Legajo), IsUnique = true)]
    [Index(nameof(Dni), IsUnique = true)]
    public class Alumno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El legajo es obligatorio")]
        [StringLength(20)]
        public string Legajo { get; set; } = string.Empty;

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

       

        // Navegación
        [ForeignKey(nameof(UsuarioId))]
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        [JsonIgnore]
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        [JsonIgnore]
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();
    }
}