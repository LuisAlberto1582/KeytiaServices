using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;

using KeytiaServiceBL;
using DSOControls2008;
using System.Web.UI;
//RZ.20140602 Se agrega using para usar objeto DataTable
using System.Data;

namespace KeytiaWeb
{
    public enum KeytiaExportFormat
    {
        xlsx,
        docx,
        pdf,
        csv
    }

    public delegate void KeytiaExportEventHandler(KeytiaExportFormat lkeFormat);

    public class KeytiaMasterPage : System.Web.UI.MasterPage
    {
        public event KeytiaExportEventHandler Export;

        protected global::System.Web.UI.HtmlControls.HtmlForm form1;
        protected global::System.Web.UI.WebControls.Button botonDefault;
        protected global::System.Web.UI.WebControls.Button btnLanguageOk;

        protected global::System.Web.UI.WebControls.Image imgCustomerLogo;

        protected global::DSOControls2008.DSOWindow wndLanguage;
        protected global::DSOControls2008.DSOWindow wndCurrency;
        protected global::DSOControls2008.DSOWindow wndPassword;

        protected global::System.Web.UI.WebControls.LinkButton imgToolNavExit;
        protected global::System.Web.UI.WebControls.Panel imgToolUsrLanguage;
        protected global::System.Web.UI.WebControls.Panel imgToolUsrCurrency;
        protected global::System.Web.UI.WebControls.Panel imgToolUsrPassword;

        protected global::System.Web.UI.WebControls.Literal usrName;
        protected global::System.Web.UI.WebControls.Literal usrNameLI;
        protected global::System.Web.UI.WebControls.Literal infoAño;
        protected global::System.Web.UI.WebControls.HyperLink lnkHomePage;
        protected global::System.Web.UI.WebControls.Image imgLogoSidebar;
        protected global::System.Web.UI.WebControls.Image imgLogoDefault;
        protected global::System.Web.UI.WebControls.Image imgLogoMovil;
        protected global::System.Web.UI.WebControls.Image imgTop;

        protected global::System.Web.UI.WebControls.HiddenField hfFechaInicio;
        protected global::System.Web.UI.WebControls.HiddenField hfFechaFin;


        protected KeytiaMasterPage()
        {
            Init += new EventHandler(KeytiaMasterPage_Init);
            PreRender += new EventHandler(KeytiaMasterPage_PreRender);
        }

        protected void KeytiaMasterPage_Init(object sender, EventArgs e)
        {
            InitMasterControls();
            InitMasterToolBars();
        }

        protected void KeytiaMasterPage_PreRender(object sender, EventArgs e)
        {
            InitLanguage();
            InitShowUserName();
            //RZ.20140602 Se retira llamado este metodo
            //CheckServiceIsRunning();

            botonDefault.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = "return false;";
            if (!Page.IsPostBack)
            {
                form1.DefaultButton = botonDefault.UniqueID;
            }
        }

        protected void KeytiaMasterPage_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "EXPORT")
                OnExport((KeytiaExportFormat)Enum.Parse(typeof(KeytiaExportFormat), (string)e.CommandArgument));

