using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class CargaEmpleado : HistoricoBase
    {
        public int EstCarga { get; set; }
        public int Empre { get; set; }
        public int BanderasCargaEmpleado { get; set; }
        public int Registros { get; set; }
        public int RegD { get; set; }
        public int RegP { get; set; }
        public int OpcCreaUsuar { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Clase { get; set; }
        public string Archivo01 { get; set; }
    }
}
