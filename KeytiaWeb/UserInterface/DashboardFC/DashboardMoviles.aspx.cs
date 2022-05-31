using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Collections;
using KeytiaServiceBL.Reportes;
using AjaxControlToolkit;
using DSOControls2008;
using System.Web.Services;
using System.Globalization;
using KeytiaWeb.UserInterface.Indicadores;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardMoviles : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        #region Variables de la pagina

        static string TituloNavegacion = string.Empty;
        string isFT = string.Empty;
        string WhereAdicional = string.Empty;

        string lsCvePerfil = string.Empty;

        //Se almacenan los parametros que llegan en el QueryString
        static Dictionary<string, string> param = new Dictionary<string, string>();
        #endregion

        #region Variables que guardan valores de queryString



        #endregion

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Modificaciones de estilo
            // Se extiende el espacio que ocupan los reportes si se encuentra un parametro en especifico para la URL
            // Actualmente estas modificaciones se usan en los reportes:
            //      Indicador Lineas Inactivas desde Dashboard Moviles
            //      Reporte de Lineas por Plan base desde menu
            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("LineasInactivas") || Request.QueryString["Nav"] == "PlanBaseLinea")
            {
                Rep1.CssClass = Rep1.CssClass.Replace("col-md-6", "col-md-12");
                Rep2.CssClass = Rep2.CssClass.Replace("col-md-6", "col-md-12");
            }
            else
            {
                Rep1.CssClass = Rep1.CssClass.Replace("col-md-12", "col-md-6");
                Rep2.CssClass = Rep2.CssClass.Replace("col-md-12", "col-md-6");
            }
            #endregion

            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();
                (Master as KeytiaOH).ChangeCalendar(false);   //Hace que la pagina tenga un solo calendario.

                LeeQueryString();

                if (!Page.IsPostBack && (ValidaConsultaFechasBD() || (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")))
                {
                    if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                    {
                        #region Inicia los valores default de los controles de fecha
                        try
                        {
                            CalculaFechasDeDashboard();
                        }
                        catch (Exception ex)
                        {
                            throw new KeytiaWebException(
                                "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                                + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                        }
                        #endregion Inicia los valores default de los controles de fecha
                    }
                }
                else
                {
                    //En este caso que es Moviles, aquí se ven meses completos, así que si se selecciona una fecha 
                    //diferente el primer mes, se mueven internamente las fechas para que siempre sea mes completo.
                    #region Fecha mes completo
                    if (Session["FechaInicio"] != null && Session["FechaInicio"] != "")
                    {
                        DateTime fechaAux = Convert.ToDateTime(Session["FechaInicio"].ToString());

                        fechaInicio = new DateTime(fechaAux.Year, fechaAux.Month, 1);
                        fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);

                        Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                        Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                    }
                    #endregion
                }

                lsCvePerfil = DSODataAccess.ExecuteScalar(ConsultaDescripcionDePerfil()).ToString();
                pnlCantMaxReg.Visible = false;

                #region Dashboard
                if (!string.IsNullOrEmpty(txtConcepto.Text) || !string.IsNullOrEmpty(txtConceptoDescripcion.Text))
                {
                    MostrarTablaConceptos();
                }
                else
                {
                    txtConcepto.Text = null;
                    txtConceptoDescripcion.Text = null;

                    if (param["Nav"] == string.Empty || param["Nav"] == "DashboardConsumoInternet"|| param["Nav"] == "DashboardUsoInternet")
                    {
                        /* Se evalua que dashboard debe ver el usuario dependiendo el iCodCatalogo de su perfil 370 ---> Empleado */
                        if (Session["iCodPerfil"].ToString() == "370")
                        {
                            ElegirDashboardMiConsumo();
                        }
                        else
                        {
                            if (param["MiConsumo"] != string.Empty)
                            {
                                param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                                if (param["Emple"] != "-1")
                                {
                                    ElegirDashboardMiConsumo();
                                    ReporteConsumoInternetPorColaborador(Rep6);
                                }
                                else
                                {
                                    Label sinEmpleadoAsignado = new Label();
                                    sinEmpleadoAsignado.Text = "Este usuario no cuenta con un empleado asignado.";
                                    Rep0.Controls.Add(sinEmpleadoAsignado);
                                }
                            }
                            else { DashboardMultiPerfil(); }
                        }
                    }
                    else { Navegaciones(); }
                }
                #endregion Dashboard

                #region Control Ayuda ALFA //NZ 20150324
                MostrarControlAyudaALFA();
                #endregion

                //NZ: 20150324
                if (param["Nav"] == "ConsultaConcepto" && ValidarPerfilParaAutocompleteConceptos())
                {
                    pnlBusqConcepto.Visible = true;
                }

                ConfiguraNavegacion(); //NZ Siempre debe ejecutarce al final
                if (string.IsNullOrEmpty(param["Nav"]))  //Solo en el primer nivel se deben mostrar los indicadores.
                {
                    pnlBusqConcepto.Visible = ValidarPerfilParaAutocompleteConceptos();
                    BuildIndicadores.ConstruirIndicadores(ref pnlIndicadores, "DashboardMoviles.aspx", Request.Path);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void ElegirDashboardMiConsumo()
        {
            switch (param["MiConsumo"])
            {
                case "TELCEL":
                    DashboardEmpleado();
                    break;
                case "NEXTEL":
                    Label lbl = new Label();
                    lbl.Text = "El dashboard con reportes de AT&T no esta disponible."; //NZ 20151012 Se cambia "nextel" por "AT&T".
                    Rep0.Controls.Add(lbl);
                    break;
                default:
                    DashboardEmpleado();
                    break;
            }
        }

        private void CalculaFechasDeDashboard()
        {
            DateTime ldtFechaInicio = new DateTime();
            string lsfechaInicio = DSODataAccess.ExecuteScalar(ConsultaFechaMaximaDeDetallFactCDR()).ToString();
            if (DateTime.TryParse(lsfechaInicio, out ldtFechaInicio))
            {
                //NZ 20150319 Se establecio que siempre se mostrara el primer dia de cada mes.
                fechaInicio = new DateTime(ldtFechaInicio.Year, ldtFechaInicio.Month, 1);
                fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
        }

        private static void RemoveColHerencia(ref DataTable dt)
        {
            #region Elimina columnas no necesarias
            if (dt.Columns.Contains("RID"))
                dt.Columns.Remove("RID");
            if (dt.Columns.Contains("RowNumber"))
                dt.Columns.Remove("RowNumber");
            if (dt.Columns.Contains("TopRID"))
                dt.Columns.Remove("TopRID");
            #endregion
        }

        #region Logica de botones de la pagina

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

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS();
        }
        protected void btnExportXLS_Click(object sender, EventArgs e)
        {
            param["Nav"] = "RepUsoInternetTabs";
            LinkButton clic = (LinkButton)sender;
            param["TipoConsumo"]  = clic.ID.Replace("btnConsumoPor","");
            ExportXLS();
        }

        #endregion



        public void DashboardEmpleado()
        {
            #region Reporte historico

            RepTabHistAnioAnteriorVsActualMovPeEmple(Rep1, "Consumo histórico");

            #endregion Reporte historico

            #region Reporte Detalle factura telcel

            RepPorConceptoEmple(Rep2, "Costo por concepto (Monto Excedido)");

            #endregion Reporte Detalle factura telcel

            #region Reporte Llamadas mas largas

            DataTable RepLlamMasLarg = DSODataAccess.Execute(ConsultaLlamadasMasLargas());
            if (RepLlamMasLarg.Rows.Count > 0)
            {
                DataView dvRepLlamMasLarg = new DataView(RepLlamMasLarg);
                RepLlamMasLarg = dvRepLlamMasLarg.ToTable(false,
                    new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos",
                                        "Costo", "Dir Llamada", "Punta A", "Punta B" });
                RepLlamMasLarg.Columns["Extension"].ColumnName = "Línea";
                RepLlamMasLarg.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepLlamMasLarg.Columns["Numero Marcado"].ColumnName = "Número marcado";
                RepLlamMasLarg.Columns["Fecha Llamada"].ColumnName = "Fecha";
                RepLlamMasLarg.Columns["Hora Llamada"].ColumnName = "Hora";
                RepLlamMasLarg.Columns["Duracion Minutos"].ColumnName = "Minutos";
                RepLlamMasLarg.Columns["Costo"].ColumnName = "Total";
                RepLlamMasLarg.Columns["Dir Llamada"].ColumnName = "Tipo";
                RepLlamMasLarg.Columns["Punta A"].ColumnName = "Localidad Origen";
                RepLlamMasLarg.Columns["Punta B"].ColumnName = "Localidad Destino";
                RepLlamMasLarg.AcceptChanges();
            }

            Rep6.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamMasLarg_T", RepLlamMasLarg,
                                true, "Totales", new string[] { "", "", "", "", "", "", "{0:c}", "", "", "" }), "RepLlamMasLarg_T", "Llamadas más largas ", Request.Path + "?Nav=LlamMasLargN1")
                );

            #endregion Reporte Llamadas mas largas

            #region Reporte Llamadas mas costosas

            DataTable RepLlamMasCostosas = DSODataAccess.Execute(ConsultaLlamadasMasCostosas());
            if (RepLlamMasCostosas.Rows.Count > 0)
            {
                DataView dvRepLlamMasCostosas = new DataView(RepLlamMasCostosas);
                RepLlamMasCostosas = dvRepLlamMasCostosas.ToTable(false,
                    new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos",
                                        "Costo", "Dir Llamada", "Punta A", "Punta B" });
                RepLlamMasCostosas.Columns["Extension"].ColumnName = "Línea";
                RepLlamMasCostosas.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepLlamMasCostosas.Columns["Numero Marcado"].ColumnName = "Número marcado";
                RepLlamMasCostosas.Columns["Fecha Llamada"].ColumnName = "Fecha";
                RepLlamMasCostosas.Columns["Hora Llamada"].ColumnName = "Hora";
                RepLlamMasCostosas.Columns["Duracion Minutos"].ColumnName = "Minutos";
                RepLlamMasCostosas.Columns["Costo"].ColumnName = "Total";
                RepLlamMasCostosas.Columns["Dir Llamada"].ColumnName = "Tipo";
                RepLlamMasCostosas.Columns["Punta A"].ColumnName = "Localidad Origen";
                RepLlamMasCostosas.Columns["Punta B"].ColumnName = "Localidad Destino";
                RepLlamMasCostosas.AcceptChanges();
            }

            Rep3.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamMasCostosas_T", RepLlamMasCostosas,
                                true, "Totales", new string[] { "", "", "", "", "", "", "{0:c}", "", "", "" }), "RepLlamMasCostosas_T", "Llamadas más costosas ", Request.Path + "?Nav=LlamMasCostN1")
                );

            #endregion Reporte Llamadas mas costosas

            #region Reporte Llamadas a celulares de red

            DataTable RepLlamACeluRed = DSODataAccess.Execute(ConsultaLlamACelularesDeLaRed(""));
            if (RepLlamACeluRed.Rows.Count > 0)
            {
                DataView dvRepLlamACeluRed = new DataView(RepLlamACeluRed);
                RepLlamACeluRed = dvRepLlamACeluRed.ToTable(false,
                    new string[] { "Codigo Exten", "Extension", "Nombre Completo", "Total", "Duracion", "Numero" });
                RepLlamACeluRed.Columns["Extension"].ColumnName = "Línea";
                RepLlamACeluRed.Columns["Nombre Completo"].ColumnName = "Responsable";
                RepLlamACeluRed.Columns["Total"].ColumnName = "Total";
                RepLlamACeluRed.Columns["Duracion"].ColumnName = "Minutos";
                RepLlamACeluRed.Columns["Numero"].ColumnName = "Llamadas";
                RepLlamACeluRed.AcceptChanges();
            }

            Rep5.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamACeluRed_T", DTIChartsAndControls.ordenaTabla(RepLlamACeluRed, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "", "" }, Request.Path + "?Nav=LlamACelRedN2&Exten={0}",
                                new string[] { "Codigo Exten" }, 1, new int[] { 0 },
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepLlamACeluRed_T", "Llamadas a celulares de la red", Request.Path + "?Nav=LlamACelRedN1")
                );
            #endregion Reporte Llamadas a celulares de red

            #region Reporte Minutos Utilizados  //BG.20150918

            DataTable RepMinsUtilizados = DSODataAccess.Execute(ConsultaRepMinutosUtilizados());
            if (RepMinsUtilizados.Rows.Count > 0)
            {
                DataView dvRepMinsUtilizados = new DataView(RepMinsUtilizados);
                RepMinsUtilizados = dvRepMinsUtilizados.ToTable(false,
                    new string[] { "Linea", "MinsDisp", "MinsUtil" });
                RepMinsUtilizados.Columns["Linea"].ColumnName = "Línea";
                RepMinsUtilizados.Columns["MinsDisp"].ColumnName = "Minutos Disponibles";
                RepMinsUtilizados.Columns["MinsUtil"].ColumnName = "Minutos Utilizados";
                RepMinsUtilizados.AcceptChanges();
            }

            Rep4.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMinsUtilizados_T", RepMinsUtilizados,
                                false, "", new string[] { "", "", "" }), "RepMinsUtilizados_T", "Minutos Utilizados", Request.Path + "?Nav=MinsUtilizadosN1")
                );

            #endregion Reporte Minutos Utilizados
        }

        public void DashboardMultiPerfil()
        {
            DataTable reportes = new DataTable();
            reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("DashboardMoviles.aspx"));

            if (param["Nav"] == "DashboardConsumoInternet" ||
                HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("dashboardconsumointernet.aspx"))
            {
                pnlConsumos.Visible = true;
                string label = (Request.QueryString["TipoConsumo"] == "Nac") ?"Consumo Internet Nacional":"Consumo Internet Internacional";
                lblTipoConsumo.Text = label;
                reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("DashboardConsumoInternet.aspx"));
                param["Dash"] = "ConsumoInternet";
                param["Nav"] = string.Empty;
                TituloNavegacion = string.Empty;
            }
            if(param["Nav"] == "DashboardUsoInternet" ||
                HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("dashboardusointernet.aspx"))
            {
                string label = (Request.QueryString["TipoConsumo"] == "Nac") ? "Consumo Internet Nacional" : "Consumo Internet Internacional";
                reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil("DashboardUsoInternet.aspx"));
            }
            BuscaReportes(reportes);

        }





        protected void BuscaReportes(DataTable RelacionReportesContenedor)
        {
            foreach (DataRow dr in RelacionReportesContenedor.Rows)
            {
                try
                {
                    Panel contenedor = (Panel)pnlMainHolder.FindControl(dr["Contenedor"].ToString());
                    string reporte = dr["Reporte"].ToString();
                    string tipoGrafDefault = dr["tipoGrafDefault"].ToString();
                    string tituloGrid = dr["tituloGrid"].ToString();
                    string tituloGrafica = dr["tituloGrafica"].ToString();
                    string pestaniaActiva = dr["pestaniaActiva"].ToString();
                    CargaReporte(reporte, contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, Convert.ToInt32(pestaniaActiva));
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "No se pudo cargar el reporte ''" + dr["Reporte"].ToString() + "''" + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }

        protected void CargaReporte(string reporte, Control contenedor, string tipoGrafDefault,
            string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            string par = (!string.IsNullOrWhiteSpace(param["TipoConsumo"]) && param["TipoConsumo"].ToLower() == "nac") ? " Nacional" : " Internacional";

            switch (reporte)
            {
                case "RepTabHistMov1Pnl":
                    RepTabHistMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabPorCenCosMov1Pnl":
                    RepTabPorCenCosMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabPorEmpleMov1Pnl":
                    RepTabPorEmpleMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabResumenPorConceptoTelcelMov1Pnl":
                    RepTabResumenPorConceptoTelcelMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabResumenPorPlanesMov1Pnl":
                    RepTabResumenPorPlanesMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabTopLinMasCarasMov1Pnl":
                    RepTabTopLinMasCarasMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepMatPorCenCosMov1Pnl":
                    RepMatPorCenCosMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepMatPorEmpleMov1Pnl":
                    RepMatPorEmpleMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabLlamACeluDeLaRedMov1Pnl":
                    RepTabLlamACeluDeLaRedMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabResumenPorEquiposMov1Pnl":
                    RepTabResumenPorEquiposMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabHistAnioAnteriorVsActualMov1Pnl":
                    RepTabHistAnioAnteriorVsActualMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabHistAnioAntVsActConFiltroMov1Pnl":
                    RepTabHistAnioAnteriorVsActualConFiltroMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepGrafConsumoAcumPorDireccion":
                    RepGrafConsumoAcumPorDireccion(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabPorEmpleAcumMov1Pnl":
                    RepTabPorEmpleAcumMov1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepConsumoJerarquico1Pnl":
                    RepTabCenCosJerarquicoP1(contenedor, tituloGrid, tituloGrafica, pestaniaActiva);
                    break;
                case "RepTabPorCostoAreaDireccion1Pnl":
                    RepTabPorCostoAreaDireccion1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabConsumoMovilesPorTecnologia1Pnl":
                    RepTabConsumoMovilesPorTecnologia1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabConsumoPorCarrierFiltroCC1Pnl":
                    RepTabConsumoPorCarrierFiltroCC1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "RepTabHistMovCantLineas1Pnl":
                    RepTabHistMovCantLineas1Pnl(contenedor, tituloGrid, pestaniaActiva);
                    break;
                case "DemoConsumoPorConecepto2Pnls":
                    DemoConsumoPorConcepto2Pnls(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, pestaniaActiva);
                    break;
                case "DemoTopLineasMasConsumoDatos":
                    RepTopLineasMasConsumoDatos(contenedor, tituloGrid, pestaniaActiva, Request.Path + "?Nav=DemoConsumoMatIntPorDiaPorConc&Linea=");
                    break;
                /*Reportes JH*/
                case "RepConsumoDatosPorDiaConcepto":
                    RepConsumoDatosPorDiaConcepto(contenedor, tituloGrid);
                    break;
                case "RepConsumoDatosNacional":
                    RepConsumoDatosNacional1Pnl(contenedor, tituloGrid, pestaniaActiva, Request.Path + "?Nav=RepConsumoDatosNacionalN2&Concepto='+CONVERT(VARCHAR,ClaveConcepto)'");
                    break;
                case "RepPorConceptoEmple1Pnl":
                    RepPorConceptoEmple(contenedor, tituloGrid);
                    break;
                case "LlamadasMasCostosas1Pnl":
                    LlamadasMasCostosas(contenedor, tituloGrid);
                    break;
                case "LlamMasLargas1Pnl":
                    LlamMasLargas(contenedor, tituloGrid);
                    break;
                case "LineasNuevasTelcel2pPnl":
                    LineasNuevasTelcel(contenedor, tituloGrid, Rep2, tituloGrafica, "");
                    break;
                case "LineasTelcelDadasDeBaja1pnl":
                    LineasTelcelDadasDeBaja(contenedor, tituloGrid);
                    break;
                case "LineasConGastoMensaje1Pnl":
                    LineasConGastoMensaje(contenedor, tituloGrid);
                    break;
                case "DetallaFactTelcel1Pnl":
                    DetallaFactTelcel(contenedor, tituloGrid);
                    break;
                #region Dashboadr Consumo Internet
                case "RepDistribucionConsumoGB":
                    DistribucionGBInternet(contenedor, tituloGrafica);
                    break;
                case "RepdDistribucionConsumoMoneda":
                    DistribucionMonedaInternet(contenedor, tituloGrafica);
                    break;
                case "RepConsolidadoApp":
                    ConsolidadoPorApp(contenedor, tituloGrid);
                    break;
                case "RepConsumoHistoricoInternet":
                    ConsumoHistoricoInternet(contenedor, tituloGrafica + par);
                    break;
                case "RepDistribucionConsumoInternetPorApp":
                    DistribucionConsumoInternetPorApp(contenedor, tituloGrafica + par);
                    break;
                case "RepTabsConsumoInternet":
                    string label = (Request.QueryString["TipoConsumo"] == "Nac") ? "Consumo Internet Nacional" : "Consumo Internet Internacional";
                    UsoInternetPorTabs(contenedor, label);
                    break;
                case "RepHistGasto12Meses":
                    HistoricoGastoInternet12Meses(contenedor, tituloGrid);
                    break;
                case "RepTabsConsumoInternetNac":
                    param["TipoConsumo"] = "Nac";
                    TabsUsoInternetPor(contenedor, tituloGrafica, "RepTabsConsumoInternetNac");
                    break;
                case "RepTabsConsumoInternetInt":
                    param["TipoConsumo"] = "Int";
                    TabsUsoInternetPor(contenedor, tituloGrafica, "RepTabsConsumoInternetInterNac");
                    break;
                #endregion
                // TODO : DM Paso 3 - Agregar ID de reporte
                default:
                    break;
            }
        }



        #region Funcionalidad de Autocomplate de conceptos. //NZ: 20150324
        //NZ: 20150324

        [WebMethod]
        public static List<ConceptosMoviles> ConsultaParaAutoComplateConcepto(string concepto)
        {
            List<ConceptosMoviles> listConceptos = new List<ConceptosMoviles>();

            StringBuilder lsb = new StringBuilder();
            lsb.Append("SELECT ConceptoFiltro, ConceptoFiltroDescripcion \n ");
            lsb.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')] \n ");
            lsb.Append("WHERE ConceptoFiltro LIKE '%" + concepto + "%'");

            DataTable conceptoDescripcion = DSODataAccess.Execute(lsb.ToString());
            foreach (DataRow row in conceptoDescripcion.Rows)
            {
                listConceptos.Add(new ConceptosMoviles()
                {
                    Concepto = row[0].ToString(),
                    Descripcion = row[1].ToString()
                });
            }
            return listConceptos;
        }

        private string ConsultaPorConcepto()
        {
            string varConcepto;
            if (string.IsNullOrEmpty(txtConcepto.Text)) { varConcepto = txtConceptoDescripcion.ToolTip; }
            else { varConcepto = txtConcepto.Text; }

            StringBuilder lsb = new StringBuilder();
            lsb.Append("SELECT CONS.Telefono, EMP.NomCompleto, CONS.Total \n ");
            lsb.Append("FROM " + DSODataContext.Schema + ".consolidadofacturasdemoviles AS CONS \n ");
            lsb.Append("LEFT JOIN " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] AS EMP \n");
            lsb.Append("ON CONS.Emple=EMP.iCodCatalogo \n");
            lsb.Append("WHERE CONS.Concepto in(select ConceptoFiltro \n");
            lsb.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('DesgConcTelcelF1','Desglose conceptos Telcel F1','Español')]) \n ");
            lsb.Append("AND CONS.Concepto='" + varConcepto + "' \n");
            lsb.Append("AND FechaFactura >= '" + Session["FechaInicio"].ToString() + " 00:00:00' \n");
            lsb.Append("AND FechaFactura <= '" + Session["FechaFin"].ToString() + " 23:59:59' \n");
            lsb.Append("AND EMP.dtIniVigencia <> EMP.dtFinVigencia \n");
            lsb.Append("AND CONS.Total > 0");

            return lsb.ToString();
        }

        protected void MostrarTablaConceptos()
        {
            //Se obtiene el DataSource para el reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaPorConcepto());

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Telefono", "NomCompleto", "Total" });
                ldt.Columns["Telefono"].ColumnName = "Línea";
                ldt.Columns["NomCompleto"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.AcceptChanges();
            }

            Rep9.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepPorConceptoGrid_T", ldt, true, "Total",
                               new string[] { "", "", "{0:c}" }),
                               "MostrarTablaConceptos_T", "Lineas por concepto")
               );


            TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { string.Format("{0} - {1}", (string.IsNullOrEmpty(txtConcepto.Text)) ? txtConceptoDescripcion.ToolTip : txtConcepto.Text, txtConceptoDescripcion.Text) });
            txtConceptoDescripcion.ToolTip = (string.IsNullOrEmpty(txtConcepto.Text)) ? txtConceptoDescripcion.ToolTip : txtConcepto.Text;
            txtConcepto.Text = null;
        }

        protected bool ValidarPerfilParaAutocompleteConceptos()
        {
            bool mostrarAutocompleteConceptos = false;
            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();
            if (vchCodigoPerfilUsuario == "supervisordet") //Este perfil es al unico al que le sirve esta funcionalidad
            {
                mostrarAutocompleteConceptos = true;
            }
            return mostrarAutocompleteConceptos;
        }

        #endregion

        #region Descarga de documento de ayuda -ALFA  //NZ: 20150324
        //NZ: 20150324
        protected void btnAyuda_Click(object sender, EventArgs e)
        {
            //NZ: 20150324 Descarga del documento 
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=Ayuda.png");
            Response.WriteFile("~/images/Icono_ayuda.png");
            Response.Flush();
            Response.End();
        }

        //NZ: 20150324
        public void MostrarControlAyudaALFA()
        {
            if (DSODataContext.Schema.ToString().ToLower() == "alfacorporativo")
            {
                btnAyuda.Visible = true;
            }
            else { btnAyuda.Visible = false; }
        }
        #endregion

        #region Reporte Consumo de Internet por colaborador //NZ: 20150326

        //NZ: 20150326
        private string ConsultaConsumoInternetPorColaborador(int usuarEmple, int filtroLinea)
        {
            StringBuilder numsLineas = new StringBuilder();

            if (filtroLinea == 0)
            {
                //No se tomara en cuanta que la linea este vigente, por si el usuario visualizar una linea dada de baja en un mes en el que estuvo activa. En el SP se filtrara con la fecha de los calendar contra la fecha factura.
                StringBuilder lsb = new StringBuilder();
                lsb.Append("SELECT iCodCatalogo AS IdLinea \n ");
                lsb.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('linea','lineas','español')] \n ");
                lsb.Append("WHERE Emple = (SELECT TOP 1 iCodCatalogo \n");
                lsb.Append("FROM " + DSODataContext.Schema + ".[vishistoricos('emple','empleados','español')] \n");
                lsb.Append("WHERE Usuar = " + usuarEmple.ToString() + " \n");
                lsb.Append("AND dtinivigencia <> dtfinvigencia \n"); //Del empleado
                lsb.Append("AND dtfinvigencia >= getdate()) \n");
                lsb.Append("AND dtinivigencia <> dtfinvigencia \n"); //De la línea.

                DataTable ldtLineas = DSODataAccess.Execute(lsb.ToString());
                if (ldtLineas.Rows.Count > 0)
                {
                    for (int i = 0; i < ldtLineas.Rows.Count; i++)
                    {
                        if (i == 0) { numsLineas.Append(ldtLineas.Rows[i][0].ToString()); }
                        else { numsLineas.Append(", " + ldtLineas.Rows[i][0].ToString()); }
                    }
                }
                else { numsLineas.Append("-1"); }

            }
            else { numsLineas.Append(filtroLinea.ToString()); }

            StringBuilder lsbSP = new StringBuilder();
            lsbSP.Append("exec [ConsumoTelcelInternet]  @Schema='" + DSODataContext.Schema + "', \n");
            //lsbSP.Append("exec [zzjhConsumoTelcelInternet]  @Schema='" + DSODataContext.Schema + "', \n");
            lsbSP.Append("@Lineas='" + numsLineas.ToString() + "', \n ");
            lsbSP.Append("@FechaInicio='" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsbSP.Append("@FechaFin='" + Session["FechaFin"].ToString() + " 23:59:59' ");

            //RM 20180221
            return lsbSP.ToString();
        }

        //NZ: 20150326
        protected void ReporteConsumoInternetPorColaborador(Control contenedor)
        {
            //DataTable dtGraf = new DataTable();
            if (DSODataContext.Schema.ToString().ToLower() == "alfacorporativo")
            {
                //NZ 20150401
                string emple = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();

                if (lsCvePerfil.ToLower() == "epmpl" || emple != "-1") //NZ 20150401 
                {
                    int filtroLinea;
                    bool resultConvert = Int32.TryParse(param["Linea"], out filtroLinea);

                    DataTable RepConIntCol = DSODataAccess.Execute(ConsultaConsumoInternetPorColaborador(Convert.ToInt32(Session["iCodUsuario"]), (resultConvert) ? filtroLinea : 0));

                    bool banderaInternetIlimitado = CalculoDeColumnas(RepConIntCol);

                    #region Acomodo de los indices de las columnas
                    //20150504 NZ Se invierten los indices de Internet Disponible para que la que se muestre sea la de tipo String. 
                    //La otra se conservo numerica para poder hacer los calculos en caso de que corresponda. 

                    //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                    int[] mostrar;
                    int[] noMostrar;
                    if (banderaInternetIlimitado)
                    {
                        mostrar = new int[] { 0, 3, 4, 7, 8 };
                        noMostrar = new int[] { 1, 2, 5, 6, 9, 10, 11, 12, 13 };
                    }
                    else
                    {
                        mostrar = new int[] { 0, 1, 2 };
                        noMostrar = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                    }

                    #endregion Acomodo de los indices de las columnas

                    #region Grafica

                    // OBTENER GRAFICA
                    DataTable dtGraf = new DataTable();
                    dtGraf = RepConIntCol;

                    if (dtGraf.Rows.Count > 0)
                    {
                        DataView dvldt = new DataView(dtGraf);
                        dtGraf = dvldt.ToTable(false, new string[] { "Descripcion", "Porc Utilizado", "Porc No Utilizado" });//"Número Telcel"
                        dtGraf.AcceptChanges();
                    }


                    #endregion Grafica


                    //BG.20151006 Se agrega la condicion de Nav= EmpleN2
                    if (param["Nav"] == "DetConsCol" || param["Nav"] == "MiConsumoInternet2Pnl") //|| Nav == "EmpleN2")
                    {
                        ////20150504 NZ Se modificaron los parametros de las columnas a mostrar y no mostrar en Web
                        Rep9.Controls.Add(
                           DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                           DTIChartsAndControls.GridView("RepConIntColNvl2_T", DTIChartsAndControls.ordenaTabla(RepConIntCol, "[% Utilizado] Desc"),
                                           false, "", new string[] { "", "", "", "", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "{0:n}", "{0:n}", "{0:n}", "", "" },
                                           "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                           "RepConIntColNvl2_T", "Detalle del consumo")
                           );
                    }
                    else
                    {

                        //TABLA
                        contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                            DTIChartsAndControls.GridView("RepConIntColNvl1", DTIChartsAndControls.ordenaTabla(RepConIntCol, "[% Utilizado] Desc"),
                                            false, "", new string[] { "", "", "", "", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "{0:n}", "{0:n}", "{0:n}", "", "" },
                                            Request.Path + "?Nav=DetConsCol&Linea={0}&Carrier={1}", new string[] { "Número Telcel", "Carrier" }, 0, new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, new int[] { 1, 2 }, new int[] { 0 }),
                                            "RepConIntColNvl1", "Consumo de Internet", Request.Path + "?Nav=MiConsumoInternet2Pnl", 2, FCGpoGraf.Matricial));// DetConsCol

                        //GRAFICA

                        string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtGraf));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafInternetUtilizado",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtGraf), "RepConIntColNvl1",
                            "Consumo de Internet", "", "Linea", "Porcentaje", 2, FCGpoGraf.Matricial, "%"), false);

                    }
                }
            }

        }

        protected void ReporteConsumoInternetPorColaborador2Pnl(Control contenedor)
        {
            //DataTable dtGraf = new DataTable();
            //TabContainer tcReportes = new TabContainer();
            //tcReportes.CssClass = "MyTabStyle";

            //TabPanel tpRep1 = new TabPanel();
            //tcReportes.Tabs.Add(tpRep1);
            //tpRep1.HeaderText = "Tabla";


            if (DSODataContext.Schema.ToString().ToLower() == "alfacorporativo")
            {
                //NZ 20150401
                string emple = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();

                if (lsCvePerfil.ToLower() == "epmpl" || emple != "-1") //NZ 20150401 
                {
                    int filtroLinea;
                    bool resultConvert = Int32.TryParse(param["Linea"], out filtroLinea);

                    DataTable RepConIntCol = DSODataAccess.Execute(ConsultaConsumoInternetPorColaborador(Convert.ToInt32(Session["iCodUsuario"]), (resultConvert) ? filtroLinea : 0));

                    bool banderaInternetIlimitado = CalculoDeColumnas(RepConIntCol);

                    #region Acomodo de los indices de las columnas
                    //20150504 NZ Se invierten los indices de Internet Disponible para que la que se muestre sea la de tipo String. 
                    //La otra se conservo numerica para poder hacer los calculos en caso de que corresponda. 

                    //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                    int[] mostrar;
                    int[] noMostrar;
                    if (banderaInternetIlimitado)
                    {
                        mostrar = new int[] { 0, 3, 4, 7, 8 };
                        noMostrar = new int[] { 1, 2, 5, 6, 9, 10, 11, 12, 13 };
                    }
                    else
                    {
                        mostrar = new int[] { 0, 1, };
                        noMostrar = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                    }

                    #endregion Acomodo de los indices de las columnas

                    #region Grafica

                    // OBTENER GRAFICA
                    DataTable dtGraf = new DataTable();
                    dtGraf = RepConIntCol;

                    //TabPanel tpRep2 = new TabPanel();
                    //tcReportes.Tabs.Add(tpRep2);
                    //tpRep2.HeaderText = "Gráfica";

                    if (dtGraf.Rows.Count > 0)
                    {
                        DataView dvldt = new DataView(dtGraf);
                        dtGraf = dvldt.ToTable(false, new string[] { "Descripcion", "Porc Utilizado", "Porc No Utilizado" });//"Número Telcel"
                        dtGraf.AcceptChanges();
                    }


                    #endregion Grafica

                    DataView dtvRep = new DataView(RepConIntCol);
                    RepConIntCol = dtvRep.ToTable(false, new string[] { "Descripcion", "Responsable", "% Utilizado", "Internet Disponible (MB)", "Consumo (MB)", "Excedio (MB)", "% Excedido", "Gasto Local", "Gasto Internacional", "Porc Utilizado", "Porc No Utilizado", "Número Telcel", "Carrier" });

                    //BG.20151006 Se agrega la condicion de Nav= EmpleN2
                    if (param["Nav"] == "DetConsCol" || param["Nav"] == "MiConsumoInternet") //|| Nav == "EmpleN2")
                    {
                        ////20150504 NZ Se modificaron los parametros de las columnas a mostrar y no mostrar en Web
                        Rep9.Controls.Add(
                           DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                           DTIChartsAndControls.GridView("RepConIntColNvl2_T", DTIChartsAndControls.ordenaTabla(RepConIntCol, "[% Utilizado] Desc"),
                                           false, "", new string[] { "", "", "", "", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "{0:n}", "{0:n}", "{0:n}", "", "" },
                                           "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                           "RepConIntColNvl2_T", "Detalle del consumo")
                           );
                    }
                    else
                    {
                        //TABLA
                        Rep1.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepConIntColNvl1_T", DTIChartsAndControls.ordenaTabla(RepConIntCol, "[% Utilizado] Desc"),
                                            false, "", new string[] { "", "", "", "", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "{0:n}", "{0:n}", "{0:n}", "", "" },
                                            Request.Path + "?Nav=DetConsCol&Linea={0}&Carrier={1}", new string[] { "Número Telcel", "Carrier" }, 0, new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, new int[] { 1, 2 }, new int[] { 0 }),
                                            "RepConIntColNvl1_T", "Consumo de Internet", Request.Path + "?Nav=MiConsumoInternet2Pnl"));// DetConsCol

                        //GRAFICA
                        Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                        "GrafInternetUtilizado_G", "Consumo de Internet", 2, FCGpoGraf.Matricial, Request.Path + "?Nav=DetConsCol&Carrier='' + convert(varchar,[Codigo Carrier])"));

                        string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dtGraf));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafInternetUtilizado",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtGraf), "GrafInternetUtilizado_G",
                            "Consumo de Internet", "", "Linea", "Porcentaje", 2, FCGpoGraf.Matricial, "%"), false);

                    }
                }
            }

            //tcReportes.ActiveTabIndex = 2;
            //contenedor.Controls.Add(tcReportes);

        }

        private static bool CalculoDeColumnas(DataTable RepConIntCol)
        {
            bool banderaInternetIlimitado = false;
            #region Columnas Calculadas
            if (RepConIntCol.Rows.Count > 0)
            {
                // 20150504 NZ Se cambia de tipo esta columna para poder manejar textos en ella.
                //RepConIntCol.Columns.Add("% Utilizado", System.Type.GetType("System.Decimal")).SetOrdinal(2);
                RepConIntCol.Columns.Add("% Utilizado", System.Type.GetType("System.String")).SetOrdinal(2);
                RepConIntCol.Columns.Add("Excedio (MB)", System.Type.GetType("System.Decimal")).SetOrdinal(5);
                RepConIntCol.Columns.Add("% Excedido", System.Type.GetType("System.Decimal")).SetOrdinal(6);


                //20150504 NZ Se hace una version de esta columna en tipo String. Para poder manejar textos o numeros en esta columna.                       
                RepConIntCol.Columns.Add("Internet Disponible (MB) ", System.Type.GetType("System.String")).SetOrdinal(9);
                RepConIntCol.Columns.Add("Porc Utilizado", System.Type.GetType("System.Decimal")).SetOrdinal(10);
                RepConIntCol.Columns.Add("Porc No Utilizado", System.Type.GetType("System.Decimal")).SetOrdinal(11);


                for (int row = 0; row < RepConIntCol.Rows.Count; row++)
                {
                    //20150410 NZ Se agrega validacion cuando no viene el consumo de internet disponible para hacer los calculos.
                    if (RepConIntCol.Rows[row][3] == DBNull.Value || Convert.ToInt32(RepConIntCol.Rows[row][3]) == 0)
                    {
                        RepConIntCol.Rows.Remove(RepConIntCol.Rows[row]);
                    }
                    else
                    {
                        decimal variable;
                        if (Convert.ToDecimal(RepConIntCol.Rows[row][3]) == (9999 * 1024))
                        {
                            RepConIntCol.Rows[row][2] = "Internet Ilimitado";
                            banderaInternetIlimitado = true;
                        }
                        else
                        {
                            variable = Convert.ToDecimal(RepConIntCol.Rows[row][4]) / Convert.ToDecimal(RepConIntCol.Rows[row][3]);
                            RepConIntCol.Rows[row][2] = (variable > 1M) ? "100" : variable.ToString("0.## %");
                        }

                        variable = Convert.ToDecimal(RepConIntCol.Rows[row][4]) - Convert.ToDecimal(RepConIntCol.Rows[row][3]);
                        RepConIntCol.Rows[row][5] = (variable < 0M) ? 0M : variable;

                        variable = Convert.ToDecimal(RepConIntCol.Rows[row][5]) / Convert.ToDecimal(RepConIntCol.Rows[row][3]);
                        RepConIntCol.Rows[row][6] = (variable < 0M) ? 0M : variable;

                        variable = Convert.ToDecimal(RepConIntCol.Rows[row][3]);
                        RepConIntCol.Rows[row][9] = (variable == (9999 * 1024)) ? "Internet Ilimitado" : variable.ToString("##,0.00");

                        //BG.20170228 Calculo del % No utilizado
                        variable = Convert.ToDecimal(RepConIntCol.Rows[row][2].ToString().Replace("%", "").Trim());
                        RepConIntCol.Rows[row][10] = variable;

                        if (variable < 100M && variable >= 0M)
                        {
                            RepConIntCol.Rows[row][11] = (100M - variable);
                        }
                        else
                        {
                            RepConIntCol.Rows[row][11] = 0M;
                        }

                    }

                }

                RepConIntCol.Columns[9].SetOrdinal(3);
                RepConIntCol.Columns[4].SetOrdinal(9);
            }
            #endregion
            return banderaInternetIlimitado;
        }

        private string ObtenerRelacionEmpleadosUsuarioPerfil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT Emple.iCodCatalogo as IDEmple, Emple.Usuar, usuario.PerfilCod \r ");
            consulta.Append("FROM " + DSODataContext.Schema + ".vEmpleado as Emple \r");
            consulta.Append("JOIN " + DSODataContext.Schema + ".vUsuario as usuario \r");
            consulta.Append("ON Emple.Usuar = usuario.iCodCatalogo \r");
            consulta.Append("AND Emple.dtfinvigencia >= getdate() \r");
            consulta.Append("AND Emple.iCodCatalogo = " + param["Emple"]);

            return consulta.ToString();
        }

        //NZ 20150708 Para Cliente ALFA se agrega el reporte de consumo de internet.
        private void ReporteConsumoInternetPorColaborador2(Control contenedor)
        {
            if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Admin" || lsCvePerfil == "Sup")
            {
                DataTable dtTemp = DSODataAccess.Execute(ObtenerRelacionEmpleadosUsuarioPerfil());
                if (dtTemp.Rows.Count > 0)
                {
                    int iCodUsuario = (dtTemp.Rows[0]["Usuar"] != DBNull.Value) ? Convert.ToInt32(dtTemp.Rows[0]["Usuar"]) : 0;

                    DataTable RepConIntCol = DSODataAccess.Execute(ConsultaConsumoInternetPorColaborador(iCodUsuario, 0));

                    bool banderaInternetIlimitado = CalculoDeColumnas(RepConIntCol);

                    #region Acomodo de los indices de las columnas
                    //20150504 NZ Se invierten los indices de Internet Disponible para que la que se muestre sea la de tipo String. 
                    //La otra se conservo numerica para poder hacer los calculos en caso de que corresponda. 

                    //Se especifica que columnas se mostraran en el nivel dependiendo de esta bandera.
                    int[] mostrar;
                    int[] noMostrar;
                    if (banderaInternetIlimitado)
                    {
                        mostrar = new int[] { 0, 3, 4, 7, 8 };
                        noMostrar = new int[] { 0, 1, 2, 5, 6, 9 };
                    }
                    else
                    {
                        mostrar = new int[] { 0, 2, 3, 4, 5, 6, 7, 8 };
                        noMostrar = new int[] { 0, 1, 9 };
                    }

                    #endregion Acomodo de los indices de las columnas

                    contenedor.Controls.Add(
                          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                          DTIChartsAndControls.GridView("RepConIntColNvl22", DTIChartsAndControls.ordenaTabla(RepConIntCol, "[% Utilizado] Desc"),
                                          false, "", new string[] { "", "", "", "", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "{0:n}" },
                                          "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                          "ReporteConsumoInternetPorColaborador2_T", "Consumo de Internet"));
                }
            }
        }

        #endregion

        #region Reporte Consumo HISTORICO de Internet por colaborador //BG: 20170224
        private string ConsultaConsumoHistoricoInternet(int idEmple, int filtroLinea)
        {
            StringBuilder numsLineas = new StringBuilder();

            if (filtroLinea == 0)
            {
                //No se tomara en cuanta que la linea este vigente, por si el usuario visualizar una linea dada de baja en un mes en el que estuvo activa. En el SP se filtrara con la fecha de los calendar contra la fecha factura.
                StringBuilder lsb = new StringBuilder();
                lsb.AppendLine("SELECT iCodCatalogo AS IdLinea");
                lsb.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')]");
                lsb.AppendLine("WHERE Emple = " + idEmple);
                lsb.AppendLine("AND dtIniVigencia <> dtFinVigencia");
                lsb.AppendLine("AND Carrier = " + param["Carrier"]);

                DataTable ldtLineas = DSODataAccess.Execute(lsb.ToString());
                if (ldtLineas.Rows.Count > 0)
                {
                    for (int i = 0; i < ldtLineas.Rows.Count; i++)
                    {
                        if (i == 0) { numsLineas.Append(ldtLineas.Rows[i][0].ToString()); }
                        else { numsLineas.Append(", " + ldtLineas.Rows[i][0].ToString()); }
                    }
                }
                else { numsLineas.Append("-1"); }
            }
            else { numsLineas.Append(filtroLinea.ToString()); }

            StringBuilder lsbSP = new StringBuilder();
            //lsbSP.Append("exec [zzBGConsumoTelcelInternetHistorico] \n");
            lsbSP.Append("exec [ConsumoTelcelInternetHistorico] \n");
            lsbSP.Append("@Schema='" + DSODataContext.Schema + "', \n");
            lsbSP.Append("@Lineas='" + numsLineas.ToString() + "', \n ");
            lsbSP.Append("@Carrier= " + param["Carrier"] + ", \n ");
            lsbSP.Append("@Fields = '[Fecha],[Número Telcel],[Responsable],[Internet Disponible (MB)], \n ");
            lsbSP.Append("[Consumo (MB)],[Gasto Local],[Gasto Internacional],[Anio],[FechaK5],[PromedioAnual]', \n ");
            lsbSP.Append("@FechaInicio='" + Session["FechaInicio"].ToString() + " 00:00:00', \n");
            lsbSP.Append("@FechaFin='" + Session["FechaFin"].ToString() + " 23:59:59' ");

            return lsbSP.ToString();
        }

        protected void ReporteConsumoHistoricoInternet()
        {
            DataTable dtGraf = new DataTable();

            if (DSODataContext.Schema.ToString().ToLower() == "alfacorporativo")
            {
                int idUsuario = 0;
                int idEmple = 0;

                if (string.IsNullOrEmpty(param["Emple"])) //Entra desde Mi Consumo.
                {
                    idUsuario = Convert.ToInt32(Session["iCodUsuario"]);
                    idEmple = Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()));
                }
                else //Entro navegando
                {
                    idEmple = Convert.ToInt32(param["Emple"]);
                    DataTable dtUsuar = DSODataAccess.Execute(ObtenerRelacionEmpleadosUsuarioPerfil());
                    if (dtUsuar.Rows.Count > 0)
                    {
                        idUsuario = Convert.ToInt32(dtUsuar.Rows[0][1]);
                    }
                }

                if (idEmple != -1 || idEmple != 0)
                {
                    int filtroLinea;
                    bool resultConvert = Int32.TryParse(param["Linea"], out filtroLinea);

                    DataTable RepConHistoricoIntCol = DSODataAccess.Execute(ConsultaConsumoHistoricoInternet(idEmple, (resultConvert) ? filtroLinea : 0));

                    bool banderaInternetIlimitado = CalculoDeColumnasRepHist(RepConHistoricoIntCol);

                    //BG.20170228 Las siguientes 2 lineas son para obtener los datos del año anterior y que se grafique.
                    if (RepConHistoricoIntCol.Rows.Count > 0)
                    {
                        int minAnio = Convert.ToInt32(RepConHistoricoIntCol.Rows[0]["Anio"]);
                        dtGraf = RepConHistoricoIntCol.AsEnumerable().Where(x => x.Field<int>("Anio") == minAnio).CopyToDataTable();
                    }

                    #region Acomodo de los indices de las columnas
                    //20150504 NZ Se invierten los indices de Internet Disponible para que la que se muestre sea la de tipo String. 
                    //La otra se conservo numerica para poder hacer los calculos en caso de que corresponda. 

                    //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                    int[] mostrar;
                    int[] noMostrar;
                    if (banderaInternetIlimitado)
                    {
                        mostrar = new int[] { 3, 4, 7, 8 };
                        noMostrar = new int[] { 0, 1, 2, 5, 6, 9 };
                    }
                    else
                    {
                        //BG.20170227 
                        mostrar = new int[] { 0, 2, 3, 7, 5, 6, 8, 10 };
                        noMostrar = new int[] { 1, 4, 9, 11, 12, 13 };
                    }

                    #endregion Acomodo de los indices de las columnas


                    //BG.20170228 Agrego gráfica histórica multiseries solo del año anterior.
                    #region Grafica

                    if (dtGraf.Rows.Count > 0)
                    {
                        DataView dvldt = new DataView(dtGraf);
                        dtGraf = dvldt.ToTable(false, new string[] { "Fecha", "Internet Disponible (MB)", "Consumo (MB)", "PromedioAnual" });
                        if (dtGraf.Rows[dtGraf.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                        {
                            dtGraf.Rows[dtGraf.Rows.Count - 1].Delete();
                        }
                        dtGraf.AcceptChanges();
                    }

                    #endregion Grafica

                    if (param["Nav"] == "DetConsCol" || param["Nav"] == "MiConsumoInternet")
                    {
                        // BG.20170228 Pinta la Tabla
                        Rep0.Controls.Add(
                           DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepConHistoricoIntColNvl2_T", DTIChartsAndControls.ordenaTabla(RepConHistoricoIntCol, "[FechaK5] Asc"),
                                           //DTIChartsAndControls.GridView("RepConHistoricoIntColNvl2", DTIChartsAndControls.ordenaTabla(RepConHistoricoIntCol, "[% Utilizado] Desc"),
                                           false, "", new string[] { "", "", "{0:p}", "{0:n}", "{0:n}", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "", "{0:c}", "{0:n}", "", "{0:c}" },
                                           "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                           "RepConHistoricoIntColNvl2_T", "Consumo Histórico de Internet")
                           );

                        // BG.20170228 Pinta la Gráfica
                        Rep9.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsumoHistInternet_G", "Consumo Histórico de Internet", 2, FCGpoGraf.Matricial));

                        string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(dtGraf));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoHistInternet",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtGraf), "RepConsumoHistInternet_G",
                            "", "", "Fecha", "Cantidad", 2, FCGpoGraf.Matricial), false);

                    }
                    else if (param["Nav"] == "CenCosN4" || param["Nav"] == "CenCosN3" || param["Nav"] == "EmpleN2") //NZ 20170512
                    {
                        Rep9.Controls.Add(
                         DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                          DTIChartsAndControls.GridView("RepConHistoricoIntColNvl22_T", DTIChartsAndControls.ordenaTabla(RepConHistoricoIntCol, "[FechaK5] Asc"),
                                         false, "", new string[] { "", "", "{0:p}", "{0:n}", "{0:n}", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "", "{0:c}", "{0:n}", "", "{0:c}" },
                                         "", new string[] { }, 0, noMostrar, mostrar, new int[] { }),
                                         "RepConHistoricoIntColNvl22_T", "Consumo Histórico de Internet")
                         );
                    }
                    else
                    {

                        Rep6.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepConHistoricoIntColNvl1", DTIChartsAndControls.ordenaTabla(RepConHistoricoIntCol, "[FechaK5] Asc"),
                                            false, "", new string[] { "", "", "{0:p}", "{0:n}", "{0:n}", "{0:p}", "{0:c}", "{0:c}", "", "{0:c}", "{0:n}", "{0:n}", "{0:c}" },
                                            Request.Path + "?Nav=DetConsCol&Linea={0}", new string[] { "Número Telcel" }, 1, new int[] { 3, 4, 5, 6, 7, 8, 10 }, new int[] { 2, 9 }, new int[] { 1 }),
                                            "RepConHistoricoIntColNvl1_T", "Consumo Histórico de Internet", Request.Path + "?Nav=MiConsumoInternet")
                            );
                    }
                }
            }
        }

        private static bool CalculoDeColumnasRepHist(DataTable RepConHistoricoIntCol)
        {
            bool banderaInternetIlimitado = false;
            #region Columnas Calculadas
            if (RepConHistoricoIntCol.Rows.Count > 0)
            {
                // 20150504 NZ Se cambia de tipo esta columna para poder manejar textos en ella.
                RepConHistoricoIntCol.Columns.Add("% Utilizado", System.Type.GetType("System.String")).SetOrdinal(2);
                RepConHistoricoIntCol.Columns.Add("Excedio (MB)", System.Type.GetType("System.Decimal")).SetOrdinal(5);
                RepConHistoricoIntCol.Columns.Add("% Excedido", System.Type.GetType("System.Decimal")).SetOrdinal(6);

                //20150504 NZ Se hace una version de esta columna en tipo String. Para poder manejar textos o numeros en esta columna.                       
                RepConHistoricoIntCol.Columns.Add("Internet Disponible (MB) ", System.Type.GetType("System.String")).SetOrdinal(9);

                for (int row = 0; row < RepConHistoricoIntCol.Rows.Count; row++)
                {
                    //20150410 NZ Se agrega validacion cuando no viene el consumo de internet disponible para hacer los calculos.
                    // concepto Internet
                    if (RepConHistoricoIntCol.Rows[row][4] == DBNull.Value || Convert.ToInt32(RepConHistoricoIntCol.Rows[row][4]) == 0)
                    {
                        RepConHistoricoIntCol.Rows.Remove(RepConHistoricoIntCol.Rows[row]);
                    }
                    else
                    {
                        decimal variable;
                        //concepto internet
                        if (Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]) == (9999 * 1024))
                        {
                            //% utilizado
                            RepConHistoricoIntCol.Rows[row][2] = "Internet Ilimitado";
                            banderaInternetIlimitado = true;
                        }
                        else
                        {
                            variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][7]) / Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                            RepConHistoricoIntCol.Rows[row][2] = (variable > 1M) ? "100 %" : variable.ToString("0.## %");
                        }

                        //consumoMB - internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][7]) - Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][5] = (variable < 0M) ? 0M : variable;  //excedio MB


                        //excedioMB - internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][5]) / Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][6] = (variable < 0M) ? 0M : variable;  // % excedido

                        //internet
                        variable = Convert.ToDecimal(RepConHistoricoIntCol.Rows[row][4]);
                        RepConHistoricoIntCol.Rows[row][9] = (variable == (9999 * 1024)) ? "Internet Ilimitado" : variable.ToString("##,0.00"); //internet

                    }
                }

                RepConHistoricoIntCol.Columns[7].SetOrdinal(5);
                RepConHistoricoIntCol.Columns[9].SetOrdinal(3);
                RepConHistoricoIntCol.Columns[4].SetOrdinal(9);
            }
            #endregion
            return banderaInternetIlimitado;
        }



        #endregion Reporte Consumo HISTORICO de Internet por colaborador //BG: 20170224

        #region Costo por concepto Remover Concepto Rentas //20150331 NZ

        private DataTable CostoXConceptoRemoverRentas(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToInt32((object)row["idConcepto"]) == 237) //Id del concepto de rentas.
                {
                    table.Rows.Remove(row);
                    break;
                }
            }

            return table;
        }

        #endregion

        #region  Grafica de Consumo acumulado por Direccion  //20150713 NZ (ALFA)
        private void RepGrafConsumoAcumPorDireccion(Control contenedor, string tituloGrid, int pestanaActiva)
        {

            //BG. 20150819 Se pone un SP de prueba, cuando se pase a produccion hay que hacer el cambio al original.

            //DataTable dtReporte = DSODataAccess.Execute("Exec ConsumoAcumPorDireccion @Schema='" + DSODataContext.Schema + "', @Usuario = " + Session["iCodUsuario"]);
            DataTable dtReporte = DSODataAccess.Execute("Exec ConsumoAcumPorDireccion @Schema='"
                                 + DSODataContext.Schema + "', @Fecha = '" + Session["FechaInicio"].ToString() + " 00:00:00',@Usuario = " + Session["iCodUsuario"]);

            #region Grid
            //BG. 20150819 se cambia el calculo de fechas.
            if (dtReporte.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false,
                    new string[] { "Descripción", "Año Anterior", "Año Actual" });
                dtReporte.Columns["Año Anterior"].ColumnName = Convert.ToString(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1);
                dtReporte.Columns["Año Actual"].ColumnName = Convert.ToString(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepAnioAntVsActGridDireccion", dtReporte, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}" }),
                               "RepGrafConsumoAcumPorDireccion", tituloGrid, "", pestanaActiva, FCGpoGraf.Matricial)
               );

            #endregion Grid

            #region Grafica
            if (dtReporte.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Descripción", Convert.ToString(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1), Convert.ToString(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year) });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Descripción"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.AcceptChanges();
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(dtReporte));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepAnioAntVsActGrafDireccion",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dtReporte), "RepGrafConsumoAcumPorDireccion",
                "", "", "Dirección", "Importe", pestanaActiva, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }
        #endregion Grafica de Consumo acumulado por Direccion

    }
}
