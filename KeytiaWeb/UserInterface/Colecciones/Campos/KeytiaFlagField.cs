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
using System.Globalization;
using System.Web.SessionState;

namespace KeytiaWeb.UserInterface
{
    public class KeytiaFlagField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSOFlags pDSOflag;

        public KeytiaFlagField() { }

        public override bool ShowInGrid
        {
            get
            {
                return false;
            }
        }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOflag;
            }
        }

        public override void CreateField()
        {
            pDSOflag = new DSOFlags();
            InitDSOControlDB();
        }

        public virtual void Fill()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib}= " + pConfigValue);
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lKDBRow["{Value}"];
                if (lKDBTable.Columns.Contains(psLang))
                {
                    lDataRow["text"] = lKDBRow[psLang];
                }
                else
                {
                    lDataRow["text"] = lKDBRow["vchDescripcion"];
                }
                lDataTable.Rows.Add(lDataRow);
            }

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();
            double lvalue;
            double lbit;
            lViewData.Sort = "value ASC, text ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value)
                {
                    lvalue = double.Parse(lViewRow["value"].ToString());
                    lbit = Math.Log10(lvalue) / Math.Log10(2);
                    if (Math.Truncate(lbit) == lbit
                        && lDataSource.Select("value = " + lvalue).Length == 0)
                    {
                        lDataSource.ImportRow(lViewRow.Row);
                    }
                }
            }

            pDSOflag.DataSource = lDataSource;
            pDSOflag.Fill();
        }

        public override void InitDSOControlDBLanguage()
        {
            base.InitDSOControlDBLanguage();
            Fill();
        }
    }

    public class RelationFlagField : KeytiaFlagField
    {
        public RelationFlagField() { }

        protected override void InitDSOControlDB()
        {
            base.InitDSOControlDB();
            pDSOflag.Descripcion = Globals.GetMsgWeb(false, "Flags");
        }

        public override int ConfigValue
        {
            get
            {
                return pConfigValue;
            }
            set
            {
                pConfigValue = value;
                pConfigName = pColumn.Replace("iCodCatalogo", "iFlags");
            }
        }

        public KeytiaBaseField CatField
        {
            get
            {
                return pCollection[this.pConfigName];
            }
        }

        public override void Fill()
        {
            string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
            DataTable lDataSource = new DataTable();
            DataRow lDataRow;
            lDataSource.Columns.Add(new DataColumn("value", typeof(int)));
            lDataSource.Columns.Add(new DataColumn("text", typeof(string)));

            if ((pConfigValue & 1) == 1)
            {
                lDataRow = lDataSource.NewRow();
                lDataRow["value"] = 1;
                lDataRow["text"] = Globals.GetMsgWeb("Exclusividad");
                lDataSource.Rows.Add(lDataRow);
            }
            if ((pConfigValue & 2) == 2)
            {
                lDataRow = lDataSource.NewRow();
                lDataRow["value"] = 2;
                lDataRow["text"] = Globals.GetMsgWeb("Responsabilidad");
                lDataSource.Rows.Add(lDataRow);
            }
            if ((pConfigValue & 4) == 4)
            {
                lDataRow = lDataSource.NewRow();
                lDataRow["value"] = 4;
                lDataRow["text"] = Globals.GetMsgWeb("Baja Cascada");
                lDataSource.Rows.Add(lDataRow);
            }

            pDSOflag.AddClientEvent("valorDefault", pConfigValue.ToString());
            pDSOflag.DataSource = lDataSource;
            pDSOflag.Fill();
        }

        public override void InitLanguage()
        {
            pDescripcion = Globals.GetMsgWeb(false, "Flags");
            Fill();
        }
    }
}
