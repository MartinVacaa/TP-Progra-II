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
        private readonly AppDbContext _context;

        public AlumnoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alumno>> AlumnosNotas()
        {
            return await _context.Alumnos.Include(a=>a.Inscripciones).ThenInclude(I=>I.Carrera).ThenInclude(c=>c.Materias).ThenInclude(m=>m.Notas).ToListAsync();
        }

        public async Task<bool> CreateAlumno(Alumno alumno)
        {
            if (alumno.Legajo != null)
            {
                return false;
            }
          
        
            await _context.Alumnos.AddAsync(alumno);
          
          
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAlumno(string legajo)
        {
           var alumno = await _context.Alumnos.FindAsync(legajo);
            if (alumno == null)
            {
                return false;
            }
            await _context.Notas.Where(a=>a.Legajo==alumno.Legajo).ExecuteDeleteAsync();
            await _context.Inscripciones.Where(i => i.Legajo == alumno.Legajo).ExecuteDeleteAsync();
            _context.Alumnos.Remove(alumno);

            var usuario= await _context.Usuarios.FindAsync(alumno.UsuarioId);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<List<Alumno>> GetAllAsync()
        {
            return await _context.Alumnos.Include(a => a.Inscripciones).ThenInclude(i => i.Carrera).ToListAsync();
        }

        public async Task<Alumno?> GetAlumnoBYId(string legajo)
        {
            return await _context.Alumnos.Include(a=>a.Notas).FirstOrDefaultAsync(a => a.Legajo == legajo);
        }

        public async Task<decimal> GetPromedioASync(string legajo)
        {
            var alumno = await  GetAlumnoBYId(legajo);
            if(alumno== null)
            {
                return 0m;
            }
            int cantnotas = alumno.Notas.Count();
            var promedio = (alumno.Notas.Sum(n => n.Valor) / cantnotas);


            return promedio;
        }

    }
}
