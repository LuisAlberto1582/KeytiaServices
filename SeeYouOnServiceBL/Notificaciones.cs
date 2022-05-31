using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KeytiaServiceBL.Alarmas;
using System.Net.Mail;
using System.Collections;
using KeytiaServiceBL.Reportes;

using KeytiaServiceBL;

namespace SeeYouOnServiceBL
{
    public class AsistenteConf : Empleado
    {
        protected int piCodAsistente;
        protected string pvchCodAsistente;
        protected int piEstAsistente;
        protected int piIdioma;
        public int iCodAsistente
        {
            get { return piCodAsistente; }
            set { piCodAsistente = value; }
        }

        public string vchCodAsistente
        {
            get { return pvchCodAsistente; }
            set { pvchCodAsistente = value; }
        }

        public AsistenteConf(int liCodUsuario)
            : base(liCodUsuario)
        {
        }
        public AsistenteConf(int liCodEmpleado, bool getSuper)
            : base(liCodEmpleado, getSuper)
        {

        }

        public AsistenteConf(int liCodEmpleado, bool getSuper, DataRow ldrAsistente)
            : this(liCodEmpleado, getSuper)
        {
            piCodAsistente = (int)Util.IsDBNull(ldrAsistente["iCodRegistro"], int.MinValue);
            pvchCodAsistente = (string)Util.IsDBNull(ldrAsistente["vchCodigo"], "");
            piEstAsistente = (int)Util.IsDBNull(ldrAsistente["EstAsistente"], int.MinValue);

        }

    }

    public class NotificacionAsistentes
    {
        protected KDBAccess kdb = new KDBAccess();
        protected int piCodUsuarioDB;
        protected string psTempPath;
        protected string psStylePath;
        protected int piCodCliente;
        protected DataRow pdrCliente;
        protected string psLogoClientePath;
        protected string psLogoKeytiaPath;
        protected string pvchIdioma;

        //Datos de la conferencia 
        protected int piCodConferencia;
        protected int piCodIdioma;
        protected DataRow pdrConferencia;
        protected string pvchAsuntoConferencia = "";
        protected string pvchParticipante = "";
        protected DateTime pdtFechaInicioReservacion;
        protected DateTime pdtFechaFinReservacion;
        protected string pvchNumericId = "";

        //Datos de los Participantes 
        protected int piEstEliminado;
        protected string pvchTMSPhoneBookContact = "";

        //Datos de los Asistentes
        protected int piCodAsistente;
        protected int piEstNotificado;

        protected string psCC;
        protected string psCCO;
        protected string psPlantilla;
        protected string psLang;

        protected MailAccess poMail;

        public int iCodUsuarioDB
        {
            get
            {
                return piCodUsuarioDB;
            }
            set
            {
                piCodUsuarioDB = value;
            }
        }

        public int iCodConferencia
        {
            get
            {
                return piCodConferencia;
            }
            set
            {
                piCodConferencia = value;
            }
        }

        public string vchIdioma
        {
            get
            {
                return pvchIdioma;
            }
            set
            {
                pvchIdioma = value;
            }
        }

        public NotificacionAsistentes()
        {
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            initVars();
            string lsStylePath = "";
            psStylePath = lsStylePath;
        }

        protected void initVars()
        {
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            psCC = "";
            psCCO = "";
            psPlantilla = "";
            //Estatus de Participante Eliminado
            piEstEliminado = (int)Util.IsDBNull(KDBUtil.SearchScalar("EstParticipante", "Eliminado",
                        "iCodCatalogo", true), 0);
            //Estatus de Asistente Notificado 
            piEstNotificado = (int)Util.IsDBNull(KDBUtil.SearchScalar("EstAsistente", "Notificado",
                        "iCodCatalogo", true), 0);

        }

        public void NotificAsistConf()
        {
            NotificaAsistentes();
        }

        public void NotificAsistConf(int liCodPart)
        {
            NotificaAsistentes(liCodPart);
        }

