using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RedirectReasonCode : HistoricoBase
    {
        public int MarcaSitio { get; set; }
        public int Value { get; set; }
        public string Descripcion { get; set; }
    }
}
