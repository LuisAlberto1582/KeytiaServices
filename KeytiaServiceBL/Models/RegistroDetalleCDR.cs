using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RegistroDetalleCDR
    {
        public int iCodRegistro { get; set; }
        public int iCodCatalogo { get; set; }
        public int iCodMaestro { get; set; }

        public int? Sitio { get; set; }
        public int? CodAuto { get; set; }
        public int? Carrier { get; set; }
        public int? Exten { get; set; }
        public int? TDest { get; set; }
        public int? Locali { get; set; }
        public int? Contrato { get; set; }
        public int? Tarifa { get; set; }
        public int? Emple { get; set; }
        public int? GpoTro { get; set; }

        public int? RegCarga { get; set; }
        public int? DuracionMin { get; set; }
        public int? DuracionSeg { get; set; }
        public int? GEtiqueta { get; set; }
        public int? AnchoDeBanda { get; set; }

        public double? Costo { get; set; }
        public double? CostoFac { get; set; }
        public double? CostoSM { get; set; }
        public double? CostoMonLoc { get; set; }
        public double? TipoCambioVal { get; set; }

        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string FechaOrigen { get; set; }

        public string VchDescripcion { get; set; }
        public string TelDest { get; set; }
        public string CircuitoSal { get; set; }
        public string GpoTroSal { get; set; }
        public string CircuitoEnt { get; set; }
        public string GpoTroEnt { get; set; }
        public string IP { get; set; }
        public string TpLlam { get; set; }
        public string Extension { get; set; }
        public string CodAut { get; set; }
        public string Etiqueta { get; set; }

        public DateTime? dtFecha { get; set; }
        public int? iCodUsuario { get; set; }
        public DateTime dtFecUltAct { get; set; }
    }
}
