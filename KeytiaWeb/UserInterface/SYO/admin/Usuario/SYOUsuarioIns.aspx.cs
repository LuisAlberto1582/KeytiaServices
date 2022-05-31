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
    public partial class SYOUsuarioIns : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!IsPostBack)
            {
                cargarPerfiles();
                cargarEmpresa();
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            agregarUsuario();
        }
        #endregion
        #region Metodos
        public void agregarUsuario()
        {
            string clave = txtUri.Text;
            string descripcion = txtDescripcion.Text;
            string nombre = txtNombre.Text;
            string npaterno = txtApellidoPaterno.Text;
            string nmaterno = txtApellidoMaterno.Text;

            string password = txtCon.Text;
            int perfil = int.Parse(ddlPerfil.SelectedValue);
            int empresa = int.Parse(ddlEmpresa.SelectedValue);
            int checkbox = int.Parse(CheckBoxList1.SelectedValue.ToString());

            string encriptado = Util.Encrypt(password);


            try
            {
                if (txtCon.Text == txtCon2.Text)
                {
                    DataTable dtResultado = DSODataAccess.Execute("EXEC SYOValidarUsuario");

                    if (dtResultado.Rows.Count == 0)
                    {
                        string cadenaConexionKeytia = ConfigurationManager.AppSettings["appConnectionString"].ToString();
                        DSODataAccess.Execute("EXEC SYOInsertarUsuario '" + clave + "','" + descripcion + "','" + nombre + "','" + npaterno + "','" + nmaterno + "'," + checkbox + ",'" + encriptado + "'," + perfil + "," + empresa, cadenaConexionKeytia);
                        ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('El usuario fue agregado exitosamente.')", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('El Uri ya existe')", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('Las contraseñas no coinciden')", true);

                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('" + ex.Message + "')", true);
            }

        }
        public void cargarPerfiles()
        {
            DataTable dtResultado = DSODataAccess.Execute("EXEC PerfilUsuario");
            ddlPerfil.DataSource = dtResultado;

            ddlPerfil.DataTextField = "vchDescripcion";
            ddlPerfil.DataValueField = "iCodCatalogo";
            ddlPerfil.DataBind();

            ddlPerfil.Items.Insert(0, new ListItem("---Seleccion Perfil---", "0"));
        }
        public void cargarEmpresa()
        {
            DataTable dtResultado = DSODataAccess.Execute("EXEC SYOObtieneEmpresas");
            ddlEmpresa.DataSource = dtResultado;

            ddlEmpresa.DataTextField = "vchDescripcion";
            ddlEmpresa.DataValueField = "iCodCatalogo";
            ddlEmpresa.DataBind();

            ddlEmpresa.Items.Insert(0, new ListItem("---Seleccion Empresa---", "0"));
        }
      
        #endregion
    }
}
