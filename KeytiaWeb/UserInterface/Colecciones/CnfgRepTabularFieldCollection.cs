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
    public class CnfgRepTabularFieldCollection : CnfgRepFieldCollection
    {
        protected int piCodEntidadCampos;

        public CnfgRepTabularFieldCollection()
        { 
        }

        public CnfgRepTabularFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) {}

        public CnfgRepTabularFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadCampos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'RepEstCampo'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadCampos + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("RepEstCampo", "Campos", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Parametros Reporte", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos Grafica", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos Grafica Historica", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
        }
    }
}
