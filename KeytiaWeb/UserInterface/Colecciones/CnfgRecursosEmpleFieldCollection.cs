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
    public class CnfgRecursosEmpleFieldCollection: HistoricFieldCollection
    {
        public CnfgRecursosEmpleFieldCollection()
        {
        }
        public CnfgRecursosEmpleFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
        : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) {}

        public CnfgRecursosEmpleFieldCollection(int liCodEntidad, int liCodMaestro)
        : base(liCodEntidad, liCodMaestro, true) { }

        protected override void FillMetaData()
        {
            base.FillMetaData();

            int liCodAtributos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'");
            if (pMetaData.Select("ConfigValue = " + liCodAtributos).Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigValue = " + liCodAtributos)[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAttibuteField";

                int liCodValores = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Valores'");
                if (pMetaData.Select("ConfigValue = " + liCodValores).Length > 0)
                {
                    lRowAtrib = pMetaData.Select("ConfigValue = " + liCodValores)[0];
                    lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAttibuteValueField";
                }
            }
        }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            lField.SubCollectionClass = "KeytiaWeb.UserInterface.CnfgEmpleadosFieldCollection";
            if (lField is KeytiaRelationField)
            {
                if ((lField.ConfigName == "Empleado - CodAutorizacion") ||
                    (lField.ConfigName == "Empleado - Extension") ||
                    (lField.ConfigName == "Empleado - Linea"))
                {
                    ((KeytiaRelationField)lField).RelationCollectionClass = "KeytiaWeb.UserInterface.RecursosEmpleRelationFieldCollection";
                }
            }
            return lField;
        }
    }
}
