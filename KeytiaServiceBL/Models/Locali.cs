using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Locali : HistoricoBase
    {
        public int Estados { get; set; }
        public int Paises { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }

        public int ICodCatEstados { get; set; }
        public int ICodCatPaises { get; set; }
    }
}
