using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class NIRPoblacionPrincipal : HistoricoBase
    {
        public int Paises { get; set; }
        public string Nir { get; set; }
        public string Descripcion { get; set; }
    }
}
