using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using KeytiaServiceBL;

namespace KeytiaWeb
{
    public partial class LoginWFA : System.Web.UI.Page
    {
        protected Usuarios oUsuario = new Usuarios();
        protected NameValueCollection lnvLD = null;
        protected string pld = "";

        protected string urlRedirect = "";


        //Campos en la ventana de Recuperación de contraseña.
        protected string psTempPath;
        protected string psMailRemitente = "";
        protected string psNomRemitente = "";
        protected string psLogoClientePath = "";
        protected string psLogoKeytiaPath = "";


        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                
                if (Session != null && Session["HomePage"] != null && string.IsNullOrEmpty(Request.QueryString["rd"]))
                {
                    Response.Redirect((string)Session["HomePage"]);
                }

                KeytiaServiceBL.DSODataContext.SetContext();
                ObtieneEmpresa();
                lblIntDat.Text = GetLangItem("MsgWeb", "Mensajes Web", "IntDatUsr");
                BtnIngresar.Text = GetLangItem("MsgWeb", "Mensajes Web", "BtnIngresar");
                //hlOlvidoPassword.Text = GetLangItem("MsgWeb", "Mensajes Web", "BtnOlvPwd");
                valUsuarioRed();
            }

            if (Request.QueryString["err"] != null)
            {
                lblMensaje.Text = GetLangItem("MsgWeb", "Mensajes Web", Request.QueryString["err"]);
            }

