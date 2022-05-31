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
    public class KeytiaDropDownOptionField : KeytiaBaseField, IKeytiaFillableField
    {
        protected DSODropDownList pDSOddl;

        public KeytiaDropDownOptionField() { }

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
                return pDSOddl;
            }
        }

        public override void CreateField()
        {
            pDSOddl = new DSODropDownList();
            InitDSOControlDB();
        }

        public void Fill()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib}= " + pConfigValue);
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));
            lDataTable.Columns.Add(new DataColumn("order", typeof(int)));

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
                lDataRow["order"] = lKDBRow["{OrdenPre}"];
                lDataTable.Rows.Add(lDataRow);
            }

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();
            lViewData.Sort = "order ASC, value ASC, text ASC";
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value
                    && lDataSource.Select("value = " + lViewRow["value"]).Length == 0)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
            }

            pDSOddl.DataSource = lDataSource;
            pDSOddl.Fill();
        }

        public override void InitDSOControlDBLanguage()
        {
            base.InitDSOControlDBLanguage();
            Fill();
        }

        //NZ 201508112
        public void InitDSOControlDBLanguageFromDataTable(DataTable dtDatos)
        {
            base.InitDSOControlDBLanguage();
            FillFromDataTable(dtDatos);
        }
        //NZ 201508112
        public void FillFromDataTable(DataTable dtDatos)
        {
            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add(new DataColumn("value", typeof(int)));
            lDataTable.Columns.Add(new DataColumn("text", typeof(string)));
            lDataTable.Columns.Add(new DataColumn("order", typeof(int)));

            int orden = 0;
            foreach (DataRow lKDBRow in dtDatos.Rows)
            {
                orden++;
                DataRow lDataRow = lDataTable.NewRow();
                lDataRow["value"] = lKDBRow["Value"];
                if (dtDatos.Columns.Contains(psLang))
                {
                    lDataRow["text"] = lKDBRow[psLang];
                }
                else
                {
                    lDataRow["text"] = lKDBRow["vchDescripcion"];
                }
                lDataRow["order"] = orden;
                lDataTable.Rows.Add(lDataRow);
            }

            DataRow opcDefault = lDataTable.NewRow();
            opcDefault["value"] = -1;
            opcDefault["text"] = "--Todos--";
            opcDefault["order"] = -1;

            lDataTable.Rows.InsertAt(opcDefault, 0);

            DataView lViewData = lDataTable.DefaultView;
            DataTable lDataSource = lDataTable.Clone();           
            foreach (DataRowView lViewRow in lViewData)
            {
                if (lViewRow["value"] != DBNull.Value
                    && lDataSource.Select("value = " + lViewRow["value"]).Length == 0)
                {
                    lDataSource.ImportRow(lViewRow.Row);
                }
            }

            pDSOddl.DataSource = lDataSource;            
            pDSOddl.Fill();
        }
    }
}
