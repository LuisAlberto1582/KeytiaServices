﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ETLServicioBase : HistoricoBase
    {
        public int EstCarga { get; set; }
        public string Clase { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }


        public int Registros { get; set; }
        public int RegD { get; set; }
        public int RegP { get; set; }
    }
}
