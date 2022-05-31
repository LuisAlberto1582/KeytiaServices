/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Top Celulares
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Top_Celulares : ReporteEspecial
    {
        public Top_Celulares()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio = true;
            pAplicaReporte.Locali = true;

            pvchCodBanderas = "BanderasTopCelulares";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "TC");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "TC", defaultValue);
        }
    }
}
