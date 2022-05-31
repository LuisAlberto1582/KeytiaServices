using System;
using System.Collections.Generic;
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
using System.Web.UI.HtmlControls;
using System.Drawing;
using KeytiaWeb.UserInterface.DashboardFC.Reportes;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardSYO : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        #region Variables que guardan valores de queryString

        string Nav = string.Empty;
        string Sitio = string.Empty;
        string Emple = string.Empty;
        string MesAnio = string.Empty;
        string UriRegistrado = string.Empty;

        #endregion

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
            //Agrega a la pagina web las referencias a las clases de javaScript que utiliza FusionCharts
            DSOControl.LoadControlScriptBlock(Page, this.GetType(), "scriptsFusionCharts", FCAndControls.cargaScriptsFusionCharts(), true, false);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            //ReporteEstandar.InitValoresSession();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                #region Almacenar en variable de sesion los urls de navegacion
                List<string> list = new List<string>();
                string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

                if (Session["pltNavegacionDashFC"] != null) //Entonces ya tiene navegacion almacenada
                {
                    list = (List<string>)Session["pltNavegacionDashFC"];
                }

                //20141114 AM. Se agrega condicion para eliminar querystring ?Opc=XXXXX
                if (lsURL.Contains("?Opc="))
                {
                    //Asegurarse eliminar navegacion previa
                    list.Clear();

                    //Le quita el parametro Opc=XXXX
                    lsURL = lsURL.Substring(0, lsURL.IndexOf("?Opc="));
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
                Session["pltNavegacionDashFC"] = list;

                //Ocultar boton de regresar cuando solo exista un elemento en la lista
                if (list.Count <= 1)
                {
                    btnRegresar.Visible = false;
                }
                else
                {
                    btnRegresar.Visible = true;
                }
                #endregion

                LeeQueryString();

                if (!Page.IsPostBack)
                {
                    #region Inicia los valores default de los controles de fecha
                    try
                    {
                        if (Session["Language"].ToString() == "Español")
                        {
                            pdtInicio.setRegion("es");
                            pdtFin.setRegion("es");
                        }

                        //RJ.20160707 Se solicita mostrar sólo el año actual y dos previos
                        pdtInicio.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);
                        pdtFin.MinDateTime = new DateTime((DateTime.Now.Year - 2), 1, 1);

                        pdtInicio.MaxDateTime = DateTime.Today;
                        pdtFin.MaxDateTime = DateTime.Today;
                        CalculaFechasDeDashboard();
                        EstablecerBanderasClientePerfil();

                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                    #endregion Inicia los valores default de los controles de fecha
                }

                if (MesAnio != string.Empty)
                {
                    if (!Page.IsPostBack)
                    {
                        AjustaFechas();
                    }
                    //Quita el parametro MesAnio del querystring
                    List<string> locList = new List<string>();
                    locList = (List<string>)Session["pltNavegacionDashFC"];
                    //Si el ultimo elemento de la lista de URL's contiene el parametro MesAnio
                    if (locList[locList.Count - 1].Contains("&MesAnio="))
                    {
                        string lsUrl = locList[locList.Count - 1];
                        //Le quita el parametro al URL
                        lsUrl = lsUrl.Substring(0, lsUrl.IndexOf("&MesAnio="));
                        //Cambia el ultimo elemento de la lista por el URL sin el parametro
                        locList[locList.Count - 1] = lsUrl;
                        //Si el elemento tiene al menos 3 URL
                        if (locList.Count >= 3)
                        {
                            //Valida que los ultimos 2 elementos sean iguales
                            if (locList[locList.Count - 1] == locList[locList.Count - 2])
                            {
                                //Elimina uno de los elementos ya que son iguales
                                locList.RemoveAt(locList.Count - 1);
                            }
                        }
                        //Guarda en session la nueva lista de URL's
                        Session["pltNavegacionDashFC"] = locList;
                    }
                }                

                #region Fechas en sesion

                //Session["FechaIniRepDashFC"] = pdtInicio.DataValue.ToString();
                //Session["FechaFinRepDashFC"] = pdtFin.DataValue.ToString();

                //if (Session["FechaIniRepDashFC"] != null && Session["FechaFinRepDashFC"] != null)
                //{
                //    pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaIniRepDashFC"].ToString().Substring(1, 10));
                //    pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashFC"].ToString().Substring(1, 10));
                //}

                Session["FechaIniRep"] = Convert.ToDateTime(pdtInicio.DataValue.ToString().Substring(1, 10));
                Session["FechaFinRep"] = Convert.ToDateTime(pdtFin.DataValue.ToString().Substring(1, 10));

                if (Session["FechaIniRep"] != null && Session["FechaFinRep"] != null)
                {
                    pdtInicio.DataValue = (object)Session["FechaIniRep"];
                    pdtFin.DataValue = (object)Session["FechaFinRep"];
                }

                #endregion

                #region Dashboard

                if (Nav == string.Empty)
                {
                    btnExportarXLS.Visible = false;
                    DashboardMultiPerfil();
                }

                #endregion Dashboard

                #region Navegaciones

                else
                {
                    btnExportarXLS.Visible = true;
                    Navegaciones();
                }

                #endregion Navegaciones

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void CalculaFechasDeDashboard()
        {
            #region Calcula Fechas
            if (Session["FechaIniRep"] != null && Session["FechaFinRep"] != null)
            {
                pdtInicio.CreateControls();
                pdtInicio.DataValue = (object)Session["FechaIniRep"];
                pdtFin.CreateControls();
                pdtFin.DataValue = (object)Session["FechaFinRep"];
            }
            else
            {
                //20141204 AM. Se cambian las fechas default por la fecha maxima de detalleCDR
                DataTable fechaMaxCDR = new DataTable();
                if (DateTime.Now.Day > Convert.ToInt32(HttpContext.Current.Session["DiaLimiteMesAnt"]))
                {
                    fechaMaxCDR = DSODataAccess.Execute(GeneraConsultaMaxFechaInicioCDR());
                }
                else
                {
                    //RJ.20160904 Calcula y obtiene el día 1 del mes previo al actual

                    DateTime ultimoDiaMesAnt = new DateTime((DateTime.Now.Year), (DateTime.Now.Month), 1); //Dia 1 del mes actual
                    ultimoDiaMesAnt = ultimoDiaMesAnt.AddDays(-1); //Ultimo día del mes anterior al actual

                    fechaMaxCDR.Columns.Add("Anio", typeof(int));
                    fechaMaxCDR.Columns.Add("Mes", typeof(int));
                    fechaMaxCDR.Columns.Add("Dia", typeof(int));

                    fechaMaxCDR.Rows.Add(ultimoDiaMesAnt.Year, ultimoDiaMesAnt.Month, ultimoDiaMesAnt.Day);

                }

                // Se valida que el datatable contenga resultados.
                if (fechaMaxCDR.Rows.Count > 0)
                {
                    int Anio;
                    int Mes;
                    int Dia;
                    // Se valida que los resultados puedan ser enteros para poder formar la fecha.
                    if (
                            int.TryParse(fechaMaxCDR.Rows[0]["Anio"].ToString(), out Anio) &&
                            int.TryParse(fechaMaxCDR.Rows[0]["Mes"].ToString(), out Mes) &&
                            int.TryParse(fechaMaxCDR.Rows[0]["Dia"].ToString(), out Dia)
                        )
                    {
                        // Se forman las fechas de inicio y fin
                        fechaInicio = new DateTime(Anio, Mes, 1);
                        fechaFinal = new DateTime(Anio, Mes, Dia);

                        // Si el dia de la fecha fin es uno, 
                        // se calculan las fechas inicio y fin del mes anterior.
                        if (Dia == 1)
                        {
                            fechaInicio = fechaInicio.AddMonths(-1);
                            fechaFinal = fechaFinal.AddDays(-1);
                        }
                    }
                }
                // Si en CDR no hay informacion entonces los valores de las fechas se calculan con los
                // valores default de las variables fechaInicio y fechaFinal
                else
                {
                    // Si el dia de la fecha fin es uno, 
                    // se calculan las fechas inicio y fin del mes anterior.
                    if (fechaFinal.Day == 1)
                    {
                        fechaInicio = fechaInicio.AddMonths(-1);
                        fechaFinal = fechaFinal.AddDays(-1);
                    }
                }

                // Se asignan los valores de las fechas a los controles.
                pdtInicio.CreateControls();
                pdtInicio.DataValue = (object)fechaInicio;
                pdtFin.CreateControls();
                pdtFin.DataValue = (object)fechaFinal;

            }

            Session["FechaIniRep"] = Convert.ToDateTime(pdtInicio.DataValue.ToString().Substring(1, 10));
            Session["FechaFinRep"] = Convert.ToDateTime(pdtFin.DataValue.ToString().Substring(1, 10));
            #endregion Calcula Fechas
        }

        #region Logica de botones de la pagina

        protected void btnAplicar_Click(object sender, EventArgs e)
        {
            //Se agrega logica para quitar el parametro MesAnio del URL
            string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
            if (lsURL.Contains("&MesAnio="))
            {
                lsURL = lsURL.Substring(0, lsURL.IndexOf("&MesAnio="));
                Response.Redirect(lsURL);
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            List<string> ltNavegacion = (List<string>)Session["pltNavegacionDashFC"];

            //obtener el numero actual de elementos de la lista
            string lsCantidadElem = ltNavegacion.Count.ToString();
            //eliminar el ultimos elemento de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            HttpContext.Current.Response.Redirect(lsLastElement);
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            //20150425 RJ.Se agrega condición para exportar el reporte de Capitel en formato Excel 2003
            if (Nav != "RepTabDetalleParaFinanzas")
            {
                ExportXLS(".xlsx");
            }
            else
            {
                ExportXLS(".xls");
            }
        }        

        #endregion

        public void DashboardMultiPerfil()
        {
            DataTable reportes = DSODataAccess.Execute(consultaConfiguracionDeReportesPorPerfil());
            buscaReportes(reportes);
        }

        public void DashboardMultiPerfilPruebas()
        {
            DataTable reportes = new DataTable();
            reportes.Columns.Add("Reporte", typeof(string));
            reportes.Columns.Add("Contenedor", typeof(string));
            reportes.Columns.Add("tipoGrafDefault", typeof(string));
            reportes.Columns.Add("tituloGrid", typeof(string));
            reportes.Columns.Add("tituloGrafica", typeof(string));
            reportes.Columns.Add("pestaniaActiva", typeof(string));

            reportes.Rows.Add(new object[] { "RepPruebaSeeYouOn", "Rep1", "bar2d", "Reporte prueba", "Reporte prueba", "2" });

            buscaReportes(reportes);
        }

        public void Navegaciones()
        {
            switch (Nav)
            {
                #region Navegaciones Reporte Usuarios con mas trafico

                case "NavRepUsuariosMasTraficoSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "SyoSourceNumber", "Total" });
                    RepUsuariosMasTraficoSYO2Pnl(Request.Path + "?Nav=NavDetalleDeLlamadasEspecificoUMTSYO&UriRegistrado={0}", Rep1, Rep2, "bar2d", "Usuarios con más trafico", "Grafica");
                    break;
                #endregion Navegaciones Reporte Usuarios con mas trafico
                #region Navegaciones Reporte Clientes con mas trafico

                case "NavClientesConMasTraficoSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Descripcion", "Segundos" });
                    RepClientesConMasTraficoSYO2Pnl(Rep1, Rep2, "bar2d", "Clientes con más trafico", "Grafica");

                    break;
                #endregion Navegaciones Reporte Clientes con mas trafico
                #region Navegaciones Reporte Clientes con menos trafico

                case "NavClientesConMenosTraficoSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Descripcion", "Segundos" });
                    RepClientesConMenosTraficoSYO2Pnl(Rep1, Rep2, "bar2d", "Clientes con menos trafico", "Grafica");
                    break;
                #endregion Navegaciones Reporte Destinos mas Marcados
                #region Destinos Mas Marcados

                case "NavDestinosMasMarcadosSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Uris Marcados", "Numero de Marcaciones" });
                    RepDestinosMasMarcadosSYO2Pnl(Rep1, Rep2, "bar2d", "Destinos más marcados", "Grafica");
                    break;
                #endregion Navegacion Reporte Destinos mas Marcados
                #region Uso de Infraestructura Por Uri

                case "NavUsoDeInfraestructuraPorUriSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "URI", "Total Segundos" });
                    RepUsoDeInfraestructuraPorUriSYO2Pnl(Rep1, Rep2, "bar2d", "Usuarios con más uso por cliente", "Grafica");
                    break;
                #endregion Uso de Infraestructura Por Uri
                #region Usuarios con menos uso por cliente

                case "NavUsuariosConMenosUsoPorClienteSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "URI", "Total Segundos" });
                    RepUsuariosConMenosUsoPorClienteSYO2Pnl(Rep1, Rep2, "bar2d", "Usuarios con menos uso por cliente", "Grafica");
                    break;
                #endregion Usuarios con menos uso por cliente
                #region Servicios Por Cliente

                case "NavServiciosPorClienteSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "URI", "Total Segundos" });
                    RepServiciosPorClienteSYO2Pnl(Request.Path + "?Nav=NavDetalleDeLlamadasEspecificoSYO&UriRegistrado={0}", Rep0, Rep2, "bar2d", "Servicios por cliente", "Grafica");
                    break;
                #endregion Servicios Por Cliente
                #region Detalle de llamadas de uri especifico

                case "NavDetalleDeLlamadasEspecificoSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de llamadas de Uri especifico" });
                    RepDetalleDeLlamadasDeUriSYO2Pnl(Rep0, "Detalle de llamadas de URI", UriRegistrado);
                    break;
                #endregion Detalle de llamadas de uri especifico
                #region Tendencia anual

                case "NavTendenciaAnualSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Tendencia anual" });
                    RepTendenciaAnualSYO2Pnl(Rep1, Rep2, "doughnut3d", "Tendencia anual", "Grafica");
                    break;
                #endregion Tendencia anual
                #region Detalle de llamadas de uri especifico de usuarios con mas trafico

                case "NavDetalleDeLlamadasEspecificoUMTSYO":
                    lblInicio.Text = FCAndControls.AgregaEtiqueta(new string[] { "Detalle de llamadas de Uri especifico" });
                    RepDetalleDeLlamadasDeUriUMTSYO2Pnl(Rep0, "Detalle de llamadas de URI", UriRegistrado);

                    break;
                #endregion Detalle de llamadas de uri especifico de usuarios con mas trafico
                // TODO : DO Paso 4 - Agregar Navegacion de reporte  NavTendenciaAnualSYO
                default:
                    break;
            }
        }

        #region Reportes

        private void RepUsuariosMasTraficoSYO1Pnl(string linkGrid, Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosMasTraficoSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "SYODisplayName", "SYOSourceNumber", "Total" });
                ldt.Columns["iCodCatalogo"].ColumnName = "iCodCatalogo";
                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["SYOSourceNumber"].ColumnName = "Uri";
                ldt.Columns["Total"].ColumnName = "Total";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab55", ldt, true, "Totales", 
                                      new string[] { "", "", "", "{0}" }, linkGrid,
                                      new string[] { "iCodCatalogo" }, 1, new int[] { 0 }, new int[] { 1, 3 }, new int[] { 2 }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavRepUsuariosMasTraficoSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "Nombre", "Uri", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Uri"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsuariosMasTrafico1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavRepUsuariosMasTraficoSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsuariosMasTrafico1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsuariosMasTrafico1Pnl", "", "", "Uri", "Tiempo de uso en minutos", "ocean", "98%", "330", "SYOUsuariosMasTrafico1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepUsuariosMasTraficoSYO2Pnl(string linkGrid, Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosMasTraficoSYO());

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "SYODisplayName", "SYOSourceNumber", "Total" });
                ldt.Columns["iCodCatalogo"].ColumnName = "iCodCatalogo";
                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["SYOSourceNumber"].ColumnName = "Uri";
                ldt.Columns["Total"].ColumnName = "Total";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab55", ldt, true, "Totales",
                                      new string[] { "", "", "", "{0}" }, linkGrid,
                                      new string[] { "iCodCatalogo" }, 1, new int[] { 0 }, new int[] { 1, 3 }, new int[] { 2 }),
                                      "Reporte", tituloGrid, 0)
                      );
            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "Nombre", "Uri", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Uri"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsuariosMasTrafico2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsuariosMasTrafico2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsuariosMasTrafico2Pnl", "", "", "Uri", "Tiempo de uso en segundos", "ocean", "98%", "330", "SYOUsuariosMasTrafico2Pnl", false), false);

            #endregion Grafica
        }
        private void RepClientesConMasTraficoSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMasTraficoSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Segundos" });

                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Segundos"].ColumnName = "Minutos";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab56", ldt, true, "Totales", 
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavClientesConMasTraficoSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Descripcion"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Descripcion"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOClientesConMasTrafico1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavClientesConMasTraficoSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOClientesConMasTrafico1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOClientesConMasTrafico1Pnl", "", "", "Cliente", "Tiempo de uso en Minutos", "ocean", "98%", "330", "SYOClientesConMasTrafico1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepClientesConMasTraficoSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMasTraficoSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Segundos" });

                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Segundos"].ColumnName = "Minutos";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab56", ldt, true, "Totales",
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Descripcion"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Descripcion"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOClientesConMasTrafico2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOClientesConMasTrafico2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOClientesConMasTrafico2Pnl", "", "", "Cliente", "Tiempo de uso en minutos", "ocean", "98%", "330", "SYOClientesConMasTrafico2Pnl", false), false);

            #endregion Grafica


        }
        private void RepClientesConMenosTraficoSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMenosTraficoSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Segundos" });

                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Segundos"].ColumnName = "Minutos";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab57", ldt, true, "Totales", 
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavClientesConMenosTraficoSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Descripcion"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Descripcion"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOClientesConMenosTrafico1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavClientesConMenosTraficoSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOClientesConMenosTrafico1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOClientesConMenosTrafico1Pnl", "", "", "Cliente", "Tiempo de uso en minutos", "ocean", "98%", "330", "SYOClientesConMenosTrafico1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepClientesConMenosTraficoSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMenosTraficoSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Segundos" });

                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Segundos"].ColumnName = "Minutos";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab57", ldt, true, "Totales", 
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Descripcion"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Descripcion"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOClientesConMenosTrafico2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOClientesConMenosTrafico2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOClientesConMenosTrafico2Pnl", "", "", "Cliente", "Tiempo de uso en minutos", "ocean", "98%", "330", "SYOClientesConMenosTrafico2Pnl", false), false);

            #endregion Grafica


        }
        private void RepDestinosMasMarcadosSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaDestinosMasMarcadosSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Marcados", "NoDeMarcaciones" });

                ldt.Columns["Marcados"].ColumnName = "DestinationNumber";
                ldt.Columns["NoDeMarcaciones"].ColumnName = "Numero de marcaciones";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab58", ldt, true, "Totales", 
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavDestinosMasMarcadosSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "DestinationNumber", "Numero de marcaciones" });
                if (ldt.Rows[ldt.Rows.Count - 1]["DestinationNumber"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["DestinationNumber"].ColumnName = "label";
                ldt.Columns["Numero de marcaciones"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYODestinosMasMarcados1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavDestinosMasMarcadosSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYODestinosMasMarcados1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYODestinosMasMarcados1Pnl", "", "", "DestinationNumber", "Numero de Marcaciones", "ocean", "98%", "330", "SYODestinosMasMarcados1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepDestinosMasMarcadosSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaDestinosMasMarcadosSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Marcados", "NoDeMarcaciones" });

                ldt.Columns["Marcados"].ColumnName = "DestinationNumber";
                ldt.Columns["NoDeMarcaciones"].ColumnName = "Numero de marcaciones";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab58", ldt, true, "Totales", 
                                      new string[] { "", "{0}" }, "",
                                      new string[] { }, 0, new int[] { }, new int[] { 0, 1 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "DestinationNumber", "Numero de marcaciones" });
                if (ldt.Rows[ldt.Rows.Count - 1]["DestinationNumber"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["DestinationNumber"].ColumnName = "label";
                ldt.Columns["Numero de marcaciones"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYODestinosMasMarcados2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYODestinosMasMarcados2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYODestinosMasMarcados2Pnl", "", "", "DestinationNumber", "Numero de Marcaciones", "ocean", "98%", "330", "SYODestinosMasMarcados2Pnl", false), false);

            #endregion Grafica


        }
        private void RepUsoDeInfraestructuraPorUriSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaUsoDeInfraestructuraPorUriSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SYODisplayName", "URI", "TotalSegundos", "Minutos", "Horas" });

                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["TotalSegundos"].ColumnName = "TotalSegundos";
                ldt.Columns["Minutos"].ColumnName = "Minutos";
                ldt.Columns["Horas"].ColumnName = "Horas";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab59", ldt, true, "Totales", 
                                      new string[] { "", "", "{0}", "{0}", "{0}" }, "",
                                      new string[] { }, 0, new int[] { 2, 4 }, new int[] { 0, 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavUsoDeInfraestructuraPorUriSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "URI", "TotalSegundos", "Minutos", "Horas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["URI"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsoDeInfraestructuraPorUri1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavUsoDeInfraestructuraPorUriSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsoDeInfraestructuraPorUri1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsoDeInfraestructuraPorUri1Pnl", "", "", "Uri", "Tiempo de duracion en minutos", "ocean", "98%", "330", "SYOUsoDeInfraestructuraPorUri1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepUsoDeInfraestructuraPorUriSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaUsoDeInfraestructuraPorUriSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SYODisplayName", "URI", "TotalSegundos", "Minutos", "Horas" });

                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["TotalSegundos"].ColumnName = "TotalSegundos";
                ldt.Columns["Minutos"].ColumnName = "Minutos";
                ldt.Columns["Horas"].ColumnName = "Horas";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab59", ldt, true, "Totales",
                                      new string[] { "", "", "{0}", "{0}", "{0}" }, "",
                                      new string[] { }, 0, new int[] { 2, 4 }, new int[] { 0, 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "URI", "TotalSegundos", "Minutos", "Horas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["URI"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsoDeInfraestructuraPorUri2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavUsoDeInfraestructuraPorUriSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsoDeInfraestructuraPorUri2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsoDeInfraestructuraPorUri2Pnl", "", "", "Uri", "Tiempo de duracion en minutos", "ocean", "98%", "330", "SYOUsoDeInfraestructuraPorUri2Pnl", false), false);


            #endregion Grafica


        }
        private void RepUsuariosConMenosUsoPorClienteSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosConMenosUsoPorClienteSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SYODisplayName", "URI", "TotalSegundos", "Minutos", "Horas" });

                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["TotalSegundos"].ColumnName = "TotalSegundos";
                ldt.Columns["Minutos"].ColumnName = "Minutos";
                ldt.Columns["Horas"].ColumnName = "Horas";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab59", ldt, true, "Totales", 
                                      new string[] { "", "", "{0}", "{0}", "{0}" }, "",
                                      new string[] { }, 0, new int[] { 2, 4 }, new int[] { 0, 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavUsuariosConMenosUsoPorClienteSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "URI", "TotalSegundos", "Minutos", "Horas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["URI"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsuariosConMenosUsoPorCliente1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavUsuariosConMenosUsoPorClienteSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsuariosConMenosUsoPorCliente1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsuariosConMenosUsoPorCliente1Pnl", "", "", "Uri", "Tiempo de duracion en minutos", "ocean", "98%", "330", "SYOUsuariosConMenosUsoPorCliente1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepUsuariosConMenosUsoPorClienteSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosConMenosUsoPorClienteSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SYODisplayName", "URI", "TotalSegundos", "Minutos", "Horas" });
                ldt.Columns["SYODisplayName"].ColumnName = "Nombre";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["TotalSegundos"].ColumnName = "TotalSegundos";
                ldt.Columns["Minutos"].ColumnName = "Minutos";
                ldt.Columns["Horas"].ColumnName = "Horas";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab59", ldt, true, "Totales", 
                                      new string[] { "", "", "{0}", "{0}", "{0}" }, "",
                                      new string[] { }, 0, new int[] { 2, 4 }, new int[] { 0, 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "URI", "TotalSegundos", "Minutos", "Horas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["URI"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOUsuariosConMenosUsoPorCliente2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOUsuariosConMenosUsoPorCliente2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOUsuariosConMenosUsoPorCliente2Pnl", "", "", "Uri", "Tiempo de duracion en minutos", "ocean", "98%", "330", "SYOUsuariosConMenosUsoPorCliente2Pnl", false), false);

            #endregion Grafica


        }
        private void RepServiciosPorClienteSYO1Pnl(string linkGrid, Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaServiciosPorClienteSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "DisplayName", "UserName", "AddressPattern", "URI", "Registro" });

                ldt.Columns["iCodCatalogo"].ColumnName = "iCodCatalogo";
                ldt.Columns["DisplayName"].ColumnName = "DisplayName";
                ldt.Columns["UserName"].ColumnName = "UserName";
                ldt.Columns["AddressPattern"].ColumnName = "AddressPattern";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["Registro"].ColumnName = "Numero de llamadas";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab60", ldt, true, "Totales", 
                                      new string[] { "", "", "", "", "", "" }, linkGrid,
                                      new string[] { "iCodCatalogo" }, 1, new int[] { 0, 2, 3 }, new int[] { 1, 5 }, new int[] { 4 }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavServiciosPorClienteSYO")
                      );

            #endregion Grid



            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepServiciosPorClienteSYO2Pnl(string linkGrid, Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaServiciosPorClienteSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "DisplayName", "UserName", "AddressPattern", "URI", "Registro" });

                ldt.Columns["iCodCatalogo"].ColumnName = "iCodCatalogo";
                ldt.Columns["DisplayName"].ColumnName = "DisplayName";
                ldt.Columns["UserName"].ColumnName = "UserName";
                ldt.Columns["AddressPattern"].ColumnName = "AddressPattern";
                ldt.Columns["URI"].ColumnName = "URI";
                ldt.Columns["Registro"].ColumnName = "Numero de llamadas";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab60", ldt, true, "Totales", 
                                      new string[] { "", "", "", "", "", "" }, linkGrid,
                                      new string[] { "iCodCatalogo" }, 1, new int[] { 0, 2, 3 }, new int[] { 1, 5 }, new int[] { 4 }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid




        }
        private void RepDetalleDeLlamadasDeUriSYO2Pnl(Control contenedorGrid, string tituloGrid, string registroUri)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleDeLlamadasDeUriSYO(registroUri));

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cliente", "SYOTime", "SYOSystemNameDesc", "SYONetworkAddress", "SYODurationSec","SYODuration","SYOSourceNumber",
                                                           "SYOSourceAddress","SYODestinationNumber","SYODestinationAddress","SYOCallTypeDesc","SYOBandwidth","SYOCauseCode",
                                                             "SYOMIBLog","SYOOwnerConference"});

                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["SYOTime"].ColumnName = "Time";
                ldt.Columns["SYOSystemNameDesc"].ColumnName = "SystemName";
                ldt.Columns["SYONetworkAddress"].ColumnName = "NetworkAddress";
                ldt.Columns["SYODurationSec"].ColumnName = "DurationSec";

                ldt.Columns["SYODuration"].ColumnName = "Duration";
                ldt.Columns["SYOSourceNumber"].ColumnName = "SourceNumber";
                ldt.Columns["SYOSourceAddress"].ColumnName = "SourceAddress";
                ldt.Columns["SYODestinationNumber"].ColumnName = "DestinationNumber";
                ldt.Columns["SYODestinationAddress"].ColumnName = "DestinationAddress";

                ldt.Columns["SYOCallTypeDesc"].ColumnName = "CallType";
                ldt.Columns["SYOBandwidth"].ColumnName = "Bandwidth";
                ldt.Columns["SYOCauseCode"].ColumnName = "CauseCode";
                ldt.Columns["SYOMIBLog"].ColumnName = "MIBLog";
                ldt.Columns["SYOOwnerConference"].ColumnName = "OwnerConference";


            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab61", ldt, true, "", 
                                      new string[] { "", "", "", "", "{0}", "", "", "", "", "", "", "", "", "", "" }, "",
                                      new string[] { }, 0, new int[] { 2, 3, 4, 7, 9, 10, 12, 13, 14 }, new int[] { 0, 1, 5, 6, 8, 11 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid
        }

        private void RepTendenciaAnualSYO1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            TabContainer tcReportes = new TabContainer();
            tcReportes.CssClass = "MyTabStyle";

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaTendenciaAnualSYO());

            #region Grid

            TabPanel tpRep1 = new TabPanel();
            tcReportes.Tabs.Add(tpRep1);
            tpRep1.HeaderText = "Tabla";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "No", "Mes", "Anterior", "Actual" });

                ldt.Columns["No"].ColumnName = "No";
                ldt.Columns["Mes"].ColumnName = "Mes";
                ldt.Columns["Anterior"].ColumnName = "Anterior";
                ldt.Columns["Actual"].ColumnName = "Minutos";
            }

            tpRep1.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab62", ldt, true, "Totales", 
                                      new string[] { "", "", "{0}", "{0}" }, "",
                                      new string[] { }, 1, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0, Request.Path + "?Nav=NavTendenciaAnualSYO")
                      );

            #endregion Grid

            #region Grafica

            TabPanel tpRep2 = new TabPanel();
            tcReportes.Tabs.Add(tpRep2);
            tpRep2.HeaderText = "Gráfica";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "No", "Mes", "Anterior", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            tpRep2.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOTendenciaAnual1Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavTendenciaAnualSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOTendenciaAnual1Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOTendenciaAnual1Pnl", "", "", "Mes", "Minutos", "ocean", "98%", "330", "SYOTendenciaAnual1Pnl", false), false);

            #endregion Grafica

            tcReportes.ActiveTabIndex = pestaniaActiva;
            contenedor.Controls.Add(tcReportes);
        }
        private void RepTendenciaAnualSYO2Pnl(Control contenedorGrid, Control contenedorGrafica, string tipoGrafDefault, string tituloGrid, string tituloGrafica)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaTendenciaAnualSYO());

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "No", "Mes", "Anterior", "Actual" });

                ldt.Columns["No"].ColumnName = "No";
                ldt.Columns["Mes"].ColumnName = "Mes";
                ldt.Columns["Anterior"].ColumnName = "Año anterior";
                ldt.Columns["Actual"].ColumnName = "Minutos";
            }

            contenedorGrid.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab62", ldt, true, "Totales",
                                      new string[] { "", "", "{0}", "{0}" }, "",
                                      new string[] { }, 1, new int[] { 0, 2 }, new int[] { 1, 3 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid

            #region Grafica



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "No", "Mes", "Año anterior", "Minutos" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";

                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
                        FCAndControls.CreaContenedorGraficaYRadioButtonsGraf1Serie("SYOTendenciaAnual2Pnl", "ControlesAlCentro", tipoGrafDefault)),
                                                                    "Gráfica", tituloGrafica, 0, Request.Path + "?Nav=NavTendenciaAnualSYO"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "SYOTendenciaAnual2Pnl",
                FCAndControls.grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                tipoGrafDefault, "SYOTendenciaAnual2Pnl", "", "", "Mes", "Minutos", "ocean", "98%", "330", "SYOTendenciaAnual2Pnl", false), false);
            #endregion Grafica


        }
        private void RepDetalleDeLlamadasDeUriUMTSYO2Pnl(Control contenedorGrid, string tituloGrid, string registroUri)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleDeLlamadasDeUriUMTSYO(registroUri));

            #region Grid



            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cliente", "SYOTime", "SYOSystemNameDesc", "SYONetworkAddress", "SYODurationSec","SYODuration","SYOSourceNumber",
                                                           "SYOSourceAddress","SYODestinationNumber","SYODestinationAddress","SYOCallTypeDesc","SYOBandwidth","SYOCauseCode",
                                                             "SYOMIBLog","SYOOwnerConference"});

                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["SYOTime"].ColumnName = "Time";
                ldt.Columns["SYOSystemNameDesc"].ColumnName = "SystemName";
                ldt.Columns["SYONetworkAddress"].ColumnName = "NetworkAddress";
                ldt.Columns["SYODurationSec"].ColumnName = "DurationSec";

                ldt.Columns["SYODuration"].ColumnName = "Duration";
                ldt.Columns["SYOSourceNumber"].ColumnName = "SourceNumber";
                ldt.Columns["SYOSourceAddress"].ColumnName = "SourceAddress";
                ldt.Columns["SYODestinationNumber"].ColumnName = "DestinationNumber";
                ldt.Columns["SYODestinationAddress"].ColumnName = "DestinationAddress";

                ldt.Columns["SYOCallTypeDesc"].ColumnName = "CallType";
                ldt.Columns["SYOBandwidth"].ColumnName = "Bandwidth";
                ldt.Columns["SYOCauseCode"].ColumnName = "CauseCode";
                ldt.Columns["SYOMIBLog"].ColumnName = "MIBLog";
                ldt.Columns["SYOOwnerConference"].ColumnName = "OwnerConference";


            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.tituloYBordesReporte(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab61", ldt, true, "", 
                                      new string[] { "", "", "", "", "{0}", "", "", "", "", "", "", "", "", "", "" }, "",
                                      new string[] { }, 0, new int[] { 2, 3, 4, 7, 9, 10, 12, 13, 14 }, new int[] { 0, 1, 5, 6, 8, 11 }, new int[] { }),
                                      "Reporte", tituloGrid, 0)
                      );

            #endregion Grid
        }
        // TODO : DO Paso 2 - Metodo que crea reporte
        /*Aqui podemos copiar uno de los metodos que estan ya hechos y hacer los ajustes necesarios para mostrar un nuevo reporte*/

        /******************************************************************************************************************************/
        private ArrayList FormatoColumRepDetallado(DataTable ldt, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 7, 8, 10 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 11, 12, 13, 14, 15, 16, 17 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 7, 8, 9 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 10, 11, 12, 13, 14, 15, 16, 17 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 8, 9, 10, 11 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 12, 13, 14, 15, 16, 17 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 7, 9, 10, 11 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 8, 12, 13, 14, 15, 16, 17 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            #endregion Logica de las columnas a mostrar

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                Array.Resize(ref columnasVisibles, columnasVisibles.Length + 1);
                columnasVisibles[columnasVisibles.Length - 1] = 18;
            }

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private ArrayList FormatoColumRepJerarquico(DataTable ldt, byte nombreTotalReal, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            Session["MuestraSM"] = 0; //alterar bandera para que nunca desglose SM para este reporte.

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 5, 9 };
                    columnasVisibles = new int[] { 4, 6, 7, 8 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 4, 9 };
                    columnasVisibles = new int[] { 5, 6, 7, 8 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 0, 3, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 2, 7, 8 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                    nombreTotalReal = 1;
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 3, 7, 8 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";
                    nombreTotalReal = 2;
                }
            }

            #endregion Logica de las columnas a mostrar

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(nombreTotalReal);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);

            EstablecerBanderasClientePerfil(); //Regresamos las banderas a los valores originales.

            return valores;
        }

        private void RepMatInventarioTelecom1Pnl(bool sobrecarga)
        {
            DataTable ldt = DSODataAccess.Execute("EXEC [RepInventarioEquiposDetall] @Esquema = '" + DSODataContext.Schema + "'");

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Cr", "Nombre", "Dirección IP", "Tipo Recurso", "GIG-E Ocu.", "Fa Ocu.", "GIG-E OFF", 
                                   "Fa OFF", "GIG-E Abajo", "Fa Abajo", "Total Puertos", "Admin.", "Marca", 
                                   "No. Serie", "Versión IOS", "Domicilio", });
            }

            Rep0.Controls.Add(
               DTIChartsAndControls.tituloYBordesReporte(
                               DTIChartsAndControls.GridView("RepMatInventarioTelecom", ldt, true, "Totales", 
                               new string[] { "", "", "", "", "{0:F0}", "{0:F0}", "{0:F0}", "{0:F0}", "{0:F0}", "{0:F0}", "{0:F0}", "", "", "", "", "" }),
                               "Reporte", "Reporte Inventario de Equipo Telecom", 0)
               );

            #endregion Grid
        }

        #endregion Reportes

        #region Consultas a BD

        ConsultasSYO consulta = new ConsultasSYO();
        public static string GeneraConsultaMaxFechaInicioCDR()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" \r");
            lsb.Append(" select \r");
            lsb.Append(" Datepart(year, Max(FechaInicio)) as Anio,  \r");
            lsb.Append(" Datepart(month, Max(FechaInicio)) as Mes,  \r");
            lsb.Append(" Datepart(day, Max(FechaInicio)) as Dia \r");
            lsb.Append(" from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')] \r");
            return lsb.ToString();
        }
        private void EstablecerBanderasClientePerfil()
        {
            StringBuilder consulta = new StringBuilder();
            consulta.Append("SELECT BanderasCliente \r");
            consulta.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')] \r");
            consulta.Append("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() \r");
            consulta.Append("AND UsuarDB = " + Session["iCodUsuarioDB"].ToString() + " \r");
            consulta.Append("AND (ISNULL(BanderasCliente,0) & 1024)/1024=1 ");

            DataTable dtConsulta = DSODataAccess.Execute(consulta.ToString());

            Session["MuestraSM"] = (dtConsulta.Rows.Count > 0) ? 1 : 0;

            StringBuilder consulta2 = new StringBuilder();
            consulta2.Append("SELECT BanderasPerfil \r");
            consulta2.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] \r");
            consulta2.Append("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() \r");
            consulta2.Append("AND iCodCatalogo = " + Session["iCodPerfil"].ToString() + " \r");
            consulta2.Append("AND (ISNULL(BanderasPerfil,0) & 2)/2=1 ");

            DataTable dtConsulta2 = DSODataAccess.Execute(consulta2.ToString());

            Session["MuestraCostoSimulado"] = (dtConsulta2.Rows.Count > 0) ? 1 : 0;
        }
        public string consultaConfiguracionDeReportesPorPerfil()
        {
            return consulta.consultaConfiguracionDeReportesPorPerfil((int)Session["iCodPerfil"]);
        }
        public string ConsultaPruebaSeeYouOn()
        {
            return consulta.ConsultaPruebaSeeYouOn((int)Session["iCodUsuario"]);
        }
        public string ConsultaUsuariosMasTraficoSYO()
        {
            return consulta.ConsultaUsuariosMasTraficoSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaClientesConMasTraficoSYO()
        {
            return consulta.ConsultaClientesConMasTraficoSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaClientesConMenosTraficoSYO()
        {
            return consulta.ConsultaClientesConMenosTraficoSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaDestinosMasMarcadosSYO()
        {
            return consulta.ConsultaDestinosMasMarcadosSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaUsoDeInfraestructuraPorUriSYO()
        {
            return consulta.ConsultaUsoDeInfraestructuraPorUriSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaUsuariosConMenosUsoPorClienteSYO()
        {
            return consulta.ConsultaUsuariosConMenosUsoPorClienteSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaServiciosPorClienteSYO()
        {
            return consulta.ConsultaServiciosPorClienteSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"));
        }
        public string ConsultaDetalleDeLlamadasDeUriSYO(string registroUri)
        {
            return consulta.ConsultaDetalleDeLlamadasDeUriSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"), registroUri);
        }
        public string ConsultaTendenciaAnualSYO()
        {
            return consulta.ConsultaTendenciaAnualSYO((int)Session["iCodUsuario"], (pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"));
        }
        public string ConsultaDetalleDeLlamadasDeUriUMTSYO(string registroUri)
        {
            return consulta.ConsultaDetalleDeLlamadasUriUMTSYO((pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00"), (pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59"), registroUri);
        }
        //// TODO : DO Paso 1 - Consulta del reporte

        #endregion Consultas a BD

        private void LeeQueryString()
        {

            #region Revisar si el querystring Nav contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["Nav"]))
            {
                try
                {
                    Nav = Request.QueryString["Nav"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (Nav) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                Nav = string.Empty;
            }

            #endregion Revisar si el querystring Nav contiene un valor

            #region Revisar si el querystring Sitio contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["Sitio"]))
            {
                try
                {
                    Sitio = Request.QueryString["Sitio"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (Sitio) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                Sitio = string.Empty;
            }

            #endregion Revisar si el querystring Sitio contiene un valor

            #region Revisar si el querystring Emple contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["Emple"]))
            {
                try
                {
                    Emple = Request.QueryString["Emple"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (Emple) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                Emple = string.Empty;
            }

            #endregion Revisar si el querystring Emple contiene un valor

            #region Revisar si el querystring MesAnio contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["MesAnio"]))
            {
                try
                {
                    MesAnio = Request.QueryString["MesAnio"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (MesAnio) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                MesAnio = string.Empty;
            }

            #endregion Revisar si el querystring MesAnio contiene un valor

            #region Revisar si el querystring UriRegistrado contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["UriRegistrado"]))
            {
                try
                {
                    UriRegistrado = Request.QueryString["UriRegistrado"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (UriRegistrado) en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                UriRegistrado = string.Empty;
            }

            #endregion Revisar si el querystring UriRegistrado contiene un valor
        }

        protected void AjustaFechas()
        {
            string[] FechaMesAnio = MesAnio.Split(new char[] { '-' });
            string Mes = FechaMesAnio[0];
            string Anio = FechaMesAnio[1];
            DateTime fechaInicioRep = new DateTime(Convert.ToInt32(Anio), MesLetraANumero(Mes), 1);
            DateTime fechaFinRep = fechaInicioRep.AddMonths(1).AddDays(-1);

            pdtInicio.DataValue = (object)fechaInicioRep;
            pdtFin.DataValue = (object)fechaFinRep;
        }

        protected int MesLetraANumero(string Mes)
        {
            switch (Mes)
            {
                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Este metodo recibe un DataTable con el listado de reportes configurados en Historicos >> Entidad: FC Configuracion de reportes  | Maestro: FC Configuracion de reportes 
        /// y por cada uno de estos elementos se genera un nuevo reporte en la pagina.
        /// </summary>
        /// <param name="RelacionReportesContenedor">Reportes configurados en historicos</param>
        protected void buscaReportes(DataTable RelacionReportesContenedor)
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
                    cargaReporte(reporte, contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, Convert.ToInt32(pestaniaActiva));
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "No se pudo cargar el reporte ''" + dr["Reporte"].ToString() + "''" + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }

        /// <summary>
        /// Este proceso invoca el metodo correspondiente al reporte que se desea cargar.
        /// </summary>
        /// <param name="reporte">Nombre del metodo que genera el reporte</param>
        /// <param name="contenedor">Control contenedor del reporte Ej. Rep0</param>
        /// <param name="tipoGrafDefault">Tipo de grafica con la que se carga el reporte por default</param>
        /// <param name="tituloGrid">Titulo de reporte tipo tabla</param>
        /// <param name="tituloGrafica">Titulo de reporte tipo grafica</param>
        /// <param name="pestaniaActiva">Pestaña que estara activa por default (Tabla o grafica) Los valores pueden ser 1 ó 2</param>
        protected void cargaReporte(string reporte, Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            /*AM La siguiente condicion se agrega para en caso de que la pestaña activa sea diferente de los valores 1 y 2 que son los unicos valores validos
             1 para la tabla y 2 para la grafica
             */
            int PestanaActiva = 0;

            if (pestaniaActiva == 1 || pestaniaActiva == 2)
            {
                PestanaActiva = pestaniaActiva;
            }
            else
            {
                PestanaActiva = 1;
            }

            switch (reporte)
            {
                case "SYOUsuariosConMasTrafico":
                    RepUsuariosMasTraficoSYO1Pnl(Request.Path + "?Nav=NavDetalleDeLlamadasEspecificoUMTSYO&UriRegistrado={0}", contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOClientesConMasTrafico":
                    RepClientesConMasTraficoSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOClientesConMenosTrafico":
                    RepClientesConMenosTraficoSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYODestinosMasMarcados":
                    RepDestinosMasMarcadosSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOUsuariosConMasUsoPorClientes":
                    RepUsoDeInfraestructuraPorUriSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOUsuariosConMenosUsoPorCliente":
                    RepUsuariosConMenosUsoPorClienteSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOServiciosPorCliente":
                    //RepServiciosPorClienteSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    RepServiciosPorClienteSYO1Pnl(Request.Path + "?Nav=NavDetalleDeLlamadasEspecificoSYO&UriRegistrado={0}", contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                case "SYOTendenciaAnual":
                    RepTendenciaAnualSYO1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;
                default:
                    break;
                // TODO : DO Paso 3 - Agregar ID de reporte
            }
        }


        #region Exportacion

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla y grafica) 1

                #region Exportar reportes con tabla y grafica

                if (Nav == "NaPruebaSeeYouOn" || Nav == "NavRepUsuariosMasTraficoSYO" || Nav == "NavClientesConMasTraficoSYO" ||
                    Nav == "NavClientesConMenosTraficoSYO" || Nav == "NavDestinosMasMarcadosSYO"
                    || Nav == "NavUsoDeInfraestructuraPorUriSYO" || Nav == "NavUsuariosConMenosUsoPorClienteSYO" || Nav == "NavTendenciaAnualSYO")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGrafico" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                    if (Nav == "NaPruebaSeeYouOn")
                    {
                        ProcesarTituloExcel(lExcel, "Reporte Consumo Por Campaña");

                        #region Reporte Prueba See You On

                        DataTable ldt = DSODataAccess.Execute(ConsultaPruebaSeeYouOn());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "SyoSourceNumber", "Total" });

                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Consumo por Campaña", new string[] { "SyoSourceNumber" }, new string[] { "SyoSourceNumber" }, new string[] { "SyoSourceNumber" }, "SyoSourceNumber", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Prueba See You On
                    }
                    if (Nav == "NavRepUsuariosMasTraficoSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Usuarios con mas trafico");

                        #region Reporte de usuarios con más trafico

                        DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosMasTraficoSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "SyoSourceNumber", "Total" });
                            ldt.Columns["SyoSourceNumber"].ColumnName = "SourceNumber";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Usuarios con mas trafico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "SourceNumber", "", "", "Total", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de usuarios con más trafico
                    }
                    if (Nav == "NavClientesConMasTraficoSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Clientes con mas trafico");

                        #region Reporte de cliente con más trafico

                        DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMasTraficoSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "Descripcion", "Segundos" });
                            ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                            ldt.Columns["Segundos"].ColumnName = "Minutos";
                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Clientes con mas trafico", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Descripcion", "", "", "Minutos", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de clientes con más trafico
                    }
                    if (Nav == "NavClientesConMenosTraficoSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Clientes con menos trafico");

                        #region Reporte de clientes con menos trafico

                        DataTable ldt = DSODataAccess.Execute(ConsultaClientesConMenosTraficoSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "Descripcion", "Segundos" });
                            ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                            ldt.Columns["Segundos"].ColumnName = "Minutos";
                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Clientes con menos trafico", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Descripcion", "", "", "Minutos", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de clientes con menos trafico
                    }
                    if (Nav == "NavDestinosMasMarcadosSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Destinos mas marcados");

                        #region Reporte de destinos más marcados

                        DataTable ldt = DSODataAccess.Execute(ConsultaDestinosMasMarcadosSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "Marcados", "NoDeMarcaciones" });
                            ldt.Columns["Marcados"].ColumnName = "DestinationNumber";
                            ldt.Columns["NoDeMarcaciones"].ColumnName = "Numero de Marcaciones";
                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Destinos mas marcados", new string[] { "Numero de Marcaciones" }, new string[] { "Numero de Marcaciones" }, new string[] { "Numero de Marcaciones" }, "DestinationNumber", "", "", "Numero de Marcaciones", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte destinos más marcados
                    }
                    if (Nav == "NavUsoDeInfraestructuraPorUriSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Usuarios con mas uso por cliente");

                        #region Reporte de usuarios con mas uso por cliente

                        DataTable ldt = DSODataAccess.Execute(ConsultaUsoDeInfraestructuraPorUriSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "URI", "TotalSegundos", "Minutos", "Horas" });

                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Usuarios con mas uso por cliente", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "URI", "", "", "URI", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de usuarios con mas uso por cliente
                    }
                    if (Nav == "NavUsuariosConMenosUsoPorClienteSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Usuarios con menos uso por cliente");

                        #region Reporte de usuarios con menos uso por cliente

                        DataTable ldt = DSODataAccess.Execute(ConsultaUsuariosConMenosUsoPorClienteSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            //ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos"});
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "URI", "TotalSegundos", "Minutos", "Horas" });

                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Usuarios con menos uso por cliente", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "URI", "", "", "URI", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de usuarios con menos uso por cliente
                    }
                    if (Nav == "NavTendenciaAnualSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Tendencia anual");

                        #region Reporte de tendencia anual

                        DataTable ldt = DSODataAccess.Execute(ConsultaTendenciaAnualSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);

                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "No", "Mes", "Anterior", "Actual" });
                            ldt.Columns["Mes"].ColumnName = "Mes";
                            ldt.Columns["Actual"].ColumnName = "Minutos";
                            ldt.AcceptChanges();
                        }

                        crearGrafico(ldt, "Tendencia anual", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Mes", "", "", "Minutos", "#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 1, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de tendencia anual
                    }
                    // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla y grafica) 2


                }

                #endregion Exportar reportes con tabla y grafica

                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla) 1

                #region Exportar Reportes solo con tabla

                if (Nav == "NavServiciosPorClienteSYO" || Nav == "NavDetalleDeLlamadasEspecificoSYO" || Nav == "NavDetalleDeLlamadasEspecificoUMTSYO")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (Nav == "NavServiciosPorClienteSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Servicios por cliente");

                        #region Reporte de servicios por cliente

                        DataTable ldt = DSODataAccess.Execute(ConsultaServiciosPorClienteSYO());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "DisplayName", "UserName", "AddressPattern", "URI", "Registro" });

                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de servicios por cliente
                    }
                    if (Nav == "NavDetalleDeLlamadasEspecificoSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Detalle de llamadas especificos");

                        #region Reporte de detalle de llamadas especificas

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleDeLlamadasDeUriSYO(UriRegistrado));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            ldt = dvRepConsumoTI.ToTable(false, new string[] {"Cliente", "SYOTime", "SYOSystemNameDesc", "SYONetworkAddress", "SYODurationSec","SYODuration","SYOSourceNumber",
                                                           "SYOSourceAddress","SYODestinationNumber","SYODestinationAddress","SYOCallTypeDesc","SYOBandwidth","SYOCauseCode",
                                                             "SYOMIBLog","SYOOwnerConference"});

                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de detalle de llamadas especificas
                    }
                    if (Nav == "NavDetalleDeLlamadasEspecificoUMTSYO")
                    {
                        ProcesarTituloExcel(lExcel, "Detalle de llamadas especificos");

                        #region Reporte de detalle de llamadas especificas de usuarios con mas trafico

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleDeLlamadasDeUriUMTSYO(UriRegistrado));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            ldt = dvRepConsumoTI.ToTable(false, new string[] {"Cliente", "SYOTime", "SYOSystemNameDesc", "SYONetworkAddress", "SYODurationSec","SYODuration","SYOSourceNumber",
                                                           "SYOSourceAddress","SYODestinationNumber","SYODestinationAddress","SYOCallTypeDesc","SYOBandwidth","SYOCauseCode",
                                                             "SYOMIBLog","SYOOwnerConference"});

                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0)
                        {
                            creaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de detalle de llamadas especificas de usuarios con mas trafico
                    }
                    // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla) 2
                }

                #endregion Exportar Reportes solo con tabla

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;


                lExcel.SalvarComo();


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes" + "_" + Nav + "_");
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

        private DataTable EliminarColumnasDeAcuerdoABanderas(DataTable Tabla)
        {
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns["CostoSimulado"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns["CostoReal"].ColumnName = "Total";
                    Tabla.Columns["SM"].ColumnName = "Servicio Medido";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    Tabla.Columns.Remove("TotalReal");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    Tabla.Columns.Remove("TotalSimulado");
                    Tabla.Columns.Remove("CostoSimulado");
                    Tabla.Columns.Remove("CostoReal");
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            return Tabla;
        }

        private static void creaTablaEnExcel(ExcelAccess lExcel, DataTable ldt, string hoja, string textoBusqueda)
        {
            object[,] datos = lExcel.DataTableToArray(FCAndControls.daFormatoACeldas(ldt), true);

            EstiloTablaExcel estilo = new EstiloTablaExcel();
            estilo.Estilo = "KeytiaGrid";
            estilo.FilaEncabezado = true;
            estilo.FilasBandas = true;
            estilo.FilaTotales = false;
            estilo.PrimeraColumna = false;
            estilo.UltimaColumna = true;
            estilo.ColumnasBandas = false;
            estilo.AutoFiltro = false;
            estilo.AutoAjustarColumnas = true;

            lExcel.Actualizar(hoja, textoBusqueda, false, datos, estilo);
        }

        /// <summary>
        /// Interpreta los metatags que se encuentren en la plantilla y los cambia por los valores que corresponde.
        /// </summary>
        /// <param name="pExcel">Objeto tipo excel</param>
        /// <param name="titulo">Titulo del reporte MetaTag {TituloReporte}</param>
        protected void ProcesarTituloExcel(ExcelAccess pExcel, string titulo)
        {
            Hashtable lhtMeta = BuscarTituloExcel(pExcel);
            string lsImg;
            int liRenLogoCliente = -1;
            int liRenLogoKeytia = -1;
            int liRenTitulo;
            int liRenglon;
            Hashtable lHTDatosImg = null;
            string lsTopLeft = null;
            string lsBottomLeft = null;
            float lfImgOffset = 0;

            string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
            string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());

            if (Session["CustomerLogo"] != null && !string.IsNullOrEmpty(Session["CustomerLogo"].ToString()) && lhtMeta["{LogoCliente}"] != null)
            {
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, Session["CustomerLogo"].ToString().Replace("~/", ""));
                lsImg = Session["CustomerLogo"].ToString().Replace("~", "").Replace("/", "\\");
                if (lsImg.StartsWith("\\"))
                {
                    lsImg = lsImg.Substring(1);
                }
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, lsImg);
                if (System.IO.File.Exists(lsImg))
                {
                    lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"], lsImg, false, false, 0, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                    if ((bool)lHTDatosImg["Inserto"])
                    {
                        lfImgOffset = (float)lHTDatosImg["Ancho"];
                        lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                        lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                        liRenLogoCliente = int.Parse(lsBottomLeft.Split(',')[0]);
                    }
                }
            }

            lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeader.png");
            if (System.IO.File.Exists(lsImg) && lhtMeta["{LogoKeytia}"] != null)
            {
                lHTDatosImg = pExcel.ReemplazaTextoPorImagen((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"], lsImg, false, false, lfImgOffset, 0, Microsoft.Office.Interop.Excel.XlPlacement.xlFreeFloating);
                if ((bool)lHTDatosImg["Inserto"])
                {
                    lsTopLeft = lHTDatosImg["TopLeft"].ToString();
                    lsBottomLeft = lHTDatosImg["BottomLeft"].ToString();
                    liRenLogoKeytia = int.Parse(lsBottomLeft.Split(',')[0]);
                }
            }

            if (lhtMeta["{LogoCliente}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoCliente}"]).set_Value(System.Type.Missing, String.Empty);

            if (lhtMeta["{LogoKeytia}"] != null)
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{LogoKeytia}"]).set_Value(System.Type.Missing, String.Empty);


            if (lhtMeta["{TituloReporte}"] != null)
            {
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, titulo);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
            if (lhtMeta["{FechasReporte}"] != null)
            {
                int piRenFechas;
                int piColFechas;

                pExcel.BuscarTexto("Reporte", "{FechasReporte}", true, out piRenFechas, out piColFechas);
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas, "Inicio:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 1, "'" + pdtInicio.Date.ToString("dd/MM/yyyy"));
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 2, "Fin:");
                pExcel.Actualizar("Reporte", piRenFechas, piColFechas + 3, "'" + pdtFin.Date.ToString("dd/MM/yyyy"));
            }
        }
        protected Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));
            lhtRet.Add("{FechasReporte}", pExcel.BuscarTexto("Reporte", "{FechasReporte}", true));

            return lhtRet;
        }

        #region Insertar grafico en excel

        protected void crearGrafico(DataTable ldt, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                             string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda,
                                             Microsoft.Office.Interop.Excel.XlChartType tipoGraf, ExcelAccess lExcel, string textoPlantilla, string HojaGrafico,
                                             string HojaDatos, float anchoGraf, float alturaGraf)
        {
            ParametrosGrafica lparametrosGraf = parametrosDeGrafica(ldt, tituloGraf, columnaDatos, leyenda, serieId, EjeX, tituloEjeX, formatoEjeX, tituloEjeY,
                                                             formatoEjeY, mostrarLeyenda);

            ProcesarGraficaExcel(HojaGrafico, HojaDatos, anchoGraf, alturaGraf, 0, 0, lparametrosGraf, lExcel, textoPlantilla, tipoGraf);
        }

        protected ParametrosGrafica parametrosDeGrafica(DataTable lsDataSource, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                                                           string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda)
        {

            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            lParametrosGrafica.Datos = lsDataSource;
            lParametrosGrafica.Title = tituloGraf;
            lParametrosGrafica.DataColumns = columnaDatos;
            lParametrosGrafica.SeriesNames = leyenda;
            lParametrosGrafica.SeriesIds = serieId;
            lParametrosGrafica.XColumn = EjeX;
            lParametrosGrafica.XIdsColumn = EjeX;
            lParametrosGrafica.XTitle = tituloEjeX;
            lParametrosGrafica.XFormat = formatoEjeX;
            lParametrosGrafica.YTitle = tituloEjeY;
            lParametrosGrafica.YFormat = formatoEjeY;
            lParametrosGrafica.ShowLegend = mostrarLeyenda;
            //lParametrosGrafica.TipoGrafica = tipoGraf;

            return lParametrosGrafica;
        }

        protected Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY,
                                                                ParametrosGrafica lParametrosGrafica, ExcelAccess lExcel, string cambiarTextoPorGraf, Microsoft.Office.Interop.Excel.XlChartType tipoGrafica)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = tipoGrafica;//GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);

            return lExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, cambiarTextoPorGraf, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns,
                     lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
        }

        #endregion

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        #endregion

    }
}
