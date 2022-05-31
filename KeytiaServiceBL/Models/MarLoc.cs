using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class MarLoc : HistoricoBase
    {
        public int? ICodCatLocali { get; set; }
        public int? ICodCatPaises { get; set; }
        public int? ICodCatTDest { get; set; }
        public string Clave { get; set; }
        public string Serie { get; set; }
        public string NumIni { get; set; }
        public string NumFin { get; set; }
        public string TipoRed { get; set; }
        public string ModalidadPago { get; set; }
        //public string RazonSocial { get; set; }

    }
}
