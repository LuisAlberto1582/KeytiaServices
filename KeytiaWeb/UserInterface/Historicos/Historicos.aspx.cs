using System;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web;
using System.Collections.Generic;
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public partial class Historicos : KeytiaPage
    {
        protected HistoricEdit Historico;
        protected KDBAccess pKDB = new KDBAccess();

        public Historicos()
        {
            Init += new EventHandler(Historicos_Init);
        }

        protected void Historicos_Init(object sender, EventArgs e)
        {
            try
            {
                int liCodAplicacion = (int)pKDB.GetHisRegByCod("OpcMnu", new string[] { Request.Params["Opc"].Replace("'", "''") }).Rows[0]["{Aplic}"];
                DataTable lKDBTable = pKDB.GetHisRegByEnt("Aplic", "Aplicaciones del Sistema", "iCodCatalogo = " + liCodAplicacion);

                if (lKDBTable.Rows[0]["{ParamVarChar3}"] != DBNull.Value)
                {
                    Historico = (HistoricEdit)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricEdit)).CodeBase, lKDBTable.Rows[0]["{ParamVarChar3}"].ToString()).Unwrap();
                }
                else
                {
                    Historico = new HistoricEdit();
                }
                if (lKDBTable.Rows[0]["{ParamVarChar4}"] != DBNull.Value)
                {
                    Historico.CollectionClass = lKDBTable.Rows[0]["{ParamVarChar4}"].ToString();
                }
                if (lKDBTable.Rows[0]["{ParamVarChar1}"] != DBNull.Value)
                {
                    Historico.SetEntidad(lKDBTable.Rows[0]["{ParamVarChar1}"].ToString());

                    if (lKDBTable.Rows[0]["{ParamVarChar2}"] != DBNull.Value)
                    {
                        Historico.SetMaestro(lKDBTable.Rows[0]["{ParamVarChar2}"].ToString());
                    }
                }

                Historico.ID = "HistoricEdit1";
                Historico.OpcMnu = Request.Params["Opc"];
                Session["OpcMenu"] = Request.Params["Opc"];             
                Historico.iCodAplicacion = liCodAplicacion;
                Historico.lblTitle = lblTitle;
                Content.Controls.Add(Historico);

                Historico.LoadScripts();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrInitControls", ex);
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            Historico.CreateControls();
            
            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.xlsx:
                    Historico.ExportXLS();
                    break;
                case KeytiaExportFormat.docx:
                    Historico.ExportDOC();
                    break;
                case KeytiaExportFormat.pdf:
                    Historico.ExportPDF();
                    break;
                case KeytiaExportFormat.csv:
                    Historico.ExportCSV();
                    break;
            }
        }

        protected override void InitLanguage()
        {
            base.InitLanguage();
            Historico.Title = Globals.GetLangItem("OpcMnu", "Opciones de Menu", Historico.OpcMnu.Replace("'", "''"));
            Historico.InitLanguage();
        }

    }
}
