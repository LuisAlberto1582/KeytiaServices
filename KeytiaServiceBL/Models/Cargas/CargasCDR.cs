using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class CargasCDR : HistoricoBase
    {
        public int Sitio { get; set; }
        public int EstCarga { get; set; }
        public int BanderasCargasCDR { get; set; }
        public int Registros { get; set; }
        public int RegD { get; set; }
        public int RegP { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime IniTasacion { get; set; }
        public DateTime FinTasacion { get; set; }
        public DateTime DurTasacion { get; set; }

        public string Clase { get; set; }
        public string Archivo01 { get; set; }
        public string Archivo01F { get; set; }
    }
}
