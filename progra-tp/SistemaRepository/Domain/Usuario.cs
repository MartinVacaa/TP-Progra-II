using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SistemaRepository.Domain
{
    [Table("Usuarios")]
    [Index(nameof(NombreUsuario), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        // En C# una propiedad no puede llamarse igual que su clase,
        // por eso se llama NombreUsuario y se mapea a la columna "Usuario".
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [StringLength(50)]
        [Column("Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [JsonIgnore] // nunca se devuelve en las respuestas de la API
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [RegularExpression("^(ALUMNO|ENCARGADO)$", ErrorMessage = "El rol debe ser ALUMNO o ENCARGADO")]
        public string Rol { get; set; } = "ALUMNO";

        public bool Activo { get; set; } = true;

        // Navegación: si el usuario es alumno, sus datos
        [JsonIgnore]
        public Alumno? Alumno { get; set; }
    }
}