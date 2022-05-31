using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class CenCosPendiente
    {
        public int iCodRegistro { set; get; }
        public int iCodCatalogo { set; get; }
        public int iCodMaestro { set; get; }
        public string vchDescripcion { set; get; }
        public int CenCos { set; get; }
        public int Cargas { set; get; }
        public int Emple { set; get; }
        public string TipoPr { set; get; }
        public string PeriodoPr { set; get; }
        public int Empre { set; get; }
        public int RegCarga { set; get; }
        public string TipoCenCost { set; get; }
        public string PresupFijo { set; get; }
        public string FechaInicio { set; get; }
        public string FechaFin { set; get; }
        public string Descripcion { set; get; }
        public string Clave { set; get; }
        public string dtFecha { set; get; }
        public int iCodUsuario { set; get; }
        public string dtFecUltAct { set; get; }
    }
}
