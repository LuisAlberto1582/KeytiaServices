using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Configuration;

namespace KeytiaWeb.UserInterface.SYO.admin.Usuario
{
    public partial class SYOUsuario : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!IsPostBack)
            {
                CargarEmpleados();

            }
        }
        protected void grdEmpleados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Modificar")
            {
                Response.Redirect("~/UserInterface/SYO/admin/Usuario/SYOUsuarioUpd.aspx?iCodCatalogo=" + e.CommandArgument.ToString());
            }

            else if (e.CommandName == "Eliminar")
            {
                Response.Redirect("~/UserInterface/SYO/admin/Usuario/SYOUsuarioDel.aspx?iCodCatalogo=" + e.CommandArgument.ToString());
            }
        }
        protected void grdEmpleados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdEmpleados.PageIndex = e.NewPageIndex;
            CargarEmpleados();
        }
        #endregion
        #region Metodos
        public void CargarEmpleados()
        {

            DataTable dtResultado = DSODataAccess.Execute("EXEC SYOObtenerUsuarios");
            grdEmpleados.DataSource = dtResultado;
            grdEmpleados.DataBind();
        }
        #endregion
    }
}
