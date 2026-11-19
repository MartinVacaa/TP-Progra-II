using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
using SistemaRepository.DTO;
using SistemaRepository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Services.Implementations
{
    public class ServiceAlumno : IServiceAlumno
    {
        private readonly IAlumnoRepository _repository;

        public ServiceAlumno(IAlumnoRepository repository)
        {
            _repository = repository;
        }

        public Task<List<AlumnoNotaDTO>> AlumnoNotas()
        {
           return _repository.AlumnosNotas();
        }

        public async Task<bool> CreateAlumno(Alumno alumno)
        {
            if (alumno.Email == null)
            {
                throw new ArgumentException("El mail del alumno no puede ser nulo");
            }
            return await _repository.CreateAlumno(alumno);
        }

        public async Task<bool> DeleteAlumno(string legajo)
        {
            return await _repository.DeleteAlumno(legajo);
        }

        public async Task<List<Alumno>> GetAllASync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<decimal> GetPromedio(string legajo)
        {
            return await _repository.GetPromedioASync(legajo);
        }
    }
}
