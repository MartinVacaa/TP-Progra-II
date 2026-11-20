using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using System.Security.Claims;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ALUMNO")]
    public class MiAlumnoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MiAlumnoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> GetMiPerfil()
        {
            var nombreUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return Unauthorized();
            }

            var alumno = await _context.Alumnos
                .AsNoTracking()
                .Where(a => a.Usuario != null &&
                            a.Usuario.NombreUsuario == nombreUsuario)
                .Select(a => new
                {
                    a.Legajo,
                    a.Nombre,
                    a.Apellido,
                    a.Dni,
                    a.Email
                })
                .FirstOrDefaultAsync();

            if (alumno == null)
            {
                return NotFound("No se encontró el alumno");
            }

            return Ok(alumno);
        }
    

    [HttpGet("notas")]
        public async Task<IActionResult> GetMisNotas()
        {
            var nombreUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return Unauthorized();
            }

            var notas = await _context.Notas
                .AsNoTracking()
                .Where(n => n.Alumno != null &&
                            n.Alumno.Usuario != null &&
                            n.Alumno.Usuario.NombreUsuario == nombreUsuario)
                .OrderBy(n => n.Materia!.Nombre)
                .ThenBy(n => n.Fecha)
                .Select(n => new
                {
                    Materia = n.Materia!.Nombre,
                    Tipo = n.Tipo,
                    Nota = n.Valor,
                    Fecha = n.Fecha
                })
                .ToListAsync();

            return Ok(notas);
        }
        [HttpGet("materias")]
        public async Task<IActionResult> GetMisMaterias()
        {
            var nombreUsuario =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return Unauthorized();
            }

            var materias = await _context.Materias
                .AsNoTracking()
                .Where(m => _context.Inscripciones.Any(i =>
                    i.CarreraId == m.CarreraId &&
                    i.Alumno != null &&
                    i.Alumno.Usuario != null &&
                    i.Alumno.Usuario.NombreUsuario == nombreUsuario))
                .Select(m => new
                {
                    m.Id,
                    m.Nombre,
                    m.Anio,
                    Carrera = m.Carrera!.Nombre
                })
                .OrderBy(m => m.Anio)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            return Ok(materias);
        }
    }
}