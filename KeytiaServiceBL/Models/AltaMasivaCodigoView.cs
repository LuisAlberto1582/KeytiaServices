using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class AltaMasivaCodigoView : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; }
        public string Sitio { get; set; }
        public string Cos { get; set; }
        public int EsVisible { get; set; }
        public int EsCodigoPersonal { get; set; }
        public string Empleado { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
        public int ICodSitio { get; set; }
        public int ICodMarcaSitio { get; set; }

    }
}
