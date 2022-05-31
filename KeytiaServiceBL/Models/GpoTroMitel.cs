using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroMitel : GpoTroComun
    {
        public int Carrier {get; set;}
        public string PrefGpoTro { get; set; }
        public string RxDigitsDialed { get; set; }
        public string RxCallingParty { get; set; }
        public string RxCalledParty { get; set; }
        public string RxDnis { get; set; }
        public string RxANI { get; set; }
        public string RxCallCompStatus { get; set; }
        public string RxCallSeqId { get; set; }
        public string MapeoCampos { get; set; }
    }
}
