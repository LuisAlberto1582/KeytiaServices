using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class DetalleLineas : DetalladoBase
    {
        public int Carrier { get; set; }
        public int Sitio { get; set; }
        public int CenCos { get; set; }
        public int Recurs { get; set; }
        public int Emple { get; set; }
        public int CtaMaestra { get; set; }
        public int TipoPlan { get; set; }
        public int EqCelular { get; set; }
        public int PlanTarif { get; set; }
        public int BanderasLinea { get; set; }
        public int INumCatalogo { get; set; }
        public double CargoFijo { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string Clave { get; set; }
        public string Tel { get; set; }
        public string Etiqueta { get; set; }
        public string IMEI { get; set; }
        public string ModeloCel { get; set; }
        public string Filler { get; set; }
    }
}
