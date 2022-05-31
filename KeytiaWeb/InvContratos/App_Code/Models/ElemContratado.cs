using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class ElemContratado
    {
        public int IdElemento { set; get; }
        public int CategoriaElementoId { set; get; }
        public string Nombre { set; get; }
        public string Descripcion { set; get; }
        public int ClaveCargo { set; get; }
        public int IActivo { set; get; }
        public bool Activo { set; get; }
        public string SActivo { set; get; }
        public string DtIniVigencia { set; get; }
        public string DtFinVigencia { set; get; }
        public string DtFecUltAct { set; get; }
    }
}