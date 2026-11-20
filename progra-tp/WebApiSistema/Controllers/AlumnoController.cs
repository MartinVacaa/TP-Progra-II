using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaRepository.Domain;
using SistemaRepository.Services.Interfaces;
using SistemaRepositroy.Domain;
using System.Security.Claims;
using WebApiSistema.DTO;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ENCARGADO")]
    public class AlumnoController : ControllerBase
    {

        private readonly IServiceAlumno _service;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public AlumnoController(IServiceAlumno service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> GetAllASync() 
        {
            var alumnos= await _service.GetAllASync();
            return Ok(alumnos);
        }

        [HttpGet("{legajo}")]
        public async Task<IActionResult> GetPromedio(string legajo)
        {
            var resultado= await _service.GetPromedio(legajo);

            if (resultado <= 0)
            {
                return StatusCode(500, "El alumno no tiene notas cargadas");
            }

            return Ok(resultado);
        }

        [HttpPost]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> CreateAlumno([FromBody]AlumnoCreateDto dto)
        {

            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Rol = "ALUMNO"
            };
            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);



            var alumno = new Alumno
            {
                Legajo = dto.Legajo,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Dni = dto.Dni,
                Usuario = usuario
            };


            alumno.Inscripciones.Add(new Inscripcion
            {
                Legajo = dto.Legajo,
                CarreraId = dto.CarreraId,
                Fecha = DateTime.Today
            });


            var resultado = new AlumnoDto
            {

                Legajo = alumno.Legajo,
                Nombre = alumno.Nombre,
                Apellido = alumno.Apellido,
                Email = alumno.Email,
                Dni = alumno.Dni,
                NombreUsuario = alumno.Usuario.NombreUsuario
            };

            try
            {
                bool result = await _service.CreateAlumno(alumno);
                if (!result)
                {
                    return BadRequest($"No se ha podido crear el alumno: el legajo {dto.Legajo} ya existe");
                }
                return StatusCode(201, new { message = $"Se ha creado el alumno con exito", resultado });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("notas")]
        public async Task<IActionResult> AlumnosNotas()
        {
            var alumnos = await _service.AlumnoNotas();
            return Ok(alumnos);
        }
        [HttpDelete("{legajo}")]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> DeleteAlumnmo(string legajo)
        {
            bool result = await _service.DeleteAlumno(legajo);

            if (!result)
            {
                return BadRequest($"No se ha podido borrar el alumno con el LEGAJO: {legajo}");
            }

            return Ok("Se ha borrado exitosamente el alumno");
        }

        [HttpPut("{legajo}")]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> UpdateAlumno(string legajo, [FromBody] AlumnoUpdateDto dto)
        {
            
                var alumno = new Alumno
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Dni = dto.Dni,
                    Email = dto.Email
                };

                try
                {
                    bool actualizado = await _service.UpdateAlumno(legajo, alumno);

                    if (!actualizado)
                    {
                        return NotFound($"No se encontró el alumno con legajo: {legajo}");
                    }

                    return Ok("Alumno actualizado correctamente");
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
        }

        [HttpGet("{legajo}/materias")]
        public async Task<IActionResult> GetMateriasCursando(string legajo)
        {
            var cantidad = await _service.ContarMateriasCursando(legajo);
            return Ok(cantidad);
        }

        [HttpGet("perfil")]
        [Authorize(Roles = "ALUMNO")]
        public async Task<IActionResult> GetPerfil()
        {
            var nombreUsuario = User.FindFirst("sub")?.Value
                                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var perfil = await _service.GetPerfilPorUsuario(nombreUsuario ?? "");
            if (perfil == null)
            {
                return NotFound("No se encontró un alumno asociado a este usuario");
            }
            return Ok(perfil);
        }




    }
}

