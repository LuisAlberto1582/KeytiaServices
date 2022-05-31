using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioAsteriskI : SitioComun
    {
        public int LongCasilla { get; set; }
        public string ZonaHoraria { get; set; }
        public string PrefAutCode { get; set; }
    }
}
