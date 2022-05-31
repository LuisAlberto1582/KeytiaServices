using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ABCCodigosViewEmpleRecurs : CargaMasiva
    {
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; }
        public int Sitio { get; set; }
        public string CveDescSitio { get; set; }
        public int Cos { get; set; }
        public string CveDescCos { get; set; }
        public int EsVisible { get; set; }
        public int EsCodigoPersonal { get; set; }
        public string Empleado { get; set; }
        public string TipoMovimiento { get; set; }

        //Datos de Apoyo
        public int ICodCatalogo { get; set; }
    }
}
