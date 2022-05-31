using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Linea : HistoricoBase
    {
        public int Carrier { get; set; }
        public int Sitio { get; set; }
        public int CenCos { get; set; }
        public int Recurs { get; set; }
        public int Emple { get; set; }
        public int CtaMaestra { get; set; }
        public int RazonSocial { get; set; }
        public int TipoPlan { get; set; }
        public int EqCelular { get; set; }
        public int PlanTarif { get; set; }
        public int BanderasLinea { get; set; }
        public int EnviarCartaCust { get; set; }

        public double CargoFijo { get; set; }

        public DateTime FecLimite { get; set; }
        public DateTime FechaFinPlan { get; set; }
        public DateTime FechaDeActivacion { get; set; }

        public string Etiqueta { get; set; }
        public string Tel { get; set; }
        public string PlanLineaFactura { get; set; }
        public string IMEI { get; set; }
        public string ModeloCel { get; set; }
        public string NumOrden { get; set; }

    }
}
