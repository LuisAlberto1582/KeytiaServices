/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Reporte Especial de Gasto Servicio Medido
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KeytiaServiceBL.ReporteEspecial
{
    class Gasto_Servicio_Medido : ReporteEspecial
    {
        public Gasto_Servicio_Medido()
            : base()
        {
            pAplicaReporte = new AplicaReporteEspecial();
            pAplicaReporte.Sitio = true;

            pAplicaReporte.ServicioMedido = true;
            pAplicaReporte.LlamadasLocales = true;

            pvchCodBanderas = "BanderasGtoServMed";
        }

        protected override bool getValBandera(string vchCodigo)
        {
            return base.getValBandera(vchCodigo + "GSM");
        }

        protected override bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return base.getValBandera(vchCodigo + "GSM", defaultValue);
        }
    }
}
