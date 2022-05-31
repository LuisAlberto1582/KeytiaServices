using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioComun : HistoricoBase
    {
        //AtributosExtra en la vista
        public int ICodEntidad { get; set; }
        public string VchDesMaestro { get; set; }
        //

        public int Empre { get; set; }
        public int Locali { get; set; }
        public int TipoSitio { get; set; }
        public int MarcaSitio { get; set; }
        public int Emple { get; set; }
        public int Sitio { get; set; }

        public int BanderasSitio { get; set; }
        public int LongExt { get; set; }
        public Int64 ExtIni { get; set; }
        public Int64 ExtFin { get; set; }

        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal LongCodAuto { get; set; }

        public string Pref { get; set; }
        public string RangosExt { get; set; }
        public string FILLER { get; set; }

    }
}
