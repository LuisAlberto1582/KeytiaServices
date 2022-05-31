using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class PendienteBase
    {
        public int ICodRegistro { get; set; }
        public int ICodCatalogo { get; set; }
        public int ICodMaestro { get; set; }
        public string VchCodigo { get; set; }

        public DateTime DtFecha { get; set; }
        public int ICodUsuario { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
