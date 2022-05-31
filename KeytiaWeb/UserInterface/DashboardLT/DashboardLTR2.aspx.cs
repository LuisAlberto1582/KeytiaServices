using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaWeb.UserInterface.DashboardLT;
using System.Text;
using KeytiaServiceBL;
using System.Data;

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public partial class DashboardLTR2 : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        /*paginaLocal se usa para mandar el nombre de la clase donde se produjo el error en el log de web*/
        string paginaLocal = "KeytiaWeb.UserInterface.DashboardLT.DashboardLTR2.aspx.cs";
        /*nombrePagina se usa para poder cambiarle el nombre a la pagina una ves que se pasen cambios a producción*/
        string nombrePagina = "DashboardLTR2.aspx";

        #region Variables que guardan valores de queryString

        int ReporteConsumoHistorico = 0;
        int ReportePorEmpleado = 0;
        int ReportePorEmpleadoNiv2 = 0;
        int ReportePorEmpleadoNiv3 = 0;
        int ReportePorEmpleadoNiv4 = 0;
        int ReportePorEmpleadoNiv5 = 0;
        int ReportePorEmpleadoNiv6 = 0;

        int ReportePorCenCos = 0;
        int ReportePorCenCosNiv2 = 0;
        int ReportePorCenCosNiv3 = 0;
        int ReportePorCenCosNiv4 = 0;
        int ReportePorCenCosNiv5 = 0;
        int ReportePorCenCosNiv6 = 0;

        int iCodEmple = 0;
        int iCodCenCos = 0;
        string NumeroMovil = string.Empty;
        string NumeroMarcado = string.Empty;

        string Etiqueta = string.Empty;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
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
            if (!list.Exists(element => element == HttpUtility.UrlDecode(lsURL)))
            {
                //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                list.Add(HttpUtility.UrlDecode(lsURL));
            }

            //Guardar en sesion la nueva lista
            Session["pltNavegacion"] = list;

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

            #region Lee Query String

            #region Revisar si el querystring ReporteConsumoHistorico contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReporteConsumoHistorico"]))
            {
                try
                {
                    ReporteConsumoHistorico = Convert.ToInt32(Request.QueryString["ReporteConsumoHistorico"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReporteConsumoHistorico) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReporteConsumoHistorico = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleado contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleado"]))
            {
                try
                {
                    ReportePorEmpleado = Convert.ToInt32(Request.QueryString["ReportePorEmpleado"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleado) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleado = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleadoNiv2 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleadoNiv2"]))
            {
                try
                {
                    ReportePorEmpleadoNiv2 = Convert.ToInt32(Request.QueryString["ReportePorEmpleadoNiv2"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleadoNiv2) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleadoNiv2 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleadoNiv3 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleadoNiv3"]))
            {
                try
                {
                    ReportePorEmpleadoNiv3 = Convert.ToInt32(Request.QueryString["ReportePorEmpleadoNiv3"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleadoNiv3) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleadoNiv3 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleadoNiv4 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleadoNiv4"]))
            {
                try
                {
                    ReportePorEmpleadoNiv4 = Convert.ToInt32(Request.QueryString["ReportePorEmpleadoNiv4"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleadoNiv4) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleadoNiv4 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleadoNiv5 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleadoNiv5"]))
            {
                try
                {
                    ReportePorEmpleadoNiv5 = Convert.ToInt32(Request.QueryString["ReportePorEmpleadoNiv5"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleadoNiv5) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleadoNiv5 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorEmpleadoNiv6 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorEmpleadoNiv6"]))
            {
                try
                {
                    ReportePorEmpleadoNiv6 = Convert.ToInt32(Request.QueryString["ReportePorEmpleadoNiv6"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorEmpleadoNiv6) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorEmpleadoNiv6 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCos contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCos"]))
            {
                try
                {
                    ReportePorCenCos = Convert.ToInt32(Request.QueryString["ReportePorCenCos"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCos) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCos = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCosNiv2 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCosNiv2"]))
            {
                try
                {
                    ReportePorCenCosNiv2 = Convert.ToInt32(Request.QueryString["ReportePorCenCosNiv2"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCosNiv2) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCosNiv2 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCosNiv3 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCosNiv3"]))
            {
                try
                {
                    ReportePorCenCosNiv3 = Convert.ToInt32(Request.QueryString["ReportePorCenCosNiv3"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCosNiv3) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCosNiv3 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCosNiv4 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCosNiv4"]))
            {
                try
                {
                    ReportePorCenCosNiv4 = Convert.ToInt32(Request.QueryString["ReportePorCenCosNiv4"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCosNiv4) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCosNiv4 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCosNiv5 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCosNiv5"]))
            {
                try
                {
                    ReportePorCenCosNiv5 = Convert.ToInt32(Request.QueryString["ReportePorCenCosNiv5"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCosNiv5) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCosNiv5 = 0;
            }

            #endregion

            #region Revisar si el querystring ReportePorCenCosNiv6 contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["ReportePorCenCosNiv6"]))
            {
                try
                {
                    ReportePorCenCosNiv6 = Convert.ToInt32(Request.QueryString["ReportePorCenCosNiv6"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReportePorCenCosNiv6) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReportePorCenCosNiv6 = 0;
            }

            #endregion

            #region Revisar si el querystring iCodCenCos contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["iCodCenCos"]))
            {
                try
                {
                    iCodCenCos = Convert.ToInt32(Request.QueryString["iCodCenCos"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (iCodCenCos) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodCenCos = 0;
            }

            #endregion

            #region Revisar si el querystring iCodEmple contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["iCodEmple"]))
            {
                try
                {
                    iCodEmple = Convert.ToInt32(Request.QueryString["iCodEmple"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (iCodEmple) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodEmple = 0;
            }

            #endregion

            #region Revisar si el querystring NumeroMovil contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["NumeroMovil"]))
            {
                try
                {
                    NumeroMovil = Request.QueryString["NumeroMovil"].ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (NumeroMovil) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                NumeroMovil = string.Empty;
            }

            #endregion

            #region Revisar si el querystring NumeroMarcado contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["NumeroMarcado"]))
            {
                try
                {
                    NumeroMarcado = Request.QueryString["NumeroMarcado"].ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (NumeroMarcado) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                NumeroMarcado = string.Empty;
            }

            #endregion

            #region Revisar si el querystring Etiqueta contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["Etiqueta"]))
            {
                try
                {
                    Etiqueta = Request.QueryString["Etiqueta"].ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (Etiqueta) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                Etiqueta = string.Empty;
            }

            #endregion

            #endregion

            #region Etiqueta de navegacion

            if (Etiqueta.Length > 0) //Entonces ya tiene navegacion almacenada
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion = (List<string>)Session["etiquetaNavegacion"];
                if (!listEtiquetaNavegacion.Contains(Etiqueta))
                {
                    listEtiquetaNavegacion.Add(Etiqueta);
                }
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.ToArray());
            }
            else
            {
                List<string> listEtiquetaNavegacion = new List<string>();
                listEtiquetaNavegacion.Add("Inicio");
                Session["etiquetaNavegacion"] = listEtiquetaNavegacion;
                lblInicio.Text = string.Join(" / ", listEtiquetaNavegacion.ToArray());
            }

            #endregion

            if (!Page.IsPostBack)
            {
                //20140423 AM. Se cambia el nombre de las variables de sesion de las fechas
                #region Inicia los valores default de los controles de fecha
                try
                {
                    if (Session["FechaInicioRepDashLT"] != null && Session["FechaFinRepDashLT"] != null)
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRepDashLT"].ToString().Substring(1, 10));
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashLT"].ToString().Substring(1, 10));
                    }

                    else
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)fechaInicio;
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)fechaFinal;
                    }

                    Session["FechaInicioRepDashLT"] = pdtInicio.DataValue.ToString();
                    Session["FechaFinRepDashLT"] = pdtFin.DataValue.ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al darle valores default a los campos de fecha en KeytiaWeb.UserInterface.DashboardLT.DashboardLT.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
                #endregion //Fin de bloque --Inicia los valores default de los controles de fecha
            }

            #region Fechas en sesion

            Session["FechaInicioRepDashLT"] = pdtInicio.DataValue.ToString();
            Session["FechaFinRepDashLT"] = pdtFin.DataValue.ToString();

            if (Session["FechaInicioRepDashLT"] != null && Session["FechaFinRepDashLT"] != null)
            {
                pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRepDashLT"].ToString().Substring(1, 10));
                pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashLT"].ToString().Substring(1, 10));
            }

            #endregion

            #region Se crea una tabla que contendra los reportes

            Table tblReportes = new Table();
            tblReportes.Width = Unit.Percentage(100);

            TableRow tblReportestr1 = new TableRow();

            TableCell tblReportestr1tc1 = new TableCell();
            tblReportestr1tc1.ColumnSpan = 2;
            tblReportestr1tc1.Width = Unit.Percentage(100);

            tblReportestr1.Controls.Add(tblReportestr1tc1);

            TableRow tblReportestr2 = new TableRow();

            TableCell tblReportestr2tc1 = new TableCell();
            tblReportestr2tc1.Width = Unit.Percentage(50);

            TableCell tblReportestr2tc2 = new TableCell();
            tblReportestr2tc2.Width = Unit.Percentage(50);

            tblReportestr2.Controls.Add(tblReportestr2tc1);
            tblReportestr2.Controls.Add(tblReportestr2tc2);

            TableRow tblReportestr3 = new TableRow();

            TableCell tblReportestr3tc1 = new TableCell();
            tblReportestr3tc1.Width = Unit.Percentage(50);

            TableCell tblReportestr3tc2 = new TableCell();
            tblReportestr3tc2.Width = Unit.Percentage(50);

            tblReportestr3.Controls.Add(tblReportestr3tc1);
            tblReportestr3.Controls.Add(tblReportestr3tc2);

            tblReportes.Controls.Add(tblReportestr1);
            tblReportes.Controls.Add(tblReportestr2);
            tblReportes.Controls.Add(tblReportestr3);

            #endregion

            if (ReporteConsumoHistorico == 1)
            {
                #region GridView y Grafica Historica

                DataTable ldt = DSODataAccess.Execute(ConsultaHistoricoDashboardPrincipal());

                ldt.Columns.Remove("Orden");

                tblReportestr2tc1.Controls.Add(
                                        DTIChartsAndControls.tituloYBordesReporte(
                                                        DTIChartsAndControls.GridView("grvRepHisto", ldt, true, "Totales",  new string[] { "", "{0:c}" }),
                                                                                                  "Reporte", "Reporte de consumo histórico", 0));

                //20140423 AM. Se agrega condicion para eliminar la ultima fila solo si el datatable contiene filas
                if (ldt.Rows.Count > 0)
                {
                    ldt.Rows.RemoveAt(ldt.Rows.Count - 1);
                }

                tblReportestr2tc2.Controls.Add(
                            DTIChartsAndControls.tituloYBordesReporte(
                                                    DTIChartsAndControls.GraficaBarras(ldt, "Mes", "Importe", "", 400, 600, "", "", "Importe", "c2", "grafRepHisto"),
                                                                                                    "Gráfica", "Gráfica de consumo histórico", 0));

                #endregion
            }

            #region Reportes por empleado y navegaciones

            if (ReportePorEmpleado == 1)
            {
                #region Empleados mas caros

                DataTable ldt = DSODataAccess.Execute(ConsultaEmpleadosMasCaros());

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvEmpleNiv1", ldt, true, "Totales", new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodEmple={0}&iCodCenCos={1}&ReportePorEmpleadoNiv2=1&Etiqueta={2}",
                                        new string[] { "Codigo Empleado", "Codigo Centro de Costos", "Empleado" }, 0, new int[] { 1, 3 }, new int[] { 2, 4, 5, 6 }, new int[] { 0 }),
                                        "Reporte", "Empleados más caros", 0));


                #endregion
            }

            if (ReportePorEmpleadoNiv2 == 1)
            {
                #region Empleados mas caros nivel 2

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel2ReportePorEmpleado(iCodEmple, iCodCenCos));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvEmpleNiv2", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:c}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodEmple=" + iCodEmple + "&iCodCenCos=" +
                                        iCodCenCos + "&ReportePorEmpleadoNiv3=1&Etiqueta={1}",
                                        new string[] { "Codigo Sitio", "Sitio" }, 0, new int[] { 1 }, new int[] { 2, 3 }, new int[] { 0 }), "Reporte", "", 0));


                #endregion
            }

            if (ReportePorEmpleadoNiv3 == 1)
            {
                #region Empleados mas caros nivel 3

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel3ReportePorEmpleado(iCodEmple, iCodCenCos));

                tblReportestr1tc1.Controls.Add(
                         DTIChartsAndControls.tituloYBordesReporte(
                                         DTIChartsAndControls.GridView("grvEmpleNiv3", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}" },
                                         "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorEmpleadoNiv4=1&NumeroMovil={1}&Etiqueta={1}",
                                         new string[] { "intFake", "Móvil" }, 1, new int[] { 0 }, new int[] { 2, 3 }, new int[] { 1 }), "Reporte", "", 0));


                #endregion
            }

            if (ReportePorEmpleadoNiv4 == 1)
            {
                #region Empleados mas caros nivel 4

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel4ReportePorEmpleado(NumeroMovil));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvEmpleNiv4", ldt, true, "Totales",
                                        new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", 
                                                           "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}",
                                                           "{0:c}", "{0:c}", "{0:c}"},
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorEmpleadoNiv5=1&NumeroMovil={0}&Etiqueta={0}",
                                        new string[] { "Linea" }, 0, new int[] { 5 }, new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 },
                                        new int[] { 0 }), "Reporte", "", 0));
                #endregion
            }

            if (ReportePorEmpleadoNiv5 == 1)
            {
                #region Empleados mas caros nivel 5

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel5ReportePorEmpleado(NumeroMovil));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvEmpleNiv5", ldt, true, "Totales", new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorEmpleadoNiv6=1&NumeroMovil=" + NumeroMovil + "&NumeroMarcado={0}&Etiqueta={0}",
                                        new string[] { "Numero marcado" }, 0, new int[] { 2 }, new int[] { 1, 3, 4, 5 }, new int[] { 0 }), "Reporte", "", 0));


                #endregion
            }

            if (ReportePorEmpleadoNiv6 == 1)
            {
                #region Empleados mas caros nivel 6

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel6ReportePorEmpleado(NumeroMovil, NumeroMarcado));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvEmpleNiv6", ldt, true, "Totales", new string[] { "", "", "{0:0,0}", "{0:c}", "", "", "" }), "Reporte", "", 0));

                #endregion
            }

            #endregion

            #region Reportes por CenCos y navegaciones

            if (ReportePorCenCos == 1)
            {
                #region CenCos mas caros

                DataTable ldt = DSODataAccess.Execute(ConsultaCenCosMasCaros());

                tblReportestr2tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvCenCosNiv1", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodCenCos={0}&ReportePorCenCosNiv2=1&Etiqueta={1}",
                                        new string[] { "Codigo Centro de Costos", "Centro de Costos" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                        "Reporte", "Consumo por centros de costos", 0));

                //20140423 AM. Se agrega condicion para eliminar la ultima fila solo si el datatable contiene filas
                if (ldt.Rows.Count > 0)
                {
                    ldt.Rows.RemoveAt(ldt.Rows.Count - 1);
                }

                tblReportestr2tc2.Controls.Add(
                            DTIChartsAndControls.tituloYBordesReporte(
                                                    DTIChartsAndControls.GraficaPie(ldt, "Centro de costos", "Total", "", 400, 600, "", "", "", "grafCenCosNiv1"),
                                                    "Gráfica", "Consumo por centro de costos", 0));

                #endregion
            }

            if (ReportePorCenCosNiv2 == 1)
            {
                #region Centro de costos nivel 2

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel2ReportePorCenCos(iCodCenCos));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvCenCosNiv2", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodEmple={0}&ReportePorCenCosNiv3=1&Etiqueta={1}",
                                        new string[] { "Codigo Empleado", "Empleado" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "Reporte", "", 0));

                #endregion
            }

            if (ReportePorCenCosNiv3 == 1)
            {
                #region Centro de costos nivel 3

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel3ReportePorCenCos(iCodCenCos, iCodEmple));

                tblReportestr1tc1.Controls.Add(
                       DTIChartsAndControls.tituloYBordesReporte(
                                       DTIChartsAndControls.GridView("grvCenCosNiv3", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}" },
                                       "~/UserInterface/DashboardLT/" + nombrePagina + "?NumeroMovil={0}&ReportePorCenCosNiv4=1&Etiqueta={0}",
                                       new string[] { "Movil" }, 1, new int[] { 0 }, new int[] { 2, 3 }, new int[] { 1 }), "Reporte", "", 0));

                #endregion
            }

            if (ReportePorCenCosNiv4 == 1)
            {
                #region Centro de costos nivel 4

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel4ReportePorCenCos(NumeroMovil));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvCenCosNiv4", ldt, true, "Totales",
                                        new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", 
                                                                           "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}",
                                                                           "{0:c}", "{0:c}", "{0:c}"},
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorCenCosNiv5=1&NumeroMovil={0}&Etiqueta={0}",
                                        new string[] { "Linea" }, 0, new int[] { 5 }, new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 },
                                        new int[] { 0 }), "Reporte", "", 0));

                #endregion
            }

            if (ReportePorCenCosNiv5 == 1)
            {
                #region Centro de costos nivel 5

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel5ReportePorCenCos(NumeroMovil));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvCenCosNiv5", ldt, true, "Totales", new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorCenCosNiv6=1&NumeroMovil=" + NumeroMovil + "&NumeroMarcado={0}&Etiqueta={0}",
                                        new string[] { "Numero marcado" }, 0, new int[] { 2 }, new int[] { 1, 3, 4, 5 }, new int[] { 0 }), "Reporte", "", 0));

                #endregion
            }

            if (ReportePorCenCosNiv6 == 1)
            {
                #region Centro de costos nivel 6

                DataTable ldt = DSODataAccess.Execute(ConsultaNivel6ReportePorCenCos(NumeroMovil, NumeroMarcado));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvCenCosNiv6", ldt, true, "Totales", new string[] { "", "", "{0:0,0}", "{0:c}", "", "", "" }), "Reporte", "", 0));

                #endregion
            }

            #endregion

            if (VerDashboard())
            {
                #region Reportes de dashboard

                #region Grafica Historica

                DataTable ldtRepConsHist = DSODataAccess.Execute(ConsultaHistoricoDashboardPrincipal());

                tblReportestr2tc1.Controls.Add(
                            DTIChartsAndControls.tituloYBordesReporte(
                                                DTIChartsAndControls.GraficaBarras(ldtRepConsHist, "Mes", "Importe", "", 400, 600, "", "", "Importe", "c2", "dashGrafHisto"),
                                                             "Gráfica", "Gráfica de consumo histórico", 0, "~/UserInterface/DashboardLT/" + nombrePagina + "?ReporteConsumoHistorico=1"));

                #endregion

                #region Empleados mas caros

                DataTable ldtRepEmpleados = DSODataAccess.Execute(ConsultaEmpleadosMasCaros());

                tblReportestr3tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("dashGrvEmple", ldtRepEmpleados, true, "Totales", new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodEmple={0}&iCodCenCos={1}&ReportePorEmpleadoNiv2=1&Etiqueta={2}",
                                        new string[] { "Codigo Empleado", "Codigo Centro de Costos", "Empleado" }, 0, new int[] { 1, 3 }, new int[] { 2, 4, 5, 6 }, new int[] { 0 }),
                                        "Reporte", "Empleados más caros", 0, "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorEmpleado=1"));


                #endregion

                #region CenCos mas caros

                DataTable ldtRepCenCos = DSODataAccess.Execute(ConsultaCenCosMasCaros());

                tblReportestr2tc2.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("dashGrvCenCos", ldtRepCenCos, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodCenCos={0}&ReportePorCenCosNiv2=1&Etiqueta={1}", new string[] { "Codigo Centro de Costos", "Centro de Costos" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                        "Reporte", "Por centros de costos", 0, "~/UserInterface/DashboardLT/" + nombrePagina + "?ReportePorCenCos=1"));

                #endregion

                #endregion
            }

            /*Se agregan los controles al contenedor principal*/
            pnlContenedorPrincipal.Controls.Add(tblReportes);
            pnlContenedorPrincipal.Controls.Add(new LiteralControl("<br />"));
            pnlContenedorPrincipal.Controls.Add(new LiteralControl("<br />"));
        }

        private bool VerDashboard()
        {
            return ReporteConsumoHistorico == 0 && ReportePorEmpleado == 0 && ReportePorCenCos == 0
                            && ReportePorEmpleadoNiv2 == 0 && ReportePorEmpleadoNiv3 == 0 && ReportePorEmpleadoNiv4 == 0
                            && ReportePorEmpleadoNiv5 == 0 && ReportePorEmpleadoNiv6 == 0
                            && ReportePorCenCosNiv2 == 0 && ReportePorCenCosNiv3 == 0 && ReportePorCenCosNiv4 == 0
                            && ReportePorCenCosNiv5 == 0 && ReportePorCenCosNiv6 == 0;
        }

        #region Consultas a SQL

        protected string ConsultaHistoricoDashboardPrincipal()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" \r");
            lsb.Append("declare @fechaFin varchar(30) \r");
            lsb.Append("declare @fechaInicio varchar(30) \r");
            lsb.Append("declare @fechaInicioActual varchar(30) \r");
            lsb.Append("declare @anio int  \r");
            lsb.Append("declare @mes int \r");
            lsb.Append("declare @dia int \r");
            lsb.Append("set @mes = MONTH(GETDATE()) \r");
            lsb.Append("set @anio = YEAR(GETDATE()) \r");
            lsb.Append("set @dia = DAY(GETDATE()) \r");
            lsb.Append("set @fechaInicioActual = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \r");
            lsb.Append("if @mes < 10 \r");
            lsb.Append("begin \r");
            lsb.Append("set @fechaInicio = CONVERT(varchar,@anio-1) + '-0' + CONVERT(varchar,@mes) + '-01 12:00:00' \r");
            lsb.Append("set @fechaFin = CONVERT(varchar,@anio) + '-0' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd(  \r");
            lsb.Append("month,1,@fechaInicioActual) -1)) + ' 23:59:59' \r");
            lsb.Append("end \r");
            lsb.Append("else \r");
            lsb.Append("begin \r");
            lsb.Append("set @fechaInicio = CONVERT(varchar,@anio-1) + '-' + CONVERT(varchar,@mes) + '-01 12:00:00' \r");
            lsb.Append("set @fechaFin = CONVERT(varchar,@anio) + '-' + CONVERT(varchar,@mes) + '-' + convert(varchar,day(dateadd(  \r");
            lsb.Append("month,1,@fechaInicioActual) -1)) + ' 23:59:59' \r");
            lsb.Append("end \r");
            lsb.Append("set @fechaInicio =  '''' + @fechaInicio +  '''' \r");
            lsb.Append("set @fechaFin =  '''' + @fechaFin +  '''' \r");
            lsb.Append("exec ConsumoHistorico  \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Nombre Mes] as [Mes], \r");
            lsb.Append("[Importe] = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio])',  \r");
            lsb.Append("@Group = '[Nombre Mes]', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = @fechaInicio, \r");
            lsb.Append("@FechaFinRep = @fechaFin, \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaEmpleadosMasCaros()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec RepTabConsumoEmpsMasCarosSP @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields=' [Empleado]=Min(upper([Nombre Completo])), \r");
            lsb.Append("[Codigo Empleado], \r");
            lsb.Append("[Centro de Costos]=MIN(upper([Nombre Centro de Costos])), \r");
            lsb.Append("[Codigo Centro de Costos], \r");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Llamadas]=SUM([TotalLlamadas]), \r");
            lsb.Append("[Duracion]=sum([Duracion Minutos])',  \r");
            lsb.Append("@Where = 'FechaInicio >= ''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''  \r");
            lsb.Append("and FechaInicio <=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Group = '[Codigo Empleado], \r");
            lsb.Append("[Codigo Centro de Costos]',  \r");
            lsb.Append("@Order = '[Total] Desc', \r");
            lsb.Append("@OrderInv = '[Total] Asc', \r");
            lsb.Append("@OrderDir = 'Asc', \r");
            lsb.Append("@Lenght = 10, \r");
            lsb.Append("@Start = 0,\r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel2ReportePorEmpleado(int iCodEmple, int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @ParamEmple varchar(max) \r");
            lsb.Append("declare @ParamCenCos varchar(max) \r");
            lsb.Append("set @ParamEmple = '" + iCodEmple + "' \r");
            lsb.Append("set @ParamCenCos = '" + iCodCenCos + "' \r");
            lsb.Append("set @Where = 'FechaInicio >= ''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio <= ''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("if @ParamEmple <> 'null' \r");
            lsb.Append("      set @Where = @Where + 'And [Codigo Empleado] in('+@ParamEmple+')' \r");
            lsb.Append("if @ParamCenCos <> 'null' \r");
            lsb.Append("      set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+') ' \r");
            lsb.Append("exec spAcumuladoMatrizRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@InnerFields='[Nombre Sitio]=MIN(upper([Nombre Sitio])), \r");
            lsb.Append("[Codigo Sitio], \r");
            lsb.Append("[Nombre Tipo Destino]=MIN(upper([Nombre Tipo Destino])), \r");
            lsb.Append("[Codigo Tipo Destino], \r");
            lsb.Append("TotalCosto = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio])',  \r");
            lsb.Append("@InnerWhere=@Where,  \r");
            lsb.Append("@InnerGroup='[Codigo Sitio], \r");
            lsb.Append("[Codigo Tipo Destino]',  \r");
            lsb.Append("@OuterFields='[Nombre Sitio] as [Sitio], \r");
            lsb.Append("[Codigo Sitio], \r");
            lsb.Append("[TELCEL] = SUM(case when [Nombre Tipo Destino] = ''TELCEL'' AND [Codigo Tipo Destino] = 83850 then [TotalCosto] else null end), \r");
            lsb.Append("[Total] = SUM([TotalCosto])',  \r");
            lsb.Append("@OuterGroup='[Nombre Sitio], \r");
            lsb.Append("[Codigo Sitio]', \r");
            lsb.Append("@Order='[Sitio] Asc,[Total] Desc', \r");
            lsb.Append("@OrderInv='[Sitio] Desc,[Total] Asc', \r");
            lsb.Append("@OrderDir='Asc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel3ReportePorEmpleado(int iCodEmple, int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @CenCos varchar(max) \r");
            lsb.Append("declare @Emple varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''  \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @CenCos = '" + iCodCenCos + "' \r");
            lsb.Append("set @Emple ='" + iCodEmple + "' \r");
            lsb.Append("if @Emple <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Codigo Empleado] in('+@Emple+')' \r");
            lsb.Append("exec ConsumoTelcelSoloEmpleRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[intFake] = 0,[Extension] as [Móvil], \r");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Minutos]=SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Extension]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[Móvil] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[Móvil] Desc', \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel4ReportePorEmpleado(string NumeroMovil)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \r");
            lsb.Append("' \r");
            lsb.Append("exec ConsumoTelcelDetalleFacRest  @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Tipo Detalle], \r");
            lsb.Append("Linea = [Extension], \r");
            lsb.Append("[Costo]=sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Min Libres Pico]=SUM([Min Libres Pico]), \r");
            lsb.Append("[Min Facturables Pico]=SUM([Min Facturables Pico]), \r");
            lsb.Append("[Min Libres No Pico]=SUM([Min Libres No Pico]), \r");
            lsb.Append("[Min Facturables No Pico]=SUM([Min Facturables No Pico]), \r");
            lsb.Append("[Tiempo Aire Nacional]=SUM([Tiempo Aire Nacional]/[TipoCambio]), \r");
            lsb.Append("[Tiempo Aire Roaming Nac]=SUM([Tiempo Aire Roaming Nac]/[TipoCambio]), \r");
            lsb.Append("[Tiempo Aire Roaming Int]=SUM([Tiempo Aire Roaming Int]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Nac]=SUM([Larga Distancia Nac]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Roam Nac]=SUM([Larga Distancia Roam Nac]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Roam Int]=SUM([Larga Distancia Roam Int]/[TipoCambio]), \r");
            lsb.Append("[Servicios Adicionales]=SUM([Servicios Adicionales]/[TipoCambio]) + SUM([Renta]/[TipoCambio]), \r");
            lsb.Append("[Serv Adic] = 7577, \r");
            lsb.Append("[Desc Tiempo Aire]=SUM([Desc Tiempo Aire]/[TipoCambio]), \r");
            lsb.Append("[Desc Tiempo Aire Roam]=SUM([Desc Tiempo Aire Roam]/[TipoCambio]), \r");
            lsb.Append("[Ajustes]=SUM([Ajustes]/[TipoCambio]), \r");
            lsb.Append("[Otros Desc]=SUM([Otros Desc]/[TipoCambio]), \r");
            lsb.Append("[Cargos Creditos]=SUM([Cargos Creditos]/[TipoCambio]), \r");
            lsb.Append("[Otros Serv]=SUM([Otros Serv]/[TipoCambio]), \r");
            lsb.Append("[Otros Servicios]=7574',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Tipo Detalle], \r");
            lsb.Append("[Extension]',  \r");
            lsb.Append("@Order = '[Tipo Detalle] Asc,[Costo] Asc,[Min Libres Pico] Asc,[Min Facturables Pico] Asc,[Min Libres No Pico] Asc,[Min Facturables No Pico] Asc, \r");
            lsb.Append("[Tiempo Aire Nacional] Asc,[Tiempo Aire Roaming Nac] Asc,[Tiempo Aire Roaming Int] Asc,[Larga Distancia Nac] Asc,[Larga Distancia Roam Nac] Asc, \r");
            lsb.Append("[Larga Distancia Roam Int] Asc,[Servicios Adicionales] Asc,[Desc Tiempo Aire] Asc,[Desc Tiempo Aire Roam] Asc,[Ajustes] Asc,[Otros Desc] Asc, \r");
            lsb.Append("[Cargos Creditos] Asc,[Otros Serv] Asc,[Linea] Desc', \r");
            lsb.Append("@OrderInv = '[Tipo Detalle] Desc,[Costo] Desc,[Min Libres Pico] Desc,[Min Facturables Pico] Desc,[Min Libres No Pico] Desc,[Min Facturables No Pico] Desc, \r");
            lsb.Append("[Tiempo Aire Nacional] Desc,[Tiempo Aire Roaming Nac] Desc,[Tiempo Aire Roaming Int] Desc,[Larga Distancia Nac] Desc,[Larga Distancia Roam Nac] Desc, \r");
            lsb.Append("[Larga Distancia Roam Int] Desc,[Servicios Adicionales] Desc,[Desc Tiempo Aire] Desc,[Desc Tiempo Aire Roam] Desc,[Ajustes] Desc,[Otros Desc] Desc, \r");
            lsb.Append("[Cargos Creditos] Desc,[Otros Serv] Desc,[Linea] Asc', \r");
            lsb.Append("@OrderDir = 'Asc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel5ReportePorEmpleado(string NumeroMovil)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+')' \r");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Numero Marcado] as [Numero marcado], \r");
            lsb.Append("[Total]=SUM([Costo]/[TipoCambio]), \r");
            lsb.Append("[Minutos]=SUM([Duracion Minutos]), \r");
            lsb.Append("LLamadas = count(*), \r");
            lsb.Append("[Tiempo promedio] = avg([Duracion Segundos]/60), \r");
            lsb.Append("[Punta B] as [Localidad]',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Numero Marcado], \r");
            lsb.Append("[Punta B]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[LLamadas] Desc,[Tiempo promedio] Desc,[Localidad] Asc,[Numero Marcado] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[LLamadas] Asc,[Tiempo promedio] Asc,[Localidad] Desc,[Numero Marcado] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel6ReportePorEmpleado(string NumeroMovil, string NumeroMarcado)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("declare @NumeroMarcado varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("set @NumeroMarcado = '''" + NumeroMarcado + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+')' \r");
            lsb.Append("if @NumeroMarcado <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Numero Marcado] in('+@NumeroMarcado+')' \r");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Fecha Llamada] as [Fecha], \r");
            lsb.Append("[Hora Llamada] as [Hora], \r");
            lsb.Append("[Duracion Minutos] as [Minutos], \r");
            lsb.Append("[Total]=([Costo]/[TipoCambio]), \r");
            lsb.Append("[Punta A] as [Localidad], \r");
            lsb.Append("[Dir Llamada] as [Tipo], \r");
            lsb.Append("[Punta B] as [Localidad origen]',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[Fecha] Asc,[Hora] Asc,[Tipo] Asc,[Localidad] Asc,[Localidad origen] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[Fecha] Desc,[Hora] Desc,[Tipo] Desc,[Localidad] Desc,[Localidad origen] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaCenCosMasCaros()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("exec ConsumoAcumuladoTodosCamposRest      \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields=' [Codigo Centro de Costos],[Centro de Costos]=Min(upper([Nombre Centro de Costos])), \r");
            lsb.Append("[Total] = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]), \r");
            lsb.Append("LLamadas = sum([TotalLlamadas]), \r");
            lsb.Append("[Minutos] = SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where = 'FechaInicio >= ''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''  \r");
            lsb.Append("and FechaInicio <= ''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Group = '[Codigo Centro de Costos]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[LLamadas] Desc,[Centro de Costos] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[LLamadas] Asc,[Centro de Costos] Desc', \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Lenght = 10, \r");
            lsb.Append("@Start = 0,\r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel2ReportePorCenCos(int iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @ParamCenCos varchar(max) \r");
            lsb.Append("set @ParamCenCos = '" + iCodCenCos + "' \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("if @ParamCenCos <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Codigo Centro de Costos] in('+@ParamCenCos+')' \r");
            lsb.Append("exec ConsumoAcumuladoTodosCamposRest      \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Codigo Empleado],[Empleado]=Min(upper([Nombre Completo])), \r");
            lsb.Append("[Total] = SUM([Costo]/[TipoCambio]) + SUM([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Total de llamadas] = sum([TotalLlamadas]), \r");
            lsb.Append("[Total de minutos] = SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where = @Where, \r");
            lsb.Append("@Group = '[Codigo Empleado]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Total de llamadas] Desc,[Total de minutos] Desc,[Empleado] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Total de llamadas] Asc,[Total de minutos] Asc,[Empleado] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");


            return lsb.ToString();
        }

        protected string ConsultaNivel3ReportePorCenCos(int iCodCenCos, int iCodEmple)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @CenCos varchar(max) \r");
            lsb.Append("declare @Emple varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @CenCos = '" + iCodCenCos + "' \r");
            lsb.Append("set @Emple ='" + iCodEmple + "' \r");
            lsb.Append("if @Emple <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Codigo Empleado] in('+@Emple+')' \r");
            lsb.Append("exec ConsumoTelcelSoloEmpleRest  \r");
            lsb.Append("@Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[intFake] = 0, [Extension] as [Movil], \r");
            lsb.Append("[Total]= sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Minutos]=SUM([Duracion Minutos])',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Extension]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[Movil] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[Movil] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");


            return lsb.ToString();
        }

        protected string ConsultaNivel4ReportePorCenCos(string NumeroMovil)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+') \r");
            lsb.Append("' \r");
            lsb.Append("exec ConsumoTelcelDetalleFacRest  @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Tipo Detalle], \r");
            lsb.Append("Linea = [Extension], \r");
            lsb.Append("[Costo]=sum([Costo]/[TipoCambio])+sum([CostoSM]/[TipoCambio]), \r");
            lsb.Append("[Min Libres Pico]=SUM([Min Libres Pico]), \r");
            lsb.Append("[Min Facturables Pico]=SUM([Min Facturables Pico]), \r");
            lsb.Append("[Min Libres No Pico]=SUM([Min Libres No Pico]), \r");
            lsb.Append("[Min Facturables No Pico]=SUM([Min Facturables No Pico]), \r");
            lsb.Append("[Tiempo Aire Nacional]=SUM([Tiempo Aire Nacional]/[TipoCambio]), \r");
            lsb.Append("[Tiempo Aire Roaming Nac]=SUM([Tiempo Aire Roaming Nac]/[TipoCambio]), \r");
            lsb.Append("[Tiempo Aire Roaming Int]=SUM([Tiempo Aire Roaming Int]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Nac]=SUM([Larga Distancia Nac]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Roam Nac]=SUM([Larga Distancia Roam Nac]/[TipoCambio]), \r");
            lsb.Append("[Larga Distancia Roam Int]=SUM([Larga Distancia Roam Int]/[TipoCambio]), \r");
            lsb.Append("[Servicios Adicionales]=SUM([Servicios Adicionales]/[TipoCambio]) + SUM([Renta]/[TipoCambio]), \r");
            lsb.Append("[Serv Adic] = 7577, \r");
            lsb.Append("[Desc Tiempo Aire]=SUM([Desc Tiempo Aire]/[TipoCambio]), \r");
            lsb.Append("[Desc Tiempo Aire Roam]=SUM([Desc Tiempo Aire Roam]/[TipoCambio]), \r");
            lsb.Append("[Ajustes]=SUM([Ajustes]/[TipoCambio]), \r");
            lsb.Append("[Otros Desc]=SUM([Otros Desc]/[TipoCambio]), \r");
            lsb.Append("[Cargos Creditos]=SUM([Cargos Creditos]/[TipoCambio]), \r");
            lsb.Append("[Otros Serv]=SUM([Otros Serv]/[TipoCambio]), \r");
            lsb.Append("[Otros Servicios]=7574',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Tipo Detalle], \r");
            lsb.Append("[Extension]',  \r");
            lsb.Append("@Order = '[Tipo Detalle] Asc,[Costo] Asc,[Min Libres Pico] Asc,[Min Facturables Pico] Asc,[Min Libres No Pico] Asc,[Min Facturables No Pico] Asc, \r");
            lsb.Append("[Tiempo Aire Nacional] Asc,[Tiempo Aire Roaming Nac] Asc,[Tiempo Aire Roaming Int] Asc,[Larga Distancia Nac] Asc,[Larga Distancia Roam Nac] Asc, \r");
            lsb.Append("[Larga Distancia Roam Int] Asc,[Servicios Adicionales] Asc,[Desc Tiempo Aire] Asc,[Desc Tiempo Aire Roam] Asc,[Ajustes] Asc,[Otros Desc] Asc, \r");
            lsb.Append("[Cargos Creditos] Asc,[Otros Serv] Asc,[Linea] Desc', \r");
            lsb.Append("@OrderInv = '[Tipo Detalle] Desc,[Costo] Desc,[Min Libres Pico] Desc,[Min Facturables Pico] Desc,[Min Libres No Pico] Desc,[Min Facturables No Pico] Desc, \r");
            lsb.Append("[Tiempo Aire Nacional] Desc,[Tiempo Aire Roaming Nac] Desc,[Tiempo Aire Roaming Int] Desc,[Larga Distancia Nac] Desc,[Larga Distancia Roam Nac] Desc, \r");
            lsb.Append("[Larga Distancia Roam Int] Desc,[Servicios Adicionales] Desc,[Desc Tiempo Aire] Desc,[Desc Tiempo Aire Roam] Desc,[Ajustes] Desc,[Otros Desc] Desc, \r");
            lsb.Append("[Cargos Creditos] Desc,[Otros Serv] Desc,[Linea] Asc', \r");
            lsb.Append("@OrderDir = 'Asc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel5ReportePorCenCos(string NumeroMovil)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+')' \r");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Numero Marcado] as [Numero marcado], \r");
            lsb.Append("[Total]=SUM([Costo]/[TipoCambio]), \r");
            lsb.Append("[Minutos]=SUM([Duracion Minutos]), \r");
            lsb.Append("LLamadas = count(*), \r");
            lsb.Append("[Tiempo promedio] = avg([Duracion Segundos]/60), \r");
            lsb.Append("[Punta B] as [Localidad]',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '[Numero Marcado], \r");
            lsb.Append("[Punta B]',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[LLamadas] Desc,[Tiempo promedio] Desc,[Localidad] Asc,[Numero Marcado] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[LLamadas] Asc,[Tiempo promedio] Asc,[Localidad] Desc,[Numero Marcado] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaNivel6ReportePorCenCos(string NumeroMovil, string NumeroMarcado)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max) \r");
            lsb.Append("declare @NumeroMovil varchar(max) \r");
            lsb.Append("declare @NumeroMarcado varchar(max) \r");
            lsb.Append("set @Where = 'FechaInicio>=''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("and FechaInicio<=''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");
            lsb.Append("set @NumeroMovil = '''" + NumeroMovil + "''' \r");
            lsb.Append("set @NumeroMarcado = '''" + NumeroMarcado + "''' \r");
            lsb.Append("if @NumeroMovil <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Extension] in('+@NumeroMovil+')' \r");
            lsb.Append("if @NumeroMarcado <> 'null' \r");
            lsb.Append("set @Where = @Where + 'And [Numero Marcado] in('+@NumeroMarcado+')' \r");
            lsb.Append("exec ConsumoTelcelDetalleLlamRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@Fields='[Fecha Llamada] as [Fecha], \r");
            lsb.Append("[Hora Llamada] as [Hora], \r");
            lsb.Append("[Duracion Minutos] as [Minutos], \r");
            lsb.Append("[Total]=([Costo]/[TipoCambio]), \r");
            lsb.Append("[Punta A] as [Localidad], \r");
            lsb.Append("[Dir Llamada] as [Tipo], \r");
            lsb.Append("[Punta B] as [Localidad origen]',  \r");
            lsb.Append("@Where =@Where, \r");
            lsb.Append("@Group = '',  \r");
            lsb.Append("@Order = '[Total] Desc,[Minutos] Desc,[Fecha] Asc,[Hora] Asc,[Tipo] Asc,[Localidad] Asc,[Localidad origen] Asc', \r");
            lsb.Append("@OrderInv = '[Total] Asc,[Minutos] Asc,[Fecha] Desc,[Hora] Desc,[Tipo] Desc,[Localidad] Desc,[Localidad origen] Desc', \r");
            //lsb.Append("@Lenght = 10, \r");
            //lsb.Append("@Start = 0, \r");
            lsb.Append("@OrderDir = 'Desc', \r");
            lsb.Append("@Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append("@Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaTemplate()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" \r");
            return lsb.ToString();
        }

        #endregion

        #region Logica de botones de la pagina

        protected void btnAplicar_Click(object sender, EventArgs e)
        {

        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            List<string> ltNavegacion = (List<string>)Session["pltNavegacion"];

            //obtener el numero actual de elementos de la lista
            string lsCantidadElem = ltNavegacion.Count.ToString();
            //eliminar el ultimo elemento de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            #region Etiqueta de navegacion

            List<string> ltEtiquetaNavegacion = (List<string>)Session["etiquetaNavegacion"];

            //eliminar el ultimo elemento de la lista
            if (ltEtiquetaNavegacion.Count > 1)
            {
                ltEtiquetaNavegacion.RemoveAt(ltEtiquetaNavegacion.Count - 1);
            }

            Session["estadoDeNavegacion"] = ltEtiquetaNavegacion;

            #endregion

            HttpContext.Current.Response.Redirect(lsLastElement);
        }

        #endregion
    }
}
