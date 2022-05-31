/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Reporte de Nodos
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Reporte_Nodos : ReporteEspecial
    {
        public Reporte_Nodos()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio = true;
            pAplicaReporte.Empre = true;
            pAplicaReporte.Emple = true;
            pAplicaReporte.Exten = true;

            pAplicaReporte.IncluirJerarquias = true;
            pAplicaReporte.AdicionarUsuarioWEB = true;
            pAplicaReporte.EnviarUltimoAcceso = true;
            pAplicaReporte.SinCtaEnviarAlReponsableCC = true;

            pvchCodBanderas = "BanderasReporteNodos";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "RN");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "RN", defaultValue);
        }
    }
}
