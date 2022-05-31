using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroComun : HistoricoBase
    {
        public int Sitio { get; set; }
        public int TDest { get; set; }
        public int Locali { get; set; }
        public int BanderasGpoTro { get; set; }
        public int OrdenAp { get; set; } //Orden de aplicación
        public int LongPreGpoTro { get; set; } //Longitud del prefijo que se debe quitar al número marc.
        public int Criterio { get; set; }

        public int SitioRel { get; set; }
    }
}
