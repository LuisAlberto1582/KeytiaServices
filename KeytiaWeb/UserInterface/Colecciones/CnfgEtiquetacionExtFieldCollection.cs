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
using System.Globalization;
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class CnfgEtiquetacionExtFieldCollection : HistoricFieldCollection
    {
        protected override void FillMetaData()
        {
            base.FillMetaData();
            if (pMetaData.Select("ConfigName = 'CenCos'").Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigName = 'CenCos'")[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoRestrictedField";
            }
            if (pMetaData.Select("ConfigName = 'Emple'").Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigName = 'Emple'")[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoRestrictedField";
            }
            if (pMetaData.Select("ConfigName = 'Sitio'").Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigName = 'Sitio'")[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoRestrictedField";
            }
        }
    }
}
