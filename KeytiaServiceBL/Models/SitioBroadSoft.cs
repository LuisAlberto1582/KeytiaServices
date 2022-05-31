using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioBroadSoft : SitioComun
    {
        private string zonaHoraria;
        public int LongCasilla { get; set; }
        public string RxDigits { get; set; }
        public string RxTipo { get; set; }

        public string ZonaHoraria
        {
            get
            {
                if (string.IsNullOrEmpty(this.zonaHoraria))
                {
                    this.zonaHoraria = "Central Standard Time (Mexico)";
                }
                return this.zonaHoraria;
            }

            set
            {
                this.zonaHoraria = value;
            }
        }
    }
}
