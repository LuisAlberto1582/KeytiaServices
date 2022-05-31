using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Extension : HistoricoBase
    {
        public int Emple { get; set; }
        public int Recurs { get; set; }
        public int Sitio { get; set; }
        public int TipoLicenciaExtension { get; set; }
        public int Cos { get; set; }
        public int EnviarCartaCust { get; set; }
        public int BanderasExtens { get; set; }

        public string Masc { get; set; }
    }
}
