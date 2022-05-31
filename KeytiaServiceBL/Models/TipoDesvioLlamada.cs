using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class TipoDesvioLlamada : HistoricoBase
    {
        public string Descripcion { get; set; }
        public int MarcaSitio { get; set; }
        public int ClaveInt { get; set; }
    }
}
