using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class TDest : HistoricoBase
    {
        public int Paises { get; set; }
        public int CatTDest { get; set; }
        public int CategoriaServicio { get; set; }
        public int BanderasTDest { get; set; }
        public int OrdenAp { get; set; }
        public int LongCveTDest { get; set; }
        public string Español { get; set; }
        public string Ingles { get; set; }
        public string Frances { get; set; }
        public string Portugues { get; set; }
        public string Aleman { get; set; }
    }
}
