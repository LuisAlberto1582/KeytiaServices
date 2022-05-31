/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Reporte de Numero de Extensiones
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Reporte_Numero_Extensiones : ReporteEspecial
    {

        public Reporte_Numero_Extensiones()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.TDest = true;
            pAplicaReporte.Client = true;

            pAplicaReporte.IncluirJerarquias = true;
            pAplicaReporte.AdicionarUsuarioWEB = true;
            pAplicaReporte.EnviarUltimoAcceso = true;

            pvchCodBanderas = "BanderasNumeroExtensiones";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "RNE");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "RNE", defaultValue);
        }
    }
}
