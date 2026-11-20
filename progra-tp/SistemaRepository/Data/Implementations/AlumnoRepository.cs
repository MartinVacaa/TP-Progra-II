using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
using SistemaRepository.DTO;
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

        public async Task<List<AlumnoNotaDTO>> AlumnosNotas()
        {
            return await _context.Notas
       .Select(n => new AlumnoNotaDTO
       {
           Legajo = n.Alumno.Legajo,
           Nombre = n.Alumno.Nombre,
           Apellido = n.Alumno.Apellido,
           Carrera = n.Materia.Carrera.Nombre,
           Materia = n.Materia.Nombre,
           Nota = n.Valor,
           Tipo= n.Tipo
       })
       .OrderBy(x => x.Apellido)
       .ThenBy(x => x.Nombre)
       .ThenBy(x => x.Carrera)
       .ThenBy(x => x.Materia)
       .ToListAsync();
        }

        public async Task<bool> CreateAlumno(Alumno alumno)
        {
            
        
            // El legajo es obligatorio y no puede repetirse (es la PK)
            if (string.IsNullOrWhiteSpace(alumno.Legajo))
            {
                return false;
            }
            bool existe = await _context.Alumnos.AnyAsync(a => a.Legajo == alumno.Legajo);
            if (existe)
            {
                return false;
            }
            foreach (var inscripcion in alumno.Inscripciones)
            {
                bool carreraExiste = await _context.Carreras.AnyAsync(c => c.Id == inscripcion.CarreraId);
                if (!carreraExiste)
                {
                    throw new ArgumentException($"La carrera con Id {inscripcion.CarreraId} no existe");
                }
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
            var alumno = await GetAlumnoBYId(legajo);
            if (alumno == null)
            {
                return 0m;
            }
            int cantnotas = alumno.Notas.Count();
            if (cantnotas == 0)
            {
                return 0m; 
            }
            var promedio = (alumno.Notas.Sum(n => n.Valor) / cantnotas);


            return promedio;
        }

        public async Task<bool> UpdateAlumno(string legajo, Alumno datosNuevos)
        {
            var alumnoExistente = await _context.Alumnos.FindAsync(legajo);

            if (alumnoExistente == null)
            {
                return false;
            }

            alumnoExistente.Nombre = datosNuevos.Nombre;
            alumnoExistente.Apellido = datosNuevos.Apellido;
            alumnoExistente.Dni = datosNuevos.Dni;
            alumnoExistente.Email = datosNuevos.Email;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<int> ContarMateriasCursando(string legajo)
        {
            return await _context.Materias
                .CountAsync(m => _context.Inscripciones
                    .Any(i => i.Legajo == legajo && i.CarreraId == m.CarreraId));
        }

        // Busca al alumno a partir del nombre de usuario que viene en el token (claim "sub")
        public async Task<AlumnoPerfilDto?> GetPerfilPorUsuario(string nombreUsuario)
        {
            var alumno = await _context.Alumnos
                .Include(a => a.Inscripciones).ThenInclude(i => i.Carrera)
                .FirstOrDefaultAsync(a => a.Usuario!.NombreUsuario == nombreUsuario);

            if (alumno == null)
            {
                return null;
            }

            return new AlumnoPerfilDto
            {
                Legajo = alumno.Legajo,
                Nombre = alumno.Nombre,
                Apellido = alumno.Apellido,
                Email = alumno.Email,
                Carrera = alumno.Inscripciones.Select(i => i.Carrera!.Nombre).FirstOrDefault(),
                MateriasCursando = await ContarMateriasCursando(alumno.Legajo)
            };
        }
    }
}
    

