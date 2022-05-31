/*
 * Nombre:		    HDL
 * Fecha:		    20130213
 * Descripción:	    Página de Sesion Expirada
 * Modificación:	
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Collections;
using System.Collections.Specialized;
namespace KeytiaWeb
{
    public partial class SessionExpirada : System.Web.UI.Page
    {
        protected Usuarios oUsuario = new Usuarios();
        protected NameValueCollection lnvLD = null;
        protected string pld = "";

        public virtual void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                KeytiaServiceBL.DSODataContext.SetContext();
                this.Title = GetLangItem("MsgWeb", "Mensajes Web", "tlSesExp");
                lblTituloSesExp.Text = GetLangItem("MsgWeb", "Mensajes Web", "tlSesExp");
                lblmsgSesExp01.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgSesExp01").Replace(";",";<br/>");
                btnAceptar.Text = GetLangItem("MsgWeb", "Mensajes Web", "btnAceptar");
                btnCancelar.Text = GetLangItem("MsgWeb", "Mensajes Web", "btnCancelar");
                lblmsgSesExp02.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgSesExp02");
                lblmsgSesExp03.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgSesExp03");
                lblmsgSesExp04.Text = GetLangItem("MsgWeb", "Mensajes Web", "msgCopyright").Replace("{0}", DateTime.Now.Year.ToString());
            }


        }

        //Funcion para Obtener la descripcion en el idioma actual, del mensaje web solicitado.
        public virtual string GetLangItem(string lsEntidad, string lsMaestro, string lsCodigo)
        {
            return Globals.GetLangItem(lsEntidad, lsMaestro, lsCodigo);
        }

        public virtual void btnAceptar_Click(object sender, EventArgs e)
        {
            //this.btnAceptar.Text = "Presionadoo";
			Response.Redirect("http://10.103.133.26/access");
        }

        public virtual void btnCancelar_Click(object sender, EventArgs e)
        {
            //this.btnCancelar.Text = "Presionadoo";
            //HttpContext.Current.Response.Redirect
            Response.Redirect("http://gentenextel:8080/");
        }

        protected virtual void valUsuarioRed()
        {
            //oUsuario.vchCodUsuario = Request.ServerVariables["LOGON_USER"];
            if (!String.IsNullOrEmpty(Request.QueryString["usr"]) && !String.IsNullOrEmpty(Request.QueryString["usrdb"]))
            {
                KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
                System.Data.DataRow ldrUsuarioDB = null;

                string lsEsquema = KeytiaServiceBL.Util.Decrypt(Request.QueryString["usrdb"]);

                ldrUsuarioDB = (kdb.GetHisRegByEnt("UsuarDB", "Usuarios DB", new string[] { "iCodCatalogo" }, "{Esquema} = '" + lsEsquema + "'")).Rows[0];

                oUsuario.vchCodUsuario = KeytiaServiceBL.Util.Decrypt(Request.QueryString["usr"]);
                oUsuario.vchCodUsuarioDB = ldrUsuarioDB["iCodCatalogo"].ToString();
            }
        }

    }
}