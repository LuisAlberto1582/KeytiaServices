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
    public class CartasCustodiaFieldCollection : HistoricFieldCollection
    {
        protected int piCodEntidadCartasCust;

        public CartasCustodiaFieldCollection()
        {
        }

        public CartasCustodiaFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CartasCustodiaFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void AgregarSubHistoricos()
        {
            piCodEntidadCartasCust = (int)DSODataAccess.ExecuteScalar("select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'CartaCust'");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + piCodEntidadCartasCust + " and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField("CartaCust", "Recursos Pendientes", "KeytiaWeb.UserInterface.CartasCustodia", "KeytiaWeb.UserInterface.CartasCustodiaFieldCollection");
            AgregarSubHistoricField("CartaCust", "Recursos Pendientes por Liberar", "KeytiaWeb.UserInterface.CartasCustodia", "KeytiaWeb.UserInterface.CartasCustodiaFieldCollection");
        }

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

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);
            if (ContainsConfigName("Atrib") && ContainsConfigName("Valores"))
            {
                KeytiaBaseField lField = GetByConfigName("Valores");
                lField.DSOControlDB.AddClientEvent("entidadField", "Atrib");
            }
        }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            lField.SubCollectionClass = "KeytiaWeb.UserInterface.CartasCustodiaFieldCollection";
            return lField;
        }

        protected override void AgregarSubHistoricField(int liCodEntidad, int liCodMaestro, string lSubHistoricClass, string lSubCollectionClass)
        {
            DataRow lMetaDataRow = pMetaData.NewRow();
            string lsMaestro = pdtMaestros.Select("iCodRegistro = " + liCodMaestro)[0]["vchDescripcion"].ToString();
            DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + lsMaestro + "'");

            lMetaDataRow["Column"] = "Mae" + liCodMaestro;
            lMetaDataRow["iCodEntidad"] = liCodEntidad;
            lMetaDataRow["ConfigValue"] = liCodMaestro;
            lMetaDataRow["Row"] = pMetaData.Rows.Count + 1;
            lMetaDataRow["Col"] = 1;
            lMetaDataRow["ColumnSpan"] = 4;
            lMetaDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.CartasCustodiaGridField";
            if (lKDBTable.Rows.Count > 0)
            {
                lMetaDataRow["LangEntity"] = "MsgWeb";
                lMetaDataRow["LangValue"] = lKDBTable.Rows[0]["iCodCatalogo"];
            }
            lMetaDataRow["SubHistoricClass"] = lSubHistoricClass;
            lMetaDataRow["SubCollectionClass"] = lSubCollectionClass;

            pMetaData.Rows.Add(lMetaDataRow);
        }
    }
}