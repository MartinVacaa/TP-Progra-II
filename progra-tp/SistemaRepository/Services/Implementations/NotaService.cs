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
    public class NotaService : INotaService
    {
        private readonly INotaRepository _repository;

        public NotaService(INotaRepository repository)
        {
            _repository = repository;
        }
        public Task<bool> CreateAsync(Nota nota)
        {
            return _repository.CreateAsync(nota);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<List<Nota>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Nota?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<bool> UpdateAsync(int id, Nota nota)
        {
            return _repository.UpdateAsync(id, nota);
        }
    }
}
