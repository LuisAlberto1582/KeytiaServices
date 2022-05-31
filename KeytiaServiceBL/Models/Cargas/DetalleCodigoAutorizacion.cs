using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class DetalleCodigoAutorizacion : DetalladoBase
    {
        public int Emple { get; set; }
        public int Recurs { get; set; }
        public int Sitio { get; set; }
        public int Cos { get; set; }
        public int INumCatalogo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Clave { get; set; }
        public string Filler { get; set; }
    }
}
