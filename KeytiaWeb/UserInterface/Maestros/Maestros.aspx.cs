/*
 * Nombre:		    DMM
 * Fecha:		    20110607
 * Descripción:	    Página de configuración de Maestros
 * Modificación:	
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using DSOControls2008;
using KeytiaServiceBL;
using System.Collections;

namespace KeytiaWeb.UserInterface
{
    public partial class Maestros : KeytiaPage
    {
        MaestroEdit MaestroEdit1;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            MaestroEdit1 = new MaestroEdit();
            MaestroEdit1.ID = "MaestroEdit1";
            Content.Controls.Add(MaestroEdit1);
            MaestroEdit1.CreateControls();
            ChildControlsCreated = true;

            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }
        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.csv:
                    MaestroEdit1.ExportarCSV();
                    break;
                case KeytiaExportFormat.xlsx:
                    MaestroEdit1.ExportarExcel();
                    break;
                case KeytiaExportFormat.pdf:
                    MaestroEdit1.ExportarPDF();
                    break;
                case KeytiaExportFormat.docx:
                    MaestroEdit1.ExportarWord();
                    break;
            }
        }
        protected override void InitLanguage()
        {
            base.InitLanguage();
            MaestroEdit1.InitLanguage();
            lblTitle.Text = HttpUtility.HtmlEncode(Globals.GetLangItem("TituloMaestros"));
        }
    }
}
