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
    public partial class SYOClienteIns : System.Web.UI.Page
    {
        #region Evento
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            agregarCliente();
        }
        #endregion
        #region
        public void agregarCliente()
        {
            string clave = txtClave.Text;
            string descripcion = txtDescripcion.Text;
            string logo = txtLogo.Text;
            string logoExportacion = txtLogo.Text;

            try
            {

                DSODataAccess.Execute("EXEC SYOInsertarCliente '" + clave + "','" + descripcion + "','" + logo + "','" + logoExportacion + "'");
                ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('El cliente fue agregado exitosamente.')", true);


            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('" + ex.Message + "')", true);
            }

        }
        #endregion
    }
}
