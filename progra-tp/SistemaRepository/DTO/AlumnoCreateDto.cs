using System.ComponentModel.DataAnnotations;

namespace WebApiSistema.DTO
{
    public class AlumnoCreateDto
    {
        
        [Required, MaxLength(50)] public string Nombre { get; set; } = null!;
        [Required, MaxLength(50)] public string Apellido { get; set; } = null!;
        [Required, MaxLength(10)] public string Dni { get; set; } = null!;
        [EmailAddress] public string? Email { get; set; }

        [Required, MaxLength(50)] public string NombreUsuario { get; set; } = null!;
        [Required, MinLength(6)] public string Password { get; set; } = null!;
    }
}