        private void NotificaAsistentes()
        {

            //Conferencia
            getDatosConferencia(piCodConferencia);

            //Participantes
            DataTable ldtPart = kdb.GetHisRegByEnt("Participante", "Participante",
                new string[] { "iCodCatalogo", "{TMSConf}", "{TMSPhoneBookContact}", "{Address}" },
                "{TMSConf} = " + piCodConferencia + " and {EstParticipante} <> " + piEstEliminado);

            if (ldtPart == null || ldtPart.Rows.Count == 0)
                return;

            int liCodTMSPhoneBookContact = int.MinValue;

            foreach (DataRow ldrPrart in ldtPart.Rows)
            {
                liCodTMSPhoneBookContact = (int)Util.IsDBNull(ldrPrart["{TMSPhoneBookContact}"], 0);
                pvchTMSPhoneBookContact = (string)Util.IsDBNull(KDBUtil.SearchScalar("TMSPhoneBookContact", liCodTMSPhoneBookContact,
                            "vchDescripcion", true), "");
                ProcesarParticipante((int)Util.IsDBNull(ldrPrart["iCodCatalogo"], int.MinValue));
            }
        }

        private void NotificaAsistentes(int liCodPart)
        {

            //Participantes
            DataTable ldtPart = kdb.GetHisRegByEnt("Participante", "Participante",
                new string[] { "iCodCatalogo", "{TMSConf}", "{TMSPhoneBookContact}", "{Address}" },
                "iCodCatalogo = " + liCodPart + " and {EstParticipante} <> " + piEstEliminado);

            if (ldtPart == null || ldtPart.Rows.Count == 0)
                return;

            //Conferencia
            piCodConferencia = (int)Util.IsDBNull(ldtPart.Rows[0]["{TMSConf}"], 0);
            getDatosConferencia(piCodConferencia);

            int liCodTMSPhoneBookContact = int.MinValue;

            foreach (DataRow ldrPrart in ldtPart.Rows)
            {
                liCodTMSPhoneBookContact = (int)Util.IsDBNull(ldrPrart["{TMSPhoneBookContact}"], 0);
                pvchTMSPhoneBookContact = (string)Util.IsDBNull(KDBUtil.SearchScalar("TMSPhoneBookContact", liCodTMSPhoneBookContact,
                            "vchDescripcion", true), "");
                ProcesarParticipante((int)Util.IsDBNull(ldrPrart["iCodCatalogo"], int.MinValue));
            }
        }

        private void ProcesarParticipante(int liCodPart)
        {
            Hashtable lhsAsistentes = getAsistentesNotificacion(liCodPart);
            foreach (AsistenteConf loAsistente in lhsAsistentes.Values)
            {
                try
                {
                    EnviarCorreo(loAsistente);
                    ActualizaEstatus(loAsistente);
                }
                catch (Exception ex)
                {
                    Util.LogException("Error notificando a un asistente.", ex);
                }
            }
        }

        private Hashtable getAsistentesNotificacion(int liCodPart)
        {
            Hashtable lhtAsistentes = new Hashtable();
            DataTable ldtAsistentes = getAsistentes(liCodPart);
            foreach (DataRow ldrAsist in ldtAsistentes.Rows)
            {
                AsistenteConf loAsistente = new AsistenteConf((int)Util.IsDBNull(ldrAsist["Emple"], int.MinValue), true, ldrAsist);
                lhtAsistentes.Add(loAsistente.iCodEmpleado, loAsistente);
            }
            return lhtAsistentes;
        }

        protected virtual DataTable getAsistentes(int liParticipante)
        {
            return DSODataAccess.Execute(
                "select iCodRegistro,iCodCatalogo,vchCodigo,Emple,EstAsistente " +
                " from [" + DSODataContext.Schema + "].[VisHistoricos('AsistenteConferencia','Asistentes','Español')] " +
                " where dtIniVigencia <> dtFinVigencia" +
                " and Participante = " + liParticipante +
                " and isNull(EstAsistente, 0) <> " + piEstNotificado);
        }

        private void getDatosConferencia(int liCodConf)
        {
            //Conferencia
            DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                new string[] { "iCodCatalogo", "{AsuntoConferencia}","{TMSSystems}", "{ConfNumericId}",
                    "{FechaInicioReservacion}", "{FechaFinReservacion}","{Client}" },
                "iCodCatalogo = " + liCodConf);
            if (ldrConf == null)
                throw new Exception("No se encontró la conferencia '" + liCodConf + "'");

