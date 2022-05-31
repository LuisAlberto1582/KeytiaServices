using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RelacionesBase
    {
        public int ICodRegistro { get; set; }
        public int ICodRelacion { get; set; }
        public string VchDescripcion { get; set; }

        public DateTime DtIniVigencia { get; set; }
        public DateTime DtFinVigencia { get; set; }
        public int ICodUsuario { get; set; }
        public DateTime DtFecUltAct { get; set; }
    }
}
