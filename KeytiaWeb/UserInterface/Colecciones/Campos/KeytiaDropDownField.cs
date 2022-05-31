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
    public class KeytiaDropDownField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSODropDownList pDSOddl;

        public KeytiaDropDownField() { }

        public override DSOControlDB DSOControlDB
        {
            get
            {
                return pDSOddl;
            }
        }

        public override void CreateField()
        {
            pDSOddl = new DSODropDownList();
            InitDSOControlDB();

            pDSOddl.SelectItemText = Globals.GetMsgWeb(false, "Seleccione");
        }

        public void Fill()
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt(pConfigName, "");
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));

            foreach (DataRow lKDBRow in lKDBTable.Rows)
            {
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lKDBRow["iCodCatalogo"];
                if (lKDBTable.Columns.Contains(psLang) && lKDBRow[psLang] != DBNull.Value)
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
            lViewData.Sort = "text ASC, value ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                lDataSource.ImportRow(lViewRow.Row);
            }

            pDSOddl.DataSource = lDataSource;
            pDSOddl.Fill();
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pDescripcion == pConfigName)
            {
                pDescripcion = Globals.GetLangItem("", "Entidades", pConfigName);
            }
        }

        public override void InitDSOControlDBLanguage()
        {
            base.InitDSOControlDBLanguage();
            Fill();
        }
    }

}
