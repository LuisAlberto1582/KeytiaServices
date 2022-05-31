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

namespace KeytiaWeb.UserInterface
{
    public class RelationFieldCollection : KeytiaFieldCollection
    {
        protected bool pContieneFlags;

        public bool ContieneFlags
        {
            get
            {
                return pContieneFlags;
            }
        }

        public RelationFieldCollection()
        { 
        }

        public RelationFieldCollection(int liCodEntidad, int liCodRelacion)
        {
            this.piCodEntidad = liCodEntidad;
            this.piCodConfig = liCodRelacion;

            InitMetaData();
            FillMetaData();
            CreateFields(true);
        }

        public RelationFieldCollection(WebControl lContainer, int liCodEntidad, int liCodRelacion, Table lTablaAtributos, ValidacionPermisos lValidarPermisos)
            : base(lContainer, liCodEntidad, liCodRelacion, lTablaAtributos, lValidarPermisos) { }

        protected override void FillMetaData()
        {
            DataTable ldt = DSODataAccess.Execute("select * from Relaciones where iCodRegistro = " + piCodConfig);
            DataTable lMetaData = pMetaData.Clone();
            DataRow lDataRow;
            int liMaxRow = 0;
            int liNoConfigValue = 0;

            foreach (DataColumn lCol in ldt.Columns)
            {
                if (lCol.ColumnName.StartsWith("iCodCatalogo")
                    && ldt.Rows[0][lCol] != DBNull.Value)
                {
                    ++liMaxRow;

                    lDataRow = lMetaData.NewRow();
                    lDataRow["Column"] = lCol.ColumnName;
                    lDataRow["ConfigValue"] = ldt.Rows[0][lCol];
                    lDataRow["Row"] = liMaxRow;
                    lDataRow["Col"] = 1;

                    if (ldt.Rows[0][lCol.ColumnName.Replace("iCodCatalogo", "iFlags")] != DBNull.Value
                        && int.Parse(ldt.Rows[0][lCol.ColumnName.Replace("iCodCatalogo", "iFlags")].ToString())!=0)
                    {
                        pContieneFlags = true;
                        lDataRow["ColumnSpan"] = 1;
                        lMetaData.Rows.Add(lDataRow);

                        lDataRow = lMetaData.NewRow();
                        lDataRow["Column"] = lCol.ColumnName.Replace("iCodCatalogo", "iFlags");
                        lDataRow["ConfigValue"] = ldt.Rows[0][lCol.ColumnName.Replace("iCodCatalogo", "iFlags")];
                        lDataRow["Row"] = liMaxRow;
                        lDataRow["Col"] = 2;
                        lDataRow["ColumnSpan"] = 1;
                    }
                    else
                    {
                        lDataRow["ColumnSpan"] = 3;
                    }
                    lMetaData.Rows.Add(lDataRow);

                }
                else if (lCol.ColumnName.EndsWith("Vigencia"))
                {
                    lDataRow = lMetaData.NewRow();
                    lDataRow["Column"] = lCol.ColumnName;
                    lDataRow["ConfigValue"] = --liNoConfigValue;
                    lDataRow["Row"] = ++liMaxRow;
                    lDataRow["Col"] = 1;
                    lDataRow["ColumnSpan"] = 1;

                    lMetaData.Rows.Add(lDataRow);
                }
            }

            DataView lViewData = lMetaData.DefaultView;
            lViewData.Sort = "Col ASC, Row ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                pMetaData.ImportRow(lViewRow.Row);
            }
        }

        protected virtual void CreateFields(bool lbOnlyConfig)
        {
            KeytiaBaseField lField = null;

            foreach (DataRow ldr in pMetaData.Rows)
            {
                lField = CreateField(ldr);
                if (!lbOnlyConfig)
                {
                    lField.Table = pTablaAtritubos;
                    lField.CreateField();
                }

                Add(lField);
            }
        }

        protected virtual KeytiaBaseField CreateField(DataRow lMetaDataRow)
        {
            KeytiaBaseField lField = null;
            if (((string)lMetaDataRow["Column"]).StartsWith("iCodCatalogo"))
            {
                lField = new RelationAutoCompleteField();
            }
            else if (((string)lMetaDataRow["Column"]).StartsWith("iFlags"))
            {
                lField = new RelationFlagField();
            }
            else if (((string)lMetaDataRow["Column"]).EndsWith("Vigencia"))
            {
                lField = new KeytiaDateField();
            }
            else
            {
                throw new NotImplementedException();
            }

            lField.Container = pContainer;
            lField.ValidarPermiso = ValidarPermiso;
            lField.Column = (string)lMetaDataRow["Column"];
            lField.ConfigValue = (int)lMetaDataRow["ConfigValue"];
            lField.iCodEntidad = piCodEntidad;
            lField.Row = (int)lMetaDataRow["Row"];
            lField.Col = (int)lMetaDataRow["Col"];
            lField.ColumnSpan = (int)lMetaDataRow["ColumnSpan"];
            lField.Collection = this;

            return lField;
        }

        protected override void CreateFields()
        {
            CreateFields(false);
        }
    }
}
