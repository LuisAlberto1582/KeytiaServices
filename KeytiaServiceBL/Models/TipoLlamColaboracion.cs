using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class TipoLlamColaboracion : HistoricoBase
    {
        public int MarcaSitio { get; set; }
        public string Descripcion { get; set; }
        public string VelocidadMinimaCarga { get; set; }
        public string VelocidadRecomendadaCarga { get; set; }
    }
}
