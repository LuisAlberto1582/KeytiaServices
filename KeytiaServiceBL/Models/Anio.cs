using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Anio
    {
        public int ICodRegistro { get; set; }
        public int ICodCatalogo { get; set; }
        public int ICodMaestro { get; set; }
        public int NumeroAnio { get; set; }
        public string VchCodigo { get; set; }
        public string VchDescripcion { get; set; }
        public DateTime DtIniVigencia { get; set; }
        public DateTime DtFinVigencia { get; set; }
        public int ICodUsuario { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
