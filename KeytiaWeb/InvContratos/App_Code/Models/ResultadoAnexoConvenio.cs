using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class ResultadoAnexoConvenio
    {
        public int ContratoId { get; set; }
        public int Id { get; set; }
        public String Folio { get; set; }
        public String Contrato { get; set; }
        public String Nombre { get; set; }
        public String CategoriaConvenio { get; set; }
        public String Descripcion { get; set; }
        public string FechaInicioVigencia { get; set; }
        public string FechaFinVigencia { get; set; }
        public String Activo { get; set; }
    }
}