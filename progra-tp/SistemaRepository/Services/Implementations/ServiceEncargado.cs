using SistemaRepository.Data.Interfaces;
using SistemaRepository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiSistema.DTO;

namespace SistemaRepository.Services.Implementations
{
    public class ServiceEncargado : IServiceEncargado
    {
        private readonly IDashBoardEncargado _repository;

        public ServiceEncargado(IDashBoardEncargado repository)
        {
                _repository = repository;
        }

        public async Task<int> AlumnosRegistrados()
        {
           return await _repository.AlumnosRegistrados();
        }

        public async Task<decimal> Porcentajedesaprobados()
        {
            return await _repository.PorcentajeDesaprobados();
        }

        public async Task<List<AlumnoPromedioDTO>> TraerPromedios()
        {
            return await _repository.TraerPromedios();
        }
    }
}
