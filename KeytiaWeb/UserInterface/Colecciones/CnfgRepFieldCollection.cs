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
    public class CnfgRepFieldCollection : HistoricFieldCollection
    {
        public CnfgRepFieldCollection()
        { 
        }

        public CnfgRepFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) {}

        public CnfgRepFieldCollection(int liCodEntidad, int liCodMaestro)
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
            //Atributos adicionales para rutas
            liCodAtributos = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'AtribAd'");
            if (pMetaData.Select("ConfigValue = " + liCodAtributos).Length > 0)
            {
                DataRow lRowAtrib = pMetaData.Select("ConfigValue = " + liCodAtributos)[0];
                lRowAtrib["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAttibuteField";
            }
        }

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);
            if (ContainsConfigName("Atrib") && ContainsConfigName("Valores"))
            {
                KeytiaBaseField lField = GetByConfigName("Valores");
                lField.DSOControlDB.AddClientEvent("entidadField", "Atrib");
            }
            if (ContainsConfigName("DataSourceRep"))
            {
                KeytiaAutoCompleteField lField = (KeytiaAutoCompleteField)GetByConfigName("DataSourceRep");
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchDataSourceRep");
            }
            if (ContainsConfigName("DataSourceRepMat"))
            {
                KeytiaAutoCompleteField lField = (KeytiaAutoCompleteField)GetByConfigName("DataSourceRepMat");
                ((DSOAutocomplete)lField.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchDataSourceRep");
            }

        }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            lField.SubCollectionClass = "KeytiaWeb.UserInterface.CnfgRepFieldCollection";
            if (lField is KeytiaRelationField
                && lField.ConfigName == "Aplicación - Estado - Perfil - Atributo - Consulta - Reporte")
            {
                ((KeytiaRelationField)lField).RelationCollectionClass = "KeytiaWeb.UserInterface.ConsultRelationFieldCollection";
            }
            return lField;
        }
    }
}
