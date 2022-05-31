using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class GpoTroCisco : GpoTroComun
    {
        public string RxDesDevN {get;set;}
        public string RxOrgDevN {get;set;}
        public string RxFiCaPaNuP {get;set;}
        public string RxFiCaPaNu {get;set;}
        public string RxCaPaNuP {get;set;}
        public string RxCaPaNu {get;set;}
        public string Pref { get; set; }
    }
}
