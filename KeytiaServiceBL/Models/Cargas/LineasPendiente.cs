using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class LineasPendiente : PendienteBase
    {
        public string VchDescripcion { get; set; }
        public int Cargas { get; set; }
        public int Carrier { get; set; }
        public int Sitio { get; set; }
        public int Empre { get; set; }
        public int CenCos { get; set; }
        public int Recurs { get; set; }
        public int Emple { get; set; }
        public int CtaMaestra { get; set; }
        public int RegCarga { get; set; }
        public int BanderasLinea { get; set; }
        public double CargoFijo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Clave { get; set; }
        public string Tel { get; set; }
        public string IMEI { get; set; }
        public string ModeloCel { get; set; }
        public string Filler { get; set; }
    }
}
