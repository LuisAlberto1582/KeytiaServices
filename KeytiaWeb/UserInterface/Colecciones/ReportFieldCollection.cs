using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using DSOControls2008;
using System.Data;

namespace KeytiaWeb.UserInterface
{
    public class ReportFieldCollection : HistoricFieldCollection
    {
        public ReportFieldCollection(ReporteEstandar lContainer, int liCodConsulta, int liCodReporte, Table lTablaParametros, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodConsulta, liCodReporte, lTablaParametros, lValidarPermisos) { }

        protected override void InitMetaData()
        {
            base.InitMetaData();
            pMetaData.Columns.Add(new DataColumn("KeytiaFieldFilter", typeof(int)));
        }

        protected override void FillMetaData()
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt("RepEstCampo", "Parametros Reporte", "{RepEst} = " + piCodConfig);
            DataTable lMetaData = pMetaData.Clone();
            DataRow lDataRow;
            DataRow lRowControles;

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                lDataRow = lMetaData.NewRow();
                lDataRow["Column"] = lKDBRow["vchCodigo"];
                lDataRow["iCodEntidad"] = piCodEntidad;
                lDataRow["ConfigValue"] = lKDBRow["{Atrib}"];
                lDataRow["Row"] = lKDBRow["{ParamRenglon}"];
                lDataRow["Col"] = lKDBRow["{ParamColumna}"];
                lDataRow["ColumnSpan"] = lKDBRow["{ParamColSpan}"];
                lDataRow["LangEntity"] = "RepEstIdiomaCmp";
                lDataRow["LangValue"] = lKDBRow["{RepEstIdiomaCmp}"];

                lRowControles = pdtControles.Select("iCodCatalogo = " + lKDBRow["{Controles}"])[0];
                lDataRow["KeytiaField"] = lRowControles["{Clase}"];

                if (lKDBTable.Columns.Contains("{DataSourceParam}"))
                {
                    lDataRow["KeytiaFieldFilter"] = lKDBRow["{DataSourceParam}"];
                }

                lMetaData.Rows.Add(lDataRow);
            }

            DataView lViewData = lMetaData.DefaultView;
            lViewData.Sort = "Col ASC, Row ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                pMetaData.ImportRow(lViewRow.Row);
            }
        }

        protected override KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = base.CreateField(lMetaDataRow);
            lField.LangEntity = lMetaDataRow["LangEntity"].ToString();
            lField.LangValue = (int)lMetaDataRow["LangValue"];

            if (lField is KeytiaAutoFilteredField)
            {
                ((KeytiaAutoFilteredField)lField).iCodDataSourceParam = (int)KeytiaServiceBL.Util.IsDBNull(lMetaDataRow["KeytiaFieldFilter"], 0);
            }
            return lField;
        }

        protected override void CreateFields(bool lbOnlyGridFields)
        {
            base.CreateFields(lbOnlyGridFields);
            if (!lbOnlyGridFields)
            {
                foreach (KeytiaBaseField lField in this)
                {
                    if (lField is KeytiaMultiSelectField)
                    {
                        ((KeytiaMultiSelectField)lField).DSOControlDB.DataValueDelimiter = "";
                    }
                }
            }
        }
    }
}
