using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class ProveedoresContacto
    {
        public string IdProveedor { get; set; }
        public string RazonSocial { get; set; }
        public string Nombre { get; set; }
        public int IdPais { get; set; }
        public string Pais { get; set; }
        public int NoProveedorSAP { get; set; }
        public int IdContacto { get; set; }
        public string NombreContacto { get; set; }
        public string CorreoElectronico { get; set; }
        public string TelefonoExtension { get; set; }
    }
}