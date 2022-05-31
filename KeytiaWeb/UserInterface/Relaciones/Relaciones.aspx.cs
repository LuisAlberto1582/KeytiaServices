/*
 * Nombre:		    DMM
 * Fecha:		    20110610
 * Descripción:	    Página de configuración de Relaciones
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

namespace KeytiaWeb.UserInterface
{
    public partial class Relaciones : KeytiaPage
    {
        RelacionEdit RelacionEdit1;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            RelacionEdit1 = new RelacionEdit();
            RelacionEdit1.ID = "RelacionEdit1";
            Content.Controls.Add(RelacionEdit1);
            RelacionEdit1.CreateControls();
            ChildControlsCreated = true;

            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }
        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.csv:
                    RelacionEdit1.ExportarCSV();
                    break;
                case KeytiaExportFormat.xlsx:
                    RelacionEdit1.ExportarExcel();
                    break;
                case KeytiaExportFormat.pdf:
                    RelacionEdit1.ExportarPDF();
                    break;
                case KeytiaExportFormat.docx:
                    RelacionEdit1.ExportarWord();
                    break;
            }
        }
        protected override void InitLanguage()
        {
            base.InitLanguage();
            RelacionEdit1.InitLanguage();
            lblTitle.Text = HttpUtility.HtmlEncode(Globals.GetLangItem("TituloRelaciones"));
        }

    }
}
