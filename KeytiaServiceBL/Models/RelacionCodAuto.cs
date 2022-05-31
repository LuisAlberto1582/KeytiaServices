using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RelacionCodAuto : RelacionesBase
    {
        public int Emple { get; set; }
        public int CodAuto { get; set; }
        public int FlagCodAuto { get; set; }
    }
}
