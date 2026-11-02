using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
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
        public async Task<List<Alumno>> GetAllASync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
