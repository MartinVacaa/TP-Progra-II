using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MateriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materias = await _context.Materias
                .OrderBy(m => m.Anio).ThenBy(m => m.Nombre)
                .Select(m => new { id = m.Id, nombre = m.Nombre, anio = m.Anio })
                .ToListAsync();

            return Ok(materias);
        }
    }
}