            pvchNumericId = (string)Util.IsDBNull(ldrConf["{ConfNumericId}"], "");
            pvchAsuntoConferencia = (string)Util.IsDBNull(ldrConf["{AsuntoConferencia}"], "");
            pdtFechaInicioReservacion = (DateTime)Util.IsDBNull(ldrConf["{FechaInicioReservacion}"], DateTime.MinValue);
            pdtFechaFinReservacion = (DateTime)Util.IsDBNull(ldrConf["{FechaFinReservacion}"], DateTime.MinValue);
            piCodCliente = (int)Util.IsDBNull(ldrConf["{Client}"], 0);

            pdrCliente = KDBUtil.SearchHistoricRow("Client", "Clientes",
                new string[] { "iCodCatalogo", "{Esquema}", "{Logo}", "{StyleSheet}" },
                "iCodCatalogo = " + piCodCliente);

            string lsLogoClientePath = (string)Util.IsDBNull(pdrCliente["{Logo}"], "");
            string psKeytiaWebFPath = Util.AppSettings("KeytiaWebFPath");

            psStylePath = System.IO.Path.Combine(psKeytiaWebFPath, pdrCliente["{StyleSheet}"].ToString().Replace("~/", "").Replace("/", "\\"));
            psPlantilla = System.IO.Path.Combine(psStylePath, @"plantillas\Conferencias\PlantillaNotificacionAsistentes.docx");

            psLogoKeytiaPath = System.IO.Path.Combine(psStylePath, @"images\KeytiaHeader.png");
            psLogoClientePath = System.IO.Path.Combine(psStylePath, lsLogoClientePath.Replace("~/", "").Replace("/", "\\"));

        }

