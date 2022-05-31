using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.RepTernium
{
    public partial class RepGerencialSolicitudes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ObtenerSolicitudes();
        }

        private void ObtenerSolicitudes()
        {
            //string strQuery = "select * from Ternium.[vishistoricos('ProcesoAutomatico','Ternium reportes gerenciales','Español')]";
            string strQuery = "exec TerniumReporteGerencialObtieneListado";
            DataTable res = DSODataAccess.Execute(strQuery);
            if (res.Rows.Count > 0)
            {
                rowGrv.Visible = true;
                InfoPanelSucces.Visible = false;
                grvListadoCargas.DataSource = res;
                grvListadoCargas.DataBind();
            }
            else
            {
                lblMensajeSuccess.Text = "No existen Cargas Actualmente";
                InfoPanelSucces.Visible = true;
                rowGrv.Visible = false;
            }
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {

        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/RepTernium/ReportesGerencialesConf.aspx", false);
        }
    }
}