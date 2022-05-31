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
using System.Collections;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using KeytiaServiceBL.Reportes;

namespace KeytiaWeb.UserInterface.DashboardLT
{
    public partial class DashboardLTCntr : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        /*paginaLocal se usa para mandar el nombre de la clase donde se produjo el error en el log de web*/
        string paginaLocal = "KeytiaWeb.UserInterface.DashboardLT.DashboardLTCntr.aspx.cs";
        /*nombrePagina se usa para poder cambiarle el nombre a la pagina una ves que se pasen cambios a producción*/
        string nombrePagina = "DashboardLTCntr.aspx";

        #region Variables que guardan valores de queryString

        int iCodExten = 0;
        int tipoLlam = 0;
        int ReporteTraficoLlam = 0;
        int DetallLlam = 0;
        string NumMarcado = string.Empty;

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
            else
            {
                btnRegresar.Visible = true;
            }
            #endregion

            #region Lee Query String

            #region Revisar si el querystring iCodExten contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["iCodExten"]))
            {
                try
                {
                    iCodExten = Convert.ToInt32(Request.QueryString["iCodExten"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (iCodExten) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                iCodExten = 0;
            }

            #endregion

            #region Revisar si el querystring tipoLlam contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["tipoLlam"]))
            {
                try
                {
                    tipoLlam = Convert.ToInt32(Request.QueryString["tipoLlam"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (tipoLlam) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                tipoLlam = 0;
            }

            #endregion

            #region Revisar si el querystring ReporteTraficoLlam contiene un valor

            if (!string.IsNullOrEmpty(Request.QueryString["ReporteTraficoLlam"]))
            {
                try
                {
                    ReporteTraficoLlam = Convert.ToInt32(Request.QueryString["ReporteTraficoLlam"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (ReporteTraficoLlam) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                ReporteTraficoLlam = 0;
            }

            #endregion

            #region Revisar si el querystring DetallLlam contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["DetallLlam"]))
            {
                try
                {
                    DetallLlam = Convert.ToInt32(Request.QueryString["DetallLlam"]);
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (DetallLlam) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                DetallLlam = 0;
            }

            #endregion

            #region Revisar si el querystring NumMarcado contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["NumMarcado"]))
            {
                try
                {
                    NumMarcado = Request.QueryString["NumMarcado"].ToString();
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (NumMarcado) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }


            #endregion

            #endregion

            if (!Page.IsPostBack)
            {
                #region Inicia los valores default de los controles de fecha
                try
                {
                    if (Session["FechaInicioRep"] != null && Session["FechaFinRep"] != null)
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRep"].ToString().Substring(1, 10));
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRep"].ToString().Substring(1, 10));
                    }

                    else
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)fechaInicio;
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)fechaFinal;
                    }

                    Session["FechaInicioRep"] = pdtInicio.DataValue.ToString();
                    Session["FechaFinRep"] = pdtFin.DataValue.ToString();
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

            Session["FechaInicioRep"] = pdtInicio.DataValue.ToString();
            Session["FechaFinRep"] = pdtFin.DataValue.ToString();

            if (Session["FechaInicioRep"] != null && Session["FechaFinRep"] != null)
            {
                pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRep"].ToString().Substring(1, 10));
                pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRep"].ToString().Substring(1, 10));
            }

            #endregion

            #region Creo una tabla que contendra los reportes

            Table tblReportes = new Table();
            tblReportes.Width = Unit.Percentage(100);

            TableRow tblReportestr1 = new TableRow();

            TableCell tblReportestr1tc1 = new TableCell();
            tblReportestr1tc1.ColumnSpan = 3;
            //tblReportestr1tc1.Width = Unit.Percentage(49);

            //TableCell tblReportestr1tc2 = new TableCell();
            //tblReportestr1tc2.Width = Unit.Percentage(49);

            tblReportestr1.Controls.Add(tblReportestr1tc1);
            //tblReportestr1.Controls.Add(tblReportestr1tc2);           

            TableRow tblReportestr2 = new TableRow();

            TableCell tblReportestr2tc1 = new TableCell();
            tblReportestr2tc1.Width = Unit.Percentage(33);

            TableCell tblReportestr2tc2 = new TableCell();
            tblReportestr2tc2.Width = Unit.Percentage(33);

            TableCell tblReportestr2tc3 = new TableCell();
            tblReportestr2tc3.Width = Unit.Percentage(33);

            tblReportestr2.Controls.Add(tblReportestr2tc1);
            tblReportestr2.Controls.Add(tblReportestr2tc2);
            tblReportestr2.Controls.Add(tblReportestr2tc3);

            TableRow tblReportestr3 = new TableRow();

            TableCell tblReportestr3tc1 = new TableCell();
            tblReportestr3tc1.Width = Unit.Percentage(33);

            TableCell tblReportestr3tc2 = new TableCell();
            tblReportestr3tc2.Width = Unit.Percentage(33);

            TableCell tblReportestr3tc3 = new TableCell();
            tblReportestr3tc3.Width = Unit.Percentage(33);

            tblReportestr3.Controls.Add(tblReportestr3tc1);
            tblReportestr3.Controls.Add(tblReportestr3tc2);
            tblReportestr3.Controls.Add(tblReportestr3tc3);

            TableRow tblReportestr4 = new TableRow();
            TableCell tblReportestr4tc1 = new TableCell();
            tblReportestr4tc1.Width = Unit.Percentage(33);

            TableCell tblReportestr4tc2 = new TableCell();
            tblReportestr4tc2.ColumnSpan = 2;

            tblReportestr4.Controls.Add(tblReportestr4tc1);
            tblReportestr4.Controls.Add(tblReportestr4tc2);

            tblReportes.Controls.Add(tblReportestr1);
            tblReportes.Controls.Add(tblReportestr2);
            tblReportes.Controls.Add(tblReportestr3);
            tblReportes.Controls.Add(tblReportestr4);


            Panel pnlReporteTraficoLlam = new Panel();
            pnlReporteTraficoLlam.Width = Unit.Percentage(100);

            #endregion

            #region Reportes de dashboard

            if (iCodExten == 0 && ReporteTraficoLlam == 0 && DetallLlam == 0)
            {

                #region GridView Llamadas de salida

                DataTable tblReportestr2tc2ldt = DSODataAccess.Execute(ConsultaLlamadasSalidaPorExtension());

                tblReportestr3tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvRepLlamSal", tblReportestr2tc2ldt, true, "Totales", new string[] { "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodExten={0}", new string[] { "iCodExten", "Extension" }, 1, new int[] { 0 }, new int[] { 2, 3, 4, 5 },
                                        new int[] { 1 }), "Reporte", "Llamadas de salida", 400));

                if (tblReportestr2tc2ldt.Rows.Count > 0)
                {
                    tblReportestr2tc2ldt.Rows.RemoveAt(tblReportestr2tc2ldt.Rows.Count - 1);
                }

                tblReportestr3tc2.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GraficaBarras(tblReportestr2tc2ldt, "Extension", "Llamadas", "",
                                                                            350, 400, "", "",
                                                                            "Numero de Llamadas", "{0}", "grafLlamadasSalida"),
                                        "Reporte", "Grafica de Llamadas de Salida", 400)

                        );

                tblReportestr3tc3.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GraficaBarras(tblReportestr2tc2ldt, "Extension", "Minutos", "",
                                                                            350, 400, "", "",
                                                                            "Cantidad de Minutos", "{0}", "grafLlamadasSalidaMinutos"),
                                        "Reporte", "Grafica de Llamadas de Salida", 400)

                        );

                #endregion

                #region Grafica de extension x importe en dashboard principal

                DataTable tblReportestr1tc1ldt = DSODataAccess.Execute(ConsultaTop10PorMinutos());

                //tblReportestr1tc1ldt.Columns.Remove("iCodExten");                
                //if (tblReportestr1tc1ldt.Rows.Count > 0)
                //    tblReportestr1tc1ldt.Rows.RemoveAt(tblReportestr1tc1ldt.Rows.Count - 1);

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                                DTIChartsAndControls.GraficaBarras(tblReportestr1tc1ldt, "Empleado", "Minutos", "", 400, 1280, "", "", "Minutos", "{0}", "grafTop10Minutos"),
                                                                                                 "Reporte", "Minutos por empleado", 0,
                                                                                                 "~/UserInterface/DashboardLT/" + nombrePagina + "?ReporteTraficoLlam=1"));

                DataTable tblReportestr4tc2ldt = DSODataAccess.Execute(ConsultaNumerosMasMarcados());

                tblReportestr4tc1.Controls.Add(
                  DTIChartsAndControls.tituloYBordesReporte(
                                          DTIChartsAndControls.GridView("grvRepNumMasMarcados", tblReportestr4tc2ldt, true, "Totales", new string[] { "", "", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?NumMarcado={1}&DetallLlam=1", new string[] { "iCodExten", "Numero" }, 1, new int[] { 0 }, new int[] { 2 },
                                        new int[] { 1 }), "Reporte", "Top 10 numeros mas marcados", 400));

                if (tblReportestr4tc2ldt.Rows.Count > 0)
                {
                    tblReportestr4tc2ldt.Rows.RemoveAt(tblReportestr4tc2ldt.Rows.Count - 1);
                }

                tblReportestr4tc2.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                                DTIChartsAndControls.GraficaBarras(tblReportestr4tc2ldt, "Numero", "VecesMarcadas", "", 350, 730, "", "", "Numero de veces Marcadas", "{0}", "grafNumerosMasMarcados"),
                                                                                                 "Reporte", "Numeros mas marcados", 400));


