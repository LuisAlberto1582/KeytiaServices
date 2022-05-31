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
    public class RecursosEmpleRelationFieldCollection: RelationFieldCollection
    {
        public RecursosEmpleRelationFieldCollection()
        {
        }

        public RecursosEmpleRelationFieldCollection(int liCodEntidad, int liCodRelacion)
            : base(liCodEntidad, liCodRelacion) { }

        public RecursosEmpleRelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        public override void InitFields()
        {
            base.InitFields();

            if (ContainsConfigName("Emple"))
            {
                RelationAutoCompleteField lField = (RelationAutoCompleteField)GetByConfigName("Emple");
                lField.SubHistoricClass = "KeytiaWeb.UserInterface.CnfgEmpleadosEdit";
            }
        }
    }
}
