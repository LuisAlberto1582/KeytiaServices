using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public class CnfgCenCosFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadPrepCenCos;

        public CnfgCenCosFieldCollection()
        {
        }
        public CnfgCenCosFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgCenCosFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadPrepCenCos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'PrepCenCos'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadPrepCenCos + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubPresupuestoField", "PrepCenCos", "Presupuesto Fijo", "KeytiaWeb.UserInterface.CnfgPrepCenCosFijo", "KeytiaWeb.UserInterface.HistoricFieldCollection");
            AgregarSubHistoricField("KeytiaWeb.UserInterface.CnfgSubPresupuestoField", "PrepCenCos", "Presupuesto Temporal", "KeytiaWeb.UserInterface.CnfgPrepCenCosTemporal", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }
    }
}
