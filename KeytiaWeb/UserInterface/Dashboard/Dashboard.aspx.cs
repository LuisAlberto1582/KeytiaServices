using System;
using System.Web.UI.WebControls;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface
{
    public partial class Dashboard : KeytiaPage
    {
        protected KDBAccess pKDB = new KDBAccess();
        protected DashboardControl pDashboard;

        public Dashboard()
        {
            Init += new EventHandler(Dashboard_Init);
        }

        protected void Dashboard_Init(object sender, EventArgs e)
        {
            try
            {
                int liCodPerfil = (int)Session["iCodPerfil"]; //pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo = " + Session["iCodUsuario"]).Rows[0]["{Perfil}"];                
                int liCodAplicacion = (int)pKDB.GetHisRegByCod("OpcMnu", new string[] { Request.Params["Opc"].Replace("'", "''") }).Rows[0]["{Aplic}"];

                pDashboard = new DashboardControl();
                pDashboard.ID = "Dashboard1";
                pDashboard.OpcMnu = Request.Params["Opc"];
                pDashboard.lblTitle = lblTitle;
                pDashboard.iCodDashboard = liCodAplicacion;
                pDashboard.iCodPerfil = liCodPerfil;
                pDashboard.ParentContainer = Content;

                Content.Controls.Add(pDashboard);
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrInitControls", ex);
            }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            pDashboard.CreateControls();

            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.xlsx:
                    pDashboard.ExportXLS();
                    break;
                case KeytiaExportFormat.docx:
                    pDashboard.ExportDOC();
                    break;
                case KeytiaExportFormat.pdf:
                    pDashboard.ExportPDF();
                    break;
                case KeytiaExportFormat.csv:
                    pDashboard.ExportCSV();
                    break;
            }
        }

        protected override void InitLanguage()
        {
            base.InitLanguage();
            pDashboard.Title = Globals.GetLangItem("OpcMnu", "Opciones de Menu", pDashboard.OpcMnu.Replace("'", "''"));
            pDashboard.InitLanguage();
        }
    }
}
