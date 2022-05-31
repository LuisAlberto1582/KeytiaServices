using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioNortel : SitioComun
    {
        public int LongCasilla { get; set; }
        public string RxTerld { get; set; }
        public string RxDigitTypeS { get; set; }
        public string RxDigitTypeE { get; set; }
        public string RxOrigld { get; set; }
    }
}
