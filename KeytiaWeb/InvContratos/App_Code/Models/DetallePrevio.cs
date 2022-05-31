using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class DetallePrevio
    {
        public string Id { get; set; }
        public string Folio { get; set; }
        public string Clave { get; set; }
        public string RelacionFolio { get; set; }
        public string Proveedor { get; set; }
        public string Vigente { get; set; }
        public string CategoriaConvenio { get; set; }
        public string CategoriaServicio { get; set; }
        public string SolicitanteNombre { get; set; }
        public string CompradorNombre { get; set; }
        public string FechaSolicitud { get; set; }
        public string FechaInicioVigencia { get; set; }
        public string FechaFinVigencia { get; set; }
        public string Descripcion { get; set; }
    }
}