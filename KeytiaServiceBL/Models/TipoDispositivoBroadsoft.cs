using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class TipoDispositivoBroadsoft : HistoricoBase
    {
        public int OrdenAp { get; set; }
        public string RegEx { get; set; }
        public int TipoDispositivo { get; set; }
    }
}
