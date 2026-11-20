using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaRepository.Services.Interfaces;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncargadoController : ControllerBase
    {

        private readonly IServiceEncargado _service;

        public EncargadoController(IServiceEncargado service)
        {
                _service = service;
        }

        [HttpGet]
        [Authorize(Roles ="ENCARGADO")]
        public async Task<IActionResult> GetPromedios()
        {
            var promedios = await _service.TraerPromedios();
            return Ok(promedios);
        }

        [HttpGet("Desaprobados")]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> PorcentajeDesaprobados()
        {
            var porcentaje= await _service.Porcentajedesaprobados();
            if (porcentaje == 0)
            {
                return NotFound("No hay notas cargadas");
            }
            return Ok(new
            {
                message = $"El porcentaje de desaprobados es de %",porcentaje
            });
        }

        [HttpGet("Registrados")]
        [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> AlumnosRegistrados()
        {
            var cantidad = await _service.AlumnosRegistrados();
            return Ok(cantidad);
        }

    }
}
