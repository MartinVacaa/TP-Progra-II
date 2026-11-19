using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiSistema.DTO;


namespace SistemaRepository.Data.Interfaces
{
    public interface IDashBoardEncargado
    {
        Task<List<AlumnoPromedioDTO>> TraerPromedios();
        Task<decimal> PorcentajeDesaprobados();
    }
}
