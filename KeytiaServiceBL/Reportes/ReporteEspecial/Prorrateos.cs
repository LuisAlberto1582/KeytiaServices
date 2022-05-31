/*
Nombre:		    Jaime Aleman
Fecha:		    20111006
Descripción:	Reporte Especial de Prorrateos
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Prorrateos : ReporteEspecial
    {
        public Prorrateos()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Emple = true;
            pAplicaReporte.Carrier = true;

            pvchCodBanderas = "BanderasProrrateos";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "PRO");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "PRO", defaultValue);
        }
    }
}
