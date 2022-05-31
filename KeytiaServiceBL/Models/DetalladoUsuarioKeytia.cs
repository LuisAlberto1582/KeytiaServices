using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class DetalladoUsuarioKeytia : DetalladoBase
    {
        public int UsuarDB { get; set; }
        public int INumRegistro { get; set; }
        public int INumCatalogo { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string VchCodUsuario { get; set; }
    }
}
