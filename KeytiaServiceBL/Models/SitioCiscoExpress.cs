using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioCiscoExpress : SitioComun
    {
        public int LongCasilla { get; set; }
        public string FormatoFecha { get; set; }
        public string FormatoHora { get; set; }
    }
}
