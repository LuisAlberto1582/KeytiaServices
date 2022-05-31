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
    public class CnfgRepMatricialFieldCollection : CnfgRepTabularFieldCollection
    {
        public CnfgRepMatricialFieldCollection()
        {
        }

        public CnfgRepMatricialFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgRepMatricialFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadCampos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'RepEstCampo'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadCampos + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("RepEstCampo", "Campos Eje Y", "KeytiaWeb.UserInterface.CnfgRepMatCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos Eje X", "KeytiaWeb.UserInterface.CnfgRepMatCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos XY", "KeytiaWeb.UserInterface.CnfgRepMatCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Parametros Reporte", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos Grafica", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
            AgregarSubHistoricField("RepEstCampo", "Campos Grafica Historica", "KeytiaWeb.UserInterface.CnfgReportesCamposEdit", "KeytiaWeb.UserInterface.CnfgRepFieldCollection");
        }
    }
}
