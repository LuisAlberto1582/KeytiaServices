using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb
{
    public partial class Utilerias : KeytiaPage
    {
        static string algo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = algo;
        }

        protected void btnWebClearCache_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.ClearCache();
        }

        protected void btnWebClearKDB_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.KDBAccess.CleanBuffer();
        }

        protected void btnStaticTest_Click(object sender, EventArgs e)
        {
            algo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Label1.Text = algo;
        }

        protected void btnWebViewCache_Click(object sender, EventArgs e)
        {
            int i;

            System.Web.UI.HtmlControls.HtmlTable t;
            System.Web.UI.HtmlControls.HtmlTableRow tr = null;
            System.Web.UI.HtmlControls.HtmlTableCell td = null;

            Panel1.Controls.Clear();

            //Variables
            {
                t = new System.Web.UI.HtmlControls.HtmlTable();
                Panel1.Controls.Add(t);

                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Variable</b>";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Valor</b>";
                tr.Controls.Add(td);


                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = "Esquema";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = KeytiaServiceBL.DSODataContext.Schema;
                tr.Controls.Add(td);


                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = "Running Mode";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = KeytiaServiceBL.DSODataContext.RunningMode.ToString();
                tr.Controls.Add(td);


                //tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                //t.Controls.Add(tr);

                //td = new System.Web.UI.HtmlControls.HtmlTableCell();
                //td.InnerHtml = "String de conexion";
                //tr.Controls.Add(td);

                //td = new System.Web.UI.HtmlControls.HtmlTableCell();
                //td.InnerHtml = KeytiaServiceBL.DSODataContext.ConnectionString;
                //tr.Controls.Add(td);


                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = "Temp Path";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = System.IO.Path.GetTempPath();
                tr.Controls.Add(td);
            }


            //Sesion
            {
                t = new System.Web.UI.HtmlControls.HtmlTable();
                Panel1.Controls.Add(t);

                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<h2>Sesion</h2>";
                tr.Controls.Add(td);

                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Llave</b>";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Valor</b>";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Tipo</b>";
                tr.Controls.Add(td);

                i = 0;

                foreach (string k in HttpContext.Current.Session.Keys)
                {
                    i++;

                    tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                    t.Controls.Add(tr);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = i.ToString();
                    td.Align = "right";
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = k;
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = HttpContext.Current.Session[k].ToString();
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = HttpContext.Current.Session[k].GetType().ToString() +
                        (HttpContext.Current.Session[k] is Hashtable ? " (" + ((Hashtable)HttpContext.Current.Session[k]).Count + " items)" :
                        (HttpContext.Current.Session[k] is DataTable ? " (" + ((DataTable)HttpContext.Current.Session[k]).Rows.Count + " rows, " + ((DataTable)HttpContext.Current.Session[k]).Columns.Count + " columns)" :
                        ""));
                    tr.Controls.Add(td);
                }
            }


            //Cache
            {
                t = new System.Web.UI.HtmlControls.HtmlTable();
                Panel1.Controls.Add(t);

                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<h2>Cache</h2>";
                tr.Controls.Add(td);

                tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                t.Controls.Add(tr);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Llave</b>";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Valor</b>";
                tr.Controls.Add(td);

                td = new System.Web.UI.HtmlControls.HtmlTableCell();
                td.InnerHtml = @"<b>Tipo</b>";
                tr.Controls.Add(td);


                System.Collections.IDictionaryEnumerator en = HttpContext.Current.Cache.GetEnumerator();
                en.Reset();

                i = 0;

                while (en.MoveNext())
                {
                    i++;

                    tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                    t.Controls.Add(tr);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = i.ToString();
                    td.Align = "right";
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = (string)en.Key;
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = en.Value.ToString();
                    tr.Controls.Add(td);

                    td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerHtml = en.Value.GetType().ToString() +
                        (en.Value is Hashtable ? " (" + ((Hashtable)en.Value).Count + " items)" :
                        (en.Value is DataTable ? " (" + ((DataTable)en.Value).Rows.Count + " rows, " + ((DataTable)en.Value).Columns.Count + " columns)" :
                        ""));
                    tr.Controls.Add(td);
                }
            }

            KeytiaServiceBL.DSODataContext.LogCache("Ver caché");
        }

        protected void btnSvcClearCache_Click(object sender, EventArgs e)
        {
            int liContext = KeytiaServiceBL.DSODataContext.GetContext();

            try
            {
                KeytiaServiceBL.DSODataContext.SetContext();

                KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro" }, "vchCodigo = 'KeytiaService'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    Hashtable lht = new Hashtable();
                    lht.Add("{LimpCache}", DateTime.Now);
                    loCom.ActualizaRegistro("Historicos", "Monitor", "Proceso", lht, (int)ldt.Rows[0]["iCodRegistro"], KeytiaServiceBL.DSODataContext.GetContext());
                }
            }
            finally
            {
                KeytiaServiceBL.DSODataContext.SetContext(liContext);
            }
        }

        protected void btnSvcClearKDB_Click(object sender, EventArgs e)
        {
            int liContext = KeytiaServiceBL.DSODataContext.GetContext();

            try
            {
                KeytiaServiceBL.DSODataContext.SetContext();

                KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro" }, "vchCodigo = 'KeytiaService'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    Hashtable lht = new Hashtable();
                    lht.Add("{LimpKDB}", DateTime.Now);
                    loCom.ActualizaRegistro("Historicos", "Monitor", "Proceso", lht, (int)ldt.Rows[0]["iCodRegistro"], KeytiaServiceBL.DSODataContext.GetContext());
                }
            }
            finally
            {
                KeytiaServiceBL.DSODataContext.SetContext(liContext);
            }
        }

        protected void btnComClearCache_Click(object sender, EventArgs e)
        {
            int liContext = KeytiaServiceBL.DSODataContext.GetContext();

            try
            {
                KeytiaServiceBL.DSODataContext.SetContext();

                KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro" }, "vchCodigo = 'KeytiaCOM'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    Hashtable lht = new Hashtable();
                    lht.Add("{LimpCache}", DateTime.Now);
                    loCom.ActualizaRegistro("Historicos", "Monitor", "Proceso", lht, (int)ldt.Rows[0]["iCodRegistro"], KeytiaServiceBL.DSODataContext.GetContext());
                }
            }
            finally
            {
                KeytiaServiceBL.DSODataContext.SetContext(liContext);
            }
        }

        protected void btnComClearKDB_Click(object sender, EventArgs e)
        {
            int liContext = KeytiaServiceBL.DSODataContext.GetContext();

            try
            {
                KeytiaServiceBL.DSODataContext.SetContext();

                KeytiaCOM.CargasCOM loCom = new KeytiaCOM.CargasCOM();
                DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "iCodRegistro" }, "vchCodigo = 'KeytiaCOM'");

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    Hashtable lht = new Hashtable();
                    lht.Add("{LimpKDB}", DateTime.Now);
                    loCom.ActualizaRegistro("Historicos", "Monitor", "Proceso", lht, (int)ldt.Rows[0]["iCodRegistro"], KeytiaServiceBL.DSODataContext.GetContext());
                }
            }
            finally
            {
                KeytiaServiceBL.DSODataContext.SetContext(liContext);
            }
        }
    }
}
