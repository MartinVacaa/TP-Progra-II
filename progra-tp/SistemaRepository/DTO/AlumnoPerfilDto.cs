namespace SistemaRepository.DTO
{
    // Datos del alumno logueado para el Portal del Alumno
    public class AlumnoPerfilDto
    {
        public string Legajo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Carrera { get; set; }
        public int MateriasCursando { get; set; }
    }
}
