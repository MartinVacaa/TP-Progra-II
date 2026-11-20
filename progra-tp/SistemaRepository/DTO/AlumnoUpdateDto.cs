using System.ComponentModel.DataAnnotations;

namespace WebApiSistema.DTO
{
    // Solo los campos que realmente actualiza AlumnoRepository.UpdateAlumno
    public class AlumnoUpdateDto
    {
        [Required, MaxLength(50)] public string Nombre { get; set; } = null!;
        [Required, MaxLength(50)] public string Apellido { get; set; } = null!;
        [Required, MaxLength(10)] public string Dni { get; set; } = null!;
        [Required, EmailAddress, MaxLength(120)] public string Email { get; set; } = null!;
    }
}