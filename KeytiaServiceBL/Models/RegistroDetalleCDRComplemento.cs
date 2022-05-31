using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.Models
{
    public class RegistroDetalleCDRComplemento
    {
        public int iCodRegistro { get; set; }
        public int iCodCatalogo { get; set; }
        public int RegCarga { get; set; }

        public int? iCodCatCodecOrigen { get; set; }
        public int? iCodCatCodecDestino { get; set; }
        public int? iCodCatAnchoBandaOrigen { get; set; }
        public int AnchoBandaOrigen { get; set; }
        public int? iCodCatAnchoBandaDestino { get; set; }
        public int AnchoBandaDestino { get; set; }
        public int? iCodCatTpLlamColaboracionOrigen { get; set; }
        public int? iCodCatTpLlamColaboracionDestino { get; set; }
        public int? iCodCatResolucionOrigen { get; set; }
        public int? iCodCatResolucionDestino { get; set; }
        public int? iCodCatDispColaboracionOrigen { get; set; }
        public int? iCodCatDispColaboracionDestino { get; set; }
        public int BanderasDetalleCDR { get; set; }
        public int CodecOrigen { get; set; }
        public int CodecDestino { get; set; }
        public int ResolucionOrigen { get; set; }
        public int ResolucionDestino { get; set; }

        public string OrigDeviceName { get; set; }
        public string DestDeviceName { get; set; }
        public string OrigCalledPartyNumber { get; set; }
        public string LastRedirectDn { get; set; }

        public string CallingPartyNumber { get; set; }
        public string CallingPartyNumberPartition { get; set; }
        public string DestLegIdentifier { get; set; }
        public string FinalCalledPartyNumber { get; set; }
        public string FinalCalledPartyNumberPartition { get; set; }

        public string AuthorizationCodeValue { get; set; }

        public DateTime dtFecUltAct { get; set; }

        public string SrcURI { get; set; }
        public string DstURI { get; set; }
        public string TrmReason { get; set; }
        public string TrmReasonCategory { get; set; }

        public string OrigCause_value { get; set; }
        public string DestCause_value { get; set; }
        public string LastRedirectRedirectReason { get; set; }

        public int? iCodCatOrigCause_value { get; set; }
        public int? iCodCatDestCause_value { get; set; }
        public int? iCodCatLastRedirectRedirectReason { get; set; }
    }
}
