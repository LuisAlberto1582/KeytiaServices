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
    public class CnfgBloqueDashboardFieldCollection : HistoricFieldCollection
    {
        public CnfgBloqueDashboardFieldCollection()
        {
        }

        public CnfgBloqueDashboardFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgBloqueDashboardFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override void FillMetaData()
        {
            base.FillMetaData();

            if (pMetaData.Select("ConfigName = 'Consul'").Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigName = 'Consul'")[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaConsultaDashboardField";
            }
        }

    }
}
