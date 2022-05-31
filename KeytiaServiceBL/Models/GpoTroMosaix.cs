using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroMosaix : GpoTroComun
    {
        public string PrefGpoTro { get; set; }
        public string RxTrn_CompCode { get; set; }
        public string RxTrn_PhoneNum { get; set; }
    }
}
