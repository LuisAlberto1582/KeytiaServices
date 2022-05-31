/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Resumen de Gasto
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Resumen_Gasto : ReporteEspecial
    {
        public Resumen_Gasto()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio = true;

            pvchCodBanderas = "BanderasResumGto";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "RG");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "RG", defaultValue);
        }
    }
}
