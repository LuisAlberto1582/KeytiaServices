using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RelacionLinea : RelacionesBase
    {
        public int Emple { get; set; }
        public int Linea { get; set; }
        public int FlagLinea { get; set; }
    }
}
