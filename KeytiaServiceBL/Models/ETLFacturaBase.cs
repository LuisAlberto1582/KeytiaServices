using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ETLFacturaBase : ETLServicioBase
    {
        public int Empre { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Moneda { get; set; }


        //Datos calculados:
        public int Carrier { get; set; }
        public DateTime FechaFactura { get; set; }
        public DateTime FechaPub { get; set; }
        public double TipoCambioVal { get; set; }

        public string Archivo01 { get; set; }
    }
}
