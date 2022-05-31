using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class ETLFacturaClaroBrasil : ETLFacturaBase
    {
        public int BanderasFacClaroBrasilCelv3 { get; set; }
        public bool CargarInfoLineasNoIdentificadas { get; set; }
        public bool PublicarLineasSinDetalle { get; set; }
        public bool ConsiderarAjustesPositivosF1 { get; set; }
        public bool ConsiderarAjustesNegativosF1 { get; set; }
        public bool SubirInformacion { get; set; }
        public bool ActualizarLineasyAtributos { get; set; }
        public bool GenerarDetalle { get; set; }
        public bool GenerarResumenes { get; set; }

    }
}
