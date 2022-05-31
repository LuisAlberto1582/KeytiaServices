using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ABCCenCosView : CargaMasiva
    {
        public int iCodCatCC { set; get; }
        public string claveCC { get; set; }
        public string descripcionCC { get; set; }
        public int iCodCatCCPadre { get; set; }
        public int empleResponsable { get; set; }
        public int banderas { get; set; }
        public string fechaMovimiento { get; set; }
        public string tipomovimiento { get; set; }

    }
}
