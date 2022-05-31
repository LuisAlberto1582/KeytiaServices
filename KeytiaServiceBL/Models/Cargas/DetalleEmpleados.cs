using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class DetalleEmpleados : DetalladoBase
    {
        public int CenCos { get; set; }
        public int TipoEm { get; set; }
        public int Puesto { get; set; }
        public int Usuar { get; set; }
        public int Emple { get; set; }
        public int INumCatalogo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string RFC { get; set; }
        public string Email { get; set; }
        public string Ubica { get; set; }
        public string NominaA { get; set; }
        public string NomCompleto { get; set; }
        public string Filler { get; set; }
    }
}
