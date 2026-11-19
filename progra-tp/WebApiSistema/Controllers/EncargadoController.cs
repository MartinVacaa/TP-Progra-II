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

        private readonly IServiceEncargado _serivice;

        public EncargadoController(IServiceEncargado service)
        {
                _serivice = service;
        }

        [HttpGet]
        //[Authorize(Roles ="ENCARGADO")]
        public async Task<IActionResult> GetPromedios()
        {
            var promedios = await _serivice.TraerPromedios();
            return Ok(promedios);
        }

        [HttpGet("Desaprobados")]
       // [Authorize(Roles = "ENCARGADO")]
        public async Task<IActionResult> PorcentajeDesaprobados()
        {
            var porcentaje= await _serivice.Porcentajedesaprobados();
            if (porcentaje == 0)
            {
                return NotFound("No hay notas cargadas");
            }
            return Ok(new
            {
                message = $"El porcentaje de desaprobados es de %",porcentaje
            });
        }
    }
}
