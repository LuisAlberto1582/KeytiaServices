using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class Usuario : HistoricoBase
    {
        public int Perfil { get; set; }
        public int Empre { get; set; }
        public int Idioma { get; set; }
        public int Moneda { get; set; }
        public int UsuarDB { get; set; }
        public int CenCos { get; set; }

        public DateTime UltAcc { get; set; }

        public string Password { get; set; }
        public string HomePage { get; set; }
        public string Email { get; set; }
        public string ConfPassword { get; set; }
        public string CategoUsuar { get; set; }
    }
}
