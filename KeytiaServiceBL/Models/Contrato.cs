using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Contrato : HistoricoBase
    {
        public int? Sitio { get; set; }
        public int? PlanServ { get; set; }
        public int DiaCorte { get; set; }

    }
}