/*
Nombre:		    Jaime Aleman
Fecha:		    20111028
Descripción:	Reporte Especial de Prorrateos
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Conciliacion : ReporteEspecial
    {
        public Conciliacion()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio  = true;

            pvchCodBanderas = "BanderasConciliacion";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "CON");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "CON", defaultValue);
        }
     }
}
