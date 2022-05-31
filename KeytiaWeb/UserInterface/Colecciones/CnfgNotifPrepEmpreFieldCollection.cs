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
    public class CnfgNotifPrepEmpreFieldCollection : HistoricFieldCollection
    {
        public CnfgNotifPrepEmpreFieldCollection()
        {
        }

        public CnfgNotifPrepEmpreFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgNotifPrepEmpreFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);

            if (lField is KeytiaRelationField
                && lField.ConfigName == "Notificaciones de Presupuestos de Empresas - Excepciones de Empleados")
            {
                ((KeytiaRelationField)lField).RelationCollectionClass = "KeytiaWeb.UserInterface.NotifPrepEmpreRelationFieldCollection";
            }
            return lField;
        }
    }
}
