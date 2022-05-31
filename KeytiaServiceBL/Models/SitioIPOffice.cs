using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioIPOffice : SitioComun
    {
        public int LongCasilla { get; set; }
        public string RxDigits { get; set; }
        public string RxTipo { get; set; }
    }
}
