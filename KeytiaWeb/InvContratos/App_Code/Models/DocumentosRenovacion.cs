using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class DocumentosRenovacion
    {
        public DateTime FechaCarga { get; set; }
        public String FaseDeRenovacion { get; set; }
        public String NombreArchivo { get; set; }
        public String Comentarios { get; set; }
        public String UsuarioCarga { get; set; }
        public String RutaArchivo { get; set; }
    }
}