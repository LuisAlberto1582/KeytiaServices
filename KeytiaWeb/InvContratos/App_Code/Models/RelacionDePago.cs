using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class RelacionDePago
    {
        public int Id { get; set; }
        public int PagoNumero { get; set; }
        public string FechaPago { get; set; }
        public string ImporteMXN { get; set; }
        public string ImporteMonedaOriginal { get; set; }
        public string TipoDeCambio { get; set; }
        public String MonedaOriginal { get; set; }
    }
}