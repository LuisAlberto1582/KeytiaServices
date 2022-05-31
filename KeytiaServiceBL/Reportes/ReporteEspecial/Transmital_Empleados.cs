/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Transmital Empleados
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Transmital_Empleados : ReporteEspecial
    {
        public Transmital_Empleados()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Emple = true;

            pAplicaReporte.IncluirJerarquias = true;
            pAplicaReporte.AdicionarUsuarioWEB = true;
            pAplicaReporte.EnviarUltimoAcceso = true;
            pAplicaReporte.SinCtaEnviarASoporteInterno = true;

            pvchCodBanderas = "BanderasTransmital";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "TE");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "TE", defaultValue);
        }
    }
}
