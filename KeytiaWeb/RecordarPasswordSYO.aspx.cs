/*
 * Nombre:		    JMR
 * Fecha:		    20120716
 * Descripción:	    Clase para recordar el password al usuario SeeYouOn
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

namespace KeytiaWeb
{
    public partial class RecordarPasswordSYO : System.Web.UI.Page
    {
        protected string psTempPath;
        protected string psMailRemitente = "";
        protected string psNomRemitente = "";
        protected string psLogoClientePath = "";
        protected string psLogoKeytiaPath = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblRecPsdUsr.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "RecPsdUsr");
            lblUsuario.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NomUsr");
            lblEmail.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "email");
            btnAceptar.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "btnAceptar");
            btnCancelar.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "btnCancelar");

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FormsAuthentication.RedirectFromLoginPage(txtUsuario.Text, false);
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            Usuarios oUsuario = new Usuarios();
            //TripleDESWrapper DesPSW = new TripleDESWrapper();
            int liCodRegistro;
            int liCodUsuarioDB;

            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());

            if (txtUsuario.Text == "" || txtEmail.Text == "")
            {
                lblMensaje.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCtaReq"); //"El Usuario y Cuenta son Requeridos";
                lblMensaje.Visible = true;
                return;
            }

            oUsuario.vchCodUsuario = txtUsuario.Text;
            oUsuario.vchEmail = txtEmail.Text;

            if (oUsuario.Consultar())
            {
                lblMensaje.Visible = false;

                liCodRegistro = (int)oUsuario.Row["iCodRegistro"];
                liCodUsuarioDB = (int)oUsuario.Row["{UsuarDB}"];

                // Se enviara el correo de su password a la cuenta de correo proporcionada
                EnviarCorreo(liCodRegistro, liCodUsuarioDB);
                FormsAuthentication.RedirectFromLoginPage(txtUsuario.Text, false);

            }
            else
            {
                lblMensaje.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCtaInc"); //"El Usuario y/o Cuenta no son correctos";
                lblMensaje.Visible = true;
            }

        }

        private void EnviarCorreo(int liCodRegistro, int liCodUsuarios)
        {
            try
            {
                //Inicializa el contexto de datos
                KeytiaServiceBL.DSODataContext.SetContext(liCodUsuarios);


                KeytiaServiceBL.KDBAccess pKDB = new KeytiaServiceBL.KDBAccess();
                System.Data.DataTable ldtUsuario = pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodRegistro = " + liCodRegistro);

                int liCodEmpresa = (int)KeytiaServiceBL.Util.IsDBNull(ldtUsuario.Rows[0]["{Empre}"], 0);

                string lsEmail = (string)KeytiaServiceBL.Util.IsDBNull(ldtUsuario.Rows[0]["{Email}"], "");
                string lvchDescripcion = (string)KeytiaServiceBL.Util.IsDBNull(ldtUsuario.Rows[0]["vchDescripcion"], "");
                string lsPasswordEnc = (string)KeytiaServiceBL.Util.IsDBNull(ldtUsuario.Rows[0]["{Password}"], "");
                string lsPassword = "";

                if (lsPasswordEnc != "")
                {
                    lsPassword = KeytiaServiceBL.Util.Decrypt(lsPasswordEnc);
                }

                string lsErrMailAsunto = Globals.GetMsgWeb("RecMailAsuntoPws");
                string lsMensaje = Globals.GetMsgWeb("RecMailMensajePws", lsEmail, lvchDescripcion, lsPassword);

                if (lsErrMailAsunto.StartsWith("#undefined-"))
                {
                    lsErrMailAsunto = "Solicitud del envío para recordar la contraseña";
                }

                if (lsMensaje.StartsWith("#undefined-"))
                {
                    lsMensaje = "La solicitud del envío para recordar la contraseña\n";
                    lsMensaje += "Para: " + lsEmail + "\n";
                    lsMensaje += "Usuario: " + lvchDescripcion + "\n";
                    lsMensaje += "Password: " + lsPassword + "\n";
                }

                KeytiaServiceBL.WordAccess loWord = new KeytiaServiceBL.WordAccess();
                loWord.Abrir(true);
                EncabezadoCorreoEmpresa(liCodEmpresa);

                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.InsertarImagen(psLogoClientePath);
                }
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.InsertarImagen(psLogoKeytiaPath);
                }

                loWord.NuevoParrafo();
                loWord.InsertarTexto(lsMensaje);

                string lsFileName = getFileName(liCodEmpresa);
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
                loWord.Cerrar();
                loWord.Salir();
                loWord = null;

                KeytiaServiceBL.MailAccess loMail = new KeytiaServiceBL.MailAccess();
                loMail.NotificarSiHayError = false;
                loMail.IsHtml = true;
                loMail.De = getRemitente();
                loMail.Asunto = lsErrMailAsunto;
                loMail.AgregarWord(lsFileName);
                loMail.Para.Add(lsEmail);
                loMail.EnviarAsincrono(liCodEmpresa);
            }
            finally
            {
                KeytiaServiceBL.DSODataContext.SetContext();
            }
        }

        private string getFileName(int liCodEmpresa)
        {
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            System.Data.DataTable ldtEmpresa = kdb.GetHisRegByEnt("Empre", "Empresas", "iCodCatalogo = " + liCodEmpresa);

            System.IO.Directory.CreateDirectory(psTempPath);
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, ldtEmpresa.Rows[0]["vchCodigo"].ToString().Trim() + ".docx"));
        }

        public void EncabezadoCorreoEmpresa(int liCodEmpresa)
        {
            System.Data.DataRow ldrCte = getCliente(liCodEmpresa);
            if (ldrCte != null)
            {
                psMailRemitente = KeytiaServiceBL.Util.IsDBNull(ldrCte["{CtaDe}"], "").ToString();
                psNomRemitente = KeytiaServiceBL.Util.IsDBNull(ldrCte["{NomRemitente}"], "").ToString();
                psLogoClientePath = KeytiaServiceBL.Util.IsDBNull(ldrCte["{Logo}"], "").ToString().Replace("/", "\\");
                psLogoClientePath = psLogoClientePath.Replace("~", HttpContext.Current.Server.MapPath("~"));
                psLogoKeytiaPath = KeytiaServiceBL.Util.IsDBNull(ldrCte["{StyleSheet}"], "").ToString().Replace("/", "\\");
                psLogoKeytiaPath = psLogoKeytiaPath.Replace("~", HttpContext.Current.Server.MapPath("~"));
                psLogoKeytiaPath = System.IO.Path.Combine(psLogoKeytiaPath, @"images\KeytiaHeader.png");
            }
        }

        protected System.Net.Mail.MailAddress getRemitente()
        {
            if (string.IsNullOrEmpty(psMailRemitente))
            {
                return new System.Net.Mail.MailAddress(KeytiaServiceBL.Util.AppSettings("appeMailID"));
            }
            else if (string.IsNullOrEmpty(psNomRemitente))
            {
                return new System.Net.Mail.MailAddress(psMailRemitente);
            }
            else
            {
                return new System.Net.Mail.MailAddress(psMailRemitente, psNomRemitente);
            }
        }

        public static System.Data.DataRow getCliente(int liCodEmpresa)
        {
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            System.Data.DataRow ldr = null;
            try
            {
                System.Data.DataTable ldt = kdb.GetHisRegByEnt("Empre", "Empresas",
                    new string[] { "{Client}" },
                    "iCodCatalogo = " + liCodEmpresa.ToString());
                if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["{Client}"] is DBNull))
                {
                    ldt = kdb.GetHisRegByEnt("Client", "Clientes",
                        "iCodCatalogo = " + ldt.Rows[0]["{Client}"].ToString());
                    if (ldt != null && ldt.Rows.Count > 0)
                    {
                        ldr = ldt.Rows[0];
                    }
                }
            }
            catch (Exception ex) { }
            return ldr;
        }
    }
}