            if (e.CommandName.ToUpper() == "EXIT")
            {
                Session.Clear();
                Session.Abandon();
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect(ConfigurationManager.AppSettings["logOutRedirect"].ToString());
            }
        }

        protected virtual void OnExport(KeytiaExportFormat lkeFormat)
        {
            if (Export != null)
                Export(lkeFormat);
        }

        public virtual void InitMasterControls()
        {
            EnsureChildControls();

            string lsStyleSheet = (string)HttpContext.Current.Session["StyleSheet"];
            Page.ClientScript.GetPostBackEventReference(this, "");

            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "master.scripts",
                //Graficas
                "<script type=\"text/javascript\" id=\"JSMain\" src=\"" + ResolveClientUrl("~/scripts/FusionCharts/fusioncharts.js") + "\" ></script>\r\n" +
                "<script type=\"text/javascript\" src=\"" + ResolveClientUrl("~/scripts/FusionCharts/themes/fusioncharts.theme.dti.js") + "\"></script>\r\n " +
                "<script type=\"text/javascript\" src=\"" + ResolveClientUrl("~/scripts/FusionCharts/themes/fusioncharts.theme.ternium.js") + "\"></script>\r\n " +
                "<script type=\"text/javascript\" src=\"" + ResolveClientUrl("~/scripts/FusionCharts/themes/fusioncharts.theme.Line.js") + "\"></script>\r\n " +
                "<script type=\"text/javascript\" id=\"JScharts\" src=\"" + ResolveClientUrl("~/scripts/FusionCharts/fusioncharts.charts.js") + "\"></script>\r\n" +
                //
                "<script language='javascript' src='" + ResolveClientUrl("~/scripts/jquery.easing.1.3.js") + "'></script>\r\n" +
                "<script language='javascript' src='" + ResolveClientUrl("~/scripts/jquery.jcontent.0.8.js") + "'></script>\r\n" +
                "<script language='javascript' src='" + ResolveClientUrl("~/scripts/masterPage.js?v=20120125a") + "'></script>\r\n" +
                "<script language='javascript'>KeytiaMaster.appPath = \"" + ResolveClientUrl("~/") + "\";</script>",
                true, false);

            DSOControl.LoadControlScriptBlock(Page, typeof(HtmlLink), "master.styles",
                "<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/jquery.css") + "\" />" +
                "<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/DSOControls2008.css") + "\" />" +
                "<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/jquery.windows-engine.css") + "\" />" +
                "<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/jcontent.css") + "\" />",
                true, true);

            DSOControl.LoadControlScriptBlock(Page, typeof(HtmlLink), "styles", Styles(lsStyleSheet), true, true);

            if (imgCustomerLogo != null)
                imgCustomerLogo.ImageUrl = (string)HttpContext.Current.Session["CustomerLogo"];

            lnkHomePage.NavigateUrl = ResolveUrl((string)HttpContext.Current.Session["HomePage"]);
            imgLogoSidebar.ImageUrl = ResolveUrl("~/img/logo-keytia-blanco-v@2x.png");
            imgLogoDefault.ImageUrl = ResolveUrl("~/img/logo-keytia-blanco@2x.png");
            imgLogoMovil.ImageUrl = ResolveUrl("~/img/logo-keytia@2x.png");

            infoAño.Text = DateTime.Now.Year.ToString();

            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "scripts", Scripts(), false, false);

            BlockBackButton();
        }

        private string Styles(string lsStyleSheet)
        {
            StringBuilder style = new StringBuilder();
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"https://fonts.googleapis.com/css?family=Poppins:300,300i,400,400i,500,500i,700,700i\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/bootstrap/css/bootstrap.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/css/components.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/css/plugins.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/layouts/layout/css/layout.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/layouts/layout/css/themes/darkblue.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/FontAwesome/css/font-awesome.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/datatables/DataTables-1.10.16/css/jquery.dataTables.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/DataTables/DataTables-1.10.16/css/dataTables.bootstrap.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/scripts/assets/global/plugins/bootstrap-select/css/bootstrap-select.min.css") + "\" />");
            style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/css/keytia.css?v=2") + "\" />");


            if (DSODataContext.Schema == "bat")
            {
                style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/css/keytiaBat.css?v=2") + "\" />");
            }
            else
            {
                style.AppendLine("<link rel=\"Stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/css/keytia.css?v=2") + "\" />");

            }

            return style.ToString();
        }

        private string Scripts()
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/jquery.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/bootstrap/js/bootstrap.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/js.cookie.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js") + "'></script>");

            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/moment.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/counterup/jquery.waypoints.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/counterup/jquery.counterup.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/flot/jquery.flot.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/flot/jquery.flot.resize.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/flot/jquery.flot.categories.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/jquery.sparkline.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" charset=\"utf-8\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/Highmaps/highmaps.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" charset=\"utf-8\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/Highmaps/mexico-all.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" charset=\"utf-8\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/Highmaps/usa-all.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" charset=\"utf-8\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/FontAwesome/fontawesome-all.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" charset=\"utf-8\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/ChartJs/Chart.bundle.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/bootstrap-tabdrop/js/bootstrap-tabdrop.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/DataTables/DataTables-1.10.16/js/jquery.dataTables.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/DataTables/datatables.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/DataTables/DataTables-1.10.16/js/dataTables.bootstrap.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/bootstrap-select/js/bootstrap-select.min.js") + "'></script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/plugins/bootstrap-select/js/i18n/defaults-es_ES.js") + "'></script>");

            script.AppendLine("<script type=\"text/javascript\"> var jQNewLook = jQuery.noConflict(true); </script>");
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/global/scripts/app.js") + "'></script>");   //
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/pages/scripts/dashboard.js") + "'></script>"); //

            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/layouts/layout/scripts/layout.js") + "'></script>");  //
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/layouts/layout/scripts/demo.js") + "'></script>"); //
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/assets/layouts/global/scripts/quick-sidebar.js") + "'></script>"); //

            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/DateRangePicker/datePickerForm.js") + "'></script>");//
            //script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/ChartJs/drawChartsInPage.js") + "'></script>");  //Demo entregable
            //script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/Highmaps/drawMapInPage.js") + "'></script>");  //Demo entregable
            //script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/DateRangePicker/datePickerHeader.js") + "'></script>"); --> Se invoca en la master           

            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/app.js?v=1") + "'></script>");  //
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/DataTables/dataTableDetail.js") + "'></script>"); //
            script.AppendLine("<script type=\"text/javascript\" src='" + ResolveClientUrl("~/scripts/js/SelectPicker/selectPicker.js") + "'></script>");//

            return script.ToString();
        }

        public virtual void InitMasterToolBars()
        {
            InitToolImage(imgToolNavExit, null, @"location.href = '" + ResolveClientUrl("~/Login.aspx") + "';");
            InitToolImage(imgToolUsrLanguage, null, (wndLanguage != null ? "DSOControls.Window.Init.call($('#" + wndLanguage.Content.ClientID + "'));$('#" + wndLanguage.Content.ClientID + "').showWindow();" : ""));
            InitToolImage(imgToolUsrCurrency, null, (wndCurrency != null ? "DSOControls.Window.Init.call($('#" + wndCurrency.Content.ClientID + "'));$('#" + wndCurrency.Content.ClientID + "').showWindow();" : ""));
            InitToolImage(imgToolUsrPassword, null, (wndPassword != null ? "DSOControls.Window.Init.call($('#" + wndPassword.Content.ClientID + "'));$('#" + wndPassword.Content.ClientID + "').showWindow();" : ""));

            imgTop.ImageUrl = ResolveUrl("~/img/icon_top.png");
        }

        protected virtual void InitLanguage()
        {
            InitToolImageText(imgToolNavExit, "Salir");
            InitToolImageText(imgToolUsrLanguage, "Idioma");
            InitToolImageText(imgToolUsrCurrency, "Moneda");
            InitToolImageText(imgToolUsrPassword, "Pwd");
        }

        protected void InitToolBar(Image limgCtl, Panel lpnlCtl)
        {
            if (limgCtl != null)
            {
                DSOControl.LoadControlScriptBlock(Page, this.GetType(), "TB." + limgCtl.ClientID,
                    "<script type=\"text/javascript\">\r\n" +
                    "var tb_" + limgCtl.ClientID + ";\r\n" +
                    "$(document).ready(function() {\r\n" +
                    "   tb_" + limgCtl.ClientID + " = new KeytiaMaster.Toolbar(\r\n" +
                    "       \"tb_" + limgCtl.ClientID + "\",\r\n" +
                    "       \"" + limgCtl.ClientID + "\",\r\n" +
                    "       \"" + lpnlCtl.ClientID + "\");\r\n" +
                    "});\r\n" +
                    "</script>\r\n",
                    false, false);
            }
        }

        protected void InitToolImage(Control limgCtl, Label llblDescription, string lsClientClick)
        {
            if (limgCtl != null)
            {
                ////limgCtl.ImageUrl = "~/images/" + lsImageFile;

                if (limgCtl is LinkButton)
                {
                    ((LinkButton)limgCtl).Command += new CommandEventHandler(KeytiaMasterPage_Command);
                }
                else
                {
                    DSOControl.LoadControlScriptBlock(Page, this.GetType(), "TBI." + limgCtl.ClientID,
                    "<script type='text/javascript'>\r\n" +
                    "var tbb_" + limgCtl.ClientID + ";\r\n" +
                    "$(document).ready(function() {\r\n" +
                    "   tbb_" + limgCtl.ClientID + " = new KeytiaMaster.ToolbarButton(\r\n" +
                    "       \"" + limgCtl.ClientID + "\",\r\n" +
                    "       " + (llblDescription != null ? "\"" + llblDescription.ClientID + "\"" : "null") + ",\r\n" +
                    "       function() {" + lsClientClick + "});\r\n" +
                    "});\r\n" +
                    "</script>\r\n",
                    false, false);
                }

            }
        }

        protected void InitToolImageText(Control limgCtl, string lsLangItem)
        {
            if (limgCtl != null)
            {
                DSOControl.LoadControlScriptBlock(Page, this.GetType(), "TBIT." + limgCtl.ClientID,
                    "<script language='javascript'>\r\n" +
                    "$(document).ready(function() {\r\n" +
                    "   tbb_" + limgCtl.ClientID + ".text = \"" + Globals.GetLangItem(lsLangItem) + "\"\r\n" +
                    "});\r\n" +
                    "</script>\r\n",
                    false, false);
            }
        }

        /*RZ.20131014*/
        /// <summary>
        /// Se encarga de revisar si el Servicio y el COM se encuentra encendido.
        /// El metodo es llamada entre cada Pre-Render que pasa por la páginas de Keytia
        /// </summary>
        protected void CheckServiceIsRunning()
        {
            /*El mensaje de alerta solo se muestra a los usuarios de perfil configurador*/
            if ((int)Session["iCodPerfil"] == KeytiaServiceBL.KDBUtil.SearchICodCatalogo("Perfil", "Config"))
            {
                //RZ.20140602 Leer el valor de la bandera para saber si se debe mostrar el mensaje
                /*NOTA: Si se quiere volver a reactivar esta validacion, ademas de descomentar las lineas
                 * es necessario crear un nuevo atributo llamado BanderasMonitorProceso de tipo flag y agregarle
                 * un historico en Valores de atributo que se llame No mostrar mensaje de servicio detenido (valor integer 1)
                 * el atributo se debe agregar al maestro Proceso de la entidad Monitor.
                 */
                if (MostrarMensajeServicio("KeytiaService"))
                {
                    /*Saber si el KeytiaService esta corriendo*/
                    bool lbKS = Pinger.ProcessIsRunning("KeytiaService");
                    /*Saber si el KeytiaCOM esta corriendo*/
                    bool lbKC = Pinger.ProcessIsRunning("KeytiaCOM");

                    /*Si el valor de la session es nulo para PingKS entonces 
                     * lo deja como false*/
                    if (HttpContext.Current.Session["PingKS"] == null)
                        HttpContext.Current.Session["PingKS"] = false;

                    /*Si el valor de la session es nulo para PingKC entonces 
                     * lo deja como false*/
                    if (HttpContext.Current.Session["PingKC"] == null)
                        HttpContext.Current.Session["PingKC"] = false;

                    /*Si el valor de lbKS es falso y el valor de la session para PingKS es false
                     * entonces muestra el mensaje de "el servicio se encuentra apagado"*/
                    if (!lbKS && !(bool)HttpContext.Current.Session["PingKS"])
                    {
                        /*Arma alert para el mensaje del servicio apagado*/
                        DSOControl.LoadControlScriptBlock(Page, this.GetType(), "PingKS",
                            "<script language='javascript'>" +
                            "$(document).ready(function() {jAlert('" + Globals.GetLangItem("KServStpd") + "'); });" +
                            "</script>\r\n", false, false);

                        //guarda en session el valor para PingKS en true
                        HttpContext.Current.Session["PingKS"] = true;
                    }

                    /*Se encuentra comentado el aviso de que el COM se encuentra apagado.
                     "La aplicación de procesamiento batch se encuentra apagada. Por favor avise a su administrador del sistema."*/
                    //if (!lbKC && !(bool)HttpContext.Current.Session["PingKC"])
                    //{
                    //    DSOControl.LoadControlScriptBlock(Page, this.GetType(), "PingKC",
                    //        "<script language='javascript'>" +
                    //        "$(document).ready(function() {jAlert('" + Globals.GetLangItem("KComStpd") + "'); });" +
                    //        "</script>\r\n", false, false);

                    //    HttpContext.Current.Session["PingKC"] = true;
                    //}

                    /*Guarda en sesion false para PingKS, si es que lbKS se encuentra en true*/
                    if (lbKS)
                        HttpContext.Current.Session["PingKS"] = false;

                    /*Guarda en sesion false para PingKS, si es que lbKS se encuentra en true*/
                    if (lbKC)
                        HttpContext.Current.Session["PingKC"] = false;
                }
            }
        }

        private bool MostrarMensajeServicio(string lsProcess)
        {
            bool lbVAlor = true;
            int liValorBandera = 0;
            KDBAccess kdb = new KDBAccess();

            //Se conecta con las credenciales default del sistema. Esquema Keytia
            DSODataContext.SetContext();

            //Extraer el valor del ping (fecha) del proceso recibido como parametro
            DataTable ldt = kdb.GetHisRegByEnt("Monitor", "Proceso", new string[] { "{BanderasMonitorProceso}" }, "vchCodigo = '" + lsProcess + "'");
            //retornar al contexto del esquema de la sesion de la pagina
            DSODataContext.SetContext((int)Session["iCodUsuarioDB"]);

            if (ldt != null && ldt.Rows.Count > 0 && ldt.Rows[0]["{BanderasMonitorProceso}"] != System.DBNull.Value)
                liValorBandera = (int)ldt.Rows[0]["{BanderasMonitorProceso}"];

            if (liValorBandera == 1)
            {
                lbVAlor = false;
            }

            return lbVAlor;
        }

        protected void BlockBackButton()
        {
            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "BlockBackButton",
                "<script language='javascript'>\r\n" +
                "var isBack = (KeytiaMaster.getCookie('isBack') != '');\r\n" +
                "KeytiaMaster.setCookie('isBack','x',1);\r\n" +
                "window.history.forward(1);\r\n" +
                "</script><script language='javascript'>\r\n" +
                "KeytiaMaster.setCookie('isBack','',1);\r\n" +
                "if (isBack)\r\n" +
                "    $(document).ready(function() {jAlert('" + Globals.GetLangItem("NoBack") + "'); });\r\n" +
                "</script>\r\n",
                true, false);
        }

        protected void InitShowUserName()
        {
            string vchUsuarioLogin;
            vchUsuarioLogin = Globals.GetLangItem("Bienvenido", (string)Session["vchCodUsuario"]);

            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "ShowUserName",
                "<script language='javascript'>\r\n" +
                "$(document).ready(function() { KeytiaMaster.showUserName('" + vchUsuarioLogin + "'); });\r\n" +
                "</script>\r\n",
                false, false);

            usrName.Text = vchUsuarioLogin;
            usrNameLI.Text = Session["vchCodUsuario"].ToString();
        }

        public virtual void ExtraerFechasRangeFrontToBack()
        {
            DateTime fechaI;
            DateTime fechaF;
            if (DateTime.TryParse(hfFechaInicio.Value, out fechaI) && DateTime.TryParse(hfFechaFin.Value, out fechaF))
            {
                Globals.SetDates(fechaI.ToString("yyyy-MM-dd"), fechaF.ToString("yyyy-MM-dd"));
            }
        }
    }
}
