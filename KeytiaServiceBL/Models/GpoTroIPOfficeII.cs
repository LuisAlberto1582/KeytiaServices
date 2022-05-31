using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroIPOfficeII :  GpoTroComun
    {
        public string NumGpoTro { get; set; }
        public string RxParty2Name { get; set; }
        public string RxDialledNumber { get; set; }
        public string RxCaller { get; set; }
        public string PrefGpoTro { get; set; }
    }
}
