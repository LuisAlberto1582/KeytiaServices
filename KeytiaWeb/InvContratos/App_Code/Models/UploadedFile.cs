using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class UploadedFile
    {
        public string RutaArchivo { get; set; }
        public DateTime FechaCarga { get; set; }
        public string Nombre { get; set; }
        public string EsVigente { get; set; }
        public int Usuar { get; set; }
    }
}