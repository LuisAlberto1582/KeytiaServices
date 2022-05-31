using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroAlcatelSerial : GpoTroComun
    {
        public int Carrier { get; set; }
        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string RxDialedNumber { get; set; }
    }
}
