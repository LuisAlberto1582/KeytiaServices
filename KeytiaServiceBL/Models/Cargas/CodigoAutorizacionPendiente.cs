using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class CodigoAutorizacionPendiente : PendienteBase
    {
        public string VchDescripcion { get; set; }
        public int Emple { get; set; }
        public int Recurs { get; set; }
        public int Sitio { get; set; }
        public int Cargas { get; set; }
        public int Cos { get; set; }
        public int RegCarga { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Clave { get; set; }
        public string Filler { get; set; }
    }
}
