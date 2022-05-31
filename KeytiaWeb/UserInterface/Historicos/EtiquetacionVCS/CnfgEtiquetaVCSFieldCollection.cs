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
    public class CnfgEtiquetaVCSFieldCollection : HistoricFieldCollection
    {
        public CnfgEtiquetaVCSFieldCollection()
        { 
        }

        public CnfgEtiquetaVCSFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgEtiquetaVCSFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro) { }

        protected override void FillMetaData()
        {
            base.FillMetaData();

            DataRow[] ldr = pMetaData.Select("ConfigName = 'TMSSystems'");
            if (ldr.Length > 0)
            {
                DataRow lDataRow = ldr[0];
                lDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.KeytiaAutoFilteredField";
            }
        }

        protected override void CreateFields()
        {
            base.CreateFields();
            if (ContainsConfigName("TMSSystems"))
            {
                KeytiaAutoFilteredField lField = (KeytiaAutoFilteredField)GetByConfigName("TMSSystems");
                lField.iCodDataSourceParam = (int)DSODataAccess.ExecuteScalar(
                    "select IsNull(iCodCatalogo, 0) " + "\r\n" +
                    "from [VisHistoricos('DataSourceParam','DataSource Parametro','Español')] " + "\r\n" +
                    "where dtIniVigencia <> dtFinVigencia " + "\r\n" +
                    "and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + "\r\n" +
                    "and dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + "\r\n" +
                    "and vchCodigo = 'DSTMSSystems'", (object)0);
            }
        }

        protected override void AgregarSubHistoricos()
        {
            int liCodEntidad = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro from Catalogos where iCodCatalogo is null and vchCodigo = 'EtiquetaVCS' and dtiniVigencia <> dtfinVigencia");
            int liCodMaestro = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro from Maestros where iCodEntidad = " + liCodEntidad + " and vchDescripcion = 'DetalleVCS' and dtIniVigencia <> dtFinVigencia");
            pdtMaestros = DSODataAccess.Execute("select iCodRegistro,iCodEntidad,vchDescripcion from Maestros where iCodEntidad = " + liCodEntidad + " and vchDescripcion = 'DetalleVCS' and dtIniVigencia <> dtFinVigencia");

            AgregarSubHistoricField(liCodEntidad, liCodMaestro, "KeytiaWeb.UserInterface.CnfgSubHisDetalleVCS", "KeytiaWeb.UserInterface.CnfgEtiquetaVCSDetFieldCollection");
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
            lMetaDataRow["KeytiaField"] = "KeytiaWeb.UserInterface.CnfgEtiquetaVCSGridField";
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
