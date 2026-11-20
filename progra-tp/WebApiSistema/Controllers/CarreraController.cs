using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ENCARGADO")]
    public class CarreraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarreraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var carreras = await _context.Carreras
                .OrderBy(c => c.Nombre)
                .Select(c => new { id = c.Id, nombre = c.Nombre, duracionAnios = c.DuracionAnios })
                .ToListAsync();

            return Ok(carreras);
        }
    }
}