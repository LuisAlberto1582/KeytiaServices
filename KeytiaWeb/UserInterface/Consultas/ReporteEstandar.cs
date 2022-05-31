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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public enum ReportState
    {
        Default,
        SubReporte,
        Aplicacion,
        DashboardReporte,
        DashboardGrafica,
        DashboardGraficaHis,
        DashboardReporte2Bloques,
        DashboardGrafica2Bloques,
        DashboardGraficaHis2Bloques,
        ConfigurandoParametros
    }

    public class ReporteEstandar : Panel, INamingContainer, IPostBackEventHandler
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();

        protected string psOpcMnu;

        protected Panel pToolBar;
        protected HtmlButton pbtnRegresar;
        protected HtmlButton pbtnParametros;

        /*RZ.20130208 Se agregan botones para Modelo y solo para ellos dirigen a reportes de telefonia*/
        protected HtmlButton pbtnRepTelFijaMeta;
        protected HtmlButton pbtnRepTelFijaTSystems;
        protected HtmlButton pbtnRepTelFijaDelloite;

        /* NZ 20160819 Se agrega botón de Exportar para Banorte */
        protected HtmlButton pbtnExportXLS;

        protected Panel pHeader;
        protected Panel pZonasReporte;
        protected Panel pPrimeraZona;
        protected Panel pSegundaZona;

        protected Panel pPanelGrafica;
        protected Panel pPanelGraficaHis;

        protected DSOExpandable pExpParametros;
        protected DSOExpandable pExpGrafica;
        protected DSOExpandable pExpGraficaHis;
        protected DSOExpandable pExpReporte;
        protected Table pTablaParametros;
        protected Table pTablaHeader;
        protected DSOChart pGrafica;
        protected DSOChart pGraficaHis;
        protected DSOGrid pGridReporte;

        protected DSOWindow pWdCorreo;
        protected Label pLblEnviaCorreo;
        protected Table pTablaCorreo;
        protected DSOTextBox pTxtPara;
        protected HtmlButton pbtnEnviaCorreo;
        protected HtmlButton pbtnCancelaCorreo;

        protected ReportFieldCollection pFields;

        protected ReporteEstandar pReporte = null;
        protected ReporteEstandar pSubReporte;
        protected HistoricEdit pSubHistorico;

        protected bool pbControlsCreated = false;

        protected bool pbAreaGrafica;
        protected bool pbAreaGraficaHis;
        protected bool pbAreaReporte;
        protected bool pbAreaParametros;
        protected bool pbOrdenCol;
        protected bool pbParamFecIniFin;
        protected bool pbReajustarParam;
        protected bool pbModoDebug;
        protected bool pbParamNumReg;

        protected DataTable pKDBTypes;

        protected DataSet pDSCampos;

        protected string psdoPostBackRutaGrid;
        protected string psdoPostBackRutaY;
        protected string psdoPostBackRutaXY;

        protected string psdoPostBackParametros;

        protected DataRow pKDBRowReporte;
        protected DataRow pKDBRowDataSource;
        protected DataTable pKDBRelRutas;
        protected DataTable pVisConsultas;
        protected DataTable pVisAplicaciones;
        protected Hashtable pHTbtnAplic;

        protected TipoReporte pTipoReporte = TipoReporte.Tabular;
        protected Orientation pOrientacionZonas = Orientation.Horizontal;
        protected OrdenZonasReporte pOrdenZonas = OrdenZonasReporte.PrimeroGraficas;

        protected DSODateTimeBox pdtInicio;
        protected DSODateTimeBox pdtFin;
        protected DSONumberEdit piNumReg;

        protected KeytiaFlagOptionField pOpcTipoGrafica;
        protected KeytiaFlagOptionField pOpcTipoGraficaHis;

        protected Hashtable pHTParam;
        protected Hashtable pHTParamDesc;

        protected DataTable pTablaTotales;

        protected string psOperAgrupacion;

        protected Control pParentContainer;

        protected int piWidth1GrVertical = 1220;
        protected int piWidth2GrVertical = 610;
        protected int piHeightGrVertical = 500;

        protected int piWidthGrHorizontal = 610;
        protected int piHeight1GrHorizontal = 548;
        protected int piHeight2GrHorizontal = 250;

        protected int piWidthDashboardGr = 610;
        protected int piWidthDashboardGr2Bloques = 1220;
        protected int piHeightDashboardGr = 400;

        //Variables para dar formato
        protected KeytiaIntegerField pIntegerField = new KeytiaIntegerField();
        protected KeytiaIntegerFormatField pIntegerFormatField = new KeytiaIntegerFormatField();
        protected KeytiaNumericField pNumericField = new KeytiaNumericField();
        protected KeytiaCurrencyField pCurrencyField = new KeytiaCurrencyField();

        protected string pvchCodIdioma = Globals.GetCurrentLanguage();
        protected string psDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
        protected string psDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");


        protected Label plblTitle;
        protected string pjsObj;

        protected string psFileKey;
        protected string psTempPath;

        public ReporteEstandar()
        {
            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), DSOUpload.EscapeFolderName(Session.SessionID));
            System.IO.Directory.CreateDirectory(psTempPath);

            Init += new EventHandler(ReporteEstandar_Init);
        }

        public ReportState State
        {
            get
            {
                if (ViewState["ReportState"] == null)
                {
                    ViewState["ReportState"] = ReportState.Default;
                }
                return (ReportState)ViewState["ReportState"];
            }
            set
            {
                ViewState["ReportState"] = value;
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

        public string TitleRuta
        {
            get
            {
                return ViewState["TitleRuta"] as string;
            }
            set
            {
                ViewState["TitleRuta"] = value;
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

        public int iCodConsulta
        {
            get
            {
                return (int)ViewState["iCodConsulta"];
            }
            set
            {
                ViewState["iCodConsulta"] = value;
            }
        }

        public int iEstadoConsulta
        {
            get
            {
                if (ViewState["iEstadoConsulta"] == null)
                {
                    ViewState["iEstadoConsulta"] = 0;
                }
                return (int)ViewState["iEstadoConsulta"];
            }
            set
            {
                ViewState["iEstadoConsulta"] = value;
                iCodEstadoConsulta = (int)pKDB.GetHisRegByEnt("EstadoConsulta", "Estado Reporte", new string[] { "iCodCatalogo" }, "{Value} =" + value).Rows[0]["iCodCatalogo"];
            }
        }

        public int iCodEstadoConsulta
        {
            get
            {
                if (ViewState["iCodEstadoConsulta"] == null)
                {
                    ViewState["iCodEstadoConsulta"] = 0;
                }
                return (int)ViewState["iCodEstadoConsulta"];
            }
            protected set
            {
                ViewState["iCodEstadoConsulta"] = value;
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

        public int iNumReporte
        {
            get
            {
                if (ViewState["iNumReporte"] == null)
                {
                    ViewState["iNumReporte"] = 0;
                }
                return (int)ViewState["iNumReporte"];
            }
            set
            {
                ViewState["iNumReporte"] = value;
            }
        }

        public int iCodReporte
        {
            get
            {
                return (int)ViewState["iCodReporte"];
            }
            set
            {
                ViewState["iCodReporte"] = value;
            }
        }

        public int iCodPerfil
        {
            get
            {
                if (ViewState["iCodPerfil"] == null)
                {
                    ViewState["iCodPerfil"] = (int)Session["iCodPerfil"];
                }
                return (int)ViewState["iCodPerfil"];
            }
            set
            {
                ViewState["iCodPerfil"] = value;
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

        public ReportFieldCollection Fields
        {
            get
            {
                return pFields;
            }
        }

        public bool bConfiguraGrid
        {
            get
            {
                if (ViewState["bConfiguraGrid"] == null)
                {
                    ViewState["bConfiguraGrid"] = true;
                }
                return (bool)ViewState["bConfiguraGrid"];
            }
            set
            {
                ViewState["bConfiguraGrid"] = value;
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

        protected string DataSourceGr
        {
            get
            {
                return Util.Decrypt(ViewState["DataSourceGr"].ToString());
            }
            set
            {
                ViewState["DataSourceGr"] = Util.Encrypt(value.Replace("\r\n", " "));
            }
        }

        protected string DataSourceGrHis
        {
            get
            {
                return Util.Decrypt(ViewState["DataSourceGrHis"].ToString());
            }
            set
            {
                ViewState["DataSourceGrHis"] = Util.Encrypt(value.Replace("\r\n", " "));
            }
        }

        public KeytiaFlagOptionField OpcTipoGrafica
        {
            get
            {
                return pOpcTipoGrafica;
            }
        }

        public KeytiaFlagOptionField OpcTipoGraficaHis
        {
            get
            {
                return pOpcTipoGraficaHis;
            }
        }

        public DSOGrid GridReporte
        {
            get
            {
                return pGridReporte;
            }
        }

        public Hashtable HTParam
        {
            get
            {
                return ViewState["HTParam"] as Hashtable;
            }
            set
            {
                ViewState["HTParam"] = value;
            }
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

        public bool bAreaGrafica
        {
            get
            {
                return pbAreaGrafica;
            }
        }

        public bool bAreaGraficaHis
        {
            get
            {
                return pbAreaGraficaHis;
            }
        }

        public bool bAreaReporte
        {
            get
            {
                return pbAreaReporte;
            }
        }

        public bool bAreaParametros
        {
            get
            {
                return pbAreaParametros;
            }
        }

        public bool bOrdenCol
        {
            get
            {
                return pbOrdenCol;
            }
        }

        public bool bParamFecIniFin
        {
            get
            {
                return pbParamFecIniFin;
            }
        }

        public bool bReajustarParam
        {
            get
            {
                return pbReajustarParam;
            }
        }

        public bool bModoDebug
        {
            get
            {
                return pbModoDebug;
            }
        }

        public Orientation OrientacionZonas
        {
            get
            {
                return pOrientacionZonas;
            }
        }

        public OrdenZonasReporte OrdenZonas
        {
            get
            {
                return pOrdenZonas;
            }
        }

        public ReporteEstandar SubReporte
        {
            get
            {
                return pSubReporte;
            }
        }

        public HistoricEdit SubHistorico
        {
            get
            {
                return pSubHistorico;
            }
        }

        public DSODateTimeBox dtInicio
        {
            get
            {
                return pdtInicio;
            }
        }

        public DSODateTimeBox dtFin
        {
            get
            {
                return pdtFin;
            }
        }

        public DSONumberEdit iNumReg
        {
            get
            {
                return piNumReg;
            }
        }

        public int NumeroRegistros
        {
            get
            {
                if (ViewState["NumeroRegistros"] == null)
                {
                    ViewState["NumeroRegistros"] = 0;
                }
                return (int)ViewState["NumeroRegistros"];
            }
            set
            {
                ViewState["NumeroRegistros"] = value;
            }
        }

        public ReporteEstandar Reporte
        {
            get
            {
                return pReporte;
            }
            set
            {
                pReporte = value;
            }
        }

        public bool RemoverReporte
        {
            get
            {
                if (ViewState["RemoverReporte"] == null)
                {
                    ViewState["RemoverReporte"] = false;
                }
                return (bool)ViewState["RemoverReporte"];
            }
            set
            {
                ViewState["RemoverReporte"] = value;
            }
        }

        public string ExportExt
        {
            get
            {
                if (ViewState["ExportExt"] == null)
                {
                    ViewState["ExportExt"] = "";
                }
                return (string)ViewState["ExportExt"];
            }
            set
            {
                ViewState["ExportExt"] = value;
            }
        }

        public virtual void LoadScripts()
        {
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "ReporteEstandar.js", "<script src='" + ResolveClientUrl("~/UserInterface/Consultas/ReporteEstandar.js?V=1") + "' type='text/javascript'></script>\r\n", true, false);
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "KeytiaFields.js", "<script src='" + ResolveClientUrl("~/UserInterface/Colecciones/Campos/KeytiaFields.js?V=2") + "' type='text/javascript'></script>\r\n", true, false);
        }

        #region PostEvents

        protected EventHandler pPostRegresarClick;
        protected EventHandler pPostInitRuta;
        protected EventHandler pPostSetSubReporteParams;

        public event EventHandler PostRegresarClick
        {
            add
            {
                pPostRegresarClick += value;
            }
            remove
            {
                pPostRegresarClick -= value;
            }
        }

        public event EventHandler PostInitRuta
        {
            add
            {
                pPostInitRuta += value;
            }
            remove
            {
                pPostInitRuta -= value;
            }
        }

        public event EventHandler PostSetSubReporteParams
        {
            add
            {
                pPostSetSubReporteParams += value;
            }
            remove
            {
                pPostSetSubReporteParams -= value;
            }
        }

        protected void FirePostRegresarClick()
        {
            if (pPostRegresarClick != null)
            {
                pPostRegresarClick(this, new EventArgs());
            }
        }

        protected void FirePostInitRuta()
        {
            if (pPostInitRuta != null)
            {
                pPostInitRuta(this, new EventArgs());
            }
        }

        protected void FirePostSetSubReporteParams()
        {
            if (pPostSetSubReporteParams != null)
            {
                pPostSetSubReporteParams(this, new EventArgs());
            }
        }

        #endregion

        public virtual void ReporteEstandar_Init(object sender, EventArgs e)
        {
            InitControls();
        }

        protected virtual void InitControls()
        {
            pToolBar = new Panel();
            pbtnRegresar = new HtmlButton();

            pExpParametros = new DSOExpandable();
            pTablaParametros = new Table();

            pHeader = new Panel();
            pTablaHeader = new Table();

            pZonasReporte = new Panel();
            pPrimeraZona = new Panel();
            pSegundaZona = new Panel();

            pPanelGrafica = new Panel();
            pPanelGraficaHis = new Panel();

            pExpGrafica = new DSOExpandable();
            pExpGraficaHis = new DSOExpandable();
            pGrafica = new DSOChart();
            pGraficaHis = new DSOChart();

            pExpReporte = new DSOExpandable();
            pGridReporte = new DSOGrid();

            pjsObj = this.ID;

            this.CssClass = "ReporteEstandar";
            Controls.Add(pToolBar);
            Controls.Add(pHeader);
            Controls.Add(pExpParametros);
            Controls.Add(pZonasReporte);
        }

        public virtual void CreateControls()
        {
            try
            {
                if (pbControlsCreated)
                {
                    return;
                }

                InitConfig();
                InitAcciones();
                InitParametros();
                InitHeader();
                InitParamSession();
                InitZonas();
                InitGrafica();
                InitGraficaHis();
                InitReporte();
                InitSubReporte();
                InitAplicacion();
                InitEnvioCorreo();

                pbControlsCreated = true;
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        public virtual void SetReportState(ReportState s)
        {
            this.Visible = true;
            pToolBar.Visible = true;
            pHeader.Visible = true;
            pExpParametros.Visible = pbAreaParametros;
            pPrimeraZona.Visible = true;
            pSegundaZona.Visible = true;
            pPanelGrafica.Visible = pbAreaGrafica;
            pPanelGraficaHis.Visible = pbAreaGraficaHis;
            pTablaParametros.Visible = false;

            if (s == ReportState.Default)
            {
                pTablaParametros.Visible = false;
                if (pbAreaParametros)
                {
                    pExpParametros.TextOptions.Text = "";
                    pExpParametros.StartOpen = false;
                }
            }
            else if (s == ReportState.SubReporte)
            {
                this.Visible = false;
            }
            else if (s == ReportState.Aplicacion)
            {
                this.Visible = false;
            }
            else if (s == ReportState.DashboardReporte
                || s == ReportState.DashboardReporte2Bloques)
            {
                pToolBar.Visible = false;
                pHeader.Visible = false;
                pExpParametros.Visible = false;

                if (pbAreaGrafica || pbAreaGraficaHis)
                {
                    if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                    {
                        pPrimeraZona.Visible = false;
                        pSegundaZona.CssClass = s.ToString();
                    }
                    else if (pOrdenZonas == OrdenZonasReporte.PrimeroReportes)
                    {
                        pPrimeraZona.CssClass = s.ToString();
                        pSegundaZona.Visible = false;
                    }
                }
                else
                {
                    pPrimeraZona.CssClass = s.ToString();
                    pSegundaZona.Visible = false;
                }
            }
            else if (s == ReportState.DashboardGrafica
                || s == ReportState.DashboardGrafica2Bloques)
            {
                pToolBar.Visible = false;
                pHeader.Visible = false;
                pExpParametros.Visible = false;

                if (pbAreaReporte)
                {
                    if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                    {
                        pPrimeraZona.CssClass = s.ToString();
                        pSegundaZona.Visible = false;
                    }
                    else if (pOrdenZonas == OrdenZonasReporte.PrimeroReportes)
                    {
                        pPrimeraZona.Visible = false;
                        pSegundaZona.CssClass = s.ToString();
                    }
                }
                else
                {
                    pPrimeraZona.CssClass = s.ToString();
                }
                if (pbAreaGraficaHis)
                {
                    pPanelGraficaHis.Visible = false;
                }

                InitChartSize(pPanelGrafica, pGrafica, s);
            }
            else if (s == ReportState.DashboardGraficaHis
                || s == ReportState.DashboardGraficaHis2Bloques)
            {
                pToolBar.Visible = false;
                pHeader.Visible = false;
                pExpParametros.Visible = false;

                if (pbAreaReporte)
                {
                    if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                    {
                        pPrimeraZona.CssClass = s.ToString();
                        pSegundaZona.Visible = false;
                    }
                    else if (pOrdenZonas == OrdenZonasReporte.PrimeroReportes)
                    {
                        pPrimeraZona.Visible = false;
                        pSegundaZona.CssClass = s.ToString();
                    }
                }
                else
                {
                    pPrimeraZona.CssClass = s.ToString();
                }
                if (pbAreaGrafica)
                {
                    pPanelGrafica.Visible = false;
                }

                InitChartSize(pPanelGraficaHis, pGraficaHis, s);
            }
            else if (s == ReportState.ConfigurandoParametros)
            {
                pTablaParametros.Visible = true;
                pExpParametros.TextOptions.Text = "";
                pExpParametros.StartOpen = true;
                pExpParametros.OnOpen = "function(){" + pjsObj + ".fnInitGridsParam();}";
            }

            State = s;
        }

        protected void InitConfig()
        {
            pKDBTypes = pKDB.GetHisRegByEnt("Types", "Tipos de Datos");
            pKDBRowReporte = pKDB.GetHisRegByEnt("RepEst", "", "iCodCatalogo = " + iCodReporte).Rows[0];

            if (!this.CssClass.Contains(pKDBRowReporte["vchCodigo"].ToString()))
            {
                this.CssClass += " " + pKDBRowReporte["vchCodigo"].ToString();
            }

            pTipoReporte = (TipoReporte)Enum.Parse(typeof(TipoReporte), DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + pKDBRowReporte["iCodMaestro"]).ToString());

            InitRepRutas();
            InitRepAplicaciones();

            int liBanderas = (int)pKDBRowReporte["{BanderasRepEstandar}"];

            pbAreaGrafica = (liBanderas & 1) == 1;
            pbAreaGraficaHis = (liBanderas & 2) == 2;
            pbAreaReporte = (liBanderas & 4) == 4;
            pbAreaParametros = (liBanderas & 8) == 8;
            pbOrdenCol = (liBanderas & 16) == 16;
            pbParamFecIniFin = (liBanderas & 32) == 32;
            pbReajustarParam = (liBanderas & 64) == 64;
            pbModoDebug = (liBanderas & 128) == 128;
            pbParamNumReg = (liBanderas & 256) == 256;

            pOrientacionZonas = (Orientation)pKDBRowReporte["{RepEstOrientacionZonas}"];
            pOrdenZonas = (OrdenZonasReporte)pKDBRowReporte["{RepEstOrdenZonas}"];

            if (pTipoReporte == TipoReporte.Tabular)
            {
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRep}"]).Rows[0];
                pDSCampos = ReporteEstandarUtil.GetCamposTabular(iCodReporte);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                if (pKDBRowReporte["{RepTipoResumen}"] == DBNull.Value
                    || int.Parse(pKDBRowReporte["{RepTipoResumen}"].ToString()) == 1)
                {
                    psOperAgrupacion = "with rollup";
                }
                else
                {
                    psOperAgrupacion = "with cube";
                }
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRep}"]).Rows[0];
                pDSCampos = ReporteEstandarUtil.GetCamposResumido(iCodReporte);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                pKDBRowDataSource = pKDB.GetHisRegByEnt("DataSourceRepMat", "DataSource Reportes", "iCodCatalogo = " + pKDBRowReporte["{DataSourceRepMat}"]).Rows[0];
                pDSCampos = ReporteEstandarUtil.GetCamposMatricial(iCodReporte);
            }
            ReporteEstandarUtil.GetCamposGr(pDSCampos, iCodReporte);

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + " = new RepEstandar('#" + this.ClientID + "'," + iCodReporte + ");");
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteEstandar), this.ID + "New", lsb.ToString(), true, false);
        }

        protected void InitRepRutas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("{Aplic} = " + iCodConsulta);
            lsb.AppendLine("and {EstadoConsulta} = " + iCodEstadoConsulta);
            lsb.AppendLine("and {Perfil} = " + iCodPerfil);
            lsb.AppendLine("and {Atrib} is not null");
            lsb.AppendLine("and {RepEst} is not null");
            lsb.AppendLine("and (isnull({Ruta},0) = " + iCodRutaConsulta + " or {Consul} is not null)");
            pKDBRelRutas = pKDB.GetRelRegByDes("Aplicación - Estado - Perfil - Atributo - Consulta - Reporte", lsb.ToString());

            lsb.Length = 0;
            lsb.AppendLine("select ValorConsulta.*, Ruta = isnull(Rel.Ruta,0)");
            lsb.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Consul','Valor Consulta','" + Globals.GetCurrentLanguage() + "')] ValorConsulta,");
            lsb.AppendLine(DSODataContext.Schema + ".[VisRelaciones('Aplicación - Estado - Perfil - Atributo - Consulta - Reporte','" + Globals.GetCurrentLanguage() + "')] Rel");
            lsb.AppendLine("where ValorConsulta.dtIniVigencia <= '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and ValorConsulta.dtFinVigencia > '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and ValorConsulta.iCodCatalogo = Rel.Consul");
            lsb.AppendLine("and Rel.dtIniVigencia <= '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Rel.dtFinVigencia > '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Rel.Aplic = " + iCodConsulta);
            lsb.AppendLine("and Rel.EstadoConsulta = " + iCodEstadoConsulta);
            lsb.AppendLine("and Rel.Perfil = " + iCodPerfil);
            lsb.AppendLine("and Rel.Atrib is not null");
            lsb.AppendLine("and Rel.Consul is not null");
            lsb.AppendLine("and Rel.RepEst is not null");
            lsb.AppendLine("and ValorConsulta.Atrib is not null");
            lsb.AppendLine("and ((exists(select Cat.iCodRegistro from [" + DSODataContext.Schema + "].Catalogos Cat");
            lsb.AppendLine("            where Cat.iCodRegistro = ValorConsulta.Valores");
            lsb.AppendLine("            and Cat.iCodCatalogo = ValorConsulta.Atrib))");
            lsb.AppendLine("    or (ValorConsulta.Valores is null");
            lsb.AppendLine("        and (ValorConsulta.Value is not null");
            lsb.AppendLine("            or ValorConsulta.FloatValue is not null");
            lsb.AppendLine("            or ValorConsulta.DateValue is not null");
            lsb.AppendLine("            or ValorConsulta.VarCharValue is not null)))");
            lsb.AppendLine("order by Ruta");

            pVisConsultas = DSODataAccess.Execute(lsb.ToString());
        }

        protected void InitRepAplicaciones()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("select Aplic.*,");
            lsb.AppendLine("    DescAplicacion = Idioma.[" + Globals.GetCurrentLanguage() + "]");
            lsb.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Aplic','Aplicaciones del Sistema','" + Globals.GetCurrentLanguage() + "')] Aplic");
            lsb.AppendLine("INNER JOIN " + DSODataContext.Schema + ".[VisRelaciones('Reporte - Perfil - Aplicacion','" + Globals.GetCurrentLanguage() + "')] RelAplic");
            lsb.AppendLine("    on Aplic.iCodCatalogo = RelAplic.Aplic");
            lsb.AppendLine("    and RelAplic.dtIniVigencia <> RelAplic.dtFinVigencia");
            lsb.AppendLine("    and RelAplic.RepEst = " + iCodReporte);
            lsb.AppendLine("    and RelAplic.Perfil = " + iCodPerfil);
            lsb.AppendLine("    and RelAplic.dtIniVigencia >= Aplic.dtIniVigencia");
            lsb.AppendLine("    and RelAplic.dtIniVigencia < Aplic.dtFinVigencia");
            lsb.AppendLine("LEFT JOIN " + DSODataContext.Schema + ".[VisHistoricos('AplicIdioma','Idioma','" + Globals.GetCurrentLanguage() + "')] Idioma");
            lsb.AppendLine("    on Idioma.iCodCatalogo = RelAplic.AplicIdioma");
            lsb.AppendLine("    and Idioma.dtIniVigencia <> Idioma.dtFinVigencia");
            lsb.AppendLine("    and RelAplic.dtIniVigencia >= Idioma.dtIniVigencia");
            lsb.AppendLine("    and RelAplic.dtIniVigencia < Idioma.dtFinVigencia");
            lsb.AppendLine("where Aplic.dtIniVigencia <= '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Aplic.dtFinVigencia > '" + pKDB.FechaVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Aplic.URL like '~/UserInterface/Historicos/Historicos.aspx%'");

            pVisAplicaciones = DSODataAccess.Execute(lsb.ToString());
        }

        protected virtual void InitAcciones()
        {
            pToolBar.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";

            pToolBar.Controls.Add(pbtnRegresar);

            pbtnRegresar.ID = "btnRegresar";
            pbtnRegresar.Attributes["class"] = "buttonBack";
            pbtnRegresar.Style["display"] = "none";

            if (pPostRegresarClick == null)
            {
                pbtnRegresar.Visible = false;
            }
            else
            {
                pbtnRegresar.Visible = true;
                pbtnRegresar.ServerClick += new EventHandler(pbtnRegresar_ServerClick);
            }

            if (pbAreaParametros || pbParamFecIniFin)
            {
                pbtnParametros = new HtmlButton();
                pbtnParametros.ID = "btnParametros";
                pbtnParametros.Attributes["class"] = "buttonPlay";
                pbtnParametros.Style["display"] = "none";
                pbtnParametros.ServerClick += new EventHandler(pbtnParametros_ServerClick);

                pToolBar.Controls.Add(pbtnParametros);
            }

            /*RZ.20130208 Se crean instancias a los objetos tipo HtmlButton*/
            pbtnRepTelFijaDelloite = new HtmlButton();
            pbtnRepTelFijaMeta = new HtmlButton();
            pbtnRepTelFijaTSystems = new HtmlButton();

            /*RZ.20130208 Se agregan atributos a botones para reportes de cliente Modelo*/
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

            /*RZ.20130214 Dejar ocultos los botones por default, solo quedaran activos para Modelo*/
            pbtnRepTelFijaMeta.Visible = false;
            pbtnRepTelFijaTSystems.Visible = false;
            pbtnRepTelFijaDelloite.Visible = false;

            //RZ.20130208 Dejar visible boton solo a usuario de esquema Modelo
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

            /* NZ Se agrega instancia de la creacion del boton de exportar. */
            pbtnExportXLS = new HtmlButton();

            //NZ Se agregan los atributos.
            pbtnExportXLS.ID = "btnExportXLS";
            pbtnExportXLS.Attributes["class"] = "button";
            pbtnExportXLS.ServerClick += new EventHandler(pbtnExportXLS_ServerClick);
            pToolBar.Controls.Add(pbtnExportXLS);

            //Agregar acciones a toolbar en base a aplicaciones configuradas
            HtmlButton lbtnAplic;
            pHTbtnAplic = new Hashtable();
            foreach (DataRow lRow in pVisAplicaciones.Rows)
            {
                if (!pHTbtnAplic.ContainsKey((int)lRow["iCodCatalogo"]))
                {
                    lbtnAplic = new HtmlButton();
                    lbtnAplic.ID = "btnAplic" + lRow["iCodCatalogo"].ToString();
                    lbtnAplic.Attributes["class"] = "buttonEdit";
                    lbtnAplic.Attributes["aplic"] = lRow["iCodCatalogo"].ToString();
                    lbtnAplic.Style["display"] = "none";
                    lbtnAplic.ServerClick += new EventHandler(lbtnAplic_ServerClick);

                    pHTbtnAplic.Add((int)lRow["iCodCatalogo"], lbtnAplic);

                    pToolBar.Controls.Add(lbtnAplic);
                }
            }
        }

        //NZ 20160819 Se agrega programacion el evento clic del botón exportar para todos los reportes nativos de Keytia.
        protected void pbtnExportXLS_ServerClick(object sender, EventArgs e)
        {
            ExportXLS();
        }

        protected void lbtnAplic_ServerClick(object sender, EventArgs e)
        {
            int liCodSubAplicacion = int.Parse(((HtmlButton)sender).Attributes["aplic"]);
            if (pVisAplicaciones.Select("iCodCatalogo = " + liCodSubAplicacion).Length > 0)
            {
                iCodSubAplicacion = liCodSubAplicacion;
                InitAplicacion();
                SetReportState(ReportState.Aplicacion);
            }
        }

        /*RZ.20130208 Se agrega metodo para el evento click del boton, lo que hara que se redireccione 
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


        protected virtual void InitParametros()
        {
            if (!pbAreaParametros)
            {
                pExpParametros.Visible = false;
                return;
            }

            psdoPostBackParametros = Page.ClientScript.GetPostBackEventReference(this, "ConfigurarParametros");

            pExpParametros.ID = "ParamWrapper";
            pExpParametros.StartOpen = false; //(iEstadoConsulta == 0);
            pExpParametros.CreateControls();
            pExpParametros.Panel.CssClass += " ZonaParametros";
            pExpParametros.Panel.Controls.Clear();
            pExpParametros.Panel.Controls.Add(pTablaParametros);
            pExpParametros.Visible = pbAreaParametros;
            pExpParametros.Panel.Width = Unit.Percentage(100);//NZ 20180712 Unit.Pixel(1220);
            //pExpParametros.OnOpen = "function(){" + pjsObj + ".fnInitGridsParam();}";
            pExpParametros.OnOpen = "function(){" + psdoPostBackParametros + "}";

            pTablaParametros.Controls.Clear();
            pTablaParametros.ID = "Parametros";
            pTablaParametros.Width = Unit.Percentage(100);

            pFields = new ReportFieldCollection(this, iCodConsulta, iCodReporte, pTablaParametros, this.ValidarPermiso);
            pFields.InitFields();

            InitValoresSession();
            pFields.IniVigencia = (DateTime)Session["FechaIniRep"];
            pFields.FinVigencia = ((DateTime)Session["FechaFinRep"]).AddDays(1);
            pFields.FillControls();
            pFields.FillAjaxControls();

            if (pFields.ContainsConfigName("FechaIniRep"))
            {
                pdtInicio = (DSODateTimeBox)pFields.GetByConfigName("FechaIniRep").DSOControlDB;
            }
            if (pFields.ContainsConfigName("FechaFinRep"))
            {
                pdtFin = (DSODateTimeBox)pFields.GetByConfigName("FechaFinRep").DSOControlDB;
            }
            if (pFields.ContainsConfigName("NumRegReporte"))
            {
                piNumReg = (DSONumberEdit)pFields.GetByConfigName("NumRegReporte").DSOControlDB;
            }
        }

        protected virtual void InitHeader()
        {
            pTablaHeader.CssClass = "RepHeader";
            pHeader.Controls.Add(pTablaHeader);

            if (pbParamFecIniFin)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaIniRep"))
                {
                    pdtInicio = new DSODateTimeBox();
                    pdtInicio.ID = "FechaIniRep";
                    pdtInicio.Row = 1;
                    pdtInicio.Table = pTablaHeader;
                    pdtInicio.ShowHour = false;
                    pdtInicio.ShowMinute = false;
                    pdtInicio.ShowSecond = false;
                    pdtInicio.CreateControls();
                    pdtInicio.AddClientEvent("keytiaField", "FechaIniRep");
                }

                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaFinRep"))
                {
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
            }
            if (pbParamNumReg)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("NumRegReporte"))
                {
                    piNumReg = new DSONumberEdit();
                    piNumReg.ID = "NumRegReporte";
                    piNumReg.Row = 1;
                    piNumReg.Table = pTablaHeader;
                    piNumReg.FormatInfo = pIntegerFormatField.FormatInfo;
                    piNumReg.NumberDigits = pIntegerFormatField.NumberDigits;
                    piNumReg.Padding = pIntegerFormatField.Padding;
                    piNumReg.CreateControls();
                    piNumReg.AddClientEvent("keytiaField", "NumRegReporte");
                }
            }
        }

        public static void InitValoresSession()
        {
            if (HttpContext.Current.Session["FechaIniRep"] == null)
            {
                int liTipoFechasDefault = 1;
                int liDiaLimiteDefault = 0;
                //RZ.20131230
                int liDiaInicioPeriodo = 1;
                DateTime ldtMaxDetalle;
                DateTime ldtFechaIniRep;
                DateTime ldtFechaFinRep;
                DataTable lDataTable;
                StringBuilder lsbQuery = new StringBuilder();


                //20140413.RJ CAMBIO LA CONSULTA PARA UTILIZAR DIRECTAMENTE LA TABLA DETALLADOS EN LUGAR DE LA VISTA
                //lsbQuery.AppendLine("select Fecha = isnull(max(FechaInicio),GetDate()) from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','" + Globals.GetCurrentLanguage() + "')] Detall");
                if (DSODataContext.Schema != "SeeYouOn")
                {
                    lsbQuery.AppendLine("select Fecha = isnull(MAX(DATE01),GetDate()) from " + DSODataContext.Schema + ".Detallados where icodMaestro = 89");
                }
                else
                {
                    lsbQuery.AppendLine("select Fecha = isnull(MAX(DATE01),GetDate()) from " + DSODataContext.Schema + ".Detallados where icodMaestro = 200011 /*DetalleMCU*/");
                }
                ldtMaxDetalle = (DateTime)DSODataAccess.ExecuteScalar(lsbQuery.ToString());

                //RZ.20131230 Se agrega DiaInicioPeriodo para nuevo tipo de fecha default
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select FechasDefault = isnull(FechasDefault,1), DiaLimiteDefault = isnull(DiaLimiteDefault,0), ");
                lsbQuery.AppendLine("DiaInicioPeriodo = isnull(DiaInicioPeriodo,1) from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
                lsbQuery.AppendLine("where Empre.iCodCatalogo = (select Usuar.Empre from " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] Usuar");
                lsbQuery.AppendLine("   where Usuar.iCodCatalogo = " + HttpContext.Current.Session["iCodUsuario"].ToString());
                lsbQuery.AppendLine("   and Usuar.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("   and Usuar.dtFinVigencia > GetDate())");
                lsbQuery.AppendLine("and Empre.dtIniVigencia <= GetDate()");
                lsbQuery.AppendLine("and Empre.dtFinVigencia > GetDate()");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                if (lDataTable.Rows.Count > 0)
                {
                    liTipoFechasDefault = (int)lDataTable.Rows[0]["FechasDefault"];
                    liDiaLimiteDefault = (int)lDataTable.Rows[0]["DiaLimiteDefault"];
                    liDiaInicioPeriodo = (int)lDataTable.Rows[0]["DiaInicioPeriodo"];
                }

                //RZ.20131230 Saber si la fecha elegida es por periodo
                if (liTipoFechasDefault == 3)
                {
                    //Calcular las fechas en base a periodo
                    //Si la el dia maximo de detalle es mayor al dia de inicio de periodo entonces
                    //la fecha inicio sera el dia de inicio de periodo
                    if (ldtMaxDetalle.Day >= liDiaInicioPeriodo)
                    {
                        ldtFechaIniRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, liDiaInicioPeriodo);

                    }
                    else
                    {
                        ldtFechaIniRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, liDiaInicioPeriodo);
                        ldtFechaIniRep = ldtFechaIniRep.AddMonths(-1);
                    }

                    ldtFechaFinRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, ldtMaxDetalle.Day);
                }
                else
                {
                    if (liTipoFechasDefault == 1)
                    {
                        ldtFechaIniRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, ldtMaxDetalle.Day);
                        ldtFechaIniRep = ldtFechaIniRep.AddMonths(-1);
                        ldtFechaIniRep = ldtFechaIniRep.AddDays(1);

                        ldtFechaFinRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, ldtMaxDetalle.Day);
                    }
                    else if (ldtMaxDetalle.Day > liDiaLimiteDefault)
                    {
                        ldtFechaIniRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, 1);
                        ldtFechaFinRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, ldtMaxDetalle.Day);
                    }
                    else
                    {
                        ldtFechaIniRep = new DateTime(ldtMaxDetalle.Year, ldtMaxDetalle.Month, 1);
                        ldtFechaFinRep = ldtFechaIniRep.AddDays(-1);
                        ldtFechaIniRep = ldtFechaIniRep.AddMonths(-1);
                    }
                }

                HttpContext.Current.Session["FechaIniRep"] = ldtFechaIniRep;
                HttpContext.Current.Session["FechaFinRep"] = ldtFechaFinRep;
            }
            if (HttpContext.Current.Session["NumRegReporte"] == null)
            {
                HttpContext.Current.Session["NumRegReporte"] = 10;
            }
        }

        protected virtual void InitParamSession()
        {

            InitValoresSession();

            if (pdtInicio != null)
            {

                //RJ.20160707 Se solicita mostrar sólo el año actual y dos previos
                pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);
                pdtFin.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);

                pdtInicio.MaxDateTime = DateTime.Today;

                if (!pdtInicio.HasValue)
                {
                    pdtInicio.DataValue = Session["FechaIniRep"];
                }
                else if ((DateTime)Session["FechaIniRep"] != pdtInicio.Date)
                {
                    pdtInicio.DataValue = Session["FechaIniRep"];
                    //HTParam = null;
                    bConfiguraGrid = true;
                }
            }

            if (pdtFin != null)
            {
                pdtFin.MaxDateTime = DateTime.Today;

                if (!pdtFin.HasValue)
                {
                    pdtFin.DataValue = Session["FechaFinRep"];
                }
                else if ((DateTime)Session["FechaFinRep"] != pdtFin.Date)
                {
                    pdtFin.DataValue = Session["FechaFinRep"];
                    //HTParam = null;
                    bConfiguraGrid = true;
                }
            }

            if (piNumReg != null)
            {
                if (!piNumReg.HasValue)
                {
                    piNumReg.DataValue = Session["NumRegReporte"];
                }
                else if ((int)Session["NumRegReporte"] != int.Parse(piNumReg.DataValue.ToString()))
                {
                    piNumReg.DataValue = Session["NumRegReporte"];
                    //HTParam = null;
                    bConfiguraGrid = true;
                }
            }

            InitHTParamSession();
        }

        protected virtual void pbtnParametros_ServerClick(object sender, EventArgs e)
        {
            if (State == ReportState.ConfigurandoParametros)
            {
                HTParam = null;
            }
            TitleRuta = null;
            RemoverReporte = false;
            if (pdtInicio != null && pdtInicio.HasValue)
            {
                Session["FechaIniRep"] = pdtInicio.Date;
            }
            else if (pdtInicio != null)
            {
                pdtInicio.DataValue = Session["FechaIniRep"];
            }

            if (pdtFin != null && pdtFin.HasValue)
            {
                Session["FechaFinRep"] = pdtFin.Date;
            }
            else if (pdtFin != null)
            {
                pdtFin.DataValue = Session["FechaFinRep"];
            }

            if (piNumReg != null && piNumReg.HasValue && int.Parse(piNumReg.DataValue.ToString()) > 0)
            {
                Session["NumRegReporte"] = int.Parse(piNumReg.DataValue.ToString());
            }
            else if (piNumReg != null)
            {
                piNumReg.DataValue = Session["NumRegReporte"];
            }

            if (pbAreaReporte)
            {
                bConfiguraGrid = true;
                pGridReporte.TxtState.Text = "";
            }
            if (pbAreaParametros)
            {
                pExpParametros.StartOpen = false;
                pExpParametros.TextOptions.Text = "";
            }

            InitHTParamSession();

            SetReportState(ReportState.Default);
        }

        protected virtual void pbtnRegresar_ServerClick(object sender, EventArgs e)
        {
            FirePostRegresarClick();
        }

        protected virtual void InitZonas()
        {
            pZonasReporte.CssClass = "Zonas";
            pZonasReporte.Controls.Add(pPrimeraZona);

            //Si tengo las dos zonas
            if ((pbAreaGrafica || pbAreaGraficaHis) && pbAreaReporte)
            {
                pZonasReporte.Controls.Add(pSegundaZona);

                if (pOrdenZonas == OrdenZonasReporte.PrimeroGraficas)
                {
                    pPrimeraZona.CssClass = "ZonaGraficas";
                    if (pbAreaGrafica)
                    {
                        pPrimeraZona.Controls.Add(pPanelGrafica);
                    }
                    if (pbAreaGraficaHis)
                    {
                        pPrimeraZona.Controls.Add(pPanelGraficaHis);
                    }

                    pSegundaZona.CssClass = "ZonaReporte";
                    pSegundaZona.Controls.Add(pExpReporte);
                }
                else if (pOrdenZonas == OrdenZonasReporte.PrimeroReportes)
                {
                    pPrimeraZona.CssClass = "ZonaReporte";
                    pPrimeraZona.Controls.Add(pExpReporte);

                    pSegundaZona.CssClass = "ZonaGraficas";
                    if (pbAreaGrafica)
                    {
                        pSegundaZona.Controls.Add(pPanelGrafica);
                    }
                    if (pbAreaGraficaHis)
                    {
                        pSegundaZona.Controls.Add(pPanelGraficaHis);
                    }
                }
            }
            else if ((pbAreaGrafica || pbAreaGraficaHis) && !pbAreaReporte)
            {
                pPrimeraZona.CssClass = "ZonaGraficas";
                if (pbAreaGrafica)
                {
                    pPrimeraZona.Controls.Add(pPanelGrafica);
                }
                if (pbAreaGraficaHis)
                {
                    pPrimeraZona.Controls.Add(pPanelGraficaHis);
                }
            }
            else
            {
                pPrimeraZona.CssClass = "ZonaReporte";
                pPrimeraZona.Controls.Add(pExpReporte);
            }

            pPrimeraZona.CssClass += pOrientacionZonas.ToString();
            pSegundaZona.CssClass += pOrientacionZonas.ToString();
        }

        protected virtual void InitGrafica()
        {
            if (!pbAreaGrafica)
            {
                return;
            }
            pPanelGrafica.CssClass = "PanelGrafica" + pOrientacionZonas.ToString();
            pPanelGrafica.Controls.Add(pExpGrafica);

            pExpGrafica.ID = "GraficaWrapper";
            pExpGrafica.StartOpen = true;
            pExpGrafica.CreateControls();
            pExpGrafica.Panel.Controls.Clear();
            pExpGrafica.Panel.Controls.Add(pGrafica);

            InitChartSize(pPanelGrafica, pGrafica, State);

            pGrafica.CreateControls();
            pGrafica.Click += new ImageMapEventHandler(pGrafica_Click);

            pOpcTipoGrafica = new KeytiaFlagOptionField();
            pOpcTipoGrafica.Container = this;
            pOpcTipoGrafica.ValidarPermiso = this.ValidarPermiso;
            pOpcTipoGrafica.Column = "RepTipoGrafica";
            pOpcTipoGrafica.ConfigValue = (int)pKDB.GetHisRegByCod("Atrib", new string[] { "RepTipoGrafica" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"];
            pOpcTipoGrafica.iCodEntidad = iCodReporte;
            pOpcTipoGrafica.CreateField();

            pExpGrafica.Panel.Controls.Add(pOpcTipoGrafica.DSOControlDB);
            pOpcTipoGrafica.InitField();
            pOpcTipoGrafica.Fill();

            ((DSORadioButtonList)pOpcTipoGrafica.DSOControlDB).RadioButtonList.AutoPostBack = true;
            if (!pOpcTipoGrafica.DSOControlDB.HasValue)
            {
                if (pKDBRowReporte["{RepTipoGrafica}"] != DBNull.Value)
                {
                    pOpcTipoGrafica.DataValue = (int)pKDBRowReporte["{RepTipoGrafica}"];
                }
                else
                {
                    pOpcTipoGrafica.DataValue = (int)TipoGrafica.Pastel;
                }
            }
        }

        protected virtual void InitGraficaHis()
        {
            if (!pbAreaGraficaHis)
            {
                return;
            }
            pPanelGraficaHis.CssClass = "PanelGraficaHis" + pOrientacionZonas.ToString();
            pPanelGraficaHis.Controls.Add(pExpGraficaHis);

            pExpGraficaHis.ID = "GraficaHisWrapper";
            pExpGraficaHis.StartOpen = true;
            pExpGraficaHis.CreateControls();
            pExpGraficaHis.Panel.Controls.Clear();
            pExpGraficaHis.Panel.Controls.Add(pGraficaHis);

            InitChartSize(pPanelGraficaHis, pGraficaHis, State);

            pGraficaHis.CreateControls();
            pGraficaHis.Click += new ImageMapEventHandler(pGraficaHis_Click);

            pOpcTipoGraficaHis = new KeytiaFlagOptionField();
            pOpcTipoGraficaHis.Container = this;
            pOpcTipoGraficaHis.ValidarPermiso = this.ValidarPermiso;
            pOpcTipoGraficaHis.Column = "RepTipoGraficaHis";
            pOpcTipoGraficaHis.ConfigValue = (int)pKDB.GetHisRegByCod("Atrib", new string[] { "RepTipoGraficaHis" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"];
            pOpcTipoGraficaHis.iCodEntidad = iCodReporte;
            pOpcTipoGraficaHis.CreateField();

            pExpGraficaHis.Panel.Controls.Add(pOpcTipoGraficaHis.DSOControlDB);
            pOpcTipoGraficaHis.InitField();
            pOpcTipoGraficaHis.Fill();

            ((DSORadioButtonList)pOpcTipoGraficaHis.DSOControlDB).RadioButtonList.AutoPostBack = true;
            if (!pOpcTipoGraficaHis.DSOControlDB.HasValue)
            {
                if (pKDBRowReporte["{RepTipoGraficaHis}"] != DBNull.Value)
                {
                    pOpcTipoGraficaHis.DataValue = (int)pKDBRowReporte["{RepTipoGraficaHis}"];
                }
                else
                {
                    pOpcTipoGraficaHis.DataValue = (int)TipoGrafica.Pastel;
                }
            }
        }

        protected virtual void InitChartSize(Panel lPanelGrafica, DSOChart lGrafica, ReportState lState)
        {
            if (lState == ReportState.Default)
            {
                lPanelGrafica.Width = Unit.Pixel(piWidthGrHorizontal);
                lGrafica.Width = Unit.Pixel(piWidthGrHorizontal);
                lGrafica.Height = Unit.Pixel(piHeightGrVertical);
                if (pOrientacionZonas == Orientation.Vertical)
                {
                    if (!pbAreaGraficaHis)
                    {
                        lPanelGrafica.Width = Unit.Pixel(piWidth1GrVertical);
                        lGrafica.Width = Unit.Pixel(piWidth1GrVertical);
                    }
                }
                else if (pOrientacionZonas == Orientation.Horizontal)
                {
                    if (pbAreaGraficaHis)
                    {
                        lGrafica.Height = Unit.Pixel(piHeight2GrHorizontal);
                    }
                    else
                    {
                        lGrafica.Height = Unit.Pixel(piHeight1GrHorizontal);
                    }
                    if (!pbAreaReporte)
                    {
                        lPanelGrafica.Width = Unit.Pixel(piWidth1GrVertical);
                        lGrafica.Width = Unit.Pixel(piWidth1GrVertical);
                    }
                }
            }
            else if (lState == ReportState.DashboardGrafica
                || lState == ReportState.DashboardGraficaHis)
            {
                lPanelGrafica.Width = Unit.Pixel(piWidthDashboardGr);
                lGrafica.Width = Unit.Pixel(piWidthDashboardGr);
                lGrafica.Height = Unit.Pixel(piHeightDashboardGr);
            }
            else if (lState == ReportState.DashboardGrafica2Bloques
                || lState == ReportState.DashboardGraficaHis2Bloques)
            {
                lPanelGrafica.Width = Unit.Pixel(piWidthDashboardGr2Bloques);
                lGrafica.Width = Unit.Pixel(piWidthDashboardGr2Bloques);
                lGrafica.Height = Unit.Pixel(piHeightDashboardGr);
            }
        }

        protected virtual void InitReporte()
        {
            if (!pbAreaReporte)
            {
                return;
            }

            pExpReporte.ID = "ReporteWrapper";
            pExpReporte.StartOpen = true;
            pExpReporte.CreateControls();
            pExpReporte.Panel.Controls.Clear();
            pExpReporte.Panel.Controls.Add(pGridReporte);

            pGridReporte.ID = "GridReporte";
            pGridReporte.CreateControls();
            if (bConfiguraGrid)
            {
                pGridReporte.TxtState.Text = "";
            }
        }

        protected virtual void InitSubReporte()
        {
            if (iCodSubReporte > 0)
            {
                pSubReporte = new ReporteEstandar();
                pSubReporte.Reporte = this;
                pSubReporte.ID = "SubR" + iCodSubReporte + "_" + (iNumReporte + 1);
                pSubReporte.OpcMnu = OpcMnu;
                pSubReporte.lblTitle = lblTitle;
                pSubReporte.iCodConsulta = iCodConsulta;
                pSubReporte.iCodReporte = iCodSubReporte;
                //pSubReporte.iCodPerfil = iCodPerfil;
                if (iCodReporte == iCodSubReporte)
                {
                    pSubReporte.iEstadoConsulta = iEstadoConsulta;
                }
                else
                {
                    pSubReporte.iEstadoConsulta = iEstadoConsulta + 1;
                }
                pSubReporte.iNumReporte = iNumReporte + 1;
                pSubReporte.iCodRutaConsulta = iCodSubRutaConsulta;

                pSubReporte.ParentContainer = ParentContainer;

                ParentContainer.Controls.Add(pSubReporte);

                pSubReporte.PostRegresarClick += new EventHandler(pSubReporte_PostRegresarClick);

                pSubReporte.CreateControls();
            }
        }

        protected void pSubReporte_PostRegresarClick(object sender, EventArgs e)
        {
            RemoverReportesRegresar();
        }

        public void RemoverReportesRegresar()
        {
            //remueve los sub reportes desde el botón de regresar

            RemoverSubReporte();
            if (pReporte != null && RemoverReporte)
            {
                RemoverReporte = false;
                pReporte.RemoverReportesRegresar();
            }
        }

        public void RemoverSubReporte()
        {
            if (iCodSubReporte > 0)
            {
                if (pdtInicio != null && pSubReporte.dtInicio != null
                    && pSubReporte.dtInicio.HasValue
                    && pdtInicio.Date != pSubReporte.dtInicio.Date)
                {
                    //HTParam = null;
                    if (pbAreaReporte)
                    {
                        bConfiguraGrid = true;
                        pGridReporte.TxtState.Text = "";
                    }
                    Session["FechaIniRep"] = pSubReporte.dtInicio.Date;
                    pdtInicio.DataValue = pSubReporte.dtInicio.Date;
                }
                if (pdtFin != null && pSubReporte.dtFin != null
                    && pSubReporte.dtFin.HasValue
                    && pdtFin.Date != pSubReporte.dtFin.Date)
                {
                    //HTParam = null;
                    if (pbAreaReporte)
                    {
                        bConfiguraGrid = true;
                        pGridReporte.TxtState.Text = "";
                    }
                    Session["FechaFinRep"] = pSubReporte.dtFin.Date;
                    pdtFin.DataValue = pSubReporte.dtFin.Date;
                }
                if (piNumReg != null && pSubReporte.iNumReg != null
                    && pSubReporte.iNumReg.HasValue
                    && int.Parse(piNumReg.DataValue.ToString()) != int.Parse(pSubReporte.iNumReg.DataValue.ToString()))
                {
                    //HTParam = null;
                    if (pbAreaReporte)
                    {
                        bConfiguraGrid = true;
                        pGridReporte.TxtState.Text = "";
                    }
                    Session["NumRegReporte"] = int.Parse(pSubReporte.iNumReg.DataValue.ToString());
                    piNumReg.DataValue = pSubReporte.iNumReg.DataValue;
                }

                InitHTParamSession();

                pSubReporte.RemoverSubReporte();

                iCodSubReporte = 0;
                ParentContainer.Controls.Remove(pSubReporte);
                SetReportState(ReportState.Default);
                pSubReporte = null;
            }
            if (iCodSubAplicacion > 0)
            {
                pSubHistorico.RemoverSubHistorico();

                iCodSubAplicacion = 0;
                ParentContainer.Controls.Remove(pSubHistorico);
                SetReportState(ReportState.Default);
                pSubHistorico = null;
            }
        }

        protected void ConfigurarParametros()
        {
            SetReportState(ReportState.ConfigurandoParametros);
        }

        protected virtual void InitAplicacion()
        {
            if (iCodSubAplicacion > 0)
            {
                DataRow lRowAplic = pVisAplicaciones.Select("iCodCatalogo = " + iCodSubAplicacion)[0];
                if (lRowAplic["ParamVarChar3"] != DBNull.Value)
                {
                    pSubHistorico = (HistoricEdit)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricEdit)).CodeBase, lRowAplic["ParamVarChar3"].ToString()).Unwrap();
                }
                else
                {
                    pSubHistorico = new HistoricEdit();
                }
                if (lRowAplic["ParamVarChar4"] != DBNull.Value)
                {
                    pSubHistorico.CollectionClass = lRowAplic["ParamVarChar4"].ToString();
                }
                if (lRowAplic["ParamVarChar1"] != DBNull.Value)
                {
                    pSubHistorico.SetEntidad(lRowAplic["ParamVarChar1"].ToString());

                    if (lRowAplic["ParamVarChar2"] != DBNull.Value)
                    {
                        pSubHistorico.SetMaestro(lRowAplic["ParamVarChar2"].ToString());
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

        protected void pSubHistorico_PostCancelarClick(object sender, EventArgs e)
        {
            RemoverSubReporte();
        }


        /// <summary>
        /// Genera la pantalla en donde se configurará el envío del reporte
        /// </summary>
        protected void InitEnvioCorreo()
        {
            //Valida que la extensión del archivo contenga algún valor (xlsx, docx, dpf, csv)
            if (!String.IsNullOrEmpty(ExportExt))
            {
                pWdCorreo = new DSOWindow();
                pLblEnviaCorreo = new Label();
                pTablaCorreo = new Table();
                pTxtPara = new DSOTextBox();
                pbtnEnviaCorreo = new HtmlButton();
                pbtnCancelaCorreo = new HtmlButton();

                Controls.Add(pWdCorreo);

                pWdCorreo.ID = "wdCorreo";
                pWdCorreo.Width = 900;
                pWdCorreo.PositionLeft = 100;
                pWdCorreo.PositionTop = 100;
                pWdCorreo.Modal = true;
                pWdCorreo.InitOnReady = false;
                pWdCorreo.OnWindowClose = "function(){" + pjsObj + ".cancelarCorreo();}";
                pWdCorreo.CreateControls();
                pWdCorreo.Content.Controls.Add(pLblEnviaCorreo);
                pWdCorreo.Content.Controls.Add(pTablaCorreo);
                pWdCorreo.Content.Controls.Add(pbtnEnviaCorreo);
                pWdCorreo.Content.Controls.Add(pbtnCancelaCorreo);

                pTablaCorreo.ID = "tblCorreo";
                pTablaCorreo.Controls.Clear();

                pTablaCorreo.Width = Unit.Percentage(100);

                pTxtPara.ID = "txtPara";
                pTxtPara.DataField = "txtPara";
                pTxtPara.Table = pTablaCorreo;
                pTxtPara.Row = 1;
                pTxtPara.CreateControls();

                pbtnEnviaCorreo.ID = "btnEnviaCorreo";
                pbtnEnviaCorreo.Attributes["class"] = "buttonOK";
                pbtnEnviaCorreo.Style["display"] = "none";
                pbtnEnviaCorreo.ServerClick += new EventHandler(pbtnEnviaCorreo_ServerClick);

                pbtnCancelaCorreo.ID = "btnCancelaCorreo";
                pbtnCancelaCorreo.Attributes["class"] = "buttonCancel";
                pbtnCancelaCorreo.Style["display"] = "none";
                pbtnCancelaCorreo.ServerClick += new EventHandler(pbtnCancelaCorreo_ServerClick);

                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".$btnCancelaCorreo = $('#" + pbtnCancelaCorreo.ClientID + "');");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(ReporteEstandar), pjsObj + ".$btnCancelaCorreo", lsb.ToString(), true, false);
            }
        }

        protected void pbtnEnviaCorreo_ServerClick(object sender, EventArgs e)
        {
            if (pTxtPara.HasValue && !pTxtPara.TextBox.Text.TrimEnd().EndsWith(";"))
            {
                pTxtPara.TextBox.Text = pTxtPara.TextBox.Text.TrimEnd() + ";";
            }
            pTxtPara.TextBox.Text = pTxtPara.TextBox.Text.Replace(",", ";");
            if (!pTxtPara.HasValue || !Regex.IsMatch(pTxtPara.TextBox.Text, @"^([a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}\s*;\s*)+$", RegexOptions.IgnoreCase))
            {
                DSOControl.jAlert(Page, pjsObj + ".ValidarListaCorreos", DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidarListaCorreosRepEst")), DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloValidarListaCorreosRepEst")));
            }
            else
            {
                InitHTParam();
                InitHTParamDesc();
                string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                //KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

                KeytiaCOM.ICargasCOM lCargasCOM = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
                lCargasCOM.EnviarReporteEstandar(iCodReporte, Util.Ht2Xml(pHTParam), Util.Ht2Xml(pHTParamDesc), lsKeytiaWebFPath, lsStylePath, pTxtPara.TextBox.Text, pKDBRowReporte["{" + Globals.GetCurrentLanguage() + "}"].ToString(), ExportExt, (int)Session["iCodUsuarioDB"]);
                Marshal.ReleaseComObject(lCargasCOM);
                lCargasCOM = null;

                ExportExt = "";
                Controls.Remove(pWdCorreo);
            }
        }

        protected void pbtnCancelaCorreo_ServerClick(object sender, EventArgs e)
        {
            ExportExt = "";
            Controls.Remove(pWdCorreo);
        }

        public virtual void InitLanguage()
        {
            pvchCodIdioma = Globals.GetCurrentLanguage();
            string lsLang = "{" + pvchCodIdioma + "}";
            pCurrencyField.SetFormatInfo(Globals.GetCurrentCurrency());

            if (!String.IsNullOrEmpty(TitleRuta))
            {
                plblTitle.Text = TitleLinks + TitleRuta;
            }
            else
            {
                plblTitle.Text = TitleLinks + Title;
            }

            InitRepAplicaciones(); //Lo vuelvo a llamar para que se llene con las descripciones en el idioma
            HtmlButton lbtnAplic;
            DataRow lRowAplic;
            foreach (int liCodAplic in pHTbtnAplic.Keys)
            {
                lbtnAplic = (HtmlButton)pHTbtnAplic[liCodAplic];
                lRowAplic = pVisAplicaciones.Select("iCodCatalogo = " + liCodAplic)[0];
                if (lRowAplic["DescAplicacion"] != DBNull.Value)
                {
                    lbtnAplic.InnerText = DSOControl.JScriptEncode(lRowAplic["DescAplicacion"].ToString());
                }
                else
                {
                    lbtnAplic.InnerText = DSOControl.JScriptEncode(lRowAplic["vchDescripcion"].ToString());
                }
            }

            if (pbtnRegresar.Visible)
            {
                pbtnRegresar.InnerText = Globals.GetMsgWeb(false, "btnRegresar");
            }

            if (pbAreaParametros)
            {
                pExpParametros.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ParamRep"));
                pExpParametros.ToolTip = Globals.GetMsgWeb(false, "ParamRep");
                pFields.IniVigencia = (DateTime)Session["FechaIniRep"];
                pFields.FinVigencia = ((DateTime)Session["FechaFinRep"]).AddDays(1);
                pFields.FillControls();
                pFields.FillAjaxControls();
                pFields.InitLanguage();

                if (State == ReportState.ConfigurandoParametros)
                {
                    SetReportState(State);
                }
                else
                {
                    pTablaParametros.Visible = false;
                }
            }

            if (pbParamFecIniFin)
            {
                pdtInicio.Descripcion = Globals.GetLangItem("Atrib", "Atributos", "FechaIniRep");
                pdtFin.Descripcion = Globals.GetLangItem("Atrib", "Atributos", "FechaFinRep");
            }

            if (pbParamNumReg)
            {
                piNumReg.Descripcion = Globals.GetLangItem("Atrib", "Atributos", "NumRegReporte");
            }

            if (pbAreaParametros || pbParamFecIniFin)
            {
                pbtnParametros.InnerText = Globals.GetMsgWeb(false, "btnParamRep");
            }

            if (State != ReportState.SubReporte
                && State != ReportState.Aplicacion)
            {
                InitNumRegistros();
                LoadScriptParam();

                FillHeader();

                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iNumeroRegistros = " + NumeroRegistros);
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(ReporteEstandar), pjsObj + ".iNumeroRegistros", lsb.ToString(), true, false);
            }

            if (pbAreaGrafica
                && (State == ReportState.Default
                || State == ReportState.ConfigurandoParametros
                || State == ReportState.DashboardGrafica
                || State == ReportState.DashboardGrafica2Bloques))
            {
                pExpGrafica.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "AreaGrafica"));
                pExpGrafica.ToolTip = Globals.GetMsgWeb(false, "AreaGrafica");
                pOpcTipoGrafica.InitLanguage();
                FillGrafica();
            }

            if (pbAreaGraficaHis
                && (State == ReportState.Default
                || State == ReportState.ConfigurandoParametros
                || State == ReportState.DashboardGraficaHis
                || State == ReportState.DashboardGraficaHis2Bloques))
            {
                pExpGraficaHis.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "AreaGraficaHis"));
                pExpGraficaHis.ToolTip = Globals.GetMsgWeb(false, "AreaGraficaHis");
                pOpcTipoGraficaHis.InitLanguage();
                FillGraficaHis();
            }

            if (pbAreaReporte
                && (State == ReportState.Default
                || State == ReportState.ConfigurandoParametros
                || State == ReportState.DashboardReporte
                || State == ReportState.DashboardReporte2Bloques))
            {
                pExpReporte.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "AreaReporte"));
                pExpReporte.OnOpen = "function(){" + pjsObj + ".fnAdjustColumnSizing('#" + pGridReporte.Grid.ClientID + "');}";

                InitGridHeaders();

                pGridReporte.Config.oLanguage = Globals.GetGridLanguage();

                if (pTipoReporte == TipoReporte.Tabular)
                {
                    InitLangGridTabular();
                }
                else if (pTipoReporte == TipoReporte.Resumido)
                {
                    InitLangGridResumido();
                }
                else if (pTipoReporte == TipoReporte.Matricial)
                {
                    InitLangGridMatricial();
                }
            }

            if (State == ReportState.SubReporte)
            {
                string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "RemoverSubReporte");
                string lsLinkRegresar;
                if (!String.IsNullOrEmpty(TitleRuta))
                {
                    lsLinkRegresar = "<a href=\"javascript:" + lsdoPostBack + "\">" + HttpUtility.HtmlEncode(TitleRuta) + "</a>";
                }
                else
                {
                    lsLinkRegresar = "<a href=\"javascript:" + lsdoPostBack + "\">" + HttpUtility.HtmlEncode(Title) + "</a>";
                }
                pSubReporte.TitleLinks = TitleLinks + lsLinkRegresar + " / ";
                pSubReporte.Title = pKDB.GetHisRegByEnt("RepEst", "", "iCodCatalogo = " + iCodSubReporte).Rows[0][lsLang].ToString();
                pSubReporte.InitLanguage();
            }

            if (State == ReportState.Aplicacion)
            {
                string lsdoPostBack = Page.ClientScript.GetPostBackEventReference(this, "RemoverSubReporte");
                string lsLinkRegresar = "<a href=\"javascript:" + lsdoPostBack + "\">" + HttpUtility.HtmlEncode(Title) + "</a>";
                lRowAplic = pVisAplicaciones.Select("iCodCatalogo = " + iCodSubAplicacion)[0];

                if (lRowAplic["DescAplicacion"] != DBNull.Value)
                {
                    pSubHistorico.Title = TitleLinks + lsLinkRegresar + " / " + lRowAplic["DescAplicacion"];
                    pSubHistorico.AlertTitle = Title + " / " + lRowAplic["DescAplicacion"];
                }
                else
                {
                    pSubHistorico.Title = TitleLinks + lsLinkRegresar + " / " + lRowAplic["vchDescripcion"];
                    pSubHistorico.AlertTitle = Title + " / " + lRowAplic["vchDescripcion"];
                }
                pSubHistorico.InitLanguage();
            }

            if (!String.IsNullOrEmpty(ExportExt))
            {
                pWdCorreo.Title = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "WdCorreoRepEst"));
                pLblEnviaCorreo.Text = Globals.GetMsgWeb(false, "LblEnvioCorreoRepEst");
                pbtnEnviaCorreo.InnerText = DSOControl.JScriptEncode(Globals.GetMsgWeb("btnEnviaCorreoRepEst"));
                pbtnCancelaCorreo.InnerText = DSOControl.JScriptEncode(Globals.GetMsgWeb("btnCancelaCorreoRepEst"));
                pTxtPara.Descripcion = Globals.GetMsgWeb(false, "TxtParaCorreoRepEst");

                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine("DSOControls.Window.Init.call($('#" + pWdCorreo.Content.ClientID + "')[0]);");
                lsb.AppendLine("$('#" + pWdCorreo.Content.ClientID + "').showWindow();");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(ReporteEstandar), pjsObj + ".showWindowCorreo", lsb.ToString(), true, false);
            }

            if (State != ReportState.ConfigurandoParametros)
            {
                pTablaParametros.Controls.Clear();
            }

            /*RZ.20130208 Se extrae el valor de la la propiedad InnerText del boton (titulo del boton)
              desde historico dado de alta en la entidad y maestro Mensajes Web (disponible solo en Modelo)*/
            pbtnRepTelFijaMeta.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaMeta");
            pbtnRepTelFijaDelloite.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaDelloite");
            pbtnRepTelFijaTSystems.InnerText = Globals.GetMsgWeb(false, "DshBtnRepTelFijaTSystems");

            //NZ 20160819 Se extrae el valor de la la propiedad InnerText del boton (titulo del boton)
            pbtnExportXLS.InnerText = Globals.GetMsgWeb(false, "btnExportaXLS");
        }

        protected virtual void FillHeader()
        {
            int liHeaderColumns = 0;
            if (pTablaHeader.Rows.Count > 0)
            {
                liHeaderColumns = pTablaHeader.Rows[0].Cells.Count;
            }

            if (pKDBRowDataSource["{RepEstDataSourceHeader}"] != DBNull.Value)
            {
                string lsDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceHeader}"].ToString(), pHTParam, pbReajustarParam);
                if (pbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceHeader", lsDataSource);
                }
                DataTable ldt = DSODataAccess.Execute(lsDataSource);
                TableRow lTableRow;
                TableCell lTableCell;
                string lsCssClass;
                int liColumnSpan;
                int liRowColumns;
                int liCol;
                if (liHeaderColumns < ldt.Columns.Count)
                {
                    liHeaderColumns = ldt.Columns.Count;
                }

                foreach (DataRow lDataRow in ldt.Rows)
                {
                    lTableRow = new TableRow();
                    liRowColumns = 0;
                    for (liCol = 0; liCol < ldt.Columns.Count; liCol++)
                    {
                        if (lDataRow[liCol] != DBNull.Value)
                        {
                            if ((ldt.Columns.Count > liCol + 1
                                && (liCol + 1) % 2 != 0 && lDataRow[liCol + 1] == DBNull.Value)
                                || ((liCol + 1) % 2 == 0 && lDataRow[liCol - 1] == DBNull.Value))
                            {
                                liColumnSpan = 2;
                            }
                            else
                            {
                                liColumnSpan = 1;
                            }

                            if (liCol == ldt.Columns.Count - 1
                                && liRowColumns + liColumnSpan < liHeaderColumns)
                            {
                                liColumnSpan = liHeaderColumns - liRowColumns;
                            }

                            if ((liCol + 1) % 2 == 0)
                            {
                                lsCssClass = "RepTcDesc Columns" + liHeaderColumns + " ColSpan" + liColumnSpan;
                            }
                            else
                            {
                                lsCssClass = "RepTcLbl Columns" + liHeaderColumns + " ColSpan" + liColumnSpan;
                            }
                            lTableCell = new TableCell();
                            lTableCell.ColumnSpan = liColumnSpan;
                            lTableCell.CssClass = lsCssClass;
                            lTableCell.Text = lDataRow[liCol].ToString();
                            lTableRow.Cells.Add(lTableCell);

                            liRowColumns += liColumnSpan;
                        }
                    }

                    if (lTableRow.Cells.Count > 0)
                    {
                        pTablaHeader.Rows.Add(lTableRow);
                    }
                }
            }

            if (pbParamFecIniFin)
            {
                if (pdtInicio.Table == pTablaHeader)
                {
                    pdtInicio.TcLbl.CssClass = "RepTcLbl Columns" + liHeaderColumns + " ColSpan" + pdtInicio.ColumnSpan;
                    pdtInicio.TcCtl.CssClass = "RepTcDesc Columns" + liHeaderColumns + " ColSpan" + pdtInicio.ColumnSpan;
                }
                if (pdtFin.Table == pTablaHeader)
                {
                    pdtFin.TcLbl.CssClass = "RepTcLbl Columns" + liHeaderColumns + " ColSpan" + pdtInicio.ColumnSpan;
                    pdtFin.TcCtl.CssClass = "RepTcDesc Columns" + liHeaderColumns + " ColSpan" + pdtInicio.ColumnSpan;
                }
            }

            if (pbParamNumReg)
            {
                if (piNumReg.Table == pTablaHeader)
                {
                    piNumReg.TcLbl.CssClass = "RepTcLbl Columns" + liHeaderColumns + " ColSpan" + piNumReg.ColumnSpan;
                }
            }

            if (pTablaHeader.Rows.Count == 0)
            {
                pHeader.Visible = false;
            }
        }

        protected virtual void InitHTParam()
        {
            if (pFields != null)
            {
                pFields.IniVigencia = (DateTime)Session["FechaIniRep"];
                pFields.FinVigencia = ((DateTime)Session["FechaFinRep"]).AddDays(1);
                pFields.FillAjaxControls();
            }

            //Si las variables de las fechas cambiaron por algun otro lugar
            if (pbParamFecIniFin
                && HTParam != null
                && ((DateTime)Session["FechaIniRep"] != pdtInicio.Date
                || (DateTime)Session["FechaFinRep"] != pdtFin.Date))
            {
                HTParam["FechaIniRep"] = Session["FechaIniRep"];
                HTParam["FechaFinRep"] = Session["FechaFinRep"];
            }

            if (pbParamNumReg
                && HTParam != null
                && (int)Session["NumRegReporte"] != int.Parse(piNumReg.DataValue.ToString()))
            {
                HTParam["NumRegReporte"] = Session["NumRegReporte"];
            }

            ReloadHTParam();

            pHTParam = new Hashtable();
            pHTParam.Add("iCodUsuario", Session["iCodUsuario"]);
            pHTParam.Add("iCodPerfil", Session["iCodPerfil"]);
            pHTParam.Add("vchCodIdioma", Globals.GetCurrentLanguage());
            pHTParam.Add("vchCodMoneda", Globals.GetCurrentCurrency());
            pHTParam.Add("Schema", DSODataContext.Schema);

            if (pbAreaParametros)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    pHTParam.Add(lField.ConfigName, lField.DataValue);
                }
            }
            if (pdtInicio != null)
            {
                if (!pHTParam.ContainsKey("FechaIniRep"))
                {
                    pHTParam.Add("FechaIniRep", pdtInicio.DataValue);
                }
            }
            if (pdtFin != null)
            {
                if (!pHTParam.ContainsKey("FechaFinRep"))
                {
                    pHTParam.Add("FechaFinRep", pdtFin.DataValueDelimiter + pdtFin.Date.ToString("yyyy-MM-dd 23:59:59") + pdtFin.DataValueDelimiter);
                }
                else if (pbAreaParametros && pFields.ContainsConfigName("FechaFinRep")
                    && pFields.GetByConfigName("FechaFinRep") is KeytiaDateField)
                {
                    pHTParam["FechaFinRep"] = pdtFin.DataValueDelimiter + pdtFin.Date.ToString("yyyy-MM-dd 23:59:59") + pdtFin.DataValueDelimiter;
                }
            }
            if (piNumReg != null)
            {
                if (!pHTParam.ContainsKey("NumRegReporte"))
                {
                    pHTParam.Add("NumRegReporte", piNumReg.DataValue);
                }
            }

            if (pbAreaGrafica)
            {
                pHTParam.Add("TipoGrafica", (int)pOpcTipoGrafica.DataValue);
            }
            if (pbAreaGraficaHis)
            {
                pHTParam.Add("TipoGraficaHis", (int)pOpcTipoGraficaHis.DataValue);
            }

            AgregarParamDataFields();

            if (HTParam == null)
            {
                HTParam = pHTParam;
            }
        }

        protected virtual void InitHTParamSession()
        {
            //Actualizar HTParam por si se cambiaron las fechas
            if (pbParamFecIniFin
                && HTParam != null)
            {
                HTParam["FechaIniRep"] = Session["FechaIniRep"];
                HTParam["FechaFinRep"] = Session["FechaFinRep"];
            }

            //Actualizar HTParam por si se cambio la cantidad de registros a mostrar
            if (pbParamNumReg
                && HTParam != null)
            {
                HTParam["NumRegReporte"] = Session["NumRegReporte"];
            }
        }

        public virtual void InitNumRegistros()
        {
            InitHTParam();

            if (pbAreaReporte
                && (State == ReportState.Default
                || State == ReportState.ConfigurandoParametros
                || State == ReportState.DashboardReporte
                || State == ReportState.DashboardReporte2Bloques))
            {
                pTablaTotales = null;
                if (pKDBRowDataSource["{RepEstDataSourceTot}"] != DBNull.Value)
                {
                    string lsRepEstDataSourceTot = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceTot}"].ToString(), pHTParam, pbReajustarParam);
                    if (pbModoDebug)
                    {
                        ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceTot", lsRepEstDataSourceTot);
                    }
                    pTablaTotales = DSODataAccess.Execute(lsRepEstDataSourceTot);
                    if (pTablaTotales.Columns.Contains("MaxRowNumber"))
                    {
                        NumeroRegistros = int.Parse(pTablaTotales.Rows[0]["MaxRowNumber"].ToString());
                        return;
                    }
                }

                if (pKDBRowDataSource["{RepEstDataSourceNumReg}"] != DBNull.Value)
                {
                    string lsRepEstDataSourceNumReg = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceNumReg}"].ToString(), pHTParam, pbReajustarParam);
                    if (pbModoDebug)
                    {
                        ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceNumReg", lsRepEstDataSourceNumReg);
                    }
                    NumeroRegistros = int.Parse(Util.IsDBNull(DSODataAccess.ExecuteScalar(lsRepEstDataSourceNumReg), 0).ToString());
                }
                else
                {
                    NumeroRegistros = 0;
                }
            }
        }

        protected virtual void AgregarParamDataFields()
        {
            if (pTipoReporte == TipoReporte.Tabular)
            {
                ReporteEstandarUtil.AgregarParamDataFieldsTabular(pHTParam, pDSCampos);
                ReporteEstandarUtil.AgregarParamGridTabular(pDSCampos, pHTParam);
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                ReporteEstandarUtil.AgregarParamDataFieldsResumido(pHTParam, pDSCampos, psOperAgrupacion);
                ReporteEstandarUtil.AgregarParamGridResumido(pDSCampos, pHTParam);
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                ReporteEstandarUtil.AgregarParamDataFieldsMatricial(pHTParam, pDSCampos);
                ReporteEstandarUtil.GetValoresEjeX(pvchCodIdioma, pKDBRowReporte, pKDBRowDataSource, pDSCampos, pHTParam, pbReajustarParam);
                ReporteEstandarUtil.AgregarParamGridMatricial(pDSCampos, pHTParam);
            }
        }

        protected virtual void LoadScriptParam()
        {
            List<Parametro> lstParams = new List<Parametro>();
            Parametro lParam;

            if (pbAreaParametros)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    lParam = new Parametro();
                    lParam.Name = lField.ConfigName;
                    if (lField.ConfigName == "FechaFinRep"
                        && lField is KeytiaDateField)
                    {
                        lParam.Value = pdtFin.DataValueDelimiter + pdtFin.Date.ToString("yyyy-MM-dd 23:59:59") + pdtFin.DataValueDelimiter;
                    }
                    else
                    {
                        lParam.Value = lField.DataValue;
                    }
                    lstParams.Add(lParam);
                }
            }
            if (pbParamFecIniFin)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaIniRep"))
                {
                    lParam = new Parametro();
                    lParam.Name = "FechaIniRep";
                    lParam.Value = pdtInicio.DataValue;
                    lstParams.Add(lParam);
                }

                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaFinRep"))
                {
                    lParam = new Parametro();
                    lParam.Name = "FechaFinRep";
                    lParam.Value = pdtFin.DataValueDelimiter + pdtFin.Date.ToString("yyyy-MM-dd 23:59:59") + pdtFin.DataValueDelimiter;
                    lstParams.Add(lParam);
                }
            }
            if (pbParamNumReg)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("NumRegReporte"))
                {
                    lParam = new Parametro();
                    lParam.Name = "NumRegReporte";
                    lParam.Value = piNumReg.DataValue;
                    lstParams.Add(lParam);
                }
            }

            if (pbAreaGrafica)
            {
                lParam = new Parametro();
                lParam.Name = "TipoGrafica";
                lParam.Value = (int)pOpcTipoGrafica.DataValue;
            }
            if (pbAreaGraficaHis)
            {
                lParam = new Parametro();
                lParam.Name = "TipoGraficaHis";
                lParam.Value = (int)pOpcTipoGraficaHis.DataValue;
            }

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type='text/javascript'>");
            lsb.AppendLine("jQuery(function($){");
            lsb.AppendLine(pjsObj + ".parametros = " + DSOControl.SerializeJSON<List<Parametro>>(lstParams));
            lsb.AppendLine("});");
            lsb.AppendLine("</script>");
            DSOControl.LoadControlScriptBlock(Page, typeof(ReporteEstandar), pjsObj + ".Parametros", lsb.ToString(), true, false);
        }

        protected virtual void ReloadHTParam()
        {
            if (HTParam == null)
                return;

            if (pbAreaParametros)
            {
                string lsValor;
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField is KeytiaVarcharField
                        || lField is KeytiaMultiSelectField)
                    {
                        lsValor = HTParam[lField.ConfigName].ToString();
                        if (!String.IsNullOrEmpty(lField.DSOControlDB.DataValueDelimiter)
                            && lsValor.ToString().StartsWith(lField.DSOControlDB.DataValueDelimiter)
                            && lsValor.ToString().EndsWith(lField.DSOControlDB.DataValueDelimiter))
                        {
                            lsValor = lsValor.Remove(0, lField.DSOControlDB.DataValueDelimiter.Length);
                            lsValor = lsValor.Remove(lsValor.Length - lField.DSOControlDB.DataValueDelimiter.Length);
                        }
                        if (lField is KeytiaVarcharField && lsValor == "null")
                        {
                            lField.DataValue = DBNull.Value;
                        }
                        else
                        {
                            lField.DataValue = lsValor;
                        }
                    }
                    else
                    {
                        lField.DataValue = HTParam[lField.ConfigName];
                    }
                }
            }

            if (pbParamFecIniFin)
            {
                pdtInicio.DataValue = HTParam["FechaIniRep"];
                pdtFin.DataValue = HTParam["FechaFinRep"];
            }
            if (pbParamNumReg)
            {
                piNumReg.DataValue = HTParam["NumRegReporte"];
            }
        }

        protected virtual void FillGrafica()
        {
            DataSourceGr = FillChart(pGrafica, pOpcTipoGrafica, "");
        }

        protected virtual void FillGraficaHis()
        {
            DataSourceGrHis = FillChart(pGraficaHis, pOpcTipoGraficaHis, "His");
        }

        protected virtual string FillChart(DSOChart lGrafica, KeytiaFlagOptionField lOpcTipoGrafica, string lsHis)
        {
            string lsTitle = "";
            StringBuilder lsbDataFields = new StringBuilder();
            string lsGroupByField = "";

            List<string> lstDataColumns = new List<string>();
            List<string> lstSeriesNames = new List<string>();
            List<string> lstSeriesIds = new List<string>();
            string lsXColumn = null;
            string lsXIdsColumn = null;
            string lsXTitle = "";
            string lsXFormat = null;
            string lsYTitle = "";
            string lsYFormat = null;
            bool lbShowLegend = true;
            DataRow lKDBRowTitulos;
            string lsSerieName;

            if (pKDBRowReporte["{RepEstTitGr" + lsHis + "}"] != DBNull.Value)
            {
                lKDBRowTitulos = pKDB.GetHisRegByEnt("RepEstTitGr" + lsHis, "Idioma", "iCodCatalogo = " + pKDBRowReporte["{RepEstTitGr" + lsHis + "}"]).Rows[0];
                if (lKDBRowTitulos["{" + Globals.GetCurrentLanguage() + "}"] != DBNull.Value)
                {
                    lsTitle = lKDBRowTitulos["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                }
                if (lKDBRowTitulos["{RepEstTitEjeX" + lsHis + "}"] != DBNull.Value)
                {
                    lsXTitle = pKDB.GetHisRegByEnt("RepEstTitEjeX" + lsHis, "Idioma", "iCodCatalogo = " + lKDBRowTitulos["{RepEstTitEjeX" + lsHis + "}"]).Rows[0]["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                }
                if (lKDBRowTitulos["{RepEstTitEjeY" + lsHis + "}"] != DBNull.Value)
                {
                    lsYTitle = pKDB.GetHisRegByEnt("RepEstTitEjeY" + lsHis, "Idioma", "iCodCatalogo = " + lKDBRowTitulos["{RepEstTitEjeY" + lsHis + "}"]).Rows[0]["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                }
                if (lKDBRowTitulos["{LeyendaGrafica" + lsHis + "}"] != DBNull.Value)
                {
                    lbShowLegend = ((int)lKDBRowTitulos["{LeyendaGrafica" + lsHis + "}"] & (int)lOpcTipoGrafica.DataValue) == (int)lOpcTipoGrafica.DataValue;
                }
            }

            bool lbPostBack = false;
            DataTable lDataOrder = pDSCampos.Tables["CamposGr" + lsHis].Clone();
            foreach (DataRow lRow in pDSCampos.Tables["CamposGr" + lsHis].Rows)
            {
                //Si el campo aplica para el tipo de grafica seleccionada
                if (((int)lRow["{BanderasCamposGr" + lsHis + "}"] & (int)lOpcTipoGrafica.DataValue) == (int)lOpcTipoGrafica.DataValue)
                {
                    lDataOrder.ImportRow(lRow);
                    if (!lbPostBack && pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0)
                    {
                        lbPostBack = true;
                    }
                    if (lsbDataFields.Length > 0)
                    {
                        lsbDataFields.AppendLine(",");
                    }
                    lsbDataFields.Append(lRow["{DataField}"].ToString());
                    if (lRow["{DataFieldRuta}"] != DBNull.Value)
                    {
                        lsbDataFields.AppendLine(",");
                        lsbDataFields.Append(lRow["{DataFieldRuta}"].ToString());
                    }

                    if ((int)lRow["{TipoCampoGr" + lsHis + "}"] == 1) //Si es de tipo serie de datos
                    {
                        lstSeriesIds.Add(lRow["{Atrib}"].ToString());

                        lsSerieName = pKDB.GetHisRegByEnt("RepEstIdiomaCmp", "Idioma Campos", "iCodCatalogo = " + lRow["{RepEstIdiomaCmp}"]).Rows[0]["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                        lstSeriesNames.Add(lsSerieName);

                        lstDataColumns.Add(ReporteEstandarUtil.GetDataFieldName(lRow));

                        //Se estable el formato de la primer serie de datos que tenga un tipo de dato definido
                        if (String.IsNullOrEmpty(lsYFormat) && lRow["{Types}"] != DBNull.Value)
                        {
                            lsYFormat = GetFormatString((int)lRow["{Types}"]);
                        }
                    }
                    else if ((int)lRow["{TipoCampoGr" + lsHis + "}"] == 2) //Si es de tipo valores eje x
                    {
                        lGrafica.AddClientEvent("iCodAtributoEjeX", lRow["{Atrib}"].ToString());
                        lsXColumn = ReporteEstandarUtil.GetDataFieldName(lRow);
                        lsXIdsColumn = lsXColumn;

                        //Se establece el formato del eje X si se tiene un tipo de datos definido
                        if (String.IsNullOrEmpty(lsXFormat) && lRow["{Types}"] != DBNull.Value)
                        {
                            lsXFormat = GetFormatString((int)lRow["{Types}"]);
                        }

                        lsGroupByField = "";
                        if (ReporteEstandarUtil.IsValidGroupByField(lRow))
                        {
                            lsGroupByField = ReporteEstandarUtil.GetGroupField(lRow);
                        }

                        if (lRow["{DataFieldRuta}"] != DBNull.Value
                            && ReporteEstandarUtil.IsValidGroupByField(lRow, "{DataFieldRuta}"))
                        {
                            lsXIdsColumn = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                            if (!String.IsNullOrEmpty(lsGroupByField))
                            {
                                lsGroupByField = lsGroupByField + "," + ReporteEstandarUtil.GetGroupField(lRow, "{DataFieldRuta}");
                            }
                            else
                            {
                                lsGroupByField = ReporteEstandarUtil.GetGroupField(lRow, "{DataFieldRuta}");
                            }
                        }
                    }
                }
            }
            lGrafica.AddClientEvent("DataColumnsLength", lstDataColumns.Count.ToString());

            lGrafica.AllowPostback = lbPostBack;
            string lsSortDir;
            string lsSortDirInv;
            pHTParam.Add("DataFieldsGr" + lsHis, lsbDataFields.ToString());
            pHTParam.Add("GroupByFieldGr" + lsHis, lsGroupByField);
            pHTParam.Add("OrderByFieldsGr" + lsHis, ReporteEstandarUtil.GetOrderByFields(lDataOrder, 1, out lsSortDir));
            pHTParam.Add("OrderByFieldsGr" + lsHis + "Inv", ReporteEstandarUtil.GetOrderByFields(lDataOrder, 2, out lsSortDirInv));
            pHTParam.Add("SortDirGr" + lsHis, lsSortDir);
            pHTParam.Add("SortDirGr" + lsHis + "Inv", lsSortDirInv);

            string lsDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceGr" + lsHis + "}"].ToString(), pHTParam, pbReajustarParam);
            if (pbModoDebug)
            {
                ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceGr" + lsHis, lsDataSource);
            }
            DataTable ldt = DSODataAccess.Execute(lsDataSource);

            switch ((TipoGrafica)lOpcTipoGrafica.DataValue)
            {
                case TipoGrafica.Pastel:
                    lGrafica.InitPieChartRep(ldt, lsTitle, lsXColumn, lsXIdsColumn, lstDataColumns.ToArray(), lstSeriesNames.ToArray(), lsXTitle, null, lsYTitle, null, lbShowLegend);
                    break;
                case TipoGrafica.Barras:
                    lGrafica.InitColumnChart(ldt, lsTitle, lstDataColumns.ToArray(), lstSeriesNames.ToArray(), lstSeriesIds.ToArray(), lsXColumn, lsXIdsColumn, lsXTitle, lsXFormat, lsYTitle, lsYFormat, lbShowLegend);
                    break;
                case TipoGrafica.Lineas:
                    lGrafica.InitLineChart(ldt, lsTitle, lstDataColumns.ToArray(), lstSeriesNames.ToArray(), lstSeriesIds.ToArray(), lsXColumn, lsXIdsColumn, lsXTitle, lsXFormat, lsYTitle, lsYFormat, lbShowLegend);
                    break;
                case TipoGrafica.Area:
                    lGrafica.InitAreaChart(ldt, lsTitle, lstDataColumns.ToArray(), lstSeriesNames.ToArray(), lstSeriesIds.ToArray(), lsXColumn, lsXIdsColumn, lsXTitle, lsXFormat, lsYTitle, lsYFormat, lbShowLegend);
                    break;
            }
            return lsDataSource;
        }

        protected virtual string GetFormatString(int iCodType)
        {
            DataRow lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + iCodType)[0];
            string lsFormatString = null;

            if (lKDBTypeRow["vchCodigo"].ToString() == "Int")
            {
                lsFormatString = pIntegerField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "IntFormat")
            {
                lsFormatString = pIntegerFormatField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Float")
            {
                lsFormatString = pNumericField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Currency")
            {
                lsFormatString = pCurrencyField.StringFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "Date")
            {
                lsFormatString = psDateFormat;
            }
            else if (lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
            {
                lsFormatString = psDateTimeFormat;
            }

            return lsFormatString;
        }

        protected virtual void InitGridHeaders()
        {
            //Necesito llamar siempre a Page.ClientScript.GetPostBackEventReference para que se registre la funcion de __doPostBack en la pagina
            if (pTipoReporte == TipoReporte.Tabular)
            {
                psdoPostBackRutaGrid = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "RutaGrid:{cmp}:{cmpAd}:{obj}"));
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                psdoPostBackRutaGrid = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "RutaGrid:{cmp}:{cmpAd}:{obj}"));
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                psdoPostBackRutaY = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "RutaMatrizY:{cmp}:{cmpAd}:{obj}"));
                psdoPostBackRutaXY = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "RutaMatrizXY:{cmp}:{cmpAd}:{rowX}:{obj}"));
                InitGridHeadersMatricial();
            }

            if (bConfiguraGrid)
            {
                InitGridConfig();
                bConfiguraGrid = false;
            }
            pGridReporte.Fill();
        }

        protected virtual void InitGridHeadersMatricial()
        {
            //Agrego todas las filas del encabezado
            TableHeaderRow lHeaderRow;
            foreach (DataRow lRow in pDSCampos.Tables["CamposX"].Rows)
            {
                lHeaderRow = new TableHeaderRow();
                lHeaderRow.TableSection = TableRowSection.TableHeader;
                pGridReporte.Grid.Rows.Add(lHeaderRow);
            }

            //Agrego una fila mas para los campos XY
            if (pDSCampos.Tables["CamposXY"].Rows.Count > 1)
            {
                lHeaderRow = new TableHeaderRow();
                lHeaderRow.TableSection = TableRowSection.TableHeader;
                pGridReporte.Grid.Rows.Add(lHeaderRow);
            }

            //Agregar columnas de Eje Y
            TableHeaderCell lHeaderCell;
            int liRowSpan;
            if (pDSCampos.Tables["CamposXY"].Rows.Count > 1)
            {
                liRowSpan = pDSCampos.Tables["CamposX"].Rows.Count + 1;
            }
            else
            {
                liRowSpan = pDSCampos.Tables["CamposX"].Rows.Count;
            }

            lHeaderRow = (TableHeaderRow)pGridReporte.Grid.Rows[0];
            foreach (DataRow lRow in pDSCampos.Tables["CamposY"].Rows)
            {
                lHeaderCell = new TableHeaderCell();
                lHeaderCell.Scope = TableHeaderScope.Column;
                lHeaderCell.RowSpan = liRowSpan;
                lHeaderRow.Cells.Add(lHeaderCell);
            }

            //Agregar encabezados de columnas de Eje X
            int lidx = 0;
            string lsDataField;
            string lsEncabezadoX;
            string lsEncabezadoXAnt;
            int liColSpan = 1;

            for (lidx = 0; lidx < pDSCampos.Tables["CamposX"].Rows.Count; lidx++)
            {
                DataRow lRow = pDSCampos.Tables["CamposX"].Rows[lidx];
                lsDataField = ReporteEstandarUtil.GetDataFieldName(lRow);

                lHeaderRow = (TableHeaderRow)pGridReporte.Grid.Rows[lidx];
                lsEncabezadoX = null;
                lsEncabezadoXAnt = null;
                foreach (DataRow ldataRow in pDSCampos.Tables["ValoresEjeX"].Rows)
                {
                    lsEncabezadoX = FormatearValor(lRow, ldataRow[lsDataField]);

                    if (lsEncabezadoXAnt == null)
                    {
                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColSpan = 1;
                    }
                    else if (lsEncabezadoX == lsEncabezadoXAnt)
                    {
                        liColSpan = liColSpan + 1;
                    }
                    else
                    {
                        lHeaderCell = new TableHeaderCell();
                        lHeaderCell.Text = lsEncabezadoXAnt;
                        lHeaderCell.ColumnSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                        lHeaderRow.Cells.Add(lHeaderCell);

                        lsEncabezadoXAnt = lsEncabezadoX;
                        liColSpan = 1;
                    }
                }
                if (lsEncabezadoX != null)
                {
                    lHeaderCell = new TableHeaderCell();
                    lHeaderCell.Text = lsEncabezadoX;
                    lHeaderCell.ColumnSpan = liColSpan * pDSCampos.Tables["CamposXY"].Rows.Count;
                    lHeaderRow.Cells.Add(lHeaderCell);
                }
            }

            //Agregar columnas de campos XY
            if (pDSCampos.Tables["CamposXY"].Rows.Count > 1)
            {
                lHeaderRow = (TableHeaderRow)pGridReporte.Grid.Rows[pGridReporte.Grid.Rows.Count - 1];
                foreach (DataRow lRow in pDSCampos.Tables["CamposXY"].Rows)
                {
                    for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                    {
                        lHeaderCell = new TableHeaderCell();
                        lHeaderCell.Scope = TableHeaderScope.Column;
                        lHeaderRow.Cells.Add(lHeaderCell);
                    }
                }
            }

            //Agregar columnas de totalizados XY para el eje X
            lHeaderRow = (TableHeaderRow)pGridReporte.Grid.Rows[0];
            foreach (DataRow lRow in pDSCampos.Tables["TotalizadosXYTotX"].Rows)
            {
                lHeaderCell = new TableHeaderCell();
                lHeaderCell.Scope = TableHeaderScope.Column;
                lHeaderCell.RowSpan = liRowSpan;
                lHeaderRow.Cells.Add(lHeaderCell);
            }
        }

        protected virtual void InitGridConfig()
        {
            pGridReporte.ClearConfig();
            pGridReporte.Config.bAutoWidth = true;
            pGridReporte.Config.sScrollX = "100%";
            if (pKDBRowReporte["{RepEstScrollH}"] != DBNull.Value && int.Parse(pKDBRowReporte["{RepEstScrollH}"].ToString()) > 50)
            {
                pGridReporte.Config.sScrollXInner = int.Parse(pKDBRowReporte["{RepEstScrollH}"].ToString()).ToString() + "%";
            }
            pGridReporte.Config.aaSorting = new List<ArrayList>();

            pGridReporte.Config.sPaginationType = "full_numbers";
            pGridReporte.Config.bJQueryUI = true;
            pGridReporte.Config.bProcessing = true;
            pGridReporte.Config.bServerSide = true;
            pGridReporte.Config.fnRowCallback = "function(nRow, aData, iDisplayIndex, iDisplayIndexFull) { return " + pjsObj + ".fnRowCallBack(nRow, aData, iDisplayIndex, iDisplayIndexFull);}";

            if (pTipoReporte == TipoReporte.Tabular)
            {
                InitGridTabular();
            }
            else if (pTipoReporte == TipoReporte.Resumido)
            {
                InitGridResumido();
            }
            else if (pTipoReporte == TipoReporte.Matricial)
            {
                InitGridMatricial();
            }

            //Agrego una columna que va a indicar si es la fila de SubTotales de pagina
            DSOGridClientColumn lCol = new DSOGridClientColumn();
            lCol.sName = "bSubTotalPag";
            lCol.aTargets.Add(pGridReporte.Config.aoColumnDefs.Count);
            lCol.sTitle = "bSubTotalPag";
            lCol.bVisible = false;

            pGridReporte.Config.aoColumnDefs.Add(lCol);

            //Agrego una columna para permitir regresar el tipo de clase de estilos para toda la fila
            lCol = new DSOGridClientColumn();
            lCol.sName = "TrCssClass";
            lCol.aTargets.Add(pGridReporte.Config.aoColumnDefs.Count);
            lCol.sTitle = "TrCssClass";
            lCol.bVisible = false;

            pGridReporte.Config.aoColumnDefs.Add(lCol);
        }

        protected virtual void InitGridTabular()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pGridReporte.AutoGenerateColumns = true;

            pGridReporte.Config.sDom = "<\"H\"Rli>tr<\"F\"p>";
            pGridReporte.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnReporte(this, sSource, aoData, fnCallback);}";
            pGridReporte.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetRepTabular");

            //primero agrego todas las columnas visibles
            DataRow lKDBTypeRow;
            int liCodAtributo;
            int liCodAtributoAd;
            foreach (DataRow lRow in pDSCampos.Tables["Campos"].Rows)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow);
                lCol.sClass = "RepDefault";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = true;
                lCol.bSortable = pbOrdenCol;

                //Reviso si tiene ruta asignada para establecer funcion de hyperlink
                if (pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0
                    || (lRow["{AtribAd}"] != DBNull.Value && pKDBRelRutas.Select("[{Atrib}] = " + lRow["{AtribAd}"].ToString()).Length > 0))
                {
                    liCodAtributo = (int)lRow["{Atrib}"];
                    liCodAtributoAd = 0;
                    if (lRow["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRow["{AtribAd}"];
                    }
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRuta(obj," + liCodAtributo + "," + liCodAtributoAd + ",\"" + psdoPostBackRutaGrid + "\");}";
                    lCol.bUseRendered = false;
                    lCol.sClass += " RepRuta";
                }

                //Reviso si tiene tipo de dato para asignar la clase de estilos
                if (lRow["{Types}"] != DBNull.Value)
                {
                    lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lRow["{Types}"])[0];
                    lCol.sClass = lCol.sClass.Replace("RepDefault", "Rep" + lKDBTypeRow["vchCodigo"].ToString());
                }

                pGridReporte.Config.aoColumnDefs.Add(lCol);
            }

            //agrego al final todas las columnas que estaran ocultas y serviran para las rutas
            foreach (DataRow lRow in pDSCampos.Tables["Campos"].Rows)
            {
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRutaAd}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected virtual void InitGridResumido()
        {
            InitGridTabular();
            pGridReporte.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetRepResumido");
        }

        protected virtual void InitGridMatricial()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pGridReporte.AutoGenerateColumns = false;

            pGridReporte.Config.sDom = "<\"H\"li>tr<\"F\"p>";
            pGridReporte.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnReporte(this, sSource, aoData, fnCallback);}";
            pGridReporte.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetRepMatricial");

            //Agregar columnas de Eje Y
            DataRow lKDBTypeRow;
            int liCodAtributo;
            int liCodAtributoAd;
            foreach (DataRow lRow in pDSCampos.Tables["CamposY"].Rows)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow);
                lCol.sClass = "RepDefault";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = true;
                lCol.bSortable = pbOrdenCol;

                //Reviso si tiene ruta asignada para establecer funcion de hyperlink
                if (pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0
                    || (lRow["{AtribAd}"] != DBNull.Value && pKDBRelRutas.Select("[{AtribAd}] = " + lRow["{AtribAd}"].ToString()).Length > 0))
                {
                    liCodAtributo = (int)lRow["{Atrib}"];
                    liCodAtributoAd = 0;
                    if (lRow["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRow["{AtribAd}"];
                    }

                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRuta(obj," + liCodAtributo + "," + liCodAtributoAd + ",\"" + psdoPostBackRutaY + "\");}";
                    lCol.bUseRendered = false;
                    lCol.sClass += " RepRuta";
                }

                //Reviso si tiene tipo de dato para asignar la clase de estilos
                if (lRow["{Types}"] != DBNull.Value)
                {
                    lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lRow["{Types}"])[0];
                    lCol.sClass = lCol.sClass.Replace("RepDefault", "Rep" + lKDBTypeRow["vchCodigo"].ToString());
                }
                pGridReporte.Config.aoColumnDefs.Add(lCol);
            }


            //Agregar columnas de campos XY
            List<string> lstAtribX = new List<string>();
            lstAtribX.Add("-1");
            foreach (DataRow lRow in pDSCampos.Tables["CamposX"].Rows)
            {
                lstAtribX.Add(lRow["{Atrib}"].ToString());
            }
            string lsAtribX = String.Join(",", lstAtribX.ToArray());
            bool lbRutaX = pKDBRelRutas.Select("[{Atrib}] in(" + lsAtribX + ")").Length > 0;

            int lidx;
            for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
            {
                foreach (DataRow lRow in pDSCampos.Tables["CamposXY"].Rows)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = "ColX" + lidx + "_" + ReporteEstandarUtil.GetDataFieldName(lRow);
                    lCol.sClass = "RepDefault";
                    lCol.aTargets.Add(lTarget++);
                    lCol.bVisible = true;
                    lCol.bSortable = pbOrdenCol;

                    //Reviso si tiene ruta asignada para establecer funcion de hyperlink
                    if (lbRutaX || pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0
                        || (lRow["{AtribAd}"] != DBNull.Value && pKDBRelRutas.Select("[{Atrib}] = " + lRow["{AtribAd}"].ToString()).Length > 0))
                    {
                        liCodAtributo = (int)lRow["{Atrib}"];
                        liCodAtributoAd = 0;
                        if (lRow["{AtribAd}"] != DBNull.Value)
                        {
                            liCodAtributoAd = (int)lRow["{AtribAd}"];
                        }

                        lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRutaXY(obj," + liCodAtributo + "," + liCodAtributoAd + "," + lidx + ",\"" + psdoPostBackRutaXY + "\");}";
                        lCol.bUseRendered = false;
                        lCol.sClass += " RepRuta";
                    }

                    //Reviso si tiene tipo de dato para asignar la clase de estilos
                    if (lRow["{Types}"] != DBNull.Value)
                    {
                        lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lRow["{Types}"])[0];
                        lCol.sClass = lCol.sClass.Replace("RepDefault", "Rep" + lKDBTypeRow["vchCodigo"].ToString());
                    }

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
            }

            //Agregar columnas de totalizados XY para el eje X
            foreach (DataRow lRow in pDSCampos.Tables["TotalizadosXYTotX"].Rows)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow);
                lCol.sClass = "RepDefault";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = true;
                lCol.bSortable = pbOrdenCol;

                //Reviso si tiene ruta asignada para establecer funcion de hyperlink
                if (pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0
                    || (lRow["{AtribAd}"] != DBNull.Value && pKDBRelRutas.Select("[{Atrib}] = " + lRow["{AtribAd}"].ToString()).Length > 0))
                {
                    liCodAtributo = (int)lRow["{Atrib}"];
                    liCodAtributoAd = 0;
                    if (lRow["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRow["{AtribAd}"];
                    }
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRuta(obj," + liCodAtributo + "," + liCodAtributoAd + ",\"" + psdoPostBackRutaY + "\");}";
                    lCol.bUseRendered = false;
                    lCol.sClass += " RepRuta";
                }

                //Reviso si tiene tipo de dato para asignar la clase de estilos
                if (lRow["{Types}"] != DBNull.Value)
                {
                    lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lRow["{Types}"])[0];
                    lCol.sClass = lCol.sClass.Replace("RepDefault", "Rep" + lKDBTypeRow["vchCodigo"].ToString());
                }

                pGridReporte.Config.aoColumnDefs.Add(lCol);
            }

            //Agregar columnas no visibles con los valores de las rutas
            foreach (DataRow lRow in pDSCampos.Tables["CamposY"].Rows)
            {
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRutaAd}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
            }

            foreach (DataRow lRow in pDSCampos.Tables["CamposXY"].Rows)
            {
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = "ColX" + lidx + "_" + ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                        lCol.aTargets.Add(lTarget++);
                        lCol.sTitle = lCol.sName;
                        lCol.bVisible = false;
                        lCol.bSortable = pbOrdenCol;

                        pGridReporte.Config.aoColumnDefs.Add(lCol);
                    }
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    for (lidx = 0; lidx < pDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = "ColX" + lidx + "_" + ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRutaAd}");
                        lCol.aTargets.Add(lTarget++);
                        lCol.sTitle = lCol.sName;
                        lCol.bVisible = false;
                        lCol.bSortable = pbOrdenCol;

                        pGridReporte.Config.aoColumnDefs.Add(lCol);
                    }
                }
            }

            foreach (DataRow lRow in pDSCampos.Tables["TotalizadosXYTotX"].Rows)
            {
                if (lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
                if (lRow["{DataFieldRutaAd}"] != DBNull.Value)
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRutaAd}");
                    lCol.aTargets.Add(lTarget++);
                    lCol.sTitle = lCol.sName;
                    lCol.bVisible = false;
                    lCol.bSortable = pbOrdenCol;

                    pGridReporte.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected virtual void InitLangGridTabular()
        {
            string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
            DataRow lRow;
            Hashtable lHTClientColumns = new Hashtable();

            foreach (DSOGridClientColumn lCol in pGridReporte.Config.aoColumnDefs)
            {
                if (lCol.bVisible)
                {
                    lRow = ReporteEstandarUtil.GetCampoByDataField(lCol.sName, pDSCampos.Tables["Campos"]);
                    lHTClientColumns.Add(lCol.sName, lRow);

                    lCol.sTitle = pKDB.GetHisRegByEnt("RepEstIdiomaCmp", "Idioma Campos", new string[] { lsLang }, "iCodCatalogo = " + lRow["{RepEstIdiomaCmp}"]).Rows[0][lsLang].ToString();
                    lCol.sTitle = DSOControl.JScriptEncode(lCol.sTitle);
                }
            }

            //Determinar si hay campos que se totalizan
            if (pDSCampos.Tables["Totalizados"].Rows.Count > 0)
            {
                InitTotalizados(lHTClientColumns);
            }
        }

        protected virtual void InitLangGridResumido()
        {
            InitLangGridTabular();
        }

        protected virtual void InitLangGridMatricial()
        {
            string lsLang = "{" + Globals.GetCurrentLanguage() + "}";
            DataRow lRow;
            Hashtable lHTClientColumns = new Hashtable();
            int liRowX = 0;
            string lsDataFieldX = ReporteEstandarUtil.GetDataFieldName(pDSCampos.Tables["CamposX"].Rows[pDSCampos.Tables["CamposX"].Rows.Count - 1]);

            foreach (DSOGridClientColumn lCol in pGridReporte.Config.aoColumnDefs)
            {
                if (lCol.bVisible)
                {
                    lRow = ReporteEstandarUtil.GetCampoByMatrizField(lCol.sName, pDSCampos, pDSCampos.Tables["ValoresEjeX"].Rows.Count);
                    lHTClientColumns.Add(lCol.sName, lRow);

                    if (pDSCampos.Tables["CamposXY"].Rows.Count == 1
                        && lRow.Table == pDSCampos.Tables["CamposXY"]
                        && lCol.sName == "ColX" + liRowX + "_" + ReporteEstandarUtil.GetDataFieldName(lRow))
                    {
                        lCol.sTitle = pDSCampos.Tables["ValoresEjeX"].Rows[liRowX][lsDataFieldX].ToString();
                        lCol.sTitle = DSOControl.JScriptEncode(lCol.sTitle);
                        liRowX = liRowX + 1;
                    }
                    else
                    {
                        lCol.sTitle = pKDB.GetHisRegByEnt("RepEstIdiomaCmp", "Idioma Campos", new string[] { lsLang }, "iCodCatalogo = " + lRow["{RepEstIdiomaCmp}"]).Rows[0][lsLang].ToString();
                        lCol.sTitle = DSOControl.JScriptEncode(lCol.sTitle);
                    }
                }
            }

            //Determinar si hay campos que se totalizan
            if (pDSCampos.Tables["TotalizadosY"].Rows.Count > 0
                || pDSCampos.Tables["TotalizadosXYTotX"].Rows.Count > 0
                || pDSCampos.Tables["TotalizadosXYTotY"].Rows.Count > 0)
            {
                InitTotalizados(lHTClientColumns);
            }
        }

        protected virtual void InitTotalizados(Hashtable lHTClientColumns)
        {
            DataTable ldt = new DataTable();

            if (pTablaTotales == null)
            {
                string lsDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSourceTot}"].ToString(), pHTParam, pbReajustarParam);
                if (pbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSourceTot", lsDataSource);
                }
                ldt = DSODataAccess.Execute(lsDataSource);
            }
            else
            {
                ldt = pTablaTotales;
            }


            TableRow lTableRow = new TableRow();
            TableHeaderCell lTableCell;

            lTableRow.TableSection = TableRowSection.TableFooter;
            pGridReporte.Grid.Rows.Add(lTableRow);

            DataRow lKDBTypeRow;
            DataRow lKDBCamposRow;

            bool lbTotal = false;

            foreach (DSOGridClientColumn lCol in pGridReporte.Config.aoColumnDefs)
            {
                if (!lCol.bVisible)
                {
                    continue;
                }
                lTableCell = new TableHeaderCell();
                lTableRow.Cells.Add(lTableCell);

                if (!lbTotal)
                {
                    lbTotal = true;
                    lTableCell.Text = Globals.GetMsgWeb(false, "Total");
                }

                if (ldt.Columns.Contains(lCol.sName))
                {
                    lKDBCamposRow = (DataRow)lHTClientColumns[lCol.sName];

                    if (lKDBCamposRow["{Types}"] != DBNull.Value)
                    {
                        lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lKDBCamposRow["{Types}"])[0];
                        lTableCell.CssClass = "Rep" + lKDBTypeRow["vchCodigo"].ToString();
                        lTableCell.Text = FormatearValor(lKDBCamposRow, ldt.Rows[0][lCol.sName]);
                    }
                }
            }
        }

        public string FormatearValor(DataRow lKDBRowCampo, object lValor)
        {
            string lsRet = "";
            DataRow lKDBTypeRow;

            if (lKDBRowCampo["{Types}"] != DBNull.Value)
            {
                lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lKDBRowCampo["{Types}"])[0];
            }
            else
            {
                lKDBTypeRow = null;
            }

            if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Int")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pIntegerField.StringFormat, pIntegerField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "IntFormat")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pIntegerFormatField.StringFormat, pIntegerFormatField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Float")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pNumericField.StringFormat, pNumericField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Currency")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, pCurrencyField.StringFormat, pCurrencyField.FormatInfo);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Date")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, psDateFormat);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, psDateTimeFormat);
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "TimeSeg")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, "TimeSeg");
            }
            else if (lKDBTypeRow != null && lKDBTypeRow["vchCodigo"].ToString() == "Time")
            {
                lsRet = ReporteEstandarUtil.FormatearValor(lValor, pvchCodIdioma, "Time");
            }
            else
            {
                lsRet = lValor.ToString();
            }

            return lsRet;
        }

        #region Exportar Documento

        protected void InitHTParamDesc()
        {
            pHTParamDesc = new Hashtable();
            if (pbAreaParametros)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    pHTParamDesc.Add(lField.ConfigName, lField.ToString());
                }
            }
            if (pbParamFecIniFin)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaIniRep"))
                {
                    pHTParamDesc.Add("FechaIniRep", pdtInicio.ToString());
                }

                if (!pbAreaParametros || !pFields.ContainsConfigName("FechaFinRep"))
                {
                    pHTParamDesc.Add("FechaFinRep", pdtFin.ToString());
                }
            }
            if (pbParamNumReg)
            {
                if (!pbAreaParametros || !pFields.ContainsConfigName("NumRegReporte"))
                {
                    pHTParamDesc.Add("NumRegReporte", piNumReg.ToString());
                }
            }
        }

        protected ReporteEstandarUtil GetReporteEstandarUtil()
        {
            InitHTParam();
            InitHTParamDesc();
            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
            return new ReporteEstandarUtil(iCodReporte, pHTParam, pHTParamDesc, lsKeytiaWebFPath, lsStylePath);
        }


        public void ExportXLS()
        {
            if (State == ReportState.SubReporte)
            {
                pSubReporte.ExportXLS();
            }
            else if (State == ReportState.Aplicacion)
            {
                pSubHistorico.ExportXLS();
            }
            else
            {
                CrearXLS(".xlsx");
            }
        }


        /// <summary>
        /// Genera el archivo de Excel
        /// </summary>
        protected void CrearXLS(string lsExt)
        {
            //Valida si la consulta regresa más de 1,000,000 de registros
            //Si es el caso entonces el reporte se enviará por correo
            //Si se trata de menos de esa cantidad, entonces se genera el archivo en línea
            if (!ValidarEnvioPorCorreo(lsExt))
            {
                ExcelAccess lExcel = null;
                try
                {
                    //Crea una nueva instancia de la clase ReporteEstandarUtil
                    ReporteEstandarUtil lReporteEstandarUtil = GetReporteEstandarUtil();

                    //Genera el archivo de Excel, le da formato y lo llena con los datos de la consulta
                    lExcel = lReporteEstandarUtil.ExportXLS();

                    //El método GetFileName() obtiene la carpeta en donde se guardará el reporte
                    //y forma el nombre completo del archivo (incluyendo dicha ruta)
                    lExcel.FilePath = GetFileName(lsExt);

                    //Guarda el archivo
                    lExcel.SalvarComo();

                    //Destruye todas las instancias de Excel utilizadas previamente
                    lExcel.Cerrar(true);
                    lExcel.Dispose();

                    //Hace un redirect hacia la pagina:
                    ExportarArchivo(lsExt);
                }
                catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
                catch (Exception e)
                {
                    throw new KeytiaWebException("ErrExportTo", e, lsExt);
                }
                finally
                {
                    if (lExcel != null)
                    {
                        lExcel.Cerrar(true);
                        lExcel.Dispose();
                        lExcel = null;
                    }
                }
            }
        }

        public void ExportDOC()
        {
            if (State == ReportState.SubReporte)
            {
                pSubReporte.ExportDOC();
            }
            else if (State == ReportState.Aplicacion)
            {
                pSubHistorico.ExportDOC();
            }
            else
            {
                CrearDOC(".docx");
            }
        }

        protected void CrearDOC(string lsExt)
        {
            if (!ValidarEnvioPorCorreo(lsExt))
            {
                WordAccess lWord = null;
                try
                {
                    ReporteEstandarUtil lReporteEstandarUtil = GetReporteEstandarUtil();
                    lWord = lReporteEstandarUtil.ExportDOC();
                    lWord.FilePath = GetFileName(lsExt);
                    lWord.SalvarComo();
                    lWord.Cerrar(true);
                    lWord.Dispose();
                    ExportarArchivo(lsExt);
                }
                catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
                catch (Exception e)
                {
                    throw new KeytiaWebException("ErrExportTo", e, lsExt);
                }
                finally
                {
                    if (lWord != null)
                    {
                        lWord.Cerrar(true);
                        lWord.Dispose();
                        lWord = null;
                    }
                }
            }
        }

        public void ExportPDF()
        {
            if (State == ReportState.SubReporte)
            {
                pSubReporte.ExportPDF();
            }
            else if (State == ReportState.Aplicacion)
            {
                pSubHistorico.ExportPDF();
            }
            else
            {
                CrearDOC(".pdf");
                //CrearXLS(".pdf");
            }
        }

        public void ExportCSV()
        {
            if (State == ReportState.SubReporte)
            {
                pSubReporte.ExportCSV();
            }
            else if (State == ReportState.Aplicacion)
            {
                pSubHistorico.ExportCSV();
            }
            else
            {
                CrearCSV();
            }
        }

        protected void CrearCSV()
        {
            if (!ValidarEnvioPorCorreo(".csv"))
            {
                TxtFileAccess lTxt = new TxtFileAccess();
                try
                {
                    ReporteEstandarUtil lReporteEstandarUtil = GetReporteEstandarUtil();
                    lTxt.FileName = GetFileName(".csv");
                    lTxt.Abrir();
                    lReporteEstandarUtil.ExportCSV(lTxt);
                    lTxt.Cerrar();
                    lTxt = null;

                    ExportarArchivo(".csv");
                }
                catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
                catch (Exception e)
                {
                    throw new KeytiaWebException("ErrExportTo", e, ".csv");
                }
                finally
                {
                    if (lTxt != null)
                    {
                        lTxt.Cerrar();
                        lTxt = null;
                    }
                }
            }
        }

        protected void ExportarArchivo(string lsExt)
        {
            string lsTitulo = HttpUtility.UrlEncode(Title);
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected string GetFileName(string lsExt)
        {
            string lsFileName = System.IO.Path.Combine(psTempPath, "rep." + psFileKey + ".temp" + lsExt);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }



        /// <summary>
        /// Valida si la consulta regresa menos de 1,000,000 de registros
        /// En caso de que regrese menos de esa cantidad, el método regresa el valor false, con lo cual
        /// indica que no se debe enviar por correo el reporte.
        /// En caso de que regrese más de esa cantidad, el método regresa true, indicanco que si debe
        /// enviarse por correo.
        /// </summary>
        /// <param name="lsExt"></param>
        /// <returns>bool Indica si se debe enviar por correo el reporte que se trata de exportar</returns>
        protected bool ValidarEnvioPorCorreo(string lsExt)
        {
            //Se valida si el nùmero de registros es menor a 1 millón
            //de ser así se descargará el archivo, de lo contrario se enviará por correo.
            if (NumeroRegistros < 1000000)
            {
                //Si el valor es false quiere decir que no es necesario enviar el reporte por mail
                return false;
            }


            ExportExt = lsExt;


            //Genera la pantalla donde se configura el envío del reporte
            InitEnvioCorreo();


            //Si el valor es true quiere decir que sí se debe enviar el reporte por mail
            return true;
        }

        #endregion

        protected virtual bool ValidarPermiso(Permiso p)
        {
            return DSONavegador.getPermiso(psOpcMnu, p);
        }

        #region Rutas

        protected virtual bool InitRuta(int liCodAtributo, object loValor)
        {
            bool lbRet = false;
            //Primero reviso si hay un valor especifico para la ruta actual
            DataRow[] lConsultas = pVisConsultas.Select("Atrib = " + liCodAtributo + " And Ruta = " + iCodRutaConsulta);
            DataRow lKDBRutaRow = null;
            foreach (DataRow lRowConsulta in lConsultas)
            {
                if (lRowConsulta["Valores"] != DBNull.Value
                    && (lRowConsulta["Valores"].ToString() == loValor.ToString()
                    || lRowConsulta["Valores"].Equals(loValor)))
                {
                    lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
                }
                else if (lRowConsulta["Value"] != DBNull.Value
                    && (lRowConsulta["Value"].ToString() == loValor.ToString()
                    || lRowConsulta["Value"].Equals(loValor)))
                {
                    lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
                }
                else if (lRowConsulta["FloatValue"] != DBNull.Value
                    && (lRowConsulta["FloatValue"].ToString() == loValor.ToString()
                    || lRowConsulta["FloatValue"].Equals(loValor)))
                {
                    lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
                }
                else if (lRowConsulta["DateValue"] != DBNull.Value
                    && (lRowConsulta["DateValue"].ToString() == loValor.ToString()
                    || lRowConsulta["DateValue"].Equals(loValor)))
                {
                    lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
                }
                else if (lRowConsulta["VarCharValue"] != DBNull.Value
                    && (lRowConsulta["VarCharValue"].ToString() == loValor.ToString()
                    || lRowConsulta["VarCharValue"].Equals(loValor)))
                {
                    lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
                }

                if (lKDBRutaRow != null)
                {
                    break;
                }
            }

            //Reviso si hay un valor especifico que me haga cambiar de ruta
            if (lKDBRutaRow == null)
            {
                lConsultas = pVisConsultas.Select("Atrib = " + liCodAtributo + " And Ruta <> " + iCodRutaConsulta);
                foreach (DataRow lRowConsulta in lConsultas)
                {
                    if (lRowConsulta["Valores"] != DBNull.Value
                        && (lRowConsulta["Valores"].ToString() == loValor.ToString()
                        || lRowConsulta["Valores"].Equals(loValor)))
                    {
                        lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + lRowConsulta["Ruta"])[0];
                    }
                    else if (lRowConsulta["Value"] != DBNull.Value
                        && (lRowConsulta["Value"].ToString() == loValor.ToString()
                        || lRowConsulta["Value"].Equals(loValor)))
                    {
                        lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + lRowConsulta["Ruta"])[0];
                    }
                    else if (lRowConsulta["FloatValue"] != DBNull.Value
                        && (lRowConsulta["FloatValue"].ToString() == loValor.ToString()
                        || lRowConsulta["FloatValue"].Equals(loValor)))
                    {
                        lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + lRowConsulta["Ruta"])[0];
                    }
                    else if (lRowConsulta["DateValue"] != DBNull.Value
                        && (lRowConsulta["DateValue"].ToString() == loValor.ToString()
                        || lRowConsulta["DateValue"].Equals(loValor)))
                    {
                        lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + lRowConsulta["Ruta"])[0];
                    }
                    else if (lRowConsulta["VarCharValue"] != DBNull.Value
                        && (lRowConsulta["VarCharValue"].ToString() == loValor.ToString()
                        || lRowConsulta["VarCharValue"].Equals(loValor)))
                    {
                        lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] = " + lRowConsulta["iCodCatalogo"] + " and isnull([{Ruta}],0) = " + lRowConsulta["Ruta"])[0];
                    }

                    if (lKDBRutaRow != null)
                    {
                        break;
                    }
                }
            }

            //Reviso los atributos que no tienen valores especificos (estos deben de pertenecer a la ruta en la que me encuentro)
            if (lKDBRutaRow == null
                && pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] is null and isnull([{Ruta}],0) = " + iCodRutaConsulta).Length > 0)
            {
                lKDBRutaRow = pKDBRelRutas.Select("[{Atrib}] = " + liCodAtributo + " and [{Consul}] is null and isnull([{Ruta}],0) = " + iCodRutaConsulta)[0];
            }

            if (lKDBRutaRow != null)
            {
                lbRet = true;
                iCodSubReporte = (int)lKDBRutaRow["{RepEst}"];
                if (lKDBRutaRow["{Ruta}"] != DBNull.Value)
                {
                    iCodSubRutaConsulta = (int)lKDBRutaRow["{Ruta}"];
                }
                InitSubReporte();
                SetReportState(ReportState.SubReporte);

                if (pSubReporte.Fields != null)
                {
                    pSubReporte.Fields.IniVigencia = (DateTime)Session["FechaIniRep"];
                    pSubReporte.Fields.FinVigencia = ((DateTime)Session["FechaFinRep"]).AddDays(1);
                }
                FirePostInitRuta();
            }

            return lbRet;
        }

        protected virtual void pGrafica_Click(object sender, ImageMapEventArgs e)
        {
            DataTable lDataSource = DSODataAccess.Execute(DataSourceGr);
            DSOChartPostBackValue lChartPostBackValue = DSOControl.DeserializeJSON<DSOChartPostBackValue>(e.PostBackValue);

            if ((TipoGrafica)pOpcTipoGrafica.DataValue == TipoGrafica.Pastel)
            {
                ProcesaRutaGraficaPastel(pGrafica, lChartPostBackValue, lDataSource, "");
            }
            else
            {
                ProcesaRutaGrafica(pGrafica, lChartPostBackValue, lDataSource, "");
            }
        }

        protected virtual void pGraficaHis_Click(object sender, ImageMapEventArgs e)
        {
            DataTable lDataSource = DSODataAccess.Execute(DataSourceGrHis);
            DSOChartPostBackValue lChartPostBackValue = DSOControl.DeserializeJSON<DSOChartPostBackValue>(e.PostBackValue);

            if ((TipoGrafica)pOpcTipoGraficaHis.DataValue == TipoGrafica.Pastel)
            {
                ProcesaRutaGraficaPastel(pGraficaHis, lChartPostBackValue, lDataSource, "His");
            }
            else
            {
                ProcesaRutaGrafica(pGraficaHis, lChartPostBackValue, lDataSource, "His");
            }
        }

        protected virtual void ProcesaRutaGraficaPastel(DSOChart lGrafica, DSOChartPostBackValue lChartPostBackValue, DataTable lDataSource, string lsHis)
        {
            int liCodAtributoSerie;
            int liCodAtributoEjeX;
            string lsSerieId;
            object loValue;
            string lsDataField;
            string lsTitleRuta = null;
            DataRow lRowCampo;
            int liDataColumnsLength = int.Parse(lGrafica.GetClientEvent("DataColumnsLength"));

            liCodAtributoSerie = int.Parse(lGrafica.GetClientEvent("iCodAtributoEjeX"));
            if (liDataColumnsLength != 1)
            {
                lsTitleRuta = lChartPostBackValue.Legend.ToString();
                lsSerieId = lChartPostBackValue.SerieId.ToString();
                loValue = lChartPostBackValue.Value;
                lsDataField = lGrafica.XValues[lChartPostBackValue.X].ToString();

                lRowCampo = ReporteEstandarUtil.GetCampoByDataField(lsDataField, pDSCampos.Tables["CamposGr" + lsHis]);
                liCodAtributoEjeX = (int)lRowCampo["{Atrib}"];
            }
            else
            {
                lsTitleRuta = lChartPostBackValue.X.ToString();
                lsSerieId = lGrafica.XValues[lChartPostBackValue.X].ToString();
                loValue = lChartPostBackValue.Value;
                lsDataField = lChartPostBackValue.SerieId.ToString();

                lRowCampo = ReporteEstandarUtil.GetCampoByDataField(lsDataField, pDSCampos.Tables["CamposGr" + lsHis]);
                liCodAtributoEjeX = (int)lRowCampo["{Atrib}"];
            }

            if (!InitRuta(liCodAtributoSerie, lsSerieId))
            {
                InitRuta(liCodAtributoEjeX, loValue);
            }

            KeytiaBaseField lField;
            if (pSubReporte != null && pSubReporte.Fields != null)
            {
                if (pSubReporte.Fields.ContainsConfigValue(liCodAtributoEjeX))
                {
                    pSubReporte.TitleRuta = lsTitleRuta;
                    lField = pSubReporte.Fields.GetByConfigValue(liCodAtributoEjeX);
                    lField.DataValue = loValue;
                }
                if (pSubReporte.Fields.ContainsConfigValue(liCodAtributoSerie))
                {
                    pSubReporte.TitleRuta = lsTitleRuta;
                    lField = pSubReporte.Fields.GetByConfigValue(liCodAtributoSerie);
                    lField.DataValue = lsSerieId;
                }
                SetSubReporteParams();
            }
        }

        protected virtual void ProcesaRutaGrafica(DSOChart lGrafica, DSOChartPostBackValue lChartPostBackValue, DataTable lDataSource, string lsHis)
        {
            object loValorEjeX = DBNull.Value;
            int liCodAtributoEjeX = int.Parse(lGrafica.GetClientEvent("iCodAtributoEjeX"));
            int liCodAtributoSerie = -1;
            object loValue = lChartPostBackValue.Value;
            int.TryParse(lChartPostBackValue.SerieId.ToString(), out liCodAtributoSerie);
            if (lGrafica.XValues.ContainsKey(lChartPostBackValue.X))
            {
                loValorEjeX = lGrafica.XValues[lChartPostBackValue.X];
            }

            //Por lo general en las graficas de tipo Barras, Lineas y Area, en el eje x es donde
            //se tendran las entidades por lo que primero se busca la ruta por el atributo del eje x
            if (!InitRuta(liCodAtributoEjeX, loValorEjeX))
            {
                InitRuta(liCodAtributoSerie, loValue);
            }

            KeytiaBaseField lField;
            if (pSubReporte != null && pSubReporte.Fields != null)
            {
                if (pSubReporte.Fields.ContainsConfigValue(liCodAtributoEjeX))
                {
                    pSubReporte.TitleRuta = lChartPostBackValue.X.ToString();
                    lField = pSubReporte.Fields.GetByConfigValue(liCodAtributoEjeX);
                    lField.DataValue = loValorEjeX;
                }
                if (pSubReporte.Fields.ContainsConfigValue(liCodAtributoSerie))
                {
                    if (String.IsNullOrEmpty(pSubReporte.TitleRuta))
                    {
                        pSubReporte.TitleRuta = lChartPostBackValue.Legend.ToString();
                    }
                    lField = pSubReporte.Fields.GetByConfigValue(liCodAtributoSerie);
                    lField.DataValue = loValue;
                }
                SetSubReporteParams();
            }
        }

        protected string GetRutaDataField(DataTable lKDBCampos, int liCodAtributo)
        {
            DataRow lRowCampo = null;
            if (lKDBCampos.Select("[{Atrib}] = " + liCodAtributo).Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[{Atrib}] = " + liCodAtributo)[0];
            }

            if (lRowCampo != null)
            {
                return GetRutaDataField(lRowCampo);
            }
            else
            {
                return null;
            }
        }

        protected string GetRutaDataField(DataRow lRowCampo)
        {
            if (lRowCampo["{DataFieldRuta}"] != DBNull.Value)
            {
                return ReporteEstandarUtil.GetDataFieldName(lRowCampo, "{DataFieldRuta}");
            }
            else
            {
                return ReporteEstandarUtil.GetDataFieldName(lRowCampo);
            }
        }

        protected string GetRutaDataFieldAd(DataTable lKDBCampos, int liCodAtributo, int liCodAtributoAd)
        {
            DataRow lRowCampo = null;
            if (lKDBCampos.Select("[{Atrib}] = " + liCodAtributo + " And [{AtribAd}] = " + liCodAtributoAd).Length > 0)
            {
                lRowCampo = lKDBCampos.Select("[{Atrib}] = " + liCodAtributo + " And [{AtribAd}] = " + liCodAtributoAd)[0];
            }

            if (lRowCampo != null && lRowCampo["{DataFieldRutaAd}"] != DBNull.Value)
            {
                return GetRutaDataFieldAd(lRowCampo);
            }
            else
            {
                return null;
            }
        }

        protected string GetRutaDataFieldAd(DataRow lRowCampo)
        {
            return ReporteEstandarUtil.GetDataFieldName(lRowCampo, "{DataFieldRutaAd}");
        }

        protected void InitRutaGrid(int liCodAtributo, int liCodAtributoAd, DataRow ldataRow)
        {
            string lsDataField;
            bool lbInitRuta = false;
            if (liCodAtributoAd != 0
                && !String.IsNullOrEmpty(lsDataField = GetRutaDataFieldAd(pDSCampos.Tables["Campos"], liCodAtributo, liCodAtributoAd)))
            {
                lbInitRuta = InitRuta(liCodAtributoAd, ldataRow[lsDataField]);
            }
            if (!lbInitRuta)
            {
                lsDataField = GetRutaDataField(pDSCampos.Tables["Campos"], liCodAtributo);
                lbInitRuta = InitRuta(liCodAtributo, ldataRow[lsDataField]);
            }

            if (lbInitRuta && pSubReporte.Fields != null)
            {
                SetTitleRuta(liCodAtributo, ldataRow, pDSCampos.Tables["Campos"]);
                SetSubReporteParams(ldataRow, pDSCampos.Tables["Campos"]);
                SetSubReporteParams();
                pSubReporte.InitSiguienteRuta();
            }
        }

        protected void InitRutaMatrizY(int liCodAtributo, int liCodAtributoAd, DataRow ldataRow)
        {
            string lsDataField;
            bool lbInitRuta = false;
            if (liCodAtributoAd != 0
                && !String.IsNullOrEmpty(lsDataField = GetRutaDataFieldAd(pDSCampos.Tables["CamposY"], liCodAtributo, liCodAtributoAd)))
            {
                lbInitRuta = InitRuta(liCodAtributoAd, ldataRow[lsDataField]);
            }
            if (!lbInitRuta
                && liCodAtributoAd != 0
                && !String.IsNullOrEmpty(lsDataField = GetRutaDataFieldAd(pDSCampos.Tables["TotalizadosXYTotX"], liCodAtributo, liCodAtributoAd)))
            {
                lbInitRuta = InitRuta(liCodAtributoAd, ldataRow[lsDataField]);
            }
            if (!lbInitRuta)
            {
                lsDataField = GetRutaDataField(pDSCampos.Tables["CamposY"], liCodAtributo);
                if (String.IsNullOrEmpty(lsDataField))
                {
                    lsDataField = GetRutaDataField(pDSCampos.Tables["TotalizadosXYTotX"], liCodAtributo);
                }
                lbInitRuta = InitRuta(liCodAtributo, ldataRow[lsDataField]);
            }

            if (lbInitRuta && pSubReporte.Fields != null)
            {
                SetTitleRuta(liCodAtributo, ldataRow, pDSCampos.Tables["CamposY"]);
                SetSubReporteParams(ldataRow, pDSCampos.Tables["CamposY"]);
                SetSubReporteParams(ldataRow, pDSCampos.Tables["TotalizadosXYTotX"]);
                SetSubReporteParams();
                pSubReporte.InitSiguienteRuta();
            }
        }

        protected void InitRutaMatrizXY(int liCodAtributo, int liCodAtributoAd, int liRowX, DataRow ldataRow)
        {
            bool lbInitRuta = false;
            int liCodAtributoX;
            string lsDataField;
            //Primero reviso por el atributo adicional
            if (liCodAtributoAd != 0
                && !String.IsNullOrEmpty(lsDataField = GetRutaDataFieldAd(pDSCampos.Tables["CamposXY"], liCodAtributo, liCodAtributoAd)))
            {
                lsDataField = "ColX" + liRowX + "_" + lsDataField;
                lbInitRuta = InitRuta(liCodAtributoAd, ldataRow[lsDataField]);
            }

            //Ahora reviso los valores de los campos X
            if (!lbInitRuta)
            {
                foreach (DataRow lRowCampo in pDSCampos.Tables["CamposX"].Rows)
                {
                    liCodAtributoX = (int)lRowCampo["{Atrib}"];
                    lsDataField = GetRutaDataField(pDSCampos.Tables["CamposX"], liCodAtributoX);
                    lbInitRuta = InitRuta(liCodAtributoX, pDSCampos.Tables["ValoresEjeX"].Rows[liRowX][lsDataField]);
                    if (lbInitRuta)
                    {
                        break;
                    }
                }
            }

            //Ahora reviso con los campos XY
            if (!lbInitRuta)
            {
                lsDataField = "ColX" + liRowX + "_" + GetRutaDataField(pDSCampos.Tables["CamposXY"], liCodAtributo);
                lbInitRuta = InitRuta(liCodAtributo, ldataRow[lsDataField]);
            }

            if (lbInitRuta && pSubReporte.Fields != null)
            {
                SetTitleRutaXY(liCodAtributo, liRowX, ldataRow);
                SetSubReporteParams(ldataRow, pDSCampos.Tables["CamposY"]);
                SetSubReporteParams(pDSCampos.Tables["ValoresEjeX"].Rows[liRowX], pDSCampos.Tables["CamposX"]);
                SetSubReporteParamsXY(ldataRow, liRowX);
                SetSubReporteParams();
                pSubReporte.InitSiguienteRuta();
            }
        }

        protected void SetTitleRuta(int liCodAtributo, DataRow ldataRow, DataTable lKDBCampos)
        {
            string lsColumna = null;
            if (pSubReporte.Fields.ContainsConfigValue(liCodAtributo)
                && lKDBCampos.Select("[{Atrib}] = " + liCodAtributo).Length > 0)
            {
                lsColumna = ReporteEstandarUtil.GetDataFieldName(lKDBCampos.Select("[{Atrib}] = " + liCodAtributo)[0]);
            }
            else
            {
                int liRepEstOrdenCampo;
                if (lKDBCampos.Select("[{Atrib}] = " + liCodAtributo).Length > 0)
                {
                    liRepEstOrdenCampo = (int)lKDBCampos.Select("[{Atrib}] = " + liCodAtributo)[0]["{RepEstOrdenCampo}"];
                }
                else
                {
                    liRepEstOrdenCampo = (int)lKDBCampos.Rows[0]["{RepEstOrdenCampo}"];
                }
                DataRow[] lRowsCampos = lKDBCampos.Select("[{RepEstOrdenCampo}] >= " + liRepEstOrdenCampo);
                foreach (DataRow lRowCampo in lRowsCampos)
                {
                    if (pSubReporte.Fields.ContainsConfigValue((int)lRowCampo["{Atrib}"]))
                    {
                        lsColumna = ReporteEstandarUtil.GetDataFieldName(lRowCampo);
                        break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(lsColumna))
            {
                pSubReporte.TitleRuta = ldataRow[lsColumna].ToString();
            }
        }

        protected void SetTitleRutaXY(int liCodAtributo, int liRowX, DataRow ldataRow)
        {
            string lsTitleRuta = "";
            int liCodAtributoX;
            string lsDataField;
            //Primero agrego los valores de los campos X
            foreach (DataRow lRowCampo in pDSCampos.Tables["CamposX"].Rows)
            {
                liCodAtributoX = (int)lRowCampo["{Atrib}"];
                if (pSubReporte.Fields.ContainsConfigValue(liCodAtributoX))
                {
                    lsDataField = ReporteEstandarUtil.GetDataFieldName(lRowCampo);
                    if (!String.IsNullOrEmpty(lsTitleRuta))
                    {
                        lsTitleRuta += " / ";
                    }
                    lsTitleRuta += pDSCampos.Tables["ValoresEjeX"].Rows[liRowX][lsDataField].ToString();
                }
            }

            //Ahora agrego los campos XY
            if (pSubReporte.Fields.ContainsConfigValue(liCodAtributo))
            {
                lsDataField = "ColX" + liRowX + "_" + ReporteEstandarUtil.GetDataFieldName(pDSCampos.Tables["CamposXY"].Select("[{Atrib}] = " + liCodAtributo)[0]);
                if (!String.IsNullOrEmpty(lsTitleRuta))
                {
                    lsTitleRuta += " / ";
                }
                lsTitleRuta += ldataRow[lsDataField].ToString();
            }

            //Reviso los campos Y en caso de no haber encontrado nada en los campos X ni en los campos XY
            if (String.IsNullOrEmpty(lsTitleRuta))
            {
                lsDataField = null;
                foreach (DataRow lRowCampo in pDSCampos.Tables["CamposY"].Rows)
                {
                    if (pSubReporte.Fields.ContainsConfigValue((int)lRowCampo["{Atrib}"]))
                    {
                        lsDataField = ReporteEstandarUtil.GetDataFieldName(lRowCampo);
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(lsDataField))
                {
                    pSubReporte.TitleRuta = ldataRow[lsDataField].ToString();
                }
            }
            else
            {
                pSubReporte.TitleRuta = lsTitleRuta;
            }
        }

        protected void SetSubReporteParams(DataRow ldataRow, DataTable lKDBCampos)
        {
            KeytiaBaseField lField;
            string lsColumna;
            foreach (DataRow lRow in lKDBCampos.Rows)
            {
                if (lRow["{Atrib}"] != DBNull.Value
                    && pSubReporte.Fields.ContainsConfigValue((int)lRow["{Atrib}"]))
                {
                    lsColumna = GetRutaDataField(lRow);
                    lField = pSubReporte.Fields.GetByConfigValue((int)lRow["{Atrib}"]);
                    lField.DataValue = ldataRow[lsColumna];
                }
                if (lKDBCampos.Columns.Contains("{AtribAd}")
                    && lKDBCampos.Columns.Contains("{DataFieldRutaAd}")
                    && lRow["{AtribAd}"] != DBNull.Value
                    && lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && pSubReporte.Fields.ContainsConfigValue((int)lRow["{AtribAd}"]))
                {
                    lsColumna = GetRutaDataFieldAd(lRow);
                    lField = pSubReporte.Fields.GetByConfigValue((int)lRow["{AtribAd}"]);
                    lField.DataValue = ldataRow[lsColumna];
                }
            }
        }

        protected void SetSubReporteParamsXY(DataRow ldataRow, int liRowX)
        {
            KeytiaBaseField lField;
            string lsColumna;
            string lsPrefijo = "ColX" + liRowX + "_";
            foreach (DataRow lRow in pDSCampos.Tables["CamposXY"].Rows)
            {
                if (lRow["{Atrib}"] != DBNull.Value
                    && pSubReporte.Fields.ContainsConfigValue((int)lRow["{Atrib}"]))
                {
                    lsColumna = lsPrefijo + GetRutaDataField(lRow);
                    lField = pSubReporte.Fields.GetByConfigValue((int)lRow["{Atrib}"]);
                    lField.DataValue = ldataRow[lsColumna];
                }
                if (lRow["{AtribAd}"] != DBNull.Value
                    && lRow["{DataFieldRutaAd}"] != DBNull.Value
                    && pSubReporte.Fields.ContainsConfigValue((int)lRow["{AtribAd}"]))
                {
                    lsColumna = lsPrefijo + GetRutaDataFieldAd(lRow);
                    lField = pSubReporte.Fields.GetByConfigValue((int)lRow["{AtribAd}"]);
                    lField.DataValue = ldataRow[lsColumna];
                }
            }
        }

        protected void SetSubReporteParams()
        {
            KeytiaBaseField lSubField;
            string lsDataValueDelimiter;
            if (pFields != null)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    if ((lField.DSOControlDB.HasValue || lField.DataValue != "null")
                        && pSubReporte.Fields.ContainsConfigValue(lField.ConfigValue))
                    {
                        lSubField = pSubReporte.Fields.GetByConfigValue(lField.ConfigValue);
                        if (!lSubField.DSOControlDB.HasValue || lSubField.DataValue == "null")
                        {
                            if (lField is KeytiaVarcharField
                                || lField is KeytiaMultiSelectField)
                            {
                                lsDataValueDelimiter = lField.DSOControlDB.DataValueDelimiter;
                                lField.DSOControlDB.DataValueDelimiter = "";

                                if (lField is KeytiaMultiSelectField
                                    && lSubField is KeytiaAutoCompleteField
                                    && lField.DataValue.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                                {
                                    lSubField.DataValue = lField.DataValue;
                                }
                                else if (lField is KeytiaMultiSelectField
                                    && lSubField is KeytiaMultiSelectField)
                                {
                                    lSubField.DataValue = lField.DataValue;
                                }
                                else
                                {
                                    lSubField.DataValue = lField.DataValue;
                                }

                                lField.DSOControlDB.DataValueDelimiter = lsDataValueDelimiter;
                            }
                            else
                            {
                                lSubField.DataValue = lField.DataValue;
                            }
                        }
                    }
                }
            }
            FirePostSetSubReporteParams();
        }

        public void InitSiguienteRuta()
        {
            //JCM 20120411 Se comentarizo todo el codigo del metodo para que no hiciera los brincos de reportes ->

            //int liCodAtributoSiguiente;
            //int liCodAtributoSiguienteAd;

            //GetAtributoSiguienteRuta(out liCodAtributoSiguiente, out liCodAtributoSiguienteAd);
            //if (liCodAtributoSiguiente < 1)
            //{
            //    return;
            //}

            ////InitNumRegistros();
            //InitHTParam();
            //if (pHTParam.ContainsKey("iDisplayStart"))
            //{
            //    pHTParam.Remove("iDisplayStart");
            //}
            //if (pHTParam.ContainsKey("iDispalyLength"))
            //{
            //    pHTParam.Remove("iDispalyLength");
            //}
            //if (pHTParam.ContainsKey("SortCol"))
            //{
            //    pHTParam.Remove("SortCol");
            //}
            //if (pHTParam.ContainsKey("SortColInv"))
            //{
            //    pHTParam.Remove("SortColInv");
            //}
            //if (pHTParam.ContainsKey("SortDir"))
            //{
            //    pHTParam.Remove("SortDir");
            //}
            //if (pHTParam.ContainsKey("SortDirInv"))
            //{
            //    pHTParam.Remove("SortDirInv");
            //}


            //pHTParam.Add("iDisplayStart", 0);
            //pHTParam.Add("iDisplayLength", 10);
            //pHTParam.Add("SortCol", "");
            //pHTParam.Add("SortColInv", "");
            //pHTParam.Add("SortDir", "Asc");
            //pHTParam.Add("SortDirInv", "Desc");

            //string lsRepEstDataSource = ReporteEstandarUtil.ParseDataSource(pKDBRowDataSource["{RepEstDataSource}"].ToString(), pHTParam, pbReajustarParam);
            //if (pbModoDebug)
            //{
            //    ReporteEstandarUtil.LogDataSource(pvchCodIdioma, pTipoReporte, pKDBRowReporte["vchCodigo"].ToString(), "RepEstDataSource", lsRepEstDataSource);
            //}
            //DataTable lDataTable = DSODataAccess.Execute(lsRepEstDataSource);
            //NumeroRegistros = lDataTable.Rows.Count;

            //if (NumeroRegistros == 0 || NumeroRegistros > 1)
            //{
            //    //Si el numero de registros es cero o tengo mas de uno
            //    return;
            //}

            //if (pTipoReporte == TipoReporte.Tabular || pTipoReporte == TipoReporte.Resumido)
            //{
            //    RemoverReporte = true;
            //    InitRutaGrid(liCodAtributoSiguiente, liCodAtributoSiguienteAd, lDataTable.Rows[0]);
            //}
            //else if (pDSCampos.Tables["CamposY"].Select("[{Atrib}] = " + liCodAtributoSiguiente).Length > 0)
            //{
            //    RemoverReporte = true;
            //    InitRutaMatrizY(liCodAtributoSiguiente, liCodAtributoSiguienteAd, lDataTable.Rows[0]);
            //}
            //else if (pDSCampos.Tables["ValoresEjeX"].Rows.Count == 0 || pDSCampos.Tables["ValoresEjeX"].Rows.Count > 1)
            //{
            //    return;
            //}
            //else
            //{
            //    RemoverReporte = true;
            //    InitRutaMatrizXY(liCodAtributoSiguiente, liCodAtributoSiguienteAd, 1, lDataTable.Rows[0]);
            //}

            //pHTParam.Remove("iDisplayStart");
            //pHTParam.Remove("iDispalyLength");
            //pHTParam.Remove("SortCol");
            //pHTParam.Remove("SortColInv");
            //pHTParam.Remove("SortDir");
            //pHTParam.Remove("SortDirInv");

            //<- JCM 20120411 Se comentarizo todo el codigo del metodo para que no hiciera los brincos de reportes
        }

        protected void GetAtributoSiguienteRuta(out int liCodAtributo, out int liCodAtributoAd)
        {
            liCodAtributo = -1;
            liCodAtributoAd = 0;
            if (pKDBRelRutas.Rows.Count == 0 || !pbAreaReporte)
            {
                //Si no tengo rutas posibles o no tengo un grid
                return;
            }

            List<string> lstAtribCampos = new List<string>();
            if (pTipoReporte == TipoReporte.Tabular || pTipoReporte == TipoReporte.Resumido)
            {
                foreach (DataRow ldataRow in pDSCampos.Tables["Campos"].Rows)
                {
                    lstAtribCampos.Add(ldataRow["{Atrib}"].ToString());
                    if (ldataRow["{AtribAd}"] != DBNull.Value)
                    {
                        lstAtribCampos.Add(ldataRow["{AtribAd}"].ToString());
                    }
                }
            }
            else
            {
                foreach (DataRow ldataRow in pDSCampos.Tables["CamposY"].Rows)
                {
                    lstAtribCampos.Add(ldataRow["{Atrib}"].ToString());
                    if (ldataRow["{AtribAd}"] != DBNull.Value)
                    {
                        lstAtribCampos.Add(ldataRow["{AtribAd}"].ToString());
                    }
                }
                foreach (DataRow ldataRow in pDSCampos.Tables["CamposX"].Rows)
                {
                    lstAtribCampos.Add(ldataRow["{Atrib}"].ToString());
                }
                foreach (DataRow ldataRow in pDSCampos.Tables["CamposXY"].Rows)
                {
                    lstAtribCampos.Add(ldataRow["{Atrib}"].ToString());
                    if (ldataRow["{AtribAd}"] != DBNull.Value)
                    {
                        lstAtribCampos.Add(ldataRow["{AtribAd}"].ToString());
                    }
                }
            }
            if (lstAtribCampos.Count == 0)
            {
                lstAtribCampos.Add("-1");
            }
            DataRow[] ldraRutasPosibles = pKDBRelRutas.Select("[{Atrib}] in(" + String.Join(",", lstAtribCampos.ToArray()) + ")");
            int liRutasPosibles = ldraRutasPosibles.Length;
            if (liRutasPosibles == 0)
            {
                //Si con los atributos del grid no tengo rutas posibles o tengo mas de una
                return;
            }

            liCodAtributo = (int)ldraRutasPosibles[0]["{Atrib}"];
            if (liRutasPosibles > 1)
            {
                //Si tengo mas de una ruta posible verifico que todas sean con el mismo atributo
                foreach (DataRow ldataRow in ldraRutasPosibles)
                {
                    if ((int)ldataRow["{Atrib}"] != liCodAtributo)
                    {
                        liCodAtributo = -1;
                        break;
                    }
                }
            }

            //Determinar valores de atributo y atributo adicional
            DataRow lRowCampo;
            if (liCodAtributo > -1 && (pTipoReporte == TipoReporte.Tabular || pTipoReporte == TipoReporte.Resumido))
            {
                if (pDSCampos.Tables["Campos"].Select("[{Atrib}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["Campos"].Select("[{Atrib}] = " + liCodAtributo)[0];
                    if (lRowCampo["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                    }
                }
                else if (pDSCampos.Tables["Campos"].Select("[{AtribAd}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["Campos"].Select("[{AtribAd}] = " + liCodAtributo)[0];
                    liCodAtributo = (int)lRowCampo["{Atrib}"];
                    liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                }
                else
                {
                    liCodAtributo = -1;
                    liCodAtributoAd = 0;
                }
            }
            else if (liCodAtributo > -1)
            {
                if (pDSCampos.Tables["CamposY"].Select("[{Atrib}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["CamposY"].Select("[{Atrib}] = " + liCodAtributo)[0];
                    if (lRowCampo["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                    }
                }
                else if (pDSCampos.Tables["CamposY"].Select("[{AtribAd}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["CamposY"].Select("[{AtribAd}] = " + liCodAtributo)[0];
                    liCodAtributo = (int)lRowCampo["{Atrib}"];
                    liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                }
                else if (pDSCampos.Tables["CamposX"].Select("[{Atrib}] = " + liCodAtributo).Length == 1)
                {
                    liCodAtributoAd = 0;
                }
                else if (pDSCampos.Tables["CamposXY"].Select("[{Atrib}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["CamposXY"].Select("[{Atrib}] = " + liCodAtributo)[0];
                    if (lRowCampo["{AtribAd}"] != DBNull.Value)
                    {
                        liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                    }
                }
                else if (pDSCampos.Tables["CamposXY"].Select("[{AtribAd}] = " + liCodAtributo).Length == 1)
                {
                    lRowCampo = pDSCampos.Tables["CamposXY"].Select("[{AtribAd}] = " + liCodAtributo)[0];
                    liCodAtributo = (int)lRowCampo["{Atrib}"];
                    liCodAtributoAd = (int)lRowCampo["{AtribAd}"];
                }
                else
                {
                    liCodAtributo = -1;
                    liCodAtributoAd = 0;
                }
            }

        }

        #endregion

        protected DataColumnCollection GetRepColumnTypesTabular()
        {
            DataTable ldt = new DataTable();
            DataColumn ldataCol;

            foreach (DataRow lRow in pDSCampos.Tables["Campos"].Rows)
            {
                //Todos los campos visibles son de tipo string
                ldataCol = new DataColumn();
                ldataCol.ColumnName = ReporteEstandarUtil.GetDataFieldName(lRow);
                ldataCol.DataType = typeof(string);
                ldt.Columns.Add(ldataCol);
            }

            foreach (DataRow lRow in pDSCampos.Tables["Campos"].Rows)
            {
                if (pKDBRelRutas.Select("[{Atrib}] = " + lRow["{Atrib}"].ToString()).Length > 0
                    && lRow["{DataFieldRuta}"] != DBNull.Value)
                {
                    //Reviso si tiene tipo de dato, de lo contratrio asigno string
                    ldataCol = new DataColumn();
                    ldataCol.ColumnName = ReporteEstandarUtil.GetDataFieldName(lRow, "{DataFieldRuta}");
                    ldataCol.DataType = GetColumnType(lRow);
                    ldt.Columns.Add(ldataCol);
                }
            }

            return ldt.Columns;
        }

        protected DataColumnCollection GetRepColumnTypesMatricial()
        {
            DataTable ldt = new DataTable();
            DataColumn ldataCol;
            DataRow lRow;

            foreach (DSOGridClientColumn lCol in pGridReporte.Config.aoColumnDefs)
            {
                //Todos los campos visibles son de tipo string
                ldataCol = new DataColumn();
                ldataCol.ColumnName = lCol.sName;
                ldataCol.DataType = typeof(string);

                if (!lCol.bVisible)
                {
                    lRow = ReporteEstandarUtil.GetCampoByMatrizField(lCol.sName, "{DataFieldRuta}", pDSCampos, pDSCampos.Tables["ValoresEjeX"].Rows.Count);
                    ldataCol.DataType = GetColumnType(lRow);
                }

                ldt.Columns.Add(ldataCol);
            }

            return ldt.Columns;
        }

        protected Type GetColumnType(DataRow lRow)
        {
            Type lRetType = typeof(string);
            DataRow lKDBTypeRow;
            if (lRow != null
                && lRow["{Types}"] != DBNull.Value)
            {
                lKDBTypeRow = pKDBTypes.Select("iCodCatalogo = " + lRow["{Types}"])[0];
                if (lKDBTypeRow["vchCodigo"].ToString() == "Int"
                    || lKDBTypeRow["vchCodigo"].ToString() == "IntFormat"
                    || lKDBTypeRow["vchCodigo"].ToString() == "TimeSeg")
                {
                    lRetType = typeof(int);
                }
                else if (lKDBTypeRow["vchCodigo"].ToString() == "Float"
                    || lKDBTypeRow["vchCodigo"].ToString() == "Currency")
                {
                    lRetType = typeof(double);
                }
                else if (lKDBTypeRow["vchCodigo"].ToString() == "Date"
                    || lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
                {
                    lRetType = typeof(DateTime);
                }
            }

            return lRetType;
        }

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "RemoverSubReporte")
            {
                RemoverSubReporte();
            }
            else if (eventArgument == "btnPostRegresar")
            {
                FirePostRegresarClick();
            }
            else if (eventArgument == "ConfigurarParametros")
            {
                ConfigurarParametros();
            }
            else if (eventArgument.StartsWith("RutaGrid"))
            {
                //El argumento tiene la siguiente estructura: RutaGrid:iCodAtributo:iCodAtributoAd:jsonData
                string[] lsArgs = eventArgument.Split(':');
                int liCodAtributo = int.Parse(lsArgs[1]);
                int liCodAtributoAd = int.Parse(lsArgs[2]);
                string lsjsonData = eventArgument.Substring(lsArgs[0].Length + lsArgs[1].Length + lsArgs[2].Length + 3);
                DataColumnCollection lcolumns = GetRepColumnTypesTabular();
                DataTable ldt = DSOControl.DeserializeDataTableJSON(HttpUtility.UrlDecode(lsjsonData), lcolumns);
                InitHTParam();
                InitRutaGrid(liCodAtributo, liCodAtributoAd, ldt.Rows[0]);
            }
            else if (eventArgument.StartsWith("RutaMatrizY"))
            {
                //El argumento tiene la siguiente estructura: RutaMatrizY:iCodAtributo:iCodAtributoAd:jsonData
                string[] lsArgs = eventArgument.Split(':');
                int liCodAtributo = int.Parse(lsArgs[1]);
                int liCodAtributoAd = int.Parse(lsArgs[2]);
                string lsjsonData = eventArgument.Substring(lsArgs[0].Length + lsArgs[1].Length + lsArgs[2].Length + 3);

                InitHTParam();
                DataColumnCollection lcolumns = GetRepColumnTypesMatricial();
                DataTable ldt = DSOControl.DeserializeDataTableJSON(HttpUtility.UrlDecode(lsjsonData), lcolumns);
                InitRutaMatrizY(liCodAtributo, liCodAtributoAd, ldt.Rows[0]);
            }
            else if (eventArgument.StartsWith("RutaMatrizXY"))
            {
                //El argumento tiene la siguiente estructura: RutaMatrizXY:iCodAtributo:iCodAtributoAd:iRowX:jsonData
                string[] lsArgs = eventArgument.Split(':');
                int liCodAtributo = int.Parse(lsArgs[1]);
                int liCodAtributoAd = int.Parse(lsArgs[2]);
                int liRowX = int.Parse(lsArgs[3]);
                string lsjsonData = eventArgument.Substring(lsArgs[0].Length + lsArgs[1].Length + lsArgs[2].Length + lsArgs[3].Length + 4);

                InitHTParam();
                DataColumnCollection lcolumns = GetRepColumnTypesMatricial();
                DataTable ldt = DSOControl.DeserializeDataTableJSON(HttpUtility.UrlDecode(lsjsonData), lcolumns);
                InitRutaMatrizXY(liCodAtributo, liCodAtributoAd, liRowX, ldt.Rows[0]);
            }
        }

        #endregion

        #region WebMethods

        public static DSOGridServerResponse GetRepTabular(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;
                string lvchCodIdioma = Globals.GetCurrentLanguage();

                DataRow lRowReporte = lKDB.GetHisRegByEnt("RepEst", "Tabular", "iCodCatalogo = " + iCodReporte).Rows[0];
                DataRow lRowDataSource = lKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + lRowReporte["{DataSourceRep}"]).Rows[0];

                DataSet lDSCampos = ReporteEstandarUtil.GetCamposTabular(iCodReporte);

                DataTable lKDBAggregates = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'AggregateFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
                DataTable lKDBTypes = lKDB.GetHisRegByEnt("Types", "Tipos de Datos");

                Hashtable lHTClientCols = GetHTClientColsTabular(gsRequest, lDSCampos);

                Hashtable lHTParam = new Hashtable();
                AgregarParamSession(lHTParam);
                ReporteEstandarUtil.AgregarParamDataFieldsTabular(lHTParam, lDSCampos);
                AgregarParamGridTabular(lHTParam, lDSCampos, gsRequest);
                AgregarParamValores(lHTParam, parametros);

                int liBanderas = (int)lRowReporte["{BanderasRepEstandar}"];
                bool lbReajustarParam = (liBanderas & 64) == 64;
                bool lbModoDebug = (liBanderas & 128) == 128;

                string lsRepEstDataSource = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSource}"].ToString(), lHTParam, lbReajustarParam);
                //string lsRepEstDataSourceNumReg = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSourceNumReg}"].ToString(), lHTParam, lbReajustarParam);

                if (lbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Tabular, lRowReporte["vchCodigo"].ToString(), "RepEstDataSource", lsRepEstDataSource);
                    //ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Tabular, lRowReporte["vchCodigo"].ToString(), "RepEstDataSourceNumReg", lsRepEstDataSourceNumReg);
                }

                lgsrRet.sEcho = gsRequest.sEcho;
                //lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsRepEstDataSourceNumReg);
                //lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                lgsrRet.iTotalRecords = iNumeroRegistros;
                lgsrRet.iTotalDisplayRecords = iNumeroRegistros;

                ldt = DSODataAccess.ExecuteQueryRep(lsRepEstDataSource);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = lRowReporte["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                ProcesaTotalizadosPagina(gsRequest, lgsrRet, ldt, GetHTCamposTotTabular(lDSCampos, lKDBAggregates));
                ProcesaFormatoColumnas(lgsrRet, ldt, lHTClientCols, lKDBTypes);

                return lgsrRet;
            }
            catch (Exception ex)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                if (ex is SqlException && ((SqlException)ex).Number == -2)
                {
                    //Si fue una exception de sql por timeout lanzo el mensaje de error de timeout
                    throw new KeytiaWebException(true, "ErrSqlTimeout", ex, lsTitulo);
                }
                else
                {
                    //lanzo el mensaje generico de error
                    throw new KeytiaWebException(true, "ErrGridData", ex, lsTitulo);
                }
            }
        }

        public static DSOGridServerResponse GetRepResumido(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;
                string lvchCodIdioma = Globals.GetCurrentLanguage();

                DataRow lRowReporte = lKDB.GetHisRegByEnt("RepEst", "Resumido", "iCodCatalogo = " + iCodReporte).Rows[0];
                DataRow lRowDataSource = lKDB.GetHisRegByEnt("DataSourceRep", "DataSource Reportes", "iCodCatalogo = " + lRowReporte["{DataSourceRep}"]).Rows[0];

                DataSet lDSCampos = ReporteEstandarUtil.GetCamposResumido(iCodReporte);

                DataTable lKDBAggregates = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'AggregateFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
                DataTable lKDBTypes = lKDB.GetHisRegByEnt("Types", "Tipos de Datos");

                string lsOperAgrupacion;
                if (lRowReporte["{RepTipoResumen}"] == DBNull.Value
                    || int.Parse(lRowReporte["{RepTipoResumen}"].ToString()) == 1)
                {
                    lsOperAgrupacion = "with rollup";
                }
                else
                {
                    lsOperAgrupacion = "with cube";
                }

                Hashtable lHTClientCols = GetHTClientColsTabular(gsRequest, lDSCampos);

                Hashtable lHTParam = new Hashtable();
                AgregarParamSession(lHTParam);
                ReporteEstandarUtil.AgregarParamDataFieldsResumido(lHTParam, lDSCampos, lsOperAgrupacion);
                AgregarParamGridResumido(lHTParam, lDSCampos, gsRequest);
                AgregarParamValores(lHTParam, parametros);

                int liBanderas = (int)lRowReporte["{BanderasRepEstandar}"];
                bool lbReajustarParam = (liBanderas & 64) == 64;
                bool lbModoDebug = (liBanderas & 128) == 128;

                string lsRepEstDataSource = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSource}"].ToString(), lHTParam, lbReajustarParam);
                //string lsRepEstDataSourceNumReg = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSourceNumReg}"].ToString(), lHTParam, lbReajustarParam);

                if (lbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Resumido, lRowReporte["vchCodigo"].ToString(), "RepEstDataSource", lsRepEstDataSource);
                    //ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Resumido, lRowReporte["vchCodigo"].ToString(), "RepEstDataSourceNumReg", lsRepEstDataSourceNumReg);
                }

                lgsrRet.sEcho = gsRequest.sEcho;
                //lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsRepEstDataSourceNumReg);
                //lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                lgsrRet.iTotalRecords = iNumeroRegistros;
                lgsrRet.iTotalDisplayRecords = iNumeroRegistros;

                ldt = DSODataAccess.ExecuteQueryRep(lsRepEstDataSource);

                ProcesaSubTotalizados(ldt, lDSCampos);
                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = lRowReporte["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                ProcesaFormatoColumnas(lgsrRet, ldt, lHTClientCols, lKDBTypes);

                return lgsrRet;
            }
            catch (Exception ex)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                if (ex is SqlException && ((SqlException)ex).Number == -2)
                {
                    //Si fue una exception de sql por timeout lanzo el mensaje de error de timeout
                    throw new KeytiaWebException(true, "ErrSqlTimeout", ex, lsTitulo);
                }
                else
                {
                    //lanzo el mensaje generico de error
                    throw new KeytiaWebException(true, "ErrGridData", ex, lsTitulo);
                }
            }
        }

        public static DSOGridServerResponse GetRepMatricial(DSOGridServerRequest gsRequest, int iCodReporte, int iNumeroRegistros, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;
                string lvchCodIdioma = Globals.GetCurrentLanguage();

                DataRow lRowReporte = lKDB.GetHisRegByEnt("RepEst", "Matricial", "iCodCatalogo = " + iCodReporte).Rows[0];
                DataRow lRowDataSource = lKDB.GetHisRegByEnt("DataSourceRepMat", "DataSource Reportes", "iCodCatalogo = " + lRowReporte["{DataSourceRepMat}"]).Rows[0];

                DataSet lDSCampos = ReporteEstandarUtil.GetCamposMatricial(iCodReporte);

                DataTable lKDBAggregates = lKDB.GetHisRegByEnt("Valores", "Valores", "{Atrib} = (select iCodRegistro from Catalogos where vchCodigo = 'AggregateFunc' and iCodCatalogo = (select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Atrib'))");
                DataTable lKDBTypes = lKDB.GetHisRegByEnt("Types", "Tipos de Datos");

                string lsDefaultSortCol = ReporteEstandarUtil.GetDataFieldName(lDSCampos.Tables["CamposY"].Rows[0], false);
                Hashtable lHTClientCols = GetHTClientColsMatricial(gsRequest, lDSCampos);

                Hashtable lHTParam = new Hashtable();
                AgregarParamSession(lHTParam);
                AgregarParamGridMatricial(lHTParam, lDSCampos, gsRequest);
                AgregarParamValores(lHTParam, parametros);
                ReporteEstandarUtil.AgregarParamDataFieldsMatricial(lHTParam, lDSCampos);

                int liBanderas = (int)lRowReporte["{BanderasRepEstandar}"];
                bool lbReajustarParam = (liBanderas & 64) == 64;
                bool lbModoDebug = (liBanderas & 128) == 128;

                ReporteEstandarUtil.GetValoresEjeX(lvchCodIdioma, lRowReporte, lRowDataSource, lDSCampos, lHTParam, lbReajustarParam);

                string lsRepEstDataSource = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSource}"].ToString(), lHTParam, lbReajustarParam);
                //string lsRepEstDataSourceNumReg = ReporteEstandarUtil.ParseDataSource(lRowDataSource["{RepEstDataSourceNumReg}"].ToString(), lHTParam, lbReajustarParam);

                if (lbModoDebug)
                {
                    ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Matricial, lRowReporte["vchCodigo"].ToString(), "RepEstDataSource", lsRepEstDataSource);
                    //ReporteEstandarUtil.LogDataSource(lvchCodIdioma, TipoReporte.Matricial, lRowReporte["vchCodigo"].ToString(), "RepEstDataSourceNumReg", lsRepEstDataSourceNumReg);
                }

                lgsrRet.sEcho = gsRequest.sEcho;
                //lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsRepEstDataSourceNumReg);
                //lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                lgsrRet.iTotalRecords = iNumeroRegistros;
                lgsrRet.iTotalDisplayRecords = iNumeroRegistros;

                ldt = DSODataAccess.ExecuteQueryRep(lsRepEstDataSource);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = lRowReporte["{" + Globals.GetCurrentLanguage() + "}"].ToString();
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                ProcesaTotalizadosPagina(gsRequest, lgsrRet, ldt, GetHTCamposTotMatricial(lDSCampos, lKDBAggregates));
                ProcesaFormatoColumnas(lgsrRet, ldt, lHTClientCols, lKDBTypes);

                return lgsrRet;
            }
            catch (Exception ex)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "AreaReporte");
                if (ex is SqlException && ((SqlException)ex).Number == -2)
                {
                    //Si fue una exception de sql por timeout lanzo el mensaje de error de timeout
                    throw new KeytiaWebException(true, "ErrSqlTimeout", ex, lsTitulo);
                }
                else
                {
                    //lanzo el mensaje generico de error
                    throw new KeytiaWebException(true, "ErrGridData", ex, lsTitulo);
                }
            }
        }

        protected static Hashtable GetHTClientColsTabular(DSOGridServerRequest gsRequest, DataSet lDSCampos)
        {
            Hashtable lHTClientCols = new Hashtable();
            DataRow lRowCampo;
            string[] laColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string lsColumn in laColumns)
            {
                lRowCampo = ReporteEstandarUtil.GetCampoByDataField(lsColumn, lDSCampos.Tables["Campos"]);
                lHTClientCols.Add(lsColumn, lRowCampo);
            }

            return lHTClientCols;
        }

        protected static Hashtable GetHTClientColsMatricial(DSOGridServerRequest gsRequest, DataSet lDSCampos)
        {
            Hashtable lHTClientCols = new Hashtable();
            DataRow lRowCampo;
            string[] laColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int liColXNum = laColumns.Length;
            foreach (string lsColumn in laColumns)
            {
                lRowCampo = ReporteEstandarUtil.GetCampoByMatrizField(lsColumn, lDSCampos, liColXNum);
                lHTClientCols.Add(lsColumn, lRowCampo);
            }

            return lHTClientCols;
        }

        protected static void AgregarParamSession(Hashtable lHTParam)
        {
            lHTParam.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
            lHTParam.Add("iCodPerfil", HttpContext.Current.Session["iCodPerfil"]);
            lHTParam.Add("vchCodIdioma", Globals.GetCurrentLanguage());
            lHTParam.Add("vchCodMoneda", Globals.GetCurrentCurrency());
            lHTParam.Add("Schema", DSODataContext.Schema);
        }

        protected static void AgregarParamGridTabular(Hashtable lHTParam, DataSet lDSCampos, DSOGridServerRequest gsRequest)
        {
            lHTParam.Add("iDisplayStart", gsRequest.iDisplayStart);
            lHTParam.Add("iDisplayLength", gsRequest.iDisplayLength);

            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = new Hashtable();
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["Campos"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            string[] laColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            DataRow lRowCampo;
            int lidx;
            if (gsRequest.iSortCol.Count > 0)
            {
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                for (lidx = 0; lidx < gsRequest.iSortCol.Count; lidx++)
                {
                    lsOrderCol = laColumns[gsRequest.iSortCol[lidx]];
                    lRowCampo = null;
                    if (lHTClientCols.ContainsKey(lsOrderCol))
                    {
                        lRowCampo = lHTClientCols[lsOrderCol] as DataRow;
                    }
                    if (lRowCampo != null)
                    {
                        lstOrdenColumnas.Remove(lsOrderCol);
                        lsOrderCol = "[" + lsOrderCol + "]";
                        switch (gsRequest.sSortDir[lidx].ToLower())
                        {
                            case "desc":
                                lsOrderDir = " Desc";
                                lsOrderDirInv = " Asc";
                                break;
                            default:
                                lsOrderDir = " Asc";
                                lsOrderDirInv = " Desc";
                                break;
                        }
                        if (lsbSortCol.Length > 0)
                        {
                            lsbSortCol.Append(",");
                            lsbSortColInv.Append(",");
                        }
                        else
                        {
                            //guardar el orden de la primer columna
                            lsSortDir = lsOrderDir.Trim();
                            lsSortDirInv = lsOrderDirInv.Trim();
                        }
                        lsbSortCol.Append(lsOrderCol + lsOrderDir);
                        lsbSortColInv.Append(lsOrderCol + lsOrderDirInv);
                    }
                }
            }

            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            lHTParam.Add("SortCol", lsbSortCol.ToString());
            lHTParam.Add("SortColInv", lsbSortColInv.ToString());

            lHTParam.Add("SortDir", lsSortDir);
            lHTParam.Add("SortDirInv", lsSortDirInv);
        }

        protected static void AgregarParamGridResumido(Hashtable lHTParam, DataSet lDSCampos, DSOGridServerRequest gsRequest)
        {
            lHTParam.Add("iDisplayStart", gsRequest.iDisplayStart);
            lHTParam.Add("iDisplayLength", gsRequest.iDisplayLength);

            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = new Hashtable();
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["Campos"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                lHTClientCols.Add(lsOrderCol, lViewRow.Row);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            string[] laColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            DataRow lRowCampo;
            int lidx;
            if (gsRequest.iSortCol.Count > 0)
            {
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                for (lidx = 0; lidx < gsRequest.iSortCol.Count; lidx++)
                {
                    lsOrderCol = laColumns[gsRequest.iSortCol[lidx]];
                    lRowCampo = null;
                    if (lHTClientCols.ContainsKey(lsOrderCol))
                    {
                        lRowCampo = lHTClientCols[lsOrderCol] as DataRow;
                    }
                    if (lRowCampo != null)
                    {
                        lstOrdenColumnas.Remove(lsOrderCol);
                        lsOrderCol = "[" + lsOrderCol + "]";
                        switch (gsRequest.sSortDir[lidx].ToLower())
                        {
                            case "desc":
                                lsOrderDir = " Desc";
                                lsOrderDirInv = " Asc";
                                break;
                            default:
                                lsOrderDir = " Asc";
                                lsOrderDirInv = " Desc";
                                break;
                        }
                        if (lsbSortCol.Length > 0)
                        {
                            lsbSortCol.Append(",");
                            lsbSortColInv.Append(",");
                        }
                        else
                        {
                            //guardar el orden de la primer columna
                            lsSortDir = lsOrderDir.Trim();
                            lsSortDirInv = lsOrderDirInv.Trim();
                        }

                        if (lDSCampos.Tables["Agrupadores"].Select("iCodRegistro = " + lRowCampo["iCodRegistro"]).Length > 0)
                        {
                            if (ReporteEstandarUtil.IsValidGroupByField(lRowCampo))
                            {
                                lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Asc,");
                                lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Desc,");
                            }
                            if (lRowCampo["{DataFieldRuta}"] != DBNull.Value
                                && ReporteEstandarUtil.IsValidGroupByField(lRowCampo, "{DataFieldRuta}"))
                            {
                                lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Asc,");
                                lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Desc,");
                            }
                        }
                        lsbSortCol.AppendLine(lsOrderCol + lsOrderDir);
                        lsbSortColInv.AppendLine(lsOrderCol + lsOrderDirInv);
                    }
                }
            }

            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lDSCampos.Tables["Agrupadores"].Select("iCodRegistro = " + lRowCampo["iCodRegistro"]).Length > 0)
                {
                    if (ReporteEstandarUtil.IsValidGroupByField(lRowCampo))
                    {
                        lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Asc,");
                        lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, false) + " Desc,");
                    }
                    if (lRowCampo["{DataFieldRuta}"] != DBNull.Value
                        && ReporteEstandarUtil.IsValidGroupByField(lRowCampo, "{DataFieldRuta}"))
                    {
                        lsbSortCol.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Asc,");
                        lsbSortColInv.Append(ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}", false) + " Desc,");
                    }
                }
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            lHTParam.Add("SortCol", lsbSortCol.ToString());
            lHTParam.Add("SortColInv", lsbSortColInv.ToString());

            lHTParam.Add("SortDir", lsSortDir);
            lHTParam.Add("SortDirInv", lsSortDirInv);
        }

        protected static void AgregarParamGridMatricial(Hashtable lHTParam, DataSet lDSCampos, DSOGridServerRequest gsRequest)
        {
            lHTParam.Add("iDisplayStart", gsRequest.iDisplayStart);
            lHTParam.Add("iDisplayLength", gsRequest.iDisplayLength);

            //Obtengo orden default
            string lsOrderCol;
            string lsSortDir = null;
            string lsSortDirInv = null;
            Hashtable lHTClientCols = GetHTClientColsMatricial(gsRequest, lDSCampos);
            List<string> lstOrdenColumnas = new List<string>();
            DataView lDataView = lDSCampos.Tables["CamposY"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
                if (lsSortDir == null && int.Parse(lViewRow.Row["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsSortDir = "Asc";
                    lsSortDirInv = "Desc";
                }
                else if (lsSortDir == null)
                {
                    lsSortDir = "Desc";
                    lsSortDirInv = "Asc";
                }
            }
            lDataView = lDSCampos.Tables["TotalizadosXYTotX"].DefaultView;
            lDataView.Sort = "[{RepEstOrdenDatos}] ASC,[{RepEstOrdenCampo}] ASC";
            foreach (DataRowView lViewRow in lDataView)
            {
                lsOrderCol = ReporteEstandarUtil.GetDataFieldName(lViewRow.Row);
                lstOrdenColumnas.Add(lsOrderCol);
            }

            StringBuilder lsbSortCol = new StringBuilder();
            StringBuilder lsbSortColInv = new StringBuilder();
            string[] laColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            DataRow lRowCampo;
            int lidx;
            if (gsRequest.iSortCol.Count > 0)
            {
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                for (lidx = 0; lidx < gsRequest.iSortCol.Count; lidx++)
                {
                    lsOrderCol = laColumns[gsRequest.iSortCol[lidx]];
                    lRowCampo = null;
                    if (lHTClientCols.ContainsKey(lsOrderCol))
                    {
                        lRowCampo = lHTClientCols[lsOrderCol] as DataRow;
                    }
                    if (lRowCampo != null)
                    {
                        if (lstOrdenColumnas.Contains(lsOrderCol))
                        {
                            lstOrdenColumnas.Remove(lsOrderCol);
                        }
                        lsOrderCol = "[" + lsOrderCol + "]";
                        switch (gsRequest.sSortDir[lidx].ToLower())
                        {
                            case "desc":
                                lsOrderDir = " Desc";
                                lsOrderDirInv = " Asc";
                                break;
                            default:
                                lsOrderDir = " Asc";
                                lsOrderDirInv = " Desc";
                                break;
                        }
                        if (lsbSortCol.Length > 0)
                        {
                            lsbSortCol.Append(",");
                            lsbSortColInv.Append(",");
                        }
                        else
                        {
                            //guardar el orden de la primer columna
                            lsSortDir = lsOrderDir.Trim();
                            lsSortDirInv = lsOrderDirInv.Trim();
                        }
                        lsbSortCol.Append(lsOrderCol + lsOrderDir);
                        lsbSortColInv.Append(lsOrderCol + lsOrderDirInv);
                    }
                }
            }

            foreach (string lsCol in lstOrdenColumnas)
            {
                if (lsbSortCol.Length > 0)
                {
                    lsbSortCol.Append(",");
                    lsbSortColInv.Append(",");
                }
                lRowCampo = lHTClientCols[lsCol] as DataRow;
                if (lRowCampo["{RepEstDirDatos}"] == DBNull.Value
                    || int.Parse(lRowCampo["{RepEstDirDatos}"].ToString()) == 1)
                {
                    lsbSortCol.Append("[" + lsCol + "] Asc");
                    lsbSortColInv.Append("[" + lsCol + "] Desc");
                }
                else
                {
                    lsbSortCol.Append("[" + lsCol + "] Desc");
                    lsbSortColInv.Append("[" + lsCol + "] Asc");
                }
            }

            lHTParam.Add("SortCol", lsbSortCol.ToString());
            lHTParam.Add("SortColInv", lsbSortColInv.ToString());

            lHTParam.Add("SortDir", lsSortDir);
            lHTParam.Add("SortDirInv", lsSortDirInv);
        }

        protected static void AgregarParamValores(Hashtable lHTParam, List<Parametro> parametros)
        {
            foreach (Parametro lParam in parametros)
            {
                lHTParam.Add(lParam.Name, lParam.Value);
            }
        }

        protected static Hashtable GetHTCamposTotTabular(DataSet lDSCampos, DataTable lKDBAggregates)
        {
            Hashtable lHTCamposTot = new Hashtable();
            string lsAggregate;
            string lsDataField;
            foreach (DataRow lRow in lDSCampos.Tables["Totalizados"].Rows)
            {
                lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString();
                lsDataField = ReporteEstandarUtil.GetDataFieldName(lRow);
                lHTCamposTot.Add(lsDataField, lsAggregate);
            }

            return lHTCamposTot;
        }

        protected static Hashtable GetHTCamposTotMatricial(DataSet lDSCampos, DataTable lKDBAggregates)
        {
            Hashtable lHTCamposTot = new Hashtable();
            string lsAggregate;
            string lsDataField;
            string lsDataFieldX;
            int lidx;
            foreach (DataRow lRow in lDSCampos.Tables["TotalizadosY"].Rows)
            {
                if (lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString();
                    lsDataField = ReporteEstandarUtil.GetDataFieldName(lRow);
                    lHTCamposTot.Add(lsDataField, lsAggregate);
                }
            }
            foreach (DataRow lRow in lDSCampos.Tables["TotalizadosXYTotY"].Rows)
            {
                if (lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString();
                    lsDataField = ReporteEstandarUtil.GetDataFieldName(lRow);
                    for (lidx = 0; lidx < lDSCampos.Tables["ValoresEjeX"].Rows.Count; lidx++)
                    {
                        lsDataFieldX = "ColX" + lidx + "_" + lsDataField;
                        lHTCamposTot.Add(lsDataFieldX, lsAggregate);
                    }
                }
            }
            foreach (DataRow lRow in lDSCampos.Tables["TotalizadosXYTotX"].Rows)
            {
                if (lRow["{AggregateFunc}"] != DBNull.Value)
                {
                    lsAggregate = lKDBAggregates.Select("[{Value}] = " + lRow["{AggregateFunc}"])[0]["vchCodigo"].ToString();
                    lsDataField = ReporteEstandarUtil.GetDataFieldName(lRow);
                    lHTCamposTot.Add(lsDataField, lsAggregate);
                }
            }

            return lHTCamposTot;
        }

        public static void ProcesaSubTotalizados(DataTable ldt, DataSet lDSCampos)
        {
            string lsDataField;
            string lsGroupingField;
            ldt.Columns.Add(new DataColumn("bSubTotalPag", typeof(int)));
            ldt.Columns.Add(new DataColumn("TrCssClass", typeof(string)));
            bool lbProcesarRuta;
            DataRow[] ldataRowsResumen;
            foreach (DataRow lRowCampo in lDSCampos.Tables["Agrupadores"].Rows)
            {
                lbProcesarRuta = true;
                lsDataField = ReporteEstandarUtil.GetDataFieldName(lRowCampo);
                lsGroupingField = ReporteEstandarUtil.GetGroupingFieldName(lRowCampo);
                if (ldt.Columns.Contains(lsGroupingField))
                {
                    ldataRowsResumen = ldt.Select("[" + lsGroupingField + "] = 1");
                    if (ldataRowsResumen.Length > 0)
                    {
                        lbProcesarRuta = false;
                    }
                    foreach (DataRow ldataRow in ldataRowsResumen)
                    {
                        ldataRow["TrCssClass"] = "SubTotalResumen";
                        if (ldt.Columns[lsDataField].DataType == typeof(string))
                        {
                            ldataRow[lsDataField] = Globals.GetMsgWeb(false, "SubTotalResumen");
                        }
                    }
                }

                if (lbProcesarRuta && lRowCampo["{DataFieldRuta}"] != DBNull.Value)
                {
                    lsGroupingField = ReporteEstandarUtil.GetGroupingFieldName(lRowCampo, "{DataFieldRuta}");
                    if (ldt.Columns.Contains(lsGroupingField))
                    {
                        ldataRowsResumen = ldt.Select("[" + lsGroupingField + "] = 1");
                        foreach (DataRow ldataRow in ldataRowsResumen)
                        {
                            ldataRow["TrCssClass"] = "SubTotalResumen";
                            if (ldt.Columns[lsDataField].DataType == typeof(string))
                            {
                                ldataRow[lsDataField] = Globals.GetMsgWeb(false, "SubTotalResumen");
                            }
                        }
                    }
                }
            }
        }

        protected static void ProcesaTotalizadosPagina(DSOGridServerRequest gsRequest, DSOGridServerResponse lgsrRet, DataTable ldt, Hashtable lHTCamposTot)
        {
            //Agrego SubTotales a nivel de pagina solo cuando es mas de una pagina
            if (gsRequest.iDisplayStart == 0
                && gsRequest.iDisplayLength >= lgsrRet.iTotalRecords)
            {
                return;
            }

            //Proceso los totalizados de la pagina
            DataRow lRowTotPagina = ldt.NewRow();
            string lsAggregateFunc;

            bool lbAddTotRow = false;
            bool lbSubTotal = false;
            foreach (DataColumn lDataCol in ldt.Columns)
            {
                lsAggregateFunc = null;
                if (lHTCamposTot.ContainsKey(lDataCol.ColumnName))
                {
                    lsAggregateFunc = lHTCamposTot[lDataCol.ColumnName].ToString();
                }

                if (lDataCol.DataType == typeof(string) && !lbSubTotal)
                {
                    lbSubTotal = true;
                    lRowTotPagina[lDataCol] = Globals.GetMsgWeb(false, "SubTotal");
                }

                if (!String.IsNullOrEmpty(lsAggregateFunc))
                {
                    lbAddTotRow = true;
                    lRowTotPagina[lDataCol] = 0;

                    if (lsAggregateFunc == "SUM"
                        || lsAggregateFunc == "AVG")
                    {
                        foreach (DataRow lDataRow in ldt.Rows)
                        {
                            if (lDataRow[lDataCol].GetType() == typeof(int))
                            {
                                lRowTotPagina[lDataCol] = (int)lRowTotPagina[lDataCol] + (int)lDataRow[lDataCol];
                            }
                            else if (lDataRow[lDataCol].GetType() == typeof(short))
                            {
                                lRowTotPagina[lDataCol] = (short)lRowTotPagina[lDataCol] + (short)lDataRow[lDataCol];
                            }
                            else if (lDataRow[lDataCol].GetType() == typeof(decimal))
                            {
                                lRowTotPagina[lDataCol] = (decimal)lRowTotPagina[lDataCol] + (decimal)lDataRow[lDataCol];
                            }
                            else if (lDataRow[lDataCol].GetType() == typeof(double))
                            {
                                lRowTotPagina[lDataCol] = (double)lRowTotPagina[lDataCol] + (double)lDataRow[lDataCol];
                            }
                            else if (lDataRow[lDataCol].GetType() == typeof(float))
                            {
                                lRowTotPagina[lDataCol] = (float)lRowTotPagina[lDataCol] + (float)lDataRow[lDataCol];
                            }
                        }
                    }

                    if (lsAggregateFunc == "AVG" && ldt.Rows.Count > 0)
                    {
                        if (lDataCol.DataType == typeof(int))
                        {
                            lRowTotPagina[lDataCol] = (int)lRowTotPagina[lDataCol] / ldt.Rows.Count;
                        }
                        else if (lDataCol.DataType == typeof(short))
                        {
                            lRowTotPagina[lDataCol] = (short)lRowTotPagina[lDataCol] / ldt.Rows.Count;
                        }
                        else if (lDataCol.DataType == typeof(decimal))
                        {
                            lRowTotPagina[lDataCol] = (decimal)lRowTotPagina[lDataCol] / ldt.Rows.Count;
                        }
                        else if (lDataCol.DataType == typeof(double))
                        {
                            lRowTotPagina[lDataCol] = (double)lRowTotPagina[lDataCol] / ldt.Rows.Count;
                        }
                        else if (lDataCol.DataType == typeof(float))
                        {
                            lRowTotPagina[lDataCol] = (float)lRowTotPagina[lDataCol] / ldt.Rows.Count;
                        }
                    }
                    else if (lsAggregateFunc == "COUNT")
                    {
                        lRowTotPagina[lDataCol] = ldt.Rows.Count;
                    }
                }
            }
            if (lbAddTotRow)
            {
                ldt.Columns.Add(new DataColumn("bSubTotalPag", typeof(int)));
                ldt.Columns.Add(new DataColumn("TrCssClass", typeof(string)));
                lRowTotPagina["bSubTotalPag"] = 1;
                lRowTotPagina["TrCssClass"] = "SubTotalPag";
                lgsrRet.sColumns += ",bSubTotalPag,TrCssClass";
                ldt.Rows.Add(lRowTotPagina);
            }
        }

        protected static void ProcesaFormatoColumnas(DSOGridServerResponse lgsrRet, DataTable ldt, Hashtable lHTClientCols, DataTable lKDBTypes)
        {
            //Proceso los formatos de las columnas
            Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
            Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();
            DataRow lKDBCamposRow;
            DataRow lKDBTypeRow;
            string lsLang = Globals.GetCurrentLanguage();

            KeytiaIntegerField lIntegerField = new KeytiaIntegerField();
            KeytiaIntegerFormatField lIntegerFormatField = new KeytiaIntegerFormatField();
            KeytiaNumericField lNumericField = new KeytiaNumericField();
            KeytiaCurrencyField lCurrencyField = new KeytiaCurrencyField();
            lCurrencyField.SetFormatInfo(Globals.GetCurrentCurrency());

            string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
            string lsDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");
            foreach (DataColumn lDataCol in ldt.Columns)
            {
                if (lDataCol.DataType == typeof(int)
                    || lDataCol.DataType == typeof(short)
                    || lDataCol.DataType == typeof(decimal)
                    || lDataCol.DataType == typeof(double)
                    || lDataCol.DataType == typeof(float))
                {
                    foreach (DataRow ldataRow in ldt.Rows)
                    {
                        if (ldataRow[lDataCol] == DBNull.Value)
                        {
                            ldataRow[lDataCol] = 0;
                        }
                    }
                }

                lKDBCamposRow = null;
                if (lHTClientCols.ContainsKey(lDataCol.ColumnName))
                {
                    lKDBCamposRow = lHTClientCols[lDataCol.ColumnName] as DataRow;
                }

                if (lKDBCamposRow != null && lKDBCamposRow["{Types}"] != DBNull.Value)
                {
                    lKDBTypeRow = lKDBTypes.Select("iCodCatalogo = " + lKDBCamposRow["{Types}"])[0];
                    if (lKDBTypeRow["vchCodigo"].ToString() == "Int")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lIntegerField.StringFormat);
                        lColFormatter.Add(lDataCol.ColumnName, lIntegerField.FormatInfo);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "IntFormat")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lIntegerFormatField.StringFormat);
                        lColFormatter.Add(lDataCol.ColumnName, lIntegerFormatField.FormatInfo);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "Float")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lNumericField.StringFormat);
                        lColFormatter.Add(lDataCol.ColumnName, lNumericField.FormatInfo);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "Currency")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lCurrencyField.StringFormat);
                        lColFormatter.Add(lDataCol.ColumnName, lCurrencyField.FormatInfo);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "Date")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lsDateFormat);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "DateTime")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, lsDateTimeFormat);
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "TimeSeg")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, "TimeSeg");
                    }
                    else if (lKDBTypeRow["vchCodigo"].ToString() == "Time")
                    {
                        lColStringFormat.Add(lDataCol.ColumnName, "Time");
                    }
                }
            }

            lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter, lsLang);
        }

        #endregion
    }
}