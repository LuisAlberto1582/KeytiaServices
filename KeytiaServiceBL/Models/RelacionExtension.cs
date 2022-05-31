using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RelacionExtension : RelacionesBase
    {
        public int Emple { get; set; }
        public int Exten { get; set; }
        public int FlagEmple { get; set; }
        public int FlagExten { get; set; }
    }
}