            AlternateLogin();
        }

        protected virtual void AlternateLogin()
        {
            if (Request.QueryString["ld"] != null)
                lnvLD = HttpUtility.ParseQueryString(KeytiaServiceBL.Util.Decrypt(Request.QueryString["ld"]));
            else if (Request.Cookies["kld"] != null)
                lnvLD = HttpUtility.ParseQueryString(KeytiaServiceBL.Util.Decrypt(Request.Cookies["kld"].Value));
            else if (!string.IsNullOrEmpty(pld))
                lnvLD = HttpUtility.ParseQueryString(pld);

            if (lnvLD != null)
            {
                if (lnvLD["lp"] != null)
                    RedirectLogin(lnvLD["lp"]);

                SaveCookieLogin(lnvLD);
            }
        }

        protected virtual void RedirectLogin(string loginPage)
        {
            if (Server.MapPath(loginPage) != Request.PhysicalPath)
                Response.Redirect(loginPage + (Request.QueryString.Count > 0 ? "?" + Request.QueryString.ToString() : ""), true);
        }

        protected virtual void SaveCookieLogin(NameValueCollection lnvLD)
        {
            if (lnvLD != null && (lnvLD["lp"] != null || lnvLD["ls"] != null))
            {
                System.Text.StringBuilder lsbCookie = new System.Text.StringBuilder();
                lsbCookie.Append(lnvLD["lp"] != null ? (lsbCookie.Length > 0 ? "&" : "") + "lp=" + lnvLD["lp"] : "");
                lsbCookie.Append(lnvLD["ls"] != null ? (lsbCookie.Length > 0 ? "&" : "") + "ls=" + lnvLD["ls"] : "");

                HttpCookie loCookieLD = new HttpCookie("kld");
                loCookieLD.Value = KeytiaServiceBL.Util.Encrypt(lsbCookie.ToString());
                loCookieLD.Expires = DateTime.Today.AddYears(1);
                Response.Cookies.Add(loCookieLD);
            }
        }


        protected virtual void BtnIngresar_Click(object sender, EventArgs e)
        {
            /*Obtiene todos los empleados que coincidan con esa nomina*/
            string nomNomina = txtUsuario.Text.ToString();
            if (txtUsuario.Text == "")
            {
                lblMensaje.Text = "La Nomina Es Requerida"; //"El Usuario y Contraseña son Requeridos";
                pnlAlerta.Visible = true;
                return;
            }
            else
            {
                int sociedad = Convert.ToInt32(cboEmpresa.SelectedValue.Replace("Seleccione Una Empresa", "0"));
                if (sociedad > 0)
                {
                    int empleado = 0;
                    empleado = ValidaNominaUser(nomNomina, sociedad);
                    //int emple = Convert.ToInt32(cboEmpleado.SelectedValue);
                    if (empleado > 0)
                    {
                        ///*Obtiene el Usuario del Empleado*/
                        string usuario = ObtieneUsuario(empleado);
                        oUsuario.vchCodUsuario = usuario;
                        //oUsuario.vchPwdUsuario = txtPassword.Text;
                        if (oUsuario.Consultar())
                        {
                            /*Actualizar el Centro de costos del empleado*/
                            //int cenCos = Convert.ToInt32(cboCenCos.SelectedValue);
                            //ActualizaCenCosEmple(emple, cenCos);
                            pnlAlerta.Visible = false;
                            IniciaPaginaUsuario();
                        }
                        else
                        {
                            // AM 20131002. Se guarda un registro en la Bitacora intentos fallidos en el esquema Keytia.
                            InsertaAccesoFallido();
                            lblMensaje.Text = "El Usuario No Existe";//GetLangItem("MsgWeb", "Mensajes Web", "UCInc"); //"El Usuario y/o Contraseña no son correctos";
                            pnlAlerta.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        lblMensaje.Text = "¡Nomina Inexistente o Nomina y Empresa Incorreca!"; //"El Usuario y Contraseña son Requeridos";
                        pnlAlerta.Visible = true;
                        return;
                    }
                }
                else
                {
                    lblMensaje.Text = "¡Seleccione Una Empresa!"; //"El Usuario y Contraseña son Requeridos";
                    pnlAlerta.Visible = true;
                    return;
                }
            }
        }


        protected virtual void IniciaPaginaUsuario()
        {
            Session["iCodUsuario"] = oUsuario.Row["iCodCatalogo"];
            Session["vchCodUsuario"] = oUsuario.vchCodUsuario;
            Session["iCodUsuarioDB"] = oUsuario.Row["{UsuarDB}"];

            if (lnvLD != null && lnvLD["ls"] != null && (int)Session["iCodUsuarioDB"] != int.Parse(lnvLD["ls"]))
            {
                KeytiaServiceBL.Util.LogMessage("El usuario '" + Session["vchCodUsuario"] + "' no pertenece al esquema '" + lnvLD["ls"] + "'");
                Session.Clear();
                HttpContext.Current.Response.Redirect("~/LoginWFA.aspx?err=UCInc", true);
            }

            Globals.Init();

            ActualizarUltimoAcceso();

            FormsAuthentication.SetAuthCookie(oUsuario.vchCodUsuario, false);

            //20161210.RJ
            //Se agrega parámetro que servirá para determinar si se obtiene la información desde tablas fijas o no
            //se valida si el HomePage configurado en tabla, ya tiene parámetros o no
            //if (!string.IsNullOrEmpty(urlRedirect))
            //{
            //    Session["HomePage"] = urlRedirect;
            //}

            //var urlHomePage = ((string)Session["HomePage"]).IndexOf('?', 0) != -1 ? ((string)Session["HomePage"]) + "&isFT=1" : ((string)Session["HomePage"]) + "?isFT=1";  //is First Time
            //Response.Redirect(urlHomePage);

            var redirectFinal = "";

            //NZ: Prueba
            if (!string.IsNullOrEmpty(urlRedirect))
            {
                redirectFinal = urlRedirect;
            }
            else
            {
                redirectFinal = ((string)Session["HomePage"]).IndexOf('?', 0) != -1 ? ((string)Session["HomePage"]) + "&isFT=1" : ((string)Session["HomePage"]) + "?isFT=1";  //is First Time
            }
           
            Response.Redirect(redirectFinal);
        }

        protected virtual void valUsuarioRed()
        {
            string lsUsername = string.Empty;
            string lsEsquema = string.Empty;

            string usr = string.Empty;
            string usrDB = string.Empty;
            bool isPT = false;


            if (!string.IsNullOrEmpty(Request.QueryString["usr"]))
            {
                usr = Request.QueryString["usr"];
            }
            else if (!string.IsNullOrEmpty(Request.Form["frmusr"]))
            {
                usr = Request.Form["frmusr"];
            }

            //Independientemente de que se encuentre usr, si se encuentra el parámetro usrpt
            //entonces se asignará ese valor a la variable usr
            if (!string.IsNullOrEmpty(Request.QueryString["usrpt"]))
            {
                usr = Request.QueryString["usrpt"];
                isPT = true;
            }


            if (!string.IsNullOrEmpty(Request.QueryString["usrdb"]))
            {
                usrDB = Request.QueryString["usrdb"];
            }
            else if (!string.IsNullOrEmpty(Request.Form["frmusrdb"]))
            {
                usrDB = Request.Form["frmusrdb"];
            }

            //Parámetros utilizados desde correos de Workflow
            string frmResp = Request.Form["frmr"];

            string frmRedirect = string.Empty;


            if (!string.IsNullOrEmpty(Request.QueryString["rd"]))
            {
                frmRedirect = Request.QueryString["rd"];
            }
            else if (!string.IsNullOrEmpty(Request.Form["frmrd"]))
            {
                frmRedirect = Request.Form["frmrd"];
            }

            //oUsuario.vchCodUsuario = Request.ServerVariables["LOGON_USER"];

            //Si la página recibe los parámetros usr o usrpt y tambien usrdb, se valida el acceso usando estos datos
            if ((!string.IsNullOrEmpty(usr))
                && !String.IsNullOrEmpty(usrDB))
            {
                KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
                System.Data.DataRow ldrUsuarioDB = null;


                //El usuarioDB siempre debe viajar encriptado
                lsEsquema = KeytiaServiceBL.Util.Decrypt(usrDB);

                ldrUsuarioDB = (kdb.GetHisRegByEnt("UsuarDB", "Usuarios DB",
                    new string[] { "iCodCatalogo" }, "{Esquema} = '" + lsEsquema + "'")).Rows[0];


                if (!isPT)
                {
                    //El username se recibe encriptado
                    lsUsername = KeytiaServiceBL.Util.Decrypt(usr);
                }
                else
                {
                    //El username se recibe como texto plano
                    lsUsername = oUsuario.DecryptSimpleAlgorithm(usr, 3); //userPlainText
                }


                if (!String.IsNullOrEmpty(frmRedirect))
                {
                    //Liga hacia la cual se generará el redirect
                    urlRedirect = KeytiaServiceBL.Util.Decrypt(frmRedirect);
                }

                oUsuario.vchCodUsuario = lsUsername;
                oUsuario.vchCodUsuarioDB = ldrUsuarioDB["iCodCatalogo"].ToString();
            }

            if (oUsuario.Consultar())
            {
                IniciaPaginaUsuario();
            }
        }

        protected virtual void ActualizarUltimoAcceso()
        {

            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();

            DateTime ldtUltAcc = DateTime.Now;

            string uName = Session["vchCodUsuario"].ToString().Length > 16 ? Session["vchCodUsuario"].ToString().Substring(0, 16) : Session["vchCodUsuario"].ToString();
            string vchCodigo = "'" + uName + " " + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            string vchDescripcion = "'" + Session["vchCodUsuario"] + " " + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            string Usuar = "'" + Session["iCodUsuario"].ToString() + "'";
            string FecAcc = "'" + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            string IP = "'" + Request.ServerVariables["remote_host"].ToString() + "'";
            string dtIniVigencia = "'" + DateTime.Today.ToString("yyyy-MM-dd") + "'";
            string dtFinVigencia = "'2079-01-01'";
            string esquema = KeytiaServiceBL.DSODataContext.Schema;
            try
            {

                //Inserta un historico con el catagolo encontrado
                //RJ.20140411.SE OMITE EL CAMPO ICODREGISTRO, PUES AL CAMBIAR LA TABLA DESTINO "REGSESION"
                //DICHO CAMPO SE CONFIGURÓ COMO IDENTITY
                //SE CAMBIA TAMBIEN EL USO DE LA VISTA, SE DEJA DIRECTAMENTE LA TABLA
                string query = "insert into " + esquema + ".RegSesion " +
                        "(iCodCatalogo, iCodMaestro, vchDescripcion, Usuar, " +
                        "BanderasSesion, FecAcc, IP, dtinivigencia, dtfinvigencia, icodusuario, dtfecultAct) " +
                        "values(NULL," +
                    //iCodCatalogo + ", 316, " + vchDescripcion + ", " + Usuar + ", NULL, " +
                        "316, " + vchDescripcion + ", " + Usuar + ", NULL, " +
                        FecAcc + ", " + IP + ", " + dtIniVigencia + ", " + dtFinVigencia + ", " +
                        "NULL, " + FecAcc + ")";

                if (KeytiaServiceBL.DSODataAccess.ExecuteNonQuery(query))
                {
                    //RZ.20140211 Se consulta el icodregistro del historico de RegSession y se almacena en variable de sesion
                    //RJ.20140411 Se cambia la tabla de la que se obtiene el icodRegistro, de Historicos a RegSesion
                    query = "select max(iCodRegistro) as iCodRegistro from " + esquema + ".RegSesion where Usuar = " + Usuar;
                    Session["iCodRegSesion"] = KeytiaServiceBL.Util.IsDBNull(KeytiaServiceBL.DSODataAccess.ExecuteScalar(query), int.MinValue);
                }
                else
                {
                    //En caso de no poder darse de alta el historico, la variable de sesion quedara como int.minvalue
                    Session["iCodRegSesion"] = int.MinValue;
                }

                if ((int)Session["iCodRegSesion"] == int.MinValue)
                {
                    throw new Exception();
                }



            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("No se pudo grabar el registro de la sesión del usuario '" + Session["vchCodUsuario"] + "'", ex);
            }

        }

        protected virtual void InsertaAccesoFallido()
        {
            Hashtable phtValoresCampos = new Hashtable();
            KeytiaServiceBL.KDBAccess kdb = new KeytiaServiceBL.KDBAccess();
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            DateTime ldtUltAcc = DateTime.Now;
            int iCodUsuarDB = Convert.ToInt32(KeytiaServiceBL.DSODataAccess.ExecuteScalar(
                            "select iCodCatalogo from [VisHistoricos('UsuarDB','Usuarios DB','Español')] \r" +
                            "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r" +
                            "and vchCodigo = 'Keytia' \r").ToString());

            phtValoresCampos.Add("vchDescripcion", Session["vchCodUsuario"] + " " + ldtUltAcc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            phtValoresCampos.Add("{UsuarRed}", txtUsuario.Text.ToString());
            phtValoresCampos.Add("{FecAcc}", ldtUltAcc);
            //phtValoresCampos.Add("{Password}", txtPassword.Text.ToString());
            phtValoresCampos.Add("{IP}", Request.ServerVariables["remote_host"].ToString());

            try
            {
                Session["iCodRegSesion"] = cargasCOM.InsertaRegistro(phtValoresCampos, "Pendientes", "Detall", "Bitacora ingresos fallidos", iCodUsuarDB);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("No se pudo grabar el registro de inicio de sesion fallido '" + Session["vchCodUsuario"] + "'", ex);
            }
        }

        public virtual string GetLangItem(string lsEntidad, string lsMaestro, string lsCodigo)
        {
            return Globals.GetLangItem(lsEntidad, lsMaestro, lsCodigo);
        }


        //Sección para la recuperacion de contraseña.

        protected void hlOlvidoPassword_Click(object sender, EventArgs e)
        {
            pnlLogin.Visible = false;
            pnlRecuperacion.Visible = true;

            lblRecPsdUsr.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "RecPsdUsr");
            //btnEnviar.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "btnAceptar");
            //btnVolver.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "btnCancelar");            
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            pnlLogin.Visible = true;
            pnlRecuperacion.Visible = false;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            Usuarios oUsuario = new Usuarios();
            //TripleDESWrapper DesPSW = new TripleDESWrapper();
            int liCodRegistro;
            int liCodUsuarioDB;

            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());

            if (txtUsuarioRecuperacion.Text == "" || txtEmailRecuperacion.Text == "")
            {
                lblMensajeAlerta.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCtaReq"); //"El Usuario y Cuenta son Requeridos";
                pnlAlertaRecuperacion.Visible = true;
                return;
            }

            oUsuario.vchCodUsuario = txtUsuarioRecuperacion.Text;
            oUsuario.vchEmail = txtEmailRecuperacion.Text;

            if (oUsuario.Consultar())
            {
                pnlAlertaRecuperacion.Visible = false;

                liCodRegistro = (int)oUsuario.Row["iCodRegistro"];
                liCodUsuarioDB = (int)oUsuario.Row["{UsuarDB}"];

                // Se enviara el correo de su password a la cuenta de correo proporcionada
                EnviarCorreo(liCodRegistro, liCodUsuarioDB);
                FormsAuthentication.RedirectFromLoginPage(txtUsuarioRecuperacion.Text, false);
            }
            else
            {
                lblMensajeAlerta.Text = Globals.GetLangItem("MsgWeb", "Mensajes Web", "UCtaInc"); //"El Usuario y/o Cuenta no son correctos";
                pnlAlertaRecuperacion.Visible = true;
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
                //loMail.EnviarAsincrono(liCodEmpresa);
                loMail.Enviar();
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
        private void ObtieneEmpresa()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @IcodCatalogo INT");
            query.AppendLine(" SELECT @IcodCatalogo = iCodCatalogo FROM K5Afirme.[vishistoricos('CenCos','Centro de costos','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND CenCos IS NULL");
            query.AppendLine(" SELECT iCodCatalogo,Descripcion FROM K5Afirme.[vishistoricos('CenCos','Centro de costos','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND CenCos = @IcodCatalogo AND Descripcion <> 'POR IDENTIFICAR' AND Descripcion <> 'DIR GRAL. SEGUROS'");
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                cboEmpresa.Items.Clear();
                cboEmpresa.DataSource = null;
                cboEmpresa.DataBind();
                cboEmpresa.Items.Insert(0, "Seleccione Una Empresa");
                cboEmpresa.SelectedIndex = 0;

                cboEmpresa.DataSource =dt;
                cboEmpresa.DataBind();
            }
        }
        private int ValidaNominaUser(string nomina,int sociedad)
        {
            int empleado = 0;
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, NomCompleto FROM K5Afirme.[vishistoricos('Emple','Empleados','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine("AND SUBSTRING(NominaA,4,10) = '"+nomina+"'");
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString());
            if(dt != null && dt.Rows.Count > 0)
            {
                int empresa = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int catalogo = Convert.ToInt32(dr["iCodCatalogo"]);
                    empresa = ObtieneSociedadEmple(catalogo);
                    if (empresa == sociedad)
                    {
                        empleado = catalogo;
                        break;
                    }
                }
            }
            else
            {
            }

            return empleado;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string nomina = txtUsuario.Text.ToString();

            if (txtUsuario.Text == "")
            {
                lblMensaje.Text = "La Nomina Es Requerida"; //"El Usuario y Contraseña son Requeridos";
                pnlAlerta.Visible = true;
                return;
            }
            else
            {
                lblMensaje.Text = ""; //"El Usuario y Contraseña son Requeridos";
                pnlAlerta.Visible = false;              
            }
        }
       
        protected void cboCenCos_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cenCos = Convert.ToInt32(cboEmpresa.SelectedValue.Replace("Seleccione Un Centro de Costos", "0"));
            if(cenCos > 0)
            {
                BtnIngresar.Enabled = true;
            }
        }
        private string ObtieneUsuario(int emple)
        {
            string usuario = "";
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT U.vchCodigo FROM K5Afirme.[vishistoricos('Emple','Empleados','Español')] AS E");
            query.AppendLine("JOIN K5Afirme.[VisHistoricos('Usuar','Usuarios','Español')]AS U");
            query.AppendLine("ON E.Usuar = U.iCodCatalogo");
            query.AppendLine("AND U.dtIniVigencia<> U.dtFinVigencia");
            query.AppendLine("AND U.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE E.dtIniVigencia<> E.dtFinVigencia");
            query.AppendLine("AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine("AND E.iCodCatalogo = "+ emple + "");
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString());
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                usuario = dr["vchCodigo"].ToString();
            }
            return usuario;
        }

        private int ObtieneSociedadEmple(int empleado)
        {
            int sociedad = 0;
            string sp = "EXEC WorkflowLineasObtieneDireccionEmple @esquema = {0}, @icodCatEmple = {1}";
            string query = string.Format(sp, "K5Afirme", empleado);
            DataTable dt = new DataTable();
            dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                sociedad = Convert.ToInt32(dr["iCodCatSociedad"]);           
            }

            return sociedad;
        }
    }
}