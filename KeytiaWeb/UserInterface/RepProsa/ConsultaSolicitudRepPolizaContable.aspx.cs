using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using KeytiaServiceBL;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepProsa
{
    public partial class ConsultaSolicitudRepPolizaContable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ObtenerCargas();
        }

        private void ObtenerCargas() {
            string strQuery = $"select * from Prosa.[vishistoricos('procesoautomatico','Prosa reporte póliza contable tel móvil','español')] order by EstCargaDesc, convert(datetime, vchCodigo) desc";
            DataTable res = DSODataAccess.Execute(strQuery);
            if (res.Rows.Count > 0) {
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

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/RepProsa/ConfigSolicitudRepPolizaContable.aspx", false);
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {

        }
    }
}