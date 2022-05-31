using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ABCExtensionesViewEmpleRecurs : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Extension { get; set; }
        public int Sitio { get; set; }
        public string CveDescSitio { get; set; }
        public int Cos { get; set; }
        public string CveDescCos { get; set; }
        public string Mascara { get; set; }
        public int EsVisible { get; set; }
        public string Empleado { get; set; }
        public int EsResponsable { get; set; }
        public string TipoMovimiento { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
    }
}
