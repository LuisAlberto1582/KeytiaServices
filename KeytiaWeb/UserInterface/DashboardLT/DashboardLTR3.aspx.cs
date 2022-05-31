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
    public partial class DashboardLTR3 : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        /*paginaLocal se usa para mandar el nombre de la clase donde se produjo el error en el log de web*/
        string paginaLocal = "KeytiaWeb.UserInterface.DashboardLT.DashboardLTR3.aspx.cs";
        /*nombrePagina se usa para poder cambiarle el nombre a la pagina una ves que se pasen cambios a producción*/
        string nombrePagina = "DashboardLTR3.aspx";

        //Variables para almacenar los valores leidos en el query string y para utilizar en consulta
        string Con = string.Empty;
        string Tel = string.Empty;

        string Etiqueta = string.Empty;
        string NavegacionNivel = string.Empty;

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
            #endregion // -- Almacenar en variable de sesion los urls de navegacion

            if (!Page.IsPostBack)
            {
                #region Inicia los valores default de los controles de fecha
                try
                {
                    if (Session["FechaInicioRepDashLTR3"] != null && Session["FechaFinRepDashLTR3"] != null)
                    {
                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRepDashLTR3"].ToString().Substring(1, 10));
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashLTR3"].ToString().Substring(1, 10));
                    }

                    else
                    {
                        DataTable fechas = DSODataAccess.Execute(consultaFechasUltimoMesTasado());

                        DateTime fechaInicio = (DateTime)fechas.Rows[0].ItemArray[0];
                        DateTime fechaFinal = (DateTime)fechas.Rows[0].ItemArray[1];

                        pdtInicio.CreateControls();
                        pdtInicio.DataValue = (object)fechaInicio;
                        pdtFin.CreateControls();
                        pdtFin.DataValue = (object)fechaFinal;
                    }

                    Session["FechaInicioRepDashLTR3"] = pdtInicio.DataValue.ToString();
                    Session["FechaFinRepDashLTR3"] = pdtFin.DataValue.ToString();
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

            Session["FechaInicioRepDashLTR3"] = pdtInicio.DataValue.ToString();
            Session["FechaFinRepDashLTR3"] = pdtFin.DataValue.ToString();

            if (Session["FechaInicioRepDashLTR3"] != null && Session["FechaFinRepDashLTR3"] != null)
            {
                pdtInicio.DataValue = (object)Convert.ToDateTime(Session["FechaInicioRepDashLTR3"].ToString().Substring(1, 10));
                pdtFin.DataValue = (object)Convert.ToDateTime(Session["FechaFinRepDashLTR3"].ToString().Substring(1, 10));
            }

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

            #region Crear tabla para indicadores

            Table tblIndicadores = new Table();
            tblIndicadores.Width = Unit.Percentage(100);

            TableRow tbltblIndicadorestr1 = new TableRow();
            TableCell tr1tc1 = new TableCell();
            tr1tc1.Width = Unit.Percentage(25);
            TableCell tr1tc2 = new TableCell();
            tr1tc2.Width = Unit.Percentage(25);
            TableCell tr1tc3 = new TableCell();
            tr1tc3.Width = Unit.Percentage(25);
            TableCell tr1tc4 = new TableCell();
            tr1tc4.Width = Unit.Percentage(25);

            tbltblIndicadorestr1.Controls.Add(tr1tc1);
            tbltblIndicadorestr1.Controls.Add(tr1tc2);
            tbltblIndicadorestr1.Controls.Add(tr1tc3);
            tbltblIndicadorestr1.Controls.Add(tr1tc4);

            tblIndicadores.Controls.Add(tbltblIndicadorestr1);


            #endregion

            #region Lee Query String

            #region Revisar si el querystring nav contiene un valor
            if (!string.IsNullOrEmpty(Request.QueryString["nav"]))
            {
                try
                {
                    NavegacionNivel = Request.QueryString["nav"];
                }
                catch (Exception ex)
                {
                    throw new KeytiaWebException(
                        "Ocurrio un error al leer el valor del querystring (nav) en " + paginaLocal
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }
            else
            {
                NavegacionNivel = "";
            }


            #endregion

            #endregion

            #region Reportes de año actual vs año anterior

            if (NavegacionNivel == "ActVsAnt")
            {

                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);
                tblReportestr1tc1.ColumnSpan = 2;

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                TableRow tblReportestr2 = new TableRow();

                TableCell tblReportestr2tc1 = new TableCell();
                tblReportestr2tc1.Width = Unit.Percentage(50);

                TableCell tblReportestr2tc2 = new TableCell();
                tblReportestr2tc2.Width = Unit.Percentage(50);

                tblReportestr2.Controls.Add(tblReportestr2tc1);
                tblReportestr2.Controls.Add(tblReportestr2tc2);

                tblReportes.Controls.Add(tblReportestr1);
                tblReportes.Controls.Add(tblReportestr2);

                #endregion // -- Creo una tabla que contendra los reportes

                DataTable grafConsHist = DSODataAccess.Execute(consultaAnioActVsAnioAnterior());

                tblReportestr1tc1.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GraficaLinea(grafConsHist, "Anio", "Nombre Mes", "Total",
                    "", 400, 1000, "Gráfica Consumo Histórico", "Mes", "Total", "c2", "grafConsHist"),
                    "Grafica Historica", "Gráfica de Consumo Historico", 450, ""));

                DataTable dtAnioAnt = grafConsHist.Select("[Anio] = " + (DateTime.Today.Year - 1)).CopyToDataTable();
                DataView dvAnioAnt = new DataView(dtAnioAnt);
                dtAnioAnt = dvAnioAnt.ToTable(false, new string[] { "Mes Anio", "Total", "Minutos", "Llamadas" });
                dtAnioAnt.Columns[0].ColumnName = "Mes";
                dtAnioAnt.Columns[1].ColumnName = "Costo";
                dtAnioAnt.Columns[2].ColumnName = "Minutos";
                dtAnioAnt.Columns[3].ColumnName = "Llamadas";
                tblReportestr2tc1.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("dtAnioAnt", dtAnioAnt, true, "Total", new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }), "Reporte", "Año anterior", 0));

                DataTable dtAnioAct = grafConsHist.Select("[Anio] = " + (DateTime.Today.Year)).CopyToDataTable();
                DataView dvAnioAct = new DataView(dtAnioAct);
                dtAnioAct = dvAnioAct.ToTable(false, new string[] { "Mes Anio", "Total", "Minutos", "Llamadas" });
                dtAnioAct.Columns[0].ColumnName = "Mes";
                dtAnioAct.Columns[1].ColumnName = "Costo";
                dtAnioAct.Columns[2].ColumnName = "Minutos";
                dtAnioAct.Columns[3].ColumnName = "Llamadas";
                tblReportestr2tc2.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("dtAnioAct", dtAnioAct, true, "Total", new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }), "Reporte", "Año actual", 0));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }

            #endregion // -- Reportes de año actual vs año anterior

            #region Reporte facturación Telmex - Distribución por UEN

            if (NavegacionNivel == "FactPorUEN")
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                TableRow tblReportestr2 = new TableRow();

                TableCell tblReportestr2tc1 = new TableCell();
                tblReportestr2tc1.Width = Unit.Percentage(100);

                tblReportestr2.Controls.Add(tblReportestr2tc1);

                tblReportes.Controls.Add(tblReportestr1);
                tblReportes.Controls.Add(tblReportestr2);

                #endregion // -- Creo una tabla que contendra los reportes

                Panel lpnl = new Panel();

                LinkButton lbtnPorGpo = new LinkButton();
                lbtnPorGpo.Text = "Ver por grupo";
                lbtnPorGpo.Font.Bold = true;
                lbtnPorGpo.Font.Size = 10;
                lbtnPorGpo.Style.Add(HtmlTextWriterStyle.Color, "Black");
                lbtnPorGpo.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=FactPorUEN" + "&VerPor=Grupo";
                LinkButton lbtnPorRazSoc = new LinkButton();
                lbtnPorRazSoc.Text = "Ver por rázon social";
                lbtnPorRazSoc.Font.Bold = true;
                lbtnPorRazSoc.Font.Size = 10;
                lbtnPorRazSoc.Style.Add(HtmlTextWriterStyle.Color, "Black");
                lbtnPorRazSoc.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=FactPorUEN" + "&VerPor=RazSoc";

                lpnl.Controls.Add(lbtnPorGpo);
                lpnl.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;"));
                lpnl.Controls.Add(lbtnPorRazSoc);

                tblReportestr1tc1.Controls.Add(DTIChartsAndControls.BordesReporte(lpnl, 0));

                if (Request.QueryString["VerPor"] == "Grupo")
                {
                    DataTable consPorGrupo = DSODataAccess.Execute(consultaFactHistoricaPorGrupo());

                    tblReportestr2tc1.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("consPorGrupo",
                        DTIChartsAndControls.ordenaTabla(consPorGrupo, "[" + consPorGrupo.Columns[0].ColumnName + "] asc, " + "[" + consPorGrupo.Columns[1].ColumnName + "] asc"),
                        true, "Total", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:0,0}" }),
                        "Reporte", "Consumo por grupo", 0));
                }

                if (Request.QueryString["VerPor"] == "RazSoc" || string.IsNullOrEmpty(Request.QueryString["VerPor"]))
                {
                    DataTable consPorUEN = DSODataAccess.Execute(consultaFactHistoricaPorUEN());

                    tblReportestr2tc1.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("consPorUEN",
                        DTIChartsAndControls.ordenaTabla(consPorUEN, "[" + consPorUEN.Columns[0].ColumnName + "] asc, " + "[" + consPorUEN.Columns[1].ColumnName + "] asc"),
                        true, "Total", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:0,0}" }),
                        "Reporte", "Consumo por UEN", 0));
                }



                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }

            #endregion // -- Reporte facturación Telmex - Distribución por UEN

            #region Reporte desglose cargos rentas y otros

            if (NavegacionNivel == "DesgCargRenYOtr")
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                tblReportes.Controls.Add(tblReportestr1);

                #endregion // -- Creo una tabla que contendra los reportes

                DataTable dtDesgCargRenYOtr = DSODataAccess.Execute(consultaDesgloseCargosRentasYOtros());

                tblReportestr1tc1.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("cons3Meses", DTIChartsAndControls.ordenaTabla(dtDesgCargRenYOtr, "[Rázon social] asc"), true, "Total",
                        new string[] { "", "", "", "", "{0:c}" }), "Reporte", "Desglose de cargos por rentas y otros", 450));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }

            #endregion // --Reporte desglose cargos rentas y otros

            #region Servicios Infinitum

            if (NavegacionNivel == "ServInfi")
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                tblReportes.Controls.Add(tblReportestr1);

                #endregion // -- Creo una tabla que contendra los reportes

                DataTable dtServInfi = DSODataAccess.Execute(consultaServInfinitum());

                tblReportestr1tc1.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("dtServInfi", DTIChartsAndControls.ordenaTabla(dtServInfi, "[Rázon social] asc"), true, "Total",
                        new string[] { "", "{0:c}", "{0:0,0}" }), "Reporte", "Servicios infinitum", 450));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }

            #endregion // --Servicios Infinitum

            #region Detalle Servicios Infinitum

            if (NavegacionNivel == "DetServInfi")
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                tblReportes.Controls.Add(tblReportestr1);

                #endregion // -- Creo una tabla que contendra los reportes

                DataTable dtDetServInfi = DSODataAccess.Execute(consultaDetalleServInfinitum());

                tblReportestr1tc1.Controls.Add(
                    DTIChartsAndControls.tituloYBordesReporte(
                        DTIChartsAndControls.GridView("cons3Meses", DTIChartsAndControls.ordenaTabla(dtDetServInfi, "[Rázon social] asc"), true, "Total",
                        new string[] { "", "", "", "", "", "{0:c}", "", "" }), "Reporte", "Detalle servicios infinitum", 450));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }

            #endregion // -- Detalle Servicios Infinitum

            #region Reporte por linea

            if (NavegacionNivel == "RepPorLinea")
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                //tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(100);

                tblReportestr1.Controls.Add(tblReportestr1tc1);

                tblReportes.Controls.Add(tblReportestr1);

                #endregion // -- Creo una tabla que contendra los reportes

                DataTable dtRepPorLinea = DSODataAccess.Execute(consultaReportePorLinea());

                //tblReportestr1tc1.Controls.Add(
                //    DTIChartsAndControls.tituloYBordesReporte(
                //        DTIChartsAndControls.GridView("dtRepPorLinea", dtRepPorLinea, true, "Total", true, 450,
                //        true, 4, typeof(decimal)), "Reporte", "Reporte por linea", 450));

                tblReportestr1tc1.Controls.Add(
                       DTIChartsAndControls.tituloYBordesReporte(
                           DTIChartsAndControls.GridView("dtRepPorLinea", DTIChartsAndControls.ordenaTabla(dtRepPorLinea, "[Rázon social] asc"), true, "Total"),
                           "Reporte", "Reporte por linea", 450));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            }
            #endregion // -- Reporte por linea

            //#region Reporte por linea Prueba

            //if (NavegacionNivel == "RepPorLineaPrueba")
            //{
            //    #region Creo una tabla que contendra los reportes

            //    Table tblReportes = new Table();
            //    //tblReportes.Width = Unit.Percentage(100);

            //    TableRow tblReportestr1 = new TableRow();

            //    TableCell tblReportestr1tc1 = new TableCell();
            //    tblReportestr1tc1.Width = Unit.Percentage(100);

            //    tblReportestr1.Controls.Add(tblReportestr1tc1);

            //    tblReportes.Controls.Add(tblReportestr1);

            //    #endregion // -- Creo una tabla que contendra los reportes

            //    DataTable dtRepPorLinea = DSODataAccess.Execute(consultaReportePorLinea());

            //    //tblReportestr1tc1.Controls.Add(
            //    //    DTIChartsAndControls.tituloYBordesReporte(
            //    //        DTIChartsAndControls.GridView("dtRepPorLinea", dtRepPorLinea, true, "Total", true, 450,
            //    //        true, 4, typeof(decimal)), "Reporte", "Reporte por linea", 450));

            //    tblReportestr1tc1.Controls.Add(
            //           DTIChartsAndControls.tituloYBordesReporte(
            //               //DTIChartsAndControls.GridView("dtRepPorLinea", DTIChartsAndControls.ordenaTabla(dtRepPorLinea, "[Rázon social] asc"), true, "Total", true, 450),
            //               //"Reporte", "Detalle servicios infinitum", 450));
            //                DTIChartsAndControls.GridView("dtRepPorLinea", DTIChartsAndControls.ordenaTabla(dtRepPorLinea, "[Rázon social] asc"), true, "Total",true,450, true),
            //               "Reporte", "Reporte por linea", 450, dtRepPorLinea));

            //    /*Se agregan los controles al contenedor principal*/
            //    pnlMainContainer.Controls.Add(tblReportes);
            //    pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            //}
            //#endregion // -- Reporte por linea Prueba

            #region Menu De pruebas

            if (NavegacionNivel == "MenuPruebas")
            {
                LinkButton dashRj = new LinkButton();
                dashRj.Text = "Dashboard propuesto RJ";
                dashRj.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=DashPrin";
                LinkButton repHistorico = new LinkButton();
                repHistorico.Text = "Comparativo 2 años";
                repHistorico.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=ActVsAnt";
                LinkButton repCons3Meses = new LinkButton();
                repCons3Meses.Text = "Reporte Consumo 3 meses";
                repCons3Meses.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=Cons3Meses";
                LinkButton DesgCargRenYOtr = new LinkButton();
                DesgCargRenYOtr.Text = "Reporte desglose cargos renta y otros";
                DesgCargRenYOtr.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=DesgCargRenYOtr";
                LinkButton ServInfi = new LinkButton();
                ServInfi.Text = "Servicios infinitum";
                ServInfi.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=ServInfi";
                LinkButton DetServInfi = new LinkButton();
                DetServInfi.Text = "Detalle servicios infinitum";
                DetServInfi.PostBackUrl = "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=DetServInfi";

                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(dashRj);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(repHistorico);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(repCons3Meses);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(DesgCargRenYOtr);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(ServInfi);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
                pnlMainContainer.Controls.Add(DetServInfi);
                pnlMainContainer.Controls.Add(new LiteralControl("<br /><br /><br />"));
            }

            #endregion

            //#region Dashboard principal
            //if (NavegacionNivel == "")
            //{
            //    #region Indicadores

            //    //int indicador1 = (int)DSODataAccess.ExecuteScalar(consultaIndicadorLineasContratadasDashInicial());
            //    //tr1tc1.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(indicador1.ToString(), "Lineas Contratadas"));

            //    //int indicador2 = (int)DSODataAccess.ExecuteScalar(consultaIndicadorLineasFijas());
            //    //tr1tc2.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(indicador2.ToString(), "Lineas Fijas"));

            //    //int indicador3 = 0;
            //    //tr1tc3.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(indicador3.ToString(), "Lineas Móviles"));

            //    //int indicador4 = (int)DSODataAccess.ExecuteScalar(consultaIndicadorNuevasLineasDashInicial());
            //    //if (indicador4 <= 0)
            //    //{
            //    //    tr1tc4.Controls.Add(DTIChartsAndControls.IndicadorHorizontal("0", "Nuevas Lineas"));
            //    //}
            //    //else
            //    //{
            //    //    tr1tc4.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(indicador4.ToString(), "Nuevas Lineas"));
            //    //}

            //    tr1tc1.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasFacturadas()).ToString(), "Lineas Facturadas"));

            //    tr1tc2.Controls.Add(DTIChartsAndControls.IndicadorHorizontal("150", "Enlaces"));

            //    tr1tc3.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasNuevas()).ToString(), "Lineas Nuevas"));

            //    tr1tc4.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasBaja()).ToString(), "Lineas Baja"));

            //    #endregion

            //    #region Creo una tabla que contendra los reportes

            //    Table tblReportes = new Table();
            //    tblReportes.Width = Unit.Percentage(100);

            //    TableRow tblReportestr1 = new TableRow();

            //    TableCell tblReportestr1tc1 = new TableCell();
            //    tblReportestr1tc1.Width = Unit.Percentage(50);

            //    TableCell tblReportestr1tc2 = new TableCell();
            //    tblReportestr1tc1.Width = Unit.Percentage(50);

            //    tblReportestr1.Controls.Add(tblReportestr1tc1);
            //    tblReportestr1.Controls.Add(tblReportestr1tc2);

            //    TableRow tblReportestr2 = new TableRow();

            //    TableCell tblReportestr2tc1 = new TableCell();
            //    tblReportestr2tc1.Width = Unit.Percentage(50);

            //    TableCell tblReportestr2tc2 = new TableCell();
            //    tblReportestr2tc2.Width = Unit.Percentage(50);

            //    tblReportestr2.Controls.Add(tblReportestr2tc1);
            //    tblReportestr2.Controls.Add(tblReportestr2tc2);

            //    tblReportes.Controls.Add(tblReportestr1);
            //    tblReportes.Controls.Add(tblReportestr2);


            //    #endregion

            //    #region Reportes de dashboard

            //    #region    Grafica Consumo Historico

            //    //DataTable grafConsHist = DSODataAccess.Execute(consultaGraficaHistorica());
            //    DataTable grafConsHist = DSODataAccess.Execute(consultaAnioActVsAnioAnterior());

            //    tblReportestr1tc1.Controls.Add(
            //    DTIChartsAndControls.tituloYBordesReporte(
            //        DTIChartsAndControls.GraficaLinea(grafConsHist, "Anio", "Nombre Mes", "Total",
            //        "", 400, 600, "Gráfica Consumo Histórico", "Mes", "Total", "c2", "grafConsHist"),
            //        "Grafica Historica", "Gráfica de Consumo Historico", 450));

            //    #endregion

            //    #region    Grafica Consumo por Tipo Telefonia

            //    DataTable grafConsTelefoniaFija = DSODataAccess.Execute(consultaTelefoniaFija());

            //    tblReportestr1tc2.Controls.Add(
            //    DTIChartsAndControls.tituloYBordesReporte(
            //        DTIChartsAndControls.GraficaAnillo(grafConsTelefoniaFija, "Telefonia", "Total",
            //        "~/UserInterface/DashboardLT/" + nombrePagina + "?nav=DashPrinNiv2",/*&Tel=#VALX",*/
            //        true, false, 400, 600, "Consumo por Tipo de Telefonia", "Telefonia", "Total", "grafConsTelefoniaFija"),
            //        "Grafica", "Por Telefonia", 450));

            //    #endregion

            //    #region Grafica por Carrier
            //    DataTable grafCosnCarrier = DSODataAccess.Execute(consultaConsumoPorCarrier());

            //    tblReportestr2tc1.Controls.Add(
            //    DTIChartsAndControls.tituloYBordesReporte(
            //        DTIChartsAndControls.GraficaAnillo(grafCosnCarrier, "Carrier", "Total", "",
            //        //"~/UserInterface/DashboardLT/" + nombrePagina + "?nav=CarN1&Carrier=#VALX", 
            //        true, false, 400, 600, "Consumo por Carrier", "Carrier", "Total", "grafConsCarrier"),
            //        "Grafica", "Por Carrier", 450));

            //    #endregion

            //    #endregion

            //    pnlMainContainer.Controls.Add(tblIndicadores);

            //    /*Se agregan los controles al contenedor principal*/
            //    pnlMainContainer.Controls.Add(tblReportes);
            //    pnlMainContainer.Controls.Add(new LiteralControl("<br />"));
            //}

            //#endregion // --Dashboard principal

            #region Dashboard Nivel 2

            //if (NavegacionNivel == "DashPrinNiv2") //primer nivel de navegacion
            if (NavegacionNivel == "") //primer nivel de navegacion
            {
                #region Creo una tabla que contendra los reportes

                Table tblReportes = new Table();
                tblReportes.Width = Unit.Percentage(100);

                TableRow tblReportestr1 = new TableRow();

                TableCell tblReportestr1tc1 = new TableCell();
                tblReportestr1tc1.Width = Unit.Percentage(50);
                tblReportestr1tc1.ColumnSpan = 3;

                TableCell tblReportestr1tc2 = new TableCell();
                tblReportestr1tc2.Width = Unit.Percentage(50);
                tblReportestr1tc2.ColumnSpan = 3;

                tblReportestr1.Controls.Add(tblReportestr1tc1);
                tblReportestr1.Controls.Add(tblReportestr1tc2);

                TableRow tblReportestr2 = new TableRow();

                TableCell tblReportestr2tc1 = new TableCell();
                tblReportestr2tc1.Width = Unit.Percentage(50);
                tblReportestr2tc1.ColumnSpan = 3;

                TableCell tblReportestr2tc2 = new TableCell();
                tblReportestr2tc2.Width = Unit.Percentage(50);
                tblReportestr2tc2.ColumnSpan = 3;

                tblReportestr2.Controls.Add(tblReportestr2tc1);
                tblReportestr2.Controls.Add(tblReportestr2tc2);

                TableRow tblReportestr3 = new TableRow();

                TableCell tblReportestr3tc1 = new TableCell();
                tblReportestr3tc1.Width = Unit.Percentage(33);
                tblReportestr3tc1.ColumnSpan = 2;

                TableCell tblReportestr3tc2 = new TableCell();
                tblReportestr3tc2.Width = Unit.Percentage(33);
                tblReportestr3tc2.ColumnSpan = 2;

                TableCell tblReportestr3tc3 = new TableCell();
                tblReportestr3tc3.Width = Unit.Percentage(33);
                tblReportestr3tc3.ColumnSpan = 2;

                tblReportestr3.Controls.Add(tblReportestr3tc1);
                tblReportestr3.Controls.Add(tblReportestr3tc2);
                tblReportestr3.Controls.Add(tblReportestr3tc3);

                tblReportes.Controls.Add(tblReportestr1);
                tblReportes.Controls.Add(tblReportestr2);
                tblReportes.Controls.Add(tblReportestr3);


                #endregion

                tr1tc1.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasFacturadas()).ToString(), "Lineas Facturadas"));

                tr1tc2.Controls.Add(DTIChartsAndControls.IndicadorHorizontal("150", "Enlaces"));

                tr1tc3.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasNuevas()).ToString(), "Lineas Nuevas"));

                tr1tc4.Controls.Add(DTIChartsAndControls.IndicadorHorizontal(DSODataAccess.ExecuteScalar(consultaLineasBaja()).ToString(), "Lineas Baja"));

                pnlMainContainer.Controls.Add(tblIndicadores);

                DataTable grafConsHist = DSODataAccess.Execute(consultaAnioActVsAnioAnterior());
                tblReportestr1tc1.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GraficaLinea(grafConsHist, "Anio", "Nombre Mes", "Total",
                    "", 400, 600, "Gráfica Consumo Histórico", "Mes", "Total", "c2", "grafConsHist"),
                    "Grafica Historica", "Gráfica de Consumo Historico", 450, ""));

                //DataTable grafConsPorTDest = DSODataAccess.Execute(consultaGraficaPorTDestSianaNiv2());
                //tblReportestr1tc2.Controls.Add(
                //DTIChartsAndControls.tituloYBordesReporte(
                //    DTIChartsAndControls.GraficaAnillo(DTIChartsAndControls.ordenaTabla(grafConsPorTDest, "[Total] desc"), "Tipo destino", "Total",
                //    "", true, true, 400, 600, "Consumo por Tipo de destino", "Tipo destino", "Total", "grafConsPorTDest"),
                //    "Grafica", "Por Tipo destino", 450));



                DataTable consTresMeses = DSODataAccess.Execute(consultaComparativo3MesesPorCategoriaClaveCargoSiana());
                tblReportestr2.Controls.Remove(tblReportestr2tc2);
                tblReportestr2tc1.ColumnSpan = 6;
                tblReportestr2tc1.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("consTresMeses",
                                                                DTIChartsAndControls.ordenaTabla(consTresMeses, "[" + consTresMeses.Columns[3].ColumnName + "] desc"),
                                                                true, "Total", new string[] { "", "{0:c}", "{0:c}", "{0:c}" }), "Reporte", "Historico 3 meses", 0));

                tblReportestr1tc2.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GraficaAnillo(DTIChartsAndControls.ordenaTabla(consTresMeses, "[" + consTresMeses.Columns[3].ColumnName + "]" + " desc"),
                    consTresMeses.Columns[0].ColumnName, consTresMeses.Columns[3].ColumnName,
                    "", true, true, 400, 600, "Consumo por categoria de clave cargo", "Categoria clave cargo", "Total", "grafConsPorCatClaveCargo"),
                    "Grafica", "Por categoria", 450));

                DataTable consTop10Lin = DSODataAccess.Execute(consultaTop10Lineas());
                tblReportestr3tc1.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("consTop10Lin",
                                                                DTIChartsAndControls.ordenaTabla(consTop10Lin, "[Total] desc"),
                                                                true, "Total", new string[] { "", "{0:c}" }), "Reporte", "Top 10 Lineas", 0));

                DataTable consTop10Dest = DSODataAccess.Execute(consultaTop10Destinos());
                tblReportestr3tc2.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("consTop10Dest",
                                                                DTIChartsAndControls.ordenaTabla(consTop10Dest, "[Total] desc"),
                                                                true, "Total", new string[] { "", "{0:c}" }), "Reporte", "Top 10 Destinos", 0));

                DataTable consTop10TelDest = DSODataAccess.Execute(consultaTop10NumMarc());
                tblReportestr3tc3.Controls.Add(
                DTIChartsAndControls.tituloYBordesReporte(
                    DTIChartsAndControls.GridView("consTop10TelDest",
                                                                DTIChartsAndControls.ordenaTabla(consTop10TelDest, "[Total] desc"),
                                                                true, "Total", new string[] { "", "{0:c}" }), "Reporte", "Top 10 Numeros marcados", 0));

                /*Se agregan los controles al contenedor principal*/
                pnlMainContainer.Controls.Add(tblReportes);
                pnlMainContainer.Controls.Add(new LiteralControl("<br />"));

            }
            #endregion // -- Dashboard Nivel 2
        }

        #region Consultas a SQL

        #region Consultas que se usan en Dashboard principal

        protected string consultaIndicadorLineasContratadasDashInicial()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("  declare @a int \n ");
            lsb.Append("  declare @b int \n ");
            lsb.Append("  declare @c int \n ");
            lsb.Append("  declare @d int \n ");
            lsb.Append("  select @a = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexsm \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  select @b = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexld \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  select @c = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexrentas \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  set @d = @a + @b + @c \n ");
            lsb.Append("  create table #temp(numeroLineas int) \n ");
            lsb.Append("  insert into #temp select @d \n ");
            lsb.Append("  select * from #temp \n ");
            return lsb.ToString();
        }
        protected string consultaIndicadorLineasFijas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("  declare @a int \n ");
            lsb.Append("  declare @b int \n ");
            lsb.Append("  declare @c int \n ");
            lsb.Append("  declare @d int \n ");
            lsb.Append("  select @a = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexsm \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  select @b = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexld \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  select @c = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexrentas \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  set @d = @a + @b + @c \n ");
            lsb.Append("  create table #temp(numeroLineas int) \n ");
            lsb.Append("  insert into #temp select @d \n ");
            lsb.Append("  select * from #temp \n ");
            return lsb.ToString();
        }
        protected string consultaIndicadorNuevasLineasDashInicial()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("  declare @a1 int \n ");
            lsb.Append("  declare @a2 int \n ");
            lsb.Append("  declare @tot int  \n ");
            lsb.Append("  select @a1 = COUNT( distinct(Telefono) )  \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexsm \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");

            lsb.Append("  select @a2 = COUNT( distinct(Telefono) )  \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexsm \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  set @tot = @a1 - @a2 \n");



            lsb.Append("  select @a1 = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexld \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");

            lsb.Append("  select @a2 = COUNT( distinct(Telefono) )  \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexld \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  set @tot = @tot + (@a1 - @a2) \n");


            lsb.Append("  select @a1 = COUNT( distinct(Telefono) ) \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexrentas \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");

            lsb.Append("  select @a2 = COUNT( distinct(Telefono) )  \n ");
            lsb.Append("  from " + DSODataContext.Schema + ".telmexrentas \n ");
            lsb.Append("  where telefono not like 'NULL' \n ");
            lsb.Append("  and telefono not like ' ' \n ");
            lsb.Append("  and telefono not like '002' \n ");
            lsb.Append("  and telefono not like '020' \n ");
            lsb.Append("  and telefono not like '999' \n ");
            lsb.Append("  and fechaPub >= '" + pdtInicio.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append("  and FechaPub < '" + pdtFin.Date.AddMonths(-1).ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            lsb.Append("  set @tot = @tot + (@a1 - @a2) \n");
            //lsb.Append("  if (@tot <= 0) \n");
            //lsb.Append("  begin \n");
            //lsb.Append("  set @tot = 0 \n");
            //lsb.Append("  end \n");
            lsb.Append("  create table #temp(numeroLineas int) \n ");
            lsb.Append("  insert into #temp select @tot \n ");
            lsb.Append("  select * from #temp \n ");

            return lsb.ToString();
        }
        protected string consultaGraficaHistorica()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" exec ConsumoHistoricoSIANA \n ");
            lsb.Append(" @Schema = '" + DSODataContext.Schema + "' \n ");
            lsb.Append(" ,@Fields = '[Nombre Mes],Total = SUM([Total]), [Anio], [Numero Mes]' \n ");
            lsb.Append(" ,@Group = '[Nombre mes], [Numero Mes], [Anio]' \n ");
            lsb.Append(" ,@Order = 'Convert(int,[Numero Mes])'  \n ");
            lsb.Append(" ,@Where = '[Nombre mes] is not null ' \n ");
            return lsb.ToString();

        }
        protected string consultaTelefoniaFija()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" select Total = SUM(Importe), Telefonia='Fija' from " + DSODataContext.Schema + ".consolidadoSIANA  \n ");
            lsb.Append(" where fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append(" and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            return lsb.ToString();
        }
        protected string consultaConsumoPorCarrier()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" select Total = SUM(Importe), Carrier='TELMEX' from " + DSODataContext.Schema + ".consolidadoSIANA  \n ");
            lsb.Append(" where fechaPub >= '" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \n ");
            lsb.Append(" and FechaPub < '" + pdtFin.Date.ToString("yyyy-MM-dd") + " 23:59:59' \n ");
            return lsb.ToString();
        }

        #endregion // -- Consultas que se usan en Dashboard principal

        protected string consultaFechasUltimoMesTasado()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" DECLARE @fechaMayorCDR SMALLDATETIME  \n");
            //lsb.Append(" SELECT @fechaMayorCDR = MAX(fechainicio) FROM " + DSODataContext.Schema + ".vDetalleCDR  \n");
            lsb.Append(" SELECT @fechaMayorCDR = MAX(fechainicio) FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n");
            lsb.Append(" /*Primer dia del ultimo mes tasado y ultimo dia del ultimo mes tasado*/  \n");
            lsb.Append(" SELECT [fechaIni] = DATEADD(MONTH,DATEDIFF(MONTH,0,@fechaMayorCDR),0),  \n");
            lsb.Append("        [fechaFin] = DATEADD(MILLISECOND,-3,DATEADD(MONTH,DATEDIFF(MONTH,0,@fechaMayorCDR)+1,0))  \n");

            return lsb.ToString();
        }

        //Indicador 1 Nivel 2
        protected string consultaLineasFacturadas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneNumeroLineasSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        //Indicador 2 Nivel 2
        protected string consultaEnlaces()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneNumeroLineasSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        //Indicador 3 Nivel 2
        protected string consultaLineasNuevas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneNumeroLineasNuevasSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        //Indicador 4 Nivel 2
        protected string consultaLineasBaja()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneNumeroLineasBajaSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        // Consulta Año Actual Vs Año Anterior
        protected string consultaAnioActVsAnioAnterior()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ConsumoHistoricoTelFija @Schema='" + DSODataContext.Schema + "' \r ");
            return lsb.ToString();
        }

        // Grafica TDest
        protected string consultaGraficaPorCatClaveCargo()
        {
            StringBuilder lsb = new StringBuilder();
            //lsb.Append("exec ObtieneConsumoPorTDestSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            lsb.Append("exec ObtieneConsumoPorCategoriaClaveCargoSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00' \r ");

            return lsb.ToString();
        }

        // Tabla 3 Meses
        protected string consultaComparativo3MesesPorCategoriaClaveCargoSiana()
        {
            StringBuilder lsb = new StringBuilder();
            //lsb.Append("exec ObtieneConsumo3MesesPorRazonSocial @Esquema='" + DSODataContext.Schema + "', \r ");
            //lsb.Append("@Fields = ' [Tipo de destino]=[TDestDesc],  \r ");
            //lsb.Append("        [Costo Abr 14] = Sum([Costo Abr 14]), \r ");
            //lsb.Append("        [Costo May 14] = Sum([Costo May 14]),  \r ");
            //lsb.Append("        [Costo Jun 14] = Sum([Costo Jun 14])', \r ");
            //lsb.Append(" @GroupBy = '[TDestDesc]' \r ");

            lsb.Append("exec ObtieneComparativo3MesesPorCategoriaClaveCargoSiana @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append(" @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r ");
            lsb.Append("@Fields='[Conceptos] = [Descripcion],[Costo 2 meses antes] = [Costo2M], [Costo 1 mes antes] = [Costo1M],[Mes actual] = [CostoMesActual]' \r ");

            return lsb.ToString();
        }

        // Tabla Top 10 Lineas
        protected string consultaTop10Lineas()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneConsumoTopNLineasSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r ");
            lsb.Append(" @NumRegs=10, @Fields = '[Linea] = [LadaTelefono], [Total] = [Costo]' \r ");
            return lsb.ToString();
        }

        // Tabla Top 10 Destinos
        protected string consultaTop10Destinos()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneConsumoTopNLocalidadesSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r ");
            lsb.Append(" @NumRegs=10, @Fields = '[Destino] = [NombrePoblacionDestino], [Total] = [Costo]' \r ");
            return lsb.ToString();
        }

        // Tabla Top 10 Numeros marcados
        protected string consultaTop10NumMarc()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneConsumoTopNTelDestCelSiana @Esquema='" + DSODataContext.Schema + "', @FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', \r ");
            lsb.Append(" @NumRegs=10, @Fields = '[Telefono destino] = [TelDest], [Total] = [Costo]' \r ");
            return lsb.ToString();
        }

        // Tabla consumo por UEN 
        protected string consultaFactHistoricaPorUEN()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("Exec ObtieneConsumo3MesesCatCveCargoPorRazonSocial @Esquema='" + DSODataContext.Schema + "'  \r ");
            return lsb.ToString();
        }

        // Tabla consumo por Grupo
        protected string consultaFactHistoricaPorGrupo()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("Exec ObtieneConsumoHistoricoCatCveCargoPorGrupo @Esquema='" + DSODataContext.Schema + "'  \r ");
            return lsb.ToString();
        }

        // Tabla desglose cargos en rentas y otros
        protected string consultaDesgloseCargosRentasYOtros()
        {
            StringBuilder lsb = new StringBuilder();
            //lsb.Append("exec ObtieneDesgloseCargosRentasYOtros @Esquema='" + DSODataContext.Schema + "',  \r ");
            //lsb.Append("@Fields='[Cuenta],[Rázon social] = [RazonSocial],[Tipo de destino] = [TDestDesc],[Monto] = [Costo]', \r ");
            //lsb.Append("@FechaIniRep='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFinRep='" + pdtInicio.Date.AddMonths(1).ToString("yyyy-MM-dd") + " 00:00:00' \r ");

            lsb.Append("exec ObtieneConsumoCargosRentasYOtros @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append("@Fields='[Cuenta],[Rázon social] = [RazonSocial], [Concepto] = [ClaveCarDesc], [Categoria] = [CategoriaClaveCargoDesc],[Monto] = [Costo]',  \r ");
            lsb.Append("@FechaIniRep='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFinRep='" + pdtInicio.Date.AddMonths(1).ToString("yyyy-MM-dd") + " 00:00:00' \r ");

            return lsb.ToString();
        }

        // Tabla Servicios Infinitum
        protected string consultaServInfinitum()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append("exec ObtieneServiciosInfinitum @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append("@Fields='[Rázon social] = [RazonSocial], [Costo servicios infinitum] = [Costo], [Cantidad servicios infinitum] = [NumReg]',  \r ");
            lsb.Append("@FechaIniRep='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFinRep='" + pdtInicio.Date.AddMonths(1).ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        // Tabla Detalle Servicios Infinitum
        protected string consultaDetalleServInfinitum()
        {
            StringBuilder lsb = new StringBuilder();
            //lsb.Append("exec ObtieneDetalleServiciosInfinitum @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append("exec ObtieneDetalleConceptosInfinitum @Esquema='" + DSODataContext.Schema + "',  \r ");
            //lsb.Append("@Fields='[Carrier], [Cuenta], [Teléfono] = [Telefono], [Localidad] = [SitioDesc],  \r ");
            //lsb.Append(" [Mes],[Costo], [Tipo de destino] = [TDestDesc], [Rázon social] = [RazonSocial]', \r ");
            lsb.Append("@Fields='[Carrier], [Cuenta], [Teléfono] = [Telefono], [Localidad] = [SitioDesc], [Mes],[Costo], [Concepto] = [ClaveCarDesc], [Rázon social] = [RazonSocial]',  \r ");
            lsb.Append("@FechaIniRep='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFinRep='" + pdtInicio.Date.AddMonths(1).ToString("yyyy-MM-dd") + " 00:00:00' \r ");
            return lsb.ToString();
        }

        // Tabla consumo por linea
        protected string consultaReportePorLinea()
        {
            StringBuilder lsb = new StringBuilder();
            //lsb.Append("Exec zzRJObtieneConsumoMatricialPorLinea  @Esquema='" + DSODataContext.Schema + "',  \r ");
            //lsb.Append("@FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFin='" + pdtInicio.Date.AddMonths(1).AddMilliseconds(-3).ToString("yyyy-MM-dd") + " 23:59:59' \r ");
            lsb.Append("Exec ObtieneConsumoMatricialPorLinea  @Esquema='" + DSODataContext.Schema + "',  \r ");
            lsb.Append("@FechaIni='" + pdtInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00', @FechaFin='" + pdtInicio.Date.AddMonths(1).AddMilliseconds(-3).ToString("yyyy-MM-dd") + " 23:59:59' \r ");
            //lsb.Append("select 'Prueba' as Col1, 1 as Col1, 2 as Col2, 3 as Col3 \r ");
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

        #endregion
    }
}
