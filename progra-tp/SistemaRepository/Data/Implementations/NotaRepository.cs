using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.Data.Implementations
{
    public class NotaRepository : INotaRepository
    {
        private readonly AppDbContext _context;

        public NotaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Nota>> GetAllAsync()
        {
            return await _context.Notas.ToListAsync();
        }

        public async Task<Nota?> GetByIdAsync(int id)
        {
            return await _context.Notas.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Nota nota)
        {
            await _context.Notas.AddAsync(nota);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, Nota nota)
        {
            var notaExistente = await _context.Notas.FindAsync(id);

            if (notaExistente == null)
            {
                return false;
            }

            notaExistente.Legajo = nota.Legajo;
            notaExistente.MateriaId = nota.MateriaId;
            notaExistente.Tipo = nota.Tipo;
            notaExistente.Valor = nota.Valor;
            notaExistente.Fecha = nota.Fecha;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var nota = await _context.Notas.FindAsync(id);

            if (nota == null)
            {
                return false;
            }

            _context.Notas.Remove(nota);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
