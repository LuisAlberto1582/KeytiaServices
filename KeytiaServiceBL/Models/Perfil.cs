﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Perfil : HistoricoBase
    {
        public int BanderasPerfil { get; set; }
        public string Español { get; set; }
        public string Ingles { get; set; }
        public string Frances { get; set; }
        public string Portugues { get; set; }
        public string Aleman { get; set; }
    }
}
