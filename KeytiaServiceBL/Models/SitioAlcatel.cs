using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioAlcatel : SitioComun
    {
        public int LongCasilla { get; set; }
        public string PathXML { get; set; }
        public string ZonaHoraria { get; set; }
        public string DescComType { get; set; }
    }
}
