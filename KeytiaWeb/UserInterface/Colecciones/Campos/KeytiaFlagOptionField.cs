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
    public class KeytiaFlagOptionField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSORadioButtonList pDSOoption;

        public KeytiaFlagOptionField() { }

        public override object DataValue
        {
            get
            {
                if (pDSOoption.HasValue)
                {
                    return int.Parse(pDSOoption.DataValue.ToString());
                }
                else
                {
                    return pDSOoption.DataValue;
                }
            }
            set
            {
                base.DataValue = value;
            }
        }

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
                return pDSOoption;
            }
        }

        public override void CreateField()
        {
            pDSOoption = new DSORadioButtonList();
            pDSOoption.DataValueDelimiter = "";
            InitDSOControlDB();
        }

        public override void InitField()
        {
            base.InitField();
            pDSOoption.RadioButtonList.RepeatDirection = RepeatDirection.Horizontal;
            pDSOoption.RadioButtonList.RepeatColumns = 8;
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
            lViewData.Sort = "value ASC, text ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value
                    && lDataSource.Select("value = " + lViewRow["value"]).Length == 0)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
            }

            pDSOoption.DataSource = lDataSource;
            pDSOoption.Fill();
        }

        public override void InitDSOControlDBLanguage()
        {
            base.InitDSOControlDBLanguage();
            Fill();
        }
    }

    public class KeytiaFlagOptionNoSelectField : KeytiaFlagOptionField
    {
        public KeytiaFlagOptionNoSelectField() { }

        public override void CreateField()
        {
            base.CreateField();
            pDSOoption.SelectItemText = Globals.GetMsgWeb(false, "NoSelectText");
        }
    }
}
