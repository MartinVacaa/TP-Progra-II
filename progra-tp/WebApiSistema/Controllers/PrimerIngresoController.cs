using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using SistemaRepository.Domain;
using System.ComponentModel.DataAnnotations;

namespace WebApiSistema.Controllers
{
    public class CambiarPasswordPrimerIngresoDto
    {
        [Required]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        public string PasswordTemporal { get; set; } = string.Empty;

        [Required]
        public string PasswordNueva { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/PrimerIngreso")]
    public class PrimerIngresoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public PrimerIngresoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword(
            [FromBody] CambiarPasswordPrimerIngresoDto datos)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.NombreUsuario == datos.Usuario.Trim());

            if (usuario == null || !usuario.Activo || !usuario.DebeCambiarPassword)
                return Unauthorized("Usuario o contraseña temporal incorrectos");

            PasswordVerificationResult resultado;
            try
            {
                resultado = _hasher.VerifyHashedPassword(
                    usuario, usuario.PasswordHash, datos.PasswordTemporal);
            }
            catch (FormatException)
            {
                return Unauthorized("Usuario o contraseña temporal incorrectos");
            }

            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized("Usuario o contraseña temporal incorrectos");

            if (datos.PasswordNueva.Length < 12)
                return BadRequest("La contraseña nueva debe tener al menos 12 caracteres");

            if (datos.PasswordNueva == datos.PasswordTemporal)
                return BadRequest("La nueva contraseña debe ser distinta de la temporal");

            usuario.PasswordHash = _hasher.HashPassword(usuario, datos.PasswordNueva);
            usuario.DebeCambiarPassword = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña actualizada. Iniciá sesión con la nueva contraseña." });
        }
    }
}
