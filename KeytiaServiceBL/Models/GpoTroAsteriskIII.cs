using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroAsteriskIII : GpoTroComun
    {
        public string RxTrmReasonCat { get; set; }
        public string RxSrcPhoneNum { get; set; }
        public string RxDstPhoneNum { get; set; }
        public string RxTrunk { get; set; }
        public string PrefGpoTro { get; set; }
    }
}
