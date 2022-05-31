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
    public class CnfgAtributosFieldCollection : HistoricFieldCollection
    {
        public CnfgAtributosFieldCollection()
        {
        }

        public CnfgAtributosFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgAtributosFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        public override void InitFields()
        {
            base.InitFields();
            if (ContainsConfigName("Types") && ContainsConfigName("Controles"))
            {
                KeytiaAutoCompleteField lFieldType = (KeytiaAutoCompleteField)GetByConfigName("Types");
                KeytiaAutoCompleteField lFieldControl = (KeytiaAutoCompleteField)GetByConfigName("Controles");

                ((DSOAutocomplete)lFieldControl.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchAtribControl");
                ((DSOAutocomplete)lFieldControl.DSOControlDB).FnSearch = "function(request, response){" + lFieldControl.JsObj + ".fnSearchAtribControl(this, request, response); }";
                lFieldControl.DSOControlDB.AddClientEvent("tipoDatoId", "#" + ((DSOAutocomplete)lFieldType.DSOControlDB).TextValue.ClientID);

                StringBuilder lsbScript = new StringBuilder();
                lsbScript.AppendLine("<script type='text/javascript'>");
                lsbScript.AppendLine("jQuery(function($){");
                lsbScript.AppendLine(lFieldControl.JsObj + ".InitAtribTypeChange('#" + ((DSOAutocomplete)lFieldType.DSOControlDB).Search.ClientID + "', '#" + ((DSOAutocomplete)lFieldControl.DSOControlDB).Search.ClientID + "');");
                lsbScript.AppendLine("});");
                lsbScript.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricEdit), "CnfgAtributosFieldCollection_InitFields" + pContainer.ClientID, lsbScript.ToString(), true, false);
            }
        }
    }
}
