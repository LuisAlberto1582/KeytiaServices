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
    public partial class DashboardMoviles
    {
        #region Reportes

        private void RepLineasInactivasCantidad(Control contenedor, string link, int pestaniaActiva)
        {
            string fechaDePagina = Session["FechaInicio"].ToString().Replace("-", "");
            DateTime fechaFinal = DateTime.ParseExact(fechaDePagina, "yyyyMMdd", CultureInfo.InvariantCulture);
            List<string> valoresTotales = new List<string>();
            List<string> nombreColumnas = new List<string>();
            string tituloReporte = "Total mensual de lineas inactivas";

            #region Arreglo Meses

            Dictionary<int, string> mesesString = new Dictionary<int, string>();
            mesesString.Add(1, "Enero");
            mesesString.Add(2, "Febrero");
            mesesString.Add(3, "Marzo");
            mesesString.Add(4, "Abril");
            mesesString.Add(5, "Mayo");
            mesesString.Add(6, "Junio");
            mesesString.Add(7, "Julio");
            mesesString.Add(8, "Agosto");
            mesesString.Add(9, "Septiembre");
            mesesString.Add(10, "Octubre");
            mesesString.Add(11, "Noviembre");
            mesesString.Add(12, "Diciembre");

            #endregion

            #region Consulta
            DataTable dtMesesInactivosOriginal = DSODataAccess.Execute(ConsultaTablaMesesInactivosCantidad());
            DataTable dtMesesInactivosCopia = new DataTable();
            #endregion

            for (int i = 5; i >= 0; i--)
            {
                nombreColumnas.Add(mesesString[fechaFinal.AddMonths(-i).Month] + " " + fechaFinal.AddMonths(-i).Year);
                dtMesesInactivosCopia.Columns.Add(mesesString[fechaFinal.AddMonths(-i).Month] + " " + fechaFinal.AddMonths(-i).Year);
            }

            foreach (DataRow row in dtMesesInactivosOriginal.Rows)
            {
                for (int i = 1; i <= 6; i++)
                {
                    valoresTotales.Add(row[i].ToString());
                }
            }

            dtMesesInactivosCopia.Rows.Add(valoresTotales.ToArray());

            if (dtMesesInactivosCopia != null && dtMesesInactivosCopia.Rows.Count > 0)
            {
                DataTable dt;

                dt = dtMesesInactivosCopia.Clone();

                foreach (DataRow row in dtMesesInactivosCopia.Rows)
                {
                    dt.ImportRow(row);
                }


                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    string[] campos = nombreColumnas.ToArray();
                    int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5 };
                    int[] camposNavegacion = new int[] { };
                    string[] formatoColumnas = new string[] { "", "", "", "", "", "" };

                    dt = dvldt.ToTable(false, campos);

                    GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dt, false, "",
                          formatoColumnas,
                          link, new string[] { "" },
                          1, camposNavegacion, camposBoundField, new int[] { }, 2);

                    dt.AcceptChanges();

                    contenedor.Controls.Clear();
                    contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
                }

            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepLineasInactivasSinImportes(Control contenedor, string link, int pestaniaActiva)
        {
            string fechaDePagina = Session["FechaInicio"].ToString().Replace("-", "");
            DateTime fechaFinal = DateTime.ParseExact(fechaDePagina, "yyyyMMdd", CultureInfo.InvariantCulture);

            #region Arreglo Meses

            Dictionary<int, string> mesesString = new Dictionary<int, string>();
            mesesString.Add(1, "Enero");
            mesesString.Add(2, "Febrero");
            mesesString.Add(3, "Marzo");
            mesesString.Add(4, "Abril");
            mesesString.Add(5, "Mayo");
            mesesString.Add(6, "Junio");
            mesesString.Add(7, "Julio");
            mesesString.Add(8, "Agosto");
            mesesString.Add(9, "Septiembre");
            mesesString.Add(10, "Octubre");
            mesesString.Add(11, "Noviembre");
            mesesString.Add(12, "Diciembre");

            #endregion

            #region Consulta
            DataTable dtMesesInactivosOriginal = DSODataAccess.Execute(ConsultaTablaMesesInactivosSinImporte());
            #endregion

            string tituloReporte = "Consumo Lineas Inactivas";

            if (dtMesesInactivosOriginal != null && dtMesesInactivosOriginal.Rows.Count > 0)
            {
                //dtMesesInactivosOriginal = DTIChartsAndControls.ordenaTabla(dtMesesInactivosOriginal, "Inactivos Desc");
                dtMesesInactivosOriginal.AcceptChanges();

                DataTable dt;
                List<string> columnasTabla = new List<string>();
                columnasTabla.Add("Linea");
                //columnasTabla.Add("Meses Inactivos");

                for (int i = 5; i >= 0; i--)
                {
                    columnasTabla.Add(mesesString[fechaFinal.AddMonths(-i).Month] + " " + fechaFinal.AddMonths(-i).Year);
                }

                string[] nuevoNombreColumnas = columnasTabla.ToArray();

                dt = dtMesesInactivosOriginal.Clone();
                dt.Columns[0].DataType = typeof(String);
                //dt.Columns[1].DataType = typeof(String);

                foreach (DataRow row in dtMesesInactivosOriginal.Rows)
                {
                    dt.ImportRow(row);
                }

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ColumnName = nuevoNombreColumnas[i];
                }

                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    string[] campos = nuevoNombreColumnas;
                    int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5, 6 };
                    int[] camposNavegacion = new int[] { };
                    string[] formatoColumnas = new string[] { "", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}" };

                    dt = dvldt.ToTable(false, campos);

                    GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dt, false, "",
                          formatoColumnas,
                          link, new string[] { "" },
                          1, camposNavegacion, camposBoundField, new int[] { }, 2);

                    dt.AcceptChanges();

                    #region Marcar celdas inactivas

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GridViewRow celda = grid.Rows[i];

                        if (dtMesesInactivosOriginal.Rows[i][1].ToString() == "Sin actividad")
                        {
                            celda.Cells[1].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][2].ToString() == "Sin actividad")
                        {
                            celda.Cells[2].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][3].ToString() == "Sin actividad")
                        {
                            celda.Cells[3].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][4].ToString() == "Sin actividad")
                        {
                            celda.Cells[4].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][5].ToString() == "Sin actividad")
                        {
                            celda.Cells[5].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][6].ToString() == "Sin actividad")
                        {
                            celda.Cells[6].CssClass = "textColorTable";
                        }
                    }

                    #endregion

                    contenedor.Controls.Clear();
                    contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
                }

            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepLineasPorPlanBase(Control contenedorGrafica, string tituloGrafica, Control contenedor, string link, int pestaniaActiva)
        {
            string fechaDePagina = Session["FechaInicio"].ToString().Replace("-", "");
            DateTime fechaFinal = DateTime.ParseExact(fechaDePagina, "yyyyMMdd", CultureInfo.InvariantCulture);

            #region Consulta
            DataTable dt = DSODataAccess.Execute(ConsultaLineasPorPlanBase());

            if (dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false,
                    new string[] { "Descripcion", "iCodCatalogo", "RentaTelefonia", "TotalMes3", "TotalMes2", "TotalMes1" });
                dt.Columns["Descripcion"].ColumnName = "Plan";
                dt.Columns["iCodCatalogo"].ColumnName = "Codigo Plan";
                dt.Columns["RentaTelefonia"].ColumnName = "Renta Base";
                dt.Columns["TotalMes3"].ColumnName = String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-2)).ToUpper();
                dt.Columns["TotalMes2"].ColumnName = String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-1)).ToUpper();
                dt.Columns["TotalMes1"].ColumnName = String.Format("{0:MMMM yyyy}", fechaFinal).ToUpper();
                dt.AcceptChanges();
            }
            #endregion

            #region Grid
            List<string> columnasTabla = new List<string>();
            columnasTabla.Add("Plan");
            columnasTabla.Add("Codigo Plan");
            columnasTabla.Add("Renta Base");
            columnasTabla.Add(String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-2)).ToUpper());
            columnasTabla.Add(String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-1)).ToUpper());
            columnasTabla.Add(String.Format("{0:MMMM yyyy}", fechaFinal).ToUpper());

            string tituloReporte = "Líneas por Plan Base";

            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                string[] campos = columnasTabla.ToArray();
                int[] camposBoundField = new int[] { 0, 2, 3, 4, 5 };
                int[] camposNavegacion = new int[] {  };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "" };

                dt = dvldt.ToTable(false, campos);

                GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dt, true, "Totales",
                      formatoColumnas,
                      "ConsumoMovilRep.aspx?Opc=OpcRepBusqConsumoMovil&Plan={0}", new string[] { "Codigo Plan" },
                      0, camposNavegacion, camposBoundField, new int[] {  }, 2);

                // Se agrega columna para links con parametro de celda
                //for (int i = 0; i < grid.Rows.Count; i++)
                //{
                //    GridViewRow fila = grid.Rows[i];
                //    fila.Cells[0].Text = "<a href='ConsumoMovilRep.aspx?Opc=OpcRepBusqConsumoMovil&Plan=" + fila.Cells[1].Text + "'>" + fila.Cells[0].Text + "</a>";
                //}

                dt.AcceptChanges();

                GridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[1].Text = "Totales";

                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
                #endregion

                #region Grafica

                if (dt.Rows.Count > 0)
                {
                    DataView dvldtGraf = new DataView(dt);
                    dt = dvldt.ToTable(false,
                        new string[] { columnasTabla[0], columnasTabla[3], columnasTabla[4], columnasTabla[5] });
                    if (dt.Rows[dt.Rows.Count - 1][columnasTabla[0]].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }
                    dt.Columns[columnasTabla[0]].ColumnName = "label";
                    dt.Columns[columnasTabla[3]].ColumnName = String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-2)).ToUpper();
                    dt.AcceptChanges();
                }

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                    "RepTabResumenPorConceptoTelcelMovGraf_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Matricial));

                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                                                    FCAndControls.ConvertDataTabletoDataTableArray(DTIChartsAndControls.selectTopNTabla(dt, String.Format("{0:MMMM yyyy}", fechaFinal.AddMonths(-2)).ToUpper() + " desc", 10)));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenPorConceptoTelcelMovGraf",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt), "RepTabResumenPorConceptoTelcelMovGraf_G",
                    "", "", "Conceptos", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);

                #endregion

            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepLineasInactivas(Control contenedor, string link, int pestaniaActiva)
        {
            string fechaDePagina = Session["FechaInicio"].ToString().Replace("-", "");
            DateTime fechaFinal = DateTime.ParseExact(fechaDePagina, "yyyyMMdd", CultureInfo.InvariantCulture);

            #region Consulta
            DataTable dtMesesInactivosOriginal = DSODataAccess.Execute(ConsultaTablaMesesInactivos());
            string tituloReporte = "Consumo Lineas Inactivas";
            if (dtMesesInactivosOriginal != null && dtMesesInactivosOriginal.Rows.Count > 0)
            {
                dtMesesInactivosOriginal = DTIChartsAndControls.ordenaTabla(dtMesesInactivosOriginal, "Inactivos Desc");
                dtMesesInactivosOriginal.AcceptChanges();

                #endregion

                DataTable dt;
                List<string> columnasTabla = new List<string>();
                columnasTabla.Add("Linea");
                columnasTabla.Add("Meses Inactivos");

                for (int i = 5; i >= 0; i--)
                {
                    columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-i)));
                }

                string[] nombreColumnas = new string[]
                {
                "TuvoActividadMesSelecMenos5",
                "TuvoActividadMesSelecMenos4",
                "TuvoActividadMesSelecMenos3",
                "TuvoActividadMesSelecMenos2",
                "TuvoActividadMesSelecMenos1",
                "TuvoActividadMesSelec"
                };

                string[] nuevoNombreColumnas = columnasTabla.ToArray();

                dt = dtMesesInactivosOriginal.Clone();
                dt.Columns[0].DataType = typeof(String);
                dt.Columns[1].DataType = typeof(String);

                foreach (DataRow row in dtMesesInactivosOriginal.Rows)
                {
                    dt.ImportRow(row);
                }


                for (int i = 0; i < nombreColumnas.Length; i++)
                {
                    dt.Columns.Remove(nombreColumnas[i]);
                }

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ColumnName = nuevoNombreColumnas[i];
                }

                

                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    string[] campos = nuevoNombreColumnas;
                    int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
                    int[] camposNavegacion = new int[] { };
                    string[] formatoColumnas = new string[] { "", "", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}", "{0:C2}" };

                    dt = dvldt.ToTable(false, campos);

                    GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dt, true, "Totales",
                          formatoColumnas,
                          link, new string[] { "" },
                          1, camposNavegacion, camposBoundField, new int[] { }, 2);

                    dt.AcceptChanges();

                    GridViewRow row = grid.Rows[grid.Rows.Count - 1];
                    row.Cells[1].Text = "Totales";
                    for (int tot = 0; tot < row.Cells.Count; tot++)
                    {
                        row.Cells[tot].CssClass = "lastRowTable";
                    }

                    #region Marcar celdas inactivas

                    for (int i = 0; i < dt.Rows.Count - 1; i++)
                    {
                        GridViewRow celda = grid.Rows[i];

                        if (dtMesesInactivosOriginal.Rows[i][3].ToString() == "0")
                        {
                            celda.Cells[2].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][5].ToString() == "0")
                        {
                            celda.Cells[3].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][7].ToString() == "0")
                        {
                            celda.Cells[4].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][9].ToString() == "0")
                        {
                            celda.Cells[5].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][11].ToString() == "0")
                        {
                            celda.Cells[6].CssClass = "textColorTable";
                        }
                        if (dtMesesInactivosOriginal.Rows[i][13].ToString() == "0")
                        {
                            celda.Cells[7].CssClass = "textColorTable";
                        }
                    }

                    #endregion

                    contenedor.Controls.Clear();
                    contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
                }

            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepTabHistMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoMov());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Total", "CantidadLineas" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["CantidadLineas"].ColumnName = "Cantidad líneas";
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistGrid", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "" }),
                               "RepTabHistMov1Pnl", tituloGrid, Request.Path + "?Nav=HistoricoN1", pestaniaActiva, FCGpoGraf.ColsCombinada)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "Cantidad líneas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
                DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(ldt);
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHistMov1Pnl",
                    FCAndControls.GraficaCombinada(DataSourceArrayJSon,
                    new string[] { "Mes", "Total", "Cantidad líneas" }, "RepTabHistMov1Pnl", "", "", "Mes", "Importe", "Cant. líneas",
                    pestaniaActiva, FCTipoEjeSecundario.line, FCGpoGraf.ColsCombinada), false);
            }


            #endregion Grafica
        }

        private void RepTabHistMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoMov());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Total", "CantidadLineas" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["CantidadLineas"].ColumnName = "Cantidad líneas";
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepHistGrid_T", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "" }),
                               "RepTabHistMov2Pnls_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "Cantidad líneas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(ldt);
            string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabHistMov2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.ColsCombinada));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHistMov2Pnls_G",
                FCAndControls.GraficaCombinada(DataSourceArrayJSon,
                new string[] { "Mes", "Total", "Cantidad líneas" },
                 "RepTabHistMov2Pnls_G", "", "", "Mes", "Importe", "Cant. líneas", pestaniaActiva, FCTipoEjeSecundario.line, FCGpoGraf.ColsCombinada), false);

            #endregion Grafica
        }

        private void RepTabPorCenCosMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(
                ConsultaConsumoPorCenCos("[link] = ''" + Request.Path + "?Nav=CarrierCCN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])"));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Nombre Centro de Costos", "Total", "Renta", "Diferencia", "link" });
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepConsXCenCosGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "?Nav=CarrierCCN2&CenCos={0}",
                                    new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorCenCosMov1Pnl", tituloGrid, Request.Path + "?Nav=CenCosN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Área", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCosMov1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCenCosMov1Pnl", "", "", "Área", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica
        }

        private void RepTabPorCenCosMov2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(
                ConsultaConsumoPorCenCos(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Nombre Centro de Costos", "Total", "Renta", "Diferencia", "link" });
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabPorCenCosMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid,
                                    new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorCenCosMov2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Área", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorCenCosMovGraf_G", tituloGrid, pestaniaActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCosMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabPorCenCosMovGraf_G", "", "", "Área", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorEmpleMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleado("[link] = ''" + Request.Path + "?Nav=EmpleN2&Emple='' + convert(varchar,[Codigo Empleado])+''&Tel='' + convert(varchar,[Linea]) + ''&Carrier=''+ convert(varchar,[Codigo Carrier])"));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia", "iCodCatCarrier", "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(DTIChartsAndControls.GridView("RepConsPorEmpleGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "", "" }, Request.Path + "?Nav=EmpleN2&Emple={0}&Tel={1}&Carrier={2}",
                                new string[] { "Codigo Empleado", "Linea", "iCodCatCarrier" }, 1, new int[] { 0, 6 },  //NZ 20180308
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabPorEmpleMov1Pnl", tituloGrid, Request.Path + "?Nav=EmpleN1", pestaniaActiva, FCGpoGraf.Tabular)
                );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmpleMov1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleMov1Pnl", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabPorEmpleMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleado("[link] = ''" + Request.Path + "?Nav=EmpleN2&Emple='' + convert(varchar,[Codigo Empleado])+''&Tel='' + convert(varchar,[Linea])+''&Carrier=''+ convert(varchar,[Codigo Carrier])"));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia", "iCodCatCarrier", "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabPorEmpleMovGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "", "" }, Request.Path + "?Nav=EmpleN2&Emple={0}&Tel={1}&Carrier={2}", //linkGrid,
                                new string[] { "Codigo Empleado", "Linea", "iCodCatCarrier" }, 1, new int[] { 0, 6 },  //NZ 20180308
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabPorEmpleMov2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorEmpleMov2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmpleMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleMov2Pnls_G", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabResumenPorConceptoTelcelMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaRepResumenPorConceptosTelcel(""));

            #region Grid
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fake", "Servicio", "Descripcion", "Mes Actual", "Mes Anterior", "Mes Anterior 2" });
                ldt.Columns["Servicio"].ColumnName = "Conceptos";
                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Mes Actual"].ColumnName = "Mes seleccionado";
                ldt.Columns["Mes Anterior"].ColumnName = "Mes anterior";
                ldt.Columns["Mes Anterior 2"].ColumnName = "2 meses atrás";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                       DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                       DTIChartsAndControls.GridView(
                                       "RepResumenConceptosTelcelGrid" + DateTime.Now.Ticks, ldt, true, "Totales",
                                       new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}" }, "",
                                       new string[] { "fake" }, 1, new int[] { 0 },
                                       new int[] { 1, 2, 3, 4, 5 }, new int[] { 0 }), "RepTabResumenPorConceptoTelcelMov1Pnl", tituloGrid, Request.Path + "?Nav=ResumenPorConcepTelcelN1", pestaniaActiva, FCGpoGraf.MatricialCoBaTa)
                       );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Conceptos", "Mes seleccionado", "Mes anterior", "2 meses atrás" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Conceptos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                                                FCAndControls.ConvertDataTabletoDataTableArray(DTIChartsAndControls.selectTopNTabla(ldt, "Mes seleccionado desc", 10)));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepResumenConceptosTelcelGrid",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabResumenPorConceptoTelcelMov1Pnl", "",
                 "", "Conceptos", "Importe", pestaniaActiva, FCGpoGraf.MatricialCoBaTa), false);

            #endregion Grafica
        }

        private void RepTabResumenPorConceptoTelcelMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepResumenPorConceptosTelcel(""));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fake", "Servicio", "Descripcion", "Mes Actual", "Mes Anterior", "Mes Anterior 2" });
                ldt.Columns["Servicio"].ColumnName = "Conceptos";
                ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                ldt.Columns["Mes Actual"].ColumnName = "Mes seleccionado";
                ldt.Columns["Mes Anterior"].ColumnName = "Mes anterior";
                ldt.Columns["Mes Anterior 2"].ColumnName = "2 meses atrás";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                       DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                       DTIChartsAndControls.GridView(
                                       "RepTabResumenPorConceptoTelcelMov_T" + DateTime.Now.Ticks, ldt, true, "Totales",
                                       new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}" }, "",
                                       new string[] { "fake" }, 1, new int[] { 0 },
                                       new int[] { 1, 2, 3, 4, 5 }, new int[] { 0 }), "RepTabResumenPorConceptoTelcelMov2Pnls", tituloGrid)
                       );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Conceptos", "Mes seleccionado", "Mes anterior", "2 meses atrás" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Conceptos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepTabResumenPorConceptoTelcelMovGraf_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Matricial));

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                                                FCAndControls.ConvertDataTabletoDataTableArray(DTIChartsAndControls.selectTopNTabla(ldt, "Mes seleccionado desc", 10)));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenPorConceptoTelcelMovGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabResumenPorConceptoTelcelMovGraf_G",
                "", "", "Conceptos", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }

        private void RepTabResumenPorPlanesMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaRepResumenPorPlanes(""));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fake", "Plan", "Total", "Cantidad" });
                ldt.Columns["Plan"].ColumnName = "Plan";
                ldt.Columns["Total"].ColumnName = "Renta";
                ldt.Columns["Cantidad"].ColumnName = "Cantidad";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(
                                    "RepResumenPorPlanesGrid" + DateTime.Now.Ticks, ldt, true, "Totales",
                                    new string[] { "", "", "{0:c}", "" }, "",
                                    new string[] { "fake" }, 1, new int[] { 0 },
                                    new int[] { 1, 2, 3 }, new int[] { 0 }), "RepTabResumenPorPlanesMov1Pnl", tituloGrid, Request.Path + "?Nav=ResumenPorPlanesN1", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Plan", "Renta" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Plan"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Plan"].ColumnName = "label";
                ldt.Columns["Renta"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepResumenPorPlanesGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabResumenPorPlanesMov1Pnl", "", "", "Plan", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabResumenPorPlanesMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepResumenPorPlanes(""));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fake", "Plan", "Total", "Cantidad" });
                ldt.Columns["Plan"].ColumnName = "Plan";
                ldt.Columns["Total"].ColumnName = "Renta";
                ldt.Columns["Cantidad"].ColumnName = "Cantidad";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepTabResumenPorPlanesMov_T" + DateTime.Now.Ticks, ldt, true, "Totales",
                                    new string[] { "", "", "{0:c}", "" }, "",
                                    new string[] { "fake" }, 1, new int[] { 0 },
                                    new int[] { 1, 2, 3 }, new int[] { 0 }), "RepTabResumenPorPlanesMov2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Plan", "Renta" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Plan"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Plan"].ColumnName = "label";
                ldt.Columns["Renta"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabResumenPorPlanesMov2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenPorPlanesMovGrag",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabResumenPorPlanesMov2Pnls_G", "", "", "Plan", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabTopLinMasCarasMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
            lblCantMaxReg.Text = "Cantidad máxima de registros:";
            if (!Page.IsPostBack)
            {
                txtCantMaxReg.Text = "10";
            }

            DataTable ldt = DSODataAccess.Execute(
                ConsultaRepTopLineasMasCaras("[link] = ''" + Request.Path + "?Nav=TopLineasMasCarasN2&Linea=" + "'' + convert(varchar,[Extension]) +''&Carrier='' + convert(varchar,[idCarrier]) "));

            #region Grid


            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "ExtensionCod", "Extension", "NombreCompleto", "Total", "iCodCatCarrier", "link" });
                ldt.Columns["Extension"].ColumnName = "Línea";
                ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(
                                    "RepTabTopLinMasCarasGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "", "{0:c}", "" }, Request.Path + "?Nav=TopLineasMasCarasN2&Linea={0}&Carrier={1}",
                                    new string[] { "ExtensionCod", "iCodCatCarrier" }, 1, new int[] { 0, 4, 5 },
                                    new int[] { 2, 3 }, new int[] { 1 }), "RepTabTopLinMasCarasMov1Pnl", tituloGrid, Request.Path + "?Nav=TopLineasMasCarasN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Colaborador", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTopLinMasCarasGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTopLinMasCarasMov1Pnl", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica

        }

        private void RepTabTopLinMasCarasMov2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
            lblCantMaxReg.Text = "Cantidad máxima de registros:";
            if (!Page.IsPostBack)
            {
                txtCantMaxReg.Text = "10";
            }

            DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "ExtensionCod", "Extension", "NombreCompleto", "Total", "iCodCatCarrier", "link" });
                ldt.Columns["Extension"].ColumnName = "Línea";
                ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepTabTopLinMasCarasMov_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "", "{0:c}", "" }, linkGrid,
                                    new string[] { "ExtensionCod", "iCodCatCarrier" }, 1, new int[] { 0, 4, 5 },
                                    new int[] { 2, 3 }, new int[] { 1 }), "RepTabTopLinMasCarasMov2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Colaborador", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabTopLinMasCarasMov2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTopLinMasCarasMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTopLinMasCarasMov2Pnls_G", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorCenCosMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMat());

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepMatPorCenCosMovGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", formatStrings.ToArray()), "RepMatPorCenCosMov1Pnl", tituloGrid, Request.Path + "?Nav=HistXCenCosN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Área", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorCenCosMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorCenCosMov1Pnl", "", "", "Área", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorCenCosMov2Pnls(Control contenedorGrafica, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMat());

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatPorCenCosMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", formatStrings.ToArray()), "RepMatPorCenCosMov2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Área", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepMatPorCenCosMovGraf_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorCenCosMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorCenCosMovGraf_G", "", "", "Área", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorEmpleMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMat());
            ldt.Columns["Empleado"].ColumnName = "Colaborador";

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepMatPorEmpleMovGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                               true, "Totales", formatStrings.ToArray()), "RepMatPorEmpleMov1Pnl", tituloGrid, Request.Path + "?Nav=HistXEmpleN1", pestaniaActiva, FCGpoGraf.Tabular)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                //ldt.Columns["Empleado"].ColumnName = "Colaborador";

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Colaborador", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorEmpleMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorEmpleMov1Pnl", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorEmpleMov2Pnls(Control contenedorGrafica, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMat());

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepMatPorEmpleMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                               true, "Totales", formatStrings.ToArray()), "RepMatPorEmpleMov2Pnls_T", tituloGrafica)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Empleado", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepMatPorEmpleMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorEmpleMovGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorEmpleMov2Pnls_G", "", "", "Colaborador", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabLlamACeluDeLaRedMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            DataTable ldt = DSODataAccess.Execute(
                ConsultaLlamACelularesDeLaRed("[link] = ''" + Request.Path + "?Nav=LlamACelRedN2&Exten='' + convert(varchar,[Extension])"));

            #region Grid


            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Exten", "Extension", "Nombre Completo", "Total", "Duracion", "Numero", "link" });
                ldt.Columns["Extension"].ColumnName = "Línea";
                ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Duracion"].ColumnName = "Minutos";
                ldt.Columns["Numero"].ColumnName = "Llamadas";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepLlamACeluRedGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "", "", "" }, Request.Path + "?Nav=LlamACelRedN2&Exten={0}",
                                new string[] { "Codigo Exten" }, 1, new int[] { 0 },
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabLlamACeluDeLaRedMov1Pnl", tituloGrid, Request.Path + "?Nav=LlamACelRedN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Línea", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Línea"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepLlamACeluRedGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamACeluDeLaRedMov1Pnl", "", "", "Línea", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabLlamACeluDeLaRedMov2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaLlamACelularesDeLaRed(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Exten", "Extension", "Nombre Completo", "Total", "Duracion", "Numero", "link" });
                ldt.Columns["Extension"].ColumnName = "Línea";
                ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Duracion"].ColumnName = "Minutos";
                ldt.Columns["Numero"].ColumnName = "Llamadas";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamACeluRedGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "", "", "" }, linkGrid,
                                new string[] { "Codigo Exten" }, 1, new int[] { 0 },
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabLlamACeluDeLaRedMov2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Línea", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Línea"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamACeluDeLaRedMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepLlamACeluRedGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamACeluDeLaRedMov2Pnls_G", "", "", "Línea", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabResumenPorEquiposMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(
                 ConsultaResumenEquiposTelcel(""));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo", "Cantidad", "Total" });
                ldt.Columns["Tipo"].ColumnName = "Tipo";
                ldt.Columns["Cantidad"].ColumnName = "Cantidad";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepTabResumenPorEquiposGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                               true, "Totales", new string[] { "", "{0:0,0}", "{0:c}" }), "RepTabResumenPorEquiposMov1Pnl", tituloGrid, Request.Path + "?Nav=ResumPorEquipoN1", pestaniaActiva, FCGpoGraf.Tabular)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenPorEquiposGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabResumenPorEquiposMov1Pnl", "", "", "Tipo", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabResumenPorEquiposMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaResumenEquiposTelcel(""));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo", "Cantidad", "Total" });
                ldt.Columns["Tipo"].ColumnName = "Tipo";
                ldt.Columns["Cantidad"].ColumnName = "Cantidad";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepTabResumenPorEquiposGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                               true, "Totales", new string[] { "", "{0:0,0}", "{0:c}" }), "RepTabResumenPorEquiposMov2Pnls_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabResumenPorEquiposMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenPorEquiposGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabResumenPorEquiposMov2Pnls_G", "", "", "Tipo", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabHistAnioAnteriorVsActualMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();

            if (DSODataContext.Schema.ToString().ToLower() == "sperto")
            {
                anioActual = "2017";
                anioAnterior = "2016";
                dosAnioAtras = "2015";
            }

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMoviles());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActGrid", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "RepTabHistAnioAnteriorVsActualMov1Pnl", tituloGrid, Request.Path + "?Nav=HistoricoAnioActVsAntN1", pestaniaActiva, FCGpoGraf.MatricialLiCoBaTa)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHistAnioAnteriorVsActualMov1Pnl", ""
                , "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.MatricialLiCoBaTa), false);

            #endregion Grafica
        }

        private void RepTabHistAnioAnteriorVsActualMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {

            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160111 se agrega un 3er año hacia atras.*/
            if (DSODataContext.Schema.ToString().ToLower() == "sperto")
            {
                anioActual = "2017";
                anioAnterior = "2016";
                dosAnioAtras = "2015";
            }
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMoviles());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActGrid_T", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "RepTabHistAnioAnteriorVsActualMov2Pnls_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepTabHistAnioAnteriorVsActualMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Matricial));

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHistAnioAnteriorVsActualMov2Pnls_G",
                tituloGrafica, "", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

            #endregion Grafica

        }

        private void RepTabHistAnioAnteriorVsActualConFiltroMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            DataTable ldt = new DataTable();

            if (param["CenCos"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [CenCos] = " + param["CenCos"] + "', \n "));
            }
            else if (param["Emple"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [Emple] = " + param["Emple"] + "', \n "));
            }

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActConFiltroGrid", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "RepTabHistAnioAnteriorVsActualConFiltroMov1Pnl", tituloGrid, "", pestaniaActiva, FCGpoGraf.Matricial));

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }


            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActConFiltroGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHistAnioAnteriorVsActualConFiltroMov1Pnl",
                "", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }

        private void RepTabHistAnioAnteriorVsActualConFiltroMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            //Se obtiene el DataSource del reporte
            DataTable ldt = new DataTable();

            //Se valida si se está recibiendo como parámetro el CenCos o el Emple, este parámetro se envía a la consulta
            if (param["CenCos"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [CenCos] = " + param["CenCos"] + "', \n "));
            }
            else if (param["Emple"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [Emple] = " + param["Emple"] + "', \n "));
            }

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActConFiltroGrid_T", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "RepTabHistAnioAnteriorVsActualConFiltroMov2Pnls_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepTabHistAnioAnteriorVsActualConFiltroMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Matricial));


            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActConFiltroGraf_G",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHistAnioAnteriorVsActualConFiltroMov2Pnls_G",
                tituloGrafica, "", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }

        private void RepMatHistPorCenCosAnioAntMov1Pnl()
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAnt());

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            Rep9.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatHistPorCenCosAnioAntMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "RepMatHistPorCenCosAnioAntMov1Pnl_T", formatStrings.ToArray()), "Reporte", "Consumo Historico Por Área del Año Anterior")  // Request.Path + "?Nav=HistXCenCosAnioActualN1")
                );

            #endregion Grid
        }

        private void RepMatHistPorCenCosAnioActMov1Pnl()
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAct());

            #region Grid

            #region Elimina columnas no necesarias en el gridview
            RemoveColHerencia(ref ldt);
            if (ldt.Columns.Contains("Codigo Centro de Costos"))
                ldt.Columns.Remove("Codigo Centro de Costos");
            #endregion // Elimina columnas no necesarias en el gridview

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatHistPorCenCosAnioActMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", formatStrings.ToArray()), "RepMatHistPorCenCosAnioActMov1Pnl_T", "Consumo Historico Por Área del Año Actual")  // Request.Path + "?Nav=HistXCenCosAnioActualN1")
                );

            #endregion Grid
        }

        private void RepMatHistPorEmpleAnioAntMov1Pnl(Control contenedor)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMatAnioAnt());

            #region Grid

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatHistPorEmpleAnioAntMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", formatStrings.ToArray()), "RepMatHistPorEmpleAnioAntMov1Pnl_T", "Consumo Historico Por Colaborador del Año Anterior")  // Request.Path + "?Nav=HistXCenCosAnioActualN1")
                );

            #endregion Grid
        }

        private void RepMatHistPorEmpleAnioActMov1Pnl(Control contenedor)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMatAnioAct());

            #region Grid

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatHistPorEmpleAnioActMovGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", formatStrings.ToArray()), "RepMatHistPorEmpleAnioActMov1Pnl_T", "Consumo Historico Por Colaborador del Año Actual")  // Request.Path + "?Nav=HistXCenCosAnioActualN1")
                );

            #endregion Grid
        }

        private void RepTabPorEmpleAcumMov1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleAcumulado()); //("[link] = ''" + Request.Path + "?Nav=EmpleN2&Emple='' + convert(varchar,[Codigo Empleado])+''&Tel='' + convert(varchar,[Linea])"));

            #region Grid


            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" }); //, "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                        DTIChartsAndControls.GridView("RepConsPorEmpleAcumGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, "",
                                new string[] { "Codigo Empleado", "Linea" }, 1, new int[] { 0 },
                                new int[] { 1, 2, 3, 4, 5 }, new int[] { }),
                                 "RepTabPorEmpleAcumMov1Pnl", tituloGrid, Request.Path + "?Nav=EmpleAcumN1", pestaniaActiva, FCGpoGraf.Tabular)
                                );



            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total" });//, "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsPorEmpleAcumGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabPorEmpleAcumMov1Pnl", "", "", "Colaborador", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabPorEmpleAcumMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleAcumulado()); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });//, "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsPorEmpleAcumGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, "", //linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0 },
                                new int[] { 1, 2, 3, 4, 5 }, new int[] { }),  // new int[] { 1 }), 
                                "RepTabPorEmpleAcumMov2Pnls_T", tituloGrafica)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total" });  //, "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorEmpleAcumMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsPorEmpleAcumGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleAcumMov2Pnls_G", "", "", "Colaborador", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }


        private void RepTabPorEmpleMov2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                            Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmple()); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });//, "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Renta"].ColumnName = "Renta";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsPorEmpleGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, "", //linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0 },
                                new int[] { 1, 2, 3, 4, 5 }, new int[] { }),  // new int[] { 1 }), 
                                "RepTabPorEmpleMov2Pnls_T", tituloGrafica)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total" });  //, "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorEmpleMov2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsPorEmpleGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleMov2Pnls_G", "", "", "Colaborador", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }




        private void RepTabDetalladoFacturaTelcel(Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalladoFacturaTelcel()); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {

                DataView dvRepEmpleAExcluir = new DataView(ldt);
                ldt = dvRepEmpleAExcluir.ToTable(false,
                    new string[] { "Tel","Gente de Prosa","Centro de Costos","ID","No Centro de Costos","Nomina", "Servicios", "Servicios Adicionales",
                                    "Tiempo Aire Nacional", "Larga Distancia Nacional" ,"Tiempo Aire Roaming Nacional", "Larga Distancia Roaming Nacional",
                                    "Tiempo Aire Roaming Internacional","Larga Distancia Roaming Internacional", "Subtotal Excedente",
                                    "IVA Excedente", "Total Excedente","Total" });


                ldt.Columns["Gente de Prosa"].ColumnName = "Empleado";
                ldt.Columns["Centro de Costos"].ColumnName = "CenCos";
                ldt.Columns["Servicios"].ColumnName = "Serv.";
                ldt.Columns["Servicios Adicionales"].ColumnName = "Serv. Adic";
                ldt.Columns["No Centro de Costos"].ColumnName = "No.CenCos";
                ldt.Columns["Tiempo Aire Nacional"].ColumnName = "Tiempo A Nac";
                ldt.Columns["Larga Distancia Nacional"].ColumnName = "LDN";
                ldt.Columns["Tiempo Aire Roaming Nacional"].ColumnName = "Tiempo A Roam Nac";
                ldt.Columns["Larga Distancia Roaming Nacional"].ColumnName = "LDRN";
                ldt.Columns["Tiempo Aire Roaming Internacional"].ColumnName = "Tiempo A Roam Int";
                ldt.Columns["Larga Distancia Roaming Internacional"].ColumnName = "LDRI";
                ldt.Columns["Subtotal Excedente"].ColumnName = "Subtotal Exc";
                ldt.Columns["IVA Excedente"].ColumnName = "IVA Exc";
                ldt.Columns["Total Excedente"].ColumnName = "Total Exc";

                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabDetalleFacturaTelcel_T", ldt, true, "Total",
                                new string[] {  "", "", "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}",
                                                "{0:c}", "{0:c}" }, "",
                                new string[] { }, 2, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }, new int[] { }),
                                "RepTabDetalladoFacturaTelcel_T", tituloGrid)
                );

            #endregion Grid
        }

        private void RepTabCenCosJerarquicoP1(Control contenedor, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {

            DataTable dtCenCos = DSODataAccess.Execute("SELECT CenCos FROM " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','Español')]" +
                " WHERE dtIniVigencia <> dtFinVigencia AND dtfinvigencia >= getdate() AND iCodCatalogo=" + Session["iCodUsuario"].ToString() + " AND CenCos IS NOT NULL");

            int idCenCos = (dtCenCos.Rows.Count == 0) ? 0 : Convert.ToInt32(dtCenCos.Rows[0][0]);

            DataTable ldt = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&CenCos=",
                Request.Path + "?Nav=CenCosJerarquicoN3&CenCos=", idCenCos)); //N2 es para navegar a la pagina de CenCos y el N3 para el reporte por empleado.

            if (idCenCos == 0)
            {
                tituloGrid = tituloGrid + "\n : EL USUARIO NO TIENE CONFIGURADO UN CENTRO DE COSTOS SUPERIOR";
            }

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "iCodCatalogo", "Descripcion", "TotalSimulado", "TotalReal", "CostoSimulado", "CostoReal",
                        "SM", "Llamadas", "Minutos", "link" });
                ldt.Columns["Descripcion"].ColumnName = "Centro de costos";
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();

                ArrayList lista = FormatoColumRepJerarquico(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            if (param["Nav"] == "CenCosJerarquicoN1")   // a dos paneles
            {
                Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2_G", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                "RepTabCenCosJerarquicoP1_T", tituloGrid));
            }
            else // a un panel
            {
                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                    new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                    "RepTabCenCosJerarquicoP1", tituloGrid, Request.Path + "?Nav=CenCosJerarquicoN1", pestaniaActiva, FCGpoGraf.Tabular)
                    );
            }

            #endregion Grid

            #region Grafica

            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }


                ldt = dvldt.ToTable(false, new string[] { "Centro de costos", "Importe", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            if (param["Nav"] == "CenCosJerarquicoN1") // a dos paneles
            {
                Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabCenCosJerarquicoP12_G", tituloGrafica, 2, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf2",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                        "RepTabCenCosJerarquicoP12_G", "", "", "Centro de costos", "Importe", 2, FCGpoGraf.Tabular), false);

            }
            else // a un panel
            {

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf2",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                        "RepTabCenCosJerarquicoP1", "", "", "Centro de costos", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);
            }


            #endregion Grafica

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

        private void RepTabCenCosJerarquicoN2(string tituloGrid, string tituloGrafica)
        {
            DataTable ldt = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&CenCos=",
                    Request.Path + "?Nav=CenCosJerarquicoN3&CenCos=", Convert.ToInt32(param["CenCos"])));

            #region Grid
            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "iCodCatalogo", "Descripcion", "TotalSimulado", "TotalReal", "CostoSimulado", "CostoReal",
                        "SM", "Llamadas", "Minutos", "link" });
                ldt.Columns["Descripcion"].ColumnName = "Centro de costos";
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();

                ArrayList lista = FormatoColumRepJerarquico(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            Rep1.Controls.Add(
                   DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                   DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                   new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                   new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                   "RepTabCenCosJerarquicoN2_T", tituloGrid));
            #endregion Grid

            #region Grafica

            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }


                ldt = dvldt.ToTable(false, new string[] { "Centro de costos", "Importe", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabCenCosJerarquicoN2_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf2",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                    "RepTabCenCosJerarquicoN2_G", "", "", "Centro de costos", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabPorEmpleMasCaros2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(consultaPorEmpleMasCaros(linkGrafica, int.MaxValue));

            string nombreColumnaColaborador = "Colaborador";
            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                nombreColumnaColaborador = "Línea";
            }
            else
            {
                nombreColumnaColaborador = "Colaborador";
            }


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            int[] camposBoundField;
            string[] camposGrafica;

            if (string.IsNullOrEmpty(linkGrid))
            {
                //NZ 20160823
                if (DSODataContext.Schema.ToLower() == "k5banorte") /*|| DSODataContext.Schema.ToLower() == "evox")*/
                {
                    hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                    camposReporte = new string[] { "Codigo Empleado", "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "Puesto" };
                    camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7 };
                }
                else
                {
                    hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                    camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" };
                    camposBoundField = new int[] { 1, 2, 3, 4, 5 };
                }
                camposGrafica = new string[] { nombreColumnaColaborador, "Total" };
            }
            else
            {
                //NZ 20160823
                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                {

                    hyperLinkFieldIndex = 1;
                    camposReporte = new string[] { "Codigo Empleado", "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "Puesto", "link" };
                    camposBoundField = new int[] { 2, 3, 4, 5, 6, 7 };
                }
                else
                {

                    hyperLinkFieldIndex = 1;
                    camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" };
                    camposBoundField = new int[] { 2, 3, 4, 5 };
                }
                camposGrafica = new string[] { nombreColumnaColaborador, "Total", "link" };
            }

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);

                //BG.20161124 Se agrega validacion de esquema para el campo nomina
                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                {
                    ldt.Columns["No Nomina"].ColumnName = "Nomina";
                }

                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(   //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 8 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid)
                );
            }
            else
            {
                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid)
                );
            }

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorEmpleMasCaros2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCaros",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleMasCaros2Pnls_G", "", "", nombreColumnaColaborador, "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void ReporteDetallado(Control contenedor, string tituloGrid)
        {
            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;

            if (RepDetallado.Rows.Count > 0)
            {
                DataView dvldt = new DataView(RepDetallado);
                RemoveColHerencia(ref RepDetallado);

                RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                ArrayList lista = FormatoColumRepDetallado(RepDetallado, columnasNoVisibles, columnasVisibles);
                RepDetallado = (DataTable)((object)lista[0]);
                columnasNoVisibles = (int[])((object)lista[1]);
                columnasVisibles = (int[])((object)lista[2]);

                //20150702 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                CambiarNombresConfig(RepDetallado, "ConsultaDetalle");
                //20150702 NZ

                //NZ 20161005
                if (RepDetallado.Columns.Contains("Duracion"))
                {
                    RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                }
                if (RepDetallado.Columns.Contains("Llamadas"))
                {
                    RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                }
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
            {
                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepDetalladoGrid_T", RepDetallado, true, "Totales",
                                    new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}",
                                    "", "", "", "", "", "", ""}, "",
                                    new string[] { "Colaborador" }, 1, columnasNoVisibles, columnasVisibles, new int[] { }),
                                    "ReporteDetallado_T", tituloGrid));
            }
            else
            {
                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepDetalladoGrid_T", RepDetallado, true, "Totales",
                                    new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}",
                                    "", "", "", "", "", ""}, "",
                                    new string[] { "Colaborador" }, 1, columnasNoVisibles, columnasVisibles, new int[] { }),
                                    "ReporteDetallado_T", tituloGrid));
            }
        }

        private void CambiarNombresConfig(DataTable reporte, string nombreConsulta)
        {
            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular(nombreConsulta);
            if (nombresCamposParticulares.Rows.Count > 0)
            {
                foreach (DataRow row in nombresCamposParticulares.Rows)
                {
                    if (reporte.Columns.Contains(row["NombreOrigCampo"].ToString()))
                    {
                        reporte.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                    }
                }
            }
        }

        private ArrayList FormatoColumRepDetallado(DataTable ldt, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 8, 9, 11 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 10, 12, 13, 14, 15, 16, 17, 18 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 8, 9, 10 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 14, 15, 16, 17, 18 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 9, 10, 11, 12 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 13, 14, 15, 16, 17, 18 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 8, 10, 11, 12 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 13, 14, 15, 16, 17, 18 };
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

        private void RepTabPorCostoAreaDireccion1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorAreaDireccion());

            #region Grid
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "idDireccion", "DescDireccion", "Importe", "Renta", "MontoExc", "Link" });
                ldt.Columns["DescDireccion"].ColumnName = "Área";
                ldt.Columns["Importe"].ColumnName = "Total";
                ldt.Columns["MontoExc"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepTabCostoAreaDireccionGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "{0}", new string[] { "Link" },
                                    1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorCostoAreaDireccion1Pnl", tituloGrid, Request.Path + "?Nav=DireccionJerarq", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid


            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Área", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCostoAreaDireccion1PnlGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabPorCostoAreaDireccion1Pnl", "", "", "Área", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepTabPorCostoAreaDireccion2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid) //string linkGrid, string linkGrafica,
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorAreaDireccion()); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "idDireccion", "DescDireccion", "Importe", "Renta", "MontoExc", "Link" });
                ldt.Columns["DescDireccion"].ColumnName = "Área";
                ldt.Columns["Importe"].ColumnName = "Total";
                ldt.Columns["MontoExc"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabCostoAreaDireccionGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "{0}", new string[] { "Link" },
                                    1, new int[] { 0, 5 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorCostoAreaDireccion2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Área", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorCostoAreaDireccion2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCostoAreaDireccionGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCostoAreaDireccion2Pnls_G", "", "", "Área", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorCostoAreaSubDireccion2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorAreaSubDireccion(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "idSubDireccion", "DescSubDireccion", "Importe", "Renta", "MontoExc", "link" });
                ldt.Columns["DescSubDireccion"].ColumnName = "Área";
                ldt.Columns["Importe"].ColumnName = "Total";
                ldt.Columns["MontoExc"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabCostoAreaSubDireccionGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid,
                                    new string[] { "idSubDireccion" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorCostoAreaSubDireccion2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Área", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Área"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorCostoAreaSubDireccion2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCostoAreaDireccionGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabPorCostoAreaSubDireccion2Pnls_G", "", "", "Área", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoEmpleCC2Pnls(string linkNav, int pestaniaActiva)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            #region Reporte Costo Por Empleado Filtrando Cencos_ Se agrega el filtro de Carrier

            DataTable RepConsEmpleCC = DSODataAccess.Execute(ConsultaConsumoEmpleFiltroCarrier(""));
            if (RepConsEmpleCC.Rows.Count > 0)
            {
                DataView dvRepConsEmpleCC = new DataView(RepConsEmpleCC);
                RepConsEmpleCC = dvRepConsEmpleCC.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Nombre Carrier", "Total", "Renta", "Diferencia", "Codigo Carrier" });
                RepConsEmpleCC.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepConsEmpleCC.Columns["Total"].ColumnName = "Total";
                RepConsEmpleCC.Columns["Renta"].ColumnName = "Renta";
                RepConsEmpleCC.Columns["Diferencia"].ColumnName = "Monto excedido";
                RepConsEmpleCC.Columns["Nombre Carrier"].ColumnName = "Carrier";
                RepConsEmpleCC.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoEmpleCC2Grid_T", DTIChartsAndControls.ordenaTabla(RepConsEmpleCC, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + linkNav,
                                new string[] { "Codigo Empleado", "Linea", "Codigo Carrier" }, 1, new int[] { 0, 7 },
                                new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }), "RepTabConsumoEmpleCC2Pnls_T", "Costo por Colaborador")
                );

            #endregion Reporte Costo Por Empleado Filtrando Cencos_Se agrega el filtro de Carrier


            #region Reporte historico_Se agrega el Carrier

            //Se obtiene el DataSource del reporte
            DataTable ldt = new DataTable();


            //BG.20180208 Se valida si se está recibiendo como parámetro el Cencos y Carrier estos parámetros se envían a la consulta
            if (param["CenCos"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistAnioActualVsAnteriorFiltroCarrier());
            }

            RemoveColHerencia(ref ldt);

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            Rep2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepHistAnioAntVsActFiltroCCGrid", ldt, true, "Totales",  //20160603 NZ Se Agregan totales
                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                    "RepTabConsumoEmpleCC2Pnls", "Consumo histórico", "", pestaniaActiva, FCGpoGraf.Matricial)
                    );


            //20160603 NZ Se elimina el ultimo renglon de la tabla (Son los totales).
            if (ldt.Rows.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                        FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActFiltroCCGraf",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabConsumoEmpleCC2Pnls",
                            "Consumo Histórico por Mes", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);

            #endregion Reporte historico_Se agrega el Carrier


            #region  ReporteConsumoHistoricoInternet Historico ArmaCamposConsumoHistoricoPorEmpleadoMat empleado
            RepMatHistPorEmpleAnioActMov1Pnl(Rep9);   /* Año Actual  */
            #endregion
        }

        private void RepTabConsumoPorCtaMaestra2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoPorCtaMaestra(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "idCuentaPadre", "CuentaPadre", "Total", "link" });
                ldt.Columns["CuentaPadre"].ColumnName = "Cuenta Padre";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoPorCtaMaestraGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Importe desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "" }, linkGrid,
                                new string[] { "idCuentaPadre" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "RepTabConsumoPorCtaMaestra2Pnls_T", tituloGrid)
                );

            #endregion Grid


            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cuenta Padre", "Importe", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Cuenta Padre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Cuenta Padre"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConsumoPorCtaMaestra2Pnls_T", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCtaMaestraGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabConsumoPorCtaMaestra2Pnls_G", "", "", "Cuenta Padre", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoLineaTelcelCtaMaestra1Pnl(Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoLineaTelcelCtaMaestra());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "idCuentaPadre", "CuentaPadre", "Linea", "Nomina", "Empleado", "CenCos",
                    "Centro de Costo", "NoEmpresa", "Empresa", "Importe" });
                ldt.Columns["CuentaPadre"].ColumnName = "Cuenta Padre";
                ldt.Columns["Nomina"].ColumnName = "No Nomina";
                ldt.Columns["CenCos"].ColumnName = "No CeCos";
                ldt.Columns["NoEmpresa"].ColumnName = "No Empresa";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoLineaTelcelCtaMaestraGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Importe desc"), true, "Totales",
                                new string[] { "", "", "", "", "", "", "", "", "", "{0:c}" }, "",
                                new string[] { "Cuenta Padre" }, 1, new int[] { 0 },
                                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new int[] { }), "RepTabConsumoLineaTelcelCtaMaestra1Pnl_T", tituloGrid)
                );

            #endregion Grid
        }

        private void RepTabConsumoMovilesPorTecnologia1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoMovilesPorTecnologia("[Link] = ''" + Request.Path + "?Nav=EmpleN1&EqCelular='' + convert(varchar,[idTecnologia])"));

            #region Grid

            RemoveColHerencia(ref ldt);

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "idTecnologia", "Tecnologia", "Total", "Renta", "Excedente", "Link" });
                ldt.Columns["Tecnologia"].ColumnName = "Tecnologias";
                ldt.Columns["Excedente"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepTabConsumoMovilesPorTecnologiaGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "?Nav=EmpleN1&EqCelular={0}", new string[] { "idTecnologia" },
                                    1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabConsumoMovilesPorTecnologia1Pnl", tituloGrid, Request.Path + "?Nav=ConsumoMovilPorTecno", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tecnologias", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tecnologias"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tecnologias"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoMovilesPorTecnologiaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsumoMovilesPorTecnologia1Pnl", "", "", "Tecnologia", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoMovilesPorTecnologia2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                      Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoMovilesPorTecnologia("[Link] = ''" + Request.Path + "?Nav=EmpleN1&EqCelular='' + convert(varchar,[idTecnologia])"));
            RemoveColHerencia(ref ldt);

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "idTecnologia", "Tecnologia", "Total", "Renta", "Excedente", "Link" });
                ldt.Columns["Tecnologia"].ColumnName = "Tecnologias";
                ldt.Columns["Excedente"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabConsumoMovilesPorTecnologiaGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "?Nav=EmpleN1&EqCelular={0}", new string[] { "idTecnologia" },
                                    1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabConsumoMovilesPorTecnologia2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tecnologias", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tecnologias"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tecnologias"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConsumoMovilesPorTecnologia2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoMovilesPorTecnologiaGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsumoMovilesPorTecnologia2Pnls_G", "", "", "Tecnologia", "Total", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoPorCarrierFiltroCC2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid, string linkGrid, string linkGrafica, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoPorCarrierFiltroCC(linkGrafica));
            RemoveColHerencia(ref ldt);

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Carrier", "Descripcion", "Total", "Renta", "Diferencia", "Link" });
                ldt.Columns["Descripcion"].ColumnName = "Carrier";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabConsumoPorCarrierFiltroCCGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid, new string[] { "Codigo Carrier" },
                                    1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabConsumoPorCarrierFiltroCC2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Carrier", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConsumoPorCarrierFiltroCC2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCarrierFiltroCCGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabConsumoPorCarrierFiltroCC2Pnls_G", "", "", "Carrier", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoPorCarrierFiltroCC1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            // string linkGrid, string linkGrafica,
            string linkGrid = Request.Path + "?Nav=EmpleN1&Carrier={0}";
            string linkGrafica = "[Link] = ''" + Request.Path + "?Nav=EmpleN1&Carrier='' + convert(varchar,[Codigo Carrier])";
            string linkLupita = "?Nav=CarrierCCN2";


            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoPorCarrierFiltroCC(linkGrafica)); //"[link] = ''" + Request.Path + "?Nav=SubDireccion&Cencos='' + convert(varchar,[idDireccion])"));
            RemoveColHerencia(ref ldt);

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Carrier", "Descripcion", "Total", "Renta", "Diferencia", "Link" });
                ldt.Columns["Descripcion"].ColumnName = "Carrier";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepTabConsumoPorCarrierFiltroCC1PnlGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid, new string[] { "Codigo Carrier" },
                                    1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabConsumoPorCarrierFiltroCC1Pnl", tituloGrid, linkLupita, pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Carrier", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCarrierFiltroCC1PnlGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                 "RepTabConsumoPorCarrierFiltroCC1Pnl", "", "", "Carrier", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void ReporteGlobalTelMovil1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoGlobalTelefoniaMovil());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Direccion", "SubDireccion", "Nombre Completo", "Nombre Carrier", "Linea", "Total", "Renta", "Diferencia" });

                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("ReporteGlobalTelMovilGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Direcccion asc, Linea asc, Total desc"), true, "Totales",
                                    new string[] { "", "", "", "", "", "{0:c}", "{0:c}", "{0:c}" }, "", new string[] { "" },
                                    0, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new int[] { }), "ReporteGlobalTelMovil1Pnl_T", tituloGrid)
                    );

            #endregion Grid
        }

        private void RepTabHistMovCantLineas1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoMoviles());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "TotalLineas", "TotalSinIVA", "TotalConIVA" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["TotalLineas"].ColumnName = "Cant. Líneas";
                ldt.Columns["TotalSinIVA"].ColumnName = "Total sin IVA";
                ldt.Columns["TotalConIVA"].ColumnName = "Total con IVA";
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistCantLineasGrid", ldt, true, "Totales",
                               new string[] { "", "{0:0,0}", "{0:c}", "{0:c}" }),
                               "RepTabHistMovCantLineas1Pnl", tituloGrid, Request.Path + "?Nav=HistoricoConCantLineasN1", pestaniaActiva, FCGpoGraf.Tabular)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Cant. Líneas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Cant. Líneas"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistCantLineasGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "RepTabHistMovCantLineas1Pnl", "", "", "Mes", "Cant. de líneas", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabHistMovCantLineas2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoMoviles());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "TotalLineas", "TotalSinIVA", "TotalConIVA" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["TotalLineas"].ColumnName = "Cant. Líneas";
                ldt.Columns["TotalSinIVA"].ColumnName = "Total sin IVA";
                ldt.Columns["TotalConIVA"].ColumnName = "Total con IVA";
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepHistCantLineasGrid_T", ldt, true, "Totales",
                               new string[] { "", "{0:0,0}", "{0:c}", "{0:c}" }),
                               "RepTabHistMovCantLineas2Pnls_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Cant. Líneas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Cant. Líneas"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabHistMovCantLineas2Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistCantLineasGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                 "RepTabHistMovCantLineas2Pnls_G", "", "", "Mes", "Cant. de líneas", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void ReporteHistorico2SeriesEnRep2(int pestaniaActiva)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            #region Reporte historico

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [Emple] = " + param["Emple"] + "', \n "));

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }
            Rep2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepHistAnioAntVsActConFiltroGrid",
                                    ldt, true, "Totales",   //20160603 NZ Se agregan totales.
                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                    "ReporteHistorico2SeriesEnRep2", "Consumo histórico", "", pestaniaActiva, FCGpoGraf.Matricial)
                    );
            //20160603 NZ Se elimina el ultimo renglon de la tabla (Son los totales).
            if (ldt.Rows.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActConFiltroGraf",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "ReporteHistorico2SeriesEnRep2",
                            "Consumo Histórico Por Mes", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);
            #endregion Reporte historico
        }

        private void ReporteHistoricoPeEmple()
        {
            #region Reporte historico

            //Se obtiene el DataSource del reporte
            DataTable GrafConsHistDetFacPEmN1 = DSODataAccess.Execute(ConsultaConsumoHistoricoPerfilEmpleado());

            if (GrafConsHistDetFacPEmN1.Rows.Count > 0)
            {
                DataView dvGrafConsHistDetFacPEmN1 = new DataView(GrafConsHistDetFacPEmN1);
                GrafConsHistDetFacPEmN1 = dvGrafConsHistDetFacPEmN1.ToTable(false, new string[] { "Nombre Mes", "Total" });
                GrafConsHistDetFacPEmN1.Columns["Nombre Mes"].ColumnName = "Mes";
                GrafConsHistDetFacPEmN1.Columns["Total"].ColumnName = "Total";
                GrafConsHistDetFacPEmN1.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepHistoricoPerfEmpleGrid_T", DTIChartsAndControls.ordenaTabla(GrafConsHistDetFacPEmN1,
                                "Total desc"), true, "Totales", new string[] { "", "{0:c}" }), "ReporteHistoricoPeEmple_T", "Consumo historico")
                );

            if (GrafConsHistDetFacPEmN1.Rows.Count > 0)
            {
                if (GrafConsHistDetFacPEmN1.Rows[GrafConsHistDetFacPEmN1.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    GrafConsHistDetFacPEmN1.Rows[GrafConsHistDetFacPEmN1.Rows.Count - 1].Delete();
                }
                GrafConsHistDetFacPEmN1.Columns["Mes"].ColumnName = "label";
                GrafConsHistDetFacPEmN1.Columns["Total"].ColumnName = "value";
                GrafConsHistDetFacPEmN1.AcceptChanges();
            }
            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ReporteHistoricoPeEmple_G", "Consumo historico", 2, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHistDetFacPEmN1_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConsHistDetFacPEmN1),
               "ReporteHistoricoPeEmple_G", "Consumo histórico de año actual", "", "Mes", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Reporte historico
        }

        private void ReporteHistoricoPeEmple(Control contenedorGrafica, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160111 se agrega un 3er año hacia atras.*/
            if (DSODataContext.Schema.ToString().ToLower() == "sperto")
            {
                anioActual = "2017";
                anioAnterior = "2016";
                dosAnioAtras = "2015";
            }
            #region Reporte historico

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMoviles());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActGrid_T", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "ReporteHistoricoPeEmple_T", tituloGrid)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "ReporteHistoricoPeEmple_G", tituloGrafica, 2, FCGpoGraf.Matricial));


            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf_G",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "ReporteHistoricoPeEmple_G",
                tituloGrafica, "", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

            #endregion Grafica
            #endregion Reporte historico
        }

        private void ReporteXEmpleado(string linkNav)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            DataTable RepConsXEmple = DSODataAccess.Execute(ConsultaCostoPorEmpleado(""));
            if (RepConsXEmple.Rows.Count > 0)
            {
                DataView dvRepConsXEmple = new DataView(RepConsXEmple);
                RepConsXEmple = dvRepConsXEmple.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                RepConsXEmple.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepConsXEmple.Columns["Total"].ColumnName = "Total";
                RepConsXEmple.Columns["Renta"].ColumnName = "Renta";
                RepConsXEmple.Columns["Diferencia"].ColumnName = "Monto excedido";
                RepConsXEmple.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsXEmple_T", DTIChartsAndControls.ordenaTabla(RepConsXEmple, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}" }, Request.Path + linkNav,
                                new string[] { "Codigo Empleado", "Linea" }, 1, new int[] { 0 },
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "ReporteXEmpleado_T", "Costo por colaborador")
                );

            //20141027 AM. Se agrega grafica historica del cencos seleccionado
            #region Reporte historico


            //Se obtiene el DataSource del reporte
            DataTable ldt = new DataTable();

            //Se valida si se está recibiendo como parámetro el CenCos o el Emple, este parámetro se envía a la consulta
            if (param["CenCos"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [CenCos] = " + param["CenCos"] + "', \n "));
            }
            else if (param["Emple"] != string.Empty)
            {
                ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [Emple] = " + param["Emple"] + "', \n "));
            }
            RemoveColHerencia(ref ldt);

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            Rep2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepHistAnioAntVsActConFiltroGrid_T", ldt, true, "Totales", //20160603 NZ Se Agregan totales
                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                    "RepHistAnioAntVsActConFiltroGrid", "Consumo histórico", "", 2, FCGpoGraf.Matricial)
                    );


            //20160603 NZ Se elimina el ultimo renglon de la tabla (Son los totales).
            if (ldt.Rows.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
            }
            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                        FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActConFiltroGraf_G",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepHistAnioAntVsActConFiltroGrid",
                            "Consumo histórico por mes", "", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

            #endregion Reporte historico
        }

        private void ReporteXEmpleadoSinNav()
        {
            #region Reporte por empleado

            DataTable RepCostoXEmple = DSODataAccess.Execute(ConsultaCostoPorEmpleadoPerfEmpleado());
            if (RepCostoXEmple.Rows.Count > 0)
            {
                DataView dvRepCostoXEmple = new DataView(RepCostoXEmple);
                RepCostoXEmple = dvRepCostoXEmple.ToTable(false,
                    new string[] { "Linea Carrier", "Total", "Renta", "Diferencia", "Codigo Carrier", "Linea" });//Nuevos 0,4,5
                //RM 20180220 Cambios
                RepCostoXEmple.Columns["Linea Carrier"].ColumnName = "Tel";
                RepCostoXEmple.Columns["Codigo Carrier"].ColumnName = "idCarrier";
                RepCostoXEmple.Columns["Renta"].ColumnName = "Renta";
                RepCostoXEmple.Columns["Diferencia"].ColumnName = "Monto excedido";
                RepCostoXEmple.AcceptChanges();
            }

            Rep0.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla( /*/ 20150331 NZ SeGridViewConFormatosEnLink Hace lo mismo que el GridView solo que este deja los formatos correctos en las columnas que son Link /*/
                            DTIChartsAndControls.GridViewConFormatosEnLink("RepCostoXEmple_T", DTIChartsAndControls.ordenaTabla(RepCostoXEmple, "Total desc"),
                            true, "Totales", false, 0, new string[] { "", "{0:c}", "{0:c}", "{0:c}" },
                            Request.Path + "?Nav=DetFacPEmN2&Concepto=237&Carrier={1}&Linea={2}",
                            new string[] { "Renta", "idCarrier", "Linea" }, 0, new int[] { },
                            new int[] { 0, 1, 3 }, new int[] { 2 }), "ReporteXEmpleadoSinNav_T", "Consumo por colaborador", Request.Path + "?Nav=CostoXEmpleN1")
            );
            #endregion Reporte por empleado
        }

        private void ReporteXConcepto(Control Contenedor)
        {
            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
            if (RepDetalleXConcepto.Rows.Count > 0)
            {
                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                    new string[] { "Concepto", "Total" });
                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                RepDetalleXConcepto.AcceptChanges();
            }

            Contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalleXConcepto_T", RepDetalleXConcepto,
                                true, "Totales", new string[] { "", "{0:c}" }), "ReporteXConcepto_T", "Detalle de factura")
                );
        }

        private void ReporteHistoricoDelEmpleado2Series(Control Contenedor)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160112 se agrega un 3er año hacia atras.*/

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMovilesConFiltro("And [Emple] = " + param["Emple"] + "', \n "));

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.AcceptChanges();
            }

            Contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepHistAnioAntVsActConFiltroGrid_T",
                                    ldt, true, "Totales",  //20160603 NZ Se agregan Totales
                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                    "ReporteHistoricoDelEmpleado2Series", "Consumo histórico", "", 2, FCGpoGraf.Matricial)
                    );

            //20160603 NZ Se elimina el ultimo renglon de la tabla (Son los totales).
            if (ldt.Rows.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
            }


            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActConFiltroGraf_G",
                            FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "ReporteHistoricoDelEmpleado2Series",
                            "", "Consumo histórico año actual vs año anterior", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

        }

        private void ReporteXConceptoConNav(Control Contenedor, string linkGrid, string tituloReporte)
        {
            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
            if (RepDetalleXConcepto.Rows.Count > 0)
            {
                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                    new string[] { "idConcepto", "Concepto", "Total", "Carrier" });
                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                RepDetalleXConcepto.AcceptChanges();
            }

            Contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                "RepDetalleXConcepto", RepDetalleXConcepto, true, "Totales",
                                new string[] { "", "", "{0:c}", "" }, linkGrid,
                                new string[] { "idConcepto", "Carrier" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "ReporteXConceptoConNav", tituloReporte)
                );
        }

        private void ReporteDetFacturaEnRep1()
        {
            #region Reporte Detalle factura telcel

            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());

            //20150331 NZ Se remueve el concepto de Rentas de este reporte.
            RepDetalleXConcepto = CostoXConceptoRemoverRentas(RepDetalleXConcepto);

            if (RepDetalleXConcepto.Rows.Count > 0)
            {
                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                    new string[] { "idConcepto", "Concepto", "Total" });
                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                RepDetalleXConcepto.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                "RepDetalleXConcepto", RepDetalleXConcepto, true, "Totales",
                                new string[] { "", "", "{0:c}" }, Request.Path + "?Nav=DetFacPEmN2&Concepto={0}",
                                new string[] { "idConcepto" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "ReporteDetFacturaEnRep1_T", "Costo por Concepto")
                );

            #endregion Reporte Detalle factura telcel
        }

        private void ReporteDetFacturaXEmpleado()
        {
            #region Reporte Costo por Empleado

            /*BG.20160317 se agrega validacion de esquema. Si se trata de nemak, agregara un nivel mas a su navegacion. */
            if (DSODataContext.Schema.ToLower().Trim() != "nemak")
            {
                DataTable RepDesgloseFacturaPorConcepto = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorConcepto());

                if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                {
                    DataView dvRepDesgloseFacturaPorConcepto = new DataView(RepDesgloseFacturaPorConcepto);
                    RepDesgloseFacturaPorConcepto = dvRepDesgloseFacturaPorConcepto.ToTable(false,
                        new string[] { "Concepto", "Descripcion", "Total" });
                    RepDesgloseFacturaPorConcepto.Columns["Total"].ColumnName = "Importe";
                    RepDesgloseFacturaPorConcepto.AcceptChanges();
                }

                //20141205 AM. Se agrega condicion para validar que el datatable tenga datos
                if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                {
                    #region Seleccion de reporte

                    if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                    {
                        Rep0.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepDesgloseFacturaPorConcepto", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                    true, "Totales", new string[] { "", "", "{0:c}" }), "ReporteDetFacturaXEmpleado", "Consumo desglosado por concepto")
                        );
                    }
                    else
                    {
                        /*BG. 20150414 Se agrega esta linea para que la variable global Emple contenga un valor(icodCatalogo), obteniendose a partir de su usuario. 
                        BG. 20150429 Agregue una validacion del Perfil para que no fallaran los filtros en el reporte para los diferentes Perfiles.*/

                        if (Session["iCodPerfil"].ToString() != "205629")
                        {
                            param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();

                        }

                        DataTable RepDesgloseFacturaPorLlamada = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorLlamadas());

                        if (RepDesgloseFacturaPorLlamada.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorLlamada = new DataView(RepDesgloseFacturaPorLlamada);
                            RepDesgloseFacturaPorLlamada = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                                new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                            RepDesgloseFacturaPorLlamada.Columns["Fecha Llamada"].ColumnName = "Fecha";
                            RepDesgloseFacturaPorLlamada.Columns["Hora Llamada"].ColumnName = "Hora";
                            RepDesgloseFacturaPorLlamada.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepDesgloseFacturaPorLlamada.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            RepDesgloseFacturaPorLlamada.Columns["Costo"].ColumnName = "Total";
                            RepDesgloseFacturaPorLlamada.Columns["Punta A"].ColumnName = "Localidad Origen";
                            RepDesgloseFacturaPorLlamada.Columns["Dir Llamada"].ColumnName = "Tipo";
                            RepDesgloseFacturaPorLlamada.Columns["Punta B"].ColumnName = "Localidad Destino";
                            RepDesgloseFacturaPorLlamada.AcceptChanges();
                        }

                        Rep0.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepDesgloseFacturaPorLlamada", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                               true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "" }), "ReporteDetFacturaXEmpleado_T", "Detalle de llamadas Telcel sin filtrar claves cargo")
                        );
                    }

                    #endregion Seleccion de reporte
                }
            }
            else
            {
                DataTable RepDesgloseFacturaPorConcepto = DSODataAccess.Execute(ConsultaConsumoDesgloseRoaming());


                //20141205 AM. Se agrega condicion para validar que el datatable tenga datos
                if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                {
                    #region Seleccion de reporte desglose roaming

                    /* BG.20160318 SE OBTIENE EL ICODCATALOGO DE LOS CONCEPTOS 
                     * Importe Tiempo Aire Roaming Internacional
                     * Importe Larga Distancia Roaming Internacional */

                    StringBuilder consultaObtieneTmpAirRI = new StringBuilder();
                    consultaObtieneTmpAirRI.Append("select iCodRegistro \r");
                    consultaObtieneTmpAirRI.Append("from " + DSODataContext.Schema + ".Catalogos \r");
                    consultaObtieneTmpAirRI.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                    consultaObtieneTmpAirRI.Append("and vchCodigo = 'ImpTmpAirRI'  \r");
                    int iCodCatTmpAirRI = Convert.ToInt32(DSODataAccess.ExecuteScalar(consultaObtieneTmpAirRI.ToString()));


                    StringBuilder consultaObtieneLDRI = new StringBuilder();
                    consultaObtieneLDRI.Append("select iCodRegistro \r");
                    consultaObtieneLDRI.Append("from " + DSODataContext.Schema + ".Catalogos \r");
                    consultaObtieneLDRI.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                    consultaObtieneLDRI.Append("and vchCodigo = 'ImpLDRI'  \r");
                    int iCodCatLDRI = Convert.ToInt32(DSODataAccess.ExecuteScalar(consultaObtieneLDRI.ToString()));



                    if (RepDesgloseFacturaPorConcepto.Rows[0]["idConcepto"].ToString() == iCodCatLDRI.ToString() ||
                        RepDesgloseFacturaPorConcepto.Rows[0]["idConcepto"].ToString() == iCodCatTmpAirRI.ToString() ||
                        RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() == "VER DETALLE"
                        )
                    {
                        if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorConcepto = new DataView(RepDesgloseFacturaPorConcepto);
                            RepDesgloseFacturaPorConcepto = dvRepDesgloseFacturaPorConcepto.ToTable(false,
                                new string[] { "Concepto", "Total" });
                            RepDesgloseFacturaPorConcepto.Columns["Total"].ColumnName = "Importe";
                            RepDesgloseFacturaPorConcepto.AcceptChanges();
                        }


                        //Detalle de llamadas
                        DataTable RepDesgloseFacturaPorLlamada = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorLlamadas());

                        if (RepDesgloseFacturaPorLlamada.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorLlamada = new DataView(RepDesgloseFacturaPorLlamada);
                            RepDesgloseFacturaPorLlamada = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                                new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                            RepDesgloseFacturaPorLlamada.Columns["Fecha Llamada"].ColumnName = "Fecha";
                            RepDesgloseFacturaPorLlamada.Columns["Hora Llamada"].ColumnName = "Hora";
                            RepDesgloseFacturaPorLlamada.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepDesgloseFacturaPorLlamada.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            RepDesgloseFacturaPorLlamada.Columns["Costo"].ColumnName = "Total";
                            RepDesgloseFacturaPorLlamada.Columns["Punta A"].ColumnName = "Localidad Origen";
                            RepDesgloseFacturaPorLlamada.Columns["Dir Llamada"].ColumnName = "Tipo";
                            RepDesgloseFacturaPorLlamada.Columns["Punta B"].ColumnName = "Localidad Destino";
                            RepDesgloseFacturaPorLlamada.AcceptChanges();
                        }

                        Rep0.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepDesgloseFacturaPorLlamada", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                               true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "" }), "RepDesgloseFacturaPorLlamada_T", "Detalle de llamadas Telcel sin filtrar claves cargo")
                        );


                        return;
                    }


                    if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DESGLOSE")
                    {
                        if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorConcepto = new DataView(RepDesgloseFacturaPorConcepto);
                            RepDesgloseFacturaPorConcepto = dvRepDesgloseFacturaPorConcepto.ToTable(false,
                                new string[] { "Concepto", "Total" });
                            RepDesgloseFacturaPorConcepto.Columns["Total"].ColumnName = "Importe";
                            RepDesgloseFacturaPorConcepto.AcceptChanges();
                        }

                        Rep0.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepDesgloseFacturaPorConcepto", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                    true, "Totales", new string[] { "", "", "{0:c}" }), "RepDesgloseFacturaPorConcepto_T", "Consumo desglosado por concepto ")
                        );
                    }
                    else if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() == "VER DESGLOSE")
                    {

                        DataTable RepDesgloseConceptosRoaming = DSODataAccess.Execute(ConsultaDetalleFacturaRoamInternVoz());

                        if (RepDesgloseConceptosRoaming.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseConceptosRoaming = new DataView(RepDesgloseConceptosRoaming);
                            RepDesgloseConceptosRoaming = dvRepDesgloseConceptosRoaming.ToTable(false,
                                new string[] { "Concepto", "Total", "idConcepto", "Ruta" });
                            RepDesgloseConceptosRoaming.Columns["Total"].ColumnName = "Importe";
                            RepDesgloseConceptosRoaming.AcceptChanges();
                        }

                        Rep0.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               DTIChartsAndControls.GridView("RepDesgloseRoamingInternVoz", DTIChartsAndControls.ordenaTabla(RepDesgloseConceptosRoaming, "Total desc"),
                               true, "Totales", new string[] { "", "{0:c}" }, Request.Path + "?Nav=EmpleN4&Concepto={0}&Emple=" + param["Emple"] + "&Linea=" + param["Linea"],
                               new string[] { "idConcepto" }, 0, new int[] { }, new int[] { 1 }, new int[] { 0 }), "RepDesgloseRoamingInternVoz_T", "Detalle Roaming Internacional Voz")
                        );
                    }

                    #endregion Seleccion de reporte desglose roaming
                }

            }


            #endregion Reporte Costo por Empleado
        }

        private void ReporteLlamadasMasLargas()
        {
            #region Reporte Llamadas mas largas

            lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
            lblCantMaxReg.Text = "Cantidad máxima de registros:";
            if (!Page.IsPostBack)
            {
                txtCantMaxReg.Text = "10";
            }

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
            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamMasLarg_T", RepLlamMasLarg,
                                true, "Totales", new string[] { "", "", "", "", "", "", "{0:c}", "", "", "" }), "ReporteLlamadasMasLargas_T", "Llamadas mas largas")
                );

            #endregion Reporte Llamadas mas largas
        }

        private void ReporteLlamadasMasCostosas()
        {
            #region Reporte Llamadas mas costosas

            lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
            lblCantMaxReg.Text = "Cantidad máxima de registros:";
            if (!Page.IsPostBack)
            {
                txtCantMaxReg.Text = "10";
            }

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
            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamMasCostosas", RepLlamMasCostosas,
                                true, "Totales", new string[] { "", "", "", "", "", "", "{0:c}", "", "", "" }), "ReporteLlamadasMasCostosas_T", "Llamadas mas costosas ")
                );

            #endregion Reporte Llamadas mas costosas
        }

        private void ReporteNumMarcACelDeLaRed()
        {
            #region Reporte numeros marcados a celulares de red

            DataTable ldt = DSODataAccess.Execute(
                ConsultaNumerosMarcadosACelRed(
                "[link] = ''" + Request.Path + "?Nav=LlamACelRedN3&Exten=" + param["Exten"].Replace("'", "") + "&NumMarc='' + convert(varchar,[Numero Marcado])"));
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "CodNumMarc", "Numero Marcado", "Etiqueta", "Total", "Duracion", "Numero", "link" });
                ldt.Columns["Numero Marcado"].ColumnName = "Número marcado";
                ldt.Columns["Etiqueta"].ColumnName = "Etiqueta";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Duracion"].ColumnName = "Minutos";
                ldt.Columns["Numero"].ColumnName = "Llamadas";
                ldt.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                "RepNumerosLlamACeluRedGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "{0:c}", "", "", "" }, Request.Path + "?Nav=LlamACelRedN3&Exten=" + param["Exten"].Replace("'", "") + "&NumMarc={0}",
                                new string[] { "CodNumMarc" }, 1, new int[] { 0 },
                                new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "ReporteNumMarcACelDeLaRed_T", "Llamadas a celulares de la red")
                );

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Número marcado", "Minutos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Número marcado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Número marcado"].ColumnName = "label";
                ldt.Columns["Minutos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ReporteNumMarcACelDeLaRed_G", "Llamadas a celulares de la red", 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepNumerosLlamACeluRedGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ReporteNumMarcACelDeLaRed_G", "", "", "Número marcado", "Minutos", 2, FCGpoGraf.Tabular), false);

            #endregion Reporte numeros marcados a celulares de red
        }

        private void ReporteDetalladoPeEmple()
        {
            #region Reporte detalle llamadas (perfil emple)

            DataTable RepDetalleLlamPerfEmple = DSODataAccess.Execute(ConsultaDetalleDeLlamadasPerfilEmple());
            if (RepDetalleLlamPerfEmple.Rows.Count > 0)
            {
                DataView dvRepDetalleLlamPerfEmple = new DataView(RepDetalleLlamPerfEmple);
                RepDetalleLlamPerfEmple = dvRepDetalleLlamPerfEmple.ToTable(false,
                    new string[] { "FecDeLlamada", "Hora Llamada", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                RepDetalleLlamPerfEmple.Columns["FecDeLlamada"].ColumnName = "Fecha";
                RepDetalleLlamPerfEmple.Columns["Hora Llamada"].ColumnName = "Hora";
                RepDetalleLlamPerfEmple.Columns["Duracion Minutos"].ColumnName = "Minutos";
                RepDetalleLlamPerfEmple.Columns["Costo"].ColumnName = "Total";
                RepDetalleLlamPerfEmple.Columns["Punta A"].ColumnName = "Localidad";
                RepDetalleLlamPerfEmple.Columns["Dir Llamada"].ColumnName = "Tipo";
                RepDetalleLlamPerfEmple.Columns["Punta B"].ColumnName = "Localidad origen";
                RepDetalleLlamPerfEmple.AcceptChanges();
            }
            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalleLlamPerfEmple", DTIChartsAndControls.ordenaTabla(RepDetalleLlamPerfEmple, "Total desc"),
                                true, "Totales", new string[] { "", "", "", "{0:c}", "", "", "" }), "ReporteDetalladoPeEmple_T", "Detalle de llamadas ")
                );

            #endregion Reporte detalle llamadas (perfil emple)
        }

        private void ReporteDetalleFacturaLinea(string linkGrid, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCarasDetFact());
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Ver Detalle Ruta", "Tipo Detalle", "Linea", "Costo", "Min Libres Pico",
                                        "Min Facturables Pico", "Min Libres No Pico", "Min Facturables No Pico", "Tiempo Aire Nacional","Tiempo Aire Roaming Nac",
                                        "Tiempo Aire Roaming Int","Larga Distancia Nac","Larga Distancia Roam Nac","Larga Distancia Roam Int","Servicios Adicionales",
                                        "Desc Tiempo Aire","Ajustes","Otros Desc", "Cargos Creditos", "Otros Serv"});
                ldt.Columns["Tipo Detalle"].ColumnName = "Ver";
                ldt.Columns["Linea"].ColumnName = "Línea";
                ldt.Columns["Costo"].ColumnName = "Total";
                ldt.Columns["Min Libres Pico"].ColumnName = "Minutos libres pico";
                ldt.Columns["Min Facturables Pico"].ColumnName = "Minutos facturables pico";
                ldt.Columns["Min Libres No Pico"].ColumnName = "Minutos libres no pico";
                ldt.Columns["Min Facturables No Pico"].ColumnName = "Minutos facturables no pico";
                ldt.Columns["Tiempo Aire Nacional"].ColumnName = "Tiempo aire nacional";
                ldt.Columns["Tiempo Aire Roaming Nac"].ColumnName = "Tiempo aire roaming nacional";
                ldt.Columns["Tiempo Aire Roaming Int"].ColumnName = "Tiempo aire roaming internacional";
                ldt.Columns["Larga Distancia Nac"].ColumnName = "Larga distancia nacional";
                ldt.Columns["Larga Distancia Roam Nac"].ColumnName = "Larga distancia roaming nacional";
                ldt.Columns["Larga Distancia Roam Int"].ColumnName = "Larga distancia roaming internacional";
                ldt.Columns["Servicios Adicionales"].ColumnName = "Servicios adicionales";
                ldt.Columns["Desc Tiempo Aire"].ColumnName = "Descuento de tiempo aire";
                ldt.Columns["Ajustes"].ColumnName = "Ajustes";
                ldt.Columns["Otros Desc"].ColumnName = "Otros descuentos";
                ldt.Columns["Cargos Creditos"].ColumnName = "Cargos y créditos";
                ldt.Columns["Otros Serv"].ColumnName = "Otros servicios";
                ldt.AcceptChanges();
            }
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                           DTIChartsAndControls.GridView(
                           "ReporteDetalleFacturaLineaGrid_T", ldt, true, "Totales",
                           new string[] { "", "", "", "{0:c}", "", "", "",
                               "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}","{0:c}","{0:c}","{0:c}","{0:c}", "{0:c}","{0:c}","{0:c}",}, linkGrid,
                           new string[] { "Ver Detalle Ruta" }, 1, new int[] { 0 },
                           new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }, new int[] { 1 }), "ReporteDetalleFacturaLinea_T", tituloGrid)
           );
        }

        private void ReportePorNumeroMarcado(string linkGrid, string linkGrafica, Control contenedorGrafica, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(ConsultaRepPorNumeroMarcado(linkGrafica));

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "NumMarc", "Numero Marcado", "Costo", "Duracion Minutos", "Llamadas", "TpoProm", "Punta B", "link" });
                ldt.Columns["Numero Marcado"].ColumnName = "Número marcado";
                ldt.Columns["Costo"].ColumnName = "Total";
                ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                ldt.Columns["Llamadas"].ColumnName = "Llamadas";
                ldt.Columns["TpoProm"].ColumnName = "Tiempo promedio";
                ldt.Columns["Punta B"].ColumnName = "Localidad";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "ReportePorNumeroMarcadoGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, linkGrid,
                                    new string[] { "NumMarc" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }), "ReportePorNumeroMarcado_T", tituloGrid)
                    );

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Número marcado", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Número marcado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Número marcado"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ReportePorNumeroMarcado_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReportePorNumeroMarcadoGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ReportePorNumeroMarcado_T", "", "", "Sitio", "Importe", 2, FCGpoGraf.Tabular), false);
        }

        private void ReporteDetalleLineaNumMarc(Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepDetalleLineaNumMarc());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Fecha Llamada", "Hora Llamada", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                ldt.Columns["Fecha Llamada"].ColumnName = "Fecha";
                ldt.Columns["Hora Llamada"].ColumnName = "Hora";
                ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                ldt.Columns["Costo"].ColumnName = "Total";
                ldt.Columns["Punta A"].ColumnName = "Localidad";
                ldt.Columns["Dir Llamada"].ColumnName = "Tipo";
                ldt.Columns["Punta B"].ColumnName = "Localidad origen";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ReporteDetalleLineaNumMarc_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                true, "Totales", new string[] { "", "", "", "{0:c}", "", "", "" }), "ReporteDetalleLineaNumMarc_T", tituloGrid));

            #endregion Grid

        }

        private void ReportePorLineas(Control Contenedor, string linkGrid, string tituloReporte, string lsCarrier)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", lsCarrier));
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "ExtensionCod", "Extension", "NombreCompleto", "Total" });
                ldt.Columns["Extension"].ColumnName = "Línea";
                ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            Contenedor.Controls.Add(
                     DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                     DTIChartsAndControls.GridView(
                                     "RepTabTopLinMasCarasGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                     new string[] { "", "", "", "{0:c}" }, linkGrid,
                                     new string[] { "ExtensionCod" }, 1, new int[] { 0 },
                                     new int[] { 2, 3 }, new int[] { 1 }), "ReportePorLineas_T", tituloReporte)
                     );
        }

        private void RepTabHistAnioAnteriorVsActualMovPeEmple(Control contenedor, string tituloGrafica)
        {
            string anioActual = DateTime.Now.Year.ToString();
            string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
            string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString(); /*BG.20160111 se agrega un 3er año hacia atras.*/
            if (DSODataContext.Schema.ToString().ToLower() == "sperto")
            {
                anioActual = "2017";
                anioAnterior = "2016";
                dosAnioAtras = "2015";
            }
            //NZ 20150512 Se agrega Tab para el grid y la grafica.

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMoviles());

            #region Grid
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepHistAnioAntVsActGrid", ldt, true, "Totales",
                               new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                               "RepTabHistAnioAnteriorVsActualMovPeEmple", tituloGrafica, Request.Path + "?Nav=HistoricoPeEmN1", 2, FCGpoGraf.Matricial)
               );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHistAnioAnteriorVsActualMovPeEmple",
                tituloGrafica, "", "Mes", "Importe", 2, FCGpoGraf.Matricial), false);

            #endregion Grafica

        }

        private void RepTabDetalleLlamsF4Telcel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleLlamsTelcelF4());
            //"[link] = ''" + Request.Path + "?Nav=DetalleLlamsTelcelF4N2&Linea=" + "'' + convert(varchar,[Linea])"));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Fecha", "Hora", "Minutos", "NumMarcado", "LugarOrigen", "LugarLlamado", "TipoLLamada", "Importe" });
                ldt.Columns["NumMarcado"].ColumnName = "Numero Marcado";
                ldt.Columns["LugarOrigen"].ColumnName = "Lugar Origen";
                ldt.Columns["LugarLlamado"].ColumnName = "Lugar Llamado";
                ldt.Columns["TipoLLamada"].ColumnName = "Tipo de LLamada";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepDetalleLlamsTelcelF4_T", ldt, false, "Totales",
                    new string[] { "", "", "{0:0,0}", "", "", "", "", "{0:c}", "" }), "RepTabDetalleLlamsF4Telcel1Pnl_T", tituloGrid));

            #endregion Grid
        }

        private void ReporteMinutosUtilizados(Control contenedorGrid)
        {
            #region Reporte Minutos Utilizados

            lblHeaderAltReporte.CssClass = "tdLblFechasNoWidth";
            lblCantMaxReg.Text = "Cantidad máxima de registros:";
            if (!Page.IsPostBack)
            {
                txtCantMaxReg.Text = "10";
            }

            DataTable RepMinsUtilizados = DSODataAccess.Execute(ConsultaRepMinutosUtilizados());
            if (RepMinsUtilizados.Rows.Count > 0)
            {
                DataView dvRepMinsUtilizados = new DataView(RepMinsUtilizados);
                RepMinsUtilizados = dvRepMinsUtilizados.ToTable(false,
                    new string[] { "Linea", "MinsDisp", });
                //new string[] { "Linea", "MinsDisp", "MinsUtil" });
                RepMinsUtilizados.Columns["Linea"].ColumnName = "Línea";
                RepMinsUtilizados.Columns["MinsDisp"].ColumnName = "Minutos Disponibles";
                //RepMinsUtilizados.Columns["MinsUtil"].ColumnName = "Minutos Utilizados";
                RepMinsUtilizados.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMinsUtilizados_t", RepMinsUtilizados,
                                false, "", new string[] { "", "" }), "ReporteMinutosUtilizados_T", "Minutos Disponibles")
                );

            #endregion Reporte Minutos Utilizados
        }

        private void RepHistMinutosUtilizados(Control contenedorGrid)
        {

            #region Reporte Historico de Minutos Utilizados

            //NZ 20170512 Se solicito en formato matricial de 3 años y que se grafiquen todos los años. (Solo graficaba un año) ademas de que se
            //respete la parte de la navegacion que ya se tiene.
            DataTable RepHistMinsUtilizados = DSODataAccess.Execute(ConsultaRepHistoricoMinsUtilizados("" + Request.Path + "?Nav="));
            RemoveColHerencia(ref RepHistMinsUtilizados);

            if (RepHistMinsUtilizados.Rows.Count > 0)
            {
                DataView dvRepMinsUtilizados = new DataView(RepHistMinsUtilizados);
                RepHistMinsUtilizados = dvRepMinsUtilizados.ToTable(false,
                    new string[] { "ClaveMes", "Mes", (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 2).ToString(), (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1).ToString(),
                                                       Convert.ToDateTime(Session["FechaInicio"].ToString()).Year.ToString() });
                RepHistMinsUtilizados.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                    DTIChartsAndControls.GridView("RepMinsUtilizados", RepHistMinsUtilizados, true, "Total",
                                 new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }, "", new string[] { }, 1,
                                new int[] { 0 }, new int[] { 1, 2, 3, 4 }, new int[] { }), "RepHistMinutosUtilizados", "Minutos Utilizados", "", 2, FCGpoGraf.Matricial)
                    );

            #endregion Reporte Historico de Minutos Utilizados

            #region Grafica Reporte Historico Minutos Utilizados
            //NZ 20170512 Se solicito en formato matricial de 3 años

            string[] lsaDataSource = null;
            DataTable dtAnios = new DataTable();
            if (RepHistMinsUtilizados.Rows.Count > 0)
            {
                if (RepHistMinsUtilizados.Rows[RepHistMinsUtilizados.Rows.Count - 1]["Mes"].ToString() == "Total")
                {
                    RepHistMinsUtilizados.Rows[RepHistMinsUtilizados.Rows.Count - 1].Delete();
                }
                string formatoLinkCenCos = Request.Path + "?Nav=RepDetalleLlamsMinsUtil&MesAnio=[0]{0}";

                //NZ se agregan los 3 años que se estan graficando.
                dtAnios.Columns.Add("Año");
                dtAnios.Rows.Add(dtAnios.NewRow()["Año"] = (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 2));
                dtAnios.Rows.Add(dtAnios.NewRow()["Año"] = (Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1));
                dtAnios.Rows.Add(dtAnios.NewRow()["Año"] = Convert.ToDateTime(Session["FechaInicio"].ToString()).Year);

                lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArrayLink
                        (RepHistMinsUtilizados, 1, new int[] { 0 }, formatoLinkCenCos, new int[] { 0 }, dtAnios));

                if (RepHistMinsUtilizados.Columns.Contains("ClaveMes"))
                {
                    RepHistMinsUtilizados.Columns.Remove("ClaveMes");
                }
                lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                      FCAndControls.ConvertDataTabletoDataTableArray(RepHistMinsUtilizados));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(RepHistMinsUtilizados),
                "RepHistMinutosUtilizados", "", "", "Mes", "Importe", 2, FCGpoGraf.Matricial, ""), false);
            }
            else
            {
                lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                      FCAndControls.ConvertDataTabletoDataTableArray(RepHistMinsUtilizados));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistAnioAntVsActGraf",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(RepHistMinsUtilizados),
                "RepHistMinutosUtilizados", "", "", "Mes", "Importe", 2, FCGpoGraf.Matricial, ""), false);
            }



            #endregion Grafica Reporte Historico Minutos Utilizados
        }

        private void RepDetalleLlamsMinsUtil(Control contenedorGrid)
        {
            #region Reporte Detalle de Llamadas (Minutos Utilizados)


            DataTable RepDetalleMinsUtil = DSODataAccess.Execute(ConsultaDetalleLlamsMinsUtil());
            if (RepDetalleMinsUtil.Rows.Count > 0)
            {
                DataView dvRepMinsUtilizados = new DataView(RepDetalleMinsUtil);
                RepDetalleMinsUtil = dvRepMinsUtilizados.ToTable(false,
                    new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos",
                        "Dir Llamada Clave", "Punta A", "Punta B" });

                RepDetalleMinsUtil.Columns["Extension"].ColumnName = "Línea";
                RepDetalleMinsUtil.Columns["Nombre Completo"].ColumnName = "Colaborador";
                RepDetalleMinsUtil.Columns["Fecha Llamada"].ColumnName = "Fecha";
                RepDetalleMinsUtil.Columns["Hora Llamada"].ColumnName = "Hora";
                RepDetalleMinsUtil.Columns["Duracion Minutos"].ColumnName = "Minutos";
                RepDetalleMinsUtil.Columns["Dir Llamada Clave"].ColumnName = "Tipo";
                RepDetalleMinsUtil.Columns["Punta A"].ColumnName = "Localidad Origen";
                RepDetalleMinsUtil.Columns["Punta B"].ColumnName = "Localidad Destino";
                RepDetalleMinsUtil.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalleMinsUtil", RepDetalleMinsUtil,
                                true, "Total", new string[] { "", "", "", "", "", "{0:0,0}", "", "", "" }), "RepDetalleLlamsMinsUtil", "Minutos Disponibles")
                );


            #endregion Reporte Detalle de Llamadas (Minutos Utilizados)
        }

        #region Reportes para demo consumo de datos moviles

        private void DemoConsumoMatIntPorDiaPorConc()
        {
            try
            {
                string idContenedorGraf = "DemoConsumoMatIntPorDiaPorConc";

                DataTable dtRep = DSODataAccess.Execute(DemoConsultaObtieneConsumoAcumPorDiaPorConcepto());
                string[] series = FCAndControls.extraeNombreColumnas(dtRep);

                DataTable[] arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtRep);

                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);

                if (dtRep != null)
                {

                    Rep0.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(idContenedorGraf, "Consumo de datos por día por concepto", 3, FCGpoGraf.MatricialConStack));


                    Page.ClientScript.RegisterStartupScript(this.GetType(), idContenedorGraf,
                                     FCAndControls.GraficaMultiSeries(lsaDataSource, series, idContenedorGraf,
                                     "", "", "Mes", "MB", 3, FCGpoGraf.MatricialConStack, ""), false);


                    Rep2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepConsporConc", dtRep, true, "Totales",
                    new string[] { "", "", "", "", "", "", "", "", "", "" }, "",
                    new string[] { }, 0, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new int[] { }),
                    "DemoConsumoMatIntPorDiaPorConc_T", "Detalle consumo de datos por día por concepto")
                    );
                }



                #region Reporte 1

                //Se obtiene el DataSource del reporte
                DataTable ldt = DSODataAccess.Execute(DemoConsultaConsumoPorConeceptoUnaLinea());

                #region Grid

                if (ldt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB" });
                    ldt.Columns["MB"].ColumnName = "MB";
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "MB Desc");
                }

                Rep1.Controls.Add(
                   DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                   DTIChartsAndControls.GridView("RepDemoonsumoPorConeceptoUnaLinea", ldt, false, "",
                                   new string[] { "", "{0:0,0}" }),
                                   "RepDemoonsumoPorConeceptoUnaLinea_T", "Consumo de datos por concepto", "", 2, FCGpoGraf.Tabular)
                   );

                #endregion Grid

                #region Grafica

                if (ldt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB" });
                    if (ldt.Rows[ldt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                    {
                        ldt.Rows[ldt.Rows.Count - 1].Delete();
                    }
                    ldt.Columns["Concepto"].ColumnName = "label";
                    ldt.Columns["MB"].ColumnName = "value";
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "MB Desc");
                    ldt.AcceptChanges();
                }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDemoonsumoPorConeceptoUnaLineaGraf",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                    "RepDemoonsumoPorConeceptoUnaLinea_T", "", "", "Concepto", "MB", 4, FCGpoGraf.Tabular, ""), false);

                #endregion Grafica

                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void DemoConsumoPorConcepto2Pnls(Control contenedor, string tipoGrafDefault, string tituloGrafica,
                                                           string tituloGrid, int pestaniaActiva)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(DemoConsultaConsumoPorConecepto());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB" });
                ldt.Columns["MB"].ColumnName = "MB";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "MB Desc");
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("DemoConsumoPorConecepto2Pnls", ldt, false, "", new string[] { "", "{0:0,0}" }),
                               "DemoConsumoPorConcepto2Pnls_T", tituloGrid, "", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Concepto"].ColumnName = "label";
                ldt.Columns["MB"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "value Desc");
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDemoonsumoPorConeceptoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "DemoConsumoPorConcepto2Pnls_T", "", "", "Concepto", "MB", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica


        }

        private void RepTopLineasMasConsumoDatos(Control contenedorGrid, string tituloGrid, int pestaniaActiva, string linkGrafica)
        {
            DataTable ldt = DSODataAccess.Execute(TopLineasMasConsumoDatos(linkGrafica));

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Linea", "Tel", "NombreCompleto", "MB", "Link" });
                ldt.Columns["Tel"].ColumnName = "Línea";
                ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(
                                    "RepTopLineasMasConsumoDatos_T", DTIChartsAndControls.ordenaTabla(ldt, "MB desc"), false, "",
                                    new string[] { "", "", "", "{0:0,0}" }, Request.Path + "?Nav=DemoConsumoMatIntPorDiaPorConc&Linea={0}",
                                    new string[] { "Linea" }, 1, new int[] { 0 },
                                    new int[] { 2, 3 }, new int[] { 1 }), "RepTopLineasMasConsumoDatos_T", tituloGrid, "", pestaniaActiva, FCGpoGraf.TabularCoBaTa)
                    );

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Colaborador", "MB", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["MB"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTopLineasMasConsumoDatos__G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTopLineasMasConsumoDatos_T", "", "", "Colaborador", "MB", pestaniaActiva, FCGpoGraf.TabularCoBaTa, ""), false);
        }

        #endregion Reportes para demo consumo de datos moviles

        public void IndicadorLineasNoIdent()
        {
            ///Llama al indicador ue calcula la lineas que no han sido identificadas
            int indicadorLineasNoIdent = (int)DSODataAccess.ExecuteScalar(ConsultaLineasNoIdent());

            if (!(indicadorLineasNoIdent != null || indicadorLineasNoIdent >= 0))
            {
                indicadorLineasNoIdent = 0;
            }

            //Imprimir el indicador en uno de los indicadores Dash Moviles
        }

        public void RepTabConsumoLineasNoIdent2Pnls(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica)
        {
            string link = Request.Path + "?Nav=ConsumoPorConcepto1Linea1Pnl&Tel=''+CONVERT(varchar,[Linea])+''&Carrier=''+CONVERT(varchar,[Codigo Carrier])+''";
            DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasNoIdent(link));

            #region Elimina col innecesaria
            //Columnas original 
            /*
                Linea	
                Total	
                Renta	
                Diferencia	
                iCodCatCarrier	
                link	
                RID	
                RowNumber	
                TopRID
             */


            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }

            #endregion

            #region Reporte
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false,
                    new string[] { "Linea", "Total", "Renta", "Diferencia", "link", "iCodCatCarrier" });
                dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                dtRepOriginal.Columns["Total"].ColumnName = "Total";
                dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                dtRepOriginal.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                                                    "ConsumoLineasNoIdent2Pnls_T",
                                                    dtRepOriginal,
                                                    true,
                                                    "Totales",
                                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}", "", "" },
                                                   Request.Path + "?Nav=ConsumoPorConcepto1Linea1Pnl&Tel={0}&Carrier={1}",
                                                    new string[] { "Linea", "iCodcatCarrier" },
                                                    0,
                                                    new int[] { 4, 5 },
                                                    new int[] { 1, 2, 3 },
                                                    new int[] { 0 }
                    ), "contConsumoLineasNoIdent2Pnls_T", tituloGrid));

            #endregion

            #region Grafica
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false, new string[] { "Linea", "Total", "link" });
                if (dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1]["Linea"].ToString() == "Totales")
                {
                    dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1].Delete();
                }
                dtRepOriginal.Columns["Linea"].ColumnName = "label";
                dtRepOriginal.Columns["Total"].ColumnName = "value";
                dtRepOriginal.AcceptChanges();
            }

            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConLinNoIdent", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConLinNoIdent",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10)), "RepTabConLinNoIdent", "", "", "Linea", "Total", 1, FCGpoGraf.Tabular), false);
            #endregion
        }

        public void RepTabConsumoPorConcepto1Linea1Pnl(Control contenedorGrid, string tituloGrid, string tel, string carrier)
        {

            DataTable dtRepOriginal = new DataTable();
            dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1Linea1Pnl(tel, carrier));

            /*
                * Concepto	
                * idConcepto
                * Detalle
                * Total	
                * Carrier
                * DescCarrier
                * RID	
                * RowNumber	
                * TopRID
            */

            #region Elimina Columnas innecesarias
            if (dtRepOriginal.Columns.Contains("Detalle"))
            {
                dtRepOriginal.Columns.Remove("Detalle");
            }

            if (dtRepOriginal.Columns.Contains("idConcepto"))
            {
                dtRepOriginal.Columns.Remove("idConcepto");
            }

            if (dtRepOriginal.Columns.Contains("Carrier"))
            {
                dtRepOriginal.Columns.Remove("Carrier");
            }

            if (dtRepOriginal.Columns.Contains("DescCarrier"))
            {
                dtRepOriginal.Columns.Remove("DescCarrier");
            }

            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }
            #endregion


            #region reporte

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabConsumoPorConcepto1Linea1Pnl_T", dtRepOriginal, true, "Totales"),
                    "RepTabConsumoPorConcepto1Linea1Pnl_T",
                    tituloGrid,
                    ""
                    )
                );

            #endregion

        }

        public void IndicadorCantLineasNuevas()
        {
            int indicadorLineasNuevas = (int)DSODataAccess.ExecuteScalar(ConsultaCantLineasNuevas());
            if (!(indicadorLineasNuevas != null || indicadorLineasNuevas >= 0))
            {
                indicadorLineasNuevas = 0;
            }

        }

        public void RepTabConsumoCantLineasNuevas2Pnls(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica)
        {
            string link = Request.Path + "?Nav=RepTabConsumoPorConcepto1LineaNueva1Pnl&Tel=''+CONVERT(varchar,[Linea])+''&Carrier=''+CONVERT(varchar,[Codigo Carrier])+''";
            DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoCantLineasNuevas(link));

            #region Elimina col innecesaria
            //Columnas original 
            /*
                Linea	
                Total	
                Renta	
                Diferencia	
                iCodCatCarrier	
                link	
                RID	
                RowNumber	
                TopRID
             */


            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }

            #endregion

            #region Reporte
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false,
                    new string[] { "Linea", "Total", "Renta", "Diferencia", "link", "iCodCatCarrier" });
                dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                dtRepOriginal.Columns["Total"].ColumnName = "Total";
                dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                dtRepOriginal.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                                                    "ConsumoLineasNoIdent2Pnls_T",
                                                    dtRepOriginal,
                                                    true,
                                                    "Totales",
                                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}", "", "" },
                                                    Request.Path + "?Nav=RepTabConsumoPorConcepto1LineaNueva1Pnl&Tel={0}&Carrier={1}",
                                                    new string[] { "Linea", "iCodcatCarrier" },
                                                    0,
                                                    new int[] { 4, 5 },
                                                    new int[] { 1, 2, 3 },
                                                    new int[] { 0 }
                    ), "contConsumoLineasNoIdent2Pnls_T", tituloGrid));

            #endregion

            #region Grafica
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false, new string[] { "Linea", "Total", "link" });
                if (dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1]["Linea"].ToString() == "Totales")
                {
                    dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1].Delete();
                }
                dtRepOriginal.Columns["Linea"].ColumnName = "label";
                dtRepOriginal.Columns["Total"].ColumnName = "value";
                dtRepOriginal.AcceptChanges();
            }

            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConLinNoIdent", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConLinNoIdent",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10)), "RepTabConLinNoIdent", "", "", "Linea", "Total", 1, FCGpoGraf.Tabular), false);
            #endregion
        }

        public void RepTabConsumoPorConcepto1LineaNueva1Pnl(Control contenedorGrid, string tituloGrid, string tel, string carrier)
        {

            DataTable dtRepOriginal = new DataTable();
            dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1LineaNueva1Pnl(tel, carrier));

            /*
                * Concepto	
                * idConcepto
                * Detalle
                * Total	
                * Carrier
                * DescCarrier
                * RID	
                * RowNumber	
                * TopRID
            */

            #region Elimina Columnas innecesarias
            if (dtRepOriginal.Columns.Contains("Detalle"))
            {
                dtRepOriginal.Columns.Remove("Detalle");
            }

            if (dtRepOriginal.Columns.Contains("idConcepto"))
            {
                dtRepOriginal.Columns.Remove("idConcepto");
            }

            if (dtRepOriginal.Columns.Contains("Carrier"))
            {
                dtRepOriginal.Columns.Remove("Carrier");
            }

            if (dtRepOriginal.Columns.Contains("DescCarrier"))
            {
                dtRepOriginal.Columns.Remove("DescCarrier");
            }

            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }
            #endregion


            #region reporte

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabConsumoPorConcepto1Linea1Pnl_T", dtRepOriginal, true, "Totales"),
                    "RepTabConsumoPorConcepto1Linea1Pnl_T",
                    tituloGrid,
                    ""
                    )
                );

            #endregion

        }

        public void IndicadorCantLineasSinActividad()
        {
            int indicadorLineasSinAct = (int)DSODataAccess.ExecuteScalar(ConsultaLineasSinAc());
            if (!(indicadorLineasSinAct != null || indicadorLineasSinAct >= 0))
            {
                indicadorLineasSinAct = 0;
            }

        }

        public void RepTabConsumoLineasSinAct2Pnls(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica)
        {

            DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasSinAct());

            #region Elimina col innecesaria
            //Columnas original 
            /*
                Linea	
                Total	
                Renta	
                Diferencia	
                iCodCatCarrier	
                link	
                RID	
                RowNumber	
                TopRID
             */


            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }

            #endregion

            #region Reporte
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false,
                    new string[] { "Linea", "Total", "Renta", "Diferencia", "iCodCatCarrier" });
                dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                dtRepOriginal.Columns["Total"].ColumnName = "Total";
                dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                dtRepOriginal.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                                                    "ConsumoLineasSinAct2Pnls_T",
                                                    dtRepOriginal,
                                                    true,
                                                    "Totales",
                                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}", "" },
                                                    "",/*Request.Path + "?Nav=RepTabConsumoPorConcepto1LineaNueva1Pnl&Tel={0}&Carrier={1}",*/
                                                    new string[] { "Linea", "iCodcatCarrier" },
                                                    0,
                                                    new int[] { 4 },
                                                    new int[] { 0, 1, 2, 3 },
                                                    new int[] { }
                    ), "ConsumoLineasSinAct2Pnls_T", tituloGrid));

            #endregion

            #region Grafica
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false, new string[] { "Linea", "Total" });
                if (dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1]["Linea"].ToString() == "Totales")
                {
                    dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1].Delete();
                }
                dtRepOriginal.Columns["Linea"].ColumnName = "label";
                dtRepOriginal.Columns["Total"].ColumnName = "value";
                dtRepOriginal.AcceptChanges();
            }

            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTaLineasSinAct", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTaLineasSinAct",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10)), "RepTaLineasSinAct", "", "", "Linea", "Total", 1, FCGpoGraf.Tabular), false);
            #endregion
        }

        //RM 20180828 Indicador cant Lineas en el Mes Anterior
        public void IndicadorCantLineasEnElMes()
        {
            int indicadorCantLineasEnElMes = (int)DSODataAccess.ExecuteScalar(ConsultaCantidadLineasEnelMes());
            if (!(indicadorCantLineasEnElMes != null || indicadorCantLineasEnElMes >= 0))
            {
                indicadorCantLineasEnElMes = 0;
            }


            //Llenar Indicador en el dasboar de moviles 

        }

        public void RepTabConsumoLineasEnElMes2Pnls(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica)
        {
            string link = Request.Path + "?Nav=ConsumoPorConcepto1Linea1PnlMesAnterior&Tel=''+CONVERT(varchar,[Linea])+''&Carrier=''+CONVERT(varchar,[Codigo Carrier])+''";
            DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasEnElMes(link));

            #region Elimina col innecesaria
            //Columnas original 
            /*
                Linea	
                Total	
                Renta	
                Diferencia	
                iCodCatCarrier	
                link	
                RID	
                RowNumber	
                TopRID
             */


            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }

            #endregion


            #region Reporte
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false,
                    new string[] { "Linea", "Total", "Renta", "Diferencia", "link", "iCodCatCarrier" });
                dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                dtRepOriginal.Columns["Total"].ColumnName = "Total";
                dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                dtRepOriginal.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                                                    "ConsumoLineasNoIdent2Pnls_T",
                                                    dtRepOriginal,
                                                    true,
                                                    "Totales",
                                                    new string[] { "", "{0:c}", "{0:c}", "{0:c}", "", "" },
                                                    Request.Path + "?Nav=ConsumoPorConcepto1Linea1PnlMesAnterior&Tel={0}&Carrier={1}",
                                                    new string[] { "Linea", "iCodcatCarrier" },
                                                    0,
                                                    new int[] { 4, 5 },
                                                    new int[] { 1, 2, 3 },
                                                    new int[] { 0 }
                    ), "contConsumoLineasNoIdent2Pnls_T", tituloGrid));

            #endregion


            #region Grafica
            if (dtRepOriginal.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtRepOriginal);
                dtRepOriginal = dvldt.ToTable(false, new string[] { "Linea", "Total", "link" });
                if (dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1]["Linea"].ToString() == "Totales")
                {
                    dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1].Delete();
                }
                dtRepOriginal.Columns["Linea"].ColumnName = "label";
                dtRepOriginal.Columns["Total"].ColumnName = "value";
                dtRepOriginal.AcceptChanges();
            }


            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConLinNoIdent", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConLinNoIdent",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10)), "RepTabConLinNoIdent", "", "", "Linea", "Total", 1, FCGpoGraf.Tabular), false);
            #endregion
        }

        public void RepTabConsumoPorConcepto1Linea1PnlMesAnterior(Control contenedorGrid, string tituloGrid, string tel, string carrier)
        {

            DataTable dtRepOriginal = new DataTable();
            dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1Linea1PnlMesAnterior(tel, carrier));

            /*
                * Concepto	
                * idConcepto
                * Detalle
                * Total	
                * Carrier
                * DescCarrier
                * RID	
                * RowNumber	
                * TopRID
            */

            #region Elimina Columnas innecesarias
            if (dtRepOriginal.Columns.Contains("Detalle"))
            {
                dtRepOriginal.Columns.Remove("Detalle");
            }

            if (dtRepOriginal.Columns.Contains("idConcepto"))
            {
                dtRepOriginal.Columns.Remove("idConcepto");
            }

            if (dtRepOriginal.Columns.Contains("Carrier"))
            {
                dtRepOriginal.Columns.Remove("Carrier");
            }

            if (dtRepOriginal.Columns.Contains("DescCarrier"))
            {
                dtRepOriginal.Columns.Remove("DescCarrier");
            }

            if (dtRepOriginal.Columns.Contains("RID"))
            {
                dtRepOriginal.Columns.Remove("RID");
            }

            if (dtRepOriginal.Columns.Contains("RowNumber"))
            {
                dtRepOriginal.Columns.Remove("RowNumber");
            }

            if (dtRepOriginal.Columns.Contains("TopRID"))
            {
                dtRepOriginal.Columns.Remove("TopRID");
            }
            #endregion


            #region reporte

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabConsumoPorConcepto1Linea1Pnl_T", dtRepOriginal, true, "Totales"),
                    "RepTabConsumoPorConcepto1Linea1Pnl_T",
                    tituloGrid,
                    ""
                    )
                );

            #endregion

        }

        #region Reporte DashMov Consumo por centro de costos
        public void RepTabConsumoPorCentroDeCostos1Pnl(Control contenedor, string tituloGrid, int pestañaActiva)
        {
            string link2Paneles = Request.Path + "?Nav=ConsumoPorCentroDeCostos2Pnl";
            string linkGrid = Request.Path + "?Nav=ConsumoPorCentroDeCostosN2&CenCos={0}";
            string linkGraf = "[link] = ''" + Request.Path + "?Nav=CarrierCCN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])";
            DataTable dtReporte = new DataTable();
            //dtReporte = DSODataAccess.Execute(ConsultaRepTabConsumoPorCentroDeCostos1Pnl(link));
            dtReporte = BuscadtPruebaConsumoPorCentroDeCostos(linkGraf);



            #region Reporte 1Pnl

            #region Grid

            if (dtReporte.Rows.Count > 0)
            {
                if (dtReporte.Columns.Contains("cenCosDesc"))
                {
                    dtReporte.Columns["cenCosDesc"].ColumnName = "Centro de Costos";
                }

                if (dtReporte.Columns.Contains("total"))
                {
                    dtReporte.Columns["total"].ColumnName = "Total";
                }

                if (dtReporte.Columns.Contains("renta"))
                {
                    dtReporte.Columns["renta"].ColumnName = "Renta";
                }

                if (dtReporte.Columns.Contains("montoExcedido"))
                {
                    dtReporte.Columns["montoExcedido"].ColumnName = "Monto Excedido";
                }
            }


            contenedor.Controls.Add(
                  DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                  DTIChartsAndControls.GridView("RepTabConsumoCenCos", DTIChartsAndControls.ordenaTabla(dtReporte, "Total desc"), true, "Totales",
                                  new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid,
                                  new string[] { "iCodCatCenCos" }, 1, new int[] { 0, 2, 6 },
                                  new int[] { 3, 4, 5 }, new int[] { 1 }), "RepTabConsumoPorCentroDeCostos1Pnl", tituloGrid, link2Paneles, pestañaActiva, FCGpoGraf.Tabular)
                  );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Centro de Costos", "Total", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Centro de Costos"].ColumnName = "label";
                dtReporte.Columns["Total"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCentroDeCostos1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)), "RepTabConsumoPorCentroDeCostos1Pnl", "", "", "Centro de Costos", "Importe", pestañaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

            #endregion

        }

        public void RepTabConsumoPorCentroDeCostos2Pnl(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGraf, int pestañaActiva)
        {
            string linkGrid = Request.Path + "?Nav=ConsumoPorCentroDeCostosN2&CenCos={0}";
            string linkGraf = "[link] = ''" + Request.Path + "?Nav=CarrierCCN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])";


            DataTable dtReporte = new DataTable();
            //dtReporte = DSODataAccess.Execute(ConsultaRepTabConsumoPorCentroDeCostos1Pnl());
            dtReporte = BuscadtPruebaConsumoPorCentroDeCostos(linkGraf);



            #region Reporte 1Pnl

            #region Grid

            if (dtReporte.Rows.Count > 0)
            {
                if (dtReporte.Columns.Contains("cenCosDesc"))
                {
                    dtReporte.Columns["cenCosDesc"].ColumnName = "Centro de Costos";
                }

                if (dtReporte.Columns.Contains("total"))
                {
                    dtReporte.Columns["total"].ColumnName = "Total";
                }

                if (dtReporte.Columns.Contains("renta"))
                {
                    dtReporte.Columns["renta"].ColumnName = "Renta";
                }

                if (dtReporte.Columns.Contains("montoExcedido"))
                {
                    dtReporte.Columns["montoExcedido"].ColumnName = "Monto Excedido";
                }
            }

            contenedorGrid.Controls.Add(
                  DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                  DTIChartsAndControls.GridView("RepTabConsumoCenCos_T", DTIChartsAndControls.ordenaTabla(dtReporte, "Total desc"), true, "Totales",
                                  new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "" }, linkGrid,
                                  new string[] { "iCodCatCenCos" }, 1, new int[] { 0, 2, 6 },
                                  new int[] { 3, 4, 5 }, new int[] { 1 }), "RepTabConsumoPorCentroDeCostos2Pnl_T", tituloGrid)

                  );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Centro de Costos", "Total", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Centro de Costos"].ColumnName = "label";
                dtReporte.Columns["Total"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConsumoPorCentroDeCostos2Pnl_G", tituloGraf, pestañaActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCentroDeCostos2Pnl_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)), "RepTabConsumoPorCentroDeCostos2Pnl_G", "", "", "Centro de Costos", "Importe", pestañaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

            #endregion

        }

        public DataTable BuscadtPruebaConsumoPorCentroDeCostos(string link)
        {
            DataTable dtReporte = new DataTable();

            if (dtReporte.Columns.Count == 0)
            {
                dtReporte.Columns.Add("iCodCatCenCos", typeof(int));
                dtReporte.Columns.Add("cenCosDesc", typeof(string));
                dtReporte.Columns.Add("cenCosNivel", typeof(int));
                dtReporte.Columns.Add("total", typeof(float));
                dtReporte.Columns.Add("renta", typeof(float));
                dtReporte.Columns.Add("montoExcedido", typeof(float));
                dtReporte.Columns.Add("link", typeof(string));
            }


            for (int i = 0; i < 1000; i++)
            {
                DataRow row = dtReporte.NewRow();

                row["iCodCatCenCos"] = 99999999;
                row["cenCosdesc"] = "ccPrueba";
                row["cenCosNivel"] = 1;
                row["total"] = 99.99;
                row["renta"] = 99.99;
                row["montoExcedido"] = 99.99;
                row["link"] = link;

                dtReporte.Rows.Add(row);
            }

            return dtReporte;

        }
        #endregion

        public void RepConsumoDatosPorDiaConcepto(Control contenedorGrid, string tituloGrid)
        {
            DataTable dtRep = DSODataAccess.Execute(ConsultaConsumoDatosporDiaConcepto());
            string[] series = FCAndControls.extraeNombreColumnas(dtRep);

            DataTable[] arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtRep);

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);

            if (dtRep != null)
            {

                Rep0.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsumoDatosConcepto", "Consumo de datos por día por concepto", 3, FCGpoGraf.MatricialConStack));


                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoDatosConcepto",
                                 FCAndControls.GraficaMultiSeries(lsaDataSource, series, "RepConsumoDatosConcepto",
                                 "", "", "Mes", "MB", 3, FCGpoGraf.MatricialConStack, ""), false);


                Rep2.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                DTIChartsAndControls.GridView("RepConsumoDatosConcepto", dtRep, true, "Totales",
                new string[] { "", "", "", "", "", "", "", "", "", "" }, "",
                new string[] { }, 0, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new int[] { }),
                "DemoConsumoMatIntPorDiaPorConc_T", "Detalle consumo de datos por día por concepto")
                );
            }

        }

        public void RepConsumoDatosNacional1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, string link)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoDatosNacional(link));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "ClaveConcepto", "Concepto", "MB" });
                ldt.Columns["MB"].ColumnName = "MB";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "MB Desc");
            }

            contenedor.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("ConsumoPorConecepto1Pnls", ldt, false, "",
                               new string[] { "", "{0:0,0}" }, Request.Path + "?Nav=RepConsumoDatosNacionalN2&Concepto={0}",
                               new string[] { "ClaveConcepto" }, 1,
                               new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                               "ConsumoPorConcepto1Pnls_T", tituloGrid, "?Nav=RepConsumoDatosNacional2Pnl", pestaniaActiva, FCGpoGraf.Tabular)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Concepto"].ColumnName = "label";
                ldt.Columns["MB"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "value Desc");
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoPorConeceptoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "ConsumoPorConcepto1Pnls_T", "", "", "Concepto", "MB", pestaniaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        public void RepConsumoDatosNacional2Pnl(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica, string link)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoDatosNacional(link));

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "ClaveConcepto", "Concepto", "MB" });
                ldt.Columns["MB"].ColumnName = "MB";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "MB Desc");
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("ConsumoPorConecepto2Pnls", ldt, false, "",
                               new string[] { "", "{0:0,0}" }, Request.Path + "?Nav=RepConsumoDatosNacionalN2&Concepto={0}",
                               new string[] { "ClaveConcepto" }, 1,
                               new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                               "ConsumoPorConcepto2Pnls_T", tituloGrid, "", 2, FCGpoGraf.Tabular)
               );

            #endregion Grid
            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Concepto", "MB", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Concepto"].ColumnName = "label";
                ldt.Columns["MB"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "value Desc");
                ldt.AcceptChanges();
            }

            contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
            "ConsumoPorConcepto1Pnls_G", tituloGrafica, 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoPorConeceptoGraf",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "ConsumoPorConcepto1Pnls_G", "", "", "Concepto", "MB", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        public void RepMatConsumoAcumHistPorDireccion(Control contenedor, string tituloGrid, int pestaniaActiva, int año)
        {
            //NZ: Año Seleccionado.
            DataTable ldt = DSODataAccess.Execute(GetConsumoHistAcumPorDireccionUnAño(año));

            #region Grid

            List<string> formatosCol = new List<string>();
            List<int> colVisibles = new List<int>();
            if (ldt.Rows.Count > 0)
            {
                formatosCol.Add("");
                formatosCol.Add("");
                colVisibles.Add(1);
                for (int i = 3; i <= ldt.Columns.Count; i++)
                {
                    formatosCol.Add("{0:c}");
                    colVisibles.Add(i - 1);
                }
            }

            Rep0.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepAcumHisAñoSelect" + año.ToString(), ldt, true, "Totales",
                               formatosCol.ToArray(), string.Empty, new string[] { }, 1, new int[] { 0 }, colVisibles.ToArray(), new int[] { }),
                               "RepAcumHisAnioSelect" + año.ToString() + "_T", tituloGrid, "", pestaniaActiva, FCGpoGraf.TabularCiBaTa)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Dirección", "TOTAL" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Dirección"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Dirección"].ColumnName = "label";
                ldt.Columns["TOTAL"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "value Desc");
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepAcumHisAnioSelect" + año.ToString() + "_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "RepAcumHisAnioSelect" + año.ToString() + "_T", "", "", "Dirección", "Importe", pestaniaActiva, FCGpoGraf.TabularCiBaTa, ""), false);

            #endregion Grafica
        }

        //NZ: Reporte de minutos.       
        public void RepUsoDeMinEnLineas(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(GetMinLineas());

                if (dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    RemoveColHerencia(ref dt);

                    if (dt.Columns.Contains("Nombre Carrier"))
                        dt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                    if (dt.Columns.Contains("Nombre Completo"))
                        dt.Columns["Nombre Completo"].ColumnName = "Responsable";
                    if (dt.Columns.Contains("Nombre Centro de Costos"))
                        dt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                }

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoMin", dt, true, "Totales", new string[] { "", "", "", "", "{0:0,0}" }),
                                "RepTabConsumoMin_T", tituloGrid, ""));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en Dashboard de Moviles:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void RepPorConceptoEmple(Control contenedor, string tituloGrid)
        {
            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());

            //20150331 NZ Se remueve el concepto de Rentas de este reporte.
            //RepDetalleXConcepto = CostoXConceptoRemoverRentas(RepDetalleXConcepto);

            if (RepDetalleXConcepto.Rows.Count > 0)
            {

                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);

                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                    new string[] { "Carrier", "idConcepto", "DescCarrier", "Concepto", "Total" });

                //RM 20180220 Cambios Alfa
                RepDetalleXConcepto.Columns["Carrier"].ColumnName = "idCarrier";
                RepDetalleXConcepto.Columns["DescCarrier"].ColumnName = "Carrier";
                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";

                RepDetalleXConcepto.AcceptChanges();
            }

            //20150331 NZ Se cambio el panel en el cual se muestra.
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalleXConcepto_T", RepDetalleXConcepto, true, "Totales",
                                new string[] { "", "", "", "", "{0:c}" }, Request.Path + "?Nav=DetFacPEmN2&Concepto={0}&Carrier={1}",
                                new string[] { "idConcepto", "idCarrier" }, 2, new int[] { 0, 1 },
                                new int[] { 2, 4 }, new int[] { 3 }), "RepDetalleXConcepto1_T", tituloGrid, Request.Path + "?Nav=DetFacPEmN1")
                );

        }

        public void RepMatConsumoAcumHistPorSubdireccion(Control contenedor, string tituloGrid, int pestaniaActiva, int año)
        {
            //NZ: Año Seleccionado.
            DataTable ldt = DSODataAccess.Execute(ConsultaRepMatConsumoAcumHistPorSubdireccion(año));

            #region Grid

            List<string> formatosCol = new List<string>();
            List<int> colVisibles = new List<int>();
            if (ldt.Rows.Count > 0)
            {
                formatosCol.Add("{0:c}");
                formatosCol.Add("");
                formatosCol.Add("{0:c}");
                formatosCol.Add("");
                colVisibles.Add(1);
                colVisibles.Add(3);
                for (int i = 4; i <= ldt.Columns.Count; i++)
                {
                    formatosCol.Add("{0:c}");
                    colVisibles.Add(i - 1);
                }
            }

            Rep0.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepAcumHisAñoSelect" + año.ToString(), ldt, true, "Totales",
                               formatosCol.ToArray(), string.Empty, new string[] { }, 3, new int[] { 0, 2 }, colVisibles.ToArray(), new int[] { }),
                               "RepAcumHisAnioSelect" + año.ToString() + "_T", tituloGrid, "", pestaniaActiva, FCGpoGraf.TabularCiBaTa)
               );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SubDireccion", "TOTAL" });
                if (ldt.Rows[ldt.Rows.Count - 1]["SubDireccion"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Subdireccion"].ColumnName = "label";
                ldt.Columns["TOTAL"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "value Desc");
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepAcumHisAnioSelect" + año.ToString() + "_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "RepAcumHisAnioSelect" + año.ToString() + "_T", "", "", "Dirección", "Importe", pestaniaActiva, FCGpoGraf.TabularCiBaTa, ""), false);

            #endregion Grafica
        }


        public void RepUsoDeMinEnLineas3Meses(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(GetMinLineas3Meses());

                if (dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    RemoveColHerencia(ref dt);

                    if (dt.Columns.Contains("Nombre Carrier"))
                        dt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                    if (dt.Columns.Contains("Nombre Completo"))
                        dt.Columns["Nombre Completo"].ColumnName = "Responsable";
                    if (dt.Columns.Contains("Nombre Centro de Costos"))
                        dt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                }

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoMin", dt, true, "Totales", new string[] { "", "", "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }),
                                "RepTabConsumoMin_T", tituloGrid, ""));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en Dashboard de Moviles:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }


        public void ConsumoDatosLineas1Mes(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsumoDatosLineas1Mes());

                if (dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    RemoveColHerencia(ref dt);


                    if (dt.Columns.Contains("FechaPub"))
                    {
                        dt.Columns.Remove("FechaPub");
                    }


                    if (dt.Columns.Contains("Nombre Carrier"))
                        dt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                    if (dt.Columns.Contains("Empleado"))
                        dt.Columns["Empleado"].ColumnName = "Responsable";
                    if (dt.Columns.Contains("CentroCostos"))
                        dt.Columns["CentroCostos"].ColumnName = "Área";
                }

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsumoDatosLineas1Mes", dt, true, "Totales", new string[] { "", "", "", "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "{0:c}", "{0:c}" }),
                                "RepTabConsumoDatosLineas1Mes_T", tituloGrid, ""));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en Dashboard de Moviles:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void LlamadasMasCostosas(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaLLamadasMasCostosas());

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos", "Costo", "Dir Llamada", "Punta A", "Punta B" });
                    dt.Columns["Extension"].ColumnName = "Extensión";
                    dt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                    dt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                    dt.Columns["Fecha Llamada"].ColumnName = "Fecha";
                    dt.Columns["Hora Llamada"].ColumnName = "Hora";
                    dt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                    dt.Columns["Costo"].ColumnName = "Total";
                    dt.Columns["Dir Llamada"].ColumnName = "Tipo";
                    dt.Columns["Punta A"].ColumnName = "Localidad Origen";
                    dt.Columns["Punta B"].ColumnName = "Localidad Destino";
                    dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();
                }

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabLlamMasCostosas", dt, true, "Totales", new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "" }),
                                    "RepTabLlamMasCostosas_T", tituloGrid, ""));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error al Mostrar el Reporte Llamadas mas Costosas:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }
        public void LlamMasLargas(Control contenedor, string tituloGrid)
        {

            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaLlamMasLargas());
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos", "Costo", "Dir Llamada", "Punta A", "Punta B" });
                    dt.Columns["Extension"].ColumnName = "Extensión";
                    dt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                    dt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                    dt.Columns["Fecha Llamada"].ColumnName = "Fecha";
                    dt.Columns["Hora Llamada"].ColumnName = "Hora";
                    dt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                    dt.Columns["Costo"].ColumnName = "Total";
                    dt.Columns["Dir Llamada"].ColumnName = "Tipo";
                    dt.Columns["Punta A"].ColumnName = "Localidad Origen";
                    dt.Columns["Punta B"].ColumnName = "Localidad Destino";
                    dt = DTIChartsAndControls.ordenaTabla(dt, "Minutos Desc");
                    dt.AcceptChanges();
                }

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamMasLargas", dt, true, "Totales", new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "" }),
                                "RepTabLlamMasLargas_T", tituloGrid, ""));
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error al Mostrar el Reporte Llamadas mas Largas:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

        }
        public void LineasNuevasTelcel(Control contenedorGrid, string tituloGrid, Control contenedorGrafica, string tituloGrafica, string link)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaLineasNuevasTelcel());
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Total", "Duracion", "Numero" });
                    dt.Columns["Extension"].ColumnName = "Línea";
                    dt.Columns["Nombre Completo"].ColumnName = "Responsable";
                    dt.Columns["Duracion"].ColumnName = "Minutos";
                    dt.Columns["Numero"].ColumnName = "Llamadas";
                    dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();
                }

                contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepTabLineasNuevasGrid_T", dt, true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                    Request.Path + "?Nav=PorConceptoN2&Linea={1}&NumMarc={0}",
                                    new string[] { /*"Línea", "Línea"*/ }, 0,
                                    new int[] { }, new int[] { 0, 1, 2 }, new int[] { /*0*/ }),
                                    "RepTabLineasNuevasGrid_T", tituloGrid)
                                    );


                #region Grafica

                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "Línea", "Total"/*, "link"*/ });
                    if (dt.Rows[dt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }
                    dt.Columns["Línea"].ColumnName = "label";
                    dt.Columns["Total"].ColumnName = "value";
                    dt.AcceptChanges();
                }

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLineasNuevasGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLineasNuevasGraf_G",
                    FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10)),
                    "RepTabLineasNuevasGraf_G", "", "", "Línea", "Total", 2, FCGpoGraf.Tabular), false);

                #endregion Grafica
            }
            catch
            {

            }
        }
        public void LineasTelcelDadasDeBaja(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaLineasTelcelBajas());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Nombre Centro de Costos" });
                dt.Columns["Extension"].ColumnName = "Línea";
                dt.Columns["Nombre Completo"].ColumnName = "Responsable";
                dt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costo";
                dt = DTIChartsAndControls.ordenaTabla(dt, "Responsable Desc");
                dt.AcceptChanges();
            }
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabLineasTelcelBajas", dt, false, "", new string[] { "", "", "" }),
                    "RepTabLineasTelcelBajas_T", tituloGrid, ""));
        }
        public void LineasConGastoMensaje(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaLineasConGastoMensaje());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Nombre Centro de Costos", "CantMens", "Total" });
                dt.Columns["Extension"].ColumnName = "Línea";
                dt.Columns["Nombre Completo"].ColumnName = "Responsable";
                dt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costo";
                dt.Columns["CantMens"].ColumnName = "Cantidad de Mensajes";

                dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                dt.AcceptChanges();
            }
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabLineasGastoMensaje", dt, true, "Totales", new string[] { "", "", "", "{0:0,0}", "{0:c}" }),
                    "RepTabLineasGastoMensaje_T", tituloGrid, ""));
        }
        public void DetallaFactTelcel(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaDetalladoFacTelcel());
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Remove("Otros Servicios");
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Linea", "PlanLinea", "Min Libres Pico", "Min Facturables Pico", "Min Libres No Pico", "Min Facturables No Pico", "Tiempo Aire Nacional"
                ,"Tiempo Aire Roaming Nac","Tiempo Aire Roaming Int","Larga Distancia Nac","Larga Distancia Roam Nac","Larga Distancia Roam Int","Servicios Adicionales","Desc Tiempo Aire","Desc Tiempo Aire Roam",
                "Otros Desc","Cargos Creditos","Otros Serv","Roaming GPRS Internacional","Costo"});
                dt.Columns["Linea"].ColumnName = "Número";
                dt.Columns["PlanLinea"].ColumnName = "Plan";
                dt.Columns["Min Libres Pico"].ColumnName = "Minutos Libres Pico";
                dt.Columns["Min Libres No Pico"].ColumnName = "Minutos Libres No Pico";
                dt.Columns["Min Facturables Pico"].ColumnName = "Minutos Facturables Pico";
                dt.Columns["Min Facturables No Pico"].ColumnName = "Minutos Facturables No Pico";
                dt.Columns["Tiempo Aire Nacional"].ColumnName = "Tiempo Aire Nacional";
                dt.Columns["Tiempo Aire Roaming Nac"].ColumnName = "Tiempo Aire Roaming Nacional";
                dt.Columns["Tiempo Aire Roaming Int"].ColumnName = "Tiempo Aire Roaming Internacional";
                dt.Columns["Larga Distancia Nac"].ColumnName = "Larga Distancia Nacional";
                dt.Columns["Larga Distancia Roam Nac"].ColumnName = "Larga Distancia Roaming Nacional";
                dt.Columns["Larga Distancia Roam Int"].ColumnName = "Larga Distancia Roaming Internacional";
                dt.Columns["Servicios Adicionales"].ColumnName = "Servicios Adicionales";
                dt.Columns["Desc Tiempo Aire"].ColumnName = "Descuento Tiempo Aire";
                dt.Columns["Desc Tiempo Aire Roam"].ColumnName = "Desc Tiempo Aire Roaming";
                dt.Columns["Otros Desc"].ColumnName = "Otros Descuentos";
                dt.Columns["Cargos Creditos"].ColumnName = "Cargos y Creditos";
                dt.Columns["Otros Serv"].ColumnName = "Otros Servicios";
                dt.Columns["Roaming GPRS Internacional"].ColumnName = "Roaming GPRS Internacional";
                dt.Columns["Costo"].ColumnName = "Total";
                dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                dt.AcceptChanges();
            }
            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabDetalleLlamTelcel", dt, true, "Totales",
                    new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}",
                    "{0:0,0}","{0:0,0}","{0:0,0}","{0:0,0}","{0:0,0}","{0:0,0}","{0:0,0}","{0:0,0}"}),
                    "RepTabDetalleLlamTelcel_T", tituloGrid, ""));
        }


        public void RepTabTotalPorRegion(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaBuscaTotalporCuentaConcentradora(1));

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "Region", "Factura", "SubTotal", "iva", "total" });


                    dt.Columns["Region"].ColumnName = "Region";
                    dt.Columns["Factura"].ColumnName = "Factura";
                    dt.Columns["SubTotal"].ColumnName = "Importe";
                    dt.Columns["iva"].ColumnName = "IVA";
                    dt.Columns["total"].ColumnName = "Total";

                    //dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();


                    contenedor.Controls.Add
                    (
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla
                        (
                            DTIChartsAndControls.GridView
                            (
                                "RepTabTotalPorCuentaConcentradora_T",
                                dt,
                                true,
                                "Totales",
                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}" },
                                "",
                                new string[] { },
                                0,
                                new int[] { },
                                new int[] { 0, 1, 2, 3, 4 },
                                new int[] { }
                           ),
                           "RepTabLineasExcedenteGrid_T",
                           tituloGrid
                       )
                    );


                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedor.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RepTabImportePorCuentaConcentradora(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaBuscaTotalporCuentaConcentradora(2));

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "CuentaConcentradora", "CenCosDesc", "SubTotal", "iva", "total" });

                    dt.Columns["CuentaConcentradora"].ColumnName = "Cta. concentradora";
                    dt.Columns["CenCosDesc"].ColumnName = "Branch";
                    dt.Columns["SubTotal"].ColumnName = "Importe";
                    dt.Columns["iva"].ColumnName = "IVA";
                    dt.Columns["total"].ColumnName = "Total";

                    //dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();


                    contenedor.Controls.Add
                    (
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla
                        (
                            DTIChartsAndControls.GridView
                            (
                                "RepTabImportePorCuentaConcentradora",
                                dt,
                                true,
                                "Totales",
                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}" },
                                "",
                                new string[] { },
                                0,
                                new int[] { },
                                new int[] { 0, 1, 2, 3, 4 },
                                new int[] { }
                           ),
                           "RepTabImportePorCuentaConcentradora_T",
                           tituloGrid
                       )
                    );


                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedor.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RepTabImportePorBranch(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaBuscaTotalporCuentaConcentradora(3));

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "CenCosDesc", "SubTotal", "iva", "total" });

                    dt.Columns["CenCosDesc"].ColumnName = "Branch";
                    dt.Columns["SubTotal"].ColumnName = "Importe";
                    dt.Columns["iva"].ColumnName = "IVA";
                    dt.Columns["total"].ColumnName = "Total";

                    //dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();


                    contenedor.Controls.Add
                    (
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla
                        (
                            DTIChartsAndControls.GridView
                            (
                                "RepTabImportePorBranch",
                                dt,
                                true,
                                "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:c}" },
                                "",
                                new string[] { },
                                0,
                                new int[] { },
                                new int[] { 0, 1, 2, 3, 4 },
                                new int[] { }
                           ),
                           "RepTabImportePorBranch_T",
                           tituloGrid
                       )
                    );


                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedor.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RepTabImportePorDepto(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaBuscaTotalporCuentaConcentradora(4));

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                    dt.Columns["DepartamentoDesc"].ColumnName = "Departamento";
                    dt.Columns["CenCosDesc"].ColumnName = "Branch";
                    dt.Columns["SubTotal"].ColumnName = "Importe";
                    dt.Columns["iva"].ColumnName = "IVA";
                    dt.Columns["total"].ColumnName = "Total";

                    //dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                    dt.AcceptChanges();


                    contenedor.Controls.Add
                    (
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla
                        (
                            DTIChartsAndControls.GridView
                            (
                                "RepTabImportePorBranch",
                                dt,
                                true,
                                "Totales",
                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}" },
                                "",
                                new string[] { },
                                0,
                                new int[] { },
                                new int[] { 0, 1, 2, 3, 4 },
                                new int[] { }
                           ),
                           "RepTabImportePorBranch_T",
                           tituloGrid
                       )
                    );


                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedor.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RepTabPorEmplePorMesMov1Pnl(Control contenedorGrafica, string tituloGrafica)
        {
            try
            {


                DataTable dt = DSODataAccess.Execute(ConsultaRepTabPorMEplePorMes());

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {

                    dt = TransponerDataTableMeses(dt);

                    List<string> listaCol = FCAndControls.extraeNombreColumnas(dt).ToList();
                    listaCol = listaCol.Where(x => x != "NumeroMes").ToList();

                    DataView dvldt = new DataView(dt);
                    dt = dvldt.ToTable(false, listaCol.ToArray());
                    if (dt.Rows[dt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }
                    dt.AcceptChanges();


                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                    "RepTabHistPorEmplePorMes_G", tituloGrafica, 0, FCGpoGraf.Matricial));

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                 FCAndControls.ConvertDataTabletoDataTableArray(dt));

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHistPorEmplePorMes",
                        FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt), "RepTabHistPorEmplePorMes_G", ""
                        , "", "Mes", "Importe", 0, FCGpoGraf.Matricial), false);

                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedorGrafica.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        private DataTable TransponerDataTableMeses(DataTable dt)
        {
            try
            {
                DataTable dtT = new DataTable();
                List<string> listaEmpleados = new List<string>();

                dtT.Columns.Add("NumeroMes", typeof(int));
                dtT.Columns.Add("Mes", typeof(string));

                dtT.Rows.Add(1, "Enero");
                dtT.Rows.Add(2, "Febrero");
                dtT.Rows.Add(3, "Marzo");
                dtT.Rows.Add(4, "Abril");
                dtT.Rows.Add(5, "Mayo");
                dtT.Rows.Add(6, "Junio");
                dtT.Rows.Add(7, "Julio");
                dtT.Rows.Add(8, "Agosto");
                dtT.Rows.Add(9, "Septiembre");
                dtT.Rows.Add(10, "Octubre");
                dtT.Rows.Add(11, "Noviembre");
                dtT.Rows.Add(12, "Diciembre");
                foreach (DataRow row in dt.Rows)
                {
                    dtT.Columns.Add(row["nombre Completo"].ToString(), typeof(double));
                    listaEmpleados.Add(row["nombre Completo"].ToString());
                }


                double totalPorEmplePorEnero = 0;
                double totalPorEmplePorFebrero = 0;
                double totalPorEmplePorMarzo = 0;
                double totalPorEmplePorAbril = 0;
                double totalPorEmplePorMayo = 0;
                double totalPorEmplePorJunio = 0;
                double totalPorEmplePorJulio = 0;
                double totalPorEmplePorAgosto = 0;
                double totalPorEmplePorSeptiembre = 0;
                double totalPorEmplePorOctubre = 0;
                double totalPorEmplePorNoviembre = 0;
                double totalPorEmplePorDiciembre = 0;
                string empledt = string.Empty;

                foreach (DataRow rowdt in dt.Rows)
                {
                    empledt = rowdt["nombre Completo"].ToString();
                    totalPorEmplePorEnero = Convert.ToDouble(rowdt["Enero"].ToString());
                    totalPorEmplePorFebrero = Convert.ToDouble(rowdt["Febrero"].ToString());
                    totalPorEmplePorMarzo = Convert.ToDouble(rowdt["Marzo"].ToString());
                    totalPorEmplePorAbril = Convert.ToDouble(rowdt["Abril"].ToString());
                    totalPorEmplePorMayo = Convert.ToDouble(rowdt["Mayo"].ToString());
                    totalPorEmplePorJunio = Convert.ToDouble(rowdt["Junio"].ToString());
                    totalPorEmplePorJulio = Convert.ToDouble(rowdt["Julio"].ToString());
                    totalPorEmplePorAgosto = Convert.ToDouble(rowdt["Agosto"].ToString());
                    totalPorEmplePorSeptiembre = Convert.ToDouble(rowdt["Septiembre"].ToString());
                    totalPorEmplePorOctubre = Convert.ToDouble(rowdt["Octubre"].ToString());
                    totalPorEmplePorNoviembre = Convert.ToDouble(rowdt["Noviembre"].ToString());
                    totalPorEmplePorDiciembre = Convert.ToDouble(rowdt["Diciembre"].ToString());

                    dtT.Rows[0][empledt] = totalPorEmplePorEnero;
                    dtT.Rows[1][empledt] = totalPorEmplePorFebrero;
                    dtT.Rows[2][empledt] = totalPorEmplePorMarzo;
                    dtT.Rows[3][empledt] = totalPorEmplePorAbril;
                    dtT.Rows[4][empledt] = totalPorEmplePorMayo;
                    dtT.Rows[5][empledt] = totalPorEmplePorJunio;
                    dtT.Rows[6][empledt] = totalPorEmplePorJulio;
                    dtT.Rows[7][empledt] = totalPorEmplePorAgosto;
                    dtT.Rows[8][empledt] = totalPorEmplePorSeptiembre;
                    dtT.Rows[9][empledt] = totalPorEmplePorOctubre;
                    dtT.Rows[10][empledt] = totalPorEmplePorNoviembre;
                    dtT.Rows[11][empledt] = totalPorEmplePorDiciembre;
                }

                return dtT;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RepTabConsumoPorLinea(Control contenedorGrid, string tituloGrid)
        {
            DataTable dtRep = DSODataAccess.Execute(ConsultaReporteConsumoFPorLinea());
            DataView dtvRep = new DataView(dtRep);


            string[] columnasTable = { "Tel", "NomCompleto", "Descripcion", "RentaLinea", "PlanLineaDesc", "MbIncluidosPlan", "MbConsumo", "LlamsConsumo" };
            dtRep = dtvRep.ToTable(false, columnasTable);


            #region Eliminar Columnas innecesarias

            if (dtRep.Columns.Contains("Id")) { dtRep.Columns.Remove("Id"); }
            if (dtRep.Columns.Contains("Linea")) { dtRep.Columns.Remove("Linea"); }
            if (dtRep.Columns.Contains("Carrier")) { dtRep.Columns.Remove("Carrier"); }
            if (dtRep.Columns.Contains("CarrierDesc")) { dtRep.Columns.Remove("CarrierDesc"); }
            if (dtRep.Columns.Contains("PlanLinea")) { dtRep.Columns.Remove("PlanLinea"); }
            if (dtRep.Columns.Contains("DatosNac")) { dtRep.Columns.Remove("DatosNac"); }
            if (dtRep.Columns.Contains("TotalNac")) { dtRep.Columns.Remove("TotalNac"); }
            if (dtRep.Columns.Contains("DatosInt")) { dtRep.Columns.Remove("DatosInt"); }
            if (dtRep.Columns.Contains("TotalInt")) { dtRep.Columns.Remove("TotalInt"); }
            if (dtRep.Columns.Contains("CantLlam")) { dtRep.Columns.Remove("CantLlam"); }
            if (dtRep.Columns.Contains("TotalLlams")) { dtRep.Columns.Remove("TotalLlams"); }
            if (dtRep.Columns.Contains("MbTolerancia")) { dtRep.Columns.Remove("MbTolerancia"); }
            if (dtRep.Columns.Contains("LlamsTolerancia")) { dtRep.Columns.Remove("LlamsTolerancia"); }
            if (dtRep.Columns.Contains("MbCriterio")) { dtRep.Columns.Remove("MbCriterio"); }
            if (dtRep.Columns.Contains("LlamsCriterio")) { dtRep.Columns.Remove("LlamsCriterio"); }
            if (dtRep.Columns.Contains("iCodCatEmple")) { dtRep.Columns.Remove("iCodCatEmple"); }
            if (dtRep.Columns.Contains("iCodcatCenCos")) { dtRep.Columns.Remove("iCodcatCenCos"); }

            #endregion

            #region Cambia nombres Columnas

            if (dtRep.Columns.Contains("Tel")) { dtRep.Columns["Tel"].ColumnName = "Linea"; }
            if (dtRep.Columns.Contains("PlanLineaDesc")) { dtRep.Columns["PlanLineaDesc"].ColumnName = "Plan celular"; }
            if (dtRep.Columns.Contains("RentaLinea")) { dtRep.Columns["RentaLinea"].ColumnName = "Renta"; }
            if (dtRep.Columns.Contains("MbIncluidosPlan")) { dtRep.Columns["MbIncluidosPlan"].ColumnName = "Megas incluidos"; }
            if (dtRep.Columns.Contains("MbConsumo")) { dtRep.Columns["MbConsumo"].ColumnName = "Megas consumidos"; }
            if (dtRep.Columns.Contains("LlamsConsumo")) { dtRep.Columns["LlamsConsumo"].ColumnName = "Cantidad llamadas"; }
            if (dtRep.Columns.Contains("NomCompleto")) { dtRep.Columns["NomCompleto"].ColumnName = "Responsable"; }
            if (dtRep.Columns.Contains("Descripcion")) { dtRep.Columns["Descripcion"].ColumnName = "CC."; }

            #endregion

            List<string> listaformatos = new List<string>();
            if (dtRep.Rows.Count > 0)
            {



                listaformatos = new List<string>();

                for (int i = 1; i <= dtRep.Columns.Count; i++)
                {
                    if (dtRep.Columns[i - 1].ColumnName.ToLower() == "renta")
                    {
                        listaformatos.Add("{0:c}");
                    }
                    else if (dtRep.Columns[i - 1].ColumnName.ToLower() == "megas incluidos" || dtRep.Columns[i - 1].ColumnName.ToLower() == "megas consumidos" || dtRep.Columns[i - 1].ColumnName.ToLower() == "cantidad llamadas")
                    {
                        listaformatos.Add("{0:0,0}");
                    }
                    else
                    {
                        listaformatos.Add("");
                    }

                }
            }

            //Elimina Nulos lo cambia por ceros
            foreach (DataRow row in dtRep.Rows)
            {

                foreach (DataColumn column in dtRep.Columns)
                {
                    if (row[column] == DBNull.Value)
                    {
                        row[column] = 0;
                    }
                }
            }


            #region Reporte
            GridView grdV = new GridView();
            grdV = DTIChartsAndControls.GridView("ReporteConsumoPorLinea", dtRep, true, "Totales", listaformatos.ToArray<string>());

            contenedorGrid.Controls.Add(
                                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                                                                                 grdV,
                                                                                                "ReporteConsumoPorLinea_T",
                                                                                                tituloGrid
                                                                                            )
                                        );
            #endregion



        }

        #region Reporte lineas sin actividad telcel

        public void RepLineasTelcelSinActividad(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsRepLineasTelcelSinActividad());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add(string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepLineasTelcelSinActividadGrid_T", dtReporte, false, "Totales", formatoCols.ToArray()),
                                "RepLineasTelcelSinActividadPnl_T", tituloGrid)
                );
        }

        #endregion Reporte lineas sin actividad telcel
        public void RepTabLineasSinAct3MesesN2(Control contenedorGrid, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaRepLineasSinAct3MesesN2());

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    //DataView dvldt = new DataView(dt);
                    //dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                    //dt.Columns["DepartamentoDesc"].ColumnName = "Línea";
                    //dt.Columns["CenCosDesc"].ColumnName = "Empleado ";
                    //dt.Columns["SubTotal"].ColumnName = "Centro de costos";
                    //dt.AcceptChanges();


                    contenedorGrid.Controls.Add
                    (DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepTabLineasSinAct3MesesN2", dt, true,
                        "Totales", new string[] { "", "{0:c}", "{0:c}", "{0:c}", "", "", "" }, "", new string[] { }, 0, new int[] { },
                        new int[] { 0, 1, 2, 3, 4, 5, 6 }, new int[] { }), "RepTabLineasSinAct3MesesN2_T", tituloGrid));
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedorGrid.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RepLineasExcedenteInternet(Control contenedorGrid, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaRepExcendenteInternet());

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    //DataView dvldt = new DataView(dt);
                    //dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                    //dt.Columns["DepartamentoDesc"].ColumnName = "Línea";
                    //dt.Columns["CenCosDesc"].ColumnName = "Empleado ";
                    //dt.Columns["SubTotal"].ColumnName = "Cencos";
                    //dt.Columns["SubTotal"].ColumnName = "Plan";
                    //dt.AcceptChanges();


                    contenedorGrid.Controls.Add
                    (DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepLineasExcedenteInternet", dt, true,
                        "Totales", new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, "", new string[] { }, 0, new int[] { },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, new int[] { }), "RepLineasExcedenteInternet_T", tituloGrid));
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedorGrid.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RepLineasExcedenPlanBase(Control contenedorGrid, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaLineasExcedenPlanBase());

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    //DataView dvldt = new DataView(dt);
                    //dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                    //dt.Columns["DepartamentoDesc"].ColumnName = "Línea";
                    //dt.Columns["CenCosDesc"].ColumnName = "Empleado ";
                    //dt.Columns["SubTotal"].ColumnName = "Cencos";
                    //dt.Columns["SubTotal"].ColumnName = "Plan";
                    //dt.AcceptChanges();


                    contenedorGrid.Controls.Add
                    (DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepLineasExcedenPlanBase", dt, true,
                        "Totales", new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, "", new string[] { }, 0, new int[] { },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, new int[] { }), "RepLineasExcedenPlanBase_T", tituloGrid));
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedorGrid.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RepTabInventarioLineasMoviles(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaInventarioLineasMoviles());

            if (dt != null && dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Extension", "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Nombre Razon social", "Nombre Tipo Plan", "Nombre Tipo Equipo", "Modelo Equipo", "IMEI", "Plan", "Plazo Forzoso" });
                dt.Columns["Extension"].ColumnName = "Linea";
                dt.Columns["No Nomina"].ColumnName = "No.Nomina";
                dt.Columns["Nombre Completo"].ColumnName = "Empleado";
                dt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                dt.Columns["Nombre Razon social"].ColumnName = "Razon social";
                dt.Columns["Nombre Tipo Plan"].ColumnName = "Tipo de Plan";
                dt.Columns["Nombre Tipo Equipo"].ColumnName = "Tipo de Equipo";
                dt.Columns["Modelo Equipo"].ColumnName = "Modelo del Equipo";
                dt.Columns["IMEI"].ColumnName = "IMEI";
                dt.Columns["Plan"].ColumnName = "Plan";
                dt.Columns["Plazo Forzoso"].ColumnName = "Plazo Forzoso";
                dt = DTIChartsAndControls.ordenaTabla(dt, "Empleado Asc");
                dt.AcceptChanges();
            }

            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabInventarioLineasMoviles_T", dt, false, "", new string[] { "", "", "", "", "", "", "", "", "", "", "{0:c}" }),
                                "RepTabInventarioLineasMoviles_T", tituloGrid, ""));

        }
        public void RepDesgloseTipoExcedente(Control contenedorGrid, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaDesgloseTipoExcedente());

                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                {
                    //DataView dvldt = new DataView(dt);
                    //dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                    //dt.Columns["DepartamentoDesc"].ColumnName = "Línea";
                    //dt.Columns["CenCosDesc"].ColumnName = "Empleado ";
                    //dt.Columns["SubTotal"].ColumnName = "Cencos";
                    //dt.Columns["SubTotal"].ColumnName = "Plan";
                    //dt.AcceptChanges();


                    contenedorGrid.Controls.Add
                    (DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepDesgloseTipoExcedente", dt, true,
                        "Totales", new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }, "", new string[] { }, 0, new int[] { },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, new int[] { }), "RepDesgloseTipoExcedente_T", tituloGrid));
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Text = "No hay datos que mostrar";

                    contenedorGrid.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Dashboard Consumo Internet
        public void DistribucionGBInternet(Control contenedorGrafica, string tituloGrafica)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaDistribucionGB());
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt.Columns["GBConsumidos"].ColumnName = "GB Consumidos";
                    dt.Columns["%GBConsumidos"].ColumnName = "% GB Consumidos";
                    dt = DTIChartsAndControls.ordenaTabla(dt, "GB Consumidos Desc");
                    dt.AcceptChanges();

                    contenedorGrafica.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView("RepConsumoConceptoGB_T", dt, true, "Totales",
                            new string[] { "", "", "{0:0,0}" }),
                            "RepConsumoConceptoGB_T", tituloGrafica, "", 2, FCGpoGraf.TabularBaCoDoTa)
                        );

                    dt = dvldt.ToTable(false, new string[] { "APP", "% GB Consumidos" });
                    if (dt.Rows[dt.Rows.Count - 1]["APP"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }

                    dt.Columns["APP"].ColumnName = "label";
                    dt.Columns["% GB Consumidos"].ColumnName = "value";
                    dt.AcceptChanges();

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoGBGraf_G",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10)),
                        "RepConsumoConceptoGB_T", "", "",
                        "APP", "% GB Consumidos", 2, FCGpoGraf.TabularBaCoDoTa, "%"), false);
                }
                else
                {
                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepDistribucionGBGraf_G", tituloGrafica, false));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void DistribucionMonedaInternet(Control contenedorGrafica, string tituloGrafica)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaDistribucionMoneda());
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt = DTIChartsAndControls.ordenaTabla(dt, "Importe Desc");
                    dt.AcceptChanges();

                    contenedorGrafica.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView("RepConsumoConceptoMoneda_T", dt, true, "Totales",
                            new string[] { "", "{0:c}", "" }),
                            "RepConsumoConceptoMoneda_T", tituloGrafica, "", 2, FCGpoGraf.TabularBaCoDoTa)
                        );

                    dt = dvldt.ToTable(false, new string[] { "APP", "%Importe" });
                    if (dt.Rows[dt.Rows.Count - 1]["APP"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }

                    dt.Columns["APP"].ColumnName = "label";
                    dt.Columns["%Importe"].ColumnName = "value";
                    dt.AcceptChanges();

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoMonedaGraf_G",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10)),
                        "RepConsumoConceptoMoneda_T", "", "",
                        "APP", "% Importe", 2, FCGpoGraf.TabularBaCoDoTa, "%"), false);


                    //contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepConsumoConceptoMonedaGraf_G", tituloGrafica));

                    //ScriptManager.RegisterStartupScript(this, typeof(Page), "RepConsumoConceptoMonedaGraf_G",
                    //    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
                    //    "RepConsumoConceptoMonedaGraf_G", "", "", "APP", "Importe", "doughnut2d"), false);
                }
                else
                {
                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoMonedaGraf_G", tituloGrafica, false));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void ConsolidadoPorApp(Control contenedorGrid, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt = DTIChartsAndControls.ordenaTabla(dt, "IMPORTE Desc");
                    dt.AcceptChanges();

                    contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView(
                              "RepTabConsolidadoApp_T", dt, true, "Totales",
                              new string[] { "", "{0:c}", "" }, 0),
                              "RepTabConsolidadoApp_T", tituloGrid, "")
                      );
                }
                else
                {
                    contenedorGrid.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepTabConsolidadoApp_T", tituloGrid, "", false));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ConsumoHistoricoInternet(Control contenedorGrafica, string tituloGrafica)
        {

            try
            {
                DataTable ldt1 = DSODataAccess.Execute(ConsultaConsumoHistoricoInternet());
                if (ldt1 != null && ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
                {
                    contenedorGrafica.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepConsumoConceptoHistGrid", ldt1, true, "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }),
                                "RepConsumoConceptoHistGrid", tituloGrafica, "", 3, FCGpoGraf.MatricialConStack)
                            );
                    if (ldt1.Rows[ldt1.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                    {
                        ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                    }
                    ldt1.AcceptChanges();

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt1));
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoHistGrid",
                        FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt1), "RepConsumoConceptoHistGrid",
                        "", "", "APP", "Importe", 3, FCGpoGraf.MatricialConStack, "$", ""), false);
                }
                else
                {
                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoHistLineaGraf_G", tituloGrafica, "", false));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UsoInternetPorTabs(Control contenedorGrafica, string tituloGrafica)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());
                if (dt != null && dt.Rows.Count > 0)
                {
                    TabInternetImporte(dt, tituloGrafica);
                    TabInternetGB(dt, tituloGrafica);
                    TabInternetImporteGB(dt, tituloGrafica);

                    TabContainerPrincipal.ActiveTabIndex = 0;
                    TabContainerPrincipal.Visible = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void TabInternetImporte(DataTable dt, string tituloGrafica)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                TabPanelUno.Visible = true;
                DataTable dt3 = dt;

                DataView dvldt = new DataView(dt3);
                dt3 = DTIChartsAndControls.ordenaTabla(dt3, "IMPORTE Desc");
                dt3.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab1Rep1_T", dt3, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}" });

                dt3 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE" });
                if (dt3.Rows[dt3.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt3.Rows[dt3.Rows.Count - 1].Delete();
                }

                dt3.Columns["APP"].ColumnName = "label";
                dt3.Columns["IMPORTE"].ColumnName = "value";
                dt3.AcceptChanges();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoTab1Rep1_Graf",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt3),
                    "RepConsumoConceptoTab1Rep1_Graf", "", "", "APP", "IMPORTE", "doughnut2d", "$", "", "dti", "88%", "190"), false);

                Tab1Rep1.Controls.Add(DTIChartsAndControls.TituloY2RowsTablaGrafica(Controles, "RepConsumoConceptoTab1Rep1_T", tituloGrafica, "RepConsumoConceptoTab1Rep1_Graf", true));

            }
        }
        public void TabInternetGB(DataTable dt, string tituloGrafica)
        {
            try
            {
                TabPanelDos.Visible = true;
                DataTable dt2 = dt;

                DataView dvldt = new DataView(dt2);
                dt2 = DTIChartsAndControls.ordenaTabla(dt2, "GB Desc");
                dt2.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab2Rep1_T", dt2, true, "Totales",
                new string[] { "", "{0:c}", "{0:0,0}" });

                dt2 = dvldt.ToTable(false, new string[] { "APP", "GB" });
                if (dt2.Rows[dt2.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt2.Rows[dt2.Rows.Count - 1].Delete();
                }

                dt2.Columns["APP"].ColumnName = "label";
                dt2.Columns["GB"].ColumnName = "value";
                dt2.AcceptChanges();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoTab2Rep1Graf_Graf",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt2),
                    "RepConsumoConceptoTab2Rep1Graf_Graf", "", "", "APP", "IMPORTE", "doughnut2d", "$", "", "dti", "88%", "190"), false);

                Tab2Rep1.Controls.Add(DTIChartsAndControls.TituloY2RowsTablaGrafica(Controles, "RepConsumoConceptoTab2Rep1_T", tituloGrafica, "RepConsumoConceptoTab2Rep1Graf_Graf", true));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TabInternetImporteGB(DataTable dt, string tituloGrafica)
        {
            try
            {
                TabPanelTres.Visible = true;

                DataTable dt1 = dt;
                DataView dvldt = new DataView(dt1);
                dt1 = DTIChartsAndControls.ordenaTabla(dt1, "IMPORTE Desc");
                dt1.AcceptChanges();

                Control Controles = DTIChartsAndControls.GridView("RepConsumoConceptoTab3Rep1_T", dt1, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}" });

                dt1 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE", "GB" });
                if (dt1.Rows[dt1.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt1.Rows[dt1.Rows.Count - 1].Delete();
                }

                dt1.Columns["APP"].ColumnName = "label";
                dt1.Columns["IMPORTE"].ColumnName = "value";
                dt1.AcceptChanges();

                DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(dt1);
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoConceptoTab3Rep1Graf_Graf",
                    FCAndControls.GraficaCombinadaSencilla(DataSourceArrayJSon,
                    new string[] { "App", "IMPORTE", "GB" }, "RepConsumoConceptoTab3Rep1Graf_Graf", "", "", "App", "Importe", "GB",
                    FCTipoEjeSecundario.area, "$", "", "dti", "88%", "195"), false);

                Tab3Rep1.Controls.Add(DTIChartsAndControls.TituloY2RowsTablaGrafica(Controles, "RepConsumoConceptoTab3Rep1_T", tituloGrafica, "RepConsumoConceptoTab3Rep1Graf_Graf", true));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DistribucionConsumoInternetPorApp(Control contenedorGrafica, string tituloGrafica)
        {
            int pestaniaActiva = 0;
            try
            {
                DataTable ldt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());
                if (ldt != null && ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    dvldt.Sort = "Importe desc, GB desc";
                    ldt = dvldt.ToTable(false, new string[] { "APP", "Importe", "GB" });

                    contenedorGrafica.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                           DTIChartsAndControls.GridView("RepHistGrid", ldt, true, "Totales", new string[] { "", "{0:c}", "" }),
                           "RepTabDistConsInternetPorApp1Pnl", tituloGrafica, Request.Path + "?Nav=DistConsumoIntPorAppN1&TipoConsumo=" + param["TipoConsumo"], pestaniaActiva, FCGpoGraf.ColsCombinada));

                    if (ldt.Rows[ldt.Rows.Count - 1]["APP"].ToString() == "Totales")
                    {
                        ldt.Rows[ldt.Rows.Count - 1].Delete();
                    }
                    ldt.Columns["APP"].ColumnName = "label";
                    ldt.Columns["Importe"].ColumnName = "value";
                    ldt.AcceptChanges();


                    DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(ldt);
                    string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "RepTabDistConsInternetPorApp1Pnl",
                        FCAndControls.GraficaCombinada(DataSourceArrayJSon,
                        new string[] { "App", "Importe", "GB" }, "RepTabDistConsInternetPorApp1Pnl", "", "", "App", "Importe", "GB",
                        pestaniaActiva, FCTipoEjeSecundario.area, FCGpoGraf.ColsCombinada), false);
                }
                else
                {
                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepConsumoConceptoHistLineaGraf_G", tituloGrafica, "", false));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DistribucionConsumoInternetPorApp2Pnls(Control contenedorGrafica, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());

            #region Grid

            if (ldt.Rows.Count > 0)
            {
                DataView ldv = new DataView(ldt);
                ldv.Sort = "Importe desc, GB desc";
                ldt = ldv.ToTable(false, new string[] { "APP", "Importe", "GB" });
                contenedorGrid.Controls.Add(
                               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                               DTIChartsAndControls.GridView("RepHistGrid_T", ldt, true, "Totales",
                                               new string[] { "", "{0:c}", "" }),
                                               "RepTabHistMov2Pnls_T", tituloGrid)
                               );

            }
            else
            {
                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, "RepTabHistMov2Pnls_T", tituloGrafica, "", false));
            }
            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                dvldt.Sort = "Importe desc, GB desc";
                ldt = dvldt.ToTable(false, new string[] { "APP", "Importe", "GB" });
                for (int i = ldt.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = ldt.Rows[i];
                    if (dr["APP"].ToString().ToLower() == "totales")
                        dr.Delete();
                }
                ldt.AcceptChanges();
                ldt.Columns["APP"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(ldt);
            string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                    "RepTabHistMov2Pnls_G", tituloGrafica, pestaniaActiva, FCGpoGraf.ColsCombinada));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHistMov2Pnls_G",
                FCAndControls.GraficaCombinada(DataSourceArrayJSon,
                new string[] { "App", "Importe", "GB" },
                 "RepTabHistMov2Pnls_G", "", "", "App", "Importe", "GB", pestaniaActiva, FCTipoEjeSecundario.area, FCGpoGraf.ColsCombinada), false);

            #endregion Grafica
        }
        private void HistoricoGastoInternet12Meses(Control contenedorGrafica, string tituloGrafica)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsumoHistorico12Meses());
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt.Columns["Fecha"].ColumnName = "MES";
                    dt.Columns["GastoNacional"].ColumnName = " Gasto internet nal";
                    dt.Columns["GastoInternac"].ColumnName = "Gasto internet intl";
                    dt.Columns["GastoPaquetes"].ColumnName = "Gasto en paquetes";
                    dt.AcceptChanges();

                    contenedorGrafica.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView("RepConsumoHistorico12MesesGrid", dt, true, "Totales",
                            new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                            "RepConsumoHistorico12MesesGrid", tituloGrafica, Request.Path + "?Nav=HistGastoInternetN1", 0, FCGpoGraf.MatricialConStack)
                            );

                    if (dt.Rows[dt.Rows.Count - 1]["MES"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }
                    dt.AcceptChanges();

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dt));
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoHistorico12MesesGrid",
                        FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt), "RepConsumoHistorico12MesesGrid",
                        "", "", "MES", "Importe", 0, FCGpoGraf.MatricialConStack, "$", ""), false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void HistoricoGastoInternet12Meses2Pnl(Control contenedorGrafica, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid, int pestaniaActiva)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsumoHistorico12Meses());
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt.Columns["Fecha"].ColumnName = "MES";
                    dt.Columns["GastoNacional"].ColumnName = " Gasto internet nal";
                    dt.Columns["GastoInternac"].ColumnName = "Gasto internet intl";
                    dt.Columns["GastoPaquetes"].ColumnName = "Gasto en paquetes";
                    dt.AcceptChanges();

                    contenedorGrid.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsumoHistorico12MesesGrid_T", dt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                "RepConsumoHistorico12MesesGrid_T", tituloGrid)
                                );

                    if (dt.Rows[dt.Rows.Count - 1]["MES"].ToString() == "Totales")
                    {
                        dt.Rows[dt.Rows.Count - 1].Delete();
                    }
                    dt.AcceptChanges();

                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                        "RepConsumoHistorico12MesesGraf_G", tituloGrafica, 0, FCGpoGraf.Matricial));

                    string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                                 FCAndControls.ConvertDataTabletoDataTableArray(dt));

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoHistorico12MesesGraf",
                        FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt), "RepConsumoHistorico12MesesGraf_G", ""
                        , "", "MES", "Importe", 0, FCGpoGraf.Matricial), false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TabsUsoInternetPor(Control contenedor, string titulo, string idContenedor)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());
                if (dt != null && dt.Rows.Count > 0)
                {
                    Control[] tablasDatos = new Control[3];
                    string[] titulosRep = new string[3];

                    TabInternetImporteV1(dt, titulo, idContenedor, ref tablasDatos, ref titulosRep, 0);
                    TabInternetGBV1(dt, titulo, idContenedor, ref tablasDatos, ref titulosRep, 1);
                    TabInternetImporteGBV1(dt, titulo, idContenedor, ref tablasDatos, ref titulosRep, 2);

                    contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañas2RowsTablaGrafica(tablasDatos, titulosRep, idContenedor, titulo, 0,
                    false, "", "", this, false));

                    //                   contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañas2RowsTablaGrafica(tablasDatos, titulosRep, idContenedor, titulo, 0,
                    //true, "btnExportXLS_Click", "btnConsumoPor" + param["TipoConsumo"], this, false));
                }
                else
                {
                    contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(new Label() { Text = "No hay información por mostrar" }, idContenedor, titulo, "", false));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void TabInternetImporteV1(DataTable dt, string tituloGrafica, string contenedor, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                DataTable dt3 = dt;

                DataView dvldt = new DataView(dt3);
                dt3 = DTIChartsAndControls.ordenaTabla(dt3, "IMPORTE Desc");
                dt3.AcceptChanges();

                titulosRep[index] = "Importes";
                tablasDatos[index] = DTIChartsAndControls.GridView(contenedor + "Tab1Rep1_T", dt3, true, "Totales",
                                new string[] { "", "{0:c}", "" });
                /*new string[] { "", "{0:c}", "{0:0,0}" });*/

                dt3 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE" });
                if (dt3.Rows[dt3.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt3.Rows[dt3.Rows.Count - 1].Delete();
                }

                dt3.Columns["APP"].ColumnName = "label";
                dt3.Columns["IMPORTE"].ColumnName = "value";
                dt3.AcceptChanges();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "graf_" + contenedor + "_" + index.ToString(),
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt3),
                    "graf_" + contenedor + "_" + index.ToString(), "", "", "APP", "IMPORTE", "doughnut2d", "$", "", "dti", "88%", "190"), false);
            }
        }
        public void TabInternetGBV1(DataTable dt, string tituloGrafica, string contenedor, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            try
            {
                DataTable dt2 = dt;

                DataView dvldt = new DataView(dt2);
                dt2 = DTIChartsAndControls.ordenaTabla(dt2, "GB Desc");
                dt2.AcceptChanges();

                titulosRep[index] = "Tráfico (GB)";

                tablasDatos[index] = DTIChartsAndControls.GridView(contenedor + "Tab2Rep1_T", dt2, true, "Totales",
                new string[] { "", "{0:c}", "" });
                /*new string[] { "", "{0:c}", "{0:0,0}" });*/

                dt2 = dvldt.ToTable(false, new string[] { "APP", "GB" });
                if (dt2.Rows[dt2.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt2.Rows[dt2.Rows.Count - 1].Delete();
                }

                dt2.Columns["APP"].ColumnName = "label";
                dt2.Columns["GB"].ColumnName = "value";
                dt2.AcceptChanges();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "graf_" + contenedor + "_" + index.ToString(),
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt2),
                    "graf_" + contenedor + "_" + index.ToString(), "", "", "APP", "IMPORTE", "doughnut2d", "$", "", "dti", "88%", "190"), false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TabInternetImporteGBV1(DataTable dt, string tituloGrafica, string contenedor, ref Control[] tablasDatos, ref string[] titulosRep, int index)
        {
            try
            {

                DataTable dt1 = dt;
                DataView dvldt = new DataView(dt1);
                dt1 = DTIChartsAndControls.ordenaTabla(dt1, "IMPORTE Desc");
                dt1.AcceptChanges();

                titulosRep[index] = "Importe/Tráfico";
                tablasDatos[index] = DTIChartsAndControls.GridView(contenedor + "Tab3Rep1_T", dt1, true, "Totales",
                    new string[] { "", "{0:c}", "" });
                /*new string[] { "", "{0:c}", "{0:0,0}" });*/

                dt1 = dvldt.ToTable(false, new string[] { "APP", "IMPORTE", "GB" });
                if (dt1.Rows[dt1.Rows.Count - 1]["APP"].ToString() == "Totales")
                {
                    dt1.Rows[dt1.Rows.Count - 1].Delete();
                }

                dt1.Columns["APP"].ColumnName = "label";
                dt1.Columns["IMPORTE"].ColumnName = "value";
                dt1.AcceptChanges();

                DataTable[] arrColumns = FCAndControls.ConvertDataTabletoDataTableArray(dt1);
                string[] DataSourceArrayJSon = FCAndControls.ConvertDataTabletoJSONString(arrColumns);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "graf_" + contenedor + "_" + index.ToString(),
                    FCAndControls.GraficaCombinadaSencilla(DataSourceArrayJSon,
                    new string[] { "App", "IMPORTE", "GB" }, "graf_" + contenedor + "_" + index.ToString(), "", "", "", "Importe", "GB",
                    FCTipoEjeSecundario.area, "$", "", "dti", "88%", "195"), false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        // TODO : DM Paso 2 - Metodo que crea reporte
        /*Aqui podemos copiar uno de los metodos que estan ya hechos y hacer los ajustes necesarios para mostrar un nuevo reporte*/

        /****************************************************************************************/

        #endregion Reportes
    }
}