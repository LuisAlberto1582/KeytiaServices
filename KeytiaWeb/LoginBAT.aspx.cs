using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb
{
    public partial class LoginBAT : Login
    {
        public LoginBAT()
        {
            pld = "lp=LoginBAT.aspx&ls=79482";
        }

        protected override void BtnIngresar_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                lblMensaje.Text = GetLangItem("MsgWeb", "Mensajes Web", "UCReq"); //"El Usuario y Contraseña son Requeridos";
                lblMensaje.Visible = true;
                return;
            }

            if (lnvLD != null && lnvLD["ls"] != null)
            {
                oUsuario.vchCodUsuarioDB = lnvLD["ls"];
                oUsuario.vchPwdUsuario = txtPassword.Text;
            }

            if (oUsuario.Consultar() && oUsuario.Row["{Password}"].Equals(KeytiaServiceBL.Util.Encrypt(txtPassword.Text)))
            {
                lblMensaje.Visible = false;
                IniciaPaginaUsuario();
            }
            else
            {
                lblMensaje.Text = GetLangItem("MsgWeb", "Mensajes Web", "UCInc"); //"El Usuario y/o Contraseña no son correctos";
                lblMensaje.Visible = true;
            }
        }

        public override string GetLangItem(string lsEntidad, string lsMaestro, string lsCodigo)
        {
            if (lsCodigo == "UCInc") lsCodigo = "UCIncBAT";
            else if (lsCodigo == "UCReq") lsCodigo = "UCReqBAT";

            return base.GetLangItem(lsEntidad, lsMaestro, lsCodigo);
        }
    }
}
