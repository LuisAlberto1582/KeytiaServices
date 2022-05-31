using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.SYO.admin.Cliente
{
    public partial class SYOCliente : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!IsPostBack)
            {
                CargarClientes();

            }
        }
        protected void grdClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Modificar")
            {
                Response.Redirect("~/UserInterface/SYO/admin/Cliente/SYOClienteUpd.aspx?iCodCatalogo=" + e.CommandArgument.ToString());
            }
            
        }
        protected void grdClientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdClientes.PageIndex = e.NewPageIndex;
            CargarClientes();
        }
        #endregion
        #region Metodos
        public void CargarClientes()
        {

            DataTable dtResultado = DSODataAccess.Execute("EXEC SYOVistaClientes");
            grdClientes.DataSource = dtResultado;
            grdClientes.DataBind();


        }
        #endregion
    }
}
