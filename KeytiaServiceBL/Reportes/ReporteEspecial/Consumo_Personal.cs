/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Consumo Personal
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Consumo_Personal : ReporteEspecial
    {
        public Consumo_Personal()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio = true;
            pAplicaReporte.CenCos = true;
            pAplicaReporte.Emple = true;

            pAplicaReporte.IncluirJerarquias = true;
            pAplicaReporte.AdicionarUsuarioWEB = true;
            pAplicaReporte.EnviarUltimoAcceso = true;
            pAplicaReporte.SinCtaEnviarAlReponsableCC = true;

            pvchCodBanderas = "BanderasConsumoPersonal";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "CP");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "CP", defaultValue);
        }
    }
}
