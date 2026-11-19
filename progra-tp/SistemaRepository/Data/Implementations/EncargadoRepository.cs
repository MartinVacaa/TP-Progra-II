using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SistemaAcademico.Data;
using SistemaRepository.Data.Interfaces;
using SistemaRepository.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiSistema.DTO;

namespace SistemaRepository.Data.Implementations
{
    public class EncargadoRepository : IDashBoardEncargado
    {
        private readonly AppDbContext _context;

        public EncargadoRepository(AppDbContext context)
        {
                _context = context;
        }

        public async Task<int> AlumnosRegistrados()
        {
            return await _context.Alumnos.CountAsync();

        }

        public async Task<decimal> PorcentajeDesaprobados()
        {
            var cantnotas = await _context.Notas.CountAsync();
            var desaprobados = await _context.Notas.CountAsync(n => n.Valor < 6);
            var porcentaje = (desaprobados * 100m) / cantnotas;

            return porcentaje;
            
        }

        public async Task<List<AlumnoPromedioDTO>> TraerPromedios()
        {
            return await _context.Alumnos
             .Select(a => new AlumnoPromedioDTO
             {
                 Legajo = a.Legajo,
                 Nombre = a.Nombre,
                 Apellido = a.Apellido,
                 Promedio = a.Notas.Average(n => (decimal?)n.Valor) ?? 0
             })
             .OrderByDescending(a => a.Promedio)
             .ToListAsync();
        }


    }
}
