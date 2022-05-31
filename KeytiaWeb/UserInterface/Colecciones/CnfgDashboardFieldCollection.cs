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
    public class CnfgDashboardFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadBloques;

        public CnfgDashboardFieldCollection()
        {
        }

        public CnfgDashboardFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgDashboardFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadBloques = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'BloqueDashboard'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadBloques + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("BloqueDashboard", "Bloque de dashboard", "KeytiaWeb.UserInterface.CnfgBloqueDashboard", "KeytiaWeb.UserInterface.CnfgBloqueDashboardFieldCollection");
        }
    }
}
