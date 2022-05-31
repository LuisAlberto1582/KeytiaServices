using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb
{
    public partial class LoginNextel : Login
    {
        

        public LoginNextel()
        {
            pld = "lp=LoginNextel.aspx&ls=79124";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
                   
            if (!Page.IsPostBack)
            {
                if (Session != null && Session["HomePage"] != null)
                {
                    Response.Redirect((string)Session["HomePage"]);
                }

                KeytiaServiceBL.DSODataContext.SetContext();

                Page.Title = GetLangItem("MsgWeb", "Mensajes Web", "msgTitLoginNextel");
                lblIntDat.Text = GetLangItem("MsgWeb", "Mensajes Web", "IntDatUsr");
                //lblUsuario.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgLoginUsuar");
                //lblPassword.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgLoginPass");
                BtnIngresar.Text = GetLangItem("MsgWeb", "Mensajes Web", "BtnIngresar");
                lblmsgSesExp02.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgSesExp02");
                lblmsgSesExp03.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgSesExp03");
                //hplDefault.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgLogNextDirCor");
                /*RZ.20130718 Se agrega re-direccionamiento a pagina que manda al directorio corporativo*/
                //hplDefault.NavigateUrl = "/directorionextel/Default.aspx";
                lblmsgSesExp04.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgCopyright").Replace("{0}", DateTime.Now.Year.ToString());

                valUsuarioRed();

                if (Request.QueryString["err"] != null)
                {
                    lblMensaje.Text = GetLangItem("MsgWeb", "Mensajes Web", Request.QueryString["err"]);
                }
                
                /***++**HD*/
                if (!Session.IsNewSession)
                {
                    //Session.Clear();
                    //Response.Redirect("~/SesionExpirada.aspx");
                    Response.Redirect("http://10.103.133.26/NextelAccess");
                }
                


                AlternateLogin();
            }
           

        }

        
    }

}