                #endregion

                #region GridView Llamadas Entrantes

                DataTable tblReportestr2tc1ldt = DSODataAccess.Execute(ConsultaLlamadasEntradaPorExtension());

                tblReportestr2tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GridView("grvRepLlamEnt", tblReportestr2tc1ldt, true, "Totales",
                                        new string[] { "", "", "", "{0:0,0}", "{0:0,0}" },
                                        "~/UserInterface/DashboardLT/" + nombrePagina + "?iCodExten={0}&tipoLlam=381",
                                        new string[] { "iCodExten", "Extension" }, 1, new int[] { 0 },
                                        new int[] { 2, 3, 4 }, new int[] { 1 }), "Reporte", "Llamadas de entrada", 400)
                        );

                if (tblReportestr2tc1ldt.Rows.Count > 0)
                {
                    //Remueve a fila de totales
                    tblReportestr2tc1ldt.Rows.RemoveAt(tblReportestr2tc1ldt.Rows.Count - 1);
                }
                tblReportestr2tc2.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GraficaBarras(tblReportestr2tc1ldt, "Extension", "Llamadas", "",
                                                                            350, 400, "", "",
                                                                            "Numero de Llamadas", "{0}", "grafLlamadasEntrantes"),
                                        "Reporte", "Grafica de Llamadas de Entrada", 400)

                        );

                tblReportestr2tc3.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                        DTIChartsAndControls.GraficaBarras(tblReportestr2tc1ldt, "Extension", "Minutos", "",
                                                                            350, 400, "", "",
                                                                            "Cantidad de Minutos", "{0}", "grafLlamadasEntrantesMinutos"),
                                        "Reporte", "Grafica de Llamadas de Entrada", 400)

                        );


                #endregion

            }

            #endregion

            #region Detalle de una extensión

            if (iCodExten != 0)
            {
                btnExportarPDF.Visible = false;
                //btnExportarXLS.Visible = false;

                DataTable tblReportestr1tc1ldt = DSODataAccess.Execute(ConsultaDetalleEmpleadoTDestCenCos(iCodExten, tipoLlam));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                DTIChartsAndControls.GridView("grvRepDetallExt", tblReportestr1tc1ldt, true, "Totales",
                                new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "" }), "Reporte", "Detalle de llamadas", 0));
            }

            #endregion

            #region Reporte trafico llamadas

            if (ReporteTraficoLlam == 1)
            {

                btnExportarPDF.Visible = false;

                #region Grafica de extension x importe

                //DataTable ldtGrafica = DSODataAccess.Execute(ConsultaLlamadasSalidaPorExtension());
                DataTable ldtGrafica = DSODataAccess.Execute(ConsultaTop10PorMinutos());

                //ldtGrafica.Columns.Remove("iCodExten");
                //ldtGrafica.Columns.Remove("Llamadas");
                //ldtGrafica.Columns.Remove("Minutos");
                //ldtGrafica.Rows.RemoveAt(ldtGrafica.Rows.Count - 1);

                //tblReportestr1tc1.Controls.Add(
                //        DTIChartsAndControls.tituloYBordesReporte(
                //                                DTIChartsAndControls.GraficaBarras(ldtGrafica, "Extension", "Costo", "", 400, 1300, "", "", "Costo", "c2", "grafCostoExten"),
                //"Reporte", "Consumo acumulado por extensión", 0));
                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                                DTIChartsAndControls.GraficaBarras(ldtGrafica, "Empleado", "Minutos", "", 400, 1300, "", "", "Minutos", "{0}", "grafTop10Minutos"),
                                                                                                 "Reporte", "Minutos por empleado", 0));

                //DataTable ldtGrafica2 = DSODataAccess.Execute(ConsultaNumerosMasMarcados());

                //tblReportestr4tc1.Controls.Add(
                //        DTIChartsAndControls.tituloYBordesReporte(
                //                                DTIChartsAndControls.GraficaBarras(ldtGrafica2, "Numero", "VecesMarcadas", "", 400, 1300, "", "", "Numero de veces Marcadas", "{0}", "grafNumerosMasMarcados"),
                //                                                                                 "Reporte", "Numeros mas marcados", 0));

                #endregion

                DataTable ldt = DTIChartsAndControls.DataTable(ConsultaReporteTraficoLlamadas());

                string[] formatCol = new string[ldt.Columns.Count];
                for (int i = 0; i < ldt.Columns.Count; i++)
                {
                    formatCol[i] = "";
                }

                pnlReporteTraficoLlam.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(DTIChartsAndControls.GridView("RepTrafLlam", ldt, true, "Totales", formatCol),
                    "Reporte", "Reporte de trafico de llamadas", 200));
            }

            #endregion

            if (DetallLlam == 1)
            {

                btnExportarPDF.Visible = false;
                //btnExportarXLS.Visible = false;

                DataTable tblReportestr2tc1ldt = DSODataAccess.Execute(ConsultaDetalleNumerosMasMarcados(NumMarcado.Replace("'", "")));

                tblReportestr1tc1.Controls.Add(
                        DTIChartsAndControls.tituloYBordesReporte(
                                DTIChartsAndControls.GridView("grvRepDetallNumMarcado", tblReportestr2tc1ldt, true, "Totales", 
                                new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "" }), "Reporte", "Detalle de numeros mas marcados", 0));

            }

            /*Se agregan los controles al contenedor principal*/
            pnlContenedorPrincipalDashR3.Controls.Add(tblReportes);
            pnlContenedorPrincipalDashR3.Controls.Add(new LiteralControl("<br />"));
            pnlContenedorPrincipalDashR3.Controls.Add(pnlReporteTraficoLlam);
            pnlContenedorPrincipalDashR3.Controls.Add(new LiteralControl("<br />"));
            pnlContenedorPrincipalDashR3.Controls.Add(new LiteralControl("<br />"));
        }

        #region Consultas a SQL

        protected string ConsultaTop10PorMinutos()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" Select top 10 \r");
            //lsb.Append(" [Extension]			= ExtenCod, \r");
            lsb.Append(" [Empleado]			= Emple.NomCompleto + ' (' + ExtenCod + ')' , \r");
            lsb.Append(" [Minutos]			= SUM(DuracionMin) \r");
            lsb.Append(" from vDetalleCDR detall \r");
            lsb.Append(" inner join [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            lsb.Append(" on detall.Emple = Emple.iCodCatalogo \r");
            lsb.Append(" and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" where TDest <> 381 \r");
            lsb.Append(" and Exten is not null \r");
            lsb.Append(" and FechaInicio >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r");
            lsb.Append(" and FechaInicio < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r");
            lsb.Append(" Group by [ExtenCod],Emple.NomCompleto Order by [Minutos] Desc \r");

            return lsb.ToString();
        }

        protected string ConsultaNumerosMasMarcados()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" Select top 10\r");
            lsb.Append(" [iCodExten]			= Exten, \r");
            lsb.Append(" Numero = '''' + TelDest, \r");
            lsb.Append(" VecesMarcadas = COUNT(*) \r");
            lsb.Append(" from vDetalleCDR \r");
            lsb.Append(" where TDest <> 381 \r");
            lsb.Append(" and Exten is not null \r");
            lsb.Append(" and FechaInicio >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r");
            lsb.Append(" and FechaInicio < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r");
            lsb.Append(" group by TelDest, Exten \r");
            lsb.Append(" order by VecesMarcadas desc \r");

            return lsb.ToString();
        }

        protected string ConsultaLlamadasSalidaPorExtension()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" /*Llamadas salida por extension*/ \r");
            lsb.Append(" Select top 10\r");
            lsb.Append(" [iCodExten]			= Exten, \r");
            lsb.Append(" [Extension]			= ExtenCod, \r");
            lsb.Append(" [Empleado]			= Emple.NomCompleto, \r");
            lsb.Append(" [Llamadas]			= COUNT(detall.iCodRegistro), \r");
            lsb.Append(" [Minutos]			= SUM(DuracionMin), \r");
            lsb.Append(" [Costo]				= SUM(CostoFac+CostoSM) \r");
            lsb.Append(" from vDetalleCDR detall \r");
            lsb.Append(" inner join [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            lsb.Append(" on detall.Emple = Emple.iCodCatalogo \r");
            lsb.Append(" and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" where TDest <> 381 \r");
            lsb.Append(" and Exten is not null \r");
            lsb.Append(" and FechaInicio >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r");
            lsb.Append(" and FechaInicio < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r");
            lsb.Append(" Group by [Exten],[ExtenCod], Emple.NomCompleto Order by [Costo] Desc \r");

            return lsb.ToString();
        }

        protected string ConsultaLlamadasEntradaPorExtension()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("  /*Llamadas entrada por extension*/ \r");
            lsb.Append(" Select top 10 \r");
            lsb.Append(" [iCodExten]			= Exten, \r");
            lsb.Append(" [Extension]			= ExtenCod, \r");
            lsb.Append(" [Empleado]			= Emple.NomCompleto, \r");
            lsb.Append(" [Llamadas]			= COUNT(detall.iCodRegistro), \r");
            lsb.Append(" [Minutos]			= SUM(DuracionMin) \r");
            lsb.Append(" from vDetalleCDR detall \r");
            lsb.Append(" inner join [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            lsb.Append(" on detall.Emple = Emple.iCodCatalogo \r");
            lsb.Append(" and dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" where TDest = 381 \r");
            lsb.Append(" and Exten is not null \r");
            lsb.Append(" and FechaInicio >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r");
            lsb.Append(" and FechaInicio < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \r");
            lsb.Append(" Group by [Exten],[ExtenCod], Emple.NomCompleto Order by [Minutos] Desc\r");

            return lsb.ToString();
        }

        protected string ConsultaDetalleNumerosMasMarcados(string NumMarcado)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)  \r");
            lsb.Append("declare @OrderInv varchar(max)  \r");
            lsb.Append("set @Where = '[Fecha Inicio] >= ''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("'+' and [Fecha Inicio] <= ''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");

            lsb.Append("+ ' and [Numero Marcado] = ''''''" + NumMarcado + "''' \r");
            lsb.Append("exec ConsumoDetalladoDashboardLT   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@Fields=' \r");
            lsb.Append("[Centro de costos] as [Centro de costos], \r");
            lsb.Append("[Empleado]	 as [Empleado], \r");
            lsb.Append("[Extensión]	 as [Extensión], \r");
            lsb.Append("[Numero Marcado] as [Numero Marcado], \r");
            lsb.Append("[Fecha] as [Fecha], \r");
            lsb.Append("[Hora] as [Hora], \r");
            lsb.Append("[Duracion] as [Duracion], \r");
            lsb.Append("[Total] as [Total], \r");
            lsb.Append("[Nombre Localidad] as [Nombre Localidad], \r");
            lsb.Append("[Nombre Sitio]	 as [Nombre Sitio], \r");
            lsb.Append("[Codigo Autorizacion] as [Codigo Autorizacion], \r");
            lsb.Append("[Nombre Carrier] as [Nombre Carrier], \r");
            lsb.Append("[Tipo Llamada] as [Tipo Llamada], \r");
            lsb.Append("[Tipo de destino] as [Tipo de destino] \r");
            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,   \r");
            lsb.Append("@Order = '[Total] desc',  \r");
            lsb.Append("@OrderDir = 'Asc',  \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@FechaIniRep = '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00',  \r");
            lsb.Append("@FechaFinRep = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59'  \r");

            return lsb.ToString();
        }


        protected string ConsultaDetalleEmpleadoTDestCenCos(int iCodExten, int tipoLlamada)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @Where varchar(max)  \r");
            lsb.Append("declare @OrderInv varchar(max)  \r");
            lsb.Append("set @Where = '[Fecha Inicio] >= ''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00'' \r");
            lsb.Append("'+' and [Fecha Inicio] <= ''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''' \r");

            if (tipoLlamada == 0)
            {
                lsb.Append("+ ' and [iCodExten] = " + iCodExten + " and [iCodTDest] <> 381' \r");

                //lsb.Append("+ ' and [Numero Marcado] = ''" + NumMarcado + "'' \r");
            }
            else
            {
                lsb.Append("+ ' and [iCodExten] = " + iCodExten + " and [iCodTDest] = 381' \r");
            }
            lsb.Append("exec ConsumoDetalladoDashboardLT   \r");
            lsb.Append("@Schema = '" + DSODataContext.Schema + "',  \r");
            lsb.Append("@Fields=' \r");
            lsb.Append("[Centro de costos] as [Centro de costos], \r");
            lsb.Append("[Empleado]	 as [Empleado], \r");
            lsb.Append("[Extensión]	 as [Extensión], \r");
            lsb.Append("[Numero Marcado] as [Numero Marcado], \r");
            lsb.Append("[Fecha] as [Fecha], \r");
            lsb.Append("[Hora] as [Hora], \r");
            lsb.Append("[Duracion] as [Duracion], \r");
            lsb.Append("[Total] as [Total], \r");
            lsb.Append("[Nombre Localidad] as [Nombre Localidad], \r");
            lsb.Append("[Nombre Sitio]	 as [Nombre Sitio], \r");
            lsb.Append("[Codigo Autorizacion] as [Codigo Autorizacion], \r");
            lsb.Append("[Nombre Carrier] as [Nombre Carrier], \r");
            lsb.Append("[Tipo Llamada] as [Tipo Llamada], \r");
            lsb.Append("[Tipo de destino] as [Tipo de destino] \r");
            lsb.Append("',  \r");
            lsb.Append("@Where = @Where,   \r");
            lsb.Append("@Order = '[Total] desc',  \r");
            lsb.Append("@OrderDir = 'Asc',  \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append(" @Idioma = '" + Session["Language"] + "', \r");
            lsb.Append("@FechaIniRep = '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00',  \r");
            lsb.Append("@FechaFinRep = '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59'  \r");

            return lsb.ToString();
        }

        protected string ConsultaReporteTraficoLlamadas()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("declare @gb varchar(max) \r");
            lsb.Append("set @gb = '' \r");
            lsb.Append("if ' [Nombre Completo], \r");
            lsb.Append("convert(varchar(10), [FechaInicio], 120)' <> '' \r");
            lsb.Append("set @gb = ' [Nombre Completo], \r");
            lsb.Append("convert(varchar(10), [FechaInicio], 120) having (1 = 1)' \r");
            lsb.Append("/*Genera fechas para campo outerfields*/ \r");
            lsb.Append("declare @fechaIniReporte datetime \r");
            lsb.Append("declare @fechaFinReporte datetime \r");
            lsb.Append("declare @outerFieldsResult varchar(max) \r");
            lsb.Append("set @outerFieldsResult = '[Nombre Completo],' +CHAR(10) \r");
            lsb.Append("set @fechaIniReporte = convert(datetime,'" + pdtInicio.Date.ToString("yyyy-MM-dd") + "') \r");
            lsb.Append("set @fechaFinReporte = convert(datetime,'" + pdtFin.Date.ToString("yyyy-MM-dd") + " ') \r");
            lsb.Append("while(@fechaIniReporte <= @fechaFinReporte) \r");
            lsb.Append("begin \r");
            lsb.Append("set @outerFieldsResult = @outerFieldsResult + '[Minutos '+replace(convert(varchar,@fechaIniReporte,103),'/','-')+ \r");
            lsb.Append("'] = SUM(case when [FechaInicio] = '''+replace(convert(varchar,@fechaIniReporte,111),'/','-')+''' then [Duracion Minutos] else 0 end),' + CHAR(10) \r");
            lsb.Append("set @outerFieldsResult = @outerFieldsResult + '[Llamadas '+replace(convert(varchar,@fechaIniReporte,103),'/','-')+ \r");
            lsb.Append("'] = SUM(case when [FechaInicio] = '''+replace(convert(varchar,@fechaIniReporte,111),'/','-')+''' then [NumRegsGrp] else 0 end),' + CHAR(10) \r");
            lsb.Append("select @fechaIniReporte = dateadd(dd,1,@fechaIniReporte) \r");
            lsb.Append("end \r");
            lsb.Append("select @outerFieldsResult = @outerFieldsResult +'[Total Minutos] = SUM([Duracion Minutos]),' + CHAR(10) \r");
            lsb.Append("select @outerFieldsResult = @outerFieldsResult +'[Total Llamadas] = SUM([NumRegsGrp])' \r");
            lsb.Append("--print @outerFieldsResult \r");
            lsb.Append("exec spDetalleRepUsuMatrizRest @Schema='" + DSODataContext.Schema + "', \r");
            lsb.Append("@InnerFields='[Nombre Completo] = UPPER([Nombre Completo]), \r");
            lsb.Append("[FechaInicio] = convert(varchar(10), [FechaInicio], 120), \r");
            lsb.Append("[Duracion Minutos] = sum([Duracion Minutos]), \r");
            lsb.Append("NumRegsGrp = count(*)',  \r");
            lsb.Append("@InnerWhere = '([Nombre Tipo Destino] like ''%Ent%'')', \r");
            lsb.Append("@InnerGroup=@gb,  \r");
            lsb.Append("@OuterFields=@outerFieldsResult,  \r");
            lsb.Append("@OuterGroup='[Nombre Completo]', \r");
            lsb.Append("@Order='[Nombre Completo] Asc,[Total Minutos] Asc,[Total Llamadas] Asc', \r");
            lsb.Append("@OrderInv='[Nombre Completo] Desc,[Total Minutos] Desc,[Total Llamadas] Desc', \r");
            //lsb.Append("@Length=10, \r");
            //lsb.Append("@Start=0, \r");
            lsb.Append("@OrderDir='Asc', \r");
            lsb.Append(" @Usuario = " + Session["iCodUsuario"] + ", \r");
            lsb.Append(" @Perfil = " + Session["iCodPerfil"] + ", \r");
            lsb.Append("@FechaIniRep = '''" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00''', \r");
            lsb.Append("@FechaFinRep = '''" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59''', \r");
            lsb.Append("@TopN = '', \r");
            lsb.Append("@TopDir = '', \r");
            lsb.Append("@Moneda = '" + Session["Currency"] + "', \r");
            lsb.Append("@Idioma = 'Español' \r");

            return lsb.ToString();
        }

        protected string ConsultaExtension(int iCodExtension)
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" select top 1 vchCodigo from [VisHistoricos('Exten','Extensiones','Español')] \r");
            lsb.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= GETDATE() \r");
            lsb.Append(" and iCodCatalogo = " + iCodExtension);
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
            //eliminar el ultimos elemento de la lista
            ltNavegacion.RemoveAt(ltNavegacion.Count - 1);
            //ltNavegacion.RemoveAt(ltNavegacion.Count - 1);

            //obtener el ultimo elemento de la lista
            string lsLastElement = ltNavegacion[ltNavegacion.Count - 1];

            #region Etiqueta de navegacion

            //List<string> ltEtiquetaNavegacion = (List<string>)Session["estadoDeNavegacion"];
            ////eliminar los tres ultimos elementos de la lista
            //for (int i = 0; i < 2; i++)
            //{
            //    if (ltEtiquetaNavegacion.Count > 1)
            //    {
            //        ltEtiquetaNavegacion.RemoveAt(ltEtiquetaNavegacion.Count - 1);
            //    }
            //}m

            //Session["estadoDeNavegacion"] = ltEtiquetaNavegacion;

            #endregion

            HttpContext.Current.Response.Redirect(lsLastElement);
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExportXLS();
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            ExportPDF();
        }

        #endregion

        #region Exportacion

        public void ExportXLS()
        {
            if (iCodExten == 0 && ReporteTraficoLlam == 0 && DetallLlam == 0)
            {
                CrearXLSDashboard(".xlsx");
            }

            if (ReporteTraficoLlam == 1)
            {
                CrearXLSRepTraficoLlam(".xlsx");
            }

            if (iCodExten != 0)
            {
                CrearXLSRepDetallExten(".xlsx", iCodExten, tipoLlam);
            }
            if (DetallLlam == 1)
            {
                CrearXLSRepDetallNumMarcados(".xlsx", DetallLlam, NumMarcado);
            }
        }

        protected void CrearXLSDashboard(string lsExt)
        {
            DataTable table1 = DTIChartsAndControls.DataTable(ConsultaTop10PorMinutos());
            DataTable table2 = DTIChartsAndControls.DataTable(ConsultaLlamadasEntradaPorExtension());
            DataTable table3 = DTIChartsAndControls.DataTable(ConsultaLlamadasSalidaPorExtension());
            DataTable table4 = DTIChartsAndControls.DataTable(ConsultaNumerosMasMarcados());

            table2.Columns.Remove("iCodExten");
            table3.Columns.Remove("iCodExten");
            table4.Columns.Remove("iCodExten");

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\Dashboard" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel);

                #region Inserta grafica

                DataTable ldtGrafica = DSODataAccess.Execute(ConsultaTop10PorMinutos());

                crearGrafico(ldtGrafica, "Minutos por empleado", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Empleado", "", "", "Empleado", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaPrincipal}", "Reporte", "DatosGr", 700, 300);

                DataTable ldtGraficaLlamEnt1 = DTIChartsAndControls.DataTable(ConsultaLlamadasEntradaPorExtension());

                ldtGraficaLlamEnt1.Columns.Remove("Minutos");
                ldtGraficaLlamEnt1.Columns.Remove("Empleado");
                ldtGraficaLlamEnt1.Columns.Remove("iCodExten");

                crearGrafico(ldtGraficaLlamEnt1, "Llamadas por extensión", new string[] { "Llamadas" }, new string[] { "Llamadas" }, new string[] { "Llamadas" }, "Extension", "", "", "Extension", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaLlamEnt1}", "Reporte", "DatosGrLlamEnt1", 350, 250);

                DataTable ldtGraficaLlamEnt2 = DTIChartsAndControls.DataTable(ConsultaLlamadasEntradaPorExtension());

                ldtGraficaLlamEnt2.Columns.Remove("Llamadas");
                ldtGraficaLlamEnt2.Columns.Remove("Empleado");
                ldtGraficaLlamEnt2.Columns.Remove("iCodExten");

                crearGrafico(ldtGraficaLlamEnt2, "Minutos por extensión", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Extension", "", "", "Extension", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaLlamEnt2}", "Reporte", "DatosGrLlamEnt2", 350, 250);

                //salida

                DataTable ldtGraficaLlamSal1 = DTIChartsAndControls.DataTable(ConsultaLlamadasSalidaPorExtension());

                ldtGraficaLlamSal1.Columns.Remove("Minutos");
                ldtGraficaLlamSal1.Columns.Remove("Empleado");
                ldtGraficaLlamSal1.Columns.Remove("iCodExten");
                ldtGraficaLlamSal1.Columns.Remove("Costo");

                crearGrafico(ldtGraficaLlamSal1, "Llamadas por extensión", new string[] { "Llamadas" }, new string[] { "Llamadas" }, new string[] { "Llamadas" }, "Extension", "", "", "Extension", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaLlamSal1}", "Reporte", "DatosGrLlamSal1", 350, 250);

                DataTable ldtGraficaLlamSal2 = DTIChartsAndControls.DataTable(ConsultaLlamadasSalidaPorExtension());

                ldtGraficaLlamSal2.Columns.Remove("Llamadas");
                ldtGraficaLlamSal2.Columns.Remove("Empleado");
                ldtGraficaLlamSal2.Columns.Remove("iCodExten");
                ldtGraficaLlamSal2.Columns.Remove("Costo");

                crearGrafico(ldtGraficaLlamSal2, "Minutos por extensión", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Extension", "", "", "Extension", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaLlamSal2}", "Reporte", "DatosGrLlamSal2", 350, 250);

                //numeros marcados

                DataTable ldtGraficaNumMar = DTIChartsAndControls.DataTable(ConsultaNumerosMasMarcados());

                ldtGraficaNumMar.Columns.Remove("iCodExten");

                crearGrafico(ldtGraficaNumMar, "Numeros mas marcados", new string[] { "VecesMarcadas" }, new string[] { "VecesMarcadas" }, new string[] { "VecesMarcadas" }, "Numero", "", "", "Numero", "{0:0,0}",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaNumMar}", "Reporte", "DatosGrNumMar", 450, 250);

                #endregion

                if (table2.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table2, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Llamadas de entrada");
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

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

                    lExcel.Actualizar("Reporte", "TituloReporte1", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepLlamEntrada", false, datos, estilo);
                }

                if (table3.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table3, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Llamadas de salida");
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

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

                    lExcel.Actualizar("Reporte", "TituloReporte2", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepLlamSalida", false, datos, estilo);
                }

                if (table4.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table4, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Numeros mas marcados");
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

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

                    lExcel.Actualizar("Reporte", "TituloReporte3", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepNumMar", false, datos, estilo);
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


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes Dashboard ");
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

        protected void CrearXLSRepTraficoLlam(string lsExt)
        {
            DataTable table1 = DTIChartsAndControls.DataTable(ConsultaReporteTraficoLlamadas());

            #region Elimina columnas no necesarias
            if (table1.Columns.Contains("RID"))
                table1.Columns.Remove("RID");
            if (table1.Columns.Contains("RowNumber"))
                table1.Columns.Remove("RowNumber");
            if (table1.Columns.Contains("TopRID"))
                table1.Columns.Remove("TopRID");
            #endregion // Elimina columnas no necesarias en el gridview

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\RepTraficoLlamadas" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel);

                #region Inserta grafica

                DataTable ldtGrafica = DSODataAccess.Execute(ConsultaTop10PorMinutos());

                //ldtGrafica.Columns.Remove("Extension");
                //ldtGrafica.Columns.Remove("Llamadas");
                //ldtGrafica.Columns.Remove("Minutos");

                //crearGrafico(ldtGrafica, "Consumo por extensión", new string[] { "Costo" }, new string[] { "Costo" }, new string[] { "Costo" }, "Extension", "", "", "Extension", "#,0",
                //                 true, TipoGrafica.Barras, lExcel, "{GraficaPrincipal}", "Reporte", "DatosGr", 2500, 300);


                crearGrafico(ldtGrafica, "Minutos por empleado", new string[] { "Minutos" }, new string[] { "Minutos" }, new string[] { "Minutos" }, "Empleado", "", "", "Empleado", "#,0",
                                 true, TipoGrafica.Barras, lExcel, "{GraficaPrincipal}", "Reporte", "DatosGr", 700, 300);


                #endregion

                if (table1.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table1, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Reporte de trafico de llamadas");
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

                    EstiloTablaExcel estilo = new EstiloTablaExcel();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = true;
                    estilo.PrimeraColumna = false;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;
                    estilo.AutoFiltro = false;
                    estilo.AutoAjustarColumnas = true;

                    lExcel.Actualizar("Reporte", "TituloReporte1", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepTraficoLlamadas", false, datos, estilo);
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


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte Trafico Llamadas ");
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

        protected void CrearXLSRepDetallExten(string lsExt, int iCodExten, int tipoLlam)
        {
            DataTable table1 = DSODataAccess.Execute(ConsultaDetalleEmpleadoTDestCenCos(iCodExten, tipoLlam));

            #region Elimina columnas no necesarias
            if (table1.Columns.Contains("RID"))
                table1.Columns.Remove("RID");
            if (table1.Columns.Contains("RowNumber"))
                table1.Columns.Remove("RowNumber");
            if (table1.Columns.Contains("TopRID"))
                table1.Columns.Remove("TopRID");
            #endregion // Elimina columnas no necesarias en el gridview

            string tipoLlamadas = string.Empty;
            if (tipoLlam == 0)
            {
                tipoLlamadas = " llamadas de Salida ";
            }
            else
            {
                tipoLlamadas = " llamadas de Entrada ";
            }

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\RepDetalleExtension" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel);

                if (table1.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table1, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Detalle de " + tipoLlamadas);
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

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

                    lExcel.Actualizar("Reporte", "TituloReporte1", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepDetalleExtension", false, datos, estilo);
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

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Detalle " + tipoLlamadas + "Extension " + DSODataAccess.ExecuteScalar(ConsultaExtension(iCodExten)).ToString() + " ");
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

        protected void CrearXLSRepDetallNumMarcados(string lsExt, int DetallLlam, string NumMarcado)
        {
            DataTable table1 = DSODataAccess.Execute(ConsultaDetalleNumerosMasMarcados(NumMarcado.Replace("'", "")));

            #region Elimina columnas no necesarias
            if (table1.Columns.Contains("RID"))
                table1.Columns.Remove("RID");
            if (table1.Columns.Contains("RowNumber"))
                table1.Columns.Remove("RowNumber");
            if (table1.Columns.Contains("TopRID"))
                table1.Columns.Remove("TopRID");
            #endregion // Elimina columnas no necesarias en el gridview

            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\RepDetalleExtension" + lsExt);
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ProcesarTituloExcel(lExcel);

                if (table1.Rows.Count > 0)
                {
                    object[,] datos = lExcel.DataTableToArray(table1, true);
                    DataTable ldtTitulo = new DataTable();
                    ldtTitulo.Columns.Add("Detalle de numero marcado: " + NumMarcado);
                    object[,] titulo = lExcel.DataTableToArray(ldtTitulo, true);

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

                    lExcel.Actualizar("Reporte", "TituloReporte1", false, titulo, estilo);
                    lExcel.Actualizar("Reporte", "RepDetalleExtension", false, datos, estilo);
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

                ExportarArchivo(lsExt, psFileKey, psTempPath, "Detalle de numero marcado: " + NumMarcado.Replace("'", "") + " ");
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

        protected void ProcesarTituloExcel(ExcelAccess pExcel)
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

            //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                " where Esquema = '" + DSODataContext.Schema + "'" +
                                                " and dtinivigencia <> dtfinVigencia " +
                                                " and dtfinVigencia>getdate()");

            //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
            if (pRowCliente != null && pRowCliente["LogoExportacion"] != DBNull.Value && lhtMeta["{LogoCliente}"] != null)
            {
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                lsImg = pRowCliente["LogoExportacion"].ToString().Replace("~", "").Replace("/", "\\");
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
                ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).set_Value(System.Type.Missing, String.Empty);

                liRenTitulo = ((Microsoft.Office.Interop.Excel.Range)lhtMeta["{TituloReporte}"]).Row;

                liRenglon = Math.Max(liRenLogoCliente, liRenLogoKeytia);
                if (liRenglon > liRenTitulo && liRenTitulo > 1)
                {
                    pExcel.InsertarFilas("Reporte", liRenTitulo - 1, liRenglon - liRenTitulo + 1);
                }
            }
        }
        protected Hashtable BuscarTituloExcel(ExcelAccess pExcel)
        {
            Hashtable lhtRet = new Hashtable();

            lhtRet.Add("{LogoCliente}", pExcel.BuscarTexto("Reporte", "{LogoCliente}", true));
            lhtRet.Add("{LogoKeytia}", pExcel.BuscarTexto("Reporte", "{LogoKeytia}", true));
            lhtRet.Add("{TituloReporte}", pExcel.BuscarTexto("Reporte", "{TituloReporte}", true));

            return lhtRet;
        }

        #region Insertar grafico en excel

        protected void crearGrafico(DataTable ldt, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                             string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda, TipoGrafica tipoGraf,
                                             ExcelAccess lExcel, string textoPlantilla, string HojaGrafico, string HojaDatos, float anchoGraf, float alturaGraf)
        {
            ParametrosGrafica lparametrosGraf = parametrosDeGrafica(ldt, tituloGraf, columnaDatos, leyenda, serieId, EjeX, tituloEjeX, formatoEjeX, tituloEjeY,
                                                             formatoEjeY, mostrarLeyenda, tipoGraf);

            ProcesarGraficaExcel(HojaGrafico, HojaDatos, anchoGraf, alturaGraf, 0, 0, lparametrosGraf, lExcel, textoPlantilla);
        }

        protected ParametrosGrafica parametrosDeGrafica(DataTable lsDataSource, string tituloGraf, string[] columnaDatos, string[] leyenda, string[] serieId, string EjeX,
                                                                           string tituloEjeX, string formatoEjeX, string tituloEjeY, string formatoEjeY, bool mostrarLeyenda, TipoGrafica tipoGraf)
        {

            ParametrosGrafica lParametrosGrafica = new ParametrosGrafica();

            lParametrosGrafica.Datos = lsDataSource;
            lParametrosGrafica.Title = tituloGraf;
            lParametrosGrafica.DataColumns = columnaDatos.ToArray();
            lParametrosGrafica.SeriesNames = leyenda.ToArray();
            lParametrosGrafica.SeriesIds = serieId.ToArray();
            lParametrosGrafica.XColumn = EjeX;
            lParametrosGrafica.XIdsColumn = EjeX;
            lParametrosGrafica.XTitle = tituloEjeX;
            lParametrosGrafica.XFormat = formatoEjeX;
            lParametrosGrafica.YTitle = tituloEjeY;
            lParametrosGrafica.YFormat = formatoEjeY;
            lParametrosGrafica.ShowLegend = mostrarLeyenda;
            lParametrosGrafica.TipoGrafica = tipoGraf;

            return lParametrosGrafica;
        }

        protected Hashtable ProcesarGraficaExcel(string lsHojaGrafico, string lsHojaDatos, float lfWidth, float lfHeight, float lfOffsetX, float lfOffsetY,
                                                                ParametrosGrafica lParametrosGrafica, ExcelAccess lExcel, string cambiarTextoPorGraf)
        {
            FormatoGrafica lFormatoGrafica = new FormatoGrafica();
            lFormatoGrafica.Titulo = lParametrosGrafica.Title;
            lFormatoGrafica.Leyendas = lParametrosGrafica.ShowLegend;
            lFormatoGrafica.XFormat = lParametrosGrafica.XFormat;
            lFormatoGrafica.YFormat = lParametrosGrafica.YFormat;
            Microsoft.Office.Interop.Excel.XlChartType lCharType = GetTipoGraficaExcel(lParametrosGrafica.TipoGrafica);

            return lExcel.InsertarGrafico(lsHojaGrafico, lsHojaDatos, cambiarTextoPorGraf, lParametrosGrafica.XColumn, lParametrosGrafica.XTitle, lParametrosGrafica.DataColumns,
                     lParametrosGrafica.SeriesNames, lParametrosGrafica.Datos, lCharType, lfWidth, lfHeight, lfOffsetX, lfOffsetY, lFormatoGrafica);
        }

        protected Microsoft.Office.Interop.Excel.XlChartType GetTipoGraficaExcel(TipoGrafica lTipoGrafica)
        {
            if (lTipoGrafica == TipoGrafica.Pastel)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlPie;
            }
            else if (lTipoGrafica == TipoGrafica.Barras)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;
            }
            else if (lTipoGrafica == TipoGrafica.Lineas)
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlLine;
            }
            else
            {
                return Microsoft.Office.Interop.Excel.XlChartType.xlArea;
            }
        }

        #endregion

        public void ExportPDF()
        {
            if (ReporteTraficoLlam == 1)
            {
                CrearDOCRepTraficoLlam(".pdf");
            }
            else
            {
                CrearDOCDashboard(".pdf");
            }
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {

            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }

        protected void CrearDOCDashboard(string lsExt)
        {
            int plantilla = 0;

            WordAccess lWord = new WordAccess();
            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lWord.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\Dashboard" + plantilla + ".docx");

                lWord.Abrir();

                lWord.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                #region inserta logos
                string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
                string lsImg;

                //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
                DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                    " where Esquema = '" + DSODataContext.Schema + "'" +
                                                    " and dtinivigencia <> dtfinVigencia " +
                                                    " and dtfinVigencia>getdate()");

                //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                if (System.IO.File.Exists(lsImg))
                {
                    lWord.PosicionaCursor("{LogoCliente}");
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                    lWord.InsertarImagen(lsImg, 131, 40);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                }
                //LogoKeytia
                lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeaderdoc.png");
                if (System.IO.File.Exists(lsImg))
                {
                    lWord.ReemplazarTextoPorImagen("{LogoKeytia}", lsImg);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoKeytia}", "");
                }

                #endregion

                #region exporta a pdf
                Chart chart1 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafTop10Minutos");
                string tempFolder = System.Configuration.ConfigurationManager.AppSettings["TempFolder"];
                string ImageURL1 = tempFolder + "grafTop10Minutos.png";
                if (chart1 != null)
                {
                    chart1.SaveImage(ImageURL1);
                    lWord.PosicionaCursor("{GraficaDashboard}");
                    lWord.ReemplazarTexto("{GraficaDashboard}", "");
                    lWord.InsertarImagen(ImageURL1, 550, 300);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaDashboard}", "");
                }
                DataTable table1 = DTIChartsAndControls.DataTable(ConsultaLlamadasEntradaPorExtension());
                table1.Columns.Remove("iCodExten");
                if (table1.Rows.Count > 0)
                {
                    lWord.PosicionaCursor("{ReporteLlamadasEntrada}");
                    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                    EstiloTablaWord estilo = new EstiloTablaWord();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = true;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;

                    lWord.PosicionaCursor("{TituloReporteLlamadasEntrada}");
                    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "Llamadas de Entrada");

                    lWord.InsertarTabla(table1, estilo.FilaEncabezado, estilo.Estilo, estilo);
                    lWord.Tabla.Columns.AutoFit();
                }
                else
                {
                    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "");
                    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                }
                Chart chart2 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasEntrantes");
                Chart chart3 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasEntrantesMinutos");
                string ImageURL2 = tempFolder + "grafLlamadasEntrantes.png";
                string ImageURL3 = tempFolder + "grafLlamadasEntrantesMinutos.png";
                if (chart2 != null)
                {
                    chart2.SaveImage(ImageURL2);
                    lWord.PosicionaCursor("{GraficaLlamEntLlam}");
                    lWord.ReemplazarTexto("{GraficaLlamEntLlam}", "");
                    lWord.InsertarImagen(ImageURL2, 400, 250);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaLlamEntLlam}", "");
                }
                if (chart3 != null)
                {
                    chart3.SaveImage(ImageURL3);
                    lWord.PosicionaCursor("{GraficaLlamEntMin}");
                    lWord.ReemplazarTexto("{GraficaLlamEntMin}", "");
                    lWord.InsertarImagen(ImageURL3, 400, 250);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaLlamEntMin}", "");
                }
                DataTable table2 = DTIChartsAndControls.DataTable(ConsultaLlamadasSalidaPorExtension());
                table2.Columns.Remove("iCodExten");
                if (table2.Rows.Count > 0)
                {
                    lWord.PosicionaCursor("{ReporteLlamadasSalida}");
                    lWord.ReemplazarTexto("{ReporteLlamadasSalida}", "");
                    EstiloTablaWord estilo = new EstiloTablaWord();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = true;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;

                    lWord.PosicionaCursor("{TituloReporteLlamadasSalida}");
                    lWord.ReemplazarTexto("{TituloReporteLlamadasSalida}", "Llamadas de Salida");

                    lWord.InsertarTabla(table2, estilo.FilaEncabezado, estilo.Estilo, estilo);
                    lWord.Tabla.Columns.AutoFit();
                }
                else
                {
                    lWord.ReemplazarTexto("{TituloReporteLlamadasSalida}", "");
                    lWord.ReemplazarTexto("{ReporteLlamadasSalida}", "");
                }
                Chart chart4 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasSalida");
                Chart chart5 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasSalidaMinutos");
                string ImageURL4 = tempFolder + "grafLlamadasSalida.png";
                string ImageURL5 = tempFolder + "grafLlamadasSalidaMinutos.png";
                if (chart4 != null)
                {
                    chart4.SaveImage(ImageURL4);
                    lWord.PosicionaCursor("{GraficaLlamSalLlam}");
                    lWord.ReemplazarTexto("{GraficaLlamSalLlam}", "");
                    lWord.InsertarImagen(ImageURL4, 400, 250);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaLlamSalLlam}", "");
                }
                if (chart5 != null)
                {
                    chart5.SaveImage(ImageURL5);
                    lWord.PosicionaCursor("{GraficaLlamSalMin}");
                    lWord.ReemplazarTexto("{GraficaLlamSalMin}", "");
                    lWord.InsertarImagen(ImageURL5, 400, 250);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaLlamSalMin}", "");
                }
                DataTable table3 = DTIChartsAndControls.DataTable(ConsultaNumerosMasMarcados());
                table3.Columns.Remove("iCodExten");
                if (table3.Rows.Count > 0)
                {
                    lWord.PosicionaCursor("{ReporteNumerosMasMarcados}");
                    lWord.ReemplazarTexto("{ReporteNumerosMasMarcados}", "");
                    EstiloTablaWord estilo = new EstiloTablaWord();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = true;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;

                    lWord.PosicionaCursor("{TituloReporteNumerosMasMarcados}");
                    lWord.ReemplazarTexto("{TituloReporteNumerosMasMarcados}", "Numeros mas Marcados");

                    lWord.InsertarTabla(table3, estilo.FilaEncabezado, estilo.Estilo, estilo);
                    lWord.Tabla.Columns.AutoFit();
                }
                else
                {
                    lWord.ReemplazarTexto("{TituloReporteNumerosMasMarcados}", "");
                    lWord.ReemplazarTexto("{ReporteNumerosMasMarcados}", "");
                }
                Chart chart6 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafNumerosMasMarcados");
                string ImageURL6 = tempFolder + "grafNumerosMasMarcados.png";
                if (chart6 != null)
                {
                    chart6.SaveImage(ImageURL6);
                    lWord.PosicionaCursor("{GraficaNumerosMasMarcados}");
                    lWord.ReemplazarTexto("{GraficaNumerosMasMarcados}", "");
                    lWord.InsertarImagen(ImageURL6, 550, 300);
                }
                else
                {
                    lWord.ReemplazarTexto("{GraficaNumerosMasMarcados}", "");
                }

                #endregion exporta a pdf

                #region exporta grafica
                //Chart chart1 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafTop10Minutos");
                //string tempFolder = System.Configuration.ConfigurationManager.AppSettings["TempFolder"];
                //string ImageURL1 = tempFolder + "grafTop10Minutos.png";
                //Chart chart2 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasEntrantes");
                //Chart chart3 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasEntrantesMinutos");
                //string ImageURL2 = tempFolder + "grafLlamadasEntrantes.png";
                //string ImageURL3 = tempFolder + "grafLlamadasEntrantesMinutos.png";
                //Chart chart4 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasSalida");
                //Chart chart5 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafLlamadasSalidaMinutos");
                //string ImageURL4 = tempFolder + "grafLlamadasSalida.png";
                //string ImageURL5 = tempFolder + "grafLlamadasSalidaMinutos.png";
                //Chart chart6 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafNumerosMasMarcados");
                //string ImageURL6 = tempFolder + "grafNumerosMasMarcados.png";
                //if (chart1 != null)
                //{
                //    chart1.SaveImage(ImageURL1);
                //    lWord.PosicionaCursor("{GraficaDashboard}");
                //    lWord.ReemplazarTexto("{GraficaDashboard}", "");
                //    lWord.InsertarImagen(ImageURL1, 550, 300);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaDashboard}", "");
                //}

                //if (chart2 != null)
                //{
                //    chart2.SaveImage(ImageURL2);
                //    lWord.PosicionaCursor("{GraficaLlamEntLlam}");
                //    lWord.ReemplazarTexto("{GraficaLlamEntLlam}", "");
                //    lWord.InsertarImagen(ImageURL2, 400, 250);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaLlamEntLlam}", "");
                //}
                //if (chart3 != null)
                //{
                //    chart3.SaveImage(ImageURL3);
                //    lWord.PosicionaCursor("{GraficaLlamEntMin}");
                //    lWord.ReemplazarTexto("{GraficaLlamEntMin}", "");
                //    lWord.InsertarImagen(ImageURL3, 400, 250);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaLlamEntMin}", "");
                //}
                //if (chart4 != null)
                //{
                //    chart4.SaveImage(ImageURL4);
                //    lWord.PosicionaCursor("{GraficaLlamSalLlam}");
                //    lWord.ReemplazarTexto("{GraficaLlamSalLlam}", "");
                //    lWord.InsertarImagen(ImageURL4, 400, 250);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaLlamSalLlam}", "");
                //}
                //if (chart5 != null)
                //{
                //    chart5.SaveImage(ImageURL5);
                //    lWord.PosicionaCursor("{GraficaLlamSalMin}");
                //    lWord.ReemplazarTexto("{GraficaLlamSalMin}", "");
                //    lWord.InsertarImagen(ImageURL5, 400, 250);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaLlamSalMin}", "");
                //}
                //if (chart6 != null)
                //{
                //    chart6.SaveImage(ImageURL6);
                //    lWord.PosicionaCursor("{GraficaNumerosMasMarcados}");
                //    lWord.ReemplazarTexto("{GraficaNumerosMasMarcados}", "");
                //    lWord.InsertarImagen(ImageURL6, 550, 300);
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{GraficaNumerosMasMarcados}", "");
                //}
                #endregion

                #region exporta Grids
                //DataTable table1 = DTIChartsAndControls.DataTable(ConsultaLlamadasEntradaPorExtension());
                //DataTable table2 = DTIChartsAndControls.DataTable(ConsultaLlamadasSalidaPorExtension());                
                //DataTable table3 = DTIChartsAndControls.DataTable(ConsultaNumerosMasMarcados());
                //table1.Columns.Remove("iCodExten");
                //table2.Columns.Remove("iCodExten");
                //table3.Columns.Remove("iCodExten");                
                //if (table1.Rows.Count > 0)
                //{
                //    lWord.PosicionaCursor("{ReporteLlamadasEntrada}");
                //    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                //    EstiloTablaWord estilo = new EstiloTablaWord();
                //    estilo.Estilo = "KeytiaGrid";
                //    estilo.FilaEncabezado = true;
                //    estilo.FilasBandas = true;
                //    estilo.FilaTotales = false;
                //    estilo.PrimeraColumna = true;
                //    estilo.UltimaColumna = true;
                //    estilo.ColumnasBandas = false;

                //    lWord.PosicionaCursor("{TituloReporteLlamadasEntrada}");
                //    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "Llamadas de Entrada");

                //    lWord.InsertarTabla(table1, estilo.FilaEncabezado, estilo.Estilo, estilo);
                //    lWord.Tabla.Columns.AutoFit();
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "");
                //    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                //}                
                //if (table2.Rows.Count > 0)
                //{
                //    lWord.PosicionaCursor("{ReporteLlamadasSalida}");
                //    lWord.ReemplazarTexto("{ReporteLlamadasSalida}", "");
                //    EstiloTablaWord estilo = new EstiloTablaWord();
                //    estilo.Estilo = "KeytiaGrid";
                //    estilo.FilaEncabezado = true;
                //    estilo.FilasBandas = true;
                //    estilo.FilaTotales = false;
                //    estilo.PrimeraColumna = true;
                //    estilo.UltimaColumna = true;
                //    estilo.ColumnasBandas = false;

                //    lWord.PosicionaCursor("{TituloReporteLlamadasSalida}");
                //    lWord.ReemplazarTexto("{TituloReporteLlamadasSalida}", "Llamadas de Salida");

                //    lWord.InsertarTabla(table2, estilo.FilaEncabezado, estilo.Estilo, estilo);
                //    lWord.Tabla.Columns.AutoFit();
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{TituloReporteLlamadasSalida}", "");
                //    lWord.ReemplazarTexto("{ReporteLlamadasSalida}", "");
                //}
                //if (table3.Rows.Count > 0)
                //{
                //    lWord.PosicionaCursor("{ReporteNumerosMasMarcados}");
                //    lWord.ReemplazarTexto("{ReporteNumerosMasMarcados}", "");
                //    EstiloTablaWord estilo = new EstiloTablaWord();
                //    estilo.Estilo = "KeytiaGrid";
                //    estilo.FilaEncabezado = true;
                //    estilo.FilasBandas = true;
                //    estilo.FilaTotales = false;
                //    estilo.PrimeraColumna = false;
                //    estilo.UltimaColumna = true;
                //    estilo.ColumnasBandas = false;

                //    lWord.PosicionaCursor("{TituloReporteNumerosMasMarcados}");
                //    lWord.ReemplazarTexto("{TituloReporteNumerosMasMarcados}", "Numeros mas Marcados");

                //    lWord.InsertarTabla(table3, estilo.FilaEncabezado, estilo.Estilo, estilo);
                //    lWord.Tabla.Columns.AutoFit();
                //}
                //else
                //{
                //    lWord.ReemplazarTexto("{TituloReporteNumerosMasMarcados}", "");
                //    lWord.ReemplazarTexto("{ReporteNumerosMasMarcados}", "");
                //}

                #endregion

                if (File.Exists(ImageURL1))
                {
                    File.Delete(ImageURL1);
                }
                if (File.Exists(ImageURL2))
                {
                    File.Delete(ImageURL2);
                }
                if (File.Exists(ImageURL3))
                {
                    File.Delete(ImageURL3);
                }
                if (File.Exists(ImageURL4))
                {
                    File.Delete(ImageURL4);
                }
                if (File.Exists(ImageURL5))
                {
                    File.Delete(ImageURL5);
                }
                if (File.Exists(ImageURL6))
                {
                    File.Delete(ImageURL6);
                }

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lWord.FilePath = lsFileName;


                lWord.SalvarComo();


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reportes Dashboard ");
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

                }
            }
        }

        protected void CrearDOCRepTraficoLlam(string lsExt)
        {
            int plantilla = 0;

            WordAccess lWord = new WordAccess();
            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lWord.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoard\Dashboard" + plantilla + ".docx");

                lWord.Abrir();

                lWord.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                #region inserta logos
                string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
                string lsImg;

                //NZ 20150707 Se cambio el campo con el que se compara el esquema en la tabla de Clientes. En ves de vchcodigo se comprara con Esquema.
                DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                    " where Esquema = '" + DSODataContext.Schema + "'" +
                                                    " and dtinivigencia <> dtfinVigencia " +
                                                    " and dtfinVigencia>getdate()");

                //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                if (System.IO.File.Exists(lsImg))
                {
                    lWord.PosicionaCursor("{LogoCliente}");
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                    lWord.InsertarImagen(lsImg, 131, 40);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                }
                //LogoKeytia
                lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeaderdoc.png");
                if (System.IO.File.Exists(lsImg))
                {
                    lWord.ReemplazarTextoPorImagen("{LogoKeytia}", lsImg);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoKeytia}", "");
                }

                #endregion

                #region exporta grafica
                Chart chart1 = (Chart)pnlContenedorPrincipalDashR3.FindControl("grafCostoExten");

                string tempFolder = System.Configuration.ConfigurationManager.AppSettings["TempFolder"];

                string ImageURL1 = tempFolder + "grafCostoExten.png";

                if (chart1 != null)
                {
                    chart1.SaveImage(ImageURL1);
                    lWord.PosicionaCursor("{Grafica}");
                    lWord.ReemplazarTexto("{Grafica}", "");
                    lWord.InsertarImagen(ImageURL1, 600, 400);
                }
                else
                {
                    lWord.ReemplazarTexto("{Grafica}", "");
                }

                #endregion

                #region exporta Grid

                DataTable table1 = DTIChartsAndControls.DataTable(ConsultaReporteTraficoLlamadas());

                #region Elimina columnas no necesarias
                if (table1.Columns.Contains("RID"))
                    table1.Columns.Remove("RID");
                if (table1.Columns.Contains("RowNumber"))
                    table1.Columns.Remove("RowNumber");
                if (table1.Columns.Contains("TopRID"))
                    table1.Columns.Remove("TopRID");
                #endregion // Elimina columnas no necesarias en el gridview

                if (table1.Rows.Count > 0)
                {
                    lWord.PosicionaCursor("{ReporteLlamadasEntrada}");
                    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                    EstiloTablaWord estilo = new EstiloTablaWord();
                    estilo.Estilo = "KeytiaGrid";
                    estilo.FilaEncabezado = true;
                    estilo.FilasBandas = true;
                    estilo.FilaTotales = false;
                    estilo.PrimeraColumna = false;
                    estilo.UltimaColumna = true;
                    estilo.ColumnasBandas = false;

                    lWord.PosicionaCursor("{TituloReporteLlamadasEntrada}");
                    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "");

                    lWord.InsertarTabla(table1, estilo.FilaEncabezado, estilo.Estilo, estilo);
                    lWord.Tabla.Columns.AutoFit();
                }
                else
                {
                    lWord.ReemplazarTexto("{TituloReporteLlamadasEntrada}", "");
                    lWord.ReemplazarTexto("{ReporteLlamadasEntrada}", "");
                }

                #endregion

                if (File.Exists(ImageURL1))
                {
                    File.Delete(ImageURL1);
                }

                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
                Session[psFileKey] = lsFileName;

                lWord.FilePath = lsFileName;


                lWord.SalvarComo();


                ExportarArchivo(lsExt, psFileKey, psTempPath, "Reporte Trafico Llamadas ");
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

                }
            }
        }

        private DataTable GetDataTable(GridView dtg)
        {
            DataTable dt = new DataTable();

            // add the columns to the datatable            
            if (dtg.HeaderRow != null)
            {

                for (int i = 0; i < dtg.HeaderRow.Cells.Count; i++)
                {
                    if (dt.Columns.Contains(dtg.HeaderRow.Cells[i].Text))
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text + i);
                    }
                    else
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text.Replace("&#243;", "ó"));
                    }
                }
            }

            //  add each of the data rows to the table
            foreach (GridViewRow row in dtg.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Replace("&nbsp;", "");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #endregion
    }
}
