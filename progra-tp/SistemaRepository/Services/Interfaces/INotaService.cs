using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Services.Interfaces
{
    public interface INotaService
    {
        Task<List<Nota>> GetAllAsync();
        Task<Nota?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Nota nota);
        Task<bool> UpdateAsync(int id, Nota nota);
        Task<bool> DeleteAsync(int id);
    }
}
