using SistemaRepository.Domain;
using SistemaRepository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Services.Interfaces
{
    public interface IServiceAlumno
    {

        Task<List<Alumno>> GetAllASync();

        Task<decimal> GetPromedio(string legajo);

        Task<bool> CreateAlumno(Alumno alumno);

        Task<List<AlumnoNotaDTO>> AlumnoNotas();

        Task<bool> DeleteAlumno(string legajo);
    }
}
