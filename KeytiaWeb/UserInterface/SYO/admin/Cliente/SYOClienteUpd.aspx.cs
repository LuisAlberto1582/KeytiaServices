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
    public partial class SYOClienteUpd : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["iCodCatalogo"] != null)
                {
                    int iCodCatalogo = int.Parse(Request.QueryString["iCodCatalogo"]);
                    CargarDatosCliente(iCodCatalogo);
                }
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["iCodCatalogo"] != null)
            {
                int iCodCatalogo = int.Parse(Request.QueryString["iCodCatalogo"]);
                ActualizarCliente(iCodCatalogo);
            }
        }
        #endregion
        #region Metodos
        public void ActualizarCliente(int iCodCatalogo)
        {
            string clave = lblClave.Text;
            string descripcion = lblDescripcion.Text;
            string logo = txtLogo.Text;
            string logoExportacion = txtLogo.Text;

            try
            {

                DSODataAccess.Execute("EXEC SYOActualizarCliente '" + clave + "','" + descripcion + "','" + logo + "','" + logoExportacion + "'," + iCodCatalogo);
                ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('El cliente fue actulizado exitosamente.')", true);


            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('" + ex.Message + "')", true);
            }

        }
        public void CargarDatosCliente(int iCodCatalogo)
        {
            DataTable dtResultado = DSODataAccess.Execute("EXEC SYOClientesPorCatalogo " + iCodCatalogo);


            if(dtResultado.Rows.Count != 0)
            {

              lblClave.Text = dtResultado.Rows[0]["vchCodigo"].ToString();
              lblDescripcion.Text = dtResultado.Rows[0]["vchDescripcion"].ToString();
              txtLogo.Text = dtResultado.Rows[0]["Logo"].ToString();
              txtLogoExportacion.Text = dtResultado.Rows[0]["LogoExportacion"].ToString();
            }


        }
        #endregion
       
    }
}
