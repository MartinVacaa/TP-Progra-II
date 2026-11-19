using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiSistema.DTO;

namespace SistemaRepository.Services.Interfaces
{
    public interface IServiceEncargado
    {
        Task<List<AlumnoPromedioDTO>> TraerPromedios();
        Task<decimal> Porcentajedesaprobados();
    }
}
