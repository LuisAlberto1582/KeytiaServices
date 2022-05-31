using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class AltaMasivaExtensionView : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Extension { get; set; }
        public string Sitio { get; set; }
        public string Cos { get; set; }
        public string Mascara { get; set; }
        public int EsVisible { get; set; }
        public string Empleado { get; set; }
        public int EsResponsable { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
        public int ICodSitio { get; set; }
        public int ICodMarcaSitio { get; set; }
    }
}
