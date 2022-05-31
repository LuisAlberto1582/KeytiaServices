/*
 * Nombre:		    PGS
 * Fecha:		    20111028
 * Descripción:	    Clase para agregar subhistóricos de distintos tipos de Tarifas
 * Modificación:	
 */
using System;
using System.Data;
using KeytiaServiceBL;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface
{
    public class CnfgTarifaFieldCollection : HistoricFieldCollection
    {
        protected string psSubHistoricoClass;
        protected string psSubHistoricoFieldClass;

        public virtual void InitCollectionTarifa(WebControl lContainer, int liCodEntidad, int liCodConfig, Table lTablaAtributos, ValidacionPermisos lValidarPermiso, string lsSubHistoricoClass, string lsSubHistoricoFieldClass)
        {
            psSubHistoricoClass = lsSubHistoricoClass;
            psSubHistoricoFieldClass = lsSubHistoricoFieldClass;
            base.InitCollection(lContainer,liCodEntidad,liCodConfig,lTablaAtributos,lValidarPermiso);
        }

        protected override void AgregarSubHistoricos()
        {

            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Tarifa'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro, vchDescripcion from Maestros where iCodEntidad =" + liCodEntidad + " and dtIniVigencia <> dtFinVigencia");
            if (pdtMaestros == null || pdtMaestros.Rows.Count == 0)
            {
                return;
            }

            for (int iMae = 0; iMae < pdtMaestros.Rows.Count; iMae++)
            {
                AgregarSubHistoricField(psSubHistoricoFieldClass, liCodEntidad, (int)pdtMaestros.Rows[iMae]["iCodRegistro"], psSubHistoricoClass, "KeytiaWeb.UserInterface.CnfgTarifaFieldCollection");
            }
        }

    }
}
