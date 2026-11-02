using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Data.Implementations
{
    public class AlumnoRepository : IAlumnoRepository
    {
        private readonly SistemaDbContext _context;

        public AlumnoRepository(SistemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alumno>> GetAllAsync()
        {
            return await _context.Alumnos.Include(a => a.Inscripciones).ThenInclude(i => i.Carrera).ToListAsync();
        }
    }
}
