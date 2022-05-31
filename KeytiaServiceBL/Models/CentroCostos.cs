using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class CentroCostos : HistoricoBase
    {
        public int CenCos { get; set; }
        public int Emple { get; set; }
        public int TipoPr { get; set; }
        public int PeriodoPr { get; set; }
        public int Empre { get; set; }
        public int TipoCenCost { get; set; }
        public int NivelJerarq { get; set; }
        public int BanderasCencos { get; set; }

        public double PresupFijo { get; set; }

        public string Descripcion { get; set; }
        public string CuentaContable { get; set; }
    }
}
