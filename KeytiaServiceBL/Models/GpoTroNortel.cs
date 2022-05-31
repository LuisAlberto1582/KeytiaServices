using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroNortel : GpoTroComun
    {
        public string RxExtension { get; set; }
        public string PrefGpoTro { get; set; }
        public string NumGpoTro { get; set; }
        public string RxDigits { get; set; }
        public string RxCodigoAut { get; set; }
    }
}
