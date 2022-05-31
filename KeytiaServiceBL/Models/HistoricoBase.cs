using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class HistoricoBase : EstructuraBase
    {
        public int ICodCatalogo { get; set; }
        public int ICodMaestro { get; set; }
        public string VchCodigo { get; set; }
        public int EntidadCat { get; set; }
    }
}
