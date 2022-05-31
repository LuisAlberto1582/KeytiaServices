using System;
using System.Web;
using System.Web.UI;
using KeytiaServiceBL;
using System.Data;

namespace KeytiaWeb.UserInterface.RepProsa
{
    public partial class ConsultaCargaLineaEmpleUnidadNegCenCos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ObtenerCargas();
        }

        private void ObtenerCargas()
        {
            string strQuery = $"SELECT * FROM prosa.[visHistoricos('Cargas','Carga Relación Empleado Unidad de negocios','Español')]";
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
            HttpContext.Current.Response.Redirect("~/UserInterface/RepProsa/ConfigCargaLineaEmpleUnidadNegCenCos.aspx", false);
        }
    }
}