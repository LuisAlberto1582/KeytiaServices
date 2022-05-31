using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class CodecVideo : HistoricoBase
    {
        public int MarcaSitio { get; set; }
        public int ClaveCodecVideo { get; set; }
        public string Descripcion { get; set; }
    }
}
