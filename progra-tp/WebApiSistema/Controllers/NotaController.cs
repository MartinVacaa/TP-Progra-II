using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaRepository.Domain;
using SistemaRepository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ENCARGADO")]
    public class NotaController : ControllerBase
    {
        private readonly INotaService _service;

        public NotaController(INotaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notas = await _service.GetAllAsync();
            return Ok(notas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nota = await _service.GetByIdAsync(id);

            if (nota == null)
            {
                return NotFound($"No se encontró la nota con ID {id}");
            }

            return Ok(nota);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Nota nota)
        {
            var creada = await _service.CreateAsync(nota);

            if (!creada)
            {
                return BadRequest("No se pudo crear la nota");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = nota.Id },
                nota
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Nota nota)
        {
            var actualizada = await _service.UpdateAsync(id, nota);

            if (!actualizada)
            {
                return NotFound($"No se encontró la nota con ID {id}");
            }

            return Ok("Nota actualizada correctamente");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminada = await _service.DeleteAsync(id);

            if (!eliminada)
            {
                return NotFound($"No se encontró la nota con ID {id}");
            }

            return Ok("Nota eliminada correctamente");
        }
    }
}
