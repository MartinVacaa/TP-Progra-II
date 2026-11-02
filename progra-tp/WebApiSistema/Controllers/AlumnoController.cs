using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaRepository.Services.Interfaces;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnoController : ControllerBase
    {

        private readonly IServiceAlumno _service;

            public AlumnoController(IServiceAlumno service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllASync() 
        {
            var alumnos= await _service.GetAllASync();
            return Ok(alumnos);
        }
    }
}
