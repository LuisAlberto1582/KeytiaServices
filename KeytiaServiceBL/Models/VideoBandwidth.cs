using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class VideoBandwidth : HistoricoBase
    {
        public int TipoLlamColaboracion { get; set; }
        public int MarcaSitio { get; set; }
        public int AnchoDeBandaMinimo { get; set; }
        public int AnchoDeBandaMaximo { get; set; }
    }
}
