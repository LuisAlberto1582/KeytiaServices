using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class BitacoraEjecucionCargas
    {
        public int ICodRegistro { get; set; }
        public int ICodCatEsquema { get; set; }
        public int ICodRegistroCarga { get; set; }
        public int ICodCatCarga { get; set; }
        public string MaestroDesc { get; set; }
        public string EstCargaCod { get; set; }
        public DateTime DtFecInsRegistro { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
