using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ExtensionCDRSitio
    {
        public int ICodRegistro { get; set; }
        public Int64 Extension { get; set; }
        public int Sitio { get; set; }
        public int MarcaSitio { get; set; }
        public int Empre { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
