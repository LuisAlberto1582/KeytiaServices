using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class ElementoContratado
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public string Elemento { get; set; }
        public string Descripcion { get; set; }
        public string CostoUnitarioMXN { get; set; }
        public string CostoUnitarioMonedaOriginal { get; set; }
        public string TipoDeCambio { get; set; }
        public string MonedaOriginal { get; set; }
    }
}