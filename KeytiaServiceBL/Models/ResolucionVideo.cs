using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ResolucionVideo : HistoricoBase
    {
        public int MarcaSitio { get; set; }
        public int ClaveResolucionVideo { get; set; }
        public string Descripcion { get; set; }
    }
}
