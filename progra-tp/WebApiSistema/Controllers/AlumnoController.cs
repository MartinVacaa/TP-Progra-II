using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaRepository.Domain;
using SistemaRepository.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebApiSistema.DTO;
using Microsoft.AspNetCore.Authorization;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
               
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Dni = dto.Dni,
                Usuario = usuario
            };
            var resultado = new AlumnoDto
            {
                
                Legajo=alumno.Legajo,
                Nombre = alumno.Nombre,
                Apellido = alumno.Apellido,
                Email = alumno.Email,
                Dni = alumno.Dni,
                NombreUsuario = alumno.Usuario.NombreUsuario
            };

            bool result=await _service.CreateAlumno(alumno);
            if (!result)
            {
                return BadRequest("No se ha podido crear el alumno nuevo");
            }
            return StatusCode(201,new { message=$"Se ha creado el alumno con exito",resultado });
           
        }
        [HttpGet("notas")]
        public async Task<IActionResult> AlumnosNotas()
        {
            var alumnos = await _service.AlumnoNotas();
            return Ok(alumnos);
        }
        [HttpDelete("{legajo}")]
        public async Task<IActionResult> DeleteAlumnmo(string legajo)
        {
            bool result = await _service.DeleteAlumno(legajo);

            if (!result)
            {
                return BadRequest($"No se ha podido borrar el alumno con el LEGAJO: {legajo}");
            }

            return Ok("Se ha borrado exitosamente el alumno");
        }
    }
}
