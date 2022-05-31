using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Collections;

namespace KeytiaWeb
{
    public partial class Login2 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["iCodUsuario"] = null;
            Label4.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "IntDatUsr");
            Label2.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NomUsr");
            Label3.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "Pwd");
            Button1.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "BtnIngresar");
            LinkButton1.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "BtnOlvPwd");
            lblMensaje.Visible = false;
            lblMensaje.Text = "";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Usuarios oUsuario = new Usuarios();

            if (TextBox1.Text == "" || TextBox2.Text == "")
            {
                lblMensaje.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCReq"); //"El Usuario y Contraseña son Requeridos";
                lblMensaje.Visible = true;
                return;
            }

            oUsuario.vchCodUsuario = TextBox1.Text;
            oUsuario.vchPwdUsuario = TextBox2.Text;

            if (oUsuario.Consultar() && oUsuario.Row[1].Equals(TextBox2.Text))
            {
                Session["iCodUsuario"] = oUsuario.Row[0];
                Session["vchCodUsuario"] = TextBox1.Text;
                lblMensaje.Visible = false;

                Globals.Init();

                if (Request.QueryString["ReturnUrl"] != null)
                    FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
                else
                {
                    FormsAuthentication.SetAuthCookie(TextBox1.Text, false);
                    Response.Redirect((string)Session["HomePage"]);
                }
            }
            else
            {
                lblMensaje.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCInc"); //"El Usuario y/o Contraseña no son correctos";
                lblMensaje.Visible = true;
            }
        }
    }
}