using SistemaRepository.Domain;
using SistemaRepository.DTO;
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
        Task<decimal> GetPromedioASync(string legajo);

        Task<Alumno?> GetAlumnoBYId(string legajo);

        Task<bool> CreateAlumno(Alumno alumno);
        Task<bool> DeleteAlumno(string legajo);
        Task<bool> UpdateAlumno(string legajo, Alumno alumno);

        Task<List<AlumnoNotaDTO>> AlumnosNotas();
        Task<int> ContarMateriasCursando(string legajo);
        Task<AlumnoPerfilDto?> GetPerfilPorUsuario(string nombreUsuario);
    }
}
