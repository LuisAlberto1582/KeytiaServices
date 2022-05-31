using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class ConsultRelationFieldCollection : RelationFieldCollection
    {
        public ConsultRelationFieldCollection()
        {
        }

        public ConsultRelationFieldCollection(int liCodEntidad, int liCodRelacion)
            : base(liCodEntidad, liCodRelacion) { }

        public ConsultRelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        public override void InitFields()
        {
            base.InitFields();
            if (ContainsConfigName("Atrib"))
            {
                RelationAutoCompleteField lFieldAtrib = (RelationAutoCompleteField)GetByConfigName("Atrib");
                ((DSOAutocomplete)lFieldAtrib.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchAttribute");
                if (ContainsConfigName("Consul"))
                {
                    RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("Consul");
                    ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchConsulta");
                    ((DSOAutocomplete)lField.DSOControlDB).FnSearch = "function(request, response){" + lField.JsObj + ".fnSearchConsulta(this, request, response); }";
                    lField.DSOControlDB.AddClientEvent("atribId", "#" + ((DSOAutocomplete)lFieldAtrib.DSOControlDB).TextValue.ClientID);
                    lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgValorConsultasEdit";
                    StringBuilder lsbScript = new StringBuilder();
                    lsbScript.AppendLine("<script type='text/javascript'>");
                    lsbScript.AppendLine("jQuery(function($){");
                    lsbScript.AppendLine(lField.JsObj + ".InitValorConsultasChange('#" + ((DSOAutocomplete)lFieldAtrib.DSOControlDB).Search.ClientID + "', '#" + ((DSOAutocomplete)lField.DSOControlDB).Search.ClientID + "');");
                    lsbScript.AppendLine("});");
                    lsbScript.AppendLine("</script>");
                    DSOControl.LoadControlScriptBlock(pContainer.Page, typeof(HistoricEdit), "ConsultRelationFieldCollection_InitFields" + pContainer.ClientID, lsbScript.ToString(), true, false);
                }
            }
        }
    }
}
