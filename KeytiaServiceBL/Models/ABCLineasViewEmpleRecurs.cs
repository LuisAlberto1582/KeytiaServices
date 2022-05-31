using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ABCLineasViewEmpleRecurs : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Linea { get; set; }
        public int Carrier { get; set; }
        public string CveDescCarrier { get; set; }
        public int Sitio { get; set; }
        public string CveDescSitio { get; set; }
        public string Empleado { get; set; }
        public int CtaMaestra { get; set; }
        public string CveDescCtaMaestra { get; set; }
        public int TipoPlan { get; set; }
        public string CveDescTipoPlan { get; set; }
        public int EqCelular { get; set; }
        public string CveDescEqCelular { get; set; }
        public int PlanTarif { get; set; }
        public string CveDescPlanTarif { get; set; }
        public int EsTelular { get; set; }
        public int EsTarjetaVPNet { get; set; }
        public int EsNoPublicable { get; set; }
        public int EsConmutada { get; set; }
        public string Telefono { get; set; }
        public string IMEI { get; set; }
        public string ModeloCel { get; set; }
        public string TipoMovimiento { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
        public bool AtribOpcConError { get; set; }
    }
}