        private void EnviarCorreo(AsistenteConf loAsistente)
        {
            List<string> lstPara = new List<string>();
            List<string> lstCC = new List<string>();
            string lsWordPath = psPlantilla;

            lstPara.Add(getDestinatarios(loAsistente));
            lstCC.Add(psCC);
            string lsPara = string.Join(";", lstPara.ToArray());
            string lsCC = string.Join(";", lstCC.ToArray());


            if (string.IsNullOrEmpty(lsPara) || string.IsNullOrEmpty(lsWordPath))
            {
                return;
            }
            WordAccess loWord = null;
            try
            {
                loWord = new WordAccess();
                loWord.FilePath = lsWordPath;
                loWord.Abrir(true);

                ReemplazarMetaTags(loWord, loAsistente, true);

                string lsFileName = getFileName(".docx");
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
                loWord.Cerrar();
                loWord.Salir();
                loWord = null;

                PrepararCorreo();

                poMail.Asunto = pvchAsuntoConferencia;
                poMail.AgregarWord(lsFileName);
                poMail.Para.Add(lsPara);
                poMail.CC.Add(lsCC);
                poMail.BCC.Add(psCCO);
                poMail.Enviar();
                if (poMail.HayError)
                {
                    string lsMensaje = "No se pudo enviar el correo de Notificación al empleado " + loAsistente.iCodEmpleado + ": " + loAsistente.vchDescripcion + ".";
                    Util.LogMessage(lsMensaje);
                    throw new Exception(lsMensaje);
                }
            }
            catch (Exception ex)
            {
                string lsMensaje = "Error al enviar el correo de Notificación al empleado " + loAsistente.iCodEmpleado + ": " + loAsistente.vchDescripcion + ".";
                Util.LogException(lsMensaje, ex);
                throw new Exception(lsMensaje, ex);
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord = null;
                }
            }
        }

        private void ActualizaEstatus(AsistenteConf loAsistente)
        {
            Hashtable lhtVal = new Hashtable();

            //Actualiza el estatus de la conferencia
            lhtVal.Clear();
            lhtVal.Add("iCodRegistro", loAsistente.iCodAsistente);
            lhtVal.Add("{EstAsistente}", piEstNotificado);
            lhtVal.Add("dtFecUltAct", DateTime.Now);

            KDBUtil.SaveHistoric("AsistenteConferencia", "Asistentes", loAsistente.vchCodAsistente, null, lhtVal);

        }

        protected void ReemplazarMetaTags(WordAccess loWord, AsistenteConf loAsistente, bool lbReemplazarMsgWeb)
        {

            loWord.Abrir(true);
            // Obten los Mensajes de la plantilla
            //{SaludoNotificacion}
            //{textoNotificacion}
            //{FechaInicioConferencia}
            //{FechaFinConferencia}

            string lsSaludoNotificacion;
            string lsTextoNotificacion;
            string lsFechaInicioConferencia;
            string lsFechaFinConferencia;

            lsSaludoNotificacion = ReporteEstandarUtil.GetLangItem(pvchIdioma, "MsgWeb", "Mensajes Web", "SaludoNotificacion", loAsistente.vchDescripcion);
            lsTextoNotificacion = ReporteEstandarUtil.GetLangItem(pvchIdioma, "MsgWeb", "Mensajes Web", "textoNotificacion", pvchAsuntoConferencia, pvchTMSPhoneBookContact);
            lsFechaInicioConferencia = ReporteEstandarUtil.GetLangItem(pvchIdioma, "MsgWeb", "Mensajes Web", "FechaInicioConferencia", pdtFechaInicioReservacion.ToString("dd-MMMM-yyyy HH:mm:ss"));
            lsFechaFinConferencia = ReporteEstandarUtil.GetLangItem(pvchIdioma, "MsgWeb", "Mensajes Web", "FechaFinConferencia", pdtFechaFinReservacion.ToString("dd-MMMM-yyyy HH:mm:ss"));


            if (System.IO.File.Exists(psLogoClientePath))
            {
                loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
            }
            else
            { loWord.ReemplazarTexto("{LogoCliente}", ""); }

            if (System.IO.File.Exists(psLogoKeytiaPath))
            {
                loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
            }
            else
            { loWord.ReemplazarTexto("{LogoKeytia}", ""); }


            loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
            loWord.ReemplazarTexto("{SaludoNotificacion}", lsSaludoNotificacion);
            loWord.ReemplazarTexto("{textoNotificacion}", lsTextoNotificacion);
            loWord.ReemplazarTexto("{FechaInicioConferencia}", lsFechaInicioConferencia);
            loWord.ReemplazarTexto("{FechaFinConferencia}", lsFechaFinConferencia);

        }

        private void PrepararCorreo()
        {
            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.De = new System.Net.Mail.MailAddress(Util.AppSettings("appeMailIDSYO"));
            poMail.Usuario = Util.AppSettings("appeMailUserSYO");
            poMail.Password = Util.AppSettings("appeMailPwdSYO");
            poMail.Puerto = Int32.Parse(Util.AppSettings("appeMailPortSYO"));
            poMail.UsarSSL = Util.AppSettings("appeMailEnableSslSYO");
            poMail.ServidorSMTP = Util.AppSettings("SmtpServerSYO");
            poMail.ReplyTo = new System.Net.Mail.MailAddress(Util.AppSettings("appeMailReplyToSYO"));
        }

        private string getDestinatarios(AsistenteConf loAsistente)
        {
            return loAsistente.Email;
        }

        protected MailAddress getRemitente()
        {
            return new MailAddress(Util.AppSettings("appeMailID"));
        }

        private string getFileName(string lsExt)
        {
            string lsFileName;
            System.IO.Directory.CreateDirectory(psTempPath);
            lsFileName = Guid.NewGuid().ToString();
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsFileName + lsExt));
        }

    }

    public class EnvioCuentasMOVI
    {
        protected KDBAccess kdb = new KDBAccess();

        //Datos de la Cuenta TMS 
        protected int piServidorTMS;
        protected int piCodCuentaMovi;
        protected string psTextoLinkSoftware = "";

        //Datos para formar el correo
        protected string psTempPath;
        protected string psStylePath;

        protected string pvchAsuntoEmail = "";
        protected string psTextoCorreo = "";
        protected string psLogoKeytiaPath;
        protected string psDespedida = "";

        protected string psCC;
        protected string psCCO;
        protected string psPlantilla;
        protected string psLang;

        protected MailAccess poMail;

        public EnvioCuentasMOVI()
        {
        }

        protected void initVars(string lsStyleSheet)
        {
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            psCC = "";
            psCCO = "";
            psPlantilla = "";

            string lsKeytiaWebFPath = Util.AppSettings("KeytiaWebFPath");

            StringBuilder lsbLog = new StringBuilder();

            psStylePath = System.IO.Path.Combine(lsKeytiaWebFPath, lsStyleSheet.Replace("~/", "").Replace("/", "\\"));
            psPlantilla = System.IO.Path.Combine(psStylePath, @"plantillas\Conferencias\PlantillaEnvioCuentaMOVI.docx");
            psLogoKeytiaPath = System.IO.Path.Combine(psStylePath, @"images\KeytiaHeader.png");

            lsbLog.AppendLine("Ruta de búsqueda de archivos: " + psStylePath);
            lsbLog.AppendLine("Ruta de la plantilla: " + psPlantilla);
            lsbLog.AppendLine("Ruta del logo: " + psLogoKeytiaPath);

            //Util.LogMessage("Rutas del envío de correo.\r\n" + lsbLog.ToString());

            pvchAsuntoEmail = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "AsuntoCorreoCtasMOVI");
            psTextoCorreo = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "textoCorreoCtasMOVI");
            psTextoLinkSoftware = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "textoLinkSoftware");
            psDespedida = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web", "DespedidaCorreoCtasMOVI");

        }

        public void EnvioCuentaServidorTMS(int liServidorTMS, string lsLang, string lsStyle)
        {
            psLang = lsLang;
            initVars(lsStyle);
            DataTable ldtCtasMOVI = getDatosCuentasMovi(liServidorTMS, 0);
            foreach (DataRow ldrMOVI in ldtCtasMOVI.Rows)
            {
                EnviarCorreo(ldrMOVI);
            }
        }

        public void EnvioCuentaServidorTMS(string lsLang, string lsStyle, int liCodCuentaMovi)
        {
            psLang = lsLang;
            initVars(lsStyle);
            DataTable ldtCtasMOVI = getDatosCuentasMovi(0, liCodCuentaMovi);
            foreach (DataRow ldrMOVI in ldtCtasMOVI.Rows)
            {
                EnviarCorreo(ldrMOVI);
            }

        }

        private DataTable getDatosCuentasMovi(int liServidorTMS, int liCodCuentaMovi)
        {
            //Cuentas MOVI
            string lsWhere = "";

            if (liCodCuentaMovi > 0)
                lsWhere = " iCodCatalogo = " + liCodCuentaMovi;

            if (liServidorTMS > 0)
                if (lsWhere == "")
                    lsWhere = "{ServidorTMS} = " + liServidorTMS;
                else
                    lsWhere = " and {ServidorTMS} = " + liServidorTMS;

            DataTable ldtCuentasMovi = kdb.GetHisRegByEnt("TMSSystems", "Cuenta Movi",
                new string[] { "iCodCatalogo", "{ServidorTMS}", "{Nombre}", "{Email}", 
                    "{UsuarioTMS}", "{Password}" }, lsWhere);

            return ldtCuentasMovi;

        }

        private void EnviarCorreo(DataRow ldrCtasMOVI)
        {
            string lsWordPath = psPlantilla;
            string lsPara = (string)Util.IsDBNull(ldrCtasMOVI["{Email}"], "");
            string lsNombre = (string)Util.IsDBNull(ldrCtasMOVI["{Nombre}"], "");

            if (string.IsNullOrEmpty(lsPara) || string.IsNullOrEmpty(lsWordPath))
            {
                return;
            }
            WordAccess loWord = null;
            try
            {
                loWord = new WordAccess();
                loWord.FilePath = lsWordPath;
                loWord.Abrir(true);

                ReemplazarMetaTags(loWord, ldrCtasMOVI, true);

                string lsFileName = getFileName(".docx");

                loWord.FilePath = lsFileName;

                loWord.SalvarComo();
                loWord.Cerrar();
                loWord.Salir();
                loWord = null;

                PrepararCorreo();

                poMail.Asunto = pvchAsuntoEmail;
                poMail.AgregarWord(lsFileName);
                poMail.Para.Add(lsPara);
                poMail.CC.Add(psCC);
                poMail.BCC.Add(psCCO);
                poMail.Enviar();
                if (poMail.HayError)
                {
                    string lsMensaje = "No se pudo enviar el correo con la cuenta MOVI de " + lsNombre + ".";
                    Util.LogMessage(lsMensaje);
                    throw new Exception(lsMensaje);
                }
            }
            catch (Exception ex)
            {
                string lsMensaje = "Error al enviar el correo con la cuenta MOVI de " + lsNombre + ".";
                Util.LogException(lsMensaje, ex);
                throw new Exception(lsMensaje, ex);
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord = null;
                }
            }
        }

        protected void ReemplazarMetaTags(WordAccess loWord, DataRow ldrCtasMOVI, bool lbReemplazarMsgWeb)
        {

            loWord.Abrir(true);
            // Obten los Mensajes de la plantilla
            //{SaludoCorreoCtasMOVI}
            //{textoCorreoCtasMOVI}
            //{textoUsuarioCtasMOVI}
            //{textoPsswCtasMOVI}
            //{textoLinkSoftware}

            string lsPasswordDes = "";
            string lsUsuario = (string)Util.IsDBNull(ldrCtasMOVI["{UsuarioTMS}"], "");
            string lsPassword = (string)Util.IsDBNull(ldrCtasMOVI["{Password}"], "");
            string lsNombre = (string)Util.IsDBNull(ldrCtasMOVI["{Nombre}"], "");

            string lsSaludoCorreoCtasMOVI = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web",
                "SaludoCorreoCtasMOVI", lsNombre);

            string lstextoUsuarioCtasMOVI = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web",
                "textoUsuarioCtasMOVI", lsUsuario);

            if (lsPassword != "")
                lsPasswordDes = KeytiaServiceBL.Util.Decrypt(lsPassword);

            string lstextoPsswCtasMOVI = ReporteEstandarUtil.GetLangItem(psLang, "MsgWeb", "Mensajes Web",
                "textoPsswCtasMOVI", lsPasswordDes);

            if (System.IO.File.Exists(psLogoKeytiaPath))
            {
                loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
            }
            else
            {
                loWord.ReemplazarTexto("{LogoKeytia}", "");
            }

            loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
            loWord.ReemplazarTexto("{SaludoCorreoCtasMOVI}", lsSaludoCorreoCtasMOVI);
            loWord.ReemplazarTexto("{textoCorreoCtasMOVI}", psTextoCorreo);
            loWord.ReemplazarTexto("{textoUsuarioCtasMOVI}", lstextoUsuarioCtasMOVI);
            loWord.ReemplazarTexto("{textoPsswCtasMOVI}", lstextoPsswCtasMOVI);
            loWord.ReemplazarTexto("{textoLinkSoftware}", psTextoLinkSoftware);
            loWord.ReemplazarTexto("{DespedidaCorreoCtasMOVI}", psDespedida);

        }

        private string getFileName(string lsExt)
        {
            string lsFileName;
            System.IO.Directory.CreateDirectory(psTempPath);
            lsFileName = Guid.NewGuid().ToString();
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsFileName + lsExt));
        }

        protected MailAddress getRemitente()
        {
            return new MailAddress(Util.AppSettings("appeMailID"));
        }

        private void PrepararCorreo()
        {
            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.De = new System.Net.Mail.MailAddress(Util.AppSettings("appeMailIDSYO_Movi"));
            poMail.Usuario = Util.AppSettings("appeMailUserSYO");
            poMail.Password = Util.AppSettings("appeMailPwdSYO");
            poMail.Puerto = Int32.Parse(Util.AppSettings("appeMailPortSYO"));
            poMail.UsarSSL = Util.AppSettings("appeMailEnableSslSYO");
            poMail.ServidorSMTP = Util.AppSettings("SmtpServerSYO");
            poMail.ReplyTo = new System.Net.Mail.MailAddress(Util.AppSettings("appeMailReplyToSYO"));
        }
    }
}
