using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class AnexoElementoContratado
    {
        public int Cantidad { get; set; }
        public string Elemento { get; set; }
        public string Descripcion { get; set; }
        public Decimal CostoUnitarioMXN { get; set; }
        public Decimal CostoUnitarioMonedaOriginal { get; set; }
        public Decimal TipoDeCambio { get; set; }
        public Decimal MonedaOriginal { get; set; }
    }
}