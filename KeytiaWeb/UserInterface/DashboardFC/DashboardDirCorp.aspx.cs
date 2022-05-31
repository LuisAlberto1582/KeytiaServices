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
using System.Linq;
using System.Web.Services;


namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class DashboardDirCorp : System.Web.UI.Page
    {
        #region Variables
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        static string TituloNavegacion = string.Empty;
        string isFT = string.Empty;

        //Se almacenan los parametros que llegan en el QueryString
        static Dictionary<string, string> param = new Dictionary<string, string>();
        #endregion

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();

                LeeQueryString();

                if (!Page.IsPostBack && ValidaConsultaFechasBD())
                {
                    #region Inicia los valores default de los controles de fecha
                    try
                    {
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

                #region Fechas en sesion

                //Se revisa si las fechas almacenadas en las variables de sesion son iguales a las fechas default que debe tomar el sistema.
                if (Convert.ToDateTime(Session["FechaInicio"]) == Convert.ToDateTime(Session["FechaInicioDefault"]) &&
                    Convert.ToDateTime(Session["FechaFin"]) == Convert.ToDateTime(Session["FechaFinDefault"]))
                {
                    isFT = "1";
                }

                #endregion

                #region Dashboard

                if (param["Nav"] == string.Empty)
                {
                    DashboardMultiPerfil();
                }
                else { Navegaciones(); }

                #endregion Navegaciones

                ConfiguraNavegacion(); //NZ Siempre debe ejecutarce al final
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void DashboardMultiPerfil()
        {
            DataTable reportes = DSODataAccess.Execute(ConsultaConfiguracionDeReportesPorPerfil());
            BuscaReportes(reportes);
        }

        private void CalculaFechasDeDashboard()
        {
            DataTable fechaMaxCDR = new DataTable();
            DateTime fechaInicioDefault = new DateTime();
            DateTime fechaFinDefault = new DateTime();

            isFT = "0";

            if (DateTime.Now.Day > Convert.ToInt32(HttpContext.Current.Session["DiaLimiteMesAnt"]))
            {
                //20141204 AM. Se cambian las fechas default por la fecha maxima de detalleCDR
                fechaMaxCDR = DSODataAccess.Execute(GeneraConsultaMaxFechaInicioCDR());
            }
            else
            {
                //RJ.20160904 Calcula y obtiene el día 1 del mes previo al actual
                DateTime ultimoDiaMesAnt = new DateTime((DateTime.Now.Year), (DateTime.Now.Month), 1).AddDays(-1); //Ultimo día del mes anterior al actual

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
                    fechaInicio = new DateTime(Anio, Mes, 1);
                    fechaFinal = new DateTime(Anio, Mes, Dia);

                    //RJ.Se utilizan para saber si el usuario ha elegido unas fechas distintas a las default en los reportes
                    fechaInicioDefault = new DateTime(Anio, Mes, 1);
                    fechaFinDefault = new DateTime(Anio, Mes, Dia);

                    // Si el dia de la fecha fin es uno, se calculan las fechas inicio y fin del mes anterior.
                    if (Dia == 1)
                    {
                        fechaInicio = fechaInicio.AddMonths(-1);
                        fechaFinal = fechaFinal.AddDays(-1);

                        //RJ.Se utilizan para saber si el usuario ha elegido unas fechas distintas a las default en los reportes
                        fechaInicioDefault = fechaInicio.AddMonths(-1);
                        fechaFinDefault = fechaFinal.AddDays(-1);
                    }
                }
            }
            else
            {
                // Si en CDR no hay informacion entonces los valores de las fechas se calculan con los
                // valores default de las variables fechaInicio y fechaFinal

                // Si el dia de la fecha fin es uno, se calculan las fechas inicio y fin del mes anterior.
                if (fechaFinal.Day == 1)
                {
                    fechaInicio = fechaInicio.AddMonths(-1);
                    fechaFinal = fechaFinal.AddDays(-1);

                    fechaInicioDefault = fechaInicio.AddMonths(-1);
                    fechaFinDefault = fechaFinal.AddDays(-1);
                }
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");

            //Se revisa si las fechas almacenadas en las variables de sesion son iguales a las fechas default que debe tomar el sistema.
            if (Convert.ToDateTime(Session["FechaInicio"]) == fechaInicioDefault &&
                Convert.ToDateTime(Session["FechaFin"]) == fechaFinDefault)
            {
                isFT = "1";
            }

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
            //20150425 RJ.Se agrega condición para exportar el reporte de Capitel en formato Excel 2003
            if (param["Nav"] != "RepTabDetalleParaFinanzas")
            {
                ExportXLS(".xlsx");
            }
            else
            {
                ExportXLS(".xls");
            }
        }

        #endregion

        #region Logica de navegación

        bool ValidaConsultaFechasBD()
        {
            bool consultarBD = false;
            if (string.IsNullOrEmpty(param["Nav"]))
            {
                if (Session["ListaNavegacion"] != null &&
                    (((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else { consultarBD = true; }
            }
            else if (param["Nav"] != string.Empty && !string.IsNullOrEmpty(param["Level"]))
            {
                if (Session["ListaNavegacion"] == null)
                {
                    consultarBD = true;
                }
                else if (((List<MapNav>)Session["ListaNavegacion"]).Count == 0)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) != null)
                {
                    consultarBD = false;
                }
                else if ((((List<MapNav>)Session["ListaNavegacion"]).LastOrDefault(x => x.URL.ToLower().Contains(Request.Path.ToLower()))) == null)
                {
                    consultarBD = true;
                }
                else
                {
                    consultarBD = true;
                }
            }
            else
            {
                consultarBD = false;
            }

            return consultarBD;
        }

        void ConfiguraNavegacion()
        {
            if (param["Nav"] == string.Empty)
            {
                pnlMapaNav.Visible = false;
                pnlIndicadores.Visible = true;

                if (Session["ListaNavegacion"] != null) //Entonces ya tiene navegacion almacenada
                {
                    List<MapNav> listaNavegacion = (List<MapNav>)Session["ListaNavegacion"];
                    listaNavegacion.Clear();
                    Session["ListaNavegacion"] = listaNavegacion;
                }
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
                        listaNavegacion.Add(new MapNav() { Titulo = "Inicio", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
                    }

                    if (listaNavegacion.Exists(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString()))
                    {
                        int index = listaNavegacion.FindIndex(x => x.URL == HttpContext.Current.Request.Url.AbsoluteUri.ToString());
                        if (index == listaNavegacion.Count - 1)
                        {
                            listaNavegacion.RemoveAt(index);
                            listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() });
                        }
                        else { listaNavegacion.RemoveRange(index + 1, (listaNavegacion.Count - 1) - index); }
                    }
                    else { listaNavegacion.Add(new MapNav() { Titulo = TituloNavegacion, URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString() }); }
                }
                else
                {
                    listaNavegacion.Add(new MapNav() { Titulo = "Inicio", URL = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('?')[0] });
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

        private void LeeQueryString()
        {
            //Se inicializan todos los posibles parametros
            param.Clear();
            param.Add("Nav", string.Empty);
            param.Add("Emple", string.Empty);
            param.Add("NumMarc", string.Empty);
            param.Add("Etiqueta", string.Empty);
            param.Add("Locali", string.Empty);

            for (int i = 0; i < param.Count; i++)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[param.Keys.ElementAt(i)]))
                    {
                        param[param.Keys.ElementAt(i)] = Request.QueryString[param.Keys.ElementAt(i)];
                    }
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring " + param.Keys.ElementAt(i) + " en " + Request.Path
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
        }

        public void Navegaciones()
        {
            string NavCenCosJer = string.Empty;
            string descTipoDestino = string.Empty;
            string descSitio = string.Empty;

            string tituloReporteYGrafica = string.Empty;
            string URLSigNivelConsulta = string.Empty;
            string URLSigNivelCampoAdicional = string.Empty;

            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();
            string tituloReportePrevio = string.Empty;


            switch (param["Nav"])
            {

                //BG.20170315 id del Reporte Llamadas por Agente (Empleado)
                case "RepTabLlamadasPorEmpleado2Pnls":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas Por Empleado" });
                    RepTabLlamadasPorEmpleado2Pnls(Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple='' + convert(varchar,[idAgente])",
                                                              Rep2, "Gráfica Llamadas por Empleado", Rep1, "Llamadas por Empleado");
                    break;

                //BG.20170315 id del Reporte Minutos por Agente (Empleado)
                case "RepTabMinutosPorEmpleado2Pnls":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Minutos Por Empleado" });
                    RepTabMinutosPorEmpleado2Pnls(Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple='' + convert(varchar,[idAgente])",
                                                              Rep2, "Gráfica Minutos por Empleado", Rep1, "Minutos por Empleado");
                    break;

                //BG.20170315 id del Reporte Llamadas por Cliente (Etiqueta)
                case "RepTabLlamadasPorEtiqueta2Pnls":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Llamadas por Etiqueta" });
                    RepTabLlamadasPorEtiqueta2Pnls(Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta='' + convert(varchar,[idCliente])",
                                                              Rep2, "Gráfica Llamadas por Etiqueta", Rep1, "Llamadas por Etiqueta");
                    break;

                //BG.20170314 id del Reporte Minutos por Cliente (Etiqueta)
                case "RepTabMinutosPorEtiqueta2Pnls":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Minutos Por Etiqueta" });
                    RepTabMinutosPorEtiqueta2Pnls(Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta={0}",
                                                              "[link] = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta='' + convert(varchar,[idCliente])",
                                                              Rep2, "Gráfica Minutos por Etiqueta", Rep1, "Minutos por Etiqueta");
                    break;


                case "RepDetalleLlamsDirCorp":
                    TituloNavegacion = FCAndControls.AgregaEtiqueta(new string[] { "Reporte Detallado" });
                    ReporteDetalladoDirCorp(Rep0, "Detalle de Llamadas");

                    break;


                // TODO : DO Paso 4 - Agregar Navegacion de reporte  
                default:
                    break;
            }
        }

        #endregion

        #region Reportes

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        /* BG.20170315 Reporte Llamadas por Empleado 1 Panel*/
        private void RepTabLlamadasPorEmpleado1Pnl(Control contenedor, string tituloGrid, string tituloGrafica, int PestanaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEmpleado(
                "link = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple='' + convert(varchar,[idAgente])"));


            #region Grid


            ldt.Columns.Remove("RID");
            ldt.Columns.Remove("TopRID");
            ldt.Columns.Remove("RowNumber");
            ldt.Columns["Agente"].ColumnName = "Empleado";


            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(DTIChartsAndControls.GridView(
               "RepTabLlamadasPorEmpleado1Pnl_T", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), true, "Totales",
               new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple={0}",
               new string[] { "idAgente" }, 1, new int[] { 0, 3 },
               new int[] { 2 }, new int[] { 1 }), "RepTabLlamadasPorEmpleado1Pnl_T", tituloGrid, Request.Path + "?Nav=RepTabLlamadasPorEmpleado2Pnls", PestanaActiva, FCGpoGraf.Tabular)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Empleado", "Llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasPorEmpleadoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasPorEmpleado1Pnl_T", "", "", "Empleado", "Llamadas", PestanaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        private void RepTabLlamadasPorEmpleado2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEmpleado(linkGrafica));


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepLlamsPorEmple = new DataView(ldt);
                ldt = dvRepLlamsPorEmple.ToTable(false,
                    new string[] { "idAgente", "Agente", "Llamadas", "link" });

                ldt.Columns["Agente"].ColumnName = "Empleado";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasPorEmpleadoGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "" }, linkGrid,
                                new string[] { "idAgente" }, 1, new int[] { 0 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabLlamadasPorEmpleado2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepLlamsPorEmple = new DataView(ldt);
                ldt = dvRepLlamsPorEmple.ToTable(false, new string[] { "Empleado", "Llamadas", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamadasPorEmpleado2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasPorEmpleadoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasPorEmpleado2Pnls_G", "", "", "Empleado", "Llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        /* BG.20170315 Reporte Minutos por Empleado */
        private void RepTabMinutosPorEmpleado1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabMinutosPorEmpleado(
                "link = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple='' + convert(varchar,[idAgente])"));


            #region Grid

            ldt.Columns.Remove("RID");
            ldt.Columns.Remove("TopRID");
            ldt.Columns.Remove("RowNumber");
            ldt.Columns["Agente"].ColumnName = "Empleado";


            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(DTIChartsAndControls.GridView(
               "RepTabMinutosPorEmpleado1Pnl_T", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), true, "Totales",
               new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepDetalleLlamsDirCorp&Emple={0}",
               new string[] { "idAgente" }, 1, new int[] { 0, 3 },
               new int[] { 2 }, new int[] { 1 }), "RepTabMinutosPorEmpleado1Pnl_T", tituloGrid, Request.Path + "?Nav=RepTabMinutosPorEmpleado2Pnls", pestaniaActiva, FCGpoGraf.Tabular)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Empleado", "Minutos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabMinutosPorEmpleadoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabMinutosPorEmpleado1Pnl_T", "", "", "Empleado", "Minutos", pestaniaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        private void RepTabMinutosPorEmpleado2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabMinutosPorEmpleado(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEmple = new DataView(ldt);
                ldt = dvRepMinsPorEmple.ToTable(false,
                    new string[] { "idAgente", "Agente", "Minutos", "link" });

                ldt.Columns["Agente"].ColumnName = "Empleado";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabMinutosPorEmpleado2Pnls_T", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "" }, linkGrid,
                                new string[] { "idAgente" }, 1, new int[] { 0 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabMinutosPorEmpleado2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEmple = new DataView(ldt);
                ldt = dvRepMinsPorEmple.ToTable(false, new string[] { "Empleado", "Minutos", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabMinutosPorEmpleado2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabMinutosPorEmpleadoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabMinutosPorEmpleado2Pnls_G", "", "", "Empleado", "Llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        /* BG.20170315 Reporte Llamadas por Etiqueta */
        private void RepTabLlamadasPorEtiqueta1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEtiqueta(
                "link = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta='' + convert(varchar,[idCliente])"));


            #region Grid

            ldt.Columns.Remove("RID");
            ldt.Columns.Remove("TopRID");
            ldt.Columns.Remove("RowNumber");
            ldt.Columns["idCliente"].ColumnName = "idEtiqueta";
            ldt.Columns["Cliente"].ColumnName = "Etiqueta";


            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(DTIChartsAndControls.GridView(
               "RepTabLlamadasPorEtiqueta1Pnl_T", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), true, "Totales",
               new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta={0}",
               new string[] { "idEtiqueta" }, 1, new int[] { 0, 3 },
               new int[] { 2 }, new int[] { 1 }), "RepTabLlamadasPorEtiqueta1Pnl_T", tituloGrid, Request.Path + "?Nav=RepTabLlamadasPorEtiqueta2Pnls", pestaniaActiva, FCGpoGraf.Tabular)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Etiqueta", "Llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Etiqueta"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Etiqueta"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasPorEtiquetaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasPorEtiqueta1Pnl_T", "", "", "Etiqueta", "Llamadas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        private void RepTabLlamadasPorEtiqueta2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEtiqueta(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepLlamsPorEtiq = new DataView(ldt);
                ldt = dvRepLlamsPorEtiq.ToTable(false,
                    new string[] { "idCliente", "Cliente", "Llamadas", "link" });

                ldt.Columns["idCliente"].ColumnName = "idEtiqueta";
                ldt.Columns["Cliente"].ColumnName = "Etiqueta";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasPorEtiquetaGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "" }, linkGrid,
                                new string[] { "idEtiqueta" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabLlamadasPorEtiqueta2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepLlamsPorEtiq = new DataView(ldt);
                ldt = dvRepLlamsPorEtiq.ToTable(false, new string[] { "Etiqueta", "Llamadas", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Etiqueta"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Etiqueta"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamadasPorEtiqueta2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasPorEtiquetaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasPorEtiqueta2Pnls_G", "", "", "Etiqueta", "Llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        /* BG.20170315 Reporte Minutos por Etiqueta */
        private void RepTabMinutosPorEtiqueta1Pnl(Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabMinutosPorEtiqueta(
                "link = ''" + Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta='' + convert(varchar,[idCliente])"));


            #region Grid

            ldt.Columns.Remove("RID");
            ldt.Columns.Remove("TopRID");
            ldt.Columns.Remove("RowNumber");
            ldt.Columns["idCliente"].ColumnName = "idEtiqueta";
            ldt.Columns["Cliente"].ColumnName = "Etiqueta";

            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRep1Nvl(DTIChartsAndControls.GridView(
               "RepTabMinutosPorEtiqueta1Pnl_T", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), true, "Totales",
               new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepDetalleLlamsDirCorp&Etiqueta={0}",
               new string[] { "idEtiqueta" }, 1, new int[] { 0, 3 },
               new int[] { 2 }, new int[] { 1 }), "RepTabMinutosPorEtiqueta1Pnl_T", tituloGrid, Request.Path + "?Nav=RepTabMinutosPorEtiqueta2Pnls", pestaniaActiva, FCGpoGraf.Tabular)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Etiqueta", "Minutos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Etiqueta"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Etiqueta"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabMinutosPorEtiquetaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabMinutosPorEtiqueta1Pnl_T", "", "", "Etiqueta", "Llamadas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        private void RepTabMinutosPorEtiqueta2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabMinutosPorEtiqueta(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEtiq = new DataView(ldt);
                ldt = dvRepMinsPorEtiq.ToTable(false,
                    new string[] { "idCliente", "Cliente", "Minutos", "link" });

                ldt.Columns["idCliente"].ColumnName = "idEtiqueta";
                ldt.Columns["Cliente"].ColumnName = "Etiqueta";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabMinutosPorEtiqueta2Pnls_T", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "" }, linkGrid,
                                new string[] { "idEtiqueta" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabMinutosPorEtiqueta2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEtiq = new DataView(ldt);
                ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Etiqueta", "Minutos", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Etiqueta"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Etiqueta"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabMinutosPorEtiqueta2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabMinutosPorEtiquetaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabMinutosPorEtiqueta2Pnls_G", "", "", "Etiqueta", "Minutos", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        /* BG.20170315 Reporte Detallado de Llamadas DirCorp */
        private void ReporteDetalladoDirCorp(Control contenedor, string tituloGrid)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetalladoDirCorp", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleDirCorp", "<script language=javascript> GetDatosTabla('ReporteDetalladoDirCorp', 'ReporteDetalladoDirCorpWebM'); </script>", false);
        }

        [WebMethod]
        public static object ReporteDetalladoDirCorpWebM()
        {
            DataTable RepDetallado = DSODataAccess.Execute(ConsultaRepDetalleDirCorporativo());

            if (RepDetallado.Rows.Count > 0)
            {
                RemoveColHerencia(ref RepDetallado);

                RepDetallado.Columns["NomEmpleado"].ColumnName = "Empleado";
                RepDetallado.Columns["CategoriaDesc"].ColumnName = "Categoría";
                RepDetallado.Columns["NumMarcado"].ColumnName = "Número Marcado";

                return FCAndControls.ConvertToJSONStringDetalle(RepDetallado);
            }
            else { return null; }
        }

        //private void ReporteDetalladoDirCorp(Control contenedor, string tituloGrid)
        //{
        //    DataTable RepDetallado = DSODataAccess.Execute(ConsultaRepDetalleDirCorporativo());

        //    RepDetallado.Columns["NomEmpleado"].ColumnName = "Empleado";
        //    RepDetallado.Columns["CategoriaDesc"].ColumnName = "Categoría";
        //    RepDetallado.Columns["NumMarcado"].ColumnName = "Número Marcado";

        //    contenedor.Controls.Add(
        //        DTIChartsAndControls.tituloYBordesReporte(
        //                        DTIChartsAndControls.GridView("RepDetalladoDirCorpGrid", RepDetallado, true, "Totales",
        //                        new string[] { "", "", "", "", "{0:0,0}", "{0:0,0}" }),
        //                        "Reporte", tituloGrid, 0)
        //        );
        //}


        // TODO : DO Paso 2 - Metodo que crea reporte
        /*Aqui podemos copiar uno de los metodos que estan ya hechos y hacer los ajustes necesarios para mostrar un nuevo reporte*/

        /******************************************************************************************************************************/

        #endregion Reportes

        #region Consultas a BD

        /// <summary>
        /// Se calcula la fecha máxima que se tomará para las consultas de los reportes
        /// Si la fecha máxima encontrada en el detalle de CDR es mayor a la fecha actual, se tomará
        /// como fecha máxima el día previo al actual
        /// </summary>
        /// <returns>Consulta que regresará la fecha máxima</returns>
        public static string GeneraConsultaMaxFechaInicioCDR()
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine(" \r");
            lsbQuery.AppendLine(" declare @fechaMaximaEnDetall date ");
            lsbQuery.AppendLine("select @fechaMaximaEnDetall = convert(date,max(date01)) from Detallados where icodmaestro=89 ");
            lsbQuery.AppendLine("if @fechaMaximaEnDetall > convert(date,getdate()) ");
            lsbQuery.AppendLine("begin ");
            lsbQuery.AppendLine("	select Datepart(year, dateadd(dd,-1,getdate())) as Anio, ");
            lsbQuery.AppendLine("			Datepart(month, dateadd(dd,-1,getdate())) as Mes, ");
            lsbQuery.AppendLine("			Datepart(day, dateadd(dd,-1,getdate())) as Dia ");
            lsbQuery.AppendLine("end ");
            lsbQuery.AppendLine("else ");
            lsbQuery.AppendLine("begin ");
            lsbQuery.AppendLine("	select Datepart(year, @fechaMaximaEnDetall) as Anio, ");
            lsbQuery.AppendLine("			Datepart(month, @fechaMaximaEnDetall) as Mes, ");
            lsbQuery.AppendLine("			Datepart(day, @fechaMaximaEnDetall) as Dia ");
            lsbQuery.AppendLine("end ");

            return lsbQuery.ToString();
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

        public string ConsultaConfiguracionDeReportesPorPerfil()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("select  \n ");
            lsb.Append("[Reporte] = CveRep.Descripcion, \n ");
            lsb.Append("[Contenedor] = IDCtrl.Descripcion, \n ");
            lsb.Append("[tipoGrafDefault] = TpGraf.Descripcion, \n ");
            lsb.Append("[tituloGrid] = Cnfg.FCTituloTabla, \n ");
            lsb.Append("[tituloGrafica] = Cnfg.FCTituloGrafica, \n ");
            lsb.Append("[pestaniaActiva] = Cnfg.FCPestanaActiva \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('FCConfReportes','FC Configuracion de reportes','Español')] Cnfg \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCCveRep','FC Clave de reporte','Español')] CveRep \n ");
            lsb.Append("on CveRep.iCodCatalogo = Cnfg.FCCveRep \n ");
            lsb.Append("and CveRep.dtIniVigencia <> CveRep.dtFinVigencia  \n ");
            lsb.Append("and CveRep.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCIDControl','FC ID de control','Español')] IDCtrl \n ");
            lsb.Append("on IDCtrl.iCodCatalogo = Cnfg.FCIDControl \n ");
            lsb.Append("and IDCtrl.dtIniVigencia <> IDCtrl.dtFinVigencia  \n ");
            lsb.Append("and IDCtrl.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCTipoGraf','FC Tipo de Grafica','Español')] TpGraf \n ");
            lsb.Append("on TpGraf.iCodCatalogo = Cnfg.FCTipoGraf \n ");
            lsb.Append("and TpGraf.dtIniVigencia <> TpGraf.dtFinVigencia  \n ");
            lsb.Append("and TpGraf.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("Join " + DSODataContext.Schema + ".[VisHistoricos('FCDashboard','FC Dashboard','Español')] Dash \n ");
            lsb.Append("on Dash.iCodCatalogo = Cnfg.FCDashboard \n ");
            lsb.Append("and Dash.dtIniVigencia <> Dash.dtFinVigencia  \n ");
            lsb.Append("and Dash.dtFinVigencia >= GETDATE() \n ");
            lsb.Append(" \n ");
            lsb.Append("where Cnfg.dtIniVigencia <> Cnfg.dtFinVigencia  \n ");
            lsb.Append("and Cnfg.dtFinVigencia >= GETDATE() \n ");
            lsb.Append("and Cnfg.Perfil = " + Session["iCodPerfil"] + " \n ");
            lsb.Append("and Dash.Descripcion = 'DashboardDirCorp.aspx' \n ");
            return lsb.ToString();
        }

        public string ConsultaRepPorEmpleado(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \n ");
            lsb.Append("declare @ParamEmple varchar(max) \n ");
            lsb.Append("declare @ParamSitio varchar(max) \n ");
            lsb.Append("declare @ParamCenCos varchar(max) \n ");
            lsb.Append("declare @ParamCarrier varchar(max) \n ");
            lsb.Append("declare @ParamExtension varchar(max) \n ");
            lsb.Append("declare @ParamCodAut varchar(max) \n ");
            lsb.Append("declare @ParamLocali varchar(max) \n ");
            lsb.Append("declare @ParamTelDest varchar(max) \n ");
            lsb.Append("declare @ParamGEtiqueta varchar(max) \n ");
            lsb.Append("declare @ParamTDest varchar(max) \n ");
            lsb.Append("set @ParamEmple = 'null' \n ");
            lsb.Append("set @ParamSitio = 'null' \n ");
            lsb.Append("set @ParamCenCos = 'null' \n ");
            lsb.Append("set @ParamCarrier = 'null' \n ");
            lsb.Append("set @ParamExtension = 'null' \n ");
            lsb.Append("set @ParamCodAut = 'null' \n ");

            if (param["Locali"] != string.Empty)
            {
                lsb.Append("set @ParamLocali = '" + param["Locali"] + "' \n ");
            }
            else
            {
                lsb.Append("set @ParamLocali = 'null' \n ");
            }
            if (param["NumMarc"] != string.Empty)
            {
                lsb.Append("set @ParamTelDest = '''" + param["NumMarc"] + "''' \n ");
            }
            else
            {
                lsb.Append("set @ParamTelDest = 'null' \n ");
            }

            lsb.Append("set @ParamGEtiqueta = 'null' \n ");
            lsb.Append("set @ParamTDest = 'null' \n ");
            lsb.Append("set @Where = 'FechaInicio >= ''" + Session["FechaInicio"].ToString() + " 00:00:00''  \n ");
            lsb.Append("and FechaInicio <= ''" + Session["FechaFin"].ToString() + " 23:59:59'' \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamEmple <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamSitio <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Sitio] in('+@ParamSitio+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamCenCos <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamCarrier <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Carrier] in('+@ParamCarrier+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamExtension <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Extension] in('+@ParamExtension+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamCodAut <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Autorizacion] in('+@ParamCodAut+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamLocali <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Localidad] in('+@ParamLocali+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamTelDest <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Numero Marcado] in('+@ParamTelDest+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamGEtiqueta <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Clave Tipo Llamada] in('+@ParamGEtiqueta+') \n ");
            lsb.Append("' \n ");
            lsb.Append("if @ParamTDest <> 'null' \n ");
            lsb.Append("      set @Where = @Where + 'And [Codigo Tipo Destino] in('+@ParamTDest+') \n ");
            lsb.Append("' \n ");
            lsb.Append("exec ConsumoDetalladoTodosCamposCDRRest    @Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("			                           @Fields='[Codigo Empleado],[Codigo Centro de Costos],[Codigo Sitio], [Nombre Completo]=Min(upper([Nombre Completo])), \n ");
            lsb.Append("												[Nombre Centro de Costos]=Min(upper([Nombre Centro de Costos])), \n ");
            lsb.Append("												[Nombre Sitio]=Min(upper([Nombre Sitio])), \n ");
            lsb.Append("												[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \n ");
            lsb.Append("												[Numero]=count(*), \n ");
            lsb.Append("												[Duracion]=sum([Duracion Minutos]) \n ");

            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                lsb.Append("                                            ,[Puesto] \n ");
            }

            lsb.Append("',  \n ");
            lsb.Append("			                           @Where = @Where, \n ");
            lsb.Append("			                           @Group = '[Codigo Empleado], \n ");
            lsb.Append("												 [Codigo Centro de Costos], \n ");

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" /* || DSODataContext.Schema.ToLower() == "evox" */)
            {
                lsb.Append("                                             [Codigo Sitio],  \n ");
                lsb.Append("                                             [Puesto]',  \n ");
            }
            else
            {
                lsb.Append("                                             [Codigo Sitio]',  \n ");
            }

            lsb.Append("			                           @Order = '[Nombre Completo] Asc,[Nombre Centro de Costos] Asc,[Nombre Sitio] Asc,[Total] Asc,[Duracion] Asc,[Numero] Asc', \n ");
            lsb.Append("			                           @OrderInv = '[Nombre Completo] Desc,[Nombre Centro de Costos] Desc,[Nombre Sitio] Desc,[Total] Desc,[Duracion] Desc,[Numero] Desc', \n ");
            lsb.Append("			                           @OrderDir = 'Asc', \n ");
            lsb.Append("			                  @Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("			                  @Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("			                           @FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("			                           @FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        //BG.20170314 Reporte de Llamadas Por Agente, Agente = Empleado
        public string ConsultaRepTabLlamadasPorEmpleado(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabLlamadasPorAgente \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idAgente],[Agente],[Llamadas] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        //BG.20170314 Reporte de Minutos Por Agente, Agente = Empleado
        public string ConsultaRepTabMinutosPorEmpleado(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabMinutosPorAgente \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idAgente],[Agente],[Minutos] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        //BG.20170314 Reporte de Llamadas Por Cliente , Cliente = Etiqueta
        public string ConsultaRepTabLlamadasPorEtiqueta(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabLlamadasPorCliente \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idCliente],[Cliente],[Llamadas] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        //BG.20170314 Reporte de Minutos Por Cliente , Cliente = Etiqueta
        public string ConsultaRepTabMinutosPorEtiqueta(string linkGrafica)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabMinutosPorCliente \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[idCliente],[Cliente],[Minutos] \n ");
            if (linkGrafica != "")
            {
                lsb.Append("," + linkGrafica + " \n ");
            }
            lsb.Append("',  \n ");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }

        //BG.20170315 Reporte Detallado Directorio Corporativo
        public static string ConsultaRepDetalleDirCorporativo()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabDetalleDirCorpConFiltro \n ");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \n ");
            lsb.Append("@Fields='[NomEmpleado],[Etiqueta],[CategoriaDesc],[NumMarcado],[Llamadas],[Minutos]', \n ");
            lsb.Append("@Empleado = " + (string.IsNullOrEmpty(param["Emple"]) ? "0" : param["Emple"]) + ",\n ");
            lsb.Append("@Etiqueta = " + (string.IsNullOrEmpty(param["Etiqueta"]) ? "''" : "'" + param["Etiqueta"] + "'") + ",\n ");
            lsb.Append("@Usuario = " + HttpContext.Current.Session["iCodUsuario"] + ",\n ");
            lsb.Append("@Perfil = " + HttpContext.Current.Session["iCodPerfil"] + ",\n ");
            lsb.Append("@FechaIniRep = '''" + HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00''', \n ");
            lsb.Append("@FechaFinRep = '''" + HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59''', \n ");
            lsb.Append("@Moneda = '" + HttpContext.Current.Session["Currency"] + "', \n ");
            lsb.Append("@Idioma = 'Español' \n ");
            return lsb.ToString();
        }


        //// TODO : DO Paso 1 - Consulta del reporte

        #endregion Consultas a BD

        /// <summary>
        /// Este metodo recibe un DataTable con el listado de reportes configurados en Historicos >> Entidad: FC Configuracion de reportes  | Maestro: FC Configuracion de reportes 
        /// y por cada uno de estos elementos se genera un nuevo reporte en la pagina.
        /// </summary>
        /// <param name="RelacionReportesContenedor">Reportes configurados en historicos</param>
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

        /// <summary>
        /// Este proceso invoca el metodo correspondiente al reporte que se desea cargar.
        /// </summary>
        /// <param name="reporte">Nombre del metodo que genera el reporte</param>
        /// <param name="contenedor">Control contenedor del reporte Ej. Rep0</param>
        /// <param name="tipoGrafDefault">Tipo de grafica con la que se carga el reporte por default</param>
        /// <param name="tituloGrid">Titulo de reporte tipo tabla</param>
        /// <param name="tituloGrafica">Titulo de reporte tipo grafica</param>
        /// <param name="pestaniaActiva">Pestaña que estara activa por default (Tabla o grafica) Los valores pueden ser 1 ó 2</param>
        protected void CargaReporte(string reporte, Control contenedor, string tipoGrafDefault, string tituloGrid, string tituloGrafica, int pestaniaActiva)
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
                //BG.20170314 id del Reporte Llamadas por Agente (Empleado)
                case "RepTabLlamadasPorEmpleado1Pnl":
                    RepTabLlamadasPorEmpleado1Pnl(contenedor, tituloGrid, tituloGrafica, PestanaActiva);
                    break;

                //BG.20170315 id del Reporte Minutos por Agente (Empleado)
                case "RepTabMinutosPorEmpleado1Pnl":
                    RepTabMinutosPorEmpleado1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;

                //BG.20170315 id del Reporte Llamadas por Cliente (Etiqueta)
                case "RepTabLlamadasPorEtiqueta1Pnl":
                    RepTabLlamadasPorEtiqueta1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;

                //BG.20170315 id del Reporte Minutos por Cliente (Etiqueta)
                case "RepTabMinutosPorEtiqueta1Pnl":
                    RepTabMinutosPorEtiqueta1Pnl(contenedor, tipoGrafDefault, tituloGrid, tituloGrafica, PestanaActiva);
                    break;

                case "RepDetalleLlamsDirCorp":
                    ReporteDetalladoDirCorp(Rep0, "Reporte Detallado");
                    break;


                // TODO : DO Paso 3 - Agregar ID de reporte

                default:
                    break;
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

                if (param["Nav"] == "RepTabLlamadasPorEmpleado2Pnls" || param["Nav"] == "RepTabMinutosPorEmpleado2Pnls" ||
                    param["Nav"] == "RepTabLlamadasPorEtiqueta2Pnls" || param["Nav"] == "RepTabMinutosPorEtiqueta2Pnls"
                    )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGrafico" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                    if (param["Nav"] == "RepTabLlamadasPorEmpleado2Pnls")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Llamadas Por Empleado");

                        #region Reporte Llamadas Por Empleado

                        DataTable GrafLlamsPorEmple = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEmpleado(""));

                        if (GrafLlamsPorEmple.Rows.Count > 0 && GrafLlamsPorEmple.Columns.Count > 0)
                        {
                            DataView dvGrafLlamsPorEmple = new DataView(GrafLlamsPorEmple);
                            GrafLlamsPorEmple = dvGrafLlamsPorEmple.ToTable(false, new string[] { "Agente", "Llamadas" });
                            GrafLlamsPorEmple.Columns["Agente"].ColumnName = "Empleado";

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafLlamsPorEmple, "Llamadas desc", 10), "Llamadas por Empleado", new string[] { "Llamadas" }, new string[] { "Llamadas" },
                            new string[] { "Total" }, "Empleado", "Empleado", "", "Total", "{0:0,0}",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafLlamsPorEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafLlamsPorEmple, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas Por Empleado
                    }
                    else if (param["Nav"] == "RepTabMinutosPorEmpleado2Pnls")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Minutos Por Empleado");

                        #region Reporte Minutos Por Empleado

                        DataTable GrafMinsPorEmple = DSODataAccess.Execute(ConsultaRepTabMinutosPorEmpleado(""));

                        if (GrafMinsPorEmple.Rows.Count > 0 && GrafMinsPorEmple.Columns.Count > 0)
                        {
                            DataView dvGrafMinsPorEmple = new DataView(GrafMinsPorEmple);
                            GrafMinsPorEmple = dvGrafMinsPorEmple.ToTable(false, new string[] { "Agente", "Minutos" });
                            GrafMinsPorEmple.Columns["Agente"].ColumnName = "Empleado";

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafMinsPorEmple, "Minutos desc", 10), "Minutos por Empleado", new string[] { "Minutos" }, new string[] { "Minutos" },
                            new string[] { "Total" }, "Empleado", "Empleado", "", "Total", "{0:0,0}",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafMinsPorEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafMinsPorEmple, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Minutos Por Empleado
                    }
                    else if (param["Nav"] == "RepTabLlamadasPorEtiqueta2Pnls")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Llamadas Por Etiqueta");

                        #region Reporte Llamadas Por Etiqueta

                        DataTable GrafLlamsPorEtiq = DSODataAccess.Execute(ConsultaRepTabLlamadasPorEtiqueta(""));

                        if (GrafLlamsPorEtiq.Rows.Count > 0 && GrafLlamsPorEtiq.Columns.Count > 0)
                        {
                            DataView dvGrafLlamsPorEtiq = new DataView(GrafLlamsPorEtiq);
                            GrafLlamsPorEtiq = dvGrafLlamsPorEtiq.ToTable(false, new string[] { "Cliente", "Llamadas" });
                            GrafLlamsPorEtiq.Columns["Cliente"].ColumnName = "Etiqueta";

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafLlamsPorEtiq, "Llamadas desc", 10), "Llamadas por Etiqueta", new string[] { "Llamadas" }, new string[] { "Llamadas" },
                            new string[] { "Total" }, "Etiqueta", "Etiqueta", "", "Total", "{0:0,0}",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafLlamsPorEtiq.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafLlamsPorEtiq, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas Por Etiqueta
                    }
                    else if (param["Nav"] == "RepTabMinutosPorEtiqueta2Pnls")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Minutos Por Etiqueta");

                        #region Reporte Minutos Por Etiqueta

                        DataTable GrafMinsPorEtiq = DSODataAccess.Execute(ConsultaRepTabMinutosPorEtiqueta(""));

                        if (GrafMinsPorEtiq.Rows.Count > 0 && GrafMinsPorEtiq.Columns.Count > 0)
                        {
                            DataView dvGrafMinsPorEtiq = new DataView(GrafMinsPorEtiq);
                            GrafMinsPorEtiq = dvGrafMinsPorEtiq.ToTable(false, new string[] { "Cliente", "Minutos" });
                            GrafMinsPorEtiq.Columns["Cliente"].ColumnName = "Etiqueta";

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafMinsPorEtiq, "Minutos desc", 10), "Minutos por Etiqueta", new string[] { "Minutos" }, new string[] { "Minutos" },
                            new string[] { "Total" }, "Etiqueta", "Etiqueta", "", "Total", "{0:0,0}",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafMinsPorEtiq.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafMinsPorEtiq, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Minutos Por Etiqueta
                    }

                    // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla y grafica) 2

                }

                #endregion Exportar reportes con tabla y grafica


                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla) 1


                #region Exportar Reportes solo con tabla


                //BG.20170315 Reporte Detallado DirCorp
                if (param["Nav"] == "RepDetalleLlamsDirCorp")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "RepDetalleLlamsDirCorp")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalleDirCorporativo());

                        //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "NomEmpleado", "Etiqueta", "CategoriaDesc", "NumMarcado", "Llamadas", "Minutos" });
                            ldt.Columns["NomEmpleado"].ColumnName = "Empleado";
                            ldt.Columns["CategoriaDesc"].ColumnName = "Categoría";
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.AcceptChanges();
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                    }

                    //TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla) 2
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

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes" + "_" + param["Nav"] + "_");
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
