/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Reporte de Colaboradores
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Reporte_Colaboradores : ReporteEspecial
    {
        public Reporte_Colaboradores()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Emple = true;

            pAplicaReporte.IncluirJerarquias = true;
            pAplicaReporte.AdicionarUsuarioWEB = true;
            pAplicaReporte.EnviarUltimoAcceso = true;
            pAplicaReporte.SinCtaEnviarASoporteInterno = true;

            pvchCodBanderas = "BanderasReporteColaboradores";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "RC");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "RC", defaultValue);
        }
    }
}
