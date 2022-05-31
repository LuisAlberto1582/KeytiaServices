/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Trafico Por Tipo de Llamada
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Trafico_Por_Tipo_Llamada : ReporteEspecial
    {
        public Trafico_Por_Tipo_Llamada()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio  = true;

            pvchCodBanderas = "BanderasTrafTipoLlam";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "TPTL");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "TPTL", defaultValue);
        }
    }
}
