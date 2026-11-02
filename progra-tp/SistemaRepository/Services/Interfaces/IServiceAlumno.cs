using SistemaRepository.Domain;
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
    }
}
