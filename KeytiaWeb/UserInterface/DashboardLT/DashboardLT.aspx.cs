using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using KeytiaServiceBL;
using System.Drawing;
using System.Data;
using System.Web.UI.DataVisualization.Charting;
using System.Collections;
using System.IO;
using KeytiaWeb.UserInterface.DashboardFC;
using DSOControls2008;
using KeytiaServiceBL.Reportes;
using AjaxControlToolkit;
using KeytiaWeb.UserInterface.Indicadores;

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public partial class DashboardLT : System.Web.UI.Page
    {
        #region Variables globales

        //NZ 20160120 Se Definio que siempre las fechas iniciales fueran dos meses atras a la fecha actual. (Con Gilberto Ramirez)  
        protected DateTime fechaInicio = new DateTime(DateTime.Now.AddMonths(-2).Year, DateTime.Now.AddMonths(-2).Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Now.AddMonths(-2).Year, DateTime.Now.AddMonths(-2).Month,
                                                     DateTime.DaysInMonth(DateTime.Now.AddMonths(-2).Year, DateTime.Now.AddMonths(-2).Month));


        //Se almacenan los parametros que llegan en el QueryString
        static string TituloNavegacion = string.Empty;
        static Dictionary<string, string> param = new Dictionary<string, string>();

        protected int iCodTipoServicio = 0;
        protected int iCodTipoDestino = 0;
        protected int iCodCenCos = 0;
        protected int iCodEmple = 0;
        protected int nav = 0;
        protected string tituloGraf = string.Empty;
        protected string TipoServicio = string.Empty;
        protected string TipoDestino = string.Empty;
        protected string CenCos = string.Empty;
        protected string Empleado = string.Empty;
        protected string cenCosDesc = string.Empty;
        protected string tipoServicioDesc = string.Empty;
        protected string tipoDestinoDesc = string.Empty;
        protected string empleDesc = string.Empty;

        //NZ
        protected bool exportacionWithGrafica = false;
        protected string exportacionGrafColumEjeX = string.Empty;
        protected string exportacionTitulo = string.Empty;
        protected bool exportacionGrafMatrcial = false;

        #region Variables utilizadas para el idioma

        protected string lsLanReporte = string.Empty;
        protected string lsLanGráfica = string.Empty;
        protected string lsLanEspañol = string.Empty;
        protected string lsLanIngles = string.Empty;
        protected string lsLanlblInicio = string.Empty;
        protected string lsLanFechaInicio = string.Empty;
        protected string lsLanFechaFin = string.Empty;
        protected string lsLanbtnRegresar = string.Empty;
        protected string lsLanbtnAplicar = string.Empty;
        protected string lsLanbtnDashPrincipal = string.Empty;
        protected string lsLanbtnDashMoviles = string.Empty;
        protected string lsLanCentroDeCostos = string.Empty;
        protected string lsLanTotales = string.Empty;
        protected string lsLanTelefoniaFija = string.Empty;
        protected string lsLanTelefoniaMovil = string.Empty;
        protected string lsLanTelefoniaEnlace = string.Empty;
        protected string lsLanTelefoniaOtros = string.Empty;
        protected string lsLanTipoDeServicio = string.Empty;
        protected string lsLanImporte = string.Empty;
        protected string lsLanConsumoPorCentroDeCostos = string.Empty;
        protected string lsLanConsumoPorTipoServicio = string.Empty;
        protected string lsLanCosto = string.Empty;
        protected string lsLanHistoricoPorCentroDeCostos = string.Empty;
        protected string lsLanHistoricoPorTipoDeTelefonia = string.Empty;
        protected string lsLanMes = string.Empty;
        protected string lsLanTipoDestino = string.Empty;
        protected string lsLanConsumoPorTipoTelefonia = string.Empty;
        protected string lsLanNombreCompleto = string.Empty;
        protected string lsLanExtension = string.Empty;
        protected string lsLanNumeroMarcado = string.Empty;
        protected string lsLanNumeroMovil = string.Empty;
        protected string lsLanFecha = string.Empty;
        protected string lsLanHora = string.Empty;
        protected string lsLanDuracion = string.Empty;
        protected string lsLanLocalidad = string.Empty;
        protected string lsLanSitio = string.Empty;
        protected string lsLanCodigoAutorizacion = string.Empty;
        protected string lsLanCarrier = string.Empty;
        protected string lsLanTipoLlamada = string.Empty;
        protected string lsLanConsumoPorTipoDestino = string.Empty;
        protected string lsLanHistoricoPorTipoDeDestino = string.Empty;
        protected string lsLanHistoricoPorOperadora = string.Empty;
        protected string lsLanImporteEnMiles = string.Empty;
        protected string lsLanAreaManager = string.Empty;
        protected string lsLanDireccion = string.Empty;
        protected string lsLanJefatura = string.Empty;
        //NZ 20151105
        protected string lsLanMoneda = string.Empty;
        #endregion

        protected string lsidioma = string.Empty;

        protected string unidadNegocio = string.Empty;

        #endregion

        protected void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
            (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();

            SeleccionaIdioma();

            #region Lee Query String

            #region Revisar si el querystring Nav contiene un valor

            //NZ 20151125 Reusare esta variable que actualmente perdio funcionalidad. La usare para manejar la navegacion de las dos primeras
            //graficas del dashboard.
            if (!string.IsNullOrEmpty(Request.QueryString["Nav"]))
            {
                nav = Convert.ToInt32(Request.QueryString["Nav"]);
            }
            else { nav = 0; }

            #endregion
            //NZ 20151124 Se cambia navegacion descripcion por navegacion iCodCatalogo
            #region Revisar si el querystring CenCos contiene un valor
            if (!string.IsNullOrEmpty(CenCos = Request.QueryString["CenCos"]))
            {
                try
                {
                    iCodCenCos = Convert.ToInt32(Request.QueryString["CenCos"]);
                    cenCosDesc = ConsultaDesc(iCodCenCos.ToString(), "CenCos");
                    tituloGraf += cenCosDesc;
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al consultar un centro de costos en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring CenCos '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                string ObtieneICodCenCosUsuario = DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()).ToString();
                iCodCenCos = Convert.ToInt32(ObtieneICodCenCosUsuario);
                cenCosDesc = ConsultaDesc(ObtieneICodCenCosUsuario, "CenCos");
            }

            #endregion
            //NZ 20151124 Se cambia navegacion descripcion por navegacion iCodCatalogo
            #region Revisar si el querystring TS contiene un valor

            if (!string.IsNullOrEmpty(TipoServicio = Request.QueryString["TS"]))
            {
                try
                {
                    iCodTipoServicio = Convert.ToInt32(Request.QueryString["TS"]);
                    tipoServicioDesc = ConsultaDesc(iCodTipoServicio.ToString(), "TS");
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al consultar un tipo servicio en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring TS '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodTipoServicio = 0;
            }
            #endregion
            //NZ 20151124 Se cambia navegacion descripcion por navegacion iCodCatalogo
            #region Revisar si el querystring TDest contiene un valor

            if (!string.IsNullOrEmpty(TipoDestino = Request.QueryString["TDest"]))
            {
                try
                {
                    iCodTipoDestino = Convert.ToInt32(Request.QueryString["TDest"]);
                    tipoDestinoDesc = ConsultaDesc(iCodTipoDestino.ToString(), "TDest");
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al consultar un tipo destino en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring TDest '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodTipoDestino = 0;
            }

            #endregion
            //NZ 20151124 Se cambia navegacion descripcion por navegacion iCodCatalogo
            #region Revisar si el querystring iCodEmple contiene un valor

            if (!string.IsNullOrEmpty(Empleado = Request.QueryString["iCodEmple"]))
            {
                try
                {
                    iCodEmple = Convert.ToInt32(Empleado);
                    empleDesc = ConsultaDesc(iCodEmple.ToString(), "Emple");
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring iCodEmple '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else { iCodEmple = 0; }

            #endregion

            #endregion

            if (!Page.IsPostBack)
            {
                if(Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString()=="")
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["fecIni"]) && !string.IsNullOrEmpty(Request.QueryString["fecFin"]))
                    {

                        try
                        {
                            Session["FechaInicio"] = Convert.ToDateTime(Request.QueryString["fecIni"]).ToString("yyyy-MM-dd");
                            Session["FechaFin"] = Convert.ToDateTime(Request.QueryString["fecFin"]).ToString("yyyy-MM-dd");
                        }
                        catch (Exception ex)
                        {
                            throw new KeytiaWebException(
                                "Ocurrio un error al darle valores a los campos de fecha en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring fecIni & fecFin '"
                                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                        }
                    }
                    else
                    {
                        Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                        Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                    }
                }
            }
 

            #region Adquiere nivel de cencos

            int nivelJerarquico;
            nivelJerarquico = AdquiereNivelCenCos();

            #endregion

            if (iCodTipoServicio > 0 && iCodTipoDestino == 0)
            {
                ReportePorTipoServicioEnCenCos();
            }
            else if (iCodTipoDestino > 0 && iCodEmple == 0)
            {
                ReporteConsumoAgrupadoPorEmpleado();
            }
            else if (iCodEmple > 0)
            {
                ReporteDetalladoPorTDestGroupEmple();
            }
            else if (iCodCenCos > 0 && iCodCenCos != (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()))
            {
                ReportePorCenCosOTipoTelefonia();
            }
            else { ReportesDashboardPrincipal(); }

            ConfiguraNavegacion();
            if (Rep0.Controls.Count > 0)
            {
                ResporteGraficasHistoricas(); //Estas se pintan al final.
            }
        }

        public void SeleccionaIdioma()
        {
            if (Session["Language"].ToString() == "Español" || Session["Language"].ToString() != "Ingles")
                lsidioma = "Site";
            if (Session["Language"].ToString() == "Ingles")
                lsidioma = "SiteENUS";

            Globals.SetLanguage(Session["Language"].ToString());

            IniciaVariablesIdioma();

            lblTipoMoneda.Text = lsLanMoneda + ConsultaDescMoneda();
        }

        void ConfiguraNavegacion()
        {
            if (iCodTipoServicio > 0 && iCodTipoDestino == 0)
            {
                int y = Session["ListaNavegacion"] != null ? ((List<MapNav>)Session["ListaNavegacion"]).Count(x => x.URL.ToLowerInvariant().Contains("nav=2")) : 0;
                if (nav == 2 && y == 0)
                {
                    TituloNavegacion = cenCosDesc + " - " + tipoServicioDesc;
                }
                else if (y > 0) { TituloNavegacion = cenCosDesc; }
                else { TituloNavegacion = tipoServicioDesc; }
            }
            else if (iCodTipoDestino > 0 && iCodEmple == 0)
            {
                TituloNavegacion = tipoDestinoDesc;
            }
            else if (iCodEmple > 0)
            {
                TituloNavegacion = empleDesc;
            }
            else if (iCodCenCos > 0 && iCodCenCos != (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()))
            {
                TituloNavegacion = cenCosDesc;
            }
            else { TituloNavegacion = string.Empty; }


            if (Request.QueryString["CenCos"] == null || Request.QueryString["CenCos"] == string.Empty)
            {
                pnlMapaNav.Visible = false;
                pnlIndicadores.Visible = true;

                if (Session["ListaNavegacion"] != null) //Entonces ya tiene navegacion almacenada
                {
                    List<MapNav> listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];
                    listaNavegacion.Clear();
                    Session["ListaNavegacion"] = listaNavegacion;
                }

                //Solo en el primer nivel se deben mostrar los indicadores.                
                BuildIndicadores.ConstruirIndicadores(ref pnlIndicadores, "DashboardLT.aspx", Request.Path);
            }
            else
            {
                pnlMapaNav.Visible = true;
                pnlIndicadores.Visible = false;

                List<MapNav> listaNavegacion = new List<MapNav>();
                if (Session["ListaNavegacion"] != null) //Entonces ya tiene navegacion almacenada
                {
                    listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];

                    if (listaNavegacion.Count == 0)
                    {
                        listaNavegacion.Add(new MapNav() { Titulo = lsLanlblInicio, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    }

                    if (listaNavegacion.Exists(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString()))
                    {
                        int index = listaNavegacion.FindIndex(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString());
                        if (index == listaNavegacion.Count - 1)
                        {
                            TituloNavegacion = listaNavegacion[index].Titulo;
                            listaNavegacion.RemoveAt(index);
                            listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() });
                        }
                        else { listaNavegacion.RemoveRange(index + 1, (listaNavegacion.Count - 1) - index); }
                    }
                    else { listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() }); }
                }
                else
                {
                    listaNavegacion.Add(new MapNav() { Titulo = lsLanlblInicio, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() });
                }

                Session["ListaNavegacion"] = listaNavegacion;

                pnlMapaNavegacion.Controls.Clear();
                pnlMapaNavegacion.Controls.Add(DTIChartsAndControls.MapaNavegacion(listaNavegacion));
            }

            //Panels
            List<Panel> Reps = new List<Panel>() { Rep0, Rep1, Rep2, Rep3, Rep4, Rep5, Rep6, Rep7, Rep8, Rep9 };
            foreach (Panel item in Reps)
            {
                if (item.Controls.Count == 0)
                {
                    item.Attributes.Add("style", "none");
                }
                else { item.Attributes.Remove("style"); }
            }

        }

        //NZ 20151118 Se cambia reporte a fusionChart
        public virtual void CreaGraficasStackEnDashboardPrincipal()
        {
            if (nav == 0)
            {
                #region Grafica en Stack por Centro de Costos

                Rep1.Controls.Clear();
                DataTable ldt2 = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCosYTipoServicioMat(iCodCenCos));
                ldt2 = DTIChartsAndControls.selectTopNTabla(ldt2, "Totales Desc", 10);

                DataTable dtAux = new DataTable();
                string[] lsaDataSource = null;
                DataTable catTDest = null;
                if (ldt2.Rows.Count > 0)
                {
                    if (ldt2.Columns.Contains(lsLanTotales))
                    {
                        ldt2.Columns.Remove(lsLanTotales);
                    }

                    string formatoLinkCenCos = Request.Path + "?Nav=2&CenCosPad=" + iCodCenCos + "&CenCos={0}&fecIni="
                                        + Session["FechaInicio"].ToString() + "&fecFin=" + Session["FechaFin"].ToString() + "&TS=[0]";

                    catTDest = DTIChartsAndControls.DataTable(ConsultaCatTDest());
                    lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                                  FCAndControls.ConvertDataTabletoDataTableArrayLink(ldt2, 1, new int[] { 0 },
                                                                        formatoLinkCenCos, new int[] { 0 }, catTDest));

                    dtAux = ldt2.Copy();  //NZ usare esta tabla para obtener informacion para la segunda grafica. (Lo necesito antes de quitar los iCods)

                    if (ldt2.Rows.Count > 0 && ldt2.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt2);
                        ldt2 = dvldt.ToTable(false, new string[] { unidadNegocio, lsLanTelefoniaFija, lsLanTelefoniaMovil, lsLanTelefoniaEnlace, lsLanTelefoniaOtros });
                    }
                }

                Rep1.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosJerarGra_G", lsLanConsumoPorCentroDeCostos + " " + unidadNegocio, 2, FCGpoGraf.MatricialConStack));


                if (lsaDataSource != null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGra",
                                     FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt2), "RepConsCenCosJerarGra_G",
                                     "", "", unidadNegocio, lsLanImporte, 2, FCGpoGraf.MatricialConStack), false);
                }

                #endregion Grafica

                #region Grafica en Stack por Tipo de Servicio

                DataTable ldt = new DataTable();
                string[] lsaDataSource2 = null;
                if (ldt2.Rows.Count > 0)
                {
                    ldt = TransponerDataTable.Transpond(ldt2);
                    ldt.Rows.RemoveAt(0);

                    Rep2.Controls.Clear();
                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        if (ldt.Rows[ldt.Rows.Count - 1]["Head"].ToString() == lsLanTotales)
                        {
                            ldt.Rows[ldt.Rows.Count - 1].Delete();
                        }
                        ldt.AcceptChanges();
                    }

                    ldt.Columns.Add("iCodCatTDest");
                    for (int i = 0; i < ldt.Rows.Count; i++)
                    {
                        ldt.Rows[i]["iCodCatTDest"] = catTDest.Rows[i][0].ToString();
                    }

                    if (dtAux.Rows.Count > 0 && dtAux.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(dtAux);
                        dtAux = dvldt.ToTable(false, new string[] { "iCodCatalogo", unidadNegocio });
                        dtAux.AcceptChanges();
                    }

                    string formatoLinkTS = Request.Path + "?Nav=2&CenCosPad=" + iCodCenCos + "&CenCos=[0]&fecIni="
                                        + Session["FechaInicio"].ToString() + "&fecFin=" + Session["FechaFin"].ToString() + "&TS={0}";

                    lsaDataSource2 = FCAndControls.ConvertDataTabletoJSONString(
                                                       FCAndControls.ConvertDataTabletoDataTableArrayLink(ldt, 0, new int[] { ldt.Columns.Count - 1 },
                                                                        formatoLinkTS, new int[] { ldt.Columns.Count - 1 }, dtAux));
                }

                Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosJerarGra2_T", lsLanConsumoPorTipoServicio, 3, FCGpoGraf.MatricialConStack));


                if (lsaDataSource2 != null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGra2",
                               FCAndControls.GraficaMultiSeries(lsaDataSource2, FCAndControls.extraeNombreColumnas(ldt), "RepConsCenCosJerarGra2_T",
                               "", "", lsLanTipoDeServicio, lsLanImporte, 3, FCGpoGraf.MatricialConStack), false);
                }

                #endregion Grafica
            }
        }

        public void IniciaVariablesIdioma()
        {
            lsLanReporte = GetGlobalResourceObject(lsidioma, "Reporte").ToString();
            lsLanGráfica = GetGlobalResourceObject(lsidioma, "Gráfica").ToString();
            lsLanEspañol = GetGlobalResourceObject(lsidioma, "Español").ToString();
            lsLanIngles = GetGlobalResourceObject(lsidioma, "Ingles").ToString();
            lsLanlblInicio = GetGlobalResourceObject(lsidioma, "Inicio").ToString();
            lsLanFechaInicio = GetGlobalResourceObject(lsidioma, "FechaInicio").ToString();
            lsLanFechaFin = GetGlobalResourceObject(lsidioma, "FechaFin").ToString();
            lsLanbtnRegresar = GetGlobalResourceObject(lsidioma, "btnRegresar").ToString();
            lsLanbtnAplicar = GetGlobalResourceObject(lsidioma, "btnAplicar").ToString();
            lsLanbtnDashPrincipal = GetGlobalResourceObject(lsidioma, "btnDashPrincipal").ToString();
            lsLanbtnDashMoviles = GetGlobalResourceObject(lsidioma, "btnDashMoviles").ToString();
            lsLanCentroDeCostos = GetGlobalResourceObject(lsidioma, "CentroDeCostos").ToString();
            lsLanTotales = GetGlobalResourceObject(lsidioma, "Totales").ToString();
            lsLanTelefoniaFija = GetGlobalResourceObject(lsidioma, "TelefoniaFija").ToString();
            lsLanTelefoniaMovil = GetGlobalResourceObject(lsidioma, "TelefoniaMovil").ToString();
            lsLanTelefoniaEnlace = GetGlobalResourceObject(lsidioma, "TelefoniaEnlace").ToString();
            lsLanTelefoniaOtros = GetGlobalResourceObject(lsidioma, "TelefoniaOtros").ToString();
            lsLanTipoDeServicio = GetGlobalResourceObject(lsidioma, "TipoDeServicio").ToString();
            lsLanImporte = GetGlobalResourceObject(lsidioma, "Importe").ToString();
            lsLanConsumoPorCentroDeCostos = GetGlobalResourceObject(lsidioma, "ConsumoPorCentroDeCostos").ToString();
            lsLanConsumoPorTipoServicio = GetGlobalResourceObject(lsidioma, "ConsumoPorTipoServicio").ToString();
            lsLanCosto = GetGlobalResourceObject(lsidioma, "Costo").ToString();
            lsLanHistoricoPorCentroDeCostos = GetGlobalResourceObject(lsidioma, "HistoricoPorCentroDeCostos").ToString();
            lsLanHistoricoPorTipoDeTelefonia = GetGlobalResourceObject(lsidioma, "HistoricoPorTipoDeTelefonia").ToString();
            lsLanMes = GetGlobalResourceObject(lsidioma, "Mes").ToString();
            lsLanTipoDestino = GetGlobalResourceObject(lsidioma, "TipoDestino").ToString();
            lsLanConsumoPorTipoTelefonia = GetGlobalResourceObject(lsidioma, "ConsumoPorTipoTelefonia").ToString();
            lsLanNombreCompleto = GetGlobalResourceObject(lsidioma, "NombreCompleto").ToString();
            lsLanExtension = GetGlobalResourceObject(lsidioma, "Extension").ToString();
            lsLanNumeroMovil = GetGlobalResourceObject(lsidioma, "NumeroMovil").ToString();
            lsLanNumeroMarcado = GetGlobalResourceObject(lsidioma, "NumeroMarcado").ToString();
            lsLanFecha = GetGlobalResourceObject(lsidioma, "Fecha").ToString();
            lsLanHora = GetGlobalResourceObject(lsidioma, "Hora").ToString();
            lsLanDuracion = GetGlobalResourceObject(lsidioma, "Duracion").ToString();
            lsLanLocalidad = GetGlobalResourceObject(lsidioma, "Localidad").ToString();
            lsLanSitio = GetGlobalResourceObject(lsidioma, "Sitio").ToString();
            lsLanCodigoAutorizacion = GetGlobalResourceObject(lsidioma, "CodigoAutorizacion").ToString();
            lsLanCarrier = GetGlobalResourceObject(lsidioma, "Carrier").ToString();
            lsLanTipoLlamada = GetGlobalResourceObject(lsidioma, "TipoLlamada").ToString();
            lsLanConsumoPorTipoDestino = GetGlobalResourceObject(lsidioma, "ConsumoPorTipoDestino").ToString();
            lsLanHistoricoPorTipoDeDestino = GetGlobalResourceObject(lsidioma, "HistoricoPorTipoDeDestino").ToString();
            lsLanHistoricoPorOperadora = GetGlobalResourceObject(lsidioma, "HistoricoPorOperadora").ToString();
            lsLanImporteEnMiles = GetGlobalResourceObject(lsidioma, "ImporteEnMiles").ToString();
            lsLanAreaManager = GetGlobalResourceObject(lsidioma, "AreaManager").ToString();
            lsLanDireccion = GetGlobalResourceObject(lsidioma, "Direccion").ToString();
            lsLanJefatura = GetGlobalResourceObject(lsidioma, "Jefatura").ToString();
            //NZ 20151105
            lsLanMoneda = GetGlobalResourceObject(lsidioma, "Moneda").ToString();

        }

        #region Codigo de botones de la pagina

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            if (Session["ListaNavegacion"] != null)
            {
                List<MapNav> listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];
                if (listaNavegacion.Count >= 2)
                {
                    listaNavegacion.RemoveAt(listaNavegacion.Count - 1);
                    HttpContext.Current.Response.Redirect(listaNavegacion[listaNavegacion.Count - 1].URL);
                }
                else
                {
                    listaNavegacion.Clear();
                    HttpContext.Current.Response.Redirect(Request.Path);
                }
            }
            else { HttpContext.Current.Response.Redirect(Request.Path); }
        }

        #endregion

        #region Consultas a SQL

        protected string ConsultaCostoPorCenCosYTipoTelefonia(int iCodCenCosPad, string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            /*Resumen por CenCos y TDest*/
            lsb.Append("declare @Where varchar(max)");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            lsb.Append("+' AND [Codigo Centro de Costos] =" + iCodCenCosPad + "'");
            lsb.Append("exec RepMatResumenPorPaisDashLT  ");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', ");
            lsb.Append("@InnerFields='[Codigo Centro de Costos], [CodCatTDest], [Categoria Tipo Telefonia] as [" + lsLanTipoDeServicio + "],");
            lsb.Append("[Total] = SUM([Costo]) + SUM([CostoSM])', ");
            lsb.Append("@InnerWhere=@Where, ");
            lsb.Append("@InnerGroup='[Codigo Centro de Costos], [CodCatTDest], [Categoria Tipo Telefonia]', ");
            lsb.Append("@OuterFields='[CodCatTDest], [" + lsLanTipoDeServicio + "],");
            if (!string.IsNullOrEmpty(linkGrafica))
            {
                lsb.Append(linkGrafica + ",");
            }
            lsb.Append("[" + lsLanTotales + "] = SUM([Total])',");
            lsb.Append("@OuterGroup='[CodCatTDest],[" + lsLanTipoDeServicio + "] , [Codigo Centro de Costos]',");
            lsb.Append("@Order='[CodCatTDest], [" + lsLanTipoDeServicio + "] Asc,[" + lsLanTotales + "] Desc',");
            lsb.Append("@OrderInv='[CodCatTDest], [" + lsLanTipoDeServicio + "] Desc,[" + lsLanTotales + "] Asc',");
            lsb.Append("@OrderDir='Desc',");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.Append("@Idioma = '" + Session["Language"] + "',");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return lsb.ToString();
        }

        protected string ConsultaCenCosDeUsuario(int iCodCenCosPad)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("DECLARE @CenCosBase INT = " + iCodCenCosPad);
            lsb.AppendLine("DECLARE @Usuar INT = " + Session["iCodUsuario"]);
            lsb.AppendLine("DECLARE @Perfil INT = " + Session["iCodPerfil"]);
            lsb.AppendLine("DECLARE @FechaFin INT = " + DateTime.Now.Year.ToString() + Convert.ToString(DateTime.Now.Month + 12));
            lsb.AppendLine("");
            lsb.AppendLine("SELECT iCodCatalogo, vchCodigo, vchDescripcion, Descripcion");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','Español')] CenCos");
            lsb.AppendLine("		JOIN (SELECT CenCos ");
            lsb.AppendLine("					FROM " + DSODataContext.Schema + ".RestCenCos Restricc");
            lsb.AppendLine("					WHERE Usuar = @Usuar");
            lsb.AppendLine("					AND perfil = @Perfil");
            lsb.AppendLine("					AND @FechaFin BETWEEN CONVERT(varchar,datepart(yyyy,Restricc.FechaInicio)) + CONVERT(varchar,datepart(mm,Restricc.FechaInicio)+12) AND CONVERT(varchar,datepart(yyyy,Restricc.FechaFin)) + CONVERT(varchar,datepart(mm,Restricc.FechaFin)+12)");
            lsb.AppendLine("					AND FechaInicio <> FechaFin	");
            lsb.AppendLine("					) AS RestCenCos");
            lsb.AppendLine("				ON CenCos.iCodCatalogo = RestCenCos.CenCos	");
            lsb.AppendLine("");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("	AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("	AND CenCos.CenCos = @CenCosBase");
            lsb.AppendLine("UNION");
            lsb.AppendLine("SELECT iCodCatalogo, vchCodigo, vchDescripcion, Descripcion");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','Español')] CenCos");
            lsb.AppendLine("		JOIN (SELECT CenCos ");
            lsb.AppendLine("					FROM " + DSODataContext.Schema + ".RestCenCos Restricc");
            lsb.AppendLine("					WHERE Usuar = @Usuar");
            lsb.AppendLine("					AND perfil = @Perfil");
            lsb.AppendLine("					AND @FechaFin BETWEEN CONVERT(varchar,datepart(yyyy,Restricc.FechaInicio)) + CONVERT(varchar,datepart(mm,Restricc.FechaInicio)+12) AND CONVERT(varchar,datepart(yyyy,Restricc.FechaFin)) + CONVERT(varchar,datepart(mm,Restricc.FechaFin)+12)");
            lsb.AppendLine("					AND FechaInicio <> FechaFin	");
            lsb.AppendLine("					) AS RestCenCos");
            lsb.AppendLine("				ON CenCos.iCodCatalogo = RestCenCos.CenCos	");
            lsb.AppendLine("");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("	AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("	AND CenCos.iCodCatalogo = @CenCosBase");
            lsb.AppendLine("    AND CenCos.CenCos IS NOT NULL");

            return lsb.ToString();
        }

        protected string ConsultaHistoricoPorCenCosMesActualMenos12(int iCodCenCosPad)
        {
            StringBuilder lsb = new StringBuilder();
            DataTable CenCos = DTIChartsAndControls.DataTable(ConsultaCenCosDeUsuario(iCodCenCosPad));

            if (CenCos.Rows.Count > 0)  //PENDIENTE: hacer pruebas de cuando la consulta no regresa nada, para ver si la pagina falla.
            {
                StringBuilder fields = new StringBuilder();
                fields.AppendLine("'Mes as [" + lsLanMes + "],");

                foreach (DataRow row in CenCos.Rows)  //NZ Arma matricial por Centro de Costos que puede ver el usuario.
                {
                    fields.AppendLine("SUM(CASE CenCos WHEN ''" + row["Descripcion"] + "'' THEN Costo ELSE 0 END) AS [" + row["Descripcion"] + "],");
                }
                fields.Replace(",", "'", fields.Length - 3, 1);

                lsb.AppendLine("exec ConsumoHistorico12MesesCenCosTdest");
                lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
                lsb.AppendLine("@Fields = " + fields.ToString() + ",");
                lsb.AppendLine("@Group = 'Mes',");
                lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
                lsb.AppendLine("@CenCosBase = " + iCodCenCosPad + ",");
                lsb.AppendLine("@Where = '[TDestDesc] not like ''%por%identificar%'' AND [CenCos] not like ''%por%identificar%'''");
            }

            return lsb.ToString();
        }

        protected string ConsultaHistoricoPorTipoTelefoniaMesActualMenos12(int iCodCenCosPad)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" exec ConsumoHistorico12MesesCenCosTdest");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "',");
            lsb.AppendLine("@Fields = 'Mes as [" + lsLanMes + "],");
            lsb.AppendLine(" [" + lsLanTelefoniaFija + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaFija + "'' then [Costo] else 0 end),");
            lsb.AppendLine(" [" + lsLanTelefoniaMovil + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaMovil + "'' then [Costo] else 0 end),");
            lsb.AppendLine(" [" + lsLanTelefoniaEnlace + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaEnlace + "'' then [Costo] else 0 end),");
            lsb.AppendLine(" [" + lsLanTelefoniaOtros + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaOtros + "'' then [Costo] else 0 end)',");
            lsb.AppendLine("@Group = 'Mes',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@CenCosBase = " + iCodCenCosPad + ",");
            lsb.AppendLine("@Where = '[TDestDesc] not like ''%por%identificar%'' AND [CenCos] not like ''%por%identificar%'''");

            return lsb.ToString();
        }

        protected string ConsultaCostoPorCenCosYTipoTelefonia(int iCodCenCosPad, int iCodTipoTelefonia, string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            /*Resumen por CenCos y TDest*/
            lsb.AppendLine("declare @Where varchar(max)");
            lsb.AppendLine("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  ");
            lsb.AppendLine("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            lsb.AppendLine("+' AND [Codigo Centro de Costos] =" + iCodCenCosPad + "'");
            lsb.AppendLine("+' AND [CodCatTDest] =" + iCodTipoTelefonia + "'");
            lsb.AppendLine("exec RepMatResumenPorPaisDashLT  ");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "', ");
            lsb.AppendLine("@InnerFields='[Codigo Centro de Costos], [iCodTDest], [Tipo Destino] as [" + lsLanTipoDestino + "],");
            lsb.AppendLine("[Total] = SUM([Costo]) + SUM([CostoSM])', ");
            lsb.AppendLine("@InnerWhere=@Where, ");
            lsb.AppendLine("@InnerGroup='[Codigo Centro de Costos], [iCodTDest], [Tipo Destino]', ");
            lsb.AppendLine("@OuterFields='[iCodTDest], [" + lsLanTipoDestino + "],");
            if (!string.IsNullOrEmpty(linkGrafica))
            {
                lsb.AppendLine(linkGrafica + ",");
            }
            lsb.AppendLine("[" + lsLanTotales + "] = SUM([Total])',");
            lsb.AppendLine("@OuterGroup='[iCodTDest], [" + lsLanTipoDestino + "] , [Codigo Centro de Costos]',");
            lsb.AppendLine("@Order='[" + lsLanTotales + "] Desc,[" + lsLanTipoDestino + "] Asc, [iCodTDest]',");
            lsb.AppendLine("@OrderInv='[iCodTDest], [" + lsLanTipoDestino + "] Desc,[" + lsLanTotales + "] Asc',");
            lsb.AppendLine("@OrderDir='Asc',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine("@Idioma = '" + Session["Language"] + "',");
            lsb.AppendLine("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', ");
            lsb.AppendLine("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59'''");

            return lsb.ToString();
        }

        protected string ConsultaCostoPorCenCosYTipoTelefoniaNav2(int iCodCenCos, string iCodTipoServicio, string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" exec ConsumoAcumuladoCenCosTDest @Schema = '" + DSODataContext.Schema + "', @Cencosbase = " + iCodCenCos + ",");
            lsb.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59',");
            lsb.AppendLine(" @Fields = '[CenCos], [" + lsLanCentroDeCostos + "] = CenCosDesc,");
            if (!string.IsNullOrEmpty(linkGrafica))
            {
                lsb.AppendLine(linkGrafica + ",");
            }
            lsb.AppendLine("    [" + lsLanImporte + "] = SUM([Costo])',");
            lsb.AppendLine(" @Where = '[CatTDest] = " + TipoServicio + "',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @Idioma = '" + Session["Language"] + "',");
            lsb.AppendLine(" @Order = '[" + lsLanImporte + "] desc',");
            lsb.AppendLine("  @Group = '[CenCos], CenCosDesc'");

            return lsb.ToString();
        }

        protected string ConsultaConsumoAgrupadoPorEmpleado(int iCodTDest, int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine("declare @Where varchar(max)");
            lsb.AppendLine("set @Where = '[FechaInicio] between ''" + Session["FechaInicio"].ToString() + "'' AND ''" + Session["FechaFin"].ToString() + "'''");
            lsb.AppendLine(" + ' AND [Codigo Tipo Destino] = " + iCodTDest + " AND [Codigo Centro de Costos] = " + iCodCenCos + "'");
            lsb.AppendLine("exec ConsumoAcumuladoTodosCamposRestDashboardLT ");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "', ");
            lsb.AppendLine("@Fields = '[Codigo Empleado],[Nombre Completo] as [" + lsLanNombreCompleto + "],[" + lsLanImporte + "] = CONVERT(MONEY,SUM([Costo]))',");
            lsb.AppendLine("@Group = '[Codigo Empleado],[Nombre Completo]',");
            lsb.AppendLine("@Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine("@Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine("@Order = '[" + lsLanImporte + "] desc',");
            lsb.AppendLine("@Where =  @Where,");
            lsb.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "', ");
            lsb.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + "'");

            return lsb.ToString();
        }

        protected string ConsultaConsumoPorCenCosYTipoServicioMat(int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" exec ConsumoAcumuladoCenCosTDest @Schema = '" + DSODataContext.Schema + "', @Cencosbase = " + iCodCenCos + ",");
            lsb.AppendLine(" @FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',");
            lsb.AppendLine(" @FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59',");
            lsb.AppendLine(" @Fields = '[CenCos] as iCodCatalogo,[" + unidadNegocio + "] = UPPER(CenCosDesc),");
            lsb.AppendLine(" [" + lsLanTelefoniaFija + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaFija + "'' then [Costo] else 0 end), ");
            lsb.AppendLine(" [" + lsLanTelefoniaMovil + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaMovil + "'' then [Costo] else 0 end), ");
            lsb.AppendLine(" [" + lsLanTelefoniaEnlace + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaEnlace + "'' then [Costo] else 0 end),");
            lsb.AppendLine(" [" + lsLanTelefoniaOtros + "] = SUM(case when [CatTDestDesc] = ''" + lsLanTelefoniaOtros + "'' then [Costo] else 0 end),");
            lsb.AppendLine(" [" + lsLanTotales + "] = SUM([Costo])',");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @Idioma = '" + Session["Language"] + "',");
            lsb.AppendLine(" @Order = '[" + lsLanTotales + "] desc',");
            lsb.AppendLine(" @Group = '[CenCos],[CenCosDesc]'");

            return lsb.ToString();
        }

        protected string ConsultaDetalleEmpleadoTDestCenCos(int iCodEmple, int iCodTDest)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine("declare @Where varchar(max) ");
            lsb.AppendLine("declare @OrderInv varchar(max) ");
            lsb.AppendLine("set @Where = '[Fecha Inicio] >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''");
            lsb.AppendLine("'+' and [Fecha Inicio] <= ''" + Session["FechaFin"].ToString() + " 23:59:59'''");
            lsb.AppendLine("+ ' and [iCodEmple] = " + iCodEmple + " and [iCodTDest] = " + iCodTDest + " '");
            lsb.AppendLine("exec ConsumoDetalladoDashboardLT  ");
            lsb.AppendLine("@Schema = '" + DSODataContext.Schema + "', ");
            lsb.AppendLine("@Fields='");
            lsb.AppendLine("[Centro de costos] as [" + lsLanCentroDeCostos + "],");
            lsb.AppendLine("[Empleado]	 as [" + lsLanNombreCompleto + "],");
            lsb.AppendLine("[Extensión]	 as [" + lsLanExtension + "],");
            lsb.AppendLine("[Numero Marcado] as [" + lsLanNumeroMarcado + "],");
            lsb.AppendLine("[Fecha] as [" + lsLanFecha + "],");
            lsb.AppendLine("[Hora] as [" + lsLanHora + "],");
            lsb.AppendLine("[Duracion] as [" + lsLanDuracion + "],");
            lsb.AppendLine("[Total] as [" + lsLanTotales + "],");
            lsb.AppendLine("[Nombre Localidad] as [" + lsLanLocalidad + "],");
            lsb.AppendLine("[Nombre Sitio]	 as [" + lsLanSitio + "],");
            lsb.AppendLine("[Codigo Autorizacion] as [" + lsLanCodigoAutorizacion + "],");
            lsb.AppendLine("[Nombre Carrier] as [" + lsLanCarrier + "],");
            lsb.AppendLine("[Tipo Llamada] as [" + lsLanTipoLlamada + "],");
            lsb.AppendLine("[Tipo de destino] as [" + lsLanTipoDestino + "]");
            lsb.AppendLine("', ");
            lsb.AppendLine("@Where = @Where,  ");
            lsb.AppendLine("@Order = '[" + lsLanTotales + "] desc', ");
            lsb.AppendLine("@OrderDir = 'Asc', ");
            lsb.AppendLine(" @Usuario = " + Session["iCodUsuario"] + ",");
            lsb.AppendLine(" @Perfil = " + Session["iCodPerfil"] + ",");
            lsb.AppendLine(" @Idioma = '" + Session["Language"] + "',");
            lsb.AppendLine("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00', ");
            lsb.AppendLine("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59' ");

            return lsb.ToString();
        }

        protected string ConsultaICodCenCosUsuario()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" select isnull(Max(CenCos),0)");
            lsb.AppendLine(" from [VisHistoricos('Usuar','Usuarios','Español')]");
            lsb.AppendLine(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE()");
            lsb.AppendLine(" and iCodCatalogo = " + Session["iCodUsuario"]);

            return lsb.ToString();
        }

        protected string ConsultaNumeroDeCenCosDependientes(string lsiCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.AppendLine(" Select COUNT(*) from [VisHistoricos('CenCos','Centro de Costos','Español')]");
            lsb.AppendLine(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE()");
            lsb.AppendLine(" and CenCos = " + lsiCodCenCos);

            return lsb.ToString();
        }

        protected string ConsultaDescMoneda()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT Español, Ingles");
            lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Moneda','Monedas','Español')]");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("    AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("    AND vchCodigo = '" + Session["Currency"].ToString() + "'");

            DataTable dtResult = DSODataAccess.Execute(lsb.ToString());
            if (dtResult.Rows.Count > 0)
            {
                if (Session["Language"].ToString() == "Español" || Session["Language"].ToString() != "Ingles")
                    return dtResult.Rows[0]["Español"].ToString();
                else if (Session["Language"].ToString() == "Ingles")
                    return dtResult.Rows[0]["Ingles"].ToString();
            }
            return "";
        }

        protected int AdquiereNivelCenCos()
        {
            int nivelJerarquico;
            if (!string.IsNullOrEmpty(CenCos = Request.QueryString["CenCosPad"]))
            {
                nivelJerarquico = (int)DSODataAccess.ExecuteScalar("select isNull(NivelJerarq,100) as NivelJerarq from [VisHistoricos('CenCos','Centro de Costos','Español')] \r" +
                                                                                           " where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() and iCodCatalogo = " + CenCos);

                switch (nivelJerarquico)
                {
                    case 0:
                        unidadNegocio = lsLanDireccion;
                        break;
                    case 1:
                        unidadNegocio = lsLanJefatura;
                        break;
                    case 2:
                        unidadNegocio = lsLanCentroDeCostos;
                        break;
                    default:
                        unidadNegocio = lsLanCentroDeCostos;
                        break;
                }
            }
            else
            {
                nivelJerarquico = (int)DSODataAccess.ExecuteScalar("select isNull(NivelJerarq,100) as NivelJerarq from [VisHistoricos('CenCos','Centro de Costos','Español')] \r" +
                                                                                           " where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() and iCodCatalogo = " + iCodCenCos);

                switch (nivelJerarquico)
                {
                    case 0:
                        unidadNegocio = lsLanAreaManager;
                        break;
                    case 1:
                        unidadNegocio = lsLanDireccion;
                        break;
                    case 2:
                        unidadNegocio = lsLanJefatura;
                        break;
                    default:
                        unidadNegocio = lsLanCentroDeCostos;
                        break;
                }
            }
            return nivelJerarquico;
        }

        protected string ConsultaDesc(string iCodCatalogo, string tipo)
        {
            StringBuilder lsb = new StringBuilder();
            string idioma = (Session["Language"].ToString() == "Español") ? "Español" : "Ingles";

            switch (tipo)
            {
                case "CenCos":
                    lsb.AppendLine("SELECT Descripcion FROM [VisHistoricos('CenCos','Centro de Costos','Español')]");
                    lsb.AppendLine("WHERE iCodCatalogo = " + iCodCatalogo);
                    lsb.AppendLine("    AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    break;
                case "TS":
                    lsb.AppendLine(" SELECT " + idioma + " AS Descripcion");
                    lsb.AppendLine(" FROM [VisHistoricos('CatTDest','Categoría de tipo destino','Español')] ");
                    lsb.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() ");
                    lsb.AppendLine("    AND iCodCatalogo = " + iCodCatalogo);
                    break;
                case "TDest":
                    lsb.AppendLine(" SELECT vchDescripcion AS Descripcion");
                    lsb.AppendLine(" FROM [VisHistoricos('TDest','Tipo de Destino','Español')]");
                    lsb.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    lsb.AppendLine("    AND iCodCatalogo = " + iCodCatalogo);
                    break;
                case "Emple":
                    lsb.AppendLine(" SELECT TOP(1) NomCompleto AS Descripcion");
                    lsb.AppendLine(" FROM [VisHistoricos('Emple','Empleados','Español')]");
                    lsb.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
                    lsb.AppendLine("    AND iCodCatalogo = " + iCodCatalogo);
                    lsb.AppendLine(" ORDER BY iCodRegistro DESC");
                    break;
                default:
                    break;
            }

            string etiqueta = string.Empty;
            int iCod = 0;
            if (int.TryParse(iCodCatalogo, out iCod))
            {
                etiqueta = "";
                etiqueta = Util.IsDBNull(DSODataAccess.ExecuteScalar(lsb.ToString()), "").ToString();
                //etiqueta = etiqueta.Split(new char[] { '(' })[0].Trim();
                return etiqueta;
            }
            else
            {
                return "";
            }
        }

        protected string ConsultaCatTDest()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("SELECT iCodCatalogo, Español");
            lsb.AppendLine("FROM [VisHistoricos('CatTDest','Categoría de tipo destino','Español')]");
            lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsb.AppendLine("    AND dtFinVigencia >= GETDATE()");
            lsb.AppendLine("ORDER BY iCodCatalogo");
            return lsb.ToString();
        }

        #endregion

        #region Reportes

        //NZ 20151126 Se cambia reporte a fusionChart
        protected void ReportePorTipoServicioEnCenCos() //Reporte por Tipo de Servicio de un CenCos
        {
            try
            {
                DataTable ldt = null;
                if (nav > 0)
                {
                    string TServicio = Request.QueryString["TS"];

                    int numeroDeCenCosDependientes = (int)DSODataAccess.ExecuteScalar(ConsultaNumeroDeCenCosDependientes(iCodCenCos.ToString()));
                    if (numeroDeCenCosDependientes == 0 ||
                        (Request.QueryString["CenCosPad"].ToString() == Request.QueryString["CenCos"].ToString()) //NZ Significa que ya se ha llegado hasta el consumo propio del padre. En caso de seguir con la unica condicion anterior se volvera ciclico.
                        )
                    {
                        //////nav = 0;

                        string armaLinkGrafica = "[link] = ''" + Request.Path + Page.Request.Url.Query + "&TDest='' + convert(varchar,[iCodTDest])";
                        ldt = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, iCodTipoServicio, armaLinkGrafica.Replace("&Nav=2", "")));

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "iCodTDest", lsLanTipoDestino, lsLanTotales, "link" });
                            ldt.AcceptChanges();
                        }

                        Rep1.Controls.Add(
                                  DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                                  DTIChartsAndControls.GridView("Nav2GridPorCenCosYTSTDest", ldt, true, lsLanTotales,
                                                  new string[] { "", "", "{0:c}" }, Request.Path + Page.Request.Url.Query.Replace("&Nav=2", "") + "&TDest={0}",
                                                  new string[] { "iCodTDest" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                                  "ReportePorTipoServicioEnCenCos_T", "Reporte")
                                                  );

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { lsLanTipoDestino, lsLanTotales, "link" });
                            if (ldt.Rows[ldt.Rows.Count - 1][lsLanTipoDestino].ToString() == lsLanTotales)
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns[lsLanTipoDestino].ColumnName = "label";
                            ldt.Columns[lsLanTotales].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("Nav2GrafPorCenCosYTSTDest_G", lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc, 2, FCGpoGraf.Tabular));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Nav2GrafPorCenCosYTSTDest",
                            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "Nav2GrafPorCenCosYTSTDest_G",
                            "", "", lsLanTipoDestino, lsLanTotales, 2, FCGpoGraf.Tabular), false);

                        #endregion Grafica

                    }
                    else
                    {
                        string armaLinkGrafica = "[link] = ''" + Request.Path + "?Nav=2" + "&CenCosPad=" + iCodCenCos +
                                                    "&CenCos='' + convert(varchar,[CenCos]) + ''" +
                                                    "&fecIni=" + Session["FechaInicio"].ToString() +
                                                    "&fecFin=" + Session["FechaFin"].ToString() + "&TS=" + iCodTipoServicio + "''";

                        ldt = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefoniaNav2(iCodCenCos, iCodTipoServicio.ToString(), armaLinkGrafica));

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "CenCos", lsLanCentroDeCostos, lsLanImporte, "link" });
                            ldt.AcceptChanges();
                        }

                        Rep1.Controls.Add(
                                  DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                                  DTIChartsAndControls.GridView("Nav2GridPorCenCosYTS", ldt, true, lsLanTotales,
                                                  new string[] { "", "", "{0:c}" },
                                                  Request.Path + "?Nav=2" + "&CenCosPad=" + iCodCenCos + "&CenCos={0}&fecIni=" + Session["FechaInicio"].ToString() +
                                                    "&fecFin=" + Session["FechaFin"].ToString() + "&TS=" + iCodTipoServicio,
                                                  new string[] { "CenCos" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                                  "Nav2GridPorCenCosYTS_T", "Reporte")
                                                  );

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { lsLanCentroDeCostos, lsLanImporte, "link" });
                            if (ldt.Rows[ldt.Rows.Count - 1][lsLanCentroDeCostos].ToString() == lsLanTotales)
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns[lsLanCentroDeCostos].ColumnName = "label";
                            ldt.Columns[lsLanImporte].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("Nav2GrafPorCenCosYTS_G", lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + tituloGraf, 2, FCGpoGraf.Tabular));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Nav2GrafPorCenCosYTS",
                            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "Nav2GrafPorCenCosYTS_G",
                            "", "", lsLanCentroDeCostos, lsLanImporte, 2, FCGpoGraf.Tabular), false);

                        #endregion Grafica
                    }
                }
                else
                {
                    string armaLinkGrafica = "[link] = ''" + Request.Path + Page.Request.Url.Query + "&TDest='' + convert(varchar,[iCodTDest])";
                    ldt = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, iCodTipoServicio, armaLinkGrafica));

                    #region Grid

                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { "iCodTDest", lsLanTipoDestino, lsLanTotales, "link" });
                        ldt.AcceptChanges();
                    }

                    Rep1.Controls.Add(
                              DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                              DTIChartsAndControls.GridView("GridPorCenCosYTDest", ldt, true, lsLanTotales,
                                              new string[] { "", "", "{0:c}" }, Request.Path + Page.Request.Url.Query + "&TDest={0}",
                                              new string[] { "iCodTDest" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                              "GridPorCenCosYTDest_T", "Reporte")
                                              );

                    #endregion Grid

                    #region Grafica

                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { lsLanTipoDestino, lsLanTotales, "link" });
                        if (ldt.Rows[ldt.Rows.Count - 1][lsLanTipoDestino].ToString() == lsLanTotales)
                        {
                            ldt.Rows[ldt.Rows.Count - 1].Delete();
                        }
                        ldt.Columns[lsLanTipoDestino].ColumnName = "label";
                        ldt.Columns[lsLanTotales].ColumnName = "value";
                        ldt.AcceptChanges();
                    }

                    Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafPorCenCosYTDest_G", lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc, 2, FCGpoGraf.Tabular));

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPorCenCosYTDest",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GrafPorCenCosYTDest_G",
                        "", "", lsLanTipoDestino, lsLanTotales, 2, FCGpoGraf.Tabular), false);

                    #endregion Grafica

                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs #region Reporte por Tipo de Servicio de un CenCos'"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        //NZ 20151124 Se cambia reporte a fusionChart
        protected void ReporteConsumoAgrupadoPorEmpleado() //Reporte por TDest en grid agrupado por empleado
        {
            try
            {
                DataTable ldt = null;

                ldt = DTIChartsAndControls.DataTable(ConsultaConsumoAgrupadoPorEmpleado(iCodTipoDestino, iCodCenCos));
                //NZ 20151125 Se cambio metodo.
                if (Session["vchCodPerfil"].ToString().ToLower() == "config")
                {
                    #region Grid

                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { "Codigo Empleado", lsLanNombreCompleto, lsLanImporte });
                        ldt.AcceptChanges();
                    }

                    Rep9.Controls.Add(
                              DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                              DTIChartsAndControls.GridView("GridConsuAgrupEmple", ldt, true, lsLanTotales,
                                              new string[] { "", "", "{0:c}" }, Request.Path + Page.Request.Url.Query + "&iCodEmple={0}",
                                              new string[] { "Codigo Empleado" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                              "GridConsuAgrupEmple_T", "Reporte")
                                              );

                    #endregion Grid
                }
                else
                {
                    #region Grid

                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { lsLanNombreCompleto, lsLanImporte });
                        ldt.AcceptChanges();
                    }

                    Rep9.Controls.Add(
                              DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                              DTIChartsAndControls.GridView("GridConsuAgrupEmple", ldt, true, lsLanTotales,
                                              new string[] { "", "{0:c}" }), "GridConsuAgrupEmple_T", "Reporte")
                                              );

                    #endregion Grid

                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs  #region Reporte por TDest en grid agrupado por empleado '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        //NZ 20151124 Se cambia reporte a fusionChart
        protected void ReporteDetalladoPorTDestGroupEmple() //Reporte detallado por TDest en grid agrupado por empleado
        {
            try
            {
                DataTable ldt = null;

                ldt = DTIChartsAndControls.DataTable(ConsultaDetalleEmpleadoTDestCenCos(iCodEmple, iCodTipoDestino));

                #region Grid

                //NZ 20151125 Se cambio metodo.
                #region Elimina columnas no necesarias en el DataTable
                if (ldt.Columns.Contains("RID"))
                    ldt.Columns.Remove("RID");
                if (ldt.Columns.Contains("RowNumber"))
                    ldt.Columns.Remove("RowNumber");
                if (ldt.Columns.Contains("TopRID"))
                    ldt.Columns.Remove("TopRID");
                #endregion

                Rep9.Controls.Add(
                             DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                             DTIChartsAndControls.GridView("GridDetalladoPorTDest", ldt, true, lsLanTotales,
                                             new string[] { "", "", "", "", "", "", "{0:n0}", "{0:c}", "", "", "", "", "", "" }), "GridDetalladoPorTDest_T", "Reporte")
                                             );
                #endregion Grid
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs  " +
                    "#region Reporte detallado por TDest en grid agrupado por empleado '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        //NZ 20151124 Se cambia reporte a fusionChart
        protected void ReportePorCenCosOTipoTelefonia()  //Reporte Stack por CenCos o Por Tipo de Telefonia ( Depende de si el CenCos tiene Centros de costos dependientes )
        {
            try
            {
                DataTable ldt = null;

                #region Se decide que navegacion le crea el reporte de stack por CenCos
                int numeroDeCenCosDependientes = (int)DSODataAccess.ExecuteScalar(ConsultaNumeroDeCenCosDependientes(iCodCenCos.ToString()));
                if (numeroDeCenCosDependientes == 0 ||
                    (Request.QueryString["CenCosPad"].ToString() == Request.QueryString["CenCos"].ToString()) //NZ Significa que ya se ha llegado hasta el consumo propio del padre. En caso de seguir con la unica condicion anterior se volvera ciclico.
                    )
                {
                    string armaLink = "[link] = ''" + Request.Path + Page.Request.Url.Query + "&TS='' + convert(varchar,[CodCatTDest])";
                    ldt = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, armaLink));

                    #region Grid
                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { "CodCatTDest", lsLanTipoDeServicio, lsLanTotales, "link" });
                        ldt.AcceptChanges();
                    }

                    Rep1.Controls.Add(
                              DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                              DTIChartsAndControls.GridView("GridPorCenCosYTS", ldt, true, lsLanTotales,
                                              new string[] { "", "", "{0:c}" }, Request.Path + Page.Request.Url.Query + "&TS={0}",
                                              new string[] { "CodCatTDest" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }), "GridPorCenCosYTS_T",
                                              "Reporte")
                                              );

                    #endregion Grid

                    #region Grafica

                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(ldt);
                        ldt = dvldt.ToTable(false, new string[] { lsLanTipoDeServicio, lsLanTotales, "link" });
                        if (ldt.Rows[ldt.Rows.Count - 1][lsLanTipoDeServicio].ToString() == lsLanTotales)
                        {
                            ldt.Rows[ldt.Rows.Count - 1].Delete();
                        }
                        ldt.Columns[lsLanTipoDeServicio].ColumnName = "label";
                        ldt.Columns[lsLanTotales].ColumnName = "value";
                        ldt.AcceptChanges();
                    }

                    Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafPorCenCosYTS_T", lsLanConsumoPorTipoServicio + " / " + cenCosDesc, 1, FCGpoGraf.Tabular));



                    Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPorCenCosYTS",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GrafPorCenCosYTS_T",
                        "", "", lsLanTipoDeServicio, lsLanTotales, 1, FCGpoGraf.Tabular), false);

                    #endregion Grafica
                }
                else
                {
                    DataTable ldttc1 = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCosYTipoServicioMat(iCodCenCos));

                    #region Grid
                    //NZ 20141124
                    Rep0.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla( /* Este caso de usara una sobrecarga de GridViewLT que respeta la altura mandada. En el dashbord de FC si no es 0 se establece en 400. Aqui no.*/
                                   DTIChartsAndControls.GridView("RepConsCenCosJeraGrid2", DTIChartsAndControls.ordenaTabla(ldttc1, lsLanTotales + " desc"), true, lsLanTotales,
                                   new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" },
                                   "?CenCosPad=" + iCodCenCos + "&CenCos={0}" + "&fecIni=" + Session["FechaInicio"].ToString() + "&fecFin=" + Session["FechaFin"].ToString(),
                                   new string[] { "iCodCatalogo" }, 1, new int[] { 0 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }), "RepConsCenCosJeraGrid2_T", "Reporte"));

                    #endregion Grid

                    CreaGraficasStackEnDashboardPrincipal();
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs " +
                    "#region Reporte Stack por CenCos o Por Tipo de Telefonia ( Depende de si el CenCos tiene Centros de costos dependientes ) '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        //NZ 20151118 Se cambia reporte a fusionChart
        protected void ReportesDashboardPrincipal()
        {
            try
            {
                DataTable ldttc1 = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCosYTipoServicioMat(iCodCenCos));

                Rep0.Controls.Add(
                   DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                   DTIChartsAndControls.GridView("RepConsCenCosJeraGrid", DTIChartsAndControls.ordenaTabla(ldttc1, lsLanTotales + " desc"), true, lsLanTotales,
                                   new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" },
                                   "?CenCosPad=" + iCodCenCos + "&CenCos={0}" + "&fecIni=" + Session["FechaInicio"].ToString() + "&fecFin=" + Session["FechaFin"].ToString(),
                                   new string[] { "iCodCatalogo" }, 1, new int[] { 0 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }), "ReportesDashboardPrincipal_T", "Reporte"));

                CreaGraficasStackEnDashboardPrincipal();

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs " +
                    "#region Dashboard principal con grafias de stack '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

        }

        //NZ 20151119 Se cambia reporte a fusionChart
        protected void ResporteGraficasHistoricas()  //Crea Graficas Historicas
        {
            DataTable dtResultado = DTIChartsAndControls.DataTable(ConsultaHistoricoPorCenCosMesActualMenos12(iCodCenCos));

            if (dtResultado.Rows.Count > 0)
            {
                dtResultado.Rows.RemoveAt(11); //Para ternium nunca se graficara el mes actual
                dtResultado.Rows.RemoveAt(10); //NZ 20160120 Se solicito que no se visualizaran en las graficas los dos ultimos meses en las graficas Historicas.

                //NZ 20151126 Se acordo que hasta Marzo del siguiente año se omitira esta condicion. El plan es conseguir los 12 meses a partir de Marzo
                for (int i = 0; i < 3; i++)
                {
                    if (dtResultado.Rows[0][lsLanMes].ToString() == "Dic" || dtResultado.Rows[0][lsLanMes].ToString() == "Ene" || dtResultado.Rows[0][lsLanMes].ToString() == "Feb")
                    {
                        dtResultado.Rows.RemoveAt(0);
                    }
                }
                //NZ FIN 20151126 Se acordo que hasta Marzo del siguiente año se omitira esta condicion. El plan es conseguir los 12 meses a partir de Marzo


                #region Grafica Historica por Centro de Costos

                Rep3.Controls.Clear();
                Rep3.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosHist_T", lsLanHistoricoPorCentroDeCostos + " " + unidadNegocio, 0, FCGpoGraf.MatricialConStack));


                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtResultado));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosHist",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtResultado), "RepConsCenCosHist_T",
                    "", "", lsLanMes, lsLanImporte, 0, FCGpoGraf.MatricialConStack), false);

            }
                #endregion Grafica Historica por Centro de Costos

            dtResultado.Clear();
            dtResultado = DTIChartsAndControls.DataTable(ConsultaHistoricoPorTipoTelefoniaMesActualMenos12(iCodCenCos));
            if (dtResultado.Rows.Count > 0)
            {
                dtResultado.Rows.RemoveAt(11); //Para ternium nunca se graficara el mes actual
                dtResultado.Rows.RemoveAt(10); //NZ 20160120 Se solicito que no se visualizaran en las graficas los dos ultimos meses en las graficas Historicas.

                //NZ 20151126 Se acordo que hasta Marzo del siguiente año se omitira esta condicion. El plan es conseguir los 12 meses a partir de Marzo
                for (int i = 0; i < 3; i++)
                {
                    if (dtResultado.Rows[0][lsLanMes].ToString() == "Dic" || dtResultado.Rows[0][lsLanMes].ToString() == "Ene" || dtResultado.Rows[0][lsLanMes].ToString() == "Feb")
                    {
                        dtResultado.Rows.RemoveAt(0);
                    }
                }
                //NZ FIN 20151126 Se acordo que hasta Marzo del siguiente año se omitira esta condicion. El plan es conseguir los 12 meses a partir de Marzo

                #region Grafica Historica por Tipo de Servicio

                Rep4.Controls.Clear();
                Rep4.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsTipoServicioHist_T", lsLanHistoricoPorTipoDeTelefonia, 0, FCGpoGraf.MatricialConStack));

                string[] lsaDataSource2 = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtResultado));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsTipoServicioHist",
                    FCAndControls.GraficaMultiSeries(lsaDataSource2, FCAndControls.extraeNombreColumnas(dtResultado), "RepConsTipoServicioHist_T",
                    "", "", lsLanMes, lsLanImporte, 0, FCGpoGraf.MatricialConStack), false);


                #endregion Grafica Historica por Tipo de Servicio
            }
        }

        #endregion Reportes

        #region Metodos Auxiliares

        protected bool EnMilesONo(DataTable dtResult, string nombreColumImport)
        {
            //NZ Basicamente el nombre de la columna que se graficara en el eje Y, donde normalmente estan los importes.
            try
            {
                if (dtResult != null && dtResult.Rows.Count > 0 && dtResult.Columns.Contains(nombreColumImport))
                {
                    decimal maxValue = Convert.ToDecimal(dtResult.Compute("max(" + nombreColumImport + ")", string.Empty));
                    if (maxValue > 1000)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ocurrió un error al intentar obtener el valor mas alto en la tabla", ex);
            }
        }

        protected void RecortarNombre(DataTable dtResult, string nombreColumna)
        {
            try
            {
                string[] cadena = null;
                string result = string.Empty;
                int item = 0;
                foreach (DataRow row in dtResult.Rows)
                {
                    item++;
                    result = string.Empty;
                    cadena = row[nombreColumna].ToString().Split(new char[] { ' ' });
                    for (int i = 0; i < cadena.Length; i++)
                    {
                        result += (cadena[i].Length > 6) ? item.ToString() + " " + cadena[i].Remove(6) + ". " : item.ToString() + " " + cadena[i] + " ";
                    }
                    row[nombreColumna] = result.Trim();
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Ocurrió un error al intentar recortar los nombres");
            }
        }

        #endregion

        #region Exportacion

        public void ExportXLS()
        {
            CrearXLS(".xlsx");
        }

        protected DataTable ObtenerTablaAExportar()
        {
            DataTable table = null;

            string TServicio = Request.QueryString["TS"];
            if (iCodTipoServicio > 0 && iCodTipoDestino == 0)
            {
                if (nav > 0)
                {
                    int numeroDeCenCosDependientes = (int)DSODataAccess.ExecuteScalar(ConsultaNumeroDeCenCosDependientes(iCodCenCos.ToString()));
                    if (numeroDeCenCosDependientes == 0)
                    {
                        table = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, iCodTipoServicio, ""));
                        if (table.Rows.Count > 0 && table.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(table);
                            table = dvldt.ToTable(false, new string[] { lsLanTipoDestino, lsLanTotales });
                            table.Columns[lsLanTotales].ColumnName = "Total";
                            table.AcceptChanges();
                        }

                        exportacionTitulo = lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc;
                        exportacionGrafColumEjeX = lsLanTipoDestino;
                        exportacionWithGrafica = true;
                    }
                    else
                    {
                        table = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefoniaNav2(iCodCenCos, TServicio, ""));
                        if (table.Rows.Count > 0 && table.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(table);
                            table = dvldt.ToTable(false, new string[] { lsLanCentroDeCostos, lsLanImporte });
                            table.Columns[lsLanImporte].ColumnName = "Total";
                            table.AcceptChanges();
                        }

                        exportacionTitulo = lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + tituloGraf;
                        exportacionGrafColumEjeX = lsLanCentroDeCostos;
                        exportacionWithGrafica = true;
                    }
                }
                else
                {
                    table = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, iCodTipoServicio, ""));

                    if (table.Rows.Count > 0 && table.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(table);
                        table = dvldt.ToTable(false, new string[] { lsLanTipoDestino, lsLanTotales });
                        table.Columns[lsLanTotales].ColumnName = "Total";
                        table.AcceptChanges();
                    }
                    exportacionTitulo = lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc;
                    exportacionGrafColumEjeX = lsLanTipoDestino;
                    exportacionWithGrafica = true;
                }
            }
            else if (iCodTipoDestino > 0 && iCodEmple == 0)
            {
                table = DTIChartsAndControls.DataTable(ConsultaConsumoAgrupadoPorEmpleado(iCodTipoDestino, iCodCenCos));

                if (table.Rows.Count > 0 && table.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(table);
                    table = dvldt.ToTable(false, new string[] { lsLanNombreCompleto, lsLanImporte });
                    table.AcceptChanges();
                }

                exportacionTitulo = lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc; ;
                exportacionWithGrafica = false;
            }
            else if (iCodEmple > 0)
            {
                table = DTIChartsAndControls.DataTable(ConsultaDetalleEmpleadoTDestCenCos(iCodEmple, iCodTipoDestino));
                #region Elimina columnas no necesarias en el DataTable
                if (table.Columns.Contains("RID"))
                    table.Columns.Remove("RID");
                if (table.Columns.Contains("RowNumber"))
                    table.Columns.Remove("RowNumber");
                if (table.Columns.Contains("TopRID"))
                    table.Columns.Remove("TopRID");
                #endregion

                exportacionTitulo = lsLanConsumoPorTipoTelefonia + " / " + tipoServicioDesc + " / " + cenCosDesc; ;
                exportacionWithGrafica = false;
            }
            else if (iCodCenCos > 0 && iCodCenCos != (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()))
            {
                int numeroDeCenCosDependientes = (int)DSODataAccess.ExecuteScalar(ConsultaNumeroDeCenCosDependientes(iCodCenCos.ToString()));
                if (numeroDeCenCosDependientes == 0)
                {
                    table = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, ""));
                    if (table.Rows.Count > 0 && table.Columns.Count > 0)
                    {
                        DataView dvldt = new DataView(table);
                        table = dvldt.ToTable(false, new string[] { lsLanTipoDeServicio, lsLanTotales });
                        table.Columns[lsLanTotales].ColumnName = "Total";
                        table.AcceptChanges();
                    }
                    exportacionTitulo = lsLanConsumoPorTipoServicio + " / " + cenCosDesc;
                    exportacionGrafColumEjeX = lsLanTipoDeServicio;
                    exportacionWithGrafica = true;
                }
                else
                {
                    table = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCosYTipoServicioMat(iCodCenCos));
                    //table = DTIChartsAndControls.DataTable(ConsultaConsumoPorTDestRecursivo(iCodCenCos.ToString()));
                    table.Columns.RemoveAt(0);
                    table.Columns[lsLanTotales].ColumnName = "Total";
                    exportacionTitulo = lsLanConsumoPorCentroDeCostos + " " + unidadNegocio + " " + cenCosDesc;
                    exportacionGrafColumEjeX = unidadNegocio;
                    exportacionWithGrafica = true;
                    exportacionGrafMatrcial = true;
                }
            }
            else
            {
                table = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCosYTipoServicioMat(iCodCenCos));
                table.Columns.RemoveAt(0);
                table.Columns[lsLanTotales].ColumnName = "Total";
                exportacionTitulo = lsLanConsumoPorCentroDeCostos + " " + unidadNegocio + " " + cenCosDesc;
                exportacionGrafColumEjeX = unidadNegocio;
                exportacionWithGrafica = true;
                exportacionGrafMatrcial = true;

            }

            return table;
        }

        protected void CrearXLS(string lsExt)
        {

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                DataTable table = ObtenerTablaAExportar();

                if (!exportacionWithGrafica)
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, exportacionTitulo);

                    if (table.Rows.Count > 0)
                    {
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, table, "Reporte", "DatosReporte");
                    }
                }
                else
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\ReporteTablaYGrafico" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, exportacionTitulo);

                    if (table.Rows.Count > 0)
                    {
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, table, "Reporte", "DatosReporte");

                        if (!exportacionGrafMatrcial)
                        {
                            ExportacionExcelRep.CrearGrafico(table, "Grafica", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, exportacionGrafColumEjeX, "", "", "Total", "$#,0.00",
                                                             true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                        else
                        {
                            ExportacionExcelRep.CrearGrafico(table, "Grafica", new string[] { lsLanTelefoniaFija, lsLanTelefoniaMovil, lsLanTelefoniaEnlace, lsLanTelefoniaOtros },
                                                           new string[] { lsLanTelefoniaFija, lsLanTelefoniaMovil, lsLanTelefoniaEnlace, lsLanTelefoniaOtros },
                                                           new string[] { lsLanTelefoniaFija, lsLanTelefoniaMovil, lsLanTelefoniaEnlace, lsLanTelefoniaOtros },
                                                           exportacionGrafColumEjeX, "", "", "Total", "$#,0.00",
                                                           true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                    }
                }

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;

                lExcel.SalvarComo();

                exportacionTitulo = exportacionTitulo.Replace("/", "").Replace("\\", "").Replace(" ", "").
                                    Replace(".", "").Replace("?", "").Replace("*", "");

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte_" + exportacionTitulo + "_");
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

                }
            }
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {

            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion

    }
}
