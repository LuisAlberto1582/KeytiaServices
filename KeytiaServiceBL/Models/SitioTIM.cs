using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioTIM : HistoricoBase
    {
        public string Descripcion { get; set; }
        public int Carrier { get; set; }
        public int SitioTIMNombrePublico { get; set; }
    }
}
