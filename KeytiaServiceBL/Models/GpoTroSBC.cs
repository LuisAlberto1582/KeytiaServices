using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroSBC : GpoTroComun
    {
        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string RxDigits { get; set; }
        public string RxCaller { get; set; }

    }
}
