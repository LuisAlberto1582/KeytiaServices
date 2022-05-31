/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Reporte Mensual por Centro de Costos
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Reporte_Mensual_Centro_Costos : ReporteEspecial
    {
        public Reporte_Mensual_Centro_Costos()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.CenCos = true;

            pvchCodBanderas = "BanderasRepMesCC";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "RMCC");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "RMCC", defaultValue);
        }
    }
}
