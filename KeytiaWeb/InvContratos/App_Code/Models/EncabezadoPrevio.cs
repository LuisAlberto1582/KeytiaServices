using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeytiaWeb.WSSS;

namespace KeytiaWeb.InvContratos.App_Code.Models
{
    public class EncabezadoPrevio : InvContBase
    {
        public string Folio { get; set; }
        public string FolioContrato { get; set; }
        public string FechaFinVigencia { get; set; }
        public string Encabezado { get; set; }
    }
}