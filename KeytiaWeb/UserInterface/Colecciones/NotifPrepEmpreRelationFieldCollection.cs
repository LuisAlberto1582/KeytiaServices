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
    public class NotifPrepEmpreRelationFieldCollection : RelationFieldCollection
    {
        public NotifPrepEmpreRelationFieldCollection()
        {
        }

        public NotifPrepEmpreRelationFieldCollection(int liCodEntidad, int liCodRelacion)
            : base(liCodEntidad, liCodRelacion) { }

        public NotifPrepEmpreRelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        public override void InitFields()
        {
            base.InitFields();
            HistoricEdit lHistorico = (HistoricEdit)pContainer;
            if (ContainsConfigName("Emple") && lHistorico.Fields.ContainsConfigName("Empre"))
            {
                KeytiaAutoCompleteField lFieldEmpre = (KeytiaAutoCompleteField)lHistorico.Fields.GetByConfigName("Empre");
                RelationAutoCompleteField lFieldEmple = (RelationAutoCompleteField)GetByConfigName("Emple");
                ((DSOAutocomplete)lFieldEmple.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchEmpreEmple"); //buscar empleados por empresa
                ((DSOAutocomplete)lFieldEmple.DSOControlDB).FnSearch = "function(request, response){" + lFieldEmple.JsObj + ".fnSearchEmpreEmple(this, request, response); }";
                lFieldEmple.DSOControlDB.AddClientEvent("empreId", "#" + ((DSOAutocomplete)lFieldEmpre.DSOControlDB).TextValue.ClientID);
                lFieldEmple.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgEmpleadosEdit";
                lFieldEmple.SubCollectionClass = "KeytiaWeb.UserInterface.CnfgEmpleadosFieldCollection";
            }
        }
    }
}
