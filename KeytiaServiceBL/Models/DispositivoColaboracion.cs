using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class DispositivoColaboracion : HistoricoBase
    {
        public int MarcaSitio { get; set; }
        public string Descripcion { get; set; }
    }
}
