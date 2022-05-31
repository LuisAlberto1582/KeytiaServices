using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardGestionActivos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            CargarDashboard();
        }

        private void CargarDashboard()
        {
            string strQuery = "";
            DataTable dtRes = DSODataAccess.Execute(strQuery);

            Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(DTIChartsAndControls.GridView("RepTotalRecursos", dtRes, true, "Totales", new string[] { "", "{0:0,0}" }, 2, false), "ReporteRecursoUltMes", "Activos por responsable", "", false));
        }
    }
}