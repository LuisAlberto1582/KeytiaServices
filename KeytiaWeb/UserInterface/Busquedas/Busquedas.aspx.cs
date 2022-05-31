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
using System.Web.UI;

namespace KeytiaWeb.UserInterface
{
    public partial class Busquedas : KeytiaPage
    {
        protected DSOBusqueda pDSOBusqueda;
        protected KDBAccess pKDB = new KDBAccess();

        public Busquedas()
        {
            Init += new EventHandler(Busquedas_Init);
            Load += new EventHandler(Busquedas_Load);
        }

        void Busquedas_Load(object sender, EventArgs e)
        {
            pDSOBusqueda.SetSearch();

            //NZ
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
        }

        protected void Busquedas_Init(object sender, EventArgs e)
        {
            pDSOBusqueda = new DSOBusqueda();
            pDSOBusqueda.ID = "Busquedas1";
            pDSOBusqueda.lblTitle = lblTitle;
            pDSOBusqueda.ParentContainer = Content;
            Content.Controls.Add(pDSOBusqueda);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            pDSOBusqueda.CreateControls();
        }

        protected override void Page_Export(KeytiaExportFormat lkeFormat)
        {
            pDSOBusqueda.Export(lkeFormat);
        }

        protected override void InitLanguage()
        {
            pDSOBusqueda.InitLanguage();
        }

        protected override bool OptionIsValid()
        {
            return true;
        }
    }
}
