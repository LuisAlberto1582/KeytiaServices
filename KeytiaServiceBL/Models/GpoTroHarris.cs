using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroHarris : GpoTroComun
    {
        public int Carrier { get; set; }

        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string RxDialedNumber { get; set; }
        public string RxAuthCode { get; set; }
        public string RxSelFac { get; set; }
        public string RxCRSta { get; set; }
        public string RxSelSta { get; set; }
        public string MapeoCampos { get; set; }
    }
}
