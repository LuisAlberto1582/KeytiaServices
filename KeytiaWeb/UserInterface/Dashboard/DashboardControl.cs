using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using KeytiaServiceBL.Reportes;
using System.Reflection;
using System.Collections.Generic;


namespace KeytiaWeb.UserInterface
{
    public enum DashboardState
    {
        Default,
        SubConsulta,
        SubAplicacion
    }

    public class DashboardControl : Panel, INamingContainer, IPostBackEventHandler
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();

        protected DataRow pRowDashboard;
        protected bool pbEtiquetarNumeros = false;

        protected string psOpcMnu;
        protected Label plblTitle;

        protected DSODateTimeBox pdtInicio;
        protected DSODateTimeBox pdtFin;

        protected Panel pToolBar;
        protected HtmlButton pbtnAplicar;
        protected HtmlButton pbtnEtiquetacion;

        /*RZ.20130207 Se agregan botones para Modelo y solo para ellos dirigen a reportes de telefonia*/
        protected HtmlButton pbtnRepTelFijaMeta;
        protected HtmlButton pbtnRepTelFijaTSystems;
        protected HtmlButton pbtnRepTelFijaDelloite;

        protected Panel pHeader;
        protected Table pTablaHeader;

        protected Panel pPanelReportes;
        protected Table pTablaReportes;

        protected ReporteEstandar pSubReporte;
        protected HistoricEdit pSubHistorico;

        protected Control pParentContainer;

        protected bool pbControlsCreated = false;

        protected DashboardFieldCollection pFields;

        public DashboardControl()
        {
            Init += new EventHandler(DashboardControl_Init);
        }

        public Control ParentContainer
        {
            get
            {
                return pParentContainer;
            }
            set
            {
                pParentContainer = value;
            }
        }

        public DashboardState State
        {
            get
            {
                if (ViewState["DashboardState"] == null)
                {
                    ViewState["DashboardState"] = DashboardState.Default;
                }
                return (DashboardState)ViewState["DashboardState"];
            }
            set
            {
                ViewState["DashboardState"] = value;
            }
        }

