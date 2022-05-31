using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class NivelTarifa : HistoricoBase    {

        public int NivelTar { get; set; }
        public int RangoInicial { get; set; }
        public int RangoFinal { get; set; }
        public int BanderasNivelTarifa { get; set; }

        public double Costo { get; set; }
        public double? CostoFac { get; set; }
        public double? CostoSM { get; set; }
        public double? CostoMonLoc { get; set; }
        public double? TipoCambioVal { get; set; }

    }
}
