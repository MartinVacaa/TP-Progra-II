using System.ComponentModel.DataAnnotations;

namespace WebApiSistema.DTO
{
    public class AlumnoDto
    {

       
        public string Legajo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string? Email { get; set; }
        public string NombreUsuario { get; set; } = null!;
    }
}
