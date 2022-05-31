using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class PlanTarifario : HistoricoBase
    {
        public int Carrier { get; set; }
        public int MinutosMismoCarrier { get; set; }
        public int MinutosOtrosCarrier { get; set; }
        public int SMSIncluidos { get; set; }

        public double DatosMBIncluidos { get; set; }
        public double RentaTelefonia { get; set; }

        public string Descripcion { get; set; }
    }
}
