/*
Nombre:		    JCMS
Fecha:		    2011-05-22
Descripción:	Página para acutalizar la lista de entidades.
Modificación:	
*/
using System;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web;
using KeytiaWeb;

namespace KeytiaWeb.UserInterface
{
    public partial class Catalogos : KeytiaPage
    {
        private CatalogEdit Catalogo;
        public Catalogos()
        {
            Init += new EventHandler(Catalogos_Init);            
        }

        void Catalogos_Init(object sender, EventArgs e)
        {
            Catalogo = new CatalogEdit();
            Content.Controls.Add(Catalogo);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            switch (lkeFormat)
            {
                case KeytiaExportFormat.xlsx:
                    Catalogo.ExportXLS();
                    break;
                case KeytiaExportFormat.docx:
                    Catalogo.ExportDOC();
                    break;
                case KeytiaExportFormat.pdf:
                    Catalogo.ExportPDF();
                    break;
                case KeytiaExportFormat.csv:
                    Catalogo.ExportCSV();
                    break;
            }
        }

        protected override void InitLanguage()
        {
            base.InitLanguage();
            Catalogo.InitLanguage();
            lblTitle.Text = Globals.GetMsgWeb(false, "TituloCatalogos");
        }
    }
}
