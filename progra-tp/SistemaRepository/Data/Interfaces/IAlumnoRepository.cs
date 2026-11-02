using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Data.Interfaces
{
    public interface IAlumnoRepository
    {
        Task<List<Alumno>> GetAllAsync();
        Task<decimal> GetPromedioASync(int id);

        Task<Alumno?> GetAlumnoBYId(int id);
    }
}
