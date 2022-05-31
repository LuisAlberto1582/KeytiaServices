using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroAsteriskI : GpoTroComun
    {
        public string RxSRC { get; set; }
        public string RxDST { get; set; }
        public string RxCode { get; set; }
        public string RxSRC2 { get; set; }
        public string MapeoCampos { get; set; }
        public string Pref { get; set; }
    }
}
