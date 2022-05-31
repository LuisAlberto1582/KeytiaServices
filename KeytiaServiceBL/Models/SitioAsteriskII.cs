using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioAsteriskII : SitioComun
    {
        public int LongCasilla { get; set; }
        public decimal LongSRCEnt { get; set; }
        public string DescEntDstChannel { get; set; }
        public string DescEnlChannel { get; set; }
        public string DescEnlDstChannel { get; set; }
        public string ZonaHoraria { get; set; }
        public string DescEntChannel { get; set; }

    }
}
