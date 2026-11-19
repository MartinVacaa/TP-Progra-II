using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRepository.DTO
{
    public class AlumnoNotaDTO
    {
        public string Legajo {  get; set; }
        public string Nombre {  get; set; }
        public string Apellido {  get; set; }

        public string Carrera { get; set; }

        public string Materia {  get; set; }
        public decimal Nota {  get; set; }
        public string Tipo { get; set; }
    }
}
