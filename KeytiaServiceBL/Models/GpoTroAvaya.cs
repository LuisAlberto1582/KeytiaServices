using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroAvaya : GpoTroComun
    {
        public int Carrier { get; set; }
        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string RxCodeUsed { get; set; }
        public string RxInTrkCode { get; set; }
        public string RxOutCrtId { get; set; }
        public string RxInCrtId { get; set; }
        public string RxDialedNumber { get; set; }
        public string RxCallingNum { get; set; }
        public string RxCodeDial { get; set; }
    }
}
