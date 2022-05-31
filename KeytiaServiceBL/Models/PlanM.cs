using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class PlanM : HistoricoBase
    {
        public int? ICodCatTDest { get; set; }
        public int? ICodCatPaises { get; set; }
        public int? OrdenAp { get; set; }
        public int? LongPrePlanM { get; set; }
        public int? BanderasPlanMarcacion { get; set; }
        public string RegEx { get; set; }
        public string ExpresionRegular { get; set; }
    }
}
