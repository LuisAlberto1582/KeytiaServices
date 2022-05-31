using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models.Cargas
{
    public class DetalleCentroCostos
    {
        public int iCodRegistro { set; get; }
        public int iCodCatalogo { set; get; }
        public int iCodMaestro { set; get; }
        public int CenCos { set; get; }
        public int Emple { set; get; }
        public string TipoPr { set; get; }
        public string PeriodoPr { set; get; }
        public int Empre { set; get; }
        public int iNumCatalogo { set; get; }
        public string TipoCenCost { set; get; }
        public string PresupFijo { set; get; }
        public string FechaInicio { set; get; }
        public string FechaFin { set; get; }
        public string Descripcion { set; get; }
        public string Clave { set; get; }
        public string dtFecha { set; get; }
        public string iCodUsuario { set; get; }
        public string dtFecUltAct { set; get; }
    }
}