        public string Title
        {
            get
            {
                return (string)ViewState["Title"];
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public Label lblTitle
        {
            get
            {
                return plblTitle;
            }
            set
            {
                plblTitle = value;
            }
        }

        public string OpcMnu
        {
            get
            {
                return psOpcMnu;
            }
            set
            {
                psOpcMnu = value;
            }
        }

        public int iCodDashboard
        {
            get
            {
                return (int)ViewState["iCodDashboard"];
            }
            set
            {
                ViewState["iCodDashboard"] = value;
            }
        }

        public int iCodPerfil
        {
            get
            {
                return (int)ViewState["iCodPerfil"];
            }
            set
            {
                ViewState["iCodPerfil"] = value;
            }
        }

        public int iCodSubConsulta
        {
            get
            {
                if (ViewState["iCodSubConsulta"] == null)
                {
                    ViewState["iCodSubConsulta"] = 0;
                }
                return (int)ViewState["iCodSubConsulta"];
            }
            set
            {
                ViewState["iCodSubConsulta"] = value;
            }
        }

        public int iCodSubReporte
        {
            get
            {
                if (ViewState["iCodSubReporte"] == null)
                {
                    ViewState["iCodSubReporte"] = 0;
                }
                return (int)ViewState["iCodSubReporte"];
            }
            set
            {
                ViewState["iCodSubReporte"] = value;
            }
        }

        public int iSubEstadoConsulta
        {
            get
            {
                if (ViewState["iSubEstadoConsulta"] == null)
                {
                    ViewState["iSubEstadoConsulta"] = 0;
                }
                return (int)ViewState["iSubEstadoConsulta"];
            }
            set
            {
                ViewState["iSubEstadoConsulta"] = value;
            }
        }

        public int iCodSubAplicacion
        {
            get
            {
                if (ViewState["iCodSubAplicacion"] == null)
                {
                    ViewState["iCodSubAplicacion"] = 0;
                }
                return (int)ViewState["iCodSubAplicacion"];
            }
            set
            {
                ViewState["iCodSubAplicacion"] = value;
            }
        }

        public int iNumSubReporte
        {
            get
            {
                if (ViewState["iNumSubReporte"] == null)
                {
                    ViewState["iNumSubReporte"] = 0;
                }
                return (int)ViewState["iNumSubReporte"];
            }
            set
            {
                ViewState["iNumSubReporte"] = value;
            }
        }

        public int iCodRutaConsulta
        {
            get
            {
                if (ViewState["iCodRutaConsulta"] == null)
                {
                    ViewState["iCodRutaConsulta"] = 0;
                }
                return (int)ViewState["iCodRutaConsulta"];
            }
            set
            {
                ViewState["iCodRutaConsulta"] = value;
            }
        }

        public int iCodSubRutaConsulta
        {
            get
            {
                if (ViewState["iCodSubRutaConsulta"] == null)
                {
                    ViewState["iCodSubRutaConsulta"] = 0;
                }
                return (int)ViewState["iCodSubRutaConsulta"];
            }
            set
            {
                ViewState["iCodSubRutaConsulta"] = value;
            }
        }

        public DashboardFieldCollection Fields
        {
            get
            {
                return pFields;
            }
        }

        public string TitleLinks
        {
            get
            {
                if (ViewState["TitleLinks"] == null)
                {
                    ViewState["TitleLinks"] = "";
                }
                return (string)ViewState["TitleLinks"];
            }
            set
            {
                ViewState["TitleLinks"] = value;
            }
        }

        public ReporteEstandar SubReporte
        {
            get
            {
                return pSubReporte;
            }
            set
            {
                pSubReporte = value;
                iCodSubConsulta = pSubReporte.iCodConsulta;
                iCodSubReporte = pSubReporte.iCodReporte;
                iSubEstadoConsulta = pSubReporte.iEstadoConsulta;
                iNumSubReporte = pSubReporte.iNumReporte;
                iCodRutaConsulta = pSubReporte.iCodRutaConsulta;
                iCodSubRutaConsulta = pSubReporte.iCodSubRutaConsulta;
            }
        }

        public HistoricEdit SubHistorico
        {
            get
            {
                return pSubHistorico;
            }
        }

        protected void DashboardControl_Init(object sender, EventArgs e)
        {
            pToolBar = new Panel();
            pbtnAplicar = new HtmlButton();
            pbtnEtiquetacion = new HtmlButton();

            /*RZ.20130207 Se crean instancias a los objetos tipo HtmlButton*/
            pbtnRepTelFijaMeta = new HtmlButton();
            pbtnRepTelFijaTSystems = new HtmlButton();
            pbtnRepTelFijaDelloite = new HtmlButton();

            pHeader = new Panel();
            pTablaHeader = new Table();

            pPanelReportes = new Panel();
            pTablaReportes = new Table();

            CssClass = "Dashboard";
            Controls.Add(pToolBar);
            Controls.Add(pHeader);
            Controls.Add(pPanelReportes);
        }

        public void CreateControls()
        {
            try
            {
                if (pbControlsCreated)
                {
                    return;
                }

                InitConfig();
                InitAcciones();
                InitHeader();
                InitFechas(false);
                InitFields();
                InitSubReporte();
                InitSubAplicacion();
                SetDashboardState(State);

                pbControlsCreated = true;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        public void SetDashboardState(DashboardState s)
        {
            this.Visible = true;
            pbtnEtiquetacion.Visible = pbEtiquetarNumeros;
            pbtnRepTelFijaMeta.Visible = false;
            pbtnRepTelFijaTSystems.Visible = false;
            pbtnRepTelFijaDelloite.Visible = false;

            //RZ.20130207 Dejar visible boton solo a usuario de esquema Modelo
            if ((int)Session["iCodUsuarioDB"] == KeytiaServiceBL.KDBUtil.SearchICodCatalogo("UsuarDB", "Modelo"))
            {
                //Si el usuario es gpomodelo(78835) ó USPROCESOSYTEC(297350)
                if ((int)Session["iCodUsuario"] == 78835 || (int)Session["iCodUsuario"] == 297350)
                {
                    pbtnRepTelFijaMeta.Visible = true;
                    pbtnRepTelFijaTSystems.Visible = true;
                    pbtnRepTelFijaDelloite.Visible = true;
                }
            }

            if (s == DashboardState.SubConsulta
                || s == DashboardState.SubAplicacion)
            {
                this.Visible = false;

                if (pFields != null)
                {
                    pTablaReportes.Controls.Clear();
                    pFields = null;
                }
            }

            State = s;
        }

        protected void InitConfig()
        {
            pRowDashboard = pKDB.GetHisRegByEnt("Aplic", "Dashboard", "iCodCatalogo = " + iCodDashboard).Rows[0];

            if (pRowDashboard["{BanderasDashboard}"] != DBNull.Value)
            {
                pbEtiquetarNumeros = ((int)pRowDashboard["{BanderasDashboard}"] & 1) == 1;
            }
        }

        protected void InitAcciones()
        {
            pToolBar.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";

            pbtnAplicar.ID = "btnAplicar";
            pbtnAplicar.Attributes["class"] = "buttonPlay";
            pbtnAplicar.Style["display"] = "none";
            pbtnAplicar.ServerClick += new EventHandler(pbtnAplicar_ServerClick);
            pToolBar.Controls.Add(pbtnAplicar);

            /*RZ.20130207 Se agregan atributos a botones para reportes de cliente Modelo*/
            pbtnRepTelFijaMeta.ID = "btnRepTelFijaMeta";
            pbtnRepTelFijaMeta.Attributes["class"] = "button";
            pbtnRepTelFijaMeta.Style["display"] = "none";
            pbtnRepTelFijaMeta.ServerClick += new EventHandler(pbtnRepTelFijaMeta_ServerClick);
            pToolBar.Controls.Add(pbtnRepTelFijaMeta);

            pbtnRepTelFijaTSystems.ID = "btnRepTelFijaTSystems";
            pbtnRepTelFijaTSystems.Attributes["class"] = "button";
            pbtnRepTelFijaTSystems.Style["display"] = "none";
            pbtnRepTelFijaTSystems.ServerClick += new EventHandler(pbtnRepTelFijaTSystems_ServerClick);
            pToolBar.Controls.Add(pbtnRepTelFijaTSystems);

            pbtnRepTelFijaDelloite.ID = "btnRepTelFijaDelloite";
            pbtnRepTelFijaDelloite.Attributes["class"] = "button";
            pbtnRepTelFijaDelloite.Style["display"] = "none";
            pbtnRepTelFijaDelloite.ServerClick += new EventHandler(pbtnRepTelFijaDelloite_ServerClick);
            pToolBar.Controls.Add(pbtnRepTelFijaDelloite);

            pbtnEtiquetacion.ID = "btnEtiquetacion";
            pbtnEtiquetacion.Attributes["class"] = "buttonEdit";
            pbtnEtiquetacion.Style["display"] = "none";
            pbtnEtiquetacion.ServerClick += new EventHandler(pbtnEtiquetacion_ServerClick);
            pToolBar.Controls.Add(pbtnEtiquetacion);
        }

        protected void InitHeader()
        {
            pHeader.CssClass = "DashboardHeader";
            pTablaHeader.CssClass = "DashboardTablaHeader";
            pHeader.Controls.Add(pTablaHeader);

            pTablaHeader.Controls.Clear();

            pdtInicio = new DSODateTimeBox();
            pdtInicio.ID = "FechaIniRep";
            pdtInicio.Row = 1;
            pdtInicio.Table = pTablaHeader;
            pdtInicio.ShowHour = false;
            pdtInicio.ShowMinute = false;
            pdtInicio.ShowSecond = false;
            pdtInicio.CreateControls();
            pdtInicio.AddClientEvent("keytiaField", "FechaIniRep");

            pdtFin = new DSODateTimeBox();
            pdtFin.ID = "FechaFinRep";
            pdtFin.Row = 1;
            pdtFin.Table = pTablaHeader;
            pdtFin.ShowHour = false;
            pdtFin.ShowMinute = false;
            pdtFin.ShowSecond = false;
            pdtFin.CreateControls();
            pdtFin.AddClientEvent("keytiaField", "FechaFinRep");
        }

        protected void InitFechas(bool lbUsarValorSession)
        {
            ReporteEstandar.InitValoresSession();

            //RJ.20160707 Se solicita mostrar sólo el año actual y dos previos
            pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);
            pdtFin.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);

            if (pdtInicio != null)
            {
                
                pdtInicio.MaxDateTime = DateTime.Today;

                if (lbUsarValorSession || !pdtInicio.HasValue)
                {
                    pdtInicio.DataValue = Session["FechaIniRep"];
                }
                else
                {
                    Session["FechaIniRep"] = pdtInicio.Date;
                }
            }

            if (pdtFin != null)
            {
                pdtFin.MaxDateTime = DateTime.Today;

                if (lbUsarValorSession || !pdtFin.HasValue)
                {
                    pdtFin.DataValue = Session["FechaFinRep"];
                }
                else
                {
                    Session["FechaFinRep"] = pdtFin.Date;
                }
            }
        }


        protected void InitFields()
        {
            pPanelReportes.CssClass = "DashboardPanelReportes";
            pPanelReportes.Controls.Add(pTablaReportes);
            pTablaReportes.Controls.Clear();
            pTablaReportes.ID = "Reportes";
            pTablaReportes.CssClass = "DashboardTablaReportes";
            pTablaReportes.Width = Unit.Percentage(100);

            if (iCodSubConsulta == 0)
            {
                pFields = new DashboardFieldCollection(this, pTablaReportes, this.ValidarPermiso);
                pFields.InitFields();
                pFields.FillControls();
            }
        }

        public void InitSubReporte()
        {
            if (iCodSubConsulta > 0)
            {
                pSubReporte = new ReporteEstandar();
                pSubReporte.ID = "SubR" + iCodSubReporte + "_" + iNumSubReporte;
                pSubReporte.OpcMnu = OpcMnu;
                pSubReporte.lblTitle = lblTitle;
                pSubReporte.iCodConsulta = iCodSubConsulta;
                pSubReporte.iCodReporte = iCodSubReporte;
                pSubReporte.iEstadoConsulta = iSubEstadoConsulta;
                pSubReporte.iNumReporte = iNumSubReporte;
                pSubReporte.iCodRutaConsulta = iCodRutaConsulta;
                pSubReporte.iCodSubRutaConsulta = iCodSubRutaConsulta;
                pSubReporte.ParentContainer = ParentContainer;

                ParentContainer.Controls.Add(pSubReporte);

                pSubReporte.PostRegresarClick += new EventHandler(pSubReporte_PostRegresarClick);

                pSubReporte.CreateControls();
                pSubReporte.LoadScripts();
            }
        }

        protected void pSubReporte_PostRegresarClick(object sender, EventArgs e)
        {
            RemoverSubReporte();
        }

        public void RemoverSubReporte()
        {
            if (iCodSubReporte > 0)
            {
                pSubReporte.RemoverSubReporte();

                iCodSubConsulta = 0;
                iCodSubReporte = 0;
                iSubEstadoConsulta = 0;
                ParentContainer.Controls.Remove(pSubReporte);
                SetDashboardState(DashboardState.Default);
                pSubReporte = null;
            }
            if (iCodSubAplicacion > 0)
            {
                pSubHistorico.RemoverSubHistorico();

                iCodSubAplicacion = 0;
                ParentContainer.Controls.Remove(pSubHistorico);
                SetDashboardState(DashboardState.Default);
                pSubHistorico = null;
            }
            if (pFields == null)
            {
                InitFields();
            }
        }

        protected void InitSubAplicacion()
        {
            if (iCodSubAplicacion > 0)
            {
                DataRow lRowAplic = pKDB.GetHisRegByEnt("Aplic", "", "iCodCatalogo = " + iCodSubAplicacion).Rows[0];
                if (lRowAplic["{ParamVarChar3}"] != DBNull.Value)
                {
                    pSubHistorico = (HistoricEdit)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricEdit)).CodeBase, lRowAplic["{ParamVarChar3}"].ToString()).Unwrap();
                }
                else
                {
                    pSubHistorico = new HistoricEdit();
                }
                if (lRowAplic["{ParamVarChar4}"] != DBNull.Value)
                {
                    pSubHistorico.CollectionClass = lRowAplic["{ParamVarChar4}"].ToString();
                }
                if (lRowAplic["{ParamVarChar1}"] != DBNull.Value)
                {
                    pSubHistorico.SetEntidad(lRowAplic["{ParamVarChar1}"].ToString());

                    if (lRowAplic["{ParamVarChar2}"] != DBNull.Value)
                    {
                        pSubHistorico.SetMaestro(lRowAplic["{ParamVarChar2}"].ToString());
                    }
                }

                pSubHistorico.ID = this.ID + "SubH" + iCodSubAplicacion;
                pSubHistorico.OpcMnu = this.OpcMnu;
                pSubHistorico.iCodAplicacion = iCodSubAplicacion;
                pSubHistorico.lblTitle = this.lblTitle;
                pSubHistorico.EsSubHistorico = true;
                ParentContainer.Controls.Add(pSubHistorico);

                pSubHistorico.LoadScripts();
                pSubHistorico.CreateControls();
                pSubHistorico.PostCancelarClick += new EventHandler(pSubHistorico_PostCancelarClick);
            }
        }

        protected void pbtnAplicar_ServerClick(object sender, EventArgs e)
        {
            if (pdtInicio.HasValue)
            {
                Session["FechaIniRep"] = pdtInicio.Date;
            }
            else
            {
                pdtInicio.DataValue = Session["FechaIniRep"];
            }

            if (pdtFin.HasValue)
            {
                Session["FechaFinRep"] = pdtFin.Date;
            }
            else
            {
                pdtFin.DataValue = Session["FechaFinRep"];
            }

            foreach (DashboardReportField lField in pFields)
            {
                lField.Reporte.HTParam = null;
                lField.Reporte.bConfiguraGrid = true;
                if (lField.Reporte.dtInicio != null)
                {
                    lField.Reporte.dtInicio.DataValue = pdtInicio.Date;
                }
                if (lField.Reporte.dtFin != null)
                {
                    lField.Reporte.dtFin.DataValue = pdtFin.Date;
                }
            }
        }

        protected void pbtnEtiquetacion_ServerClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Historicos.aspx?Opc=OpcMiEiqueta");
            /* RZ.20130322 Se retira la parte de la creacion de la subaplicacion, para dejar un redirect
             * hacia la opcion de menu "Mi etiquetacion" esto para darle mejor performance
            DataTable lKDBTable = pKDB.GetHisRegByCod("Aplic", new string[] { "MiEtiquetacion" });
            if (lKDBTable.Rows.Count > 0)
            {
                iCodSubAplicacion = (int)lKDBTable.Rows[0]["iCodCatalogo"];
                InitSubAplicacion();
                SetDashboardState(DashboardState.SubAplicacion);
            }
            */ 
        }

        protected void pSubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            RemoverSubReporte();
        }

        /*RZ:20130207 Se agrega metodo para el evento click del boton, lo que hara que se redireccione 
          a la consulta del reporte indicado en la opcion de menu del URL*/
        protected void pbtnRepTelFijaMeta_ServerClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Consultas/Consultas.aspx?Opc=OpcConRepTabTopTenTelFijaMetaSP");
        }

        protected void pbtnRepTelFijaTSystems_ServerClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Consultas/Consultas.aspx?Opc=OpcConsRepTabTopTenTelFijaTSystemSP");
        }

        protected void pbtnRepTelFijaDelloite_ServerClick(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Consultas/Consultas.aspx?Opc=OpcConsRepTabTopTenTelFijaDelloiteSP");
        }

        public void InitLanguage()
        {
            string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
            pdtInicio.Descripcion = Globals.GetLangItem("Atrib", "Atributos", "FechaIniRep");
            pdtFin.Descripcion = Globals.GetLangItem("Atrib", "Atributos", "FechaFinRep");
            pbtnAplicar.InnerText = Globals.GetMsgWeb(false, "DashboardBtnAplicar");
            pbtnEtiquetacion.InnerText = Globals.GetMsgWeb(false, "DashboardBtnEtiquetarNumeros");

            /*RZ.20130207 Se extrae el valor de la la propiedad InnerText del boton (titulo del boton)
              desde historico dado de alta en la entidad y maestro Mensajes Web (disponible solo en Modelo)*/
            pbtnRepTelFijaMeta.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaMeta");
            pbtnRepTelFijaDelloite.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaDelloite");
            pbtnRepTelFijaTSystems.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaTSystems");

            InitFechas(true);

            if (State == DashboardState.Default)
            {
                pFields.InitLanguage();
                plblTitle.Text = TitleLinks + Title;
            }
            else if (State == DashboardState.SubConsulta)
            {
                string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "RemoverSubReporte");
                string lsLinkRegresar = "<a href=\"javascript:" + lsdoPostBack + "\">" + HttpUtility.HtmlEncode(Title) + "</a>";

                pSubReporte.TitleLinks = TitleLinks + lsLinkRegresar + " / ";
                pSubReporte.Title = pKDB.GetHisRegByEnt("RepEst", "", "iCodCatalogo = " + iCodSubReporte).Rows[0][lsLang].ToString();
                pSubReporte.InitLanguage();
            }
            else if (State == DashboardState.SubAplicacion)
            {
                string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "RemoverSubReporte");
                string lsLinkRegresar = "<a href=\"javascript:" + lsdoPostBack + "\">" + HttpUtility.HtmlEncode(Title) + "</a>";

                pSubHistorico.Title = TitleLinks + lsLinkRegresar + " / " + Globals.GetMsgWeb(false, "DashboardBtnEtiquetarNumeros");
                pSubHistorico.AlertTitle = Title + " / " + Globals.GetMsgWeb(false, "DashboardBtnEtiquetarNumeros");
                pSubHistorico.InitLanguage();
            }
        }

        #region Exportar Documento

        public void ExportXLS()
        {
            if (State == DashboardState.SubConsulta)
            {
                pSubReporte.ExportXLS();
            }
            else if (State == DashboardState.SubAplicacion)
            {
                pSubHistorico.ExportXLS();
            }
            else
            {
                DSOControl.jAlert(Page, this.ID + ".ExpNoDisp", DSOControl.JScriptEncode(Globals.GetLangItem("ExpNoDisp")), DSOControl.JScriptEncode(Title));
            }
        }

        public void ExportDOC()
        {
            if (State == DashboardState.SubConsulta)
            {
                pSubReporte.ExportDOC();
            }
            else if (State == DashboardState.SubAplicacion)
            {
                pSubHistorico.ExportDOC();
            }
            else
            {
                DSOControl.jAlert(Page, this.ID + ".ExpNoDisp", DSOControl.JScriptEncode(Globals.GetLangItem("ExpNoDisp")), DSOControl.JScriptEncode(Title));
            }
        }

        public void ExportPDF()
        {
            if (State == DashboardState.SubConsulta)
            {
                pSubReporte.ExportPDF();
            }
            else if (State == DashboardState.SubAplicacion)
            {
                pSubHistorico.ExportPDF();
            }
            else
            {
                DSOControl.jAlert(Page, this.ID + ".ExpNoDisp", DSOControl.JScriptEncode(Globals.GetLangItem("ExpNoDisp")), DSOControl.JScriptEncode(Title));
            }
        }

        public void ExportCSV()
        {
            if (State == DashboardState.SubConsulta)
            {
                pSubReporte.ExportCSV();
            }
            else if (State == DashboardState.SubAplicacion)
            {
                pSubHistorico.ExportCSV();
            }
            else
            {
                DSOControl.jAlert(Page, this.ID + ".ExpNoDisp", DSOControl.JScriptEncode(Globals.GetLangItem("ExpNoDisp")), DSOControl.JScriptEncode(Title));
            }
        }

        #endregion

        protected bool ValidarPermiso(Permiso p)
        {
            return DSONavegador.getPermiso(psOpcMnu, p);
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "RemoverSubReporte")
            {
                RemoverSubReporte();
            }
        }

        #endregion
    }
}
