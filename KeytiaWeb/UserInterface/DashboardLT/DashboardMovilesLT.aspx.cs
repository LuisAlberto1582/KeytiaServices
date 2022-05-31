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

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public partial class DashboardMovilesLT : System.Web.UI.Page
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

        protected void Page_Load(object sender, EventArgs e)
        {
            //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
            (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();

            #region Seleccion de idioma

            SeleccionaIdioma();

            #endregion         

            #region Almacenar en variable de sesion los urls de navegacion
            List<string> list = new List<string>();
            string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            if (Session["pltNavegacion"] != null) //Entonces ya tiene navegacion almacenada
            {
                list = (List<string>)Session["pltNavegacion"];
            }

            //Si el url no contiene querystring y la lista tiene urls hay que limpiar la lista
            if (!(lsURL.Contains("?")) && list.Count > 0)
            {
                //Asegurarse eliminar navegacion previa
                list.Clear();
            }

            //Si no existe entonces quiere decir que estoy en un nuevo nivel de navegacion
            if (!list.Exists(element => element == lsURL))
            {
                //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                list.Add(lsURL);
            }

            //Guardar en sesion la nueva lista
            Session["pltNavegacion"] = list;

            //Ocultar boton de regresar cuando solo exista un elemento en la lista
            if (list.Count <= 1)
            {
                btnRegresar.Visible = false;
            }

            #endregion

            #region Etiqueta de navegacion

            if (Session["estadoDeNavegacion"] != null
                && !string.IsNullOrEmpty(Request.QueryString["CenCos"])) //Entonces ya tiene navegacion almacenada
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion = (List<string>)Session["estadoDeNavegacion"];
                //NZ
                ConsultaDesc(Request.QueryString["CenCos"].ToString(), "CenCos");
                if (!listEtiquetaNavegacion.Contains(cenCosDesc))
                {
                    //NZ 20151109 Con los centros de costos se cambia navegacion para los ID y no por la descripción.
                    //listEtiquetaNavegacion.Add(Request.QueryString["CenCos"]);
                    listEtiquetaNavegacion.Add(cenCosDesc);
                }
                if (!listEtiquetaNavegacion.Contains(Request.QueryString["TS"]))
                {
                    listEtiquetaNavegacion.Add(Request.QueryString["TS"]);
                }
                if (!listEtiquetaNavegacion.Contains(Request.QueryString["TDest"]))
                {
                    listEtiquetaNavegacion.Add(Request.QueryString["TDest"]);
                }
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.Where(i => i != null).ToArray());
            }
            else
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion.Add(lsLanlblInicio);
                Session["estadoDeNavegacion"] = listEtiquetaNavegacion;
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.ToArray());
            }

            #endregion

            #region Lee Query String

            #region Revisar si el querystring Nav contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["Nav"]))
            {
                nav = Convert.ToInt32(Request.QueryString["Nav"]);
            }
            else
            {
                nav = 0;
            }

            #endregion

            #region Revisar si el querystring CenCos contiene un valor
            if (!string.IsNullOrEmpty(CenCos = Request.QueryString["CenCos"]))
            {
                try
                {
                    if (nav > 0)
                    {
                        if (!int.TryParse(Request.QueryString["CenCos"], out iCodCenCos))
                        {
                            int iCodCatalogoCenCos = 0;

                            if (int.TryParse(DSODataAccess.ExecuteScalar("select Max(iCodCatalogo) " +
                                "from [VisHistoricos('CenCos','Centro de Costos','Español')]" +
                                "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() " +
                                "and Descripcion = '" + CenCos + "' and CenCos = " + Request.QueryString["CenCosPad"]).ToString(), out iCodCatalogoCenCos))
                            {
                                iCodCenCos = (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCos());
                            }
                        }
                    }
                    else
                    {
                        if (!int.TryParse(Request.QueryString["CenCos"], out iCodCenCos))
                        {
                            iCodCenCos = (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCos());
                        }
                    }

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
            }

            #endregion

            #region Revisar si el querystring TS contiene un valor

            if (!string.IsNullOrEmpty(TipoServicio = Request.QueryString["TS"]))
            {
                try
                {
                    object temp = DSODataAccess.ExecuteScalar(ConsultaICodTipoServicio());
                    if (temp != null)
                    {
                        iCodTipoServicio = (int)temp;
                    }
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

            #region Revisar si el querystring TDest contiene un valor

            if (!string.IsNullOrEmpty(TipoDestino = Request.QueryString["TDest"]))
            {
                try
                {
                    DataRow temp = DSODataAccess.ExecuteDataRow(ConsultaiCodTDest(iCodCenCos, iCodTipoServicio, Request.QueryString["TDest"]));

                    if (temp != null)
                    {
                        iCodTipoDestino = (int)temp["iCodTDest"];
                    }
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

            #region Revisar si el querystring iCodEmple contiene un valor

            if (!string.IsNullOrEmpty(Empleado = Request.QueryString["iCodEmple"]))
            {
                try
                {
                    iCodEmple = Convert.ToInt32(Empleado);
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs al revisar un valor de querystring iCodEmple '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodEmple = 0;
            }

            #endregion

            #endregion

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["fecIni"]) && !string.IsNullOrEmpty(Request.QueryString["fecFin"]))
                {
                    try
                    {
                        // Crea fechas que se mandan en querystring
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)Convert.ToDateTime(Request.QueryString["fecIni"]);
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)Convert.ToDateTime(Request.QueryString["fecFin"]);
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
                    #region Inicia los valores default de los controles de fecha
                    try
                    {
                        if (Session["FechaInicioRepDashLT"] != null && Session["FechaFinRepDashLT"] != null)
                        {
                            pdtInicio.CreateControls();
                            pdtInicio.DataValue =
                                (object)Convert.ToDateTime(Session["FechaInicioRepDashLT"].ToString().Substring(1, 10));
                            pdtFin.CreateControls();
                            pdtFin.DataValue =
                                (object)Convert.ToDateTime(Session["FechaFinRepDashLT"].ToString().Substring(1, 10));
                        }

                        else
                        {
                            pdtInicio.CreateControls();
                            pdtInicio.DataValue = (object)fechaInicio;
                            pdtFin.CreateControls();
                            pdtFin.DataValue = (object)fechaFinal;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion //Fin de bloque --Inicia los valores default de los controles de fecha
                }

            }

            #region Fechas en sesion
            //Si la fecha de inicio es mayor a la fecha fin no se hara el cambio de fechas, de lo contrario marcará error.
            if (pdtInicio.Date < pdtFin.Date)
            {
                Session["FechaInicioRepDashLT"] = pdtInicio.DataValue.ToString();
                Session["FechaFinRepDashLT"] = pdtFin.DataValue.ToString();
            }

            if (Session["FechaInicioRepDashLT"] != null && Session["FechaFinRepDashLT"] != null)
            {
                pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRepDashLT"].ToString().Substring(1, 10));
                pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashLT"].ToString().Substring(1, 10));
            }

            #endregion

            #region Adquiere nivel de cencos

            int nivelJerarquico = AdquiereNivelCenCosMovil();

            #endregion

            if (iCodTipoServicio > 0 && iCodTipoDestino == 0)
            {
                ReportePorTipoDestinoEnCenCosMovil();
            }
            else if (iCodTipoDestino > 0 && iCodEmple == 0)
            {
                ReporteTDestAgrupadoPorEmpleMovil();
            }
            else if (iCodEmple > 0)
            {
                ReporteTDestDetalladoGroupEmpleMovil();
            }
            else if (iCodCenCos > 0 && iCodCenCos != (int)DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()))
            {
                ReportePorCenCosOTDestMovil();
            }
            else
            {
                DashboardPrincipalMovil();
            }

            if (tblReportes.Visible) //Crea Graficas Historicas
            {
                ReportesHistoricosMoviles();
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

            #region Cargar textos de controles, segun idioma seleccionado

            //NZ 20151105
            //lblTipoMoneda.Text = lsLanMoneda + ConsultaDescMoneda();

            #endregion
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


        protected void CreaGraficasStackEnDashboardPrincipal()
        {
            DataTable ldt = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCodYTDestMoviles(iCodCenCos, "Móvil"));
            //ldt = DTIChartsAndControls.selectTopNTabla(ldt, "Totales Desc", 10);
            if (lblInicio.Text.Contains('/'))
            {
                //NZ Se quita el recorte de los nombres de los centros de costos.
                //RecortarNombre(ldt, lsLanCentroDeCostos);
            }

            tc3.Controls.Add(agregaTituloYBordesDeReporte(creaGraficaStack(ldt,
                                                                            lsLanTipoDestino,
                                                                            unidadNegocio,
                                                                            lsLanImporte,
                                                                            lsLanConsumoPorCentroDeCostos + " " + unidadNegocio,
                                                                            "&CenCosPad=" + iCodCenCos + "&CenCos",
                                                                            "GrafSatck1",
                                                                            unidadNegocio,
                                                                            lsLanCosto,
                                                                            EnMilesONo(ldt, lsLanImporte)
                                                                         ), "Gráfica", 1));

            tc4.Controls.Add(agregaTituloYBordesDeReporte(creaGraficaStack(ldt,
                                                                            unidadNegocio,
                                                                            lsLanTipoDestino,
                                                                            lsLanImporte,
                                                                            lsLanConsumoPorTipoDestino,
                                                                            "&TS=Móvil&Nav=2&CenCosPad=" +
                                                                            DSODataAccess.ExecuteScalar(ConsultaICodCenCosUsuario()).ToString() + "&TDest",
                                                                            "GrafSatck2",
                                                                            lsLanTipoDestino,
                                                                            lsLanCosto,
                                                                            EnMilesONo(ldt, lsLanImporte)
                                                                         ), "Gráfica", 1));
        }

        #region Codigo de botones de la pagina

        protected void btnDashPrincipal_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/DashboardLT/DashboardLT.aspx");
        }

        protected void btnDashMoviles_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/DashboardLT/DashboardMovilesLT.aspx");
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            List<string> ltNavegacion = (List<string>)Session["pltNavegacion"];

            //obtener el numero actual de elementos de la lista
            string lsCantidadElem = ltNavegacion.Count.ToString();
            //eliminar los dos ultimos elementos de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            #region Etiqueta de navegacion

            List<string> ltEtiquetaNavegacion = (List<string>)Session["estadoDeNavegacion"];
            //eliminar los tres ultimos elementos de la lista
            for (int i = 0; i < 2; i++)
            {
                if (ltEtiquetaNavegacion.Count > 1)
                {
                    ltEtiquetaNavegacion.RemoveAt(ltEtiquetaNavegacion.Count - 1);
                }
            }

            Session["estadoDeNavegacion"] = ltEtiquetaNavegacion;

            #endregion

            HttpContext.Current.Response.Redirect(lsLastElement);
        }

        #endregion

        #region Consultas SQL

        protected string ConsultaConsumoPorCenCosYTipoDestinoMat(int iCodCenCos, int iCodCatTDest)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec ObtieneConsumoJerarquicoPorTDest @esquema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@usuario=" + Session["iCodUsuario"] + ", \r");
            lsb.Append("@CatTDest=" + iCodCatTDest + ",  \r");
            lsb.Append("@CenCos=" + iCodCenCos + ",  \r");
            lsb.Append("@fieldCenCos = '" + unidadNegocio + "',  \r");
            lsb.Append("@FechaInicio='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r");
            lsb.Append("@FechaFin = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59', \r");
            lsb.Append("@Order = '[Total] desc'\r");

            return lsb.ToString();
        }

        protected string ConsultaConsumoPorCenCodYTDestMoviles(int iCodCenCos, string CatTDestDesc)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" exec ConsumoAcumuladoCenCosTDest \r");
            lsb.Append(" @Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append(" @Fields = 'iCodCatalogo = 0, [" + unidadNegocio + "] = CenCosDesc,[TDestDesc] as [" + lsLanTipoDestino + "], [" + lsLanImporte + "] = SUM([Costo])', \r");
            lsb.Append(" @Group = '[CenCosDesc],[TDestDesc]',\r");
            lsb.Append(" @Order = '[" + lsLanImporte + "] desc', \r");
            lsb.Append(" @FechaIniRep = '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00',\r");
            lsb.Append(" @FechaFinRep = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59',\r");
            lsb.Append(" @CenCosBase = " + iCodCenCos + ", \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append(" @CatTDest = 85764, \r");
            //lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append(" @Where = '[CatTDestDesc] = ''" + CatTDestDesc + "''' \r");

            return lsb.ToString();
        }

        protected string ConsultaConsumoPorCenCodYTDestMovilesNav2(int iCodCenCos, string CatTDestDesc, string TDestDesc)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" exec ConsumoAcumuladoCenCosTDest \r");
            lsb.Append(" @Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append(" @Fields = 'iCodCatalogo = 0, [" + lsLanCentroDeCostos + "] = CenCosDesc, [" + lsLanImporte + "] = SUM([Costo])', \r");
            lsb.Append(" @Group = '[CenCosDesc]',\r");
            lsb.Append(" @Order = '[" + lsLanImporte + "] desc', \r");
            lsb.Append(" @FechaIniRep = '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00',\r");
            lsb.Append(" @FechaFinRep = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59',\r");
            lsb.Append(" @CenCosBase = " + iCodCenCos + ", \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            //lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append(" @Where = '[CatTDestDesc] = ''" + CatTDestDesc + "'' AND [TDestDesc] = ''" + TDestDesc + "''' \r");

            return lsb.ToString();
        }

        protected string ConsultaConsumoTDestDeUnCenCos(int iCodCenCos, string CatTDestDesc)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" exec ConsumoAcumuladoCenCosTDest \r");
            lsb.Append(" @Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append(" @Fields = 'iCodCatalogo = 0, [TDestDesc] as [" + lsLanTipoDestino + "], [" + lsLanImporte + "] = SUM([Costo])', \r");
            lsb.Append(" @Group = '[CenCosDesc],[TDestDesc]',\r");
            lsb.Append(" @Order = '[" + lsLanImporte + "] desc', \r");
            lsb.Append(" @FechaIniRep = '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00',\r");
            lsb.Append(" @FechaFinRep = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59',\r");
            lsb.Append(" @CenCosBase = " + iCodCenCos + ", \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            //lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append(" @Where = '[CatTDestDesc] = ''" + CatTDestDesc + "''' \r");

            return lsb.ToString();
        }

        protected string ConsultaHistoricoPorCenCosMesActualMenos12Moviles(int iCodCenCosPad, string catTDestDesc)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec ConsumoHistorico12MesesCenCosTdest  \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[" + lsLanCentroDeCostos + "] = CenCos, Mes as [" + lsLanMes + "], [" + lsLanCosto + "] = SUM (Costo)', \r");
            lsb.Append("@Group = 'CenCos, Mes', \r");
            lsb.Append("@Where = 'CatTDestDesc = ''" + catTDestDesc + "'' AND [TDestDesc] not like ''%por%identificar%'' AND [CenCos] not like ''%por%identificar%''', \r");
            //lsb.Append("@Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@CenCosBase = " + iCodCenCosPad + " \r");

            return lsb.ToString();
        }

        protected string ConsultaHistoricoPorTipoDestinoMesActualMenos12(int iCodCenCosPad, string catTDestDesc)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec ConsumoHistorico12MesesCenCosTdest  \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields = '[" + lsLanTipoDestino + "] = TDestDesc, Mes as [" + lsLanMes + "], [" + lsLanCosto + "] = SUM (Costo) ', \r");
            lsb.Append("@Group = 'TDestDesc, Mes', \r");
            lsb.Append("@Where = 'CatTDestDesc = ''" + catTDestDesc + "'' AND [TDestDesc] not like ''%por%identificar%'' AND [CenCos] not like ''%por%identificar%''', \r");
            //lsb.Append("@Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@CenCosBase = " + iCodCenCosPad + " \r");

            return lsb.ToString();
        }

        protected int AdquiereNivelCenCosMovil()
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

        protected string ConsultaDetalleMovilesEmpleadoTDestCenCos(int iCodEmple, int iCodTDest)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)  \r");
            lsb.Append("declare @OrderInv varchar(max)  \r");
            lsb.Append("set @Where = '[Fecha Inicio] >= ''" + Session["FechaInicio"].ToString() + " 00:00:00'' \r");
            lsb.Append("'+' and [Fecha Inicio] <= ''" + Session["FechaFin"].ToString() + " 23:59:59''' \r");
            lsb.Append("+ ' and [iCodEmple] = " + iCodEmple + " and [iCodTDest] = " + iCodTDest + " ' \r");
            lsb.Append("exec ConsumoDetalladoMovilesDashboardLT   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@Fields=' \r");
            lsb.Append("[Centro de costos] as [" + lsLanCentroDeCostos + "], \r");
            lsb.Append("[Empleado]	 as [" + lsLanNombreCompleto + "], \r");
            lsb.Append("[Móvil]	 as [" + lsLanNumeroMovil + "], \r");
            lsb.Append("[Número Marcado] as [" + lsLanNumeroMarcado + "], \r");
            lsb.Append("[Fecha] as [" + lsLanFecha + "], \r");
            lsb.Append("[Hora] as [" + lsLanHora + "], \r");
            lsb.Append("[Duracion] as [" + lsLanDuracion + "], \r");
            lsb.Append("[Total] as [" + lsLanTotales + "], \r");
            lsb.Append("[Nombre Carrier] as [" + lsLanCarrier + "] \r");

            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,   \r");
            lsb.Append("@Order = '[" + lsLanTotales + "] desc',  \r");
            lsb.Append("@OrderDir = 'Asc',  \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + " 00:00:00',  \r");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + " 23:59:59'  \r");

            return lsb.ToString();
        }

        protected string ConsultaiCodTDest(int iCodCenCosPad, int iCodTipoTelefonia, string tipoDestino)
        {
            StringBuilder lsb = new StringBuilder();

            /*Resumen por CenCos y TDest*/
            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Request.QueryString["fecIni"] + " 00:00:00''   \r");
            lsb.Append("and FechaInicio <= ''" + Request.QueryString["fecFin"] + " 23:59:59''' \r");
            lsb.Append("+' AND [Codigo Centro de Costos] =" + iCodCenCosPad + "' \r");
            lsb.Append("+' AND [CodCatTDest] =" + iCodTipoTelefonia + "' \r");
            lsb.Append("+' AND [Tipo Destino] = ''" + tipoDestino + "''' \r");
            lsb.Append("exec RepMatResumenPorPaisDashLT   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@InnerFields='[iCodTDest]', \r");
            lsb.Append("@InnerWhere=@Where,  \r");
            lsb.Append("@InnerGroup='[iCodTDest]',  \r");
            lsb.Append("@OuterFields='[iCodTDest]', \r");
            lsb.Append("@OuterGroup='[iCodTDest]', \r");
            lsb.Append("@OrderDir='Asc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + Request.QueryString["fecIni"] + " 00:00:00''',  \r");
            lsb.Append("@FechaFinRep = '''" + Request.QueryString["fecFin"] + " 23:59:59''' \r");

            return lsb.ToString();
        }

        protected string ConsultaICodCenCos()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" select isnull(Max(iCodCatalogo), 0) \r");
            lsb.Append(" from [VisHistoricos('CenCos','Centro de Costos','Español')] \r");
            lsb.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" and Descripcion = '" + CenCos + "' and CenCos = " + Request.QueryString["CenCosPad"]);

            return lsb.ToString();
        }

        protected string ConsultaICodTipoServicio()
        {
            StringBuilder lsb = new StringBuilder();

            //lsb.Append(" Select iCodRegistro From Catalogos \r");
            //lsb.Append(" where vchDescripcion like '%" + TipoServicio + "%' and iCodCatalogo = ");
            //lsb.Append(" (Select top 1 iCodRegistro From Catalogos \r");
            //lsb.Append(" where  vchCodigo = 'CatTdest' and iCodCatalogo is null) ");

            lsb.Append(" select isnull(max(iCodCatalogo),0) as [iCodCatalogo] \r");
            lsb.Append(" from [VisHistoricos('CatTDest','Categoría de tipo destino','Español')] \r");
            lsb.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" and (Español = '" + TipoServicio + "' or Ingles = '" + TipoServicio + "'  \r");
            lsb.Append(" or Frances = '" + TipoServicio + "' or Portugues = '" + TipoServicio + "' or Aleman = '" + TipoServicio + "') \r");

            return lsb.ToString();
        }   

        protected string ConsultaICodCenCosUsuario()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" select isnull(Max(CenCos),0) \r");
            lsb.Append(" from [VisHistoricos('Usuar','Usuarios','Español')] \r");
            lsb.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" and iCodCatalogo = " + Session["iCodUsuario"]);

            return lsb.ToString();
        }

        protected string ConsultaConsumoAgrupadoPorEmpleado(int iCodTDest, int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("set @Where = '[FechaInicio] between ''" + Session["FechaInicio"].ToString() + "'' AND ''" + Session["FechaFin"].ToString() + "''' \r");
            lsb.Append(" + ' AND [Codigo Tipo Destino] = " + iCodTDest + " AND [Codigo Centro de Costos] = " + iCodCenCos + "' \r");
            lsb.Append("exec ConsumoAcumuladoTodosCamposRestDashboardLT  \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@Fields = '[Codigo Empleado],[Nombre Completo] as [" + lsLanNombreCompleto + "],[" + lsLanImporte + "] = CONVERT(MONEY,SUM([Costo]))', \r");
            lsb.Append("@Group = '[Codigo Empleado],[Nombre Completo]', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@Order = '[" + lsLanImporte + "] desc', \r");
            lsb.Append("@Where =  @Where, \r");
            lsb.Append("@FechaIniRep = '" + Session["FechaInicio"].ToString() + "',  \r");
            lsb.Append("@FechaFinRep = '" + Session["FechaFin"].ToString() + "' \r");

            return lsb.ToString();
        }

        protected string ConsultaNumeroDeCenCosDependientes(string lsiCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" Select COUNT(*) from [VisHistoricos('CenCos','Centro de Costos','Español')] \r");
            lsb.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" and CenCos = " + lsiCodCenCos);

            return lsb.ToString();
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
                    lsb.AppendLine(" SELECT " + idioma + " AS Descripcion \r");
                    lsb.AppendLine(" FROM [VisHistoricos('CatTDest','Categoría de tipo destino','Español')] ");
                    lsb.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() ");
                    lsb.AppendLine("    AND iCodCatalogo = " + iCodCatalogo);
                    break;
                case "TDest":
                    lsb.AppendLine(" SELECT vchDescripcion AS Descripcion \r");
                    lsb.AppendLine(" FROM [VisHistoricos('TDest','Tipo de Destino','Español')]  \r");
                    lsb.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() \r");
                    lsb.AppendLine("    AND iCodCatalogo = " + iCodCatalogo);
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

        protected string ConsultaCostoPorCenCosYTipoTelefonia(int iCodCenCosPad, int iCodTipoTelefonia, string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            /*Resumen por CenCos y TDest*/
            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''   \r");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59''' \r");
            lsb.Append("+' AND [Codigo Centro de Costos] =" + iCodCenCosPad + "' \r");
            lsb.Append("+' AND [CodCatTDest] =" + iCodTipoTelefonia + "' \r");
            lsb.Append("exec RepMatResumenPorPaisDashLT   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@InnerFields='[Codigo Centro de Costos], [iCodTDest], [Tipo Destino] as [" + lsLanTipoDestino + "], \r");
            lsb.Append("[Total] = SUM([Costo]) + SUM([CostoSM])',  \r");
            lsb.Append("@InnerWhere=@Where,  \r");
            lsb.Append("@InnerGroup='[Codigo Centro de Costos], [iCodTDest], [Tipo Destino]',  \r");
            lsb.Append("@OuterFields='[iCodTDest], [" + lsLanTipoDestino + "], \r");
            if (!string.IsNullOrEmpty(linkGrafica))
            {
                lsb.Append(linkGrafica + ", \r");
            }
            lsb.Append("[" + lsLanTotales + "] = SUM([Total])', \r");
            lsb.Append("@OuterGroup='[iCodTDest], [" + lsLanTipoDestino + "] , [Codigo Centro de Costos]', \r");
            lsb.Append("@Order='[" + lsLanTotales + "] Desc,[" + lsLanTipoDestino + "] Asc, [iCodTDest]', \r");
            lsb.Append("@OrderInv='[iCodTDest], [" + lsLanTipoDestino + "] Desc,[" + lsLanTotales + "] Asc', \r");
            lsb.Append("@OrderDir='Asc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''',  \r");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''' \r");

            return lsb.ToString();
        }

        #endregion

        #region Reportes  //NZ

        private void ReportePorTipoDestinoEnCenCosMovil() //Reporte por Tipo de Servicio de un CenCos
        {
            try
            {
                DataTable ldt = null;
                if (nav > 0)
                {
                    tblReportes.Visible = false;

                    ldt = DTIChartsAndControls.DataTable(ConsultaConsumoPorCenCodYTDestMovilesNav2(iCodCenCos, "Móvil", TipoDestino));
                    tc2tblGridYGraf.Controls.Add(
                        agregaTituloYBordesDeReporte(creaGraficaBarras(ldt,
                                                                       lsLanCentroDeCostos, lsLanImporte,
                                                                       lsLanConsumoPorTipoDestino + " / " + TipoDestino + " / " + tituloGraf,
                                                                       "&CenCosPad=" + iCodCenCos + "&CenCos",
                                                                       unidadNegocio,
                                                                       lsLanImporte,
                                                                       EnMilesONo(ldt, lsLanImporte)
                                                                       ), "Gráfica", 1));

                    tc1tblGridYGraf.Controls.Add(
                        agregaTituloYBordesDeReporte(creaGridView(ldt,
                                                                  "?&Nav=2&TS=Móvil&TDest=" + TipoDestino + "&CenCosPad=" + iCodCenCos + "&CenCos={0}" +  //NZ
                                                                  "&fecIni=" + pdtInicio.Date.ToString("yyyy-MM-dd") +
                                                                  "&fecFin=" + pdtFin.Date.ToString("yyyy-MM-dd"),
                                                                  "iCodCatalogo", lsLanCentroDeCostos, lsLanCentroDeCostos,
                                                                  new string[] { "iCodCatalogo", lsLanCentroDeCostos }, 0), "Reporte", 1));

                }
                else
                {
                    tblReportes.Visible = false;
                    ldt = DTIChartsAndControls.DataTable(ConsultaCostoPorCenCosYTipoTelefonia(iCodCenCos, iCodTipoServicio, ""));
                    tc2tblGridYGraf.Controls.Add(
                        agregaTituloYBordesDeReporte(creaGraficaBarras(ldt,
                                                                       lsLanTipoDestino,
                                                                       lsLanTotales,
                                                                       lsLanConsumoPorTipoTelefonia + " / " + TipoServicio + " / " + cenCosDesc, "TDest",  //NZ Request.QueryString["CenCos"]
                                                                       lsLanTipoDestino,
                                                                       lsLanTotales,
                                                                       EnMilesONo(ldt, lsLanTotales)
                                                                       ), "Gráfica", 1));

                    tc1tblGridYGraf.Controls.Add(agregaTituloYBordesDeReporte(creaGridView(ldt,
                                                                                           Page.Request.Url.Query + "&TDest={1}",
                                                                                           "iCodCatalogo", lsLanTipoDestino, lsLanTipoDestino,
                                                                                           new string[] { "iCodCatalogo", lsLanTipoDestino }, 0), "Reporte", 1));
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs #region Reporte por Tipo de Servicio de un CenCos'"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void ReporteTDestAgrupadoPorEmpleMovil() //Reporte por TDest en grid agrupado por empleado
        {
            try
            {
                DataTable ldt = null;
                tblReportes.Visible = false;

                if (Session["vchCodPerfil"].ToString().ToLower() == "config")
                {
                    ldt = DTIChartsAndControls.DataTable(ConsultaConsumoAgrupadoPorEmpleado(iCodTipoDestino, iCodCenCos));
                    tr1tblGridYGraf.Controls.Remove(tc2tblGridYGraf);
                    tc1tblGridYGraf.ColumnSpan = 2;
                    tc1tblGridYGraf.Controls.Add(agregaTituloYBordesDeReporte(creaGridView(ldt,
                                                                                           "&iCodEmple={0}",
                                                                                           "Codigo Empleado",
                                                                                           lsLanNombreCompleto,
                                                                                           lsLanNombreCompleto,
                                                                                           new string[] { "Codigo Empleado" }, 0), "Reporte", 2));
                }
                else
                {
                    ldt = DTIChartsAndControls.DataTable(ConsultaConsumoAgrupadoPorEmpleado(iCodTipoDestino, iCodCenCos));
                    tr1tblGridYGraf.Controls.Remove(tc2tblGridYGraf);
                    tc1tblGridYGraf.ColumnSpan = 2;
                    tc1tblGridYGraf.Controls.Add(agregaTituloYBordesDeReporte(creaGridView(ldt,
                                                                                           true,
                                                                                           0,
                                                                                           new int[] { 0 },
                                                                                           new string[] { "", "", "{0:c}" },
                                                                                           1), "Reporte", 2));
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs  #region Reporte por TDest en grid agrupado por empleado '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void ReporteTDestDetalladoGroupEmpleMovil()  //Reporte detallado por TDest en grid agrupado por empleado
        {
            try
            {
                DataTable ldt = null;
                tblReportes.Visible = false;

                ldt = DTIChartsAndControls.DataTable(ConsultaDetalleMovilesEmpleadoTDestCenCos(iCodEmple, iCodTipoDestino));
                tr1tblGridYGraf.Controls.Remove(tc2tblGridYGraf);
                tc1tblGridYGraf.ColumnSpan = 2;
                tc1tblGridYGraf.Controls.Add(agregaTituloYBordesDeReporte(creaGridView(ldt, false, 0), "Reporte", 2));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                    "Ocurrio un error al generar reportes en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs  " +
                    "#region Reporte detallado por TDest en grid agrupado por empleado '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void ReportePorCenCosOTDestMovil() //Reporte Stack por CenCos o Por Tipo de Destino ( Depende de si el CenCos tiene Centros de costos dependientes )
        {
            try
            {
                DataTable ldt = null;
                tblReportes.Visible = false;

                #region Se decide que navegacion le crea el reporte de stack por CenCos
                int numeroDeCenCosDependientes = (int)DSODataAccess.ExecuteScalar(ConsultaNumeroDeCenCosDependientes(iCodCenCos.ToString()));
                if (numeroDeCenCosDependientes == 0)
                {
                    tblReportes.Visible = false;
                    ldt = DTIChartsAndControls.DataTable(ConsultaConsumoTDestDeUnCenCos(iCodCenCos, "Móvil"));

                    tc2tblGridYGraf.Controls.Add(
                        agregaTituloYBordesDeReporte(creaGraficaBarras(ldt,
                                                                      lsLanTipoDestino,
                                                                      lsLanImporte,
                                                                      lsLanConsumoPorTipoDestino + " / " + cenCosDesc, //NZ Request.QueryString["CenCos"]
                                                                      "&TS=Móvil&TDest",
                                                                      lsLanTipoDestino,
                                                                      lsLanImporte,
                                                                      EnMilesONo(ldt, lsLanImporte)
                                                                      ), "Gráfica", 1));

                    tc1tblGridYGraf.Controls.Add(
                        agregaTituloYBordesDeReporte(creaGridView(ldt,
                                                                  Page.Request.Url.Query + "&TS=Móvil&TDest={1}",
                                                                  "iCodCatalogo", lsLanTipoDestino, lsLanTipoDestino,
                                                                  new string[] { "iCodCatalogo", lsLanTipoDestino }, 0), "Reporte", 1));
                }
                else
                {
                    tblReportes.Visible = true;
                    tblGridYGraf.Visible = false;

                    tr1.Controls.Remove(tc2);
                    tc1.ColumnSpan = 2;
                    tc1.Controls.Add(
                          agregaTituloYBordesDeReporte(creaGridView(DTIChartsAndControls.DataTable(
                                                        ConsultaConsumoPorCenCosYTipoDestinoMat(iCodCenCos, 85764)),
                                                        "?&CenCosPad=" + iCodCenCos + "&CenCos={0}" +  //NZ
                                                        "&fecIni=" + pdtInicio.Date.ToString("yyyy-MM-dd") +
                                                        "&fecFin=" + pdtFin.Date.ToString("yyyy-MM-dd"),
                                                        "cencos", unidadNegocio, unidadNegocio,
                                                        new string[] { "cencos", unidadNegocio }, 150), "Reporte", 2));


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

        private void DashboardPrincipalMovil()  // Dashboard principal con grafias de stack
        {
            try
            {
                tblGridYGraf.Visible = false;

                tr1.Controls.Remove(tc2);
                tc1.ColumnSpan = 2;
                tc1.Controls.Add(
                      agregaTituloYBordesDeReporte(creaGridView(DTIChartsAndControls.DataTable(
                                                    ConsultaConsumoPorCenCosYTipoDestinoMat(iCodCenCos, 85764)),
                                                    "?&CenCosPad=" + iCodCenCos + "&CenCos={0}" +   //NZ
                                                    "&fecIni=" + pdtInicio.Date.ToString("yyyy-MM-dd") +
                                                    "&fecFin=" + pdtFin.Date.ToString("yyyy-MM-dd"),
                                                    "cencos", unidadNegocio, unidadNegocio,
                                                    new string[] { "cencos", unidadNegocio }, 150), "Reporte", 2));


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

        private void ReportesHistoricosMoviles() //Crea Graficas Historicas
        {
            DataTable dtResultado = DTIChartsAndControls.DataTable(ConsultaHistoricoPorCenCosMesActualMenos12Moviles(iCodCenCos, "Móvil"));
            tc5.Controls.Add(agregaTituloYBordesDeReporte(creaGraficaLinea(dtResultado,
                                                                           lsLanHistoricoPorCentroDeCostos + " " + unidadNegocio,
                                                                           lsLanCentroDeCostos,
                                                                           lsLanMes,
                                                                           lsLanCosto,
                                                                           string.Empty,
                                                                           lsLanCosto,
                                                                           "GrafHist1",
                                                                           EnMilesONo(dtResultado, lsLanCosto)
                                                                           ), "Gráfica", 1));

            dtResultado.Clear();
            dtResultado = DTIChartsAndControls.DataTable(ConsultaHistoricoPorTipoDestinoMesActualMenos12(iCodCenCos, "Móvil"));
            tc6.Controls.Add(agregaTituloYBordesDeReporte(creaGraficaLinea(dtResultado,
                                                                           lsLanHistoricoPorOperadora,
                                                                           lsLanTipoDestino,
                                                                           lsLanMes,
                                                                           lsLanCosto,
                                                                           string.Empty,
                                                                           lsLanCosto,
                                                                           "GrafHist2",
                                                                           EnMilesONo(dtResultado, lsLanCosto)
                                                                          ), "Gráfica", 1));
        }

        #endregion
        
        ///Graficador 
        protected Control creaGraficaLinea(DataTable localDT, string lsTituloReporte, string seriesAgrupadasPor, string EjeX, string EjeY, string tituloEjeX, string tituloEjeY, string chartid, bool formatoMiles)
        {
            #region Proceso de creacion de la grafica de linea
            if (localDT.Rows.Count > 0)
            {
                try
                {
                    Chart chart = new Chart();

                    chart.DataBindCrossTable(localDT.AsEnumerable(), seriesAgrupadasPor, EjeX, EjeY, "");
                    chart.AlignDataPointsByAxisLabel();

                    foreach (Series s in chart.Series)
                    {
                        s.ChartType = SeriesChartType.Line;
                        s.BorderWidth = 2;
                        //s.Url = Session["HomePage"].ToString() + "?" + s.Name.ToString().Replace(" ", "").Trim();
                    }

                    chart.Legends.Add("Legend");
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;

                    chart.ChartAreas.Add("ChartArea");

                    chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                    chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;

                    chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.Minimum = 0;

                    if (formatoMiles) //NZ 20151106
                    {
                        chart.ChartAreas[0].AxisY.LabelStyle.Format = "$ 0,";
                        tituloEjeY += lsLanImporteEnMiles;
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisY.LabelStyle.Format = "$ #,#.##";
                    }

                    if (lsTituloReporte.Length > 0)
                    {
                        chart.Titles.Add(new Title(lsTituloReporte,
                                                    Docking.Top,
                                                    new Font("Verdana", 12f, FontStyle.Bold),
                                                    Color.Black
                                                     ));
                    }
                    if (tituloEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(tituloEjeX,
                                                    Docking.Bottom,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black
                                                     ));
                    }
                    if (tituloEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(tituloEjeY,
                                                    Docking.Left,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black
                                                     ));
                    }
                    chart.Height = 400;
                    chart.Width = 610;

                    chart.ID = chartid;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaLinea() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        protected Control creaGraficaStack(DataTable DataSource, string SeriesAgrupadasPor, string EjeX, string EjeY, string TituloGrafica, string Navegacion, string chartid, string tituloEjeX, string tituloEjeY, bool formatoMiles)
        {
            #region Proceso de creacion de la grafica
            if (DataSource.Rows.Count > 0)
            {
                try
                {
                    Chart Chart1 = new Chart();
                    Chart1.ChartAreas.Add("ChartArea1");

                    Chart1.DataBindCrossTable(DataSource.AsEnumerable(), SeriesAgrupadasPor, EjeX, EjeY, null);
                    Chart1.AlignDataPointsByAxisLabel();

                    foreach (Series serie in Chart1.Series)
                    {
                        serie.ChartType = SeriesChartType.StackedColumn;
                        if (Navegacion != string.Empty || Navegacion != "" || Navegacion.Length > 0)
                        {
                            serie.Url = Page.Request.Url.LocalPath +
                                        "?" + Navegacion + HttpUtility.HtmlEncode("=#VALX") +
                                        "&fecIni=" + Session["FechaInicio"].ToString() +
                                        "&fecFin=" + Session["FechaFin"].ToString();
                        }
                        serie.IsXValueIndexed = false;
                    }

                    Chart1.AlternateText = "no se encontraron datos para las fechas señaladas";

                    if (formatoMiles) //NZ 20151106
                    {
                        Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "$ 0,";
                        tituloEjeY += lsLanImporteEnMiles;
                    }
                    else
                    {
                        Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "$ #,#.##";
                    }

                    if (TituloGrafica.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(TituloGrafica,
                                                    Docking.Top,
                                                    new Font("Verdana", 12f, FontStyle.Bold),
                                                    Color.Black));
                    }

                    // Set X axis margin for the area chart
                    Chart1.ChartAreas["ChartArea1"].BackColor = Color.White;
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisX.Name = EjeX;
                    Chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;

                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
                    Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    if (tituloEjeX.Length > 0)
                    {
                        //NZ 20151106                                           
                        Chart1.Titles.Add(new Title(tituloEjeX,
                                                    Docking.Bottom,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));

                    }
                    if (tituloEjeY.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(tituloEjeY,
                                                    Docking.Left,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));
                    }

                    Chart1.Height = Unit.Pixel(400);
                    Chart1.Width = Unit.Pixel(610);

                    // Show as 2D or 3
                    //Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = checkBoxShow3D.Checked;

                    Chart1.Legends.Add("Legend");
                    Chart1.Legends[0].IsTextAutoFit = true;
                    Chart1.Legends[0].TextWrapThreshold = 10;
                    Chart1.Legends[0].Docking = Docking.Bottom;
                    Chart1.Legends[0].Alignment = StringAlignment.Center;

                    //PT.20140213 : Se agregó un id para encontar la grafica al momentot de exportar
                    Chart1.ID = chartid;

                    return Chart1;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaStack() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si DataSource no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        protected Control creaGraficaPie(DataTable DataSource, string EjeX, string EjeY, string TituloGrafica, string tituloEjeX, string tituloEjeY)
        {
            #region Proceso de creacion de la grafica
            if (DataSource.Rows.Count > 0)
            {
                try
                {
                    Chart Chart1 = new Chart();
                    Chart1.ChartAreas.Add("ChartArea1");
                    Chart1.DataSource = DataSource;

                    Chart1.Series.Add(EjeX);
                    Chart1.Series[EjeX].ChartType = SeriesChartType.Pie;
                    Chart1.Series[EjeX].Label = "#PERCENT";
                    Chart1.Series[EjeX].LegendText = "#VALX";
                    Chart1.Series[EjeX].Url = Session["HomePage"].ToString() + "?" + Chart1.Series[EjeX].Name.ToString().Replace(" ", "").Trim();

                    Chart1.Series[EjeX].XValueMember = EjeX;
                    Chart1.Series[EjeX].YValueMembers = EjeY;
                    Chart1.Series[EjeX].IsVisibleInLegend = true;
                    Chart1.Series[EjeX].IsValueShownAsLabel = true;

                    Chart1.DataBind();
                    Chart1.DataManipulator.GroupByAxisLabel("SUM", EjeX);

                    if (TituloGrafica.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(TituloGrafica,
                                                    Docking.Top,
                                                    new Font("Verdana", 12f, FontStyle.Bold),
                                                    Color.Black));
                    }

                    // Set X axis margin for the area chart
                    Chart1.ChartAreas["ChartArea1"].BackColor = Color.White;
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisX.Name = EjeX;
                    Chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;

                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;

                    Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    if (tituloEjeX.Length > 0)
                    {
                        //NZ 20151106                                           
                        Chart1.Titles.Add(new Title(tituloEjeX,
                                                    Docking.Bottom,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));
                    }
                    if (tituloEjeY.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(tituloEjeY,
                                                    Docking.Left,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));
                    }

                    Chart1.Height = Unit.Pixel(400);
                    Chart1.Width = Unit.Pixel(610);

                    Chart1.Legends.Add("Legend");
                    Chart1.Legends[0].MaximumAutoSize = 20;
                    Chart1.Legends[0].IsTextAutoFit = true;
                    Chart1.Legends[0].TextWrapThreshold = 10;
                    Chart1.Legends[0].Docking = Docking.Bottom;
                    Chart1.Legends[0].Alignment = StringAlignment.Center;

                    return Chart1;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaPie() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si DataSource no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        protected Control creaGraficaBarras(DataTable DataSource, string EjeX, string EjeY, string TituloGrafica, string Navegacion, string tituloEjeX, string tituloEjeY, bool formatoMiles)
        {
            #region Elimina columnas no necesarias en el gridview
            if (DataSource.Columns.Contains("RID"))
                DataSource.Columns.Remove("RID");
            if (DataSource.Columns.Contains("RowNumber"))
                DataSource.Columns.Remove("RowNumber");
            if (DataSource.Columns.Contains("TopRID"))
                DataSource.Columns.Remove("TopRID");
            #endregion // Elimina columnas no necesarias en el gridview

            #region Proceso de creacion de la grafica
            if (DataSource.Rows.Count > 0)
            {
                try
                {
                    Chart Chart1 = new Chart();
                    Chart1.ChartAreas.Add("ChartArea1");
                    Chart1.DataSource = DataSource;

                    Chart1.Series.Add(EjeX);
                    Chart1.Series[EjeX].ChartType = SeriesChartType.Column;
                    string url = Page.Request.Url.PathAndQuery;

                    if (url.Contains("&fecIni=") && nav == 0)
                    {
                        url = System.Text.RegularExpressions.Regex.Replace(url, "&fecIni=.{10}&fecFin=.{10}",
                                                                            "&fecIni=" + Session["FechaInicio"].ToString() +
                                                                            "&fecFin=" + Session["FechaFin"].ToString());
                    }
                    if (nav > 0)
                    {
                        if (url.Contains("&CenCos="))
                        {
                            int comienzaQSCenCos = url.IndexOf("&CenCos=");
                            url = url.Substring(0, comienzaQSCenCos);
                        }

                        if (url.Contains("&CenCosPad="))
                        {
                            url = System.Text.RegularExpressions.Regex.Replace(url, "&CenCosPad=[0-9]*", "");
                        }
                    }

                    Chart1.Series[EjeX].Url = url + "&" + Navegacion + "=#VALX";

                    Chart1.Series[EjeX].XValueMember = EjeX;
                    Chart1.Series[EjeX].YValueMembers = EjeY;

                    Chart1.DataBind();
                    Chart1.AlternateText = "no se encontraron datos para las fechas señaladas";

                    if (formatoMiles) //NZ 20151106
                    {
                        Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "$ 0,";
                        tituloEjeY += lsLanImporteEnMiles;
                    }
                    else
                    {
                        Chart1.ChartAreas[0].AxisY.LabelStyle.Format = "$ #,#.##";
                    }

                    if (TituloGrafica.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(TituloGrafica,
                                                   Docking.Top,
                                                   new Font("Verdana", 12f, FontStyle.Bold),
                                                   Color.Black
                                                    ));
                    }

                    // Set X axis margin for the area chart
                    Chart1.ChartAreas["ChartArea1"].BackColor = Color.White;
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisX.Name = EjeX;
                    Chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = false;

                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    Chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
                    Chart1.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = false;

                    Chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
                    Chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                    if (tituloEjeX.Length > 0)
                    {
                        //NZ 20151106                                           
                        Chart1.Titles.Add(new Title(tituloEjeX,
                                                    Docking.Bottom,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));
                    }
                    if (tituloEjeY.Length > 0)
                    {
                        //NZ 20151106
                        Chart1.Titles.Add(new Title(tituloEjeY,
                                                    Docking.Left,
                                                    new Font("Verdana", 10f, FontStyle.Bold),
                                                    Color.Black));
                    }

                    Chart1.Legends.Add("Legend");
                    Chart1.Legends[0].IsTextAutoFit = true;
                    Chart1.Legends[0].TextWrapThreshold = 10;
                    Chart1.Legends[0].Docking = Docking.Bottom;
                    Chart1.Legends[0].Alignment = StringAlignment.Center;

                    Chart1.Height = Unit.Pixel(400);
                    Chart1.Width = Unit.Pixel(610);
                    Chart1.CssClass = "ColumnChart_ChartAreaAxisXTitleFont";
                    // Show as 2D or 3
                    //Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = checkBoxShow3D.Checked;

                    return Chart1;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaBarras() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        protected Control creaGraficaHistorica(DataTable localDT, string lsTituloReporte, string EjeX, string EjeY, string tituloEjeX, string tituloEjeY, bool formatoMiles)
        {
            #region Proceso de creacion de grafica
            if (localDT.Rows.Count > 0)
            {
                try
                {
                    Chart chart = new Chart();
                    chart.ChartAreas.Add("ChartArea1");
                    chart.DataSource = localDT;

                    chart.Series.Add(EjeX);
                    chart.Series[EjeX].ChartType = SeriesChartType.Line;
                    chart.Series[EjeX].Url = Session["HomePage"].ToString() + "?" + chart.Series[EjeX].Name.ToString().Replace(" ", "").Trim();

                    chart.Series[EjeX].XValueMember = EjeX;
                    chart.Series[EjeX].YValueMembers = EjeY;

                    chart.DataBind();
                    chart.DataManipulator.GroupByAxisLabel("SUM", EjeX);

                    chart.Legends.Add("Legends");
                    chart.Legends["Legends"].LegendStyle = LegendStyle.Column;

                    chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                    chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                    chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;

                    chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(214, 211, 214);
                    chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
                    chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;

                    if (formatoMiles) //NZ 20151106
                    {
                        chart.ChartAreas[0].AxisY.LabelStyle.Format = "$ 0,";
                        tituloEjeY += lsLanImporteEnMiles;
                    }
                    else
                    {
                        chart.ChartAreas[0].AxisY.LabelStyle.Format = "$ #,#.##";
                    }

                    if (lsTituloReporte.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        lsTituloReporte,
                                                        Docking.Top,
                                                        new Font("Verdana", 12f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    if (tituloEjeX.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        EjeX,
                                                        Docking.Bottom,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }
                    if (tituloEjeY.Length > 0)
                    {
                        chart.Titles.Add(new Title(
                                                        EjeY,
                                                        Docking.Left,
                                                        new Font("Verdana", 10f, FontStyle.Bold),
                                                        Color.Black
                                                     ));
                    }

                    chart.Legends.Add("Legend");
                    chart.Legends[0].IsTextAutoFit = true;
                    chart.Legends[0].TextWrapThreshold = 10;
                    chart.Legends[0].Docking = Docking.Bottom;
                    chart.Legends[0].Alignment = StringAlignment.Center;

                    chart.Height = 400;
                    chart.Width = 610;

                    return chart;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGraficaHistorica() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }
        
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
        
        protected Control creaGridView(DataTable localDT, bool agregaTotales, int height)
        {
            Panel lpnl = new Panel();
            if (height > 0)
            {
                lpnl.ScrollBars = ScrollBars.Vertical;
                lpnl.Height = height;
            }
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            TableCell ltc = new TableCell();
            ltc.Width = Unit.Percentage(100);

            Panel lpnl1 = new Panel();
            lpnl1.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";
            lpnl1.Height = 10;
            lpnl1.Width = Unit.Percentage(100);

            Panel lpnl2 = new Panel();
            lpnl2.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-tr ui-helper-clearfix";
            lpnl2.Height = 10;
            lpnl2.Width = Unit.Percentage(100);

            if (height > 0)
            {
                ltbl.Height = height;
            }
            ltbl.HorizontalAlign = HorizontalAlign.Center;

            #region Proceso de creacion del gridview
            if (localDT.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (localDT.Columns.Contains("RID"))
                        localDT.Columns.Remove("RID");
                    if (localDT.Columns.Contains("RowNumber"))
                        localDT.Columns.Remove("RowNumber");
                    if (localDT.Columns.Contains("TopRID"))
                        localDT.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.CssClass = "DSOGrid";
                    lgrv.Height = Unit.Percentage(100);
                    lgrv.Width = Unit.Percentage(100);

                    lgrv.RowStyle.CssClass = "grvitemStyle";
                    lgrv.HeaderStyle.CssClass = "titulosReportes";
                    lgrv.AlternatingRowStyle.CssClass = "grvalternateItemStyle";

                    if (agregaTotales == true)
                    {
                        #region Agrega una linea al DataTable para calcular los totales

                        DataRow dr = localDT.NewRow();

                        dr[0] = lsLanTotales;

                        for (int ent = 1; ent < localDT.Columns.Count; ent++)
                        {
                            dr[localDT.Columns[ent].ColumnName] = localDT.Compute("Sum([" + localDT.Columns[ent].ColumnName + "])", "");
                        }

                        localDT.Rows.Add(dr);
                        localDT.AcceptChanges();

                        #endregion // Agrega una linea al DataTable para calcular los totales

                        #region Agrega un BoundField por cada columna del datarow

                        int i = 0;
                        foreach (DataColumn dc in localDT.Columns)
                        {
                            BoundField bf = new BoundField();
                            bf.DataField = dc.ColumnName;
                            bf.HeaderText = dc.ColumnName;
                            if (i >= 1)
                            {
                                bf.DataFormatString = "{0:c}";
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            }
                            else
                            {
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            }

                            lgrv.Columns.Add(bf);

                            i++;
                        }

                        #endregion // Agrega un BoundField por cada columna del datarow

                        lgrv.AutoGenerateColumns = false;
                    }
                    lgrv.DataSource = localDT;
                    lgrv.DataBind();

                    if (agregaTotales == true)
                    {
                        // Le da formato a la ultima fila del gridview para darle estilo keytia
                        lgrv.Rows[localDT.Rows.Count - 1].CssClass = "titulosReportes";
                    }
                    //PT: Agrega un id al gridview
                    lgrv.ID = "ReporteGrid";
                    ltc.Controls.Add(lpnl1);
                    ltc.Controls.Add(lgrv);
                    ltc.Controls.Add(lpnl2);
                    ltr.Controls.Add(ltc);
                    ltbl.Controls.Add(ltr);


                    lpnl.Controls.Add(ltbl);

                    return lpnl;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

                    return lblConMensaje("Ocurrio un error al generar la grafica");
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        //NZ 20151105
        protected Control creaGridView(DataTable localDT, bool agregaTotales, int height, int[] columnasNoVisibles, string[] formatStringColumnas, int columPalabraTotal)
        {
            Panel lpnl = new Panel();
            if (height > 0)
            {
                lpnl.ScrollBars = ScrollBars.Vertical;
                lpnl.Height = height;
            }
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            TableCell ltc = new TableCell();
            ltc.Width = Unit.Percentage(100);

            Panel lpnl1 = new Panel();
            lpnl1.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";
            lpnl1.Height = 10;
            lpnl1.Width = Unit.Percentage(100);

            Panel lpnl2 = new Panel();
            lpnl2.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-tr ui-helper-clearfix";
            lpnl2.Height = 10;
            lpnl2.Width = Unit.Percentage(100);

            if (height > 0)
            {
                ltbl.Height = height;
            }
            ltbl.HorizontalAlign = HorizontalAlign.Center;

            #region Proceso de creacion del gridview
            if (localDT.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (localDT.Columns.Contains("RID"))
                        localDT.Columns.Remove("RID");
                    if (localDT.Columns.Contains("RowNumber"))
                        localDT.Columns.Remove("RowNumber");
                    if (localDT.Columns.Contains("TopRID"))
                        localDT.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.CssClass = "DSOGrid";
                    lgrv.Height = Unit.Percentage(100);
                    lgrv.Width = Unit.Percentage(100);

                    lgrv.RowStyle.CssClass = "grvitemStyle";
                    lgrv.HeaderStyle.CssClass = "titulosReportes";
                    lgrv.AlternatingRowStyle.CssClass = "grvalternateItemStyle";

                    if (agregaTotales == true)
                    {
                        #region Agrega una linea al DataTable para calcular los totales

                        DataRow dr = localDT.NewRow();

                        dr[columPalabraTotal] = lsLanTotales;

                        for (int ent = 0; ent < localDT.Columns.Count; ent++)
                        {
                            if (localDT.Columns[ent].DataType != System.Type.GetType("System.String") && !columnasNoVisibles.Contains(ent))
                            {
                                dr[localDT.Columns[ent].ColumnName] = localDT.Compute("Sum([" + localDT.Columns[ent].ColumnName + "])", "");
                            }
                            else
                            {
                                if (localDT.Columns[ent].DataType != System.Type.GetType("System.String"))
                                {
                                    dr[ent] = 0;
                                }
                                else if (ent != columPalabraTotal)
                                {
                                    dr[ent] = "";
                                }
                            }
                        }

                        localDT.Rows.Add(dr);
                        localDT.AcceptChanges();

                        #endregion // Agrega una linea al DataTable para calcular los totales

                        #region Agrega un BoundField por cada columna del datarow

                        int i = 0;
                        foreach (DataColumn dc in localDT.Columns)
                        {
                            BoundField bf = new BoundField();
                            bf.DataField = dc.ColumnName;
                            bf.HeaderText = dc.ColumnName;

                            //NZ 20151105
                            if (columnasNoVisibles.Contains(i))
                            {
                                bf.DataField = localDT.Columns[i].ColumnName;
                                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                                bf.Visible = false;
                            }
                            else
                            {
                                bf.DataFormatString = formatStringColumnas[i];
                                if (dc.DataType.FullName == "System.String")
                                {
                                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                                }
                                else
                                {
                                    bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                                }
                            }

                            lgrv.Columns.Add(bf);

                            i++;
                        }
                        #endregion // Agrega un BoundField por cada columna del datarow

                        lgrv.AutoGenerateColumns = false;
                    }
                    lgrv.DataSource = localDT;
                    lgrv.DataBind();

                    if (agregaTotales == true)
                    {
                        // Le da formato a la ultima fila del gridview para darle estilo keytia
                        lgrv.Rows[localDT.Rows.Count - 1].CssClass = "titulosReportes";
                    }
                    //PT: Agrega un id al gridview
                    lgrv.ID = "ReporteGrid";
                    ltc.Controls.Add(lpnl1);
                    ltc.Controls.Add(lgrv);
                    ltc.Controls.Add(lpnl2);
                    ltr.Controls.Add(ltc);
                    ltbl.Controls.Add(ltr);


                    lpnl.Controls.Add(ltbl);

                    return lpnl;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

                    return lblConMensaje("Ocurrio un error al generar la grafica");
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        protected Control creaGridView(DataTable localDT, string NavegacionUrl, string columnaICodNavegacion,
            string headerTextNav, string nombreDeColumnaAMostrar, string[] urlFields, int height)
        {
            Panel lpnl = new Panel();
            if (height > 0)
            {
                lpnl.ScrollBars = ScrollBars.Vertical;
                lpnl.Height = height;
            }
            Table ltbl = new Table();
            ltbl.Width = Unit.Percentage(100);
            TableRow ltr = new TableRow();
            ltr.Width = Unit.Percentage(100);
            TableCell ltc = new TableCell();
            ltc.Width = Unit.Percentage(100);

            Panel lpnl1 = new Panel();
            lpnl1.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix";
            lpnl1.Height = 10;
            lpnl1.Width = Unit.Percentage(100);

            Panel lpnl2 = new Panel();
            lpnl2.CssClass = "fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix";
            lpnl2.Height = 10;
            lpnl2.Width = Unit.Percentage(100);

            if (height > 0)
            {
                ltbl.Height = height;
            }
            ltbl.HorizontalAlign = HorizontalAlign.Center;

            #region Proceso de creacion del gridview
            if (localDT.Rows.Count > 0)
            {
                try
                {
                    #region Elimina columnas no necesarias en el gridview
                    if (localDT.Columns.Contains("RID"))
                        localDT.Columns.Remove("RID");
                    if (localDT.Columns.Contains("RowNumber"))
                        localDT.Columns.Remove("RowNumber");
                    if (localDT.Columns.Contains("TopRID"))
                        localDT.Columns.Remove("TopRID");
                    #endregion // Elimina columnas no necesarias en el gridview

                    GridView lgrv = new GridView();
                    lgrv.CssClass = "DSOGrid";
                    lgrv.Height = Unit.Percentage(100);
                    lgrv.Width = Unit.Percentage(100);

                    lgrv.RowStyle.CssClass = "grvitemStyle";
                    lgrv.HeaderStyle.CssClass = "titulosReportes";
                    lgrv.AlternatingRowStyle.CssClass = "grvalternateItemStyle";

                    #region Agrega una linea al DataTable para calcular los totales

                    DataRow dr = localDT.NewRow();

                    dr[0] = 0;
                    dr[1] = lsLanTotales;

                    for (int ent = 2; ent < localDT.Columns.Count; ent++)
                    {
                        dr[localDT.Columns[ent].ColumnName] = localDT.Compute("Sum([" + localDT.Columns[ent].ColumnName + "])", "");
                    }

                    localDT.Rows.Add(dr);
                    localDT.AcceptChanges();

                    #endregion // Agrega una linea al DataTable para calcular los totales

                    #region Agrega un tipo de columna por cada columna del datarow

                    string url = Page.Request.Url.PathAndQuery;
                    if (NavegacionUrl.Contains("&CenCosPad") && NavegacionUrl.Contains("&CenCos") &&
                        NavegacionUrl.Contains("&fecIni") && NavegacionUrl.Contains("&fecFin"))
                    {
                        url = Page.Request.Url.AbsolutePath;
                    }

                    int i = 0;
                    foreach (DataColumn dc in localDT.Columns)
                    {
                        BoundField bf = new BoundField();
                        bf.DataField = dc.ColumnName;
                        bf.HeaderText = dc.ColumnName;

                        HyperLinkField hlf = new HyperLinkField();
                        hlf.HeaderText = dc.ColumnName;

                        if (i >= 2)
                        {
                            bf.DataFormatString = "{0:c}";
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                            lgrv.Columns.Add(bf);
                        }
                        else if (i == 0)  // Columna de iCodCatalogo
                        {
                            bf.DataField = columnaICodNavegacion;
                            bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            bf.Visible = false;
                            lgrv.Columns.Add(bf);
                        }
                        else
                        {
                            hlf.HeaderText = headerTextNav;
                            hlf.DataNavigateUrlFields = urlFields;
                            hlf.DataNavigateUrlFormatString = url + NavegacionUrl;
                            hlf.DataTextField = nombreDeColumnaAMostrar;
                            hlf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            hlf.ControlStyle.Font.Bold = true;
                            lgrv.Columns.Add(hlf);
                        }

                        i++;
                    }

                    #endregion // Agrega un BoundField por cada columna del datarow

                    lgrv.AutoGenerateColumns = false;
                    lgrv.DataSource = localDT;
                    lgrv.DataBind();

                    // Le da formato a la ultima fila del gridview para darle estilo keytia
                    lgrv.Rows[localDT.Rows.Count - 1].CssClass = "titulosReportes";

                    //PT: Agrega un id al gridview
                    lgrv.ID = "ReporteGrid";


                    ltc.Controls.Add(lpnl1);
                    ltc.Controls.Add(lgrv);
                    ltc.Controls.Add(lpnl2);
                    ltr.Controls.Add(ltc);
                    ltbl.Controls.Add(ltr);

                    lpnl.Controls.Add(ltbl);

                    return lpnl;
                }

                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error en metodo creaGridView() en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

                    return lblConMensaje("Ocurrio un error al generar la grafica");
                }
            }
            #endregion

            #region Si localDT no contiene datos
            else
            {
                return lblReporteSinDatos();
            }
            #endregion
        }

        /// <summary>
        /// Agrega un titulo con estilo de los reportes de dashboard de keytia y un pequeño borde para seccionar cada reporte.
        /// </summary>
        /// <param name="control">Control al que se desea agregar el titulo y borde</param>
        /// <param name="titulo">Titulo que se mostrara al usuario</param>
        /// <param name="celdasOcupadas">En cuantas celdas se muestra el reporte</param>
        /// <returns></returns>
        protected Control agregaTituloYBordesDeReporte(Control control, string titulo, int celdasOcupadas)
        {
            Panel panel = new Panel();
            //panel.BorderColor = Color.FromArgb(198, 219, 239);
            //panel.BorderWidth = 1;
            //panel.HorizontalAlign = HorizontalAlign.Center;
            panel.CssClass = "PanelTitulosYBordeReportes";

            if (celdasOcupadas == 1)
            {
                panel.Height = 450;
            }

            Panel header = new Panel();
            header.CssClass = "titulosReportes";
            header.HorizontalAlign = HorizontalAlign.Left;

            Label lLabel = new Label();
            lLabel.Text = GetGlobalResourceObject(lsidioma, titulo).ToString();
            header.Controls.Add(lLabel);

            panel.Controls.Add(header);
            panel.Controls.Add(control);

            return panel;
        }

        protected static Control lblReporteSinDatos()
        {
            Panel lpanel = new Panel();
            lpanel.HorizontalAlign = HorizontalAlign.Center;

            Label reporteSinDatos = new Label();
            reporteSinDatos.Text = "No existen datos de este reporte para el rango de fechas seleccionadas";

            lpanel.Controls.Add(reporteSinDatos);

            return lpanel;
        }

        protected static Control lblConMensaje(string mensaje)
        {
            Panel lpanel = new Panel();
            lpanel.HorizontalAlign = HorizontalAlign.Center;

            Label reporteSinDatos = new Label();
            reporteSinDatos.Text = mensaje;

            lpanel.Controls.Add(reporteSinDatos);

            return lpanel;
        }

 
    }
}
