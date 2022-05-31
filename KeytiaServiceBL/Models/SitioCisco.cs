using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class SitioCisco : SitioComun
    {
        private string zonaHoraria;

        public int CampoLlaveConf { get; set; }
        public int LongCasilla { get; set; }

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

        public string RxVoiceMail { get; set; }
    }
}
