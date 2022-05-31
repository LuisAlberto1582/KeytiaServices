﻿using System;
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
    public partial class SYOUsuarioUpd : System.Web.UI.Page
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!IsPostBack)
            {
                if (Request.QueryString["iCodCatalogo"] != null)
                {
                    int iCodCatalogo = int.Parse(Request.QueryString["iCodCatalogo"]);
                    cargarPerfiles();
                    cargarEmpresa();
                    CargarDatosUsuario(iCodCatalogo);
                    
                }
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["iCodCatalogo"] != null)
            {
                int iCodCatalogo = int.Parse(Request.QueryString["iCodCatalogo"]);
                ActualizarUsuario(iCodCatalogo);
            }
        }
        #endregion
        #region Metodos
        public void ActualizarUsuario(int iCodCatalogo)
        {
            string clave = lblUri.Text;
            string descripcion = lblDescripcion.Text;
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

                    string cadenaConexionKeytia = ConfigurationManager.AppSettings["appConnectionString"].ToString();
                    DSODataAccess.Execute("EXEC SYOActualizarUsuario '" + clave + "','" + descripcion + "','" + nombre + "','" + npaterno + "','" + nmaterno + "'," + checkbox + ",'" + encriptado + "'," + perfil + "," + empresa + "," + iCodCatalogo, cadenaConexionKeytia);
                    ClientScript.RegisterStartupScript(this.Page.GetType(), "alerta", "alert('El usuario fue actualizado exitosamente.')", true);
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
       
        public void CargarDatosUsuario(int iCodCatalogo)
        {
            DataTable dtResultado = DSODataAccess.Execute("EXEC SYOEmpleadosPorCatalogo " + iCodCatalogo);
            DataTable dtResultadoUri = DSODataAccess.Execute("EXEC SYOUriPorEmpleado " + iCodCatalogo);
            DataTable dtResultadoUsuario = DSODataAccess.Execute("EXEC SYOUsuarioPorEmpleado " + iCodCatalogo);

            string result = "";
            string contraseña = "";
            if (dtResultadoUri.Rows.Count != 0)
            {
                if (dtResultadoUri.Rows[0]["SYOBanderasUri"].ToString() == "1")
                {
                    result = "1";
                }
                else if (dtResultadoUri.Rows[0]["SYOBanderasUri"].ToString() == "2")
                {
                    result = "2";
                }
                else if (dtResultadoUri.Rows[0]["SYOBanderasUri"].ToString() == "4")
                {
                    result = "4";
                }
                lblUri.Text = dtResultadoUri.Rows[0]["vchDescripcion"].ToString();
            }

            CheckBoxList1.SelectedValue = result;
           

            

            if (dtResultado.Rows.Count != 0)
            {
                txtNombre.Text = dtResultado.Rows[0]["Nombre"].ToString();
                txtApellidoPaterno.Text = dtResultado.Rows[0]["Paterno"].ToString();
                txtApellidoMaterno.Text = dtResultado.Rows[0]["Materno"].ToString();
            }
            if (dtResultadoUsuario.Rows.Count != 0)
            {
                lblDescripcion.Text = dtResultadoUsuario.Rows[0]["vchDescripcion"].ToString();
                contraseña = Util.Decrypt(dtResultadoUsuario.Rows[0]["Password"].ToString());
                ddlPerfil.Text = dtResultadoUsuario.Rows[0]["Perfil"].ToString();
                ddlEmpresa.Text = dtResultadoUsuario.Rows[0]["Empre"].ToString();
            }

            txtCon.Text = contraseña;
            txtCon2.Text = contraseña;

        }
       

        #endregion
    }
}
