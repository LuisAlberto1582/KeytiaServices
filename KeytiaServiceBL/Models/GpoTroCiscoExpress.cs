using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroCiscoExpress : GpoTroComun
    {
        public int Carrier { get; set; }
        public int LongPreExt { get; set; }

        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string MapeoCampos { get; set; }
        public string RxNumMarcado { get; set; }
        public string RxExt { get; set; }
        public string PrefExt { get; set; }
    }
}
