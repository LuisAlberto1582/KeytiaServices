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
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.Services;
using KeytiaWeb.UserInterface.Indicadores;
using KeytiaWeb.Resources;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class Dashboard
    {

        #region Reportes

        private void RepCantContestadasYNoContestadas(Control contenedor, string link, int pestaniaActiva)
        {
            #region Variables de configuración para reporte

            DataTable dtCantLineasContestadasNoContestadas;
            string tituloReporte;
            List<string> nombreColumnas;

            #endregion

            dtCantLineasContestadasNoContestadas = DSODataAccess.Execute(RepCantLineasContestadasYNoContestadas());

            tituloReporte = "Cantidad de Llamadas entrantes no contestadas y devueltas";

            nombreColumnas = new List<string>();
            foreach (DataColumn dataCol in dtCantLineasContestadasNoContestadas.Columns)
            {
                nombreColumnas.Add(dataCol.ColumnName);
            }

            if (dtCantLineasContestadasNoContestadas.Rows.Count > 0 && dtCantLineasContestadasNoContestadas.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtCantLineasContestadasNoContestadas);
                string[] campos = nombreColumnas.ToArray();
                int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5, 6 };
                int[] camposNavegacion = new int[] { };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "" };

                dtCantLineasContestadasNoContestadas = dvldt.ToTable(false, campos);

                GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil", dtCantLineasContestadasNoContestadas, false, "",
                      formatoColumnas,
                      link, new string[] { "" },
                      1, camposNavegacion, camposBoundField, new int[] { }, 2);

                // Alineamos las celdas
                foreach (int index in Enumerable.Range(0, grid.Columns.Count))
                {
                    grid.Columns[index].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                }

                dtCantLineasContestadasNoContestadas.AcceptChanges();

                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));

            }
        }

        private void RepTabHist1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            int[] indexColumnasHyperlinkField = new int[] { 0 };
            int[] indexColumnasBoundField = new int[] { 2 };

            string linkGraficaSiguienteNivel = " ''" + Request.Path + "?Nav=HistoricoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&MesAnio='' ";
            string linkGridSiguienteNivel = Request.Path + "?Nav=HistoricoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&MesAnio={0}";
            string linkURLNavegacion = Request.Path + "?Nav=HistoricoN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString();

            if (DSODataContext.Schema.ToLower() == "seveneleven")
            {
                //Este esquema no va a tener navegacion
                linkGraficaSiguienteNivel = " ''#'' ";
                linkGridSiguienteNivel = "";

                indexColumnasHyperlinkField = new int[] { };
                indexColumnasBoundField = new int[] { 0, 2 };
            }

            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(
                    ConsultaConsumoHistorico(linkGraficaSiguienteNivel, omitirInfoCDR, omitirInfoSiana));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "Total", "link" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}" }, linkGridSiguienteNivel,
                                      new string[] { "Mes Anio" }, 0, new int[] { 1, 3 }, indexColumnasBoundField, indexColumnasHyperlinkField),
                                      "RepTabHist1Pnl", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes Anio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes Anio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabHist1Pnl", "", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);


            #endregion Grafica
        }

        private void RepTabHistLlamadas1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            int[] indexColumnasHyperlinkField = new int[] { 0 };
            int[] indexColumnasBoundField = new int[] { 2 };

            string linkGraficaSiguienteNivel = " ''" + Request.Path + "?Nav=HistoricoLlamadasN2&MesAnio='' ";
            string linkGridSiguienteNivel = Request.Path + "?Nav=HistoricoLlamadasN2&MesAnio={0}";
            string linkURLNavegacion = Request.Path + "?Nav=HistoricoLlamadasN1";

            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(
                    ConsultaConsumoHistorico(linkGraficaSiguienteNivel, 0, 0, 1));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "Total", "link" });
                ldt.Columns["Mes Anio"].ColumnName = "Mes - Año";
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["Total"].ColumnName = "Llamadas";
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab1", ldt, true, "Totales",
                                      new string[] { "", "", "" }, linkGridSiguienteNivel,
                                      new string[] { "Mes - Año" }, 0, new int[] { 1, 3 }, indexColumnasBoundField, indexColumnasHyperlinkField),
                                      "RepTabHist1Pnl", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes - Año", "Llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes - Año"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes - Año"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabHist1Pnl", "", "", "Mes", "Llamadas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);


            #endregion Grafica
        }

        private void RepTabHistLlamadasPnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            int[] indexColumnasHyperlinkField = new int[] { 1 };
            int[] indexColumnasBoundField = new int[] { 2 };

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistorico(linkGrafica, omitirInfoCDR, omitirInfoSiana, 1));


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "Total", "link" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsHistGrid", ldt, true, "Totales",
                                      new string[] { "", "", "" }, linkGrid,
                                      new string[] { "Mes Anio" }, 1, new int[] { 0, 3 }, indexColumnasBoundField, indexColumnasHyperlinkField),
                                      "RepTabHist2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabHist2Pnls_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist2Pnls_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabHist2Pnls_G", "", "", "Mes", "Llamadas", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabPorEmpleMasCarosLlamadasPnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                   Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorEmpleMasCaros(linkGrafica, int.MaxValue, omitirInfoCDR, omitirInfoSiana));

            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            int[] camposBoundField;
            string[] camposGrafica;

            hyperLinkFieldIndex = 1;
            camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion", "link" };
            camposBoundField = new int[] { 2, 3, 4 };

            camposGrafica = new string[] { "Colaborador", "Cantidad llamadas", "link" };


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);

                ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            //NZ 20160823

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid", ldt, true, "Totales",
                    new string[] { "", "", "", "{0:0,0}", "{0:0,0}" }, linkGrid,
                    new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                    camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid)
);


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Colaborador"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasCaros_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCaros_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsEmpleMasCaros_G", "", "", "Colaborador", "Llamadas", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        private void RepTabHist2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            int[] indexColumnasHyperlinkField = new int[] { 1 };
            int[] indexColumnasBoundField = new int[] { 2 };

            if (DSODataContext.Schema.ToLower() == "seveneleven")
            {
                //Este esquema no va a tener navegacion
                indexColumnasHyperlinkField = new int[] { };
                indexColumnasBoundField = new int[] { 1, 2 };

                linkGrid = "";
                linkGrafica = "''#''";
            }

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistorico(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "Total", "link" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsHistGrid", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}" }, linkGrid,
                                      new string[] { "Mes Anio" }, 1, new int[] { 0, 3 }, indexColumnasBoundField, indexColumnasHyperlinkField),
                                      "RepTabHist2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabHist2Pnls_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist2Pnls_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabHist2Pnls_G", "", "", "Mes", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabHist1PnlPrs(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPrs("[link] = ''" + Request.Path + "?Nav=HistoricoN2Prs&MesAnio='' + replace([Mes Anio],'' '',''-'')"));

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                RemoveColHerencia(ref ldt);
                ldt.Columns.Remove("Orden");

                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "TotalSimulado",
                    "TotalReal", "CostoSimulado", "CostoReal", "SM"}); //"link" //NZ Se remueve la columna del link por que no habra navegacion para el cliente Prosa

                ArrayList lista = FormatoColumRepHistoricoN1(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsHistGridTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "" }, Request.Path + "?Nav=HistoricoN2Prs&MesAnio={0}",
                                      new string[] { "Mes Anio" }, 1, columnasNoVisibles, columnasVisibles, new int[] { }),
                                      "RepTabHist1PnlPrs", tituloGrid, Request.Path + "?Nav=HistoricoN1Prs", pestaniaActiva, FCGpoGraf.Tabular)
                      );

            #endregion Grid

            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }

                ldt = dvldt.ToTable(false, new string[] { "Mes", "Importe" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist1PnlPrs",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabHist1PnlPrs", "", "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private ArrayList FormatoColumRepHistoricoN1(DataTable ldt, byte nombreTotalReal, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["Nombre Mes"].ColumnName = "Mes";
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 5, 7 };
                    columnasVisibles = new int[] { 1, 4, 6 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 4, 7 };
                    columnasVisibles = new int[] { 1, 5, 6 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";  /*BG. Costo = Real */
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 3, 4, 5, 6, 7 };
                    columnasVisibles = new int[] { 1, 2 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                    nombreTotalReal = 1; //Si vale el TotalSimulado Se cambio por Total (Titulo)
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 4, 5, 6, 7 };
                    columnasVisibles = new int[] { 1, 3 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";        /*BG. Costo = Real */
                    nombreTotalReal = 2; //Si vale el TotalReal Se cambio por Total (Titulo)
                }
            }

            #endregion Logica de las columnas a mostrar

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(nombreTotalReal);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private void RepTabHist2PnlsPrs(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPrs(linkGrafica));

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                RemoveColHerencia(ref ldt);
                ldt.Columns.Remove("Orden");

                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "TotalSimulado",
                    "TotalReal", "CostoSimulado", "CostoReal", "SM"});  //"link" //NZ Se remueve la columna del link por que no habra navegacion para el cliente Prosa

                ArrayList lista = FormatoColumRepHistoricoN1(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }


            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsHistGrid", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "" }, "",
                                      new string[] { "Mes Anio" }, 1, columnasNoVisibles, columnasVisibles, new int[] { }),
                                      "RepTabHist2PnlsPrs_T", tituloGrid)
                      );

            #endregion Grid

            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }

                ldt = dvldt.ToTable(false, new string[] { "Mes", "Importe" });  //"link" //NZ Se remueve la columna del link por que no habra navegacion para el cliente Prosa
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsHistGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHistGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GrafConsHistGraf_G", "", "", "Mes", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorSitio1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {


            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
            {
                campoOrdenamiento = "Total desc";
                arrBoundFieldCols = new int[] { 2, 3, 4 };
                campoAGraficar = "Total";
                numberPrefix = "$ ";
            }
            else
            {
                campoOrdenamiento = "Cantidad llamadas desc";
                arrBoundFieldCols = new int[] { 3, 4 };
                campoAGraficar = "Cantidad llamadas";
                numberPrefix = " ";
            }


            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(
                ConsultaConsumoPorSitio("[link] = ''" + Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio='' + convert(varchar,[Codigo Sitio])", omitirInfoCDR, omitirInfoSiana));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Sitios: " + ldt.Rows.Count.ToString();
                }

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, campoOrdenamiento);
            }



            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=SitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={0}",
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, arrBoundFieldCols, new int[] { 1 }),
                                      "RepTabPorSitio1Pnl", tituloGrid, Request.Path + "?Nav=SitioN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", campoAGraficar, "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }

            //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10
            if (DSODataContext.Schema.ToLower() == "ula")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorSitio1Pnl",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabPorSitio1Pnl", "", "", "Sitio", campoAGraficar, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, numberPrefix), false);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioTab2",
                  FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                  "RepTabPorSitio1Pnl", "", "", "Sitio", campoAGraficar, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, numberPrefix), false);
            }

            #endregion Grafica
        }

        private void RepTabPorSitio2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorSitio(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
            {
                campoOrdenamiento = "Total desc";
                arrBoundFieldCols = new int[] { 2, 3, 4 };
                campoAGraficar = "Total";
                numberPrefix = "$ ";
            }
            else
            {
                campoOrdenamiento = "Cantidad llamadas desc";
                arrBoundFieldCols = new int[] { 3, 4 };
                campoAGraficar = "Cantidad llamadas";
                numberPrefix = " ";
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Sitios: " + ldt.Rows.Count.ToString();
                }
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";        //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";       //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, campoOrdenamiento);
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioGrid", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, arrBoundFieldCols, new int[] { 1 }),
                                      "RepTabPorSitio2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", campoAGraficar, "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorSitioGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular, numberPrefix));


            //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10
            if (DSODataContext.Schema.ToLower() == "ula")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioGraf_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GrafConsPorSitioGraf_G", "", "", "Sitio",
                    campoAGraficar, grafActiva, FCGpoGraf.Tabular, numberPrefix), false);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioGraf_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafConsPorSitioGraf_G", "", "", "Sitio", campoAGraficar, grafActiva, FCGpoGraf.Tabular, numberPrefix), false);
            }


            #endregion Grafica
        }

        private void RepTabPorTDest1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            int[] indicesCamposBoundField;
            string[] formatoColumna;

            Session["OmiteTipoDestino"] = "0";

            string linkGrid = Request.Path + "?Nav=TDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest={0}";
            string linkGrafica = "[link] = ''" + Request.Path + "?Nav=TDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest='' + convert(varchar,[Codigo Tipo Destino])";

            string vchCodigoPerfilUsuario =
                DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();


            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                linkGrid = string.Empty;
                linkGrafica = string.Empty;
            }


            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorTipoDestino(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            /*bandera que identifica si se desglosara el costo de la llamada*/
            int desglosaCosto = 0;
            if (Session["DesgloseCosto"] != null)
            {
                desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
            }


            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4 };
                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 4 };
                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }


                camposGrafica = new string[] { "Tipo de destino", campoAGraficar };
            }
            else
            {
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 3, 4 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }

                //if (desglosaCosto == 1)
                //{
                //    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                //    formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                //    indicesCamposBoundField = new int[] { 2, 3, 4, 5, 6 };
                //}
                //else
                //{
                //    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                //    formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                //    indicesCamposBoundField = new int[] { 2, 3, 4 };
                //}

                camposGrafica = new string[] { "Tipo de destino", campoAGraficar, "link" };

            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total: " + ldt.Rows.Count.ToString();
                }

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                ldt.Columns["Total"].ColumnName = "Total";
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                     formatoColumna, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 7 }, indicesCamposBoundField, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDest1Pnl", tituloGrid, Request.Path + "?Nav=TDestN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorTDest1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "RepTabPorTDest1Pnl", "", "",
                "Tipo de destino", campoAGraficar, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, numberPrefix), false);

            #endregion Grafica
        }

        private void RepTabPorTDest2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid, int[] camposLink, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorTipoDestino(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            string[] formatoColumna;
            int[] indicesCamposBoundField;
            int[] camposNav;
            /*bandera que identifica si se desglosara el costo de la llamada*/
            Session["OmiteTipoDestino"] = "0";
            int desglosaCosto = 0;
            if (Session["DesgloseCosto"] != null)
            {
                desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
            }

            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0 };
                        camposNav = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0 };
                        camposNav = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0 };
                        camposNav = new int[] { 2, 3, 4 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:0,0}", "{0:0,0}" };

                        indicesCamposBoundField = new int[] { 0 };
                        camposNav = new int[] { 2, 3, 4 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }


                camposGrafica = new string[] { "Tipo de destino", campoAGraficar };
            }
            else
            {
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0, 7 };
                        camposNav = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0, 7 };
                        camposNav = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 0, 5 };
                        camposNav = new int[] { 2, 3, 4 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:0,0}", "{0:0,0}", "" };
                        indicesCamposBoundField = new int[] { 0, 4 };
                        camposNav = new int[] { 2, 3 };
                        //new int[] { 0, 5 }
                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }

                camposGrafica = new string[] { "Tipo de destino", campoAGraficar, "link" };

            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total: " + ldt.Rows.Count.ToString();
                }

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                if (ldt.Columns.Contains("Total"))
                {
                    ldt.Columns["Total"].ColumnName = "Total";
                }


                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                }
            }


            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                      formatoColumna, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, indicesCamposBoundField, camposNav, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDest2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorTDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorTDestGraf_G", "", "", "Tipo de destino", campoAGraficar, grafActiva, FCGpoGraf.Tabular, numberPrefix, "", "dti", "98%", "385"), false);

            #endregion Grafica
        }

        private void RepTabPorTDest2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorTipoDestino(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            string[] formatoColumna;
            int[] indicesCamposBoundField;
            /*bandera que identifica si se desglosara el costo de la llamada*/
            int desglosaCosto = 0;
            if (Session["DesgloseCosto"] != null)
            {
                desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
            }

            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {
                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 3, 4 };
                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                        formatoColumna = new string[] { "", "", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 1, 2, 4 };
                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }


                camposGrafica = new string[] { "Tipo de destino", campoAGraficar };
            }
            else
            {
                if (desglosaCosto == 1)
                {

                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4, 5, 6 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {


                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4, 6 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }
                }
                else
                {
                    if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 2, 3, 4 };

                        campoOrdenamiento = "Total desc";
                        campoAGraficar = "Total";
                        numberPrefix = "$ ";
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                        indicesCamposBoundField = new int[] { 3, 4 };

                        campoOrdenamiento = "Cantidad llamadas desc";
                        campoAGraficar = "Cantidad llamadas";
                        numberPrefix = " ";
                    }

                }

                //if (desglosaCosto == 1)
                //{
                //    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                //    formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                //    indicesCamposBoundField = new int[] { 2, 3, 4, 5, 6 };
                //}
                //else
                //{
                //    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                //    formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                //    indicesCamposBoundField = new int[] { 2, 3, 4 };
                //}

                camposGrafica = new string[] { "Tipo de destino", campoAGraficar, "link" };

            }
            //if (string.IsNullOrEmpty(linkGrid))
            //{
            //    hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink                
            //    if (desglosaCosto == 1)
            //    {
            //        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
            //        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
            //        indicesCamposBoundField = new int[] { 1, 2, 3, 4, 5, 6 };
            //    }
            //    else
            //    {
            //        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
            //        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
            //        indicesCamposBoundField = new int[] { 1, 2, 3, 4 };
            //    }

            //    camposGrafica = new string[] { "Tipo de destino", "Total" };
            //}
            //else
            //{
            //    if (desglosaCosto == 1)
            //    {
            //        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
            //        formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
            //        indicesCamposBoundField = new int[] { 2, 3, 4, 5, 6 };
            //    }
            //    else
            //    {
            //        camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
            //        formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
            //        indicesCamposBoundField = new int[] { 2, 3, 4 };
            //    }

            //    camposGrafica = new string[] { "Tipo de destino", "Total", "link" };
            //}

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Sitios: " + ldt.Rows.Count.ToString();
                }

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                ldt.Columns["Total"].ColumnName = "Total";

                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                if (DSODataContext.Schema.ToLower() == "k5banorte")
                {
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                }
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                      formatoColumna, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 7 }, indicesCamposBoundField, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDest2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorTDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorTDestGraf_G", "", "", "Tipo de destino", campoAGraficar, grafActiva, FCGpoGraf.Tabular, numberPrefix), false);

            #endregion Grafica
        }
        private void RepTabPorTDestLlamadasPnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                                Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorTipoDestino(linkGrafica, omitirInfoCDR, omitirInfoSiana));


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            string[] formatoColumna;
            int[] indicesCamposBoundField;

            camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Numero", "Duracion", "link" };
            formatoColumna = new string[] { "", "", "{0:0,0}", "{0:0,0}" };
            indicesCamposBoundField = new int[] { 2, 3 };


            camposGrafica = new string[] { "Tipo de destino", "Cantidad llamadas", "link" };


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                      formatoColumna, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 6 }, indicesCamposBoundField, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDest2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorTDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorTDestGraf_G", "", "", "Tipo de destino", "Llamadas", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        private void RepTabPorCenCosJer1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorCenCosJerarquico("[link] = ''" + Request.Path +
                                                                                            "?Nav=CenCosJerN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Nombre Centro de Costos", "Total", "TotLlamadas", "Duracion Minutos", "link" });
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";         //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepConsCenCosJerarGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=CenCosJerN2&CenCos={0}",
                                new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepTabPorCenCosJer1Pnl", tituloGrid, Request.Path + "?Nav=CenCosJerN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de costos", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCosJer1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCenCosJer1Pnl", "", "", "Centro de costos", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorCenCosJer2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorCenCosJerarquico(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Nombre Centro de Costos", "Total", "TotLlamadas", "Duracion Minutos", "link" });
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";         //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsCenCosJerarGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepTabPorCenCosJer2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de costos", "Total", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosJerarGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsCenCosJerarGraf_G", "", "", "Centro de costos", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorEmpleMasCaros1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string nombreColumnaColaborador = "Colaborador";
            int numeroRegistros = 10;
            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();
            string[] formatos = new string[] { };
            int[] colNoNavegacion = new int[] { };
            int[] colsNoMostrar = new int[] { };

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                nombreColumnaColaborador = "Línea";
            }
            else { nombreColumnaColaborador = "Colaborador"; }

            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaPorEmpleMasCaros("[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])",
                10));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                string[] columnas = new string[] { };
                if (DSODataContext.Schema.ToLower() == "institutomora")
                {
                    columnas = new string[] { "Codigo Empleado", "Extension", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" };
                    //ldt.Columns["Extension"].ColumnName = "Extensión";
                }
                else if (DSODataContext.Schema.ToLower() == "fca")
                {
                    columnas = new string[] { "Codigo Empleado", "Nombre Completo", "Total", "Numero", "Duracion", "link" };
                    formatos = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" };
                    colNoNavegacion = new int[] { 2, 3, 4 };
                }
                else
                {
                    columnas = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" };
                    formatos = new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" };
                    colNoNavegacion = new int[] { 2, 3, 4, 5 };
                }

                ldt = dvldt.ToTable(false, columnas);

                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;

                if (DSODataContext.Schema.ToLower() != "fca")
                {
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                }
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            if (DSODataContext.Schema.ToLower() == "institutomora")
            {
                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                        DTIChartsAndControls.GridView(
                        "RepConsEmpleMasCarosGrid", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", numeroRegistros), true, "Totales",
                        new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                        new string[] { "Codigo Empleado" }, 1, new int[] { 0 },
                        new int[] { 1, 3, 4, 5, 6 }, new int[] { 2 }), "RepTabPorEmpleMasCaros1Pnl", tituloGrid, Request.Path + "?Nav=EmpleMCN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                );
            }
            else
            {

                contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView(
                            "RepConsEmpleMasCarosGrid", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", numeroRegistros), true, "Totales",
                           formatos, Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                            new string[] { "Codigo Empleado" }, 1, new int[] { 0 },
                            colNoNavegacion, new int[] { 1 }), "RepTabPorEmpleMasCaros1Pnl", tituloGrid, Request.Path + "?Nav=EmpleMCN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                );
            }



            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { nombreColumnaColaborador, "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmpleMasCaros1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", numeroRegistros)),
                "RepTabPorEmpleMasCaros1Pnl", "", "", nombreColumnaColaborador, "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica

        }

        private void RepTabPorEmpleMasCaros2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                     Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorEmpleMasCaros(linkGrafica, int.MaxValue, omitirInfoCDR, omitirInfoSiana));

            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
            {
                campoOrdenamiento = "Total desc";
                campoAGraficar = "Total";
                numberPrefix = "$ ";
            }
            else
            {
                campoOrdenamiento = "Cantidad llamadas desc";
                campoAGraficar = "Cantidad llamadas";
                numberPrefix = " ";
            }

            string nombreColumnaColaborador = "Colaborador";
            string vchCodigoPerfilUsuario =
                DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                nombreColumnaColaborador = "Línea";
            }
            else
            {
                nombreColumnaColaborador = "Colaborador";
            }


            int hyperLinkFieldIndex = 1;
            string[] camposReporte = new string[] { };
            int[] camposBoundField = new int[] { };
            string[] camposGrafica = new string[] { };

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
                    //camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" };
                    //camposBoundField = new int[] { 1, 2, 3, 4, 5 };

                    if (DSODataContext.Schema.ToLower() == "institutomora")
                    {

                        camposReporte = new string[] { "Codigo Empleado", "Extension", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" };
                        camposBoundField = new int[] { 1, 2, 3, 4, 5, 6 };

                    }
                    else if (DSODataContext.Schema.ToLower() == "fca")
                    {
                        camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Total", "Numero", "Duracion" };

                        if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                        {
                            camposBoundField = new int[] { 1, 2, 3, 4 };
                        }
                        else
                        {
                            camposBoundField = new int[] { 1, 3, 4 };
                        }
                    }
                    else
                    {

                        camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" };

                        if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                        {
                            camposBoundField = new int[] { 1, 2, 3, 4, 5 };
                        }
                        else
                        {
                            camposBoundField = new int[] { 1, 2, 4, 5 };
                        }

                    }
                }
                camposGrafica = new string[] { nombreColumnaColaborador, campoAGraficar };
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
                    if (DSODataContext.Schema.ToLower() == "institutomora")
                    {
                        hyperLinkFieldIndex = 2;
                        camposReporte = new string[] { "Codigo Empleado", "Extension", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" };
                        camposBoundField = new int[] { 1, 3, 4, 5, 6 };

                    }
                    else if (DSODataContext.Schema.ToLower() == "fca")
                    {
                        hyperLinkFieldIndex = 1;
                        camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Total", "Numero", "Duracion", "link" };

                        if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                        {
                            camposBoundField = new int[] { 2, 3, 4 };
                        }
                        else
                        {
                            camposBoundField = new int[] { 3, 4 };
                        }
                    }
                    else
                    {
                        hyperLinkFieldIndex = 1;
                        camposReporte = new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" };

                        if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                        {
                            camposBoundField = new int[] { 2, 3, 4, 5 };
                        }
                        else
                        {
                            camposBoundField = new int[] { 2, 4, 5 };
                        }
                    }

                }

                camposGrafica = new string[] { nombreColumnaColaborador, campoAGraficar, "link" };
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Colaboradores: " + ldt.Rows.Count.ToString();
                }


                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);

                //BG.20161124 Se agrega validacion de esquema para el campo nomina
                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                {
                    ldt.Columns["No Nomina"].ColumnName = "Nomina";
                }

                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                if (DSODataContext.Schema.ToLower() != "fca")
                {
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                }

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
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 8 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid)
                );
            }
            else
            {
                if (DSODataContext.Schema.ToLower() == "institutomora")
                {
                    contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid", ldt, true, "Totales",
                            new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                            new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                            camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid)
                    );
                }
                else if (DSODataContext.Schema.ToLower() == "fca")
                {
                    contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid", ldt, true, "Totales",
                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                    new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                    camposBoundField, new int[] { 1 }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid));
                }
                else
                {
                    contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid", ldt, true, "Totales",
                            new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                            new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                            camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2Pnls_T", tituloGrid));
                }


            }

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns[campoAGraficar].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasCaros_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCaros_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsEmpleMasCaros_G", "", "", nombreColumnaColaborador, campoAGraficar, grafActiva, FCGpoGraf.Tabular, numberPrefix), false);

            #endregion Grafica
        }

        private void RepTabPorTpLlam1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = null;
            string linkTable;

            if (DSODataContext.Schema.ToUpper() == "K5IPAB")
            {
                ldt = DSODataAccess.Execute(GeneraConsultaPorTpLlamada1Pnl("[link] = ''" + Request.Path + "?Nav=RepTabImporteEmplePorTpLlam&TipoLlam=" + "'' + convert(varchar,[Clave Tipo Llamada])"));
                linkTable = Request.Path + "?Nav=RepTabImporteEmplePorTpLlam&TipoLlam={0}";
            }
            else
            {
                ldt = DSODataAccess.Execute(GeneraConsultaPorTpLlamada1Pnl("[link] = ''" + Request.Path + "?Nav=TpLlamN2&TipoLlam=" + "'' + convert(varchar,[Clave Tipo Llamada])"));
                linkTable = Request.Path + "?Nav=TpLlamN2&TipoLlam={0}";
            }


            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Clave Tipo Llamada", "Tipo Llamada", "Total", "link" });
                ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView(
                                "RepPorTpLlamGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "" }, linkTable,
                                new string[] { "Clave Tipo Llamada" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "RepTabPorTpLlam1Pnl", tituloGrid, Request.Path + "?Nav=TpLlamN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo de llamada", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de llamada"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de llamada"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorTpLlam1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorTpLlam1Pnl", "", "", "Tipo de llamada", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorTpLlam2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = null;
            string linkTable;
            if (DSODataContext.Schema.ToUpper() == "K5IPAB")
            {
                ldt = DSODataAccess.Execute(GeneraConsultaPorTpLlamada1Pnl("[link] = ''" + Request.Path + "?Nav=RepTabImporteEmplePorTpLlam&TipoLlam=" + "'' + convert(varchar,[Clave Tipo Llamada])"));
                linkTable = Request.Path + "?Nav=RepTabImporteEmplePorTpLlam&TipoLlam={0}";
            }
            else
            {
                ldt = DSODataAccess.Execute(GeneraConsultaPorTpLlamada1Pnl("[link] = ''" + Request.Path + "?Nav=TpLlamN2&TipoLlam=" + "'' + convert(varchar,[Clave Tipo Llamada])"));
                linkTable = Request.Path + "?Nav=TpLlamN2&TipoLlam={0}";
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Clave Tipo Llamada", "Tipo Llamada", "Total", "link" });
                ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }



            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                "RepPorTpLlamGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "" }, linkTable,
                                new string[] { "Clave Tipo Llamada" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "RepTabPorTpLlam2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo de llamada", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de llamada"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de llamada"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepPorTpLlamGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepPorTpLlamGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepPorTpLlamGraf_G", "", "", "Tipo de llamada", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorSitio1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorSitioMat("[link] = ''" + Request.Path + "?Nav=SitioN2&Sitio=" + "'' + convert(varchar,[Codigo Sitio])"));

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            List<int> boundFields = new List<int>();
            for (int i = 2; i < ldt.Columns.Count - 1; i++)
            {
                boundFields.Add(i);
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["TotalCosto"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(
                                    "RepPorSitioMatGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    formatStrings.ToArray(), Request.Path + "?Nav=SitioN2&Sitio={0}",
                                    new string[] { "Codigo Sitio" }, 1, new int[] { 0 }, boundFields.ToArray(), new int[] { 1 }),
                                    "RepMatPorSitio1Pnl", tituloGrid, Request.Path + "?Nav=SitioMatN1", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorSitio1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorSitio1Pnl", "", "", "Sitio", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorSitio2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorSitioMat(linkGrafica));

            #region Grid

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            List<int> boundFields = new List<int>();
            for (int i = 2; i < ldt.Columns.Count - 1; i++)
            {
                boundFields.Add(i);
            }

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["TotalCosto"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepPorSitioMatGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    formatStrings.ToArray(), linkGrid,
                                    new string[] { "Codigo Sitio" }, 1, new int[] { 0 },
                                    boundFields.ToArray(), new int[] { 1 }), "RepMatPorSitio2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepPorSitioMatGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepPorSitioMatGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepPorSitioMatGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorCenCos1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorCenCos("[link] = ''" + Request.Path + "?Nav=CenCosN2&CenCos='' + convert(varchar,[Codigo Centro de Costos])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Centro de Costos", "Total", "Llamadas", "Minutos", "link" });

                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepConsCenCosGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=CenCosN2&CenCos={0}",
                                new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepTabPorCenCos1Pnl", tituloGrid, Request.Path + "?Nav=CenCosN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false, new string[] { "Centro de costos", "Total", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCos1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCenCos1Pnl", "", "", "Centro de costos", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorCenCos2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                       Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorCenCos(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Centro de Costos", "Total", "Llamadas", "Minutos", "link" });

                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsCenCosGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepTabPorCenCos2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false, new string[] { "Centro de costos", "Total", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorCenCos2Pnls_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCos2Pnls_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCenCos2Pnls_G", "", "", "Centro de costos", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabPorSitio2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                               Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorSitio(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Codigo Sitio", "Sitio", "Total", "Llamadas", "Minutos", "link" });

                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsSitioGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepTabPorSitio2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false, new string[] { "Sitio", "Total", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorSitio2Pnls_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorSitio2Pnls_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorSitio2Pnls_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorCenCosLlamadasPnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                       Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorCenCos(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Centro de Costos", "Llamadas", "Minutos", "link" });

                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsCenCosGrid", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }),
                                "RepTabPorCenCos2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false, new string[] { "Centro de costos", "Cantidad llamadas", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de costos"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorCenCos2Pnls_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorCenCos2Pnls_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorCenCos2Pnls_G", "", "", "Centro de costos", "Llamadas", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorCarrier1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorCarrierMat("[link] = ''" + Request.Path + "?Nav=RepPorCarrierN2&Carrier=" + "'' + convert(varchar,[Codigo Carrier])"));

            #region Grid

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            List<int> boundFields = new List<int>();
            for (int i = 2; i < ldt.Columns.Count - 1; i++)
            {
                boundFields.Add(i);
            }

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                ldt.Columns["TotalCosto"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView(
                                    "RepPorCarrierMatGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    formatStrings.ToArray(), Request.Path + "?Nav=RepPorCarrierN2&Carrier={0}",
                                    new string[] { "Codigo Carrier" }, 1, new int[] { 0 }, boundFields.ToArray(), new int[] { 1 }),
                                    "RepMatPorCarrier1Pnl", tituloGrid, Request.Path + "?Nav=RepPorCarrierN1", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Carrier", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepMatPorCarrier1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepMatPorCarrier1Pnl", "", "", "Carrier", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepMatPorCarrier2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaConsumoPorCarrierMat(linkGrafica));

            #region Grid

            RemoveColHerencia(ref ldt);

            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            formatStrings.Add("");
            for (int i = 1; i < ldt.Columns.Count; i++)
            {
                formatStrings.Add("{0:c}");
            }

            List<int> boundFields = new List<int>();
            for (int i = 2; i < ldt.Columns.Count - 1; i++)
            {
                boundFields.Add(i);
            }

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                ldt.Columns["TotalCosto"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepPorCarrierMatGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    formatStrings.ToArray(), linkGrid,
                                    new string[] { "Codigo Carrier" }, 1, new int[] { 0 },
                                    boundFields.ToArray(), new int[] { 1 }), "RepMatPorCarrier2Pnls_T", tituloGrid)
                    );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Carrier", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepPorCarrierMatGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepPorCarrierMatGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepPorCarrierMatGraf_G", "", "", "Carrier", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorDiaDeSemana1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistPorDiaDeLaSemana());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "DiaSemana", "Total" });
                ldt.Columns["DiaSemana"].ColumnName = "Día";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepHistPorDiaDeLaSemanaGrid", ldt, true, "Totales",
                                new string[] { "", "{0:c}" }),
                                "RepTabPorDiaDeSemana1Pnl", tituloGrid, Request.Path + "?Nav=RepHistPorDiaSemN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Día", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Día"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Día"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorDiaDeSemana1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabPorDiaDeSemana1Pnl", "", "", "Día", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorDiaDeSemana2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepHistPorDiaDeLaSemana());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "DiaSemana", "Total" });
                ldt.Columns["DiaSemana"].ColumnName = "Día";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepHistPorDiaDeLaSemanaGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}" }),
                                "RepTabPorDiaDeSemana2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Día", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Día"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Día"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepHistPorDiaDeLaSemanaGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistPorDiaDeLaSemanaGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "RepHistPorDiaDeLaSemanaGraf_G", "", "", "Día", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorConsSimuSitiosTDest2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepConsumoSimuladoSitiosDestino(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Sitio Origen", "Nombre Sitio Origen", "Total2", "Duracion", "Llamadas", "link" });
                ldt.Columns["Nombre Sitio Origen"].ColumnName = "Sitio";
                ldt.Columns["Total2"].ColumnName = "Total";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";    //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepConsSimuladoSitiosDestGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                    new string[] { "Codigo Sitio Origen" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorConsSimuSitiosTDest2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsSimuladoSitiosDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsSimuladoSitiosDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsSimuladoSitiosDestGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorConsSimuTDest2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepConsumoSimuladoDestino(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Sitio Destino", "Nombre Sitio Destino", "Total", "Duracion Minutos", "TotLlamadas", "link" });
                ldt.Columns["Nombre Sitio Destino"].ColumnName = "Destino";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";         //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepConsSimuladoDestGrid_T", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                    new string[] { "Codigo Sitio Destino" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4 }, new int[] { 1 }), "RepTabPorConsSimuTDest2Pnls_T", tituloGrid)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Destino", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsSimuladoDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsSimuladoDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsSimuladoDestGraf_G", "", "", "Destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabHist3Anios1Pnl(Control contenedor, string tituloGrid, string tituloGrafica, int pestaniaActiva)
        {
            //NZEste metodo no lo toquen
            DataTable ldt = DSODataAccess.Execute(ConsultaRepHistorico3Anios());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Mes", "2 Años atras", "Año anterior", "Año actual" });
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepHistorico3AniosGrid", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                                "RepTabHist3Anios1Pnl", tituloGrid, Request.Path + "?Nav=Hist3AniosN1", pestaniaActiva, FCGpoGraf.Matricial)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabHist3Anios1Pnl",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), "RepTabHist3Anios1Pnl",
                tituloGrafica, "", "Mes", "Importe", pestaniaActiva, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }

        private void RepTabHist3Anios2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaRepHistorico3Anios());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Mes", "2 Años atras", "Año anterior", "Año actual" });
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                             DTIChartsAndControls.GridView("RepHistorico3AniosGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:c}" }),
                             "RepTabHist3Anios2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();
            }



            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepHistorico3AniosGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            //contenedorGrafica.Controls.Add(DTIChartsAndControls.tituloYBordesReporte(new LiteralControl(
            //    FCAndControls.CreaContenedorGraficaYRadioButtonsGrafMultiSerie(
            //    "RepHistorico3AniosGraf_G", "ControlesAlCentro", tipoGrafDefault)), "Gráfica", tituloGrafica, 0));

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepHistorico3AniosGraf_G",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt), tituloGrafica,
                "", "", "Mes", "Importe", grafActiva, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }

        private void RepTabConsColaboradores1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaReporteColaboradores());

            #region Grid

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                    DTIChartsAndControls.GridView("RepTabConsColaboradoresGrid", ldt, true, "Totales",
                    new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }),
                    "RepTabConsColaboradores1Pnl", tituloGrid, Request.Path + "?Nav=ConColaboradoresN1", pestaniaActiva, FCGpoGraf.Tabular)
            );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
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

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsColaboradores1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsColaboradores1Pnl", "", "", "Colaborador", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsColaboradores2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaReporteColaboradores());

            #region Grid

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabConsColaboradoresGrid_T", ldt, true, "Totales",
                    new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }),
                    "RepTabConsColaboradores2Pnls_T", tituloGrid)

            );




            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
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

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabConsColaboradoresGraf_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsEntreSitiosGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsEntreSitiosGraf_G", "", "", "Colaborador", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTopAreaPrs1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaAreasMayorConsumo("[link] = ''" + Request.Path + "?Nav=TopAreasN2&CenCos='' + convert(varchar,[idArea])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepTopArea = new DataView(ldt);
                ldt = dvRepTopArea.ToTable(false,
                    new string[] { "idArea", "Area", "Importe", "Porcentaje", "link" });
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepTopAreaPrs", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:p}" }, Request.Path + "?Nav=TopAreasN2&CenCos={0}",
                                new string[] { "idArea" }, 1, new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }),
                                "RepTopAreaPrs1Pnl", tituloGrid, Request.Path + "?Nav=TopAreasN1", pestaniaActiva, FCGpoGraf.Tabular)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepTopArea = new DataView(ldt);
                ldt = dvRepTopArea.ToTable(false, new string[] { "Area", "Importe", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Area"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Area"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTopAreaPrs1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTopAreaPrs1Pnl", "", "", "Area", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTopAreaPrs2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaAreasMayorConsumo(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepTopArea = new DataView(ldt);
                ldt = dvRepTopArea.ToTable(false,
                    new string[] { "idArea", "Area", "Importe", "Porcentaje", "link" });
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTopAreaPrs_T", ldt, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:p}" }, linkGrid,
                                new string[] { "idArea" }, 1, new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }),
                                "RepTopAreaPrs2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepTopArea = new DataView(ldt);
                ldt = dvRepTopArea.ToTable(false, new string[] { "Area", "Importe", "link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Area"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Area"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTopAreaGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTopAreaGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTopAreaGraf_G", "", "", "Area", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabDetalleLlamsF4Telcel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleLlamsTelcelF4());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Fecha", "Hora", "Minutos", "NumMarcado", "LugarOrigen", "LugarLlamado", "TipoLLamada", "Importe" });
                ldt.Columns["NumMarcado"].ColumnName = "Numero Marcado";
                ldt.Columns["LugarOrigen"].ColumnName = "Lugar Origen";
                ldt.Columns["LugarLlamado"].ColumnName = "Lugar Llamado";
                ldt.Columns["TipoLLamada"].ColumnName = "Tipo de LLamada";
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                    "RepDetalleLlamsTelcelF4_T", ldt, false, "Totales",
                    new string[] { "", "", "{0:0,0}", "", "", "", "", "{0:c}", "" })
                    , "RepTabDetalleLlamsF4Telcel1Pnl_T", "Detallado de Llamadas Telcel F4"));


            #endregion Grid

        }

        private ArrayList FormatoColumRepTabTipoDestinoPrsN1(DataTable ldt, byte nombreTotalReal, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 5, 9 };
                    columnasVisibles = new int[] { 4, 6, 7, 8 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 4, 9 };
                    columnasVisibles = new int[] { 5, 6, 7, 8 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";  /*BG. Costo = Real */
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 3, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 2, 7, 8 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                    nombreTotalReal = 2; //Si vale el TotalSimulado Se cambio por Total (Titulo)
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 3, 7, 8 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";        /*BG. Costo = Real */
                    nombreTotalReal = 3; //Si vale el TotalReal Se cambio por Total (Titulo)
                }
            }

            #endregion Logica de las columnas a mostrar

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(nombreTotalReal);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private void RepTabTipoDestinoPrs2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestPrs(
                "link = ''" + Request.Path + "?Nav=RepTDestPrsN3&Carrier=" + param["Carrier"] + "&TDest=" + "'' + convert(varchar,[Codigo Tipo Destino])"
                ));

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "TotalSimulado",
                    "TotalReal", "CostoSimulado", "CostoReal", "SM", "Numero", "Duracion", "link" });


                //ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                ArrayList lista = FormatoColumRepTabTipoDestinoPrsN1(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabTipoDestinoPrsGrid_T", ldt, true, "Totales",
                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepTDestPrsN3&Carrier=" + param["Carrier"] + "&TDest={0}",
                                      new string[] { "Codigo Tipo Destino" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                      "RepTabTipoDestinoPrs2Pnls_T", tituloGrid)
           );

            #endregion Grid


            //BG.20150612
            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }  //BG.20150612


            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }

                ldt = dvldt.ToTable(false,
                    new string[] { "Tipo de destino", "Importe", "link" });
                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }



            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepTabTipoDestinoPrsGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTipoDestinoPrsGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTipoDestinoPrsGraf_G", "", "", "Tipo de destino", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private ArrayList FormatoColumRepTabTipoDestinoPrsN2(DataTable ldt, byte nombreTotalReal, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            ldt.Columns["SM"].ColumnName = "Servicio Medido";

            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 5, 9 };
                    columnasVisibles = new int[] { 4, 6, 7, 8 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 3, 4, 9 };
                    columnasVisibles = new int[] { 5, 6, 7, 8 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";  /*BG. Costo = Real */
                }
            }
            else
            {
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1) /*BG. CostoFac = Simulado */
                {
                    columnasNoVisibles = new int[] { 0, 3, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 2, 7, 8 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                    nombreTotalReal = 1; //Si vale el TotalSimulado Se cambio por Total (Titulo)
                }
                else
                {
                    columnasNoVisibles = new int[] { 0, 2, 4, 5, 6, 9 };
                    columnasVisibles = new int[] { 3, 7, 8 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";        /*BG. Costo = Real */
                    nombreTotalReal = 2; //Si vale el TotalReal Se cambio por Total (Titulo)
                }
            }

            #endregion Logica de las columnas a mostrar

            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(nombreTotalReal);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private void RepTabTipoDestinoPrs1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestPrsN2("" + Request.Path + "?Nav="));

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                RemoveColHerencia(ref ldt);

                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Carrier", "Nombre Carrier", "TotalSimulado",
                    "TotalReal", "CostoSimulado", "CostoReal", "SM", "Numero", "Duracion", "link" });


                //ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                ArrayList lista = FormatoColumRepTabTipoDestinoPrsN2(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                    DTIChartsAndControls.GridView("RepTabTipoDestinoPrsGrid", ldt, true, "Totales",
                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                      new string[] { "link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                      "RepTabTipoDestinoPrs1Pnl", tituloGrid, Request.Path + "?Nav=RepTDestPrsN1", pestaniaActiva, FCGpoGraf.Tabular)
            );

            #endregion Grid

            //BG.20150612
            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }  //BG.20150612


            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }


                ldt = dvldt.ToTable(false,
                    new string[] { "Carrier", "Importe", "link" });
                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTipoDestinoPrs1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTipoDestinoPrs1Pnl", "", "", "Carrier", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabTipoDestinoPrs2PnlsNiv2(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestPrsN2(
                "" + Request.Path + "?Nav="));

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                RemoveColHerencia(ref ldt);

                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Carrier", "Nombre Carrier", "TotalSimulado",
                    "TotalReal", "CostoSimulado", "CostoReal", "SM", "Numero", "Duracion", "link" });

                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                ArrayList lista = FormatoColumRepTabTipoDestinoPrsN2(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }



            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTipoDestinoPrsGrid_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                     new string[] { "link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                     "RepTabTipoDestinoPrs2PnlsNiv2_T", tituloGrid)
           );

            #endregion Grid

            //BG.20150629
            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }  //BG.20150629


            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }


                ldt = dvldt.ToTable(false,
                    new string[] { "Carrier", "Importe", "link" });
                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns["Carrier"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabTipoDestinoPrsGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTipoDestinoPrsGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTipoDestinoPrsGraf_G", "", "", "Carrier", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabTipoDestinoPrsLD2PnlsNiv2(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestLDPrsN2(
                "link = ''" + Request.Path + "?Nav=RepTDestPrsLDN2&Carrier=" + param["Carrier"] + "&TDest=" + "'' + convert(varchar,[Codigo Tipo Destino])"
                ));


            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;


            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);
                RemoveColHerencia(ref ldt);

                ldt = dvldt.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "TotalSimulado",
                "TotalReal", "CostoSimulado", "CostoReal", "SM", "Numero", "Duracion", "link" });

                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de Destino";
                //ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                ArrayList lista = FormatoColumRepTabTipoDestinoPrsN1(ldt, nombreTotalReal, columnasNoVisibles, columnasVisibles);
                ldt = (DataTable)((object)lista[0]);
                nombreTotalReal = (byte)((object)lista[1]);
                columnasNoVisibles = (int[])((object)lista[2]);
                columnasVisibles = (int[])((object)lista[3]);

                //ldt.AcceptChanges();
            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTipoDestinoPrsGrid_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepTDestPrsLDN3&TDest={0}&Carrier=" + param["Carrier"],
                                     new string[] { "Codigo Tipo Destino" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                     "RepTabTipoDestinoPrsLD2PnlsNiv2_T", tituloGrid)
           );

            #endregion Grid

            //BG.20150612
            if (nombreTotalReal == 1)
            {
                ldt.Columns["Total"].ColumnName = "TotalSimulado";
            }
            else if (nombreTotalReal == 2)
            {
                ldt.Columns["Total"].ColumnName = "TotalReal";
            }  //BG.20150612


            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                {
                    ldt.Columns["TotalSimulado"].ColumnName = "Importe";
                }
                else { ldt.Columns["TotalReal"].ColumnName = "Importe"; }


                ldt = dvldt.ToTable(false, new string[] { "Tipo de Destino", "Total", "link" });
                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns["Tipo de Destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabTipoDestinoPrsGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTipoDestinoPrsGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTipoDestinoPrsGraf_G", "", "", "Tipo de Destino", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabTipoDestinoPrs2PnlsNiv3(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestCencosPrsN3(
                 "link = ''" + Request.Path + "?Nav=RepTDestPrsN4&TDest=" + param["TDest"] + "&Carrier=" + param["Carrier"] + "&CenCos=" + "'' + convert(varchar,[Codigo Centro de Costos])"
                ));

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Centro de Costos", "Nombre Centro de Costos", "TotImporte", "LLamadas", "TotMin", "link" });
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                ldt.Columns["TotImporte"].ColumnName = "Total";
                ldt.Columns["LLamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["TotMin"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            #region Grid


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTipoDestinoPrsGrid_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepTDestPrsN4&TDest=" + param["TDest"] + "&Carrier=" + param["Carrier"] + "&CenCos={0}",
                                     new string[] { "Codigo Centro de Costos" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                     "RepTabTipoDestinoPrs2PnlsNiv3_T", tituloGrid)
           );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Centro de Costos", "Total", "link" });
                //Se valida si el DataTable tiene fila de totales (Si es "true" entonces elimina esa fila)
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                //Se cambia el nombre de las columnas del DataTable porque asi lo necesita el metodo que genera las graficas
                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabTipoDestinoPrsGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabTipoDestinoPrsGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabTipoDestinoPrsGraf_G", "", "", "Centro de Costos", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        /* BG.20170216 REORTE DETALLADO ENLACES ORANGE */
        private void RepTabTipoDestinoPrsOrange1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlOrangePrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Empleado", "CenCos", "Importe" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlOrange_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}" }), "RepTabTipoDestinoPrsOrange1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        /* BG.20170216 REORTE DETALLADO ENLACES UNINET COLOMBIA */
        private void RepTabTipoDestinoPrsUninet1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlUninetCOLPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Cia", "Cuenta", "C.C", "Proyecto", "Producto", "Temporal", "Debito Valor", "Credito Valor", "Descripcion" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlUninetCol_T", ldt, true, "Totales",
                   new string[] { "", "", "", "", "", "", "{0:c}", "{0:c}", "" }), "RepTabTipoDestinoPrsUninet1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        /* BG.20170216 REORTE DETALLADO ENLACES BESTEL */
        private void RepTabTipoDestinoPrsBestel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlBestelPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Concepto", "Importe" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlBestel_T", ldt, true, "Totales",
                   new string[] { "", "{0:c}" }), "RepTabTipoDestinoPrsBestel1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20160908 REPORTE RENTAS TELMEX. Se agregan 4 tipos de destinos (Renta, Enlaces, Uninet, 800E) los cuales mostraran el detalle de la factura de Telmex
        private void RepTabTipoDestinoPrsDetallRenta1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleRentasPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "CtaTelmex", "CenCos", "Descripcion", "Folio", "Tipo", "Costo" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallRenta_T", ldt, true, "Totales",
                   new string[] { "", "", "", "", "", "{0:c}" }),
                                "RepTabTipoDestinoPrsDetallRenta1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20160912 REPORTE ENLACES TELMEX. Se agregan 4 tipos de destinos (Renta, Enlaces, Uninet, 800E) los cuales mostraran el detalle de la factura de Telmex
        private void RepTabTipoDestinoPrsDetallEnlaces1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlacesPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "CtaTelmex", "CenCos", "Descripcion", "Folio", "Comercio", "Costo" });
                ldt.AcceptChanges();
            }

            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlace_T", ldt, true, "Totales",
                   new string[] { "", "", "", "", "", "{0:c}" }),
                                "RepTabTipoDestinoPrsDetallEnlaces1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20160912 REPORTE UNINET TELMEX. Se agregan 4 tipos de destinos (Renta, Enlaces, Uninet, 800E) los cuales mostraran el detalle de la factura de Telmex
        private void RepTabTipoDestinoPrsDetallUninet1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleUninetPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Identificacion de Servicios", "LeyendaCobro", "Importe", "Comercio" });
                ldt.Columns["LeyendaCobro"].ColumnName = "Leyenda Cobro";
                ldt.AcceptChanges();
            }


            #region Grid


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallUninet_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}", "" }),
                                "RepTabTipoDestinoPrsDetallUninet1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20160912 REPORTE 800E TELMEX. Se agregan 4 tipos de destinos (Renta, Enlaces, Uninet, 800E) los cuales mostraran el detalle de la factura de Telmex
        private void RepTabTipoDestinoPrsDetall800E1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalle800EntPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Cuenta", "LadaTelefono", "No800", "Comercio", "Cencos", "Llamadas", "Minutos", "Costo" });
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetall800E_T", ldt, true, "Totales",
                   new string[] { "", "", "", "", "", "", "", "{0:c}" }),
                                "RepTabTipoDestinoPrsDetall800E1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20161004 REPORTE ENLACES AXTEL. Se agrega 1 TDest el cual se mostrará el detalle de la factura de AXTEL
        private void RepTabTipoDestinoPrsDetallEnlAxtel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlAxtelPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Banco", "Referencia", "Descripcion", "Costo" });
                ldt.Columns["Banco"].ColumnName = "Banco/Comercio";
                ldt.Columns["Costo"].ColumnName = "Costo Facturado";

                ldt.AcceptChanges();
            }

            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlAxtel_T", ldt, true, "Totales",
                   new string[] { "", "", "", "{0:c}", "", "", "" }),
                                "RepTabTipoDestinoPrsDetallEnlAxtel1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20161004 REPORTE ENLACES AVANTEL. Se agrega 1 TDest el cual se mostrará el detalle de la factura de AVANTEL
        private void RepTabTipoDestinoPrsDetallEnlAvantel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlAvantelPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Banco", "Referencia", "CostoFact", "CC" });
                ldt.Columns["Banco"].ColumnName = "Banco/Comercio";
                ldt.Columns["CostoFact"].ColumnName = "Costo Facturado";

                ldt.AcceptChanges();
            }


            #region Grid


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlAvantel_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}", "", "", "", "" }),
                                "RepTabTipoDestinoPrsDetallEnlAvantel1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20170116 REPORTE LLAMADAS 800E AVANTEL. Se agrega 1 TDest el cual se mostrará el detalle de la factura de AVANTEL
        private void RepTabTipoDestinoPrsDetall800EAvantel1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalle800EAvantelPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Cuenta", "NumDestino", "NumOrigen", "FechaInicio", "DuracionMin", "Importe", "CdDestino" });
                ldt.Columns["NumDestino"].ColumnName = "Numero Destino";
                ldt.Columns["NumOrigen"].ColumnName = "Numero Origen";
                ldt.Columns["FechaInicio"].ColumnName = "Fecha";
                ldt.Columns["Importe"].ColumnName = "Monto Total Con Desc.";
                ldt.Columns["CdDestino"].ColumnName = "Cd. Destino";

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetall800EAvantel_T", ldt, true, "Totales",
                   new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "" }),
                                "RepTabTipoDestinoPrsDetall800EAvantel1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20170216 REPORTE RENTAS TELNOR. Los cuales mostraran el detalle de la factura de TELNOR
        private void RepTabTipoDestinoPrsDetallRentaTelnor1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleRentasTelnorPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });

                ldt.AcceptChanges();
            }


            #region Grid


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallRentaTelnor_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}" }), "RepTabTipoDestinoPrsDetallRentaTelnor1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20170216 REPORTE SM TELNOR. Los cuales mostraran el detalle de la factura de TELNOR
        private void RepTabTipoDestinoPrsDetallSMTelnor1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleSMTelnorPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallSMTelnor_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}" }),
                                "RepTabTipoDestinoPrsDetallSMTelnor1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG. 20170216 REPORTE ENLACES TELNOR. Los cuales mostraran el detalle de la factura de TELNOR
        private void RepTabTipoDestinoPrsDetallEnlTelnor1Pnl(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlTelnorPrs());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });

                ldt.AcceptChanges();
            }


            #region Grid

            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabTDestPrsDetallEnlTelnor_T", ldt, true, "Totales",
                   new string[] { "", "", "{0:c}" }), "RepTabTipoDestinoPrsDetallEnlTelnor1Pnl_T", tituloGrid)
           );

            #endregion Grid
        }

        //BG.20151026 SE CREA METODO PARA EL REPORTE DE CONSUMO POR CAMPAÑAS PROSA
        private void RepTabConsumoPorCampaniaPrs1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepPrsConsumoPorCampania());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Campaña", "Importe", "Llamadas", "Minutos" });
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("RepPrsConsumosPorCampania", ldt, true, "Total",
                                    new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }, "", new string[] { }, 0, new int[] { },
                                    new int[] { 0, 1, 2, 3 }, new int[] { }),
                                    "RepTabConsumoPorCampaniaPrs1Pnl", tituloGrid, Request.Path + "?Nav=RepPrsConsumosPorCampaniaN1", pestaniaActiva, FCGpoGraf.Tabular)
                    );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Campaña", "Importe" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Campaña"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Campaña"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsumoPorCampaniaPrs1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsumoPorCampaniaPrs1Pnl", "", "", "Campaña", "Total", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsumoPorCampaniaPrs2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepPrsConsumoPorCampania());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                     new string[] { "Campaña", "Importe", "Llamadas", "Minutos" });
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepPrsConsumosPorCampania_T", ldt, true, "Totales",
                                    new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}" }, "",
                                    new string[] { "" }, 0, new int[] { },
                                    new int[] { 0, 1, 2, 3 }, new int[] { }), "RepTabConsumoPorCampaniaPrs2Pnls_T", tituloGrid)
                    );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Campaña", "Importe" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Campaña"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Campaña"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepPrsConsumosPorCampaniaGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepPrsConsumosPorCampaniaGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepPrsConsumosPorCampaniaGraf_G", "", "", "Campaña", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabDetalladoEnlacesTelmex2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                               Control contenedorGrid, string tituloGrid)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalladoEnlacesTelmex());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                      new string[] { "Folio", "PuntaA", "PuntaB", "Total" });
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView(
                                    "RepDetalladoEnlacesTelmex_T", ldt, true, "Total",
                                    new string[] { "", "", "", "{0:c}" }, "",
                                    new string[] { }, 0, new int[] { },
                                    new int[] { 0, 1, 2, 3 }, new int[] { }), "RepTabDetalladoEnlacesTelmex2Pnls_T", tituloGrid)
                    );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Folio", "Total" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Folio"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Folio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDetalladoEnlacesTelmexGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDetalladoEnlacesTelmexGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDetalladoEnlacesTelmexGraf_G", "", "", "Folio", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorTDestDashboard1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkGrid = Request.Path + "?Nav=TDestN2&TDest={0}";
            string linkGrafica = "[link] = ''" + Request.Path + "?Nav=TDestN2&TDest='' + convert(varchar,[Codigo Tipo Destino])";

            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                linkGrid = string.Empty;
                linkGrafica = string.Empty;
            }

            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaReportePorTDestDashboard(linkGrid, linkGrafica));

            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            int[] indicesCamposBoundField;

            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                camposGrafica = new string[] { "Tipo de destino", "Total" };
                indicesCamposBoundField = new int[] { 1, 2, 3, 4 };
            }
            else
            {
                camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                camposGrafica = new string[] { "Tipo de destino", "Total", "link" };
                indicesCamposBoundField = new int[] { 2, 3, 4 };

            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");

            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 5 }, indicesCamposBoundField, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDestDashboard1Pnl", tituloGrid, Request.Path + "?Nav=TDestDashN1", pestaniaActiva, FCGpoGraf.Tabular)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorTDestDashboard1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "RepTabPorTDestDashboard1Pnl", "", "",
                "Tipo de destino", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorTDestDashboard2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaReportePorTDestDashboard(linkGrid, linkGrafica));


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;

            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                camposGrafica = new string[] { "Tipo de destino", "Total" };
            }
            else
            {
                camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                camposGrafica = new string[] { "Tipo de destino", "Total", "link" };
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid_T", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDestDashboard2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorTDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorTDestGraf_G", "", "", "Tipo de destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorEmpleMasCarosDash1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string nombreColumnaColaborador = "Colaborador";
            int numeroRegistros = 10;
            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                nombreColumnaColaborador = "Línea";
            }
            else
            {
                nombreColumnaColaborador = "Colaborador";
            }

            //Se obtiene el DataSource del reporte
            DataTable ldt = new DataTable();
            if (DSODataContext.Schema.ToString().ToUpper() == "BIMBO")
            {
                ldt = DSODataAccess.Execute(RepTabTopEmpleMasCaros(numeroRegistros));
            }
            else
            {

                ldt = DSODataAccess.Execute(ConsultaReporteTopNEmpleDashboard("" + Request.Path + "?Nav=EmpleMCN2&Emple=", "", numeroRegistros));
            }


            #region Grid
            string[] formatos = new string[] { };
            int[] columnas = new int[] { };

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (DSODataContext.Schema.ToString().ToUpper() == "BIMBO")
                {
                    ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion", "CostoFija", "CostoMovil", "Total", "link" });
                    ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                    ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                    ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                    ldt.Columns["CostoFija"].ColumnName = "Consumo Total de Telefonía Fija";
                    ldt.Columns["CostoMovil"].ColumnName = "Consumo Total de Telefonía Móvil";
                    ldt.Columns["Total"].ColumnName = "Total";

                    formatos = new string[] { "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "" };
                    columnas = new int[] { 2, 3, 4, 5, 6, 7 };
                }
                else
                {
                    ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion", "Total", "link" });
                    ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                    ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                    ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                    ldt.Columns["Total"].ColumnName = "Total";

                    formatos = new string[] { "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}", "" };
                    columnas = new int[] { 2, 3, 4, 5 };
                }

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                ldt.AcceptChanges();
            }




            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView(
                            "RepConsEmpleMasCarosGrid", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", numeroRegistros), true, "Totales",
                            formatos, Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                            new string[] { "Codigo Empleado" }, 1, new int[] { 0 }, columnas, new int[] { 1 }),
                            "RepTabPorEmpleMasCarosDash1Pnl", tituloGrid, Request.Path + "?Nav=EmpleMCDashN1", pestaniaActiva, FCGpoGraf.Tabular)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { nombreColumnaColaborador, "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmpleMasCarosDash1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", numeroRegistros)),
                "RepTabPorEmpleMasCarosDash1Pnl", "", "", nombreColumnaColaborador, "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorEmpleMasCarosDash2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            int numeroRegistros = 10;
            DataTable ldt = new DataTable();

            if (DSODataContext.Schema.ToUpper() == "BIMBO")
            {
                ldt = DSODataAccess.Execute(RepTabTopEmpleMasCaros(numeroRegistros));
            }
            else
            {
                ldt = DSODataAccess.Execute(ConsultaReporteTopNEmpleDashboard(linkGrafica, "", numeroRegistros));
            }


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

            if (DSODataContext.Schema.ToUpper() == "BIMBO")
            {
                if (ldt != null && ldt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(ldt);
                    string[] formatos = new string[] { };
                    int[] columnas = new int[] { };

                    ldt = dvldt.ToTable(false,
                    new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion", "CostoFija", "CostoMovil", "Total", "link" });
                    ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                    ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                    ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                    ldt.Columns["CostoFija"].ColumnName = "Consumo Total de Telefonía Fija";
                    ldt.Columns["CostoMovil"].ColumnName = "Consumo Total de Telefonía Móvil";
                    ldt.Columns["Total"].ColumnName = "Total";

                    formatos = new string[] { "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "" };
                    columnas = new int[] { 2, 3, 4, 5, 6, 7 };


                    #region Grid
                    contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid_T", ldt, true, "Totales",
                                    formatos, linkGrid,
                                    new string[] { "Codigo Empleado" }, 1, new int[] { 0, 8 },
                                    columnas, new int[] { hyperLinkFieldIndex }), "Reporte", tituloGrid)
                    );


                    #endregion Grid

                    #region Grafica
                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                    {
                        camposGrafica = new string[] { nombreColumnaColaborador, "Total" };
                        DataView dvldt1 = new DataView(ldt);
                        ldt = dvldt1.ToTable(false, camposGrafica);

                        if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                        {
                            ldt.Rows[ldt.Rows.Count - 1].Delete();
                        }
                        ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                        ldt.Columns["Total"].ColumnName = "value";
                        ldt.AcceptChanges();
                    }
                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasCaros_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCaros_G",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                        "RepConsEmpleMasCaros_G", "", "", nombreColumnaColaborador, "Importe", grafActiva, FCGpoGraf.Tabular), false);

                    #endregion Grafica
                }

            }
            else
            {
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

                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
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
                                    camposBoundField, new int[] { hyperLinkFieldIndex }), "Reporte", tituloGrid)
                    );
                }
                else
                {

                    contenedorGrid.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                    DTIChartsAndControls.GridView("RepConsEmpleMasCarosGrid_T", ldt, true, "Totales",
                                    new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                    new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                                    camposBoundField, new int[] { hyperLinkFieldIndex }), "Reporte", tituloGrid)
                    );
                }

                #endregion Grid

                #region Grafica

                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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


                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasCaros_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCaros_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                    "RepConsEmpleMasCaros_G", "", "", nombreColumnaColaborador, "Importe", grafActiva, FCGpoGraf.Tabular), false);

                #endregion Grafica
            }
        }

        private void RepTabHistDash2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoDashboard(string.Empty, linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "Total", "link" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsHistGrid_T", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}" }, linkGrid,
                                      new string[] { "Mes Anio" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "RepTabHistDash2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Mes", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Orden asc");
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsHistGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHistGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "GrafConsHistGraf_G", "", "", "Mes", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabAccesosAgrupado(string linkGrid, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaAccesosAgrupado()); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvRepAccesosAgrupado = new DataView(ldt);
                ldt = dvRepAccesosAgrupado.ToTable(false,
                    new string[] { "Usuario", "Descripcion Usuario", "Cantidad Accesos" });
                ldt.Columns["Usuario"].ColumnName = "CuentaUsuario";
                ldt.Columns["Descripcion Usuario"].ColumnName = "Descripción";
                ldt.Columns["Cantidad Accesos"].ColumnName = "Cantidad de Accesos";


                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabAccesosAgrupado_T", ldt, false, "",
                                new string[] { "", "", "{0:0,0}" }, linkGrid,
                                new string[] { "CuentaUsuario" }, 2, new int[] { }, new int[] { 1, 2, 3 }, new int[] { 0 }),
                                "RepTabAccesosAgrupado_T", tituloGrid)
                );

            #endregion Grid
        }

        private void RepTabAccesosAgrupadoN2(string linkGrid, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaAccesosAgrupadoN2(param["Usuario"])); //(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {

                DataView dvRepAccesosAgrupado = new DataView(ldt);
                ldt = dvRepAccesosAgrupado.ToTable(false,
                    new string[] { "Usuario", "Descripcion Usuario", "Fecha Acceso" });
                ldt.Columns["Usuario"].ColumnName = "CuentaUsuario";
                ldt.Columns["Descripcion Usuario"].ColumnName = "Descripción";
                ldt.Columns["Fecha Acceso"].ColumnName = "Fecha Acceso";


                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabAccesosAgrupadoN2_T", ldt, false, "",
                                new string[] { "", "", "" }, linkGrid,
                                new string[] { }, 2, new int[] { }, new int[] { 0, 1, 2, 3 }, new int[] { }),
                                "RepTabAccesosAgrupadoN2_T", tituloGrid)
                );

            #endregion Grid
        }

        private void ReporteDetalladoConsumoSimulado(Control contenedor, string tituloGrid)
        {
            DataTable RepDetallado = DSODataAccess.Execute(ConsultaRepConsumoSimuladoDestinoDetalle());

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalladoGrid_T", RepDetallado, true, "Totales",
                                new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "" }),
                                "ReporteDetalladoConsumoSimulado_T", tituloGrid)
                );
        }

        private void RepTabDetalleParaFinanzasPrs1Pnls(Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleParaFinanzas());

            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "FechaInicio", "Numero Marcado", "Nombre Tipo Destino", "Extension", "Duracion" });
                ldt.Columns["FechaInicio"].ColumnName = "Fecha";
                ldt.Columns["Numero Marcado"].ColumnName = "Numero";
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Recurso";
                ldt.Columns["Extension"].ColumnName = "Clave";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "DuracionMinutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            #region Grid


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                   DTIChartsAndControls.GridView("RepTabDetalleParaFinanzasPrs1Pnls_T", ldt, false, "Totales",
                   new string[] { "", "", "", "", "{0:G}" }), "RepTabDetalleParaFinanzasPrs1Pnls_T", "Detalle para Finanzas"));

            #endregion Grid
        }

        private void ReporteDetallado(Control contenedor, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetallado", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleFija", "<script language=javascript> GetDatosTabla('ReporteDetallado', 'ReporteDetalladoWebM'); </script>", false);
        }

        [WebMethod]
        public static object ReporteDetalladoWebM()
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaDetalle());

                if (dt.Rows.Count > 0)
                {
                    int[] columnasNoVisibles = null;
                    int[] columnasVisibles = null;

                    DataView dvldt = new DataView(dt);
                    RemoveColHerencia(ref dt);

                    dt = DTIChartsAndControls.ordenaTabla(dt, "[TotalSimulado] desc");

                    ArrayList lista = FormatoColumRepDetallado(dt, columnasNoVisibles, columnasVisibles);
                    dt = (DataTable)((object)lista[0]);
                    columnasNoVisibles = (int[])((object)lista[1]);
                    columnasVisibles = (int[])((object)lista[2]);

                    //20150702 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                    CambiarNombresConfig(dt, "ConsultaDetalle");

                    if (dt.Columns.Contains("Duracion"))
                        dt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"             
                    if (dt.Columns.Contains("Llamadas"))
                        dt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";

                    for (int i = columnasNoVisibles.Length - 1; i >= 0; i--)
                    {
                        dt.Columns.RemoveAt(columnasNoVisibles[i]);
                    }

                    return FCAndControls.ConvertToJSONStringDetalle(dt);
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en Dashboard de Fija:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private static void CambiarNombresConfig(DataTable reporte, string nombreConsulta)
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

        private static ArrayList FormatoColumRepDetallado(DataTable ldt, int[] columnasNoVisibles, int[] columnasVisibles)
        {
            if (ldt.Columns.Contains("SM"))
                ldt.Columns["SM"].ColumnName = "Servicio Medido";


            #region Logica de las columnas a mostrar
            if (Convert.ToInt32(HttpContext.Current.Session["MuestraSM"]) == 1)
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 10, 11, 14 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 15, 16, 17, 18 };
                    ldt.Columns["CostoSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 10, 11, 12, 13 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 14, 15, 16, 17, 18 };
                    ldt.Columns["CostoReal"].ColumnName = "Total";
                }
            }
            else
            {
                if (Convert.ToInt32(HttpContext.Current.Session["MuestraCostoSimulado"]) == 1)
                {
                    columnasNoVisibles = new int[] { 10, 12, 14 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 15, 16, 17, 18 };
                    ldt.Columns["TotalSimulado"].ColumnName = "Total";
                }
                else
                {
                    columnasNoVisibles = new int[] { 9, 10, 13, 15, 16 };
                    columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 11, 12, 14, 17, 18 };
                    ldt.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            if (Convert.ToString(HttpContext.Current.Session["OcultarColumnImporte"]) == "1")
            {
                columnasNoVisibles = new int[] { 9, 10, 13, 14, 15, 16 };
                columnasVisibles = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 17, 18 };
            }




            #endregion Logica de las columnas a mostrar

            //NZ 20160823
            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {
                Array.Resize(ref columnasVisibles, columnasVisibles.Length + 1);
                columnasVisibles[columnasVisibles.Length - 1] = 18;
            }

            int omitirInfoCDR = 0;
            if (param["omitirInfoCDR"] != string.Empty)
            {
                omitirInfoCDR = Convert.ToInt32(param["omitirInfoCDR"]);
            }

            if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
            {
                if (omitirInfoCDR == 1)
                {
                    ///= new int[] { 7, 8, 9 };
                    Array.Clear(columnasNoVisibles, 0, 4);
                    columnasNoVisibles = new int[] { 7, 9, 10 };
                    Array.Resize(ref columnasVisibles, columnasVisibles.Length + 1);
                    columnasVisibles[columnasVisibles.Length - 1] = 13;
                }
                else
                {
                    Array.Clear(columnasNoVisibles, 0, 4);
                    columnasNoVisibles = new int[] { 7, 9, 10 };
                    Array.Resize(ref columnasVisibles, columnasVisibles.Length + 1);
                    columnasVisibles[columnasVisibles.Length - 1] = 13;
                }
            }



            ArrayList valores = new ArrayList();
            valores.Add(ldt);
            valores.Add(columnasNoVisibles);
            valores.Add(columnasVisibles);
            return valores;
        }

        private void ReportePorTipoDestinoPeEmple()
        {
            //Se obtiene el DataSource del reporte
            DataTable RepPorTDestPeEm = DSODataAccess.Execute(ConsultaPorTipoDestinoPeEm());

            if (RepPorTDestPeEm.Rows.Count > 0 && RepPorTDestPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepPorTDestPeEm);
                RepPorTDestPeEm = dvGrafConsHist.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                RepPorTDestPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo destino";
                RepPorTDestPeEm.Columns["Total"].ColumnName = "Total";
                RepPorTDestPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                RepPorTDestPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
            }



            Rep0.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("RepPorTDestPeEmGrid_T", RepPorTDestPeEm, true, "Totales",
                                      new string[] { "", "", "{0:c}", "", "" }, Request.Path + "?Nav=TDestPeEmN2&TDest={0}",
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                      "ReportePorTipoDestinoPeEmple_T", "Reporte por tipo de destino")
                      );
        }

        private void ReportePorNumMarcadoPeEmple(string link)
        {
            //Se obtiene el DataSource del reporte
            DataTable RepPorNumMarcPeEm = DSODataAccess.Execute(ConsultaPorNumMarcPeEm());
            int[] colVisibles = new int[] { };
            int[] colNonav = new int[] { };
            if (RepPorNumMarcPeEm.Rows.Count > 0 && RepPorNumMarcPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepPorNumMarcPeEm);

                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarc", "Clave Tipo Llamada", "Numero Marcado", "Etiqueta", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                    RepPorNumMarcPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                    colVisibles = new int[] { 0, 1 };
                    colNonav = new int[] { 3, 4, 5, 6, 7, 8 };
                }
                else
                {
                    if (DSODataContext.Schema.ToLower() != "bimbo")
                    {
                        RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarc", "Clave Tipo Llamada", "Numero Marcado", "Nombre Localidad", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                        colVisibles = new int[] { 0, 1 };
                        colNonav = new int[] { 3, 4, 5, 6, 7, 8 };
                        RepPorNumMarcPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                    }
                    else
                    {
                        RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarc", "Clave Tipo Llamada", "Numero Marcado", "Nombre Localidad", "Nombre Tipo Destino",
                                                                                                     "Total", "Numero", "Duracion"});
                        colVisibles = new int[] { 0, 1, 8 };
                        colNonav = new int[] { 3, 4, 5, 6, 7 };
                    }

                    RepPorNumMarcPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                }

                RepPorNumMarcPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                RepPorNumMarcPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                RepPorNumMarcPeEm.Columns["Total"].ColumnName = "Total";
                RepPorNumMarcPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";        //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                RepPorNumMarcPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";       //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"


            }


            Rep0.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("RepPorNumMarcPeEmGrid_T", RepPorNumMarcPeEm, true, "Totales",
                                      new string[] { "", "", "", "", "", "{0:c}", "", "", "" }, link,
                                      new string[] { "CodNumMarc", "Clave Tipo Llamada" }, 2, colVisibles, colNonav, new int[] { 2 }),
                                      "ReportePorNumMarcadoPeEmple_T", "Reporte por número marcado")
                      );
        }

        private void ReporteDetalladoPeEm()
        {
            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetallePeEm());

            if (RepDetallado.Rows.Count > 0 && RepDetallado.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepDetallado);
                if (DSODataContext.Schema.ToUpper() != "BIMBO")
                {
                    RepDetallado = dvGrafConsHist.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado",
                                                                                                  "Tipo Llamada", "Etiqueta" });
                    RepDetallado.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                    RepDetallado.Columns["Etiqueta"].ColumnName = "Etiqueta";
                }
                else
                {
                    RepDetallado = dvGrafConsHist.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado" });
                }


                RepDetallado.Columns["Extension"].ColumnName = "Extensión";
                RepDetallado.Columns["Fecha"].ColumnName = "Fecha";
                RepDetallado.Columns["Hora"].ColumnName = "Hora";
                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";            //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                RepDetallado.Columns["Total"].ColumnName = "Total";
                RepDetallado.Columns["Nombre Localidad"].ColumnName = "Localidad";
                RepDetallado.Columns["Numero Marcado"].ColumnName = "Número marcado";

            }

            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalladoGrid_T", RepDetallado, true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "", "", "", "" }), "ReporteDetalladoPeEm_T", "Detalle de llamadas ")
                );
        }

        private void ReportePorTipoLlamadaPeEm()
        {
            #region Reporte por tipo de llamada (Grafica)

            DataTable GrafPorTipoLlamPeEm = null;

            #region //20160722 NZ Se agrega esta seccion para cuando entren los esquemas de Evox, Banorte y K5Banorte
            if (DSODataContext.Schema.ToString().ToLower() == "evox"
                                                    || DSODataContext.Schema.ToString().ToLower() == "banorte"
                                                    || DSODataContext.Schema.ToString().ToLower() == "k5banorte")
            {
                GrafPorTipoLlamPeEm = DSODataAccess.Execute(consultaPorTipoLlamPeEmDetalleBanorte(
                "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));

                if (GrafPorTipoLlamPeEm.Rows.Count > 0)
                {
                    string gpoNoIdentificado = DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = '0NoIdent'").ToString();

                    foreach (DataRow row in GrafPorTipoLlamPeEm.Rows)
                    {
                        if (row["Clave Tipo Llamada"].ToString() == gpoNoIdentificado)
                        {
                            row["link"] = "/UserInterface/Historicos/Etiquetacion/EtiquetacionEmple.aspx";
                            break;
                        }
                    }
                }
            } //20160722 NZ
            else
            {
                //NZ 20160921
                GrafPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaPorTipoLlamPeEm(
                "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));
            }

            #endregion

            if (GrafPorTipoLlamPeEm.Rows.Count > 0 && GrafPorTipoLlamPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(GrafPorTipoLlamPeEm);
                GrafPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Tipo Llamada", "Total", "link" });
                GrafPorTipoLlamPeEm.Columns["Tipo Llamada"].ColumnName = "label";
                GrafPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";
                GrafPorTipoLlamPeEm.Columns["link"].ColumnName = "link";
            }


            Rep0.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafPorTipoLlamPeEm_G", "Consumo por tipo de llamada", 4, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPorTipoLlamPeEm_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPorTipoLlamPeEm), "GrafPorTipoLlamPeEm_G",
                 "Consumo por tipo de llamada", "", "Etiqueta", "Total", 4, FCGpoGraf.Tabular), false);

            #endregion Reporte por tipo de llamada (Grafica)
        }

        private void ReportePorNumMasCarosPeEm()
        {
            #region Reporte Numeros mas caros

            //Se obtiene el DataSource del reporte
            DataTable RepNumMasCarosPeEm = DSODataAccess.Execute(ConsultaNumerosMasCarosPeEm());
            int[] colNoVisibles = new int[] { };
            int[] colNav = new int[] { };
            if (RepNumMasCarosPeEm.Rows.Count > 0 && RepNumMasCarosPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepNumMasCarosPeEm);

                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Etiqueta",
                                                                                                               "Tipo Llamada", "Total", "Duracion","Numero" });

                }
                else
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Nombre Localidad",
                                                                                                               "Tipo Llamada", "Total", "Duracion","Numero" });

                    RepNumMasCarosPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                }

                RepNumMasCarosPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                RepNumMasCarosPeEm.Columns["Total"].ColumnName = "Total";
                RepNumMasCarosPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                RepNumMasCarosPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";       //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"

            }

            if (DSODataContext.Schema.ToLower() != "bimbo")
            {
                RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                colNoVisibles = new int[] { 0 };
                colNav = new int[] { 2, 3, 4, 5, 6 };
            }
            else
            {
                colNoVisibles = new int[] { 0, 3 };
                colNav = new int[] { 2, 4, 5, 6 };
            }

            Rep0.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("RepNumMasCarosPeEmGrid_T", RepNumMasCarosPeEm, true, "Totales",
                                      new string[] { "", "", "", "", "{0:c}", "", "" }, Request.Path + "?Nav=NumMasCarosN2&NumMarc={0}",
                                      new string[] { "CodNumMarcado" }, 1, colNoVisibles, colNav, new int[] { 1 }),
                                      "ReportePorNumMasCarosPeEm_T", "Reporte números mas caros")
                      );

            #endregion Reporte Numeros mas caros
        }

        private void ReporteHistoricoPeEm()
        {
            #region Reporte Historico

            //Se obtiene el DataSource del reporte
            DataTable GrafConsHist = DSODataAccess.Execute(ConsultaHistoricoPeEm());
            if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(GrafConsHist);
                GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                GrafConsHist.Columns["Total"].ColumnName = "Total";
                GrafConsHist.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("GrafConsHistGrid_T", GrafConsHist, true, "Totales", new string[] { "", "{0:c}" }),
                                "ReporteHistoricoPeEm_T", "Detalle de llamadas ")
                );

            if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
            {
                if (GrafConsHist.Rows[GrafConsHist.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    GrafConsHist.Rows[GrafConsHist.Rows.Count - 1].Delete();
                }
                GrafConsHist.Columns["Mes"].ColumnName = "label";
                GrafConsHist.Columns["Total"].ColumnName = "value";
                GrafConsHist.AcceptChanges();
            }

            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsHist_G", "Consumo historico", 0, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHist_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConsHist),
                "GrafConsHist_G", "Consumo historico", "", "Mes", "Importe", 0, FCGpoGraf.Tabular), false);

            #endregion Reporte Historico
        }

        private void ReporteExtenDondeSeUsoCodAutoPeEm()
        {
            #region Reporte Extensiones en las que se utilizo el codigo de llamadas

            DataTable RepExtenDondeUtilizoCodAut = DSODataAccess.Execute(ConsultaExtensionesEnLasQueSeUsoElCodAuto());

            if (RepExtenDondeUtilizoCodAut.Rows.Count > 0 && RepExtenDondeUtilizoCodAut.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepExtenDondeUtilizoCodAut);
                RepExtenDondeUtilizoCodAut = dvGrafConsHist.ToTable(false, new string[] { "Codigo Autorizacion", "Extension", "Llamadas" });
                RepExtenDondeUtilizoCodAut.Columns["Codigo Autorizacion"].ColumnName = "Código de autorización";
                RepExtenDondeUtilizoCodAut.Columns["Extension"].ColumnName = "Extensión";
                RepExtenDondeUtilizoCodAut.Columns["Llamadas"].ColumnName = "Llamadas";
            }

            Rep0.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepExtenDondeUtilizoCodAutGrid_T", RepExtenDondeUtilizoCodAut, true, "Totales",
                                new string[] { "", "", "" }), "ReporteExtenDondeSeUsoCodAutoPeEm_T", "Extensiones en las que se usó el código de llamadas ")
                );

            #endregion Reporte Extensiones en las que se utilizo el codigo de llamadas
        }

        private void ReporteConPorTipoLlamadaPeEm()
        {

            #region Reporte Consumo por tipo de llamada (Grafica)

            DataTable GrafConPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaConsumoPorTipoLlamadaPeEm());

            if (GrafConPorTipoLlamPeEm.Rows.Count > 0 && GrafConPorTipoLlamPeEm.Columns.Count > 0)
            {

                DataView dvGrafConsHist = new DataView(GrafConPorTipoLlamPeEm);
                GrafConPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Etiqueta", "Total", "llamadas", "minutos" });
                GrafConPorTipoLlamPeEm.Columns["llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "llamadas" por "Cantidad llamadas"
                GrafConPorTipoLlamPeEm.Columns["minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "minutos" por "Cantidad minutos"
                GrafConPorTipoLlamPeEm.Columns["etiqueta"].ColumnName = "label";
                GrafConPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";

            }

            Rep0.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConPorTipoLlamPeEm_G", "Consumo por tipo de llamada", 4, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConPorTipoLlamPeEm_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConPorTipoLlamPeEm), "GrafConPorTipoLlamPeEm_G",
                "Consumo por tipo de llamada", "", "", "", 4, FCGpoGraf.Tabular), false);

            #endregion Reporte Consumo por tipo de llamada (Grafica)

        }

        private void ReportePorLineas(Control Contenedor, string linkGrid, string tituloReporte, string lsCarrier)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", lsCarrier));
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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
                                     "RepTabTopLinMasCarasGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                     new string[] { "", "", "", "{0:c}" }, linkGrid,
                                     new string[] { "ExtensionCod" }, 1, new int[] { 0 },
                                     new int[] { 2, 3 }, new int[] { 1 }), "ReportePorLineas_T", tituloReporte)
                     );
        }

        private void ReporteXConceptoConNav(Control Contenedor, string linkGrid, string tituloReporte)
        {
            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
            if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
            {
                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                    new string[] { "idConcepto", "Concepto", "Total" });
                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                RepDetalleXConcepto.AcceptChanges();
            }


            Contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                "RepDetalleXConcepto_T", DTIChartsAndControls.ordenaTabla(RepDetalleXConcepto, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}" }, linkGrid,
                                new string[] { "idConcepto" }, 1, new int[] { 0 },
                                new int[] { 2 }, new int[] { 1 }), "ReporteXConceptoConNav_T", tituloReporte)
                );
        }

        private void ReporteDetFacturaXEmpleado()
        {
            #region Reporte Costo por Empleado

            DataTable RepDesgloseFacturaPorConcepto = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorConcepto());

            if (RepDesgloseFacturaPorConcepto.Rows.Count > 0 && RepDesgloseFacturaPorConcepto.Columns.Count > 0)
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
                                DTIChartsAndControls.GridView("RepDesgloseFacturaPorConcepto_T", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                true, "Totales", new string[] { "", "", "{0:c}" }), "ReporteDetFacturaXEmpleado_T", "Consumo desglosado por concepto ")
                    );
                }
                else
                {
                    DataTable RepDesgloseFacturaPorLlamada = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorLlamadas());

                    if (RepDesgloseFacturaPorLlamada.Rows.Count > 0 && RepDesgloseFacturaPorLlamada.Columns.Count > 0)
                    {
                        DataView dvRepDesgloseFacturaPorLlamada = new DataView(RepDesgloseFacturaPorLlamada);
                        RepDesgloseFacturaPorLlamada = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                            new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                        RepDesgloseFacturaPorLlamada.Columns["Fecha Llamada"].ColumnName = "Fecha";
                        RepDesgloseFacturaPorLlamada.Columns["Hora Llamada"].ColumnName = "Hora";
                        RepDesgloseFacturaPorLlamada.Columns["Numero Marcado"].ColumnName = "Número marcado";
                        RepDesgloseFacturaPorLlamada.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";        //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                        RepDesgloseFacturaPorLlamada.Columns["Costo"].ColumnName = "Total";
                        RepDesgloseFacturaPorLlamada.Columns["Punta A"].ColumnName = "Localidad Origen";
                        RepDesgloseFacturaPorLlamada.Columns["Dir Llamada"].ColumnName = "Tipo";
                        RepDesgloseFacturaPorLlamada.Columns["Punta B"].ColumnName = "Localidad Destino";
                        RepDesgloseFacturaPorLlamada.AcceptChanges();
                    }

                    Rep0.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                           DTIChartsAndControls.GridView("RepDesgloseFacturaPorLlamada_T", DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                           true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "" }), "ReporteDetFacturaXEmpleado_T", "Detalle de llamadas Telcel sin filtrar claves cargo")
                    );

                }

                #endregion Seleccion de reporte
            }

            #endregion Reporte Costo por Empleado
        }

        private void ReporteDetalleFacturaNextel()
        {
            DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
            if (datosFactura.Rows.Count > 0)
            {
                #region Tiempo de uso

                DataView dvTiempoDeUso = new DataView(datosFactura);
                DataTable dtTiempoDeUso = dvTiempoDeUso.ToTable(false, new string[] { "Tiempo de uso", "Total" });
                dtTiempoDeUso = dtTiempoDeUso.Select("Total is not null").CopyToDataTable();



                Rep1.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTiempoDeUsoNextel_T", dtTiempoDeUso,
                                false, "", new string[] { "", "{0:0,0}" }), "ReporteDetalleFacturaNextel1_T", "Tiempo de uso")
                    );

                #endregion Tiempo de uso

                #region Desglose

                DataView dvDesglose = new DataView(datosFactura);
                DataTable dtDesglose = dvDesglose.ToTable(false, new string[] { "Desglose", "Importe" });

                Rep2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesgloseNextel", dtDesglose,
                                true, "Total", new string[] { "", "{0:c}" }), "ReporteDetalleFacturaNextel2_T", "Desglose")
                    );

                #endregion Desglose
            }
        }

        private void RepTabPorEmpleMasCarosConSitio1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string nombreColumnaColaborador = "Colaborador";
            string vchCodigoPerfilUsuario = DSODataAccess.ExecuteScalar("select vchcodigo from " + DSODataContext.Schema + ".[VisHistoricos('Perfil','Perfiles','Español')] where dtinivigencia<>dtfinvigencia and dtfinvigencia>=getdate() and icodcatalogo = " + Session["iCodPerfil"].ToString()).ToString().ToLower();

            if (vchCodigoPerfilUsuario == "adminsucursal") //Este perfil fue diseñado inicialmente para Bimbo para que mostrara el consumo de Telmex solamente
            {
                nombreColumnaColaborador = "Línea";
            }
            else { nombreColumnaColaborador = "Colaborador"; }

            DataTable ldt = DSODataAccess.Execute(ConsultaPorEmpleMasCarosConSitio("[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                new string[] { "Codigo Empleado", "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                ldt.Columns["ExtensionEmple"].ColumnName = "Extensión";
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
           DTIChartsAndControls.TituloYPestañasRep1Nvl(
                           DTIChartsAndControls.GridView(
                           "RepConsEmpleMasCarosGrid", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), true, "Totales",
                           new string[] { "", "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                           new string[] { "Codigo Empleado" }, 1, new int[] { 0 }, new int[] { 2, 3, 4, 5, 6, 7 }, new int[] { 1 }),
                           "RepTabPorEmpleMasCarosConSitio1Pnl", tituloGrid, Request.Path + "?Nav=EmpleMCConSitioN1", pestaniaActiva, FCGpoGraf.Tabular)
           );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { nombreColumnaColaborador, "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmpleMasCarosConSitio1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmpleMasCarosConSitio1Pnl", "", "", nombreColumnaColaborador, "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabPorEmpleMasCarosConSitio2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorEmpleMasCarosConSitio(linkGrafica));

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
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink
                //NZ 20160823
                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                {
                    camposReporte = new string[] { "Codigo Empleado", "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion", "Puesto" };
                    camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                }
                else
                {
                    camposReporte = new string[] { "Codigo Empleado", "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion" };
                    camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7 };
                }

                camposGrafica = new string[] { nombreColumnaColaborador, "Total" };
            }
            else
            {
                hyperLinkFieldIndex = 1;
                //NZ 20160823
                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                {
                    camposReporte = new string[] { "Codigo Empleado", "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion", "Puesto", "link" };
                    camposBoundField = new int[] { 2, 3, 4, 5, 6, 7, 8 };
                }
                else
                {
                    camposReporte = new string[] { "Codigo Empleado", "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion", "link" };
                    camposBoundField = new int[] { 2, 3, 4, 5, 6, 7 };
                }

                camposGrafica = new string[] { nombreColumnaColaborador, "Total", "link" };
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                ldt.Columns["ExtensionEmple"].ColumnName = "Extensión";
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
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosConSitioGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 2, new int[] { 0 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCarosConSitio2Pnls_T", tituloGrid)
                );
            }
            else
            {
                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosConSitioGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 2, new int[] { 0 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCarosConSitio2Pnls_T", tituloGrid)
                );
            }

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasCarosConSitio_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasCarosConSitio_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsEmpleMasCarosConSitio_G", "", "", nombreColumnaColaborador, "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabCenCosJerarquicoP1(
            Control contenedor, string tituloGrid, int pestaniaActiva, string tituloGrafica,
            int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable dtCenCos = DSODataAccess.Execute("SELECT CenCos FROM " + DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','Español')]" +
                " WHERE dtIniVigencia <> dtFinVigencia AND dtfinvigencia >= getdate() AND iCodCatalogo=" + Session["iCodUsuario"].ToString() + " AND CenCos IS NOT NULL");

            int idCenCos = (dtCenCos.Rows.Count == 0) ? 0 : Convert.ToInt32(dtCenCos.Rows[0][0]);

            DataTable ldt = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=",
                Request.Path + "?Nav=CenCosJerarquicoN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", idCenCos, omitirInfoCDR, omitirInfoSiana)); //N2 es para navegar a la pagina de CenCos y el N3 para el reporte por empleado.

            if (idCenCos == 0)
            {
                tituloGrid = tituloGrid + "\n : EL USUARIO NO TIENE CONFIGURADO UN CENTRO DE COSTOS SUPERIOR";
            }

            #region Grid

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Plazas: " + ldt.Rows.Count.ToString();
                }

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

            if (param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoSoloCDRN1" || param["Nav"] == "CenCosJerarquicoSoloSianaN1")   // a dos paneles
            {
                Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                "RepTabCenCosJerarquicoP1_T", tituloGrid));
            }
            else // a un panel
            {
                if (omitirInfoCDR == 0 && omitirInfoSiana == 0)
                {
                    contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                        DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                        new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                        new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                        "RepTabCenCosJerarquicoP1", tituloGrid, Request.Path + "?Nav=CenCosJerarquicoN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                        );
                }
                else
                {
                    //Quiere decir que sí debe filtrar o bien CDR o bien Siana, se irá al reporte especial que lo hace
                    if (omitirInfoCDR == 1)
                    {
                        contenedor.Controls.Add(
                                DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                                DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                                new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                                new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                                "RepTabCenCosJerarquicoP1", tituloGrid, Request.Path + "?Nav=CenCosJerarquicoSoloCDRN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                                );
                    }
                    else
                    {
                        contenedor.Controls.Add(
                                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                                    DTIChartsAndControls.GridView("RepConsCenCosJerarGrid2", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                                    new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, "{0}",
                                                    new string[] { "Link" }, 1, columnasNoVisibles, columnasVisibles, new int[] { 1 }),
                                                    "RepTabCenCosJerarquicoP1", tituloGrid, Request.Path + "?Nav=CenCosJerarquicoSoloSianaN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                                    );
                    }
                }
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

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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

            if (param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoSoloCDRN1" || param["Nav"] == "CenCosJerarquicoSoloSianaN1") // a dos paneles
            {
                Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabCenCosJerarquicoP1_G", tituloGrafica, pestaniaActiva, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCenCosJerarquicoP1_G",
               FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
               "RepTabCenCosJerarquicoP1_G", "", "", "Centro de costos", "Importe", pestaniaActiva, FCGpoGraf.Tabular), false);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCenCosJerarquicoP1",
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

        private void RepTabCenCosJerarquicoN2(int grafActiva, string tituloGrid, string tituloGrafica,
            int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=",
                    Request.Path + "?Nav=CenCosJerarquicoN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", Convert.ToInt32(param["CenCos"]), omitirInfoCDR, omitirInfoSiana));

            #region Grid
            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;
            byte nombreTotalReal = 0;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() == "SEVENELEVEN")
                {
                    tituloGrid += "<br>Total de Centro de Costos: " + ldt.Rows.Count.ToString();
                }

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

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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
            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosJerarGraf2_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf2_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                    "RepConsCenCosJerarGraf2_G", "", "", "Centro de costos", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabResumenSpeedDials2PnlsN1(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepTabResumenSpeedDial(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = dvldt.ToTable(false,
                new string[] { "Sitio", "SitioDesc", "Numero Marcado", "Costo", "Llamadas", "Minutos", "link" });

                ldt.Columns["Sitio"].ColumnName = "idSitio";
                ldt.Columns["SitioDesc"].ColumnName = "Sitio";
                ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                ldt.Columns["Costo"].ColumnName = "Total";
                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC, Cantidad llamadas DESC ");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView(
                            "RepTabResumenSpeedDialGrid_T", ldt, true, "Totales",
                            new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                            new string[] { "Número Marcado", "idSitio" }, 2, new int[] { 0, 6 },
                            new int[] { 1, 3, 4, 5 }, new int[] { 2 }), "RepTabResumenSpeedDials2PnlsN1_T", tituloGrid)
            );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Número Marcado", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Número Marcado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt = DTIChartsAndControls.selectTopNTabla(ldt, "Total DESC", 10);

                ldt.Columns["Número Marcado"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Rep2.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsCenCosJerarGraf2_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsCenCosJerarGraf2_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                    "RepConsCenCosJerarGraf2_G", "", "", "Centro de costos", "Importe", grafActiva, FCGpoGraf.Tabular), false);



            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabResumenSpeedDialGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabResumenSpeedDialGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt),
                "RepTabResumenSpeedDialGraf_G", "", "", "Número Marcado", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

            CrearBotonExportDirectorioSpeedDial();
        }

        private void RepMatConsumoPorCampañaTipoDest1Pnl(Control contenedorGrid, string tituloGrid)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaRepMatConsumoPorCampañaTipoDest());

            #region Grid

            /* Lista con los formatos por Columna */
            List<string> formatStrings = new List<string>();
            formatStrings.Add("");
            formatStrings.Add("");
            formatStrings.Add("");
            formatStrings.Add("");
            formatStrings.Add("");
            for (int i = 5; i < ldt.Columns.Count; i++)
            {
                if (ldt.Columns[i].ColumnName.Contains("Total"))
                { formatStrings.Add("{0:c}"); }
                else
                { formatStrings.Add("{0:0,0}"); }
            }


            /* Campos que NO tienen Navegación */
            List<int> boundFields = new List<int>();
            for (int i = 0; i < ldt.Columns.Count; i++)
            {
                if (i != 0 && i != 1 && i != 3)
                {
                    boundFields.Add(i);
                }

            }


            contenedorGrid.Controls.Add(
                 DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                     DTIChartsAndControls.GridView("RepMatConsumoPorCampañaTipoDest_T", ldt, true, "Totales",
                     formatStrings.ToArray(), "",
                     new string[] { }, 4, new int[] { 0, 1, 3 },
                     boundFields.ToArray(), new int[] { }), "RepMatConsumoPorCampañaTipoDest1Pnl_T", tituloGrid)
                     );


            #endregion Grid
        }

        private void RepTabTraficoProHora2Pnl(Control contenedorDDL, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            DataTable original = DSODataAccess.Execute(ConsultaReporteTraficoPorHora());
            DataTable ldt = original.Clone();
            ldt.Columns[1].DataType = typeof(string);

            foreach (DataRow row in original.Rows)
            {
                ldt.ImportRow(row);
            }

            original.Clear();


            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SitioDesc", "Hora", "CantLlamadas" });

                ldt.Columns["SitioDesc"].ColumnName = "Sitios";
                ldt.Columns["Hora"].ColumnName = "Hora";
                ldt.Columns["CantLlamadas"].ColumnName = "Cantidad Llamadas";

                ldt.AcceptChanges();
            }

            #region DDL

            DataTable dtSitios = DSODataAccess.Execute(ConsultaBuscaSitios());

            var lblSitio = new Label();
            lblSitio.Text = "sitio: ";

            var ddl = new DropDownList();

            if (dtSitios.Rows.Count > 0)
            {
                ddl.DataSource = dtSitios;
                ddl.DataValueField = "iCodCatalogo";
                ddl.DataTextField = "vchDescripcion";
                ddl.DataBind();
            }

            ddl.ID = "ddlSitiosRepTabTraficoPorHora";
            ddl.AutoPostBack = true;
            ddl.Style.Add("marging", "20px");
            ddl.SelectedValue = ((param["Sitio"] != null && param["Sitio"].Length > 0) ? param["Sitio"] : "0");
            ddl.SelectedIndexChanged += FiltraPorSitioRepTabTraficoPorHora_Click;


            var pnl = new Panel();
            pnl.Controls.Add(lblSitio);
            pnl.Controls.Add(ddl);

            contenedorDDL.Controls.Add(pnl);

            #endregion


            #region Grid

            contenedorGrid.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                DTIChartsAndControls.GridView("RepTabTraficoPorHora_T", ldt, true, "Totales", new string[] { "", "", "{0:0,0}" },
                    "", new string[] { "" }, 0, new int[] { }, new int[] { 0, 1, 2 }, new int[] { }), "RepTabTraficoProHora2Pnl_T", tituloGrid));

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitios", "Hora", "Cantidad Llamadas" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Sitios"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Hora"].ColumnName = "label";
                ldt.Columns["Cantidad Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add
                (DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GraficaRepTabTraficoPorHora_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript
                (this.GetType(), "GraficaRepTabTraficoPorHora_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GraficaRepTabTraficoPorHora_G", "", "", "Hora", "Cant. Llamadas",
                    grafActiva, FCGpoGraf.Tabular, "", "", "dti", "98%", "385"), false);

            #endregion Grafica
        }

        protected void FiltraPorSitioRepTabTraficoPorHora_Click(object sender, EventArgs e)
        {
            var ddl = sender as DropDownList;
            int iCodCatSitio = Convert.ToInt32(ddl.SelectedValue.ToString());
            Page.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?Nav=RepTabTraficoPorHora2Pnl&Sitio=" + iCodCatSitio + "");
        }

        private void ReporteLlamadasBuzonDeVoz1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtLlamsBuzonVoz = DSODataAccess.Execute(RepTabLlamadasBuzonDeVoz());

            if (dtLlamsBuzonVoz.Rows.Count > 0 && dtLlamsBuzonVoz.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtLlamsBuzonVoz);
                dtLlamsBuzonVoz = dvldt.ToTable(false, new string[] { "Fecha", "Segundos", "Minutos", "LlamsBuzon" });
                dtLlamsBuzonVoz.Columns["LlamsBuzon"].ColumnName = "Llamadas en Buzón";
                dtLlamsBuzonVoz.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamadasBuzonDeVozGrid_T", dtLlamsBuzonVoz, true, "Totales",
                                new string[] { "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }),
                                "ReporteLlamadasBuzonDeVoz1Pnl_T", tituloGrid)
                );
        }

        private void ReporteDetalleLlamsBuzonDeVoz1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtDetalleBuzonVoz = DSODataAccess.Execute(RepTabDetalleLlamsBuzonDeVoz());

            if (dtDetalleBuzonVoz.Rows.Count > 0 && dtDetalleBuzonVoz.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtDetalleBuzonVoz);

                string cliente = DSODataContext.Schema;

                if (cliente.ToUpper() == "SWISSHOSPITAL")
                {
                    dtDetalleBuzonVoz = dvldt.ToTable(false, new string[] { "Fecha", "Hora", "Duracion", "NumMarcado", "Extension", "ExtensionIntermedia", "ExtensionInicial", "Buzon", });

                    dtDetalleBuzonVoz.Columns["ExtensionIntermedia"].ColumnName = "Extension Intermedia";
                    dtDetalleBuzonVoz.Columns["ExtensionInicial"].ColumnName = "Extension Inicial";
                    dtDetalleBuzonVoz.Columns["Extension"].ColumnName = "Extensión Final";
                }
                else
                {
                    dtDetalleBuzonVoz = dvldt.ToTable(false, new string[] { "Fecha", "Hora", "Duracion", "NumMarcado", "Extension", "Buzon" });
                    dtDetalleBuzonVoz.Columns["Extension"].ColumnName = "Extensión";
                }

                dtDetalleBuzonVoz.Columns["Duracion"].ColumnName = "Duración";
                dtDetalleBuzonVoz.Columns["NumMarcado"].ColumnName = "Número Marcado";
                dtDetalleBuzonVoz.Columns["Buzon"].ColumnName = "Buzón";
                dtDetalleBuzonVoz.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalleLlamsBuzonDeVozGrid_T", dtDetalleBuzonVoz, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "", "", "", "", "" }),
                                "ReporteDetalleLlamsBuzonDeVoz1Pnl_T", tituloGrid)
                );
        }

        private void RepEstTabCodEnMasDeNExtensiones1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepEstTabCodEnMasDeNExtensiones());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Nombre Completo", "No Nomina", "Total", "Llamadas", "Duracion Minutos", "CantCod", "Nombre Centro de Costos", "Nombre Sitio", "Codigo Sitio" });
                dtReporte.Columns["Codigo Autorizacion"].ColumnName = "Código";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Responsable";
                dtReporte.Columns["No Nomina"].ColumnName = "Nómina";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["CantCod"].ColumnName = "Núm. de Extensiones";
                dtReporte.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte.AcceptChanges();


            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepEstTabCodEnMasDeNExtensionesGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepEstTabCodEnMasDeNExtensionesN2&Codigo={0}&Sitio={1}",
                                new string[] { "Código", "Codigo Sitio" }, 0,
                                new int[] { 9 }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new int[] { 0 }),
                                "RepEstTabCodEnMasDeNExtensiones1Pnl_T", tituloGrid)
                );




        }

        private void RepEstTabCodEnMasDeNExtensionesN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepEstTabCodEnMasDeNExtensionesN2());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false,
                    new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total de minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepEstTabCodEnMasDeNExtensionesN2Grid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "{0:0,0}", "{0:c}", "", "", "", "" }),
                                "RepEstTabCodEnMasDeNExtensionesN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatDiasProcesados1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatDiasProcesados());
            List<string> formatoCols = new List<string>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i > 0) ? "{0:0,0}" : string.Empty);

                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Fecha Asc");
                dtReporte.AcceptChanges();
            }



            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatDiasProcesadosGrid_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepMatDiasProcesados1Pnl_T", tituloGrid)
                );
        }

        private void RepTabCodigosNoAsignados1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabCodigosNoAsignados());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio" });
                dtReporte.Columns["Codigo Autorizacion"].ColumnName = "Código de Autorización";
                dtReporte.Columns["Total"].ColumnName = "Importe";
                dtReporte.Columns["Numero"].ColumnName = "Total de llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de minutos";
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabCodigosNoAsignadosGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepTabCodigosNoAsignadosN2&Codigo={0}&Sitio={1}",
                                new string[] { "Código de Autorización", "Codigo Sitio" }, 0,
                                new int[] { 5 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabCodigosNoAsignados1Pnl_T", tituloGrid)
                );
        }

        private void RepTabCodigosNoAsignados2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabCodigosNoAsignados());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio", "link" });
                ldt.Columns["Codigo Autorizacion"].ColumnName = "Código de Autorización";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabCodigosNoAsignadosGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepTabCodigosNoAsignadosN2&Codigo={0}&Sitio={1}",
                                new string[] { "Código de Autorización", "Codigo Sitio" }, 0,
                                new int[] { 5, 6 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabCodigosNoAsignados2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Código de Autorización", "Importe", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Código de Autorización"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Código de Autorización"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabCodigosNoAsignadosGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabCodigosNoAsignadosGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabCodigosNoAsignadosGraf_G", "", "", "Código de Autorización", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabCodigosNoAsignadosN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabCodigosNoAsignadosN2());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Codigo Autorizacion", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabCodigosNoAsignadosN2Grid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "", "", "" }),
                                "RepTabCodigosNoAsignadosN21Pnl_T", tituloGrid)
                );
        }

        private void RepTabExtensionesNoAsignadas1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabExtensionesNoAsignadas());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Total"].ColumnName = "Importe";
                dtReporte.Columns["Numero"].ColumnName = "Total de llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de minutos";
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabExtensionesNoAsignadasGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepTabExtensionesNoAsignadasN2&Extension={0}&Sitio={1}",
                                new string[] { "Extensión", "Codigo Sitio" }, 0,
                                new int[] { 5 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabExtensionesNoAsignadas1Pnl_T", tituloGrid)
                );
        }

        private void RepTabExtensionesNoAsignadas2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabExtensionesNoAsignadas());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio", "link" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }



            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabExtensionesNoAsignadasGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepTabExtensionesNoAsignadasN2&Extension={0}&Sitio={1}",
                                new string[] { "Extensión", "Codigo Sitio" }, 0,
                                new int[] { 5, 6 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabExtensionesNoAsignadas2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Importe", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabExtensionesNoAsignadasGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabExtensionesNoAsignadasGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabExtensionesNoAsignadasGraf_G", "", "", "Extensión", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabExtensionesNoAsignadasN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabExtensionesNoAsignadasN2());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Codigo Autorizacion", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabExtensionesNoAsignadasN2Grid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "", "", "" }),
                                "RepTabExtensionesNoAsignadasN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCarrier1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCarrier());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCarrierGrid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorCarrierN2&Carrier={0}",
                                new string[] { "Codigo Carrier" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "RepMatConsumoPorCarrier1Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCarrierN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCarrierN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            string link = (DSODataContext.Schema.ToLower() == "fca") ? Request.Path + "?Nav=RepMatConsumoPorCarrierN4&Carrier=" + param["Carrier"] + "&Sitio={0}" : Request.Path + "?Nav=RepMatConsumoPorCarrierN3&Carrier=" + param["Carrier"] + "&Sitio={0}";
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCarrierN2Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                link,
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "RepMatConsumoPorCarrierN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCarrierN31Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCarrierN3());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCarrierN3Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorCarrierN4&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Cencos={0}",
                                new string[] { "Codigo Centro de Costos" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorCarrierN31Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCarrierN41Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCarrierN4());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            string link = (DSODataContext.Schema.ToLower() == "fca") ? Request.Path + "?Nav=RepMatConsumoPorCarrierN5&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Emple={0}" : Request.Path + "?Nav=RepMatConsumoPorCarrierN5&Carrier=" + param["Carrier"] + "&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple={0}";
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCarrierN4Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                link,
                                new string[] { "Codigo Emple" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorCarrierN41Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorEmple1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorEmple());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    if (DSODataContext.Schema.ToString().ToLower() == "institutomora")
                    {
                        if (i == 1)
                        {
                            boundfieldCols.Add(i);
                        }

                        formatoCols.Add((i < 6) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                        if (i > 5)
                        {
                            boundfieldCols.Add(i);
                        }
                    }
                    else
                    {
                        formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency
                        if (i > 4)
                        {
                            boundfieldCols.Add(i);
                        }
                    }


                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            int[] col = null;
            int colTotales;
            int campNav;
            if (DSODataContext.Schema.ToString().ToLower() == "institutomora")
            {
                col = new int[] { 0, 2, 3, 4 };
                colTotales = 1;
                campNav = 5;
            }
            else
            {
                col = new int[] { 0, 1, 2, 3 };
                colTotales = 4;
                campNav = 4;
            }
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorEmpleGrid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorEmpleN2&Emple={0}",
                                new string[] { "Codigo Emple" }, colTotales,
                                col, boundfieldCols.ToArray(), new int[] { campNav }),
                                "RepMatConsumoPorEmple1Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorEmpleN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorEmpleN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorEmpleN2Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorEmpleN3&Emple=" + param["Emple"] + "&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "RepMatConsumoPorEmpleN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorSitio1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorSitio());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }
            string link = "";
            if (DSODataContext.Schema.ToLower() == "fca")
            {
                link = Request.Path + "?Nav=RepMatConsumoPorSitioN3&Sitio={0}";
            }
            else
            {
                link = Request.Path + "?Nav=RepMatConsumoPorSitioN2&Sitio={0}";
            }
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorSitioN2Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                link,
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "RepMatConsumoPorSitio1Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorSitioN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorSitioN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorSitioN2Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorSitioN3&Sitio=" + param["Sitio"] + "&Cencos={0}",
                                new string[] { "Codigo Centro de Costos" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorSitioN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorSitioN31Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorSitioN3());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }
            string lnk = "";
            if (DSODataContext.Schema.ToLower() == "fca")
            {
                lnk = Request.Path + "?Nav=RepMatConsumoPorSitioN4&Sitio=" + param["Sitio"] + "&Emple={0}";
            }
            else
            {
                lnk = Request.Path + "?Nav=RepMatConsumoPorSitioN4&Sitio=" + param["Sitio"] + "&CenCos=" + param["CenCos"] + "&Emple={0}";
            }
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorSitioN3Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                lnk,
                                new string[] { "Codigo Emple" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorSitioN31Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCenCosSJ1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCenCosSJ());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCenCosSJGrid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorCenCosSJN2&Cencos={0}",
                                new string[] { "Codigo Centro de Costos" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorCenCosSJ1Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCenCosSJN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCenCosSJN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCenCosSJN2Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorCenCosSJN3&CenCos=" + param["CenCos"] + "&Emple={0}",
                                new string[] { "Codigo Emple" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatConsumoPorCenCosSJN21Pnl_T", tituloGrid)
                );
        }

        private void RepMatConsumoPorCenCosSJN31Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatConsumoPorCenCosSJN3());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatConsumoPorCenCosSJN3Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatConsumoPorCenCosSJN4&CenCos=" + param["CenCos"] + "&Emple=" + param["Emple"] + "&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "RepMatConsumoPorCenCosSJN31Pnl_T", tituloGrid)
                );
        }

        private void ReporteDetalladoCinvestav1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());

            int[] columnasNoVisibles = null;
            int[] columnasVisibles = null;

            if (RepDetallado.Rows.Count > 0 && RepDetallado.Columns.Count > 0)
            {
                DataView dvldt = new DataView(RepDetallado);
                if (RepDetallado.Columns.Contains("RID"))
                    RepDetallado.Columns.Remove("RID");
                if (RepDetallado.Columns.Contains("RowNumber"))
                    RepDetallado.Columns.Remove("RowNumber");
                if (RepDetallado.Columns.Contains("TopRID"))
                    RepDetallado.Columns.Remove("TopRID");
                if (RepDetallado.Columns.Contains("Centro de costos"))
                    RepDetallado.Columns.Remove("Centro de costos");
                if (RepDetallado.Columns.Contains("Llamadas"))
                    RepDetallado.Columns.Remove("Llamadas");
                if (RepDetallado.Columns.Contains("Carrier"))
                    RepDetallado.Columns.Remove("Carrier");
                if (RepDetallado.Columns.Contains("Codigo Autorizacion"))
                    RepDetallado.Columns.Remove("Codigo Autorizacion");
                if (RepDetallado.Columns.Contains("Localidad"))
                    RepDetallado.Columns.Remove("Localidad");

                RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                columnasVisibles = new int[] { 1, 0, 2, 3, 4, 5, 7, 12, 13 };
                columnasNoVisibles = new int[] { 6, 8, 9, 10, 11 };

                if (RepDetallado.Columns.Contains("Colaborador"))
                {
                    RepDetallado.Columns["Colaborador"].ColumnName = "Nombre";
                }

                if (RepDetallado.Columns.Contains("Tipo llamada"))
                {
                    RepDetallado.Columns["Tipo llamada"].ColumnName = "Tipo";
                }

                if (RepDetallado.Columns.Contains("Duracion"))
                {
                    RepDetallado.Columns["Duracion"].ColumnName = "Duración";
                }

                if (RepDetallado.Columns.Contains("Llamadas"))
                {
                    RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                }

                if (RepDetallado.Columns.Contains("Numero Marcado"))
                {
                    RepDetallado.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                }

                if (RepDetallado.Columns.Contains("Tipo de destino"))
                {
                    RepDetallado.Columns["Tipo de destino"].ColumnName = "Localización";
                }

                if (RepDetallado.Columns.Contains("TotalReal"))
                {
                    RepDetallado.Columns["TotalReal"].ColumnName = "Cargo";
                }



            }
            else
            {
                columnasNoVisibles = new int[1];
                columnasVisibles = new int[1];
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDetalladoGrid_T", RepDetallado, true, "Totales",
                                new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "", "", "" }, "",
                                new string[] { "Colaborador" }, 0, columnasNoVisibles, columnasVisibles, new int[] { }),
                                "ReporteDetalladoCinvestav1Pnl_T", tituloGrid));

        }

        private void ConsEmpsMasCarosN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCarosN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsEmpsMasCarosN3Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=ConsEmpsMasCarosN3&Emple=" + param["Emple"] + "&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "ConsEmpsMasCarosN21Pnl_T", tituloGrid)
                );
        }

        private void ConsumoEmpsMasTiempoN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsumoEmpsMasTiempoN2());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 3) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 2)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsumoEmpsMasTiempoN3Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=onsumoEmpsMasTiempoN3&Emple=" + param["Emple"] + "&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 2,
                                new int[] { 0, 1 }, boundfieldCols.ToArray(), new int[] { 2 }
                                ),
                                "ConsumoEmpsMasTiempoN21Pnl_T", tituloGrid)
                );
        }

        #region Reportes RM y JH

        private void ConsumoEmpsMasTiempo1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsumoEmpsMasTiempo(string.Empty));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Duracion", "Codigo Empleado" });
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total de Minutos Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsumoEmpsMasTiempoGrid_T", dtReporte, true, "Total",
                                new string[] { "", "{0:0,0}" },
                                Request.Path + "?Nav=ConsumoEmpsMasTiempoN2&Emple={0}",
                                new string[] { "Codigo Empleado" }, 0,
                                new int[] { 2 }, new int[] { 1 }, new int[] { 0 }),
                                "ConsumoEmpsMasTiempo1Pnl_T", tituloGrid)
                );
        }

        private void ConsumoEmpsMasTiempo2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                           Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsumoEmpsMasTiempo(linkGrafica));

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Duracion", "Codigo Empleado", "link" });
                dtReporte.Columns["Nombre Completo"].ColumnName = "Colaborador";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total de Minutos Desc");
                dtReporte.AcceptChanges();
            }



            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsumoEmpsMasTiempoGrid_T", dtReporte, true, "Total",
                                new string[] { "", "{0:0,0}" },
                                Request.Path + "?Nav=ConsumoEmpsMasTiempoN2&Emple={0}",
                                new string[] { "Codigo Empleado" }, 0,
                                new int[] { 2, 3 }, new int[] { 1 }, new int[] { 0 }),
                                "ConsumoEmpsMasTiempo2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Colaborador", "Total de Minutos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Colaborador"].ToString() == "Total")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Colaborador"].ColumnName = "label";
                dtReporte.Columns["Total de Minutos"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "ConsumoEmpsMasTiempoGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsumoEmpsMasTiempoGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "ConsumoEmpsMasTiempoGraf_G", "", "", "Nombre Completo", "Duración", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica


        }

        private void RepTabMenuConsLLamadasMasTiempo1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabMenuConsLLamadasMasTiempo());
            string[] cols = new string[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() != "BIMBO")
                {
                    cols = new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Duracion Minutos", "Costo", "Nombre Localidad", "Tipo de Llamada" };
                    dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                }
                else
                {
                    cols = new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Duracion Minutos", "Costo", "Nombre Localidad" };
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, cols);

                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.AcceptChanges();
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Minutos Desc");
            }


            contenedor.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("RepTabMenuConsLLamadasMasTiempoGrid_T", dtReporte, true, "Total",
                          new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "", "" }),
                          "RepTabMenuConsLLamadasMasTiempo1Pnl_T", tituloGrid)
          );
        }

        private void RepEstTabExtAbiertas2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepEstTabExtAbiertas());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extension", "Llamadas", "Costo", "Nombre Sitio", "ExtSitio" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["Costo"].ColumnName = "Total";
                ldt.Columns["Nombre Sitio"].ColumnName = "Ubicación";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Desc");
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepEstTabExtAbiertasGrid_T", ldt, true, "Total",
                                new string[] { "", "{0:0,0}", "{0:c}", "", "" },
                                Request.Path + "",
                                new string[] { "" }, 0,
                                new int[] { 4 }, new int[] { 0, 1, 2, 3 }, new int[] { }),
                                "RepEstTabExtAbiertas2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas", "Ubicación", "ExtSitio" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }


                ldt.Columns["ExtSitio"].ColumnName = "label";
                ldt.Columns["Llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepEstTabExtAbiertasGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepEstTabExtAbiertasGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepEstTabExtAbiertasGraf_G", "", "", "Extensón", "Llamadas", grafActiva, FCGpoGraf.Tabular, "", "", "dti", "98%", "330"), false);

            #endregion Grafica
        }

        private void RepTabConsPorEmpleado2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleado());

            #region Grid
            string link = string.Empty;
            string[] formatos = new string[] { };
            string[] camposLinks = new string[] { };
            int[] colsNoMostrar = new int[] { };
            int[] colsNoNaveg = new int[] { };
            int indexNav = 2;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "CostoFija", "Tel Fija", "CostoMovil", "Tel Movil", "Total" });
                    link = Request.Path + "?Nav=RepTabConsPorEmpleadoN2&Emple={0}";
                    formatos = new string[] { "", "", "{0:c}", "", "{0:c}", "", "{0:c}" };
                    camposLinks = new string[] { "Codigo Empleado" };
                    colsNoMostrar = new int[] { 1, 3, 5 };
                    colsNoNaveg = new int[] { 2, 4, 6 };
                    indexNav = 0;
                }
                else
                {
                    ldt = dvldt.ToTable(false, new string[] { "Nombre Centro de Costos", "Codigo Centro de Costos", "Nombre Completo", "Codigo Empleado", "CostoFija", "Tel Fija", "CostoMovil", "Tel Movil", "Total" });
                    ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                    link = Request.Path + "?Nav=RepTabConsPorEmpleadoN2&Emple={0}&Cencos={1}";
                    formatos = new string[] { "", "", "", "", "{0:c}", "", "{0:c}", "", "{0:c}" };
                    camposLinks = new string[] { "Codigo Empleado", "Codigo Centro de Costos" };
                    colsNoNaveg = new int[] { 0, 4, 8, 6 };
                    colsNoMostrar = new int[] { 1, 3, 5, 7 };
                }

                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["CostoFija"].ColumnName = "Consumo Total de Telefonía Fija";
                ldt.Columns["CostoMovil"].ColumnName = "Consumo Total de Telefonía Móvil";
                ldt.Columns["Total"].ColumnName = "Importe Total";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPorEmpleadoGrid_T", ldt, true, "Total",
                                formatos, link, camposLinks, 0, colsNoMostrar, colsNoNaveg, new int[] { indexNav }),
                                "RepTabConsPorEmpleado2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica
            DataTable dtGraf = DSODataAccess.Execute(RepTabConsPorEmpleadoGraf());
            if (dtGraf.Rows.Count > 0 && dtGraf.Columns.Count > 0)
            {
                DataView dvldtGraf = new DataView(dtGraf);
                dtGraf = dvldtGraf.ToTable(false, new string[] { "Nombre Categoria Tipo Destino", "Total" });

                for (int i = 0; i < dtGraf.Rows.Count; i++)
                {
                    string nom = dtGraf.Rows[i]["Nombre Categoria Tipo Destino"].ToString();
                    if (nom == "")
                    {
                        dtGraf.Rows[i].Delete();
                    }
                }
                dtGraf.Columns["Nombre Categoria Tipo Destino"].ColumnName = "label";
                dtGraf.Columns["Total"].ColumnName = "value";
                dtGraf.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepTabConsPorEmpleadoGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsPorEmpleadoGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtGraf, "value desc", 10)),
                "RepTabConsPorEmpleadoGraf_G", "", "", "Tipo Destino", "Total", grafActiva, FCGpoGraf.Tabular), false);



            #endregion Grafica
        }
        private void RepTabVariacionRentaLineas(Control contenedor, string tituloGrid)
        {

            DataTable dtReporte = DSODataAccess.Execute(Consulta12MesesVariacionRenta());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);

                List<string> nombresColumna = new List<string>();

                foreach (DataColumn column in dtReporte.Columns)
                {
                    nombresColumna.Add(column.ColumnName);
                }
                dtReporte = dvldt.ToTable(false, new string[] { nombresColumna[0], nombresColumna[1], nombresColumna[2], nombresColumna[3], nombresColumna[4], nombresColumna[5], nombresColumna[6], nombresColumna[7], nombresColumna[8], nombresColumna[9], nombresColumna[10], nombresColumna[11], nombresColumna[12], nombresColumna[13], nombresColumna[14], nombresColumna[15], nombresColumna[16], nombresColumna[17], nombresColumna[18], nombresColumna[19], nombresColumna[20], nombresColumna[21], nombresColumna[22], nombresColumna[23], nombresColumna[24], nombresColumna[25], nombresColumna[26], nombresColumna[27] });

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Linea Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepTabVariacionRentaLineas_T", dtReporte, false, "",
                            new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }),
                            "RepTabVariacionRentaLineas_T", tituloGrid)
            );
            StringBuilder cstext1 = new StringBuilder();
            cstext1.Append("var tds = document.getElementsByTagName(\"td\");");
            cstext1.Append("var tdsEnUnaFila = document.querySelectorAll(\"tr\");");
            cstext1.Append("var contador=1;");
            cstext1.Append("var child = tdsEnUnaFila[contador].childNodes;");
            cstext1.Append("for(var i = 0, j = tds.length; i < j; ++i){");
            cstext1.Append("child = tdsEnUnaFila[contador].childNodes;");
            cstext1.Append("for(var z = 8; z <= 28; z+=2){");
            cstext1.Append("    if(child[z].innerText.includes(\"-\")){child[z].style.color = \"#FF0000\";}else if(child[z].innerText.includes(\"0.00\")){child[z].style.color =\"#000000\"}  else{child[z].style.color =\"#00AA00\"}}");
            cstext1.Append("    ++contador;}");


            Page.ClientScript.RegisterStartupScript(GetType(), "CambiaColorTextoEnTD", cstext1.ToString(), true);

        }
        private void RepTabConsPorEmpleadoN22Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleadoN2(linkGrafica));

            #region Grid
            string[] formatos = new string[] { };
            string linkGrid = "";
            string[] campLink = new string[] { };
            int[] noMostrar = new int[] { };
            int[] colNoNavs = new int[] { };
            int indexLink = 1;
            int tot = 1;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion", "Codigo Empleado", "Link" });
                    formatos = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" };
                    linkGrid = Request.Path + "?Nav=RepTabConsPorEmpleadoN3&Emple={0}&TDest={1}";
                    campLink = new string[] { "Codigo Empleado", "Codigo Tipo Destino" };
                    noMostrar = new int[] { 1, 5, 6 };
                    colNoNavs = new int[] { 2, 3, 4 };
                    indexLink = 0;
                    tot = 0;
                }
                else
                {
                    ldt = dvldt.ToTable(false, new string[] { "Codigo Centro de Costos", "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion", "Codigo Empleado", "Link" });
                    ldt.Columns["Codigo Centro de Costos"].ColumnName = "cenCos";
                    formatos = new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "", "", "" };
                    linkGrid = Request.Path + "?Nav=RepTabConsPorEmpleadoN3&Emple={0}&Cencos={1}&TDest={2}";
                    campLink = new string[] { "Codigo Empleado", "cenCos", "Codigo Tipo Destino" };
                    noMostrar = new int[] { 0, 2, 6, 7 };
                    colNoNavs = new int[] { 3, 4, 5 };
                }

                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPorEmpleadoN2Grid_T", ldt, true, "Total",
                                formatos, linkGrid, campLink, tot, noMostrar, colNoNavs, new int[] { indexLink }),

                                "RepTabConsPorEmpleadoN22Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Tipo Destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepTabConsPorEmpleadoN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsPorEmpleadoN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsPorEmpleadoN2Graf_G", "", "", "Tipo Destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsPorEmpleadoN32Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleadoN3(linkGrafica));

            #region Grid
            string[] formatos = new string[] { };
            string linkGrid = "";
            string[] campLink = new string[] { };
            int[] noMostrar = new int[] { };
            int[] colNoNavs = new int[] { };
            int indexLink = 1;
            int tot = 1;

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    ldt = dvldt.ToTable(false, new string[] { "Numero Marcado", "Total", "Numero", "Duracion", "Tipo Llamada", "Clave Tipo Llamada", "Codigo Empleado", "Codigo Tipo Destino", "Link" });
                    formatos = new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "", "", "", "" };
                    linkGrid = Request.Path + "?Nav=RepTabConsPorEmpleadoN4&Emple={0}&NumMarc={1}&TipoEtiqueta={2}&TDest={3}";
                    campLink = new string[] { "Codigo Empleado", "Número Marcado", "Clave Tipo Llamada", "Codigo Tipo Destino" };
                    noMostrar = new int[] { 5, 6, 7 };
                    colNoNavs = new int[] { 1, 2, 3, 4 };
                }
                else
                {
                    ldt = dvldt.ToTable(false, new string[] { "Numero Marcado", "Total", "Numero", "Duracion", "Tipo Llamada", "Clave Tipo Llamada", "Codigo Centro de Costos", "Codigo Empleado", "Codigo Tipo Destino", "Link" });
                    formatos = new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "", "", "", "", "" };
                    linkGrid = Request.Path + "?Nav=RepTabConsPorEmpleadoN4&Emple={0}&Cencos={1}&NumMarc={2}&TipoEtiqueta={3}&TDest={4}";
                    campLink = new string[] { "Codigo Empleado", "Codigo Centro de Costos", "Número Marcado", "Clave Tipo Llamada", "Codigo Tipo Destino" };
                    noMostrar = new int[] { 5, 6, 7, 8 };
                    colNoNavs = new int[] { 1, 2, 3, 4 };
                }

                ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                ldt.Columns["Total"].ColumnName = "Importe Total";
                ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPorEmpleadoN3Grid_T", ldt, true, "Total",
                               formatos, linkGrid, campLink, 0,
                               noMostrar, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabConsPorEmpleadoN32Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Número Marcado", "Importe Total", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Número Marcado"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Número Marcado"].ColumnName = "label";
                ldt.Columns["Importe Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepTabConsPorEmpleadoN3Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsPorEmpleadoN3Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsPorEmpleadoN3Graf_G", "", "", "Número Marcado", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabConsPorEmpleadoN41Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabConsPorEmpleadoN4());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepTabConsPorEmpleadoN4Grid_T", dtReporte, true, "Total",
                            new string[] { "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "" }),
                            "RepTabConsPorEmpleadoN41Pnl_T", tituloGrid)
            );
        }

        private void RepTabConsPobmasMindeConv1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabConsPobmasMindeConv());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Codigo Localidad", "Costo", "Duracion", "TotLlam" });
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["TotLlam"].ColumnName = "Llamadas";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Minutos Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPobmasMindeConvGrid_T", dtReporte, true, "Total",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                Request.Path + "?Nav=RepTabConsPobmasMindeConvN2&Locali={0}",
                                new string[] { "Codigo Localidad" }, 0,
                                new int[] { 1 }, new int[] { 2, 3, 4 }, new int[] { 0 }),
                                "RepTabConsPobmasMindeConv1Pnl_T", tituloGrid)
                );
        }

        private void RepTabConsPobmasMindeConvN22Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                             Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabConsPobmasMindeConvN2(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "TotImporte", "LLamadas", "TotMin", "Codigo Localidad", "Link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["TotImporte"].ColumnName = "Total de Importe";
                ldt.Columns["LLamadas"].ColumnName = "Total de Llamadas";
                ldt.Columns["TotMin"].ColumnName = "Total de Minutos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Minutos Desc");
                ldt.AcceptChanges();
            }

            string navGrid = (DSODataContext.Schema.ToUpper() == "FCA") ? Request.Path + "?Nav=ConsEmpsMasCarosN3&Emple={0}" : Request.Path + "?Nav=RepTabConsPobmasMindeConvN3&Emple={0}&Locali={1}";
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPobmasMindeConvN2Grid_T", ldt, true, "Total",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                navGrid,
                                new string[] { "Codigo Empleado", "Codigo Localidad" }, 0,
                                new int[] { 1, 5, 6 }, new int[] { 2, 3, 4 }, new int[] { 0 }),
                                "RepTabConsPobmasMindeConvN22Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Total de Importe", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Total de Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepTabConsPobmasMindeConvN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsPobmasMindeConvN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsPobmasMindeConvN2Graf_G", "", "", "Empleado", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabConsPobmasMindeConvN32Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                            Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabConsPobmasMindeConvN3(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo Llamada", "Clave Tipo Llamada", "TotImporte", "TotLlam", "TotMin", "Codigo Empleado", "Codigo Localidad", "link" });
                ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                ldt.Columns["TotImporte"].ColumnName = "Total";
                ldt.Columns["TotLlam"].ColumnName = " Llamadas";
                ldt.Columns["TotMin"].ColumnName = "Minutos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPobmasMindeConvN2Grid_T", ldt, true, "Total",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                Request.Path + "?Nav=RepTabConsPobmasMindeConvN4&Emple={0}&Locali={1}&TipoLlam={2}",
                                new string[] { "Codigo Empleado", "Codigo Localidad", "Clave Tipo Llamada" }, 0,
                                new int[] { 1 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                                "RepTabConsPobmasMindeConvN32Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo de Llamada", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Llamada"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de Llamada"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "RepTabConsPobmasMindeConvN3Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabConsPobmasMindeConvN3Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabConsPobmasMindeConvN3Graf_G", "", "", "Tipo de Llamada", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabConsPobmasMindeConv1N4Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabConsPobmasMindeConvN4());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabConsPobmasMindeConvN4Grid_T", dtReporte, true, "Total",
                                new string[] { "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "" },
                                Request.Path + "",
                                new string[] { "" }, 0,
                                new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, new int[] { }),
                                "RepTabConsPobmasMindeConv1N4Pnl_T", tituloGrid)
                );
        }

        private void RepTabLLamsFueraHoraLaboral1Pnl(Control contenedor, string tituloGrid)
        {
            string WhereHorario = BuscaHorario();
            DataTable dtReporte = DSODataAccess.Execute(RepTabLLamsFueraHoraLaboral(WhereHorario));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Centro de Costos" });
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Fecha"].ColumnName = "Fecha";
                dtReporte.Columns["Hora"].ColumnName = "Hora";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }



            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLLamsFueraHoraLaboralGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "", "", "", "{0:c}", "" }),
                                "RepTabLLamsFueraHoraLaboral1Pnl_T", tituloGrid)
                );
        }

        //RM 20190212 BuscaHorario
        public string BuscaHorario()
        {
            try
            {
                string WhereHorario = string.Empty;

                StringBuilder query = new StringBuilder();

                query.AppendLine("Select ");
                query.AppendLine("    vchCodigo = vchCodigo,");
                query.AppendLine("    vchDescripcion = vchDescripcion,");
                query.AppendLine("    Horainicio = HoraInicioHorarioLaboral,");
                query.AppendLine("    MinutoInicio = MinutoInicioHorarioLaboral,");
                query.AppendLine("    HoraFin = HoraFinHorarioLaboral,");
                query.AppendLine("    MinutoFin = MinutoFinHorarioLaboral,");
                query.AppendLine("    Horario = '	(DATEPART(WEEKDAY,FechaInicio) = '+convert(varchar,NumeroDia)+' And '+");
                query.AppendLine("			    '		('+");
                query.AppendLine("			    '			SUBSTRING(CONVERT(varchar,FechaInicio,121),12,8) < ' +Case When HoraInicioHorarioLaboral < 10 Then '''''0'+ convert(varchar,HoraInicioHorarioLaboral) Else ''''''+convert(varchar,HoraInicioHorarioLaboral) End  +':'+ Case When MinutoInicioHorarioLaboral < 10 Then '0'+ convert(varchar,MinutoInicioHorarioLaboral)+'''''' Else convert(varchar,MinutoInicioHorarioLaboral)+'''''' End + ' OR '+");
                query.AppendLine("			    '			SUBSTRING(CONVERT(varchar,FechaInicio,121),12,8) > ' +Case When HoraFinHorarioLaboral < 10 Then '''''0'+ convert(varchar,HoraFinHorarioLaboral) Else ''''''+convert(varchar,HoraFinHorarioLaboral) End  +':'+ Case When MinutoFinHorarioLaboral < 10 Then '0'+ convert(varchar,MinutoFinHorarioLaboral) +''''''Else convert(varchar,MinutoFinHorarioLaboral)+'''''' End +");
                query.AppendLine("			    '		)) Or '");
                query.AppendLine("From [" + DSODataContext.Schema + "].[vishistoricos('horariolaboral','horarios laborales Diarios','español')]");
                query.AppendLine("Where  dtinivigencia <> dtFinVigencia");
                query.AppendLine("And dtiniVigencia <= '" + Session["FechaInicio"].ToString() + " 00:00:00'");
                query.AppendLine("And dtFinVigencia >= '" + Session["FechaFin"].ToString() + " 23:59:59'");
                query.AppendLine("order by vchCodigo ");

                DataTable dtHorario = DSODataAccess.Execute(query.ToString());
                List<string> listaAnds = new List<string>();

                if (dtHorario != null && dtHorario.Rows.Count > 0 && dtHorario.Columns.Count > 0)
                {
                    foreach (DataRow row in dtHorario.Rows)
                    {
                        listaAnds.Add(row["Horario"].ToString());
                    }

                    listaAnds[listaAnds.Count - 1] = listaAnds[listaAnds.Count - 1].Substring(0, listaAnds[listaAnds.Count - 1].Length - 3);
                }


                WhereHorario = "And (";
                foreach (string horario in listaAnds)
                {
                    WhereHorario = WhereHorario + " " + horario;
                }
                WhereHorario = WhereHorario + " ) ";

                if (WhereHorario == "And ( ) ")
                {
                    WhereHorario = "";
                }

                //WhereHorario = "And (" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 2 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 3 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 4 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 5 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''19:00'') Or" +
                //    " DATEPART(WEEKDAY, FechaInicio) = 6 And(SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) < ''09:00'' OR    SUBSTRING(CONVERT(varchar, FechaInicio, 121), 12, 8) > ''15:00'')" +
                //    ") ";

                return WhereHorario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ConsLocalidMasMarcadas1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLocalidMasMarcadas(1));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Codigo Localidad", "Nombre Tipo Destino", "Codigo Tipo Destino", "Numero", "Duracion", "Total", });

                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                dtReporte.Columns["Numero"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";


                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total de Llamadas Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsLocalidMasMarcadasGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "", "", "", "{0:c}" },
                                Request.Path + "?Nav=ConsLocalidMasMarcadasN2&locali={0}&tDest={1}",
                                new string[] { "Codigo localidad", "Codigo tipo destino" }, 0,
                                new int[] { 1, 3 }, new int[] { 2, 4, 5, 6 }, new int[] { 0 }),
                                "ConsLocalidMasMarcadas1Pnl_T", tituloGrid)
                );
        }

        private void ConsLocalidMasMarcadasN22Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLocalidMasMarcadasN2());

            //Datatable para grafica
            DataTable ldt = dtReporte.Copy();

            #region Grid
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "TotImporte", "LLamadas", "TotMin" });

                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["TotImporte"].ColumnName = "Total Importe";
                dtReporte.Columns["LLamadas"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["TotMin"].ColumnName = "Total de Minutos";


                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsLocalidMasMarcadasN2Grid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "{0:c}", "", "" },
                                linkGrid,
                                new string[] { }, 0,
                                new int[] { 1 }, new int[] { 0, 2, 3, 4 }, new int[] { }),
                                "ConsLocalidMasMarcadasN22Pnl_T", tituloGrid)
                );
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "TotImporte" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre Completo"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre Completo"].ColumnName = "label";
                ldt.Columns["TotImporte"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "ConsLocalidMasMarcadasN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsLocalidMasMarcadasN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ConsLocalidMasMarcadasN2Graf_G", "", "", "Empleado", "Total Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void ConsEmpmasLlam1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpmasLlam());
            int[] indexColumnasBoundField = new int[] { };
            string[] formato = new string[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);

                if (Convert.ToString(Session["OcultarColumnImporte"]) == "1")
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "Numero", "Duracion" });
                    indexColumnasBoundField = new int[] { 2, 3 };
                    formato = new string[] { "", "", "{0:0,0}", "{0:0,0}" };
                }
                else
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "Total", "Duracion", "Numero" });
                    indexColumnasBoundField = new int[] { 2, 3, 4 };
                    formato = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                }



                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["Numero"].ColumnName = "Llamadas";


                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Llamadas Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("ConsEmpmasLlamGrid_T", dtReporte, true, "Totales",
                            formato,
                            Request.Path + "?Nav=ConsEmpmasLlamN2&Emple={0}",
                            new string[] { "Codigo Empleado" }, 0,
                            new int[] { 1 }, indexColumnasBoundField, new int[] { 0 }),
                            "ConsEmpmasLlam1Pnl_T", tituloGrid)
            );

        }

        private void ConsEmpmasLlamN22Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpmasLlamN2(linkGrafica));

            //Datatable para grafica
            DataTable ldt = dtReporte.Copy();
            int[] indexColumnasBoundField = new int[] { };
            string[] formato = new string[] { };
            string ejeY = "";
            string prefijo = "";
            #region Grid
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                if (Convert.ToString(Session["OcultarColumnImporte"]) == "1")
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Numero", "Duracion" });
                    indexColumnasBoundField = new int[] { 2, 3 };
                    formato = new string[] { "", "", "{0:0,0}", "{0:0,0}" };
                    ejeY = "Numero";
                    prefijo = "";
                }
                else
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion" });
                    indexColumnasBoundField = new int[] { 2, 3, 4 };
                    formato = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                    ejeY = "Total";
                    prefijo = "$";
                }

                dtReporte.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                if (DSODataContext.Schema != "IKUSIVelatia")
                {
                    if (dtReporte.Columns.Contains("Total"))
                    {
                        dtReporte.Columns["Total"].ColumnName = "Importe Total";
                    }

                }
                dtReporte.Columns["Numero"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";


                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsEmpmasLlamN2Grid_T", dtReporte, true, "Totales",
                                formato,
                                linkGrid,
                                new string[] { "Codigo Tipo Destino" }, 0,
                                new int[] { 1 }, indexColumnasBoundField, new int[] { 0 }),
                                "ConsEmpmasLlamN22Pnl_T", tituloGrid)
                );
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);


                ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", ejeY, "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre Tipo Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "label";
                ldt.Columns[ejeY].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                       "ConsEmpmasLlamN22PnlGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsEmpmasLlamN22PnlGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ConsEmpmasLlamN22PnlGraf_G", "", "", "Tipo Destino", "Total", grafActiva, FCGpoGraf.Tabular, prefijo), false);

            #endregion Grafica
        }

        private void ConsEmpmasLlamN31Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpmasLlamN3(""));
            string[] formato = new string[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);


                if (Convert.ToString(Session["OcultarColumnImporte"]) == "1")
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });

                    formato = new string[] { "", "", "", "", "", "", "", "" };

                }
                else
                {
                    dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });

                    formato = new string[] { "", "", "", "", "{0:c}", "", "", "", "" };
                    dtReporte.Columns["Total"].ColumnName = "Importe Total";
                    dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                }

                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total Miutos";

                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";




                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsEmpmasLlamN31PnlGrid_T",
                                dtReporte, true, "Totales", formato),
                                "ConsEmpmasLlamN31Pnl_T", tituloGrid)
                );
        }

        private void ConsEmpsMasCaros1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCaros(""));
            string[] cols = new string[] { };
            string[] formatos = new string[] { };
            int[] colsNoMostrar = new int[] { };
            int[] colsNoNav = new int[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);

                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    cols = new string[] { "Nombre Completo", "Codigo Empleado", "Total", "Numero", "Duracion" };
                    formatos = new string[] { "", "", "{0:c}", "", "" };
                    colsNoMostrar = new int[] { 1};
                    colsNoNav = new int[] { 2,3,4};
                }
                else
                {
                    cols = new string[] { "Nombre Completo", "Codigo Empleado", "Centro de Costos", "Codigo Centro de Costos", "Total", "Numero", "Duracion" };
                    dtReporte.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                    formatos = new string[] { "", "", "", "", "{0:c}", "", "" };
                    colsNoMostrar = new int[] { 1, 3 };
                    colsNoNav = new int[] { 2, 4, 5, 6 };
                }

                dtReporte = dvldt.ToTable(false, cols);
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Numero"].ColumnName = "Total Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total Minutos";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsEmpsMasCaros1PnlGrid_T", dtReporte, true, "Totales",
                        formatos,Request.Path + "?Nav=ConsEmpsMasCarosN2&Emple={0}", new string[] { "Codigo Empleado" },
                        0, colsNoMostrar, colsNoNav, new int[] { 0 }),
                        "ConsEmpsMasCaros1Pnl_T", tituloGrid
                 )
            );

        }

        private void RepTabConsumosPorNumMarcTipoLlamada1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabConsumosPorNumMarcTipoLlamada());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Numero Marcado", "Llamadas", "Duracion Minutos", "Total", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });

                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Etiqueta"].ColumnName = "Localidad";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "RepTabConsumosPorNumMarcTipoLlamada1PnlGrid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "", "{0:c}", "", "", "" },
                        Request.Path + "?Nav=RepTabConsumosPorNumMarcTipoLlamadaN2&NumMarc={0}&TipoLlam={1}",
                        new string[] { "Número Marcado", "Clave Tipo Llamada" },
                        0,
                        new int[] { 5 },
                        new int[] { 1, 2, 3, 4, 6 },
                        new int[] { 0 }),
                        "RepTabConsumosPorNumMarcTipoLlamada1Pnl_T",
                        tituloGrid
                 )
            );


        }

        private void RepTabConsumosPorNumMarcTipoLlamadaN21Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabConsumosPorNumMarcTipoLlamadaN2());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });


                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "RepTabConsumosPorNumMarcTipoLlamadaN21PnlGrid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "", "", "{0:c}", "", "", "", "", "" },
                        "",
                        new string[] { "Número Marcado" },
                        0,
                        new int[] { 8 },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 },
                        new int[] { }),
                        "RepTabConsumosPorNumMarcTipoLlamadaN21Pnl_T",
                        tituloGrid
                 )
            );
        }

        private void ConsEmpsMasCarosN42Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCarosN4(linkGrafica));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion", "link" });

                dtReporte.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Numero"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            #region Grid




            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsEmpsMasCarosN4Grid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "{0:c}", "", "" },
                        linkGrid,
                        new string[] { "Codigo Tipo Destino" },
                        0,
                        new int[] { 1, 5 },
                        new int[] { 2, 3, 4 },
                        new int[] { 0 }),
                        "ConsEmpsMasCarosN42Pnl_T",
                        tituloGrid
                    )
                );
            #endregion

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Tipo Destino", "Importe Total", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Tipo Destino"].ColumnName = "label";
                dtReporte.Columns["Importe Total"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(
                                        DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                                "ConsEmpsMasCarosN42PnlGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsEmpsMasCarosN42PnlGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "ConsEmpsMasCarosN42PnlGraf_G", "", "", "Tipo Destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void ConsEmpsMasCarosN52Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCarosN5(linkGrafica));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Carrier", "Codigo Carrier", "Total", "Numero", "Duracion", "link" });

                dtReporte.Columns["Nombre Carrier"].ColumnName = "Carrier";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Numero"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            #region Grid


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsEmpsMasCarosN5Grid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "{0:c}", "", "" },
                        linkGrid,
                        new string[] { "Codigo Carrier" },
                        0,
                        new int[] { 1, 5 },
                        new int[] { 2, 3, 4 },
                        new int[] { 0 }),
                        "ConsEmpsMasCarosN52Pnl_T",
                        tituloGrid
                    )
                );
            #endregion

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Carrier", "Importe Total", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Carrier"].ColumnName = "label";
                dtReporte.Columns["Importe Total"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                                        DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                                "ConsEmpsMasCarosN5Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsEmpsMasCarosN5Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "ConsEmpsMasCarosN5Graf_G", "", "", "Tipo Destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void ConsEmpsMasCarosN62Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCarosN6(linkGrafica));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Numero Marcado", "Total", "Numero", "Duracion", "Tipo Llamada", "Clave Tipo Llamada", "link" });

                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Numero"].ColumnName = "Total de Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            #region Grid


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsEmpsMasCarosN6Grid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "{0:c}", "", "", "", "", "" },
                        linkGrid,
                        new string[] { "Número Marcado" },
                        0,
                        new int[] { 5, 6 },
                        new int[] { 1, 2, 3, 4 },
                        new int[] { 0 }),
                        "ConsEmpsMasCarosN62Pnl_T",
                        tituloGrid
                    )
                );
            #endregion

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Número Marcado", "Importe Total", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Número Marcado"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Número Marcado"].ColumnName = "label";
                dtReporte.Columns["Importe Total"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                                        DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                                "ConsEmpsMasCarosN6Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsEmpsMasCarosN6Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "ConsEmpsMasCarosN6Graf_G", "", "", "Tipo Destino", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void ConsEmpsMasCarosN71Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpsMasCarosN7(""));

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });

                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total de Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";


                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            #region Grid

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsEmpsMasCarosN7Grid_T",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "", "", "{0:c}", "", "", "", "", "" },
                        "",
                        new string[] { },
                        0,
                        new int[] { 8 },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 },
                        new int[] { }),
                        "ConsEmpsMasCarosN71Pnl_T",
                        tituloGrid
                    )
                );
            #endregion


        }

        private void ConsLlamsMasCaras1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLlamsMasCaras());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(
                    false,
                    new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Sitio", "Nombre Localidad" });
                //new string[] { "Extension", "Nombre Completo", "Codigo Empleado", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Sitio", "Codigo Sitio", "Nombre Localidad", "Codigo Localidad" });

                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.AcceptChanges();
            }
            #region Grid

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsLlamsMasCarasGrid",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "", "", "", "", "{0:c}", "", "" },
                        "",
                        new string[] { },
                        0,
                        new int[] { },
                        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        new int[] { }),
                        "ConsLlamsMasCaras1Pnl_T",
                        tituloGrid
                    )
                );
            #endregion
        }

        private void ConsNumerosMasMarcadas1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsNumerosMasMarcadas());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(
                    false,
                    new string[] { "Numero Marcado", "Llamadas", "Nombre Localidad", "Codigo Localidad", "Total", "Cant Emp", "Duracion" });


                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Cant Emp"].ColumnName = "Número  de Empleados";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";

                dtReporte.AcceptChanges();
            }

            #region Grid


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsLlamsMasCarasGrid",
                        dtReporte,
                        true,
                        "Totales",
                        new string[] { "", "", "", "", "{0:c}", "", "", "", "" },
                        Request.Path + "?Nav=ConsNumerosMasMarcadasN2&NumMarc={0}&locali={1}",
                        new string[] { "Número Marcado", "Codigo Localidad" },
                        0,
                        new int[] { 3 },
                        new int[] { 1, 2, 4, 5, 6 },
                        new int[] { 0 }),
                        "ConsNumerosMasMarcadas1Pnl_T",
                        tituloGrid
                    )
                );
            #endregion




        }

        private void ConsNumerosMasMarcadasN22Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsNumerosMasMarcadasN2(linkGrafica));
            DataTable dtReporteGraf = DSODataAccess.Execute(ConsNumerosMasMarcadasN2Graf());
            string[] cols = new string[] { };
            string[] formCols = new string[] { };
            string[] campsLinks = new string[] { };
            int[] colsNosMostrar = new int[] { };
            int[] colsNoNav = new int[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);

                if(DSODataContext.Schema.ToUpper()=="FCA")
                {
                    cols = new string[] { "Nombre Completo", "Codigo Empleado", "Nombre Sitio", "Codigo Sitio", "Total", "Numero", "Duracion", "link" };
                    formCols = new string[] {  "", "", "", "", "{0:c}", "", "", "" };
                    campsLinks = new string[] { "Codigo Empleado", "Codigo Sitio" };
                    colsNosMostrar = new int[] { 1, 3, 7};
                    colsNoNav = new int[] { 2, 4, 5,6 };
                }
                else
                {
                    cols = new string[] { "Nombre Completo", "Codigo Empleado", "Centro de Costos", "Codigo Centro de Costos", "Nombre Sitio", "Codigo Sitio", "Total", "Numero", "Duracion", "link" };
                    dtReporte.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                    formCols = new string[] { "", "", "", "", "", "", "{0:c}", "", "", "" };
                    campsLinks = new string[] { "Codigo Empleado", "Codigo Centro de Costos", "Codigo Sitio" };
                    colsNosMostrar = new int[] { 1, 3, 5, 9 };
                    colsNoNav = new int[] { 2, 4, 6, 7, 8 };
                }

                dtReporte = dvldt.ToTable(false, cols);
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";                
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Nombre del Sitio";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Numero"].ColumnName = "Total Llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Total Minutos";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }


            //DataTable para la grafica
            if (dtReporteGraf.Rows.Count > 0 && dtReporteGraf.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporteGraf);
                dtReporteGraf = dvldt.ToTable(false, new string[] { "Nombre Sitio", "Total" });
                dtReporteGraf.Columns["Nombre Sitio"].ColumnName = "label";
                dtReporteGraf.Columns["Total"].ColumnName = "value";

                dtReporteGraf.AcceptChanges();
            }

            #region Grid

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsNumerosMasMarcadasN2Grid_T",dtReporte,true,"Totales",
                        formCols, linkGrid,campsLinks,0, colsNosMostrar, colsNoNav, new int[] { 0 }),
                    "ConsNumerosMasMarcadasN22Pnl_T",tituloGrid
                    )
                );
            #endregion

            #region Grafica
            contenedorGrafica.Controls.Add(
                                        DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                                            "ConsNumerosMasMarcadasN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsNumerosMasMarcadasN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporteGraf, "value desc", 10)),
                "ConsNumerosMasMarcadasN2Graf_G", "", "", "Nombre del Sitio", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void ConsNumerosMasMarcadasN31Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsNumerosMasMarcadasN3());
            string[] columns = new string[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() != "BIMBO")
                {
                    columns = new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" };
                }
                else
                {
                    columns = new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado" };
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, columns);



                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Duracion"].ColumnName = "Total Minutos";
                dtReporte.Columns["Total"].ColumnName = "Importe Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                dtReporte.AcceptChanges();
            }

            #region Grid


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("ConsNumerosMasMarcadasN3Grid_T",
                        dtReporte, true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "", "" },
                        "", new string[] { }, 0, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        new int[] { }), "ConsNumerosMasMarcadasN31Pnl_T", tituloGrid)
                );
            #endregion




        }

        #endregion Reportes RM y JH

        private void RepTabLlamadasEntreSitios2Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                    Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepObtieneLlamEntradasEnlace(linkGrafica));

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "icodSitio", "Descripcion", "Llamadas", "Link" });
                ldt.Columns["Descripcion"].ColumnName = "Sitio";
                ldt.Columns["Llamadas"].ColumnName = "Llamadas de entrada";

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc");
                ldt.AcceptChanges();
            }




            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasEntreSitiosGrid_T", ldt, true, "Total",
                                new string[] { "", "", "{0:0,0}" },
                                Request.Path + "?Nav=RepTabLlamadasEntreSitiosN2&Sitio={0}",
                                new string[] { "icodSitio" }, 1,
                                new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabLlamadasEntreSitios2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEtiq = new DataView(ldt);
                ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "icodSitio", "Sitio", "Llamadas de entrada", "Link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Llamadas de entrada"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamadasEntreSitiosGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasEntreSitiosGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasEntreSitiosGraf_G", "", "", "Sitio", "Llamadas de entrada", grafActiva, FCGpoGraf.Tabular, "", "", "dti", "98%", "385"), false);

            #endregion Grafica
        }

        private void RepTabLlamadasEntreSitiosN22Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                   Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabLlamadasEntreSitiosN2(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "sitioDestino", "SitioQueLlamo", "vchDescripcion", "TotalLlam", "Link" });
                ldt.Columns["vchDescripcion"].ColumnName = "Sitio";
                ldt.Columns["TotalLlam"].ColumnName = "Llamadas de entrada";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasEntreSitiosN2Grid_T", ldt, true, "Total",
                                new string[] { "", "", "", "{0:0,0}", "" },
                                Request.Path + "?Nav=RepTabLlamadasEntreSitiosN3&Sitio={0}&SitioDest={1}",
                                new string[] { "sitioDestino", "SitioQueLlamo" }, 2,
                                new int[] { 0, 1, 4 }, new int[] { 3 }, new int[] { 2 }),
                                "RepTabLlamadasEntreSitiosN22Pnls_T", tituloGrid)
                );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEtiq = new DataView(ldt);
                ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Sitio", "Llamadas de entrada", "Link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Llamadas de entrada"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamadasEntreSitiosN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasEntreSitiosN2Graf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasEntreSitiosN2Graf_G", "", "", "Sitio", "Llamadas de entrada", grafActiva, FCGpoGraf.Tabular, "", "", "dti", "98%", "385"), false);

            #endregion Grafica
        }

        private void RepTabLlamadasEntreSitiosN32Pnls(string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                  Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabLlamadasEntreSitiosN3(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "SitioOrigen", "Teldest", "TotalLlam", "Link" });
                ldt.Columns["Teldest"].ColumnName = "Extensión Destino";
                ldt.Columns["TotalLlam"].ColumnName = "Llamadas Recibidas";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Recibidas Desc");
                ldt.AcceptChanges();
            }


            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasEntreSitiosN3Grid_T", ldt, true, "Total",
                                new string[] { "", "", "{0:0,0}", "" },
                                Request.Path + "?Nav=RepTabLlamadasEntreSitiosN4&Sitio={0}&Extension={1}" + "&SitioDest=" + param["SitioDest"],
                                new string[] { "SitioOrigen", "Extensión Destino" }, 1,
                                new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabLlamadasEntreSitiosN32Pnls_T", tituloGrid)
                );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepMinsPorEtiq = new DataView(ldt);
                ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Extensión Destino", "Llamadas Recibidas", "Link" });

                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión Destino"].ToString() == "Total")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión Destino"].ColumnName = "label";
                ldt.Columns["Llamadas Recibidas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamadasEntreSitiosN3Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamadasEntreSitiosN3Graf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamadasEntreSitiosN3Graf_G", "", "", "Extensión", "Llamadas Recibidas", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepTabLlamadasEntreSitiosN41Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabLlamadasEntreSitiosN4());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "SitioDesc", "NomCompleto","NumeroqueMarco",
                    "NumeroMarcado", "FechaLlamada", "HoraLlamada", "DuracionMin", "CostoTotal",
                    "LocaliDesc","CarrierDesc","TDestDesc","Etiqueta" });

                dtReporte.Columns["SitioDesc"].ColumnName = "Centro de Costos";
                dtReporte.Columns["NomCompleto"].ColumnName = "Colaborador";
                dtReporte.Columns["NumeroqueMarco"].ColumnName = "Extensión";
                dtReporte.Columns["NumeroMarcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["FechaLlamada"].ColumnName = "Fecha";
                dtReporte.Columns["HoraLlamada"].ColumnName = "Hora";
                dtReporte.Columns["DuracionMin"].ColumnName = "Cantidad de Minutos";
                dtReporte.Columns["CostoTotal"].ColumnName = "Costo";
                dtReporte.Columns["LocaliDesc"].ColumnName = "Localidad";
                dtReporte.Columns["CarrierDesc"].ColumnName = "Carrier";
                dtReporte.Columns["TDestDesc"].ColumnName = "Tipo de Destino";


                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Cantidad de Minutos Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamadasEntreSitiosN4Grid_T", dtReporte, true, "Total",
                                new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "" }),
                                "RepTabLlamadasEntreSitiosN41Pnl_T", tituloGrid)
                );
        }

        private void ConsLugmasCost2Pnls(string linkGrid, string linkGrafica,
            Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLugmasCost(1));
            DataTable ldt = dtReporte.Copy();

            #region Grid
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Codigo Localidad", "Costo", "Duracion", "TotLlam", });
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Costo"].ColumnName = "Total Importe";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["TotLlam"].ColumnName = "Llamadas";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Importe Desc");
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsLugmasCostGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" },
                                Request.Path + "?Nav=ConsLugmasCostN2&Locali={0}",
                                new string[] { "Codigo Localidad" }, 0,
                                new int[] { 1 }, new int[] { 2, 3, 4 }, new int[] { 0 }),
                                "ConsLugmasCost2Pnls_T", tituloGrid)
                );

            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Costo", "Link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre Localidad"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre Localidad"].ColumnName = "label";
                ldt.Columns["Costo"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ConsLugmasCostGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsLugmasCostGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ConsLugmasCostGraf_G", "", "", "Localidad", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void ConsLugmasCostN22Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsLugmasCostN2(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");

            }


            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioGrid_T", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                      "ConsLugmasCostN22Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorSitioGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorSitioGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);



            #endregion Grafica
        }

        private void ConsLugmasCostN32Pnls(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaConsLugmasCostN3());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsLugmasCostN3Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                Request.Path + "?Nav=ConsLugmasCostN4&Locali=" + param["Locali"] + "&Sitio=" + param["Sitio"] + "&Cencos={0}",
                                new string[] { "Codigo Centro de Costos" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "ConsLugmasCostN32Pnls_T", tituloGrid)
                );
        }

        private void ConsLugmasCostN42Pnls(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLugmasCostN4());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            string nav = (DSODataContext.Schema.ToUpper() == "FCA") ? Request.Path + "?Nav=ConsLugmasCostN5&Locali=" + param["Locali"] + "&Sitio=" + param["Sitio"] + "&Emple={0}" : Request.Path + "?Nav=ConsLugmasCostN5&Locali=" + param["Locali"] + "&Sitio=" + param["Sitio"] + "&Cencos=" + param["CenCos"] + "&Emple={0}";
            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsLugmasCostN4Grid_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                nav,
                                new string[] { "Codigo Emple" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "ConsLugmasCostN42Pnls_T", tituloGrid)
                );
        }

        private void ConsEmpmasLlam2Pnls(string linkGrid, string linkGrafica,
            Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpmasLlam());
            DataTable ldt = dtReporte.Copy();

            #region Grid
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "Total", "Duracion", "Numero" });
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["Numero"].ColumnName = "Llamadas";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Llamadas Desc");
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("ConsEmpmasLlamGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "{0:c}", "", "" },
                                Request.Path + "?Nav=ConsEmpmasLlamN2&Emple={0}",
                                new string[] { "Codigo Empleado" }, 0,
                                new int[] { 1 }, new int[] { 2, 3, 4 }, new int[] { 0 }),
                                "ConsEmpmasLlam2Pnls_T", tituloGrid)
                );
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre Completo"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre Completo"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ConsEmpmasLlamGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsEmpmasLlamGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ConsEmpmasLlamGraf_G", "", "", "Localidad", "Total", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepConsumoPorSucursalesv2Bimbo1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaConsumoPorSucursalBimbo(string.Empty));
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Importe Telmex", "Total Llams Telmex", "Importe Axtel", "Total Llams Axtel", "Total Gral" });
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte.Columns["Total Llams Telmex"].ColumnName = "Llamadas Telmex";
                dtReporte.Columns["Total Llams Axtel"].ColumnName = "Llamadas Axtel";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Gral Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsumoPorSucursalesv2BimboGrid_T", dtReporte, true, "Total Gral",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:c}" },
                                Request.Path + "?Nav=RepPorSucursalv2N2&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 1,
                                new int[] { 0 }, new int[] { 2, 3, 4, 5, 6 },
                                new int[] { 1 }),
                                "RepConsumoPorSucursalesv2Bimbo_T", tituloGrid)
                );
        }

        private void RepConsumoPorSucursalesv2Bimbo2Pnls(string linkGrid, string linkGrafica,
            Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaConsumoPorSucursalBimbo(linkGrafica));

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Importe Telmex", "Total Llams Telmex", "Importe Axtel", "Total Llams Axtel", "Total Gral", "link" });
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte.Columns["Total Llams Telmex"].ColumnName = "Llamadas Telmex";
                dtReporte.Columns["Total Llams Axtel"].ColumnName = "Llamadas Axtel";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Gral Desc");
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsumoPorSucursalesv2BimboGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "{0:c}", "{0:0,0}", "{0:c}", "{0:0,0}", "{0:c}", "" },
                                Request.Path + "?Nav=RepPorSucursalv2N2&Sitio={0}",
                                new string[] { "Codigo Sitio" }, 1,
                                new int[] { 0, 7 }, new int[] { 2, 3, 4, 5, 6 },
                                new int[] { 1 }),
                                "RepConsumoPorSucursalesv2Bimbo_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Sitio", "Total Gral", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Sitio"].ColumnName = "label";
                dtReporte.Columns["Total Gral"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepConsumoPorSucursalesv2BimboGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoPorSucursalesv2BimboGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "RepConsumoPorSucursalesv2BimboGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);



            #endregion Grafica
        }

        private void RepConsumoPorSucursalesv2BimboN22Pnls(string linkGrid, string linkGrafica,
            Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaConsumoPorSucursalBimboN2(linkGrafica));

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Telefono", "Total Llams Telmex", "Importe Telmex", "Importe Axtel", "Total Llams Axtel", "Total Gral", "link" });
                dtReporte.Columns["Total Llams Telmex"].ColumnName = "Llamadas Telmex";
                dtReporte.Columns["Total Llams Axtel"].ColumnName = "Llamadas Axtel";

                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Gral Desc");
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsumoPorSucursalesv2BimboN2Grid_T", dtReporte, true, "Totales",
                                new string[] { "", "{0:0,0}", "{0:c}", "{0:c}", "{0:0,0}", "{0:c}", "" }),
                                "RepConsumoPorSucursalesv2BimboN2_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Telefono", "Total Gral" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Telefono"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Telefono"].ColumnName = "label";
                dtReporte.Columns["Total Gral"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepConsumoPorSucursalesv2BimboN2Graf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsumoPorSucursalesv2BimboN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "RepConsumoPorSucursalesv2BimboN2Graf_G", "", "", "Telefono", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepIndConcentracionGasto2Nvl()
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndConcentracionGasto2Nvl());

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "iCodCatEmple", "Colaborador", "Centro de Costos", "Llamadas", "Duracion", "Importe" });
                dtReporte.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                dtReporte.Columns["Duracion"].ColumnName = "Cantidad minutos";
                dtReporte.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepIndConcentracionGasto2NvlGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "{0:0,0}", "{0:0,0}", "{0:c}" }, Request.Path + "?Nav=RepIndConcentracionGastoN3&Emple={0}&Indicador=1",
                                new string[] { "iCodCatEmple" }, 1, new int[] { 0 }, new int[] { 2, 3, 4, 5 }, new int[] { 1 }),
                                "RepIndConcentracionGasto2Nvl_T", "Empleados que concentran el 15% del gasto total")
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                dtReporte.AsEnumerable().ToList()
                       .ForEach(x => x.SetField<string>("link", string.Format("{0}?Nav=RepIndConcentracionGastoN3&Emple={1}&Indicador=1", Request.Path, x.Field<int>("iCodCatEmple"))));

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Colaborador", "Importe", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Colaborador"].ColumnName = "label";
                dtReporte.Columns["Importe"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            Rep2.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepIndConcentracionGasto2Nvl_G", "Empleados que concentran el 15% del gasto total", 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepIndConcentracionGasto2Nvl_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
                "RepIndConcentracionGasto2Nvl_G", "", "", "Colaborador", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        private void RepIndCodAutoNuevos2Nvl()
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndCodAutoNuevos2Nvl());

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "iCodCatSitio", "Sitio", "Asignados", "No Asignados", "Importe" });
                dtReporte.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepIndCodAutoNuevos2NvlGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:c}" }, string.Empty,
                                new string[] { }, 1, new int[] { 0 }, new int[] { 1, 2, 3, 4 }, new int[] { }),
                                "RepIndCodAutoNuevos2Nvl_T", "Códigos nuevos")
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Sitio", "Importe" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Sitio"].ColumnName = "label";
                dtReporte.Columns["Importe"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            Rep2.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepIndCodAutoNuevos2Nvl_G", "Gráfica códigos nuevos", 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepIndCodAutoNuevos2Nvl_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
                "RepIndCodAutoNuevos2Nvl_G", "", "", "Sitio", "Importe", 1, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepIndExtenNuevas2Nvl()
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndExtenNuevas2Nvl());

            #region Grid

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "iCodCatSitio", "iCodCatExten", "Extensión", "Sitio", "Asignadas", "No Asignadas", "Importe" });
                dtReporte.AcceptChanges();
            }

            Rep1.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepIndExtenNuevas2NvlGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "", "", "", "{0:c}" }, string.Empty,
                                new string[] { }, 2, new int[] { 0, 1 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { }),
                                "RepIndExtenNuevas2Nvl_T", "Extensiones nuevas")
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Importe" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Importe"].ColumnName = "value";
                dtReporte.AcceptChanges();
            }

            Rep2.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas(
                "RepIndExtenNuevas2Nvl_G", "Top 10 Extensiones nuevas", 2, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepIndExtenNuevas2Nvl_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10)),
                "RepIndExtenNuevas2Nvl_G", "", "", "Sitio", "Importe", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        private void RepIndLlamMayorDuracion2Nvl(int opc = 0)
        {
            var valor = ConsultaRepIndLlamMayorDuracion2Nvl(opc);

            StringBuilder query = new StringBuilder();
            query.AppendLine("DECLARE @valores VARCHAR(40)");
            query.AppendLine("SELECT @valores = COALESCE(@valores + ',','') + (Convert(varchar,iCodCatalogo))");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");

            if (opc == 0)
            {
                query.AppendLine(" AND vchCodigo IN('CelLoc','CelNac')");
            }
            else
            {
                query.AppendLine(" AND vchCodigo IN('LDM')");
            }
            //if (DSODataContext.Schema.ToUpper() != "K5SCHINDLER" && DSODataContext.Schema.ToUpper() != "SCHINDLER" && DSODataContext.Schema.ToUpper() != "KIONETWORKSDRP"&& DSODataContext.Schema.ToUpper() != "KIOC12" && DSODataContext.Schema.ToUpper() != "K5REDIT")
            //{
            //    query.AppendLine(" AND vchCodigo IN('CelLoc','CelNac')");
            //}
            //else
            //{
            //    query.AppendLine(" AND vchCodigo IN('LDM')");
            //}

            query.AppendLine("SELECT @valores");

            string tiposDestino = DSODataAccess.ExecuteScalar(query.ToString()).ToString();
            if (opc == 0)
            {
                WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
                ReporteDetallado(Rep0, "Llamada(s) de mayor duración a celular");
            }
            else
            {
                WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
                ReporteDetallado(Rep0, "Llamada(s) de mayor duración, larga distancia mundial");
            }

            //if (!string.IsNullOrEmpty(valor) && !string.IsNullOrEmpty(tiposDestino) && (DSODataContext.Schema.ToUpper() != "K5SCHINDLER" && DSODataContext.Schema.ToUpper() != "SCHINDLER" && DSODataContext.Schema.ToUpper() != "KIONETWORKSDRP" && DSODataContext.Schema.ToUpper() != "K5REDIT"))
            //{
            //    WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
            //    ReporteDetallado(Rep0, "Llamada(s) de mayor duración a celular");
            //}
            //else if (!string.IsNullOrEmpty(valor) && !string.IsNullOrEmpty(tiposDestino) && (DSODataContext.Schema.ToUpper() == "K5SCHINDLER" || DSODataContext.Schema.ToUpper() == "SCHINDLER" || DSODataContext.Schema.ToUpper() == "KIONETWORKSDRP" || DSODataContext.Schema.ToUpper() == "KIOC12" || DSODataContext.Schema.ToUpper() == "K5REDIT"))
            //{
            //    WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
            //    ReporteDetallado(Rep0, "Llamada(s) de mayor duración, larga distancia mundial");
            //}
        }

        private void RepTabBuscaLineasAVencer(Control contenedorGrid, string tituloGrid, int mesesParaVencimiento)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsultaBuscaLineasAVencer(mesesParaVencimiento));
            DataTable dtCloneRep = dtReporte.Clone();

            foreach (DataRow row in dtReporte.Rows)
            {
                dtCloneRep.ImportRow(row);
            }

            if (!dtCloneRep.Columns.Contains("FechaFinPlan"))
            {
                dtCloneRep.Columns.Add("FechaFinPlan", typeof(string));
            }


            #region Grid
            if (dtCloneRep.Rows.Count > 0 && dtCloneRep.Columns.Count > 0)
            {

                //Modificar cada fecha fin vencida por la palabra vencido

                foreach (DataRow row in dtCloneRep.Rows)
                {
                    DateTime datetimeFechaFin = new DateTime();

                    if (DateTime.TryParse(row["FechaVigencia"].ToString(), out datetimeFechaFin))
                    {
                        if (datetimeFechaFin <= DateTime.Now)
                        {
                            row["FechaFinPlan"] = "VENCIDO";

                            dtReporte.AcceptChanges();
                        }
                        else
                        {
                            row["FechaFinPlan"] = datetimeFechaFin.ToString("yyyy-MM-dd");
                        }
                    }
                }

                DataView dvldt = new DataView(dtCloneRep);
                dtCloneRep = dvldt.ToTable(false, new string[] { "línea", "carrier", "nomCompleto", "fechaFinPlan", "meses" });
                dtCloneRep.Columns["línea"].ColumnName = "Línea";
                dtCloneRep.Columns["carrier"].ColumnName = "Carrier";
                dtCloneRep.Columns["nomCompleto"].ColumnName = "Colaborador";
                dtCloneRep.Columns["fechaFinPlan"].ColumnName = "Fecha Fin";
                dtCloneRep.Columns["meses"].ColumnName = "Meses";

                //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "meses");
                dtCloneRep.AcceptChanges();
            }


            GridView grdvdtRep = new GridView();
            grdvdtRep = DTIChartsAndControls.GridView("RepTabBuscaLineasAVencerGrid", dtCloneRep, false, "Totales",
                                new string[] { "", "", "", "", "" });

            grdvdtRep.RowDataBound += (sender, e) => CambiaColorLetra_RowDataBound(sender, e, "Meses", "Fecha Fin");
            grdvdtRep.DataBind();


            contenedorGrid.Controls.Add(
               DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                               grdvdtRep,
                               "RepTabBuscaLineasAVencer_T", tituloGrid)
               );

            #endregion

        }

        /// <summary>
        /// Cambia propiedades del color de la Cell del gridview, [(Valor < 0) = rojo], [(Valor > 0) = verde], [(Valor = 0) = gris]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="columnaValor">Nombre de columna que contiene valor a validar</param>
        public void CambiaColorLetra_RowDataBound(object sender, GridViewRowEventArgs e, string nombreColumnaValor, string fechaFinPlan)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int meses =
                 Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, nombreColumnaValor));

                string fechaFin = DataBinder.Eval(e.Row.DataItem, fechaFinPlan).ToString();

                if (meses < 0 || fechaFin.ToLower() == "vencido")
                {
                    e.Row.Cells[3].Style.Add("color", "#ff3333");
                    e.Row.Cells[4].Style.Add("color", "#ff3333");
                }
                if (meses == 1)
                {
                    e.Row.Cells[3].Style.Add("color", "#efae17");
                    e.Row.Cells[4].Style.Add("color", "#efae17");
                }
                if (meses == 2)
                {
                    e.Row.Cells[3].Style.Add("color", "#b3f235");
                    e.Row.Cells[4].Style.Add("color", "#b3f235");
                }
                if (meses > 2)
                {
                    e.Row.Cells[3].Style.Add("color", "#49ba41");
                    e.Row.Cells[4].Style.Add("color", "#49ba41");
                }

                e.Row.Cells[3].BorderColor = System.Drawing.Color.Black;
                e.Row.Cells[4].BorderColor = System.Drawing.Color.Black;

                e.Row.Cells[3].Style.Add(HtmlTextWriterStyle.FontWeight, "Bold");
                e.Row.Cells[4].Style.Add(HtmlTextWriterStyle.FontWeight, "Bold");

            }
        }

        public void ConRepTabConsHistAnioActualVsAnterior22Pnl(Control contenedorGrid, string tituloGrid, Control contenedorGraf, string tituloGrafica)
        {
            DataTable dtOriginal = DSODataAccess.Execute(ConsultaConRepTabConsHistAnioActualVsAnterior2());

            if (dtOriginal.Rows.Count > 0 && dtOriginal.Columns.Count > 0)
            {

                string[] columnasNombres = FCAndControls.extraeNombreColumnas(dtOriginal);
                foreach (string col in columnasNombres)
                {
                    if
                        (
                            (col != "Nombre Mes") &&
                            (col != "Total Anterior") &&
                            (col != "Total Actual") &&
                            (col != "FechaKeytia")
                        )
                    {
                        dtOriginal.Columns.Remove(col);
                        dtOriginal.AcceptChanges();
                    }
                }



                if (dtOriginal.Columns.Contains("Nombre Mes"))
                {
                    dtOriginal.Columns["Nombre Mes"].ColumnName = "Mes";
                }

                if (dtOriginal.Columns.Contains("Total Anterior"))
                {
                    dtOriginal.Columns["Total Anterior"].ColumnName = "Total Año Anterior";
                }

                if (dtOriginal.Columns.Contains("Total Actual"))
                {
                    dtOriginal.Columns["Total Actual"].ColumnName = "Total Año Actual";
                }


                #region Reporte

                DataTable dtReporte = dtOriginal.Clone();
                foreach (DataRow row in dtOriginal.Rows)
                {
                    dtReporte.ImportRow(row);
                }

                if (dtReporte.Columns.Contains("FechaKeytia"))
                {
                    dtReporte.Columns.Remove("FechaKeytia");
                }

                GridView grdV = new GridView();
                grdV = DTIChartsAndControls.GridView("RepAñoAntvsAñoActGrid", dtReporte, false, "Totales", new string[] { "", "", "" });

                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                               grdV,
                                                "RepAñoAntvsAñoActGrid_T", tituloGrid));

                #endregion

                #region Grafica
                if (dtOriginal.Columns.Contains("FechaKeytia"))
                {
                    dtOriginal.Columns.Remove("FechaKeytia");
                }

                DataTable[] arrDT;
                arrDT = FCAndControls.ConvertDataTabletoDataTableArray(dtOriginal);

                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(arrDT);
                string[] series = FCAndControls.extraeNombreColumnas(dtOriginal);

                //20180718 Grafica de stack NZ

                contenedorGraf.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("contAnioActvsAnioAnt", tituloGrafica, 0, FCGpoGraf.MatricialConStack));
                if (lsaDataSource != null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "contAnioActvsAnioAnt_Graf",
                        FCAndControls.GraficaMultiSeries(lsaDataSource, series, "contAnioActvsAnioAnt",
                                     "", "", "Mes", "Importe", 0, FCGpoGraf.MatricialConStack), false);
                }

                #endregion



            }

        }

        public void ReporteMatricialDiasProcesados(Control contenedorGrid, string tituloGrid)
        {
            DataTable dtRep = DSODataAccess.Execute(ConsultaReporteMatricialDiasProcesados());
            DataView dtvRep = new DataView(dtRep);


            #region Eliminar Columnas innecesarias


            if (dtRep.Columns.Contains("RID"))
            {
                dtRep.Columns.Remove("RID");
            }

            if (dtRep.Columns.Contains("RowNumber"))
            {
                dtRep.Columns.Remove("RowNumber");
            }

            if (dtRep.Columns.Contains("TopRID"))
            {
                dtRep.Columns.Remove("TopRID");
            }


            #endregion

            #region Cambia nombres Columnas
            if (dtRep.Columns.Contains("Fecha"))
            {
                dtRep.Columns["Fecha"].ColumnName = "Día Procesado";
            }


            if (dtRep.Columns.Contains("Llamadas"))
            {
                dtRep.Columns["Llamadas"].ColumnName = "Total de Llamadas";
            }

            #endregion

            string[] nombresSitios = FCAndControls.extraeNombreColumnas(dtRep).Where(i => (i != "Día Procesado" && i != "Total de Llamadas")).ToArray<string>();
            string[] nombresColumnas = new string[] { "Día Procesado" }.AsEnumerable().Concat(nombresSitios).Concat(new string[] { "Total de Llamadas" }).ToArray();
            List<string> listaFormtos = new List<string>();
            listaFormtos.Add("");

            for (int i = 1; i <= nombresColumnas.Count(); i++)
            {
                listaFormtos.Add("{0:0,0}");
            }

            dtRep = dtvRep.ToTable(false, nombresColumnas);

            if (dtRep.Rows.Count > 0 && dtRep.Columns.Count > 0)
            {
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
                grdV = DTIChartsAndControls.GridView("ReporteMatricialDiasProcesados", dtRep, true, "Totales", listaFormtos.ToArray<string>());

                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                               grdV,
                                                "ReporteMatricialDiasProcesados_T", tituloGrid));
                #endregion

            }

        }




        public void RepTabReporteLlamadasEntradasFiltroExten(Control contenedorGrid, string tituloGrid, string urlSigNivel)
        {
            DataTable dtRep = DSODataAccess.Execute(ConsultaReporteLlamadasEntradasFiltroExten());


            List<string> listaformatos = new List<string>();
            if (dtRep.Rows.Count > 0)
            {

                DataView dtv = new DataView(dtRep);

                string[] columnas = { "Tdest", "Extension", "Responsable", "CantLlams", "CantMin", "CantNumDist" };
                dtRep = dtv.ToTable(false, columnas);

                int tdest = Convert.ToInt32(dtRep.Rows[0]["Tdest"].ToString());


                #region Eliminar Columnas innecesarias
                //if (dtRep.Columns.Contains("Tdest")) { dtRep.Columns.Remove("Tdest"); }
                #endregion

                #region Cambia nombres Columnas

                if (dtRep.Columns.Contains("Extension")) { dtRep.Columns["Extension"].ColumnName = "Extensión"; }
                if (dtRep.Columns.Contains("CantLlams")) { dtRep.Columns["CantLlams"].ColumnName = "Cant. Llamadas"; }
                if (dtRep.Columns.Contains("CantMin")) { dtRep.Columns["CantMin"].ColumnName = "Cant. Minutos"; }
                if (dtRep.Columns.Contains("CantNumDist")) { dtRep.Columns["CantNumDist"].ColumnName = "Cant. números  distintos"; }

                #endregion

                listaformatos = new List<string>();

                for (int i = 1; i <= dtRep.Columns.Count; i++)
                {
                    if (dtRep.Columns[i - 1].ColumnName.ToLower().Contains("cant"))
                    {
                        listaformatos.Add("{0:0,0}");
                    }
                    else
                    {
                        listaformatos.Add("");
                    }

                }


            }


            string[] urlFields = { "Tdest", "Extensión" };
            int[] indexmostrarEnWeb = { 0 };
            int[] indexColNoNavegacion = { 2, 3, 4, 5 };
            int[] indexColConNavegacion = { 1 };

            #region Reporte
            GridView grdV = new GridView();
            grdV = DTIChartsAndControls.GridView
                (
                    "ReporteLlamsEntradaFiltroPorExten",
                    dtRep,
                    true,
                    "Totales",
                    listaformatos.ToArray<string>(),
                    urlSigNivel,
                    urlFields,
                    1,
                    indexmostrarEnWeb,
                    indexColNoNavegacion,
                    indexColConNavegacion,
                    2
                );

            contenedorGrid.Controls.Add(
                                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                                                                                 grdV,
                                                                                                "ReporteLlamsEntradaFiltroPorExten_T",
                                                                                                tituloGrid
                                                                                            )
                                        );
            #endregion


        }

        private void RepTabIndicadorExtensionesNoAsignadas2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                    Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepIndicadorExtensionesNoAsignadas());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }



            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabIndExtensionesNoAsignadasGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                "",
                                new string[] { "Extensión", "Codigo Sitio" }, 0,
                                new int[] { 5, 6 }, new int[] { 0, 1, 2, 3, 4 }, new int[] { }),
                                "RepTabIndExtensionesNoAsignadas2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Importe" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabIndExtensionesNoAsignadasGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabIndExtensionesNoAsignadasGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabIndExtensionesNoAsignadasGraf_G", "", "", "Extensión", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabIndicadorCodigosNoAsignados2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                          Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepIndicadorCodigosNoAsignados());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Total", "Numero", "Duracion", "Nombre Sitio", "Codigo Sitio" });
                ldt.Columns["Codigo Autorizacion"].ColumnName = "Código de Autorización";
                ldt.Columns["Total"].ColumnName = "Importe";
                ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabIndCodigosNoAsignadosGrid_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:0,0}", "{0:0,0}", "", "" },
                                "",
                                new string[] { "Código de Autorización", "Codigo Sitio" }, 0,
                                new int[] { 5, 6 }, new int[] { 0, 1, 2, 3, 4 }, new int[] { }),
                                "RepTabIndCodigosNoAsignados2Pnls_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Código de Autorización", "Importe" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Código de Autorización"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Código de Autorización"].ColumnName = "label";
                ldt.Columns["Importe"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabIndCodigosNoAsignadosGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabIndCodigosNoAsignadosGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabIndCodigosNoAsignadosGraf_G", "", "", "Código de Autorización", "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        public void RepLineasExcedentesTelcel2pnl(Control contenedorGrid, string tituloGrid, Control contenedorGrafica, string tituloGrafica, string link)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaExcedenteLineasTelcel(link));
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Linea", "Linea", "Nombre Completo", "Nombre Centro de Costos", "Renta", "Excedente", "Porcentaje", "Total", "link" });
                ldt.Columns["Linea"].ColumnName = "Línea";
                ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costo";
                ldt.Columns["Porcentaje"].ColumnName = "Service Tag";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLineasExcedenteGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:c}", "{0:0,0}", "{0:c}" },
                                Request.Path + "?Nav=PorConceptoN2&Linea={1}&NumMarc={0}",
                                new string[] { "Línea", "Codigo Linea" }, 1,
                                new int[] { 0, 8 }, new int[] { 2, 3, 4, 5, 6, 7 }, new int[] { 1 }),
                                "RepTabLineasExcedenteGrid_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Línea", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Línea"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLineasExcedenteGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLineasExcedenteGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLineasExcedenteGraf_G", "", "", "Línea", "Total", 2, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepPorCategoriaExcedente2pnl(Control contenedor, string link, Control contenedorGrafica)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepPorCategoria(link));
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                #region Reporte Concepto
                //param.Linea = dt.Rows[0]["Linea"].ToString();

                DataView dvdt = new DataView(dt);
                dt = dvdt.ToTable(false, new string[] { "idConcepto", "Concepto", "Total", "Codigo Linea", "Linea", "link" });
                dt.AcceptChanges();

                GridView grid = DTIChartsAndControls.GridView("RepPorCategoria", dt, true, "Totales",
                        new string[] { "", "", "{0:c}", "", "", "" }, Request.Path + "?Nav=PorConceptoN3&Level={0}&Concepto={2}&Linea={1}",
                        new string[] { "Concepto", "Codigo Linea", "idConcepto" }, 1,
                        new int[] { 0, 3, 5 }, new int[] { 2 }, new int[] { 1 });

                //grid.RowDataBound += (sender, e) => cambiaURLFunctionJava_RowDataBound(sender, e);
                grid.DataBind();
                grid.UseAccessibleHeader = true;
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;

                GridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[1].Text = "Totales";
                for (int tot = 0; tot < row.Cells.Count; tot++)
                {
                    row.Cells[tot].CssClass = "lastRowTable";
                }

                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "RepPorCategoria", "Por categoría", string.Empty));

                #endregion

                #region Grafica

                //dt.Columns.Add("link");
                //foreach (DataRow ldr in dt.Rows)
                //{
                //    ldr["link"] = string.Format(link, ldr["Linea"].ToString(), ldr["Codigo Linea"].ToString(), ldr["Codigo Carrier"].ToString(), ldr["idConcepto"].ToString())
                //        .Replace("javascript.", "javascript:").Replace(";", "");
                //}

                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Concepto", "Total", "link" });
                if (dt.Rows[dt.Rows.Count - 1]["Concepto"].ToString() == "Totales")
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                }
                dt.Columns["Concepto"].ColumnName = "label";
                dt.Columns["Total"].ColumnName = "value";
                dt.AcceptChanges();

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepPorCategoriaGraf", "Por categoría"));

                ScriptManager.RegisterStartupScript(this, typeof(Page), "RepPorCategoriaGraf",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
                    "RepPorCategoriaGraf", "", "", "", "Importe", "bar2d"), false);

                #endregion

            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepConsumoPorCategoria", "Consumo por categoría", string.Empty));
            }
        }
        private void RepPorConcepto(Control Contenedor, string link)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepPorConceptoDesglose());
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvRepDesgloseFacturaPorConcepto = new DataView(dt);
                dt = dvRepDesgloseFacturaPorConcepto.ToTable(false, new string[] { "Concepto", "Descripcion", "Total" });
                dt.Columns["Descripcion"].ColumnName = "Descripción";
                dt.AcceptChanges();

                #region Seleccion de reporte

                if (dt.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                {
                    Contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                                DTIChartsAndControls.GridView("RepPorConcepto", DTIChartsAndControls.ordenaTabla(dt, "Total desc"),
                                true, "Totales", new string[] { "", "", "{0:c}" }), "RepPorConcepto", "Por Concepto", string.Empty));
                }
                else
                {
                    DataTable dtLlams = DSODataAccess.Execute(ConsultaRepPorConceptoDetalleLamadas());

                    if (dtLlams.Rows.Count > 0 && dtLlams.Columns.Count > 0)
                    {
                        DataView dvRepDesgloseFacturaPorLlamada = new DataView(dtLlams);
                        dtLlams = dvRepDesgloseFacturaPorLlamada.ToTable(false,
                            new string[] { "Fecha Llamada", "Hora Llamada", "Numero Marcado", "Duracion Minutos", "Costo", "Punta A", "Dir Llamada", "Punta B" });
                        dtLlams.Columns["Fecha Llamada"].ColumnName = "Fecha";
                        dtLlams.Columns["Hora Llamada"].ColumnName = "Hora";
                        dtLlams.Columns["Numero Marcado"].ColumnName = "Número marcado";
                        dtLlams.Columns["Duracion Minutos"].ColumnName = "Minutos";
                        dtLlams.Columns["Costo"].ColumnName = "Total";
                        dtLlams.Columns["Punta A"].ColumnName = "Localidad Origen";
                        dtLlams.Columns["Dir Llamada"].ColumnName = "Tipo";
                        dtLlams.Columns["Punta B"].ColumnName = "Localidad Destino";
                        dtLlams.AcceptChanges();
                    }

                    Contenedor.Controls.Clear();
                    Contenedor.Controls.Add(
                     DTIChartsAndControls.TituloYBordeRepSencilloTabla(
                           DTIChartsAndControls.GridView("RepDesgloseFacturaPorLlamada", DTIChartsAndControls.ordenaTabla(dtLlams, "Total desc"),
                           true, "Totales", new string[] { "", "", "", "", "{0:c}", "", "", "" }), "RepDesgloseLlamadas", "Desglose de llamadas", string.Empty));
                }

                #endregion Seleccion de reporte
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepPorConcepto", "Por Concepto", string.Empty));

            }
        }

        private void RepMinutosPorCarrier(Control contenedor, Control contenedorGrafica)
        {
            try
            {

                string linkGraf = "[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Carrier=" + "'' + convert(varchar,[Codigo Carrier])";

                DataTable dt = new DataTable();
                dt = DSODataAccess.Execute(ConsultaRepMinutosPorCarrier(linkGraf));


                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {

                    /*Elimina Col innecesarias*/

                    if (dt.Columns.Contains("RID"))
                    {
                        dt.Columns.Remove("RID");
                    }

                    if (dt.Columns.Contains("RowNumber"))
                    {
                        dt.Columns.Remove("RowNumber");
                    }

                    if (dt.Columns.Contains("TopRID"))
                    {
                        dt.Columns.Remove("TopRID");
                    }


                    DataTable dtGraf = new DataTable();
                    DataView dv = new DataView(dt);
                    dt = dv.ToTable(false, new string[] { "Codigo Carrier", "Nombre Carrier", "Duracion", "Total", "Numero", "link" });
                    dtGraf = dv.ToTable(false, new string[] { "Nombre Carrier", "Duracion", "link" });

                    dt = DTIChartsAndControls.ordenaTabla(dt, "Nombre Carrier");
                    dtGraf = DTIChartsAndControls.ordenaTabla(dtGraf, "Nombre Carrier");

                    if (dt.Columns.Contains("Nombre Carrier"))
                    {
                        dt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                        dt.AcceptChanges();
                    }
                    if (dt.Columns.Contains("Total"))
                    {
                        dt.Columns["Total"].ColumnName = "Importe";
                        dt.AcceptChanges();
                    }
                    if (dt.Columns.Contains("Numero"))
                    {
                        dt.Columns["Numero"].ColumnName = "Llamadas";
                        dt.AcceptChanges();
                    }
                    if (dt.Columns.Contains("Duracion"))
                    {
                        dt.Columns["Duracion"].ColumnName = "Minutos";
                        dt.AcceptChanges();
                    }
                    #region Tabla

                    contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepMinPorCarrier", DTIChartsAndControls.ordenaTabla(dt, "Carrier"), true, "Totales",
                            new string[] { "", "", "{0:0,0}", "{0:c}", "{0:0,0}", "" }, Request.Path + "?Nav=EmpleMCN2&Carrier={0}",
                            new string[] { "Codigo Carrier" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }), "RepMinPorCarrier", "Minutos por carrier"));

                    #endregion


                    #region Grafica
                    if (dtGraf.Columns.Contains("Nombre Carrier"))
                    {
                        dtGraf.Columns["Nombre Carrier"].ColumnName = "label";
                        dtGraf.AcceptChanges();
                    }

                    if (dtGraf.Columns.Contains("Duracion"))
                    {
                        dtGraf.Columns["Duracion"].ColumnName = "value";
                        dtGraf.AcceptChanges();
                    }

                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloGrafica("RepMinPorCarrierGraf", "Minutos por Carrier"));

                    ScriptManager.RegisterStartupScript(this, typeof(Page), "RepMinPorCarrierGraf",
                        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtGraf),
                        "RepMinPorCarrierGraf", "", "", "Carrier", "Minutos", "bar2d", ""), false);
                    #endregion
                }
                else
                {
                    Label sinInfo = new Label();
                    sinInfo.Text = "No hay información por mostrar";
                    contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepPorConcepto", "Por Concepto", string.Empty));

                    contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepPorConcepto", "Por Concepto", string.Empty));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void RepAccesEtiquetacion(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaRepEtiquetacion());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Nombre Centro de Costos2", "Gasto Laboral", "Gasto Personal", "Gasto Por Identificar", "Gasto Total", "Fecha Ultima Etiqueta", "Fecha Ultimo Acceso" });
                dt.Columns["Nombre Centro de Costos2"].ColumnName = "Centro de Costos";
                dt.Columns["Nombre Completo"].ColumnName = "Empleado";
                dt.Columns["Fecha Ultima Etiqueta"].ColumnName = "Ultima Etiquetación";
                dt.Columns["Fecha Ultimo Acceso"].ColumnName = "Ultimo Acceso";
                dt = DTIChartsAndControls.ordenaTabla(dt, "Gasto Total Desc");
                dt.AcceptChanges();

                contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepTabLineasExcedenteGrid_T", dt, true, "Totales",
                            new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "", "" },
                             "",
                            new string[] { }, 0,
                            new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new int[] { }),
                            "RepTabLineasExcedenteGrid_T", tituloGrid));
            }
        }

        private void RepTabEmpsConLlamsACel10Digs1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string nombreColumnaColaborador = "Colaborador";
            int numeroRegistros = int.MaxValue;

            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(ConsultaEmpsConLlamsACel10Digs("[link] = ''" + Request.Path + "?Nav=EmpleMCN2&Emple='' + convert(varchar,[Codigo Empleado])",
                10));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                new string[] { "Codigo Empleado", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "link" });

                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
            DTIChartsAndControls.TituloYPestañasRep1Nvl(
                            DTIChartsAndControls.GridView(
                            "RepTabEmpsConLlamsACelGrid", DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", numeroRegistros), true, "Totales",
                            new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}",
                            new string[] { "Codigo Empleado" }, 1, new int[] { 0 },
                            new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabEmpsConLlamsACel1Pnl", tituloGrid, Request.Path + "?Nav=EmpleCel10DigsN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
            );


            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { nombreColumnaColaborador, "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns[nombreColumnaColaborador].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabEmpsConLlamsACel1Pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", numeroRegistros)),
                "RepTabEmpsConLlamsACel1Pnl", "", "", nombreColumnaColaborador, "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica

        }

        private void RepTabEmpsConLlamsACel10Digs2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaEmpsConLlamsACel10Digs(linkGrafica, int.MaxValue));

            string nombreColumnaColaborador = "Colaborador";
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
                if (DSODataContext.Schema.ToLower() == "k5banorte")
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

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);

                if (DSODataContext.Schema.ToLower() == "k5banorte")
                {
                    ldt.Columns["No Nomina"].ColumnName = "Nomina";
                }

                ldt.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                ldt.AcceptChanges();
            }

            if (DSODataContext.Schema.ToLower() == "k5banorte")
            {

                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(   //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                                DTIChartsAndControls.GridView("RepTabEmpsConLlamsACelGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 8 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabEmpsConLlamsACel2Pnls_T", tituloGrid)
                );
            }
            else
            {
                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabEmpsConLlamsACelGrid", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabEmpsConLlamsACel2Pnls_T", tituloGrid)
                );
            }

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabEmpsConLlamsACel_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabEmpsConLlamsACel_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabEmpsConLlamsACel_G", "", "", nombreColumnaColaborador, "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }

        public void ReportePorEmpleConJer(Control contenedor, string tituloGrid)
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaReportePorEmpleConJer());

                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {

                    /*Elimina Columna innecesarias*/
                    if (dt.Columns.Contains("Nivel"))
                    {
                        dt.Columns.Remove("Nivel");
                    }

                    //if (dt.Columns.Contains("Emple"))
                    //{
                    //    dt.Columns.Remove("Emple");
                    //}

                    if (dt.Columns.Contains("CenCos"))
                    {
                        dt.Columns.Remove("CenCos");
                    }

                    dt.AcceptChanges();

                    /*Renombrar Columnas*/
                    if (dt.Columns.Contains("Jefe"))
                    {
                        dt.Columns["Jefe"].ColumnName = "Jefe Directo";
                    }

                    if (dt.Columns.Contains("NominaA"))
                    {
                        dt.Columns["NominaA"].ColumnName = "Nómina";
                    }

                    if (dt.Columns.Contains("NomCompleto"))
                    {
                        dt.Columns["NomCompleto"].ColumnName = "Colaborador";
                    }

                    if (dt.Columns.Contains("CenCosDesc"))
                    {
                        dt.Columns["CenCosDesc"].ColumnName = "Centro costos";
                    }

                    if (dt.Columns.Contains("Costo"))
                    {
                        dt.Columns["Costo"].ColumnName = "Importe";
                    }

                    if (dt.Columns.Contains("Dur"))
                    {
                        dt.Columns["Dur"].ColumnName = "Minutos";
                    }

                    if (dt.Columns.Contains("Llams"))
                    {
                        dt.Columns["Llams"].ColumnName = "Llamadas";
                    }
                    dt = DTIChartsAndControls.ordenaTabla(dt, "Importe Desc");

                    dt.AcceptChanges();
                    /*GRID*/

                    Session["ReportePorEmpleConJer"] = 1;
                    contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                    "RepTabReportePorEmpleConJerGrid", dt, true, "Totales",
                                    new string[] { "", "", "", "{0:c}", "{0:0}", "{0:0,0}" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}&EmpleConJer=1",
                                    new string[] { "Emple" }, 1, new int[] { 0 },
                                    new int[] { 1, 3, 4, 5 }, new int[] { 2 }), "RepTabReportePorEmpleConJerGrid_T", tituloGrid));
                }
                else
                {
                    Label sinInfo = new Label();
                    sinInfo.Text = "No hay información por mostrar";
                    contenedor.Controls.Add
                        (
                                            DTIChartsAndControls.TituloYBordeRepSencilloTabla
                                            (
                                                sinInfo,
                                                "ReportePorEmpleConJer",
                                                tituloGrid,
                                                string.Empty
                                            )
                       );
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RepTabTotLlamNoContestadas(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadas());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "ClaveSitio", "SitioDesc", "Total", "link" });
                ldt.Columns["SitioDesc"].ColumnName = "Sitio";
                ldt.Columns["Total"].ColumnName = "Total de Llamdas No Contestadas";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Llamdas No Contestadas Desc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("ReptabTotLlamNoContestadas1pnl", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "" }, Request.Path + "?Nav=HistoricoN2&Sitio={0}",
                                      new string[] { "ClaveSitio" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "ReptabTotLlamNoContestadas1pnl", tituloGrid, Request.Path + "?Nav=RepLlamPerdidas2pnl", pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total de Llamdas No Contestadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total de Llamdas No Contestadas"].ColumnName = "value";

                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReptabTotLlamNoContestadas1pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "ReptabTotLlamNoContestadas1pnl", "", "", "Sitio", "Total de Llamdas No Contestadas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        public void RepTabTotLlamNoContestadas2Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadas());
            #region Grid
            LlamPerd.Visible = true;
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "ClaveSitio", "SitioDesc", "Total", "link" });
                ldt.Columns["SitioDesc"].ColumnName = "Sitio";
                ldt.Columns["Total"].ColumnName = "Total de Llamdas No Contestadas";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Llamdas No Contestadas Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamNocontestadasGrid_T", ldt, true, "Totales",
                                new string[] { "", "", "{0:0,0}", "" },
                                Request.Path + "?Nav=RepLlamPerdidasN22Pnl&Sitio={1}",
                                new string[] { "Sitio", "ClaveSitio" }, 1,
                                new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                "RepTabLlamNocontestadasGrid_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total de Llamdas No Contestadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total de Llamdas No Contestadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamNoConteatadasGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamNoConteatadasGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamNoConteatadasGraf_G", "", "", "Sitio", "Total de Llamdas No Contestadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        public void RepTabTotLlamNoContestadasN22Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsutaReporteLlamNoContestadasN2());
            LlamPerd.Visible = true;
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatalogo", "NomCompleto", "Descripcion", "Total", "link" });
                ldt.Columns["NomCompleto"].ColumnName = "Colaborador";
                ldt.Columns["Descripcion"].ColumnName = "Centro de Costos";
                ldt.Columns["Total"].ColumnName = "Total";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabLlamNocontestadasN2Grid_T", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:0,0}", "" },
                                Request.Path + "?Nav=RepLlamPerdidasN3&Emple={1}",
                                new string[] { "Colaborador", "iCodCatalogo" }, 1,
                                new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }),
                                "RepTabLlamNocontestadasN2Grid_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabLlamNoConteatadasN2Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabLlamNoConteatadasN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabLlamNoConteatadasN2Graf_G", "", "", "Colaborador", "Total", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        public void RepTabTotLlamNoContestadasN3(Control contenedor, string tituloGrid)
        {
            LlamPerd.Visible = true;
            DataTable dt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadasN3(1));
            if (dt != null && dt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "SitioDesc", "FechaInicio", "NoMarca", "ExtOriginal", "EmpleOrig", "ExteDesvia", "EmpleDesvia", "ExteFinal", "EmpleFinal" });
                dt.Columns["SitioDesc"].ColumnName = " Sitio ";
                dt.Columns["FechaInicio"].ColumnName = " Fecha ";
                dt.Columns["NoMarca"].ColumnName = " No. que Marca ";
                dt.Columns["ExtOriginal"].ColumnName = " Ext Original ";
                dt.Columns["EmpleOrig"].ColumnName = " Empleado Origen ";
                dt.Columns["ExteDesvia"].ColumnName = " Exte que Desvia ";
                dt.Columns["EmpleDesvia"].ColumnName = " Empleado que Desvia ";
                dt.Columns["ExteFinal"].ColumnName = " Ext Final ";
                dt.Columns["EmpleFinal"].ColumnName = " Empleado Final ";
                dt = DTIChartsAndControls.ordenaTabla(dt, "Fecha Desc");
                dt.AcceptChanges();



                contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepTabDetallLlamPerdidasGrid_T", dt, false, "",
                                            new string[] { "", "", "", "", "", "", "", "", "" }),
                                            "RepTabDetallLlamPerdidasGrid_T", tituloGrid)
                            );


            }
        }
        #region Siana SEVEN ELEVEN
        private void RepTabPorSitioSiana1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorSitioSiana("[link] = ''" + Request.Path + "?Nav=SitioSianaN2&Sitio='' + convert(varchar,[Codigo Sitio])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioSianaTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=SitioSianaN2&Sitio={0}",
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                      "GrafConsPorSitioSianaTab1", tituloGrid, Request.Path + "?Nav=SitioSianaN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioSianaTab2",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "GrafConsPorSitioSianaTab1", "", "", "Sitio", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);


            #endregion Grafica
        }
        private void RepTabPorSitioSiana2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                             Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorSitioSiana(linkGrafica));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Total", "Numero", "Duracion", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";        //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";       //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";
                ldt.AcceptChanges();

                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                }

            }


            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioGrid", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 5 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                      "RepTabPorSitio2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorSitioGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));


            //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10
            if (DSODataContext.Schema.ToLower() == "ula")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioGraf_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "GrafConsPorSitioGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioGraf_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafConsPorSitioGraf_G", "", "", "Sitio", "Importe", grafActiva, FCGpoGraf.Tabular), false);
            }


            #endregion Grafica
        }
        private void RepTabPorEmpleMasCarosSiana2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaPorEmpleMasCarosSiana(linkGrafica, int.MaxValue));

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

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosSianaGrid", DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), true, "Totales",
                                new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 8 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCarosSiana2Pnls_T", tituloGrid)
                );
            }
            else
            {
                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepConsEmpleMasCarosSianaGrid", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, linkGrid,
                                new string[] { "Codigo Empleado" }, 1, new int[] { 0, 6 },
                                camposBoundField, new int[] { hyperLinkFieldIndex }), "RepTabPorEmpleMasCaros2SianaPnls_T", tituloGrid)
                );
            }

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
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


            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepConsEmpleMasSianaCaros_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepConsEmpleMasSianaCaros_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepConsEmpleMasSianaCaros_G", "", "", nombreColumnaColaborador, "Importe", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica
        }
        private void RepTabPorSianaTDest2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                Control contenedorGrid, string tituloGrid, int[] camposLink)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorTipoDestinoSiana(linkGrafica));


            int hyperLinkFieldIndex = 1;
            string[] camposReporte;
            string[] camposGrafica;
            string[] formatoColumna;
            /*bandera que identifica si se desglosara el costo de la llamada*/
            int desglosaCosto = 0;
            if (Session["DesgloseCosto"] != null)
            {
                desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
            }


            if (string.IsNullOrEmpty(linkGrid))
            {
                hyperLinkFieldIndex = 100; //Indice que no se encuentra en el arreglo y por lo tanto no incluye un hyperlink

                if (desglosaCosto == 1)
                {
                    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" };
                    formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                }
                else
                {
                    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" };
                    formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                }

                camposGrafica = new string[] { "Tipo de destino", "Total" };
            }
            else
            {

                if (desglosaCosto == 1)
                {
                    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion", "link" };
                    formatoColumna = new string[] { "", "", "{0:c}", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}" };
                }
                else
                {
                    camposReporte = new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion", "link" };
                    formatoColumna = new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                }

                camposGrafica = new string[] { "Tipo de destino", "Total", "link" };
            }

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposReporte);
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt.Columns["Total"].ColumnName = "Total";

                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                }
            }


            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GrafConsPorTDestGrid", ldt, true, "Totales",
                                      formatoColumna, linkGrid,
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 5 }, camposLink, new int[] { hyperLinkFieldIndex }),
                                      "RepTabPorTDest2Pnls_T", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, camposGrafica);

                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de destino"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsPorTDestGraf_G", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTDestGraf_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafConsPorTDestGraf_G", "", "", "Tipo de destino", "Importe", grafActiva, FCGpoGraf.Tabular, "$", "", "dti", "98%", "385"), false);

            #endregion Grafica
        }
        private void ReporteDetalladoSiana(Control contenedor, string tituloGrid)
        {
            //NZ Crea el contenedor de la tabla.
            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepDetalleSoloTabla("ReporteDetallado", tituloGrid));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "funcionDetalleFija", "<script language=javascript> GetDatosTabla('ReporteDetallado', 'ReporteDetalladoWebMSiana'); </script>", false);
        }

        [WebMethod]
        public static object ReporteDetalladoWebMSiana()
        {
            try
            {
                DataTable dt = DSODataAccess.Execute(ConsultaDetalleSiana());

                if (dt.Rows.Count > 0)
                {
                    int[] columnasNoVisibles = null;
                    int[] columnasVisibles = null;

                    DataView dvldt = new DataView(dt);
                    RemoveColHerencia(ref dt);

                    dt = DTIChartsAndControls.ordenaTabla(dt, "[TotalSimulado] desc");

                    ArrayList lista = FormatoColumRepDetallado(dt, columnasNoVisibles, columnasVisibles);
                    dt = (DataTable)((object)lista[0]);
                    columnasNoVisibles = (int[])((object)lista[1]);
                    columnasVisibles = (int[])((object)lista[2]);

                    //20150702 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                    CambiarNombresConfig(dt, "ConsultaDetalle");

                    if (dt.Columns.Contains("Duracion"))
                        dt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"             
                    if (dt.Columns.Contains("Llamadas"))
                        dt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";

                    for (int i = columnasNoVisibles.Length - 1; i >= 0; i--)
                    {
                        dt.Columns.RemoveAt(columnasNoVisibles[i]);
                    }

                    return FCAndControls.ConvertToJSONStringDetalle(dt);
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en Dashboard de Fija:" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }
        #endregion

        #region BOOKING&MANAGMENT
        public void RepTabSeeYouOnUtilCliente(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasCliente());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Client", "Cliente", "Utilizacion", "Cantidad", "link", "UtilizacionGraf" });
                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepTabSeeYouOnUtilCliente1pnl", ldt, true, "Totales",
                                      new string[] { "", "", "", "{0:0,0}" }, Request.Path + "?Nav=RepTabSeeYouOnUtilClienteN2&Client={1}",
                                      new string[] { "Cliente", "Client" }, 1, new int[] { 0, 4, 5 }, new int[] { 2, 3 }, new int[] { 1 }),
                                      "RepTabSeeYouOnUtilCliente1pnl", tituloGrid, Request.Path + "?Nav=RepTabSeeYouOnUtilCliente2pnl", pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica
            DataTable ldt1 = DSODataAccess.Execute(ConsultaUtilSistemasCliente());
            if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt1);
                ldt1 = dvldt.ToTable(false, new string[] { "Cliente", "UtilizacionGraf", "link" });
                if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                {
                    ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                }
                ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "UtilizacionGraf Desc");
                ldt1.Columns["Cliente"].ColumnName = "label";
                ldt1.Columns["UtilizacionGraf"].ColumnName = "value";
                ldt1.AcceptChanges();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilCliente1pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10)), "RepTabSeeYouOnUtilCliente1pnl", "", "", "Sistema", "Horas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);
            }



            #endregion Grafica
        }
        public void RepTabSeeYouOnUtilCliente2pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasCliente());
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Client", "Cliente", "Utilizacion", "Cantidad", "link" });
                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc");
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabSeeYouOnUtilCliente_T", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:0,0}" },
                                Request.Path + "?Nav=RepTabSeeYouOnUtilClienteN2&Client={1}",
                                new string[] { "Cliente", "Client" }, 1,
                                new int[] { 0, 4 }, new int[] { 2, 3 }, new int[] { 1 }),
                                "RepTabSeeYouOnUtilCliente_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica
            DataTable ldt1 = DSODataAccess.Execute(ConsultaUtilSistemasCliente());
            if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt1);
                ldt1 = dvldt.ToTable(false, new string[] { "Cliente", "UtilizacionGraf", "link" });
                if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                {
                    ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                }
                ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "UtilizacionGraf Desc");
                ldt1.Columns["Cliente"].ColumnName = "label";
                ldt1.Columns["UtilizacionGraf"].ColumnName = "value";
                ldt1.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabSeeYouOnUtilClienteGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilClienteGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10), "value desc", 10)),
                "RepTabSeeYouOnUtilClienteGraf_G", "", "", "Sistema", "Horas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        public void RepTabSeeyouOnUtilClienteN2(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasClienteN2());
            if (ldt != null && ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "VCSSourceSystem", "Utilizacion", "Cantidad" });
                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["Sistema"].ColumnName = "Sistema";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Número de Eventos Desc");
                ldt.AcceptChanges();


                contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepTabSeeyouOnUtilClienteN2Grid_T", ldt, true, "Total",
                                            new string[] { "", "", "", "", "{0:0,0}" }, Request.Path + "?Nav=RepTabSeeYouOnUtilClienteN3&Sistema={1}",
                                            new string[] { "Sistema", "VCSSourceSystem" }, 0, new int[] { 2 }, new int[] { 0, 3, 4 }, new int[] { 1 }),
                                            "RepTabSeeyouOnUtilClienteN2Grid_T", tituloGrid)
                            );


            }
        }
        public void RepTabSeeyouOnUtilClienteN3(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasClienteN3());
            if (ldt != null && ldt.Rows.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "FechaInicio", "VCSName", "NetworkAddress", "DuracionSeg", "SourceNumber", "DestinationNumber", "Bandwidth" });
                ldt.Columns["FechaInicio"].ColumnName = "Fecha";
                ldt.Columns["VCSName"].ColumnName = "Nombre Sistema";
                ldt.Columns["NetworkAddress"].ColumnName = "Dirección de Red";
                ldt.Columns["DuracionSeg"].ColumnName = "Duración (Seg)";
                ldt.Columns["SourceNumber"].ColumnName = "Origen";
                ldt.Columns["DestinationNumber"].ColumnName = "Destino";
                ldt.Columns["Bandwidth"].ColumnName = "Ancho de Banda";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Fecha Asc");
                ldt.AcceptChanges();


                contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepTabSeeyouOnUtilClienteN3Grid_T", ldt, false, "",
                                            new string[] { "", "", "", "", "", "", "{0:0,0}" }),
                                            "RepTabSeeyouOnUtilClienteN3Grid_T", tituloGrid)
                            );


            }
        }
        public void RepTabSeeYounOnUtilSistema(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistema());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "VCSSourceSystem", "Utilizacion", "Cantidad", "link" });
                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["Sistema"].ColumnName = "Sistema";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc");
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepTabSeeYouOnUtilSistema1pnl", ldt, true, "Totales",
                                      new string[] { "", "", "", "", "{0:0,0}" }, Request.Path + "?Nav=RepTabSeeYouOnUtilClienteN3&Sistema={1}",
                                       new string[] { "Sistema", "VCSSourceSystem" }, 0, new int[] { 2, 5 }, new int[] { 0, 3, 4 }, new int[] { 1 }),
                                      "RepTabSeeYouOnUtilSistema1pnl", tituloGrid, Request.Path + "?Nav=RepTabSeeYouOnUtilSistema2pnl", pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica
            DataTable ldt1 = ldt;
            if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
            {
                DataView dvldt1 = new DataView(ldt1);
                ldt1 = dvldt1.ToTable(false, new string[] { "Cliente", "Sistema", "Número de Eventos", "link" });
                if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                {
                    ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                }
                ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "UtilizacionGraf Desc");
                ldt1.AcceptChanges();

                ldt1.Columns["Sistema"].ColumnName = "label";
                ldt1.Columns["Número de Eventos"].ColumnName = "value";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilSistema1pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10)), "RepTabSeeYouOnUtilSistema1pnl", "", "", "Sistema", "Eventos", pestaniaActiva, FCGpoGraf.Tabular, ""), false);
            }



            #endregion Grafica
        }
        public void RepTabSeeYounOnUtilSistema2Pnl(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistema());
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "VCSSourceSystem", "Utilizacion", "Cantidad", "link" });
                ldt.Columns["Cliente"].ColumnName = "Cliente";
                ldt.Columns["Sistema"].ColumnName = "Sistema";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepTabSeeYouOnUtilSistema_T", ldt, true, "Totales",
                                 new string[] { "", "", "", "", "{0:0,0}" },
                                Request.Path + "?Nav=RepTabSeeYouOnUtilSistemaN2&Sistema={1}",
                                new string[] { "Sistema", "VCSSourceSystem" }, 0,
                                new int[] { 2, 5 }, new int[] { 0, 3, 4 }, new int[] { 1 }),
                                "RepTabSeeYouOnUtilSistema_T", tituloGrid)
                );

            #endregion Grid
            #region Grafica
            DataTable ldt1 = ldt;
            if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt1);
                ldt1 = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "Número de Eventos", "link" });
                if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                {
                    ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                }
                ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "Número de Eventos Desc");
                ldt1.Columns["Sistema"].ColumnName = "label";
                ldt1.Columns["Número de Eventos"].ColumnName = "value";
                ldt1.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabSeeYouOnUtilSistemaGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilSistemaGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10), "value desc", 10)),
                "RepTabSeeYouOnUtilSistemaGraf_G", "", "", "Sistema", "Eventos", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }
        public void RepTabSeeYouOnUtilSistemaHist(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemaHistorico());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Utilizacion" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepTabSeeYouOnUtilSistemaHist1pnl", ldt, false, "",
                                      new string[] { "", "", }),
                                      "RepTabSeeYouOnUtilSistemaHist1pnl", tituloGrid, Request.Path + "?Nav=RepTabSeeYouOnUtilSistemaHist2pnl", pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt1 = new DataView(ldt);
                ldt = dvldt1.ToTable(false, new string[] { "Mes", "Utilización" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Utilización"].ColumnName = "value";

                ldt.AcceptChanges();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilSistemaHist1pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(ldt), "RepTabSeeYouOnUtilSistemaHist1pnl", "", "", "Mes", "Utilización", pestaniaActiva, FCGpoGraf.Tabular, ""), false);
            }
            #endregion Grafica
        }
        public void RepTabSeeYouOnUtilSistemaHist2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemaHistorico());
            #region grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Utilizacion" });
                ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                ldt.AcceptChanges();

                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabSeeYouOnUtilSistemaHist_T", ldt, false, "",
                     new string[] { "", "" }),
                    "RepTabSeeYouOnUtilSistemaHist_T", tituloGrid));
            }
            #endregion
            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt1 = new DataView(ldt);
                ldt = dvldt1.ToTable(false, new string[] { "Mes", "Utilización" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.AcceptChanges();

                ldt.Columns["Mes"].ColumnName = "label";
                ldt.Columns["Utilización"].ColumnName = "value";

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabSeeYouOnUtilSistemaGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnUtilSistemaHist_G",
                    FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(ldt),
                    "RepTabSeeYouOnUtilSistemaGraf_G", "", "", "Mes", "Utilización", 2, FCGpoGraf.Tabular, ""), false);

            }
            #endregion
        }
        public void RepTabSeeYouOnHorasHombreCencos(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCencosHorasHombre());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cencosdesc", "TiempoTotal", "TiempoTotalOrig", "Cantidad", "Promedio" });
                ldt.Columns["Cencosdesc"].ColumnName = "Centro de Costos";
                ldt.Columns["TiempoTotal"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                ldt.Columns["Promedio"].ColumnName = "Tiempo Promedio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "TiempoTotalOrig Desc");
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepTabSeeYouOnCencosHoras1pnl", ldt, true, "Totales",
                                      new string[] { "", "", "", "{0:0,0}", "" }, "",
                                       new string[] { }, 0, new int[] { 2 }, new int[] { 0, 1, 3, 4 }, new int[] { }),
                                      "RepTabSeeYouOnCencosHoras1pnl", tituloGrid, Request.Path + "?Nav=RepTabSeeYouOnCencosHoras2Pnl", pestaniaActiva, FCGpoGraf.TabularLiBaCoTa)
                      );

            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt1 = new DataView(ldt);
                ldt = dvldt1.ToTable(false, new string[] { "Centro de Costos", "TiempoTotalOrig" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "TiempoTotalOrig Desc");
                ldt.AcceptChanges();

                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["TiempoTotalOrig"].ColumnName = "value";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnCencosHoras1pnl",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "RepTabSeeYouOnCencosHoras1pnl", "", "", "Centro de Costos", "Horas", pestaniaActiva, FCGpoGraf.Tabular, ""), false);
            }



            #endregion Grafica
        }
        public void RepTabSeeYouOnHorasHombreCencos2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                                        Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaCencosHorasHombre());
            #region grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Cencosdesc", "TiempoTotal", "TiempoTotalOrig", "Cantidad", "Promedio" });
                ldt.Columns["Cencosdesc"].ColumnName = "Centro de Costos";
                ldt.Columns["TiempoTotal"].ColumnName = "Utilización";
                ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                ldt.Columns["Promedio"].ColumnName = "Tiempo Promedio";
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "TiempoTotalOrig Desc");
                ldt.AcceptChanges();

                contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepTabSeeYouOnCencosHoras_T", ldt, true, "Totales",
                     new string[] { "", "", "", "{0:0,0}", "" }, "",
                     new string[] { }, 0, new int[] { 2 }, new int[] { 0, 1, 3, 4 }, new int[] { }),
                    "RepTabSeeYouOnCencosHoras_T", tituloGrid));
            }
            #endregion
            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt1 = new DataView(ldt);
                ldt = dvldt1.ToTable(false, new string[] { "Centro de Costos", "TiempoTotalOrig" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "TiempoTotalOrig Desc");
                ldt.AcceptChanges();
                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["TiempoTotalOrig"].ColumnName = "value";

                contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabSeeYouOnCencosHorasGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabSeeYouOnCencosHorasGraf_G",
                    FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                    "RepTabSeeYouOnCencosHorasGraf_G", "", "", "Centro de Costos", "Horas", 2, FCGpoGraf.Tabular, ""), false);

            }
            #endregion
        }
        #endregion
        private void RepTabImportePorEmpleyTipoLlamada(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaLlamPorTipoDestinoN2());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "iCodCatEmple", "nominaEmple", "nombreEmple", "cencosDesc", "Total" });
                ldt.Columns["nominaEmple"].ColumnName = "Nomina";
                ldt.Columns["nombreEmple"].ColumnName = "Colaborador";
                ldt.Columns["cencosDesc"].ColumnName = "Centro de Costos";
                ldt.Columns["Total"].ColumnName = "Importe";

                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe desc");
            }


            contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                        DTIChartsAndControls.GridView("RepTabImportePorEmpleyTipoLlamadaGrid_T", ldt, true, "Totales",
                                        new string[] { "", "", "", "", "{0:c}" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}&EmpleConJer=1",
                                        new string[] { "iCodCatEmple" }, 1, new int[] { 0 },
                                         new int[] { 2, 3, 4 }, new int[] { 1 }),
                                        "RepTabImportePorEmpleyTipoLlamadaGrid_T", tituloGrid)
                        );
        }

        private void RepTeleMarketing(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaRepMarketing());
            if (ldt.Rows.Count > 0 && ldt != null)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Llamadas" });
                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["Llamadas"].ColumnName = "Cantidad de Llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad de Llamadas desc");
            }
            contenedor.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                        DTIChartsAndControls.GridView("RepTabTeleMarketing_T", ldt, false, "",
                                        new string[] { "", "{0:0,0}" }),
                                        "RepTabTeleMarketing_T", tituloGrid)
                        );
        }

        private void RepConsumoBolsasDiebold(Control contenedor, string tituloGrid, int pestaniaActiva, string tituloGrafica)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoBolsa());
            #region Grid            
            contenedor.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRep1Nvl(
                             DTIChartsAndControls.GridView("GrafBolsaDiariaGrid", ldt, false, "",
                             new string[] { "", "", "{0:0,0}", "", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepTabBolsaDiariaN2&FiltroNav={0}",
                             new string[] { "Filtro" }, 0, new int[] { 0 }, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new int[] { 1 }),
                             "RepTabBolsaDiaria_T", tituloGrid, Request.Path + "?Nav=RepBolsaConsumoN1", pestaniaActiva, FCGpoGraf.Matricial)
              );

            #endregion Grid

            #region Grafica
            DataTable ldt1 = DSODataAccess.Execute(ConsultaConsumoBolsaDiaria());
            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt1));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabBolsaDiaria_T",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt1), "RepTabBolsaDiaria_T",
                tituloGrafica, "", "Dia", "Bolsa Consumida", pestaniaActiva, FCGpoGraf.Matricial), false);
            #endregion Grafica
        }
        private void RepConsumoBolsasDiebold2pnl(string linkGrid, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                             Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(ConsultaConsumoBolsa());
            #region Grid
            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GrafBolsaDiariaGrid", ldt, false, "",
                          new string[] { "", "", "{0:0,0}", "", "{0:0,0}", "", "{0:0,0}", "", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepTabBolsaDiariaN2&FiltroNav={0}",
                          new string[] { "Filtro" }, 0, new int[] { 0 }, new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new int[] { 1 }),
                          "RepTabBolsaDiaria2Pnls_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica
            DataTable ldt1 = DSODataAccess.Execute(ConsultaConsumoBolsaDiaria());
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabBolsaDiariaN2Graf_G", tituloGrafica, 1, FCGpoGraf.Matricial));
            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ldt1));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabBolsaDiariaN2Graf_G",
                FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(ldt1), "RepTabBolsaDiariaN2Graf_G",
                tituloGrafica, "", "Dia", "Bolsa Consumida", 2, FCGpoGraf.Matricial), false);

            #endregion Grafica
        }
        private void RepConsumoBolsasDieboldN2(Control contenedor, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(ConsultaObtienFiltroDetalle());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string filtroq = string.IsNullOrEmpty(dr["Filtro"].ToString()) ? " AND Carrier = 371 " : dr["Filtro"].ToString();

                DataTable ldt = DSODataAccess.Execute(ConsultaDetalleConsumo(filtroq));

                if (ldt.Rows.Count > 0 && ldt != null)
                {
                    DataView dvldt = new DataView(ldt);
                    ldt = dvldt.ToTable(false, new string[] { "Centro de costos", "Colaborador", "Extensión", "Numero Marcado", "Fecha", "Hora",
                                                                "Duracion","Costo","Localidad","Sitio","Carrier","Tipo de destino"});
                    ldt.AcceptChanges();
                }

                contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                            DTIChartsAndControls.GridView("RepTabDetalleConsumo_T", ldt, false, "",
                                            new string[] { "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "" }),
                                            "RepTabDetalleConsumo_T", tituloGrid)
                            );
            }
        }
        #region Reporte Qualtia por Centro de costos
        // EV Para este reporte se toman los datos de la Organización como Centro de costos a petición de eflores
        // Esta consideración se hizo desde los SP para no tener que modificar el reporte en caso de cambios
        private void RepMatQualtiaPorCenCosNv1(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatQualtiaPorCenCos());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["Total"].SetOrdinal(5);
                dtReporte.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatQualtiaPorCenCosGridN1_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatQualtiaPorCenCosN2&Cencos={0}",
                                new string[] { "Codigo Centro de Costos" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatQualtiaPorCenCosPnlN1_T", tituloGrid)
                );
        }

        private void RepMatQualtiaPorCenCosNv2(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepMatQualtiaPorEmple());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i < 5) ? string.Empty : "{0:c}"); //La 0,1 y 2 son campos de texto, a partir del 3 don currency

                    if (i > 4)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["NomCompleto"].ColumnName = "Nombre";
                dtReporte.Columns["Total"].SetOrdinal(5);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepMatQualtiaPorCenCosGridN2_T", dtReporte, true, "Totales", formatoCols.ToArray(),
                                Request.Path + "?Nav=RepMatQualtiaPorCenCosN3&Emple={0}",
                                new string[] { "Codigo Emple" }, 4,
                                new int[] { 0, 1, 2, 3 }, boundfieldCols.ToArray(), new int[] { 4 }
                                ),
                                "RepMatQualtiaPorCenCosPnlN2_T", tituloGrid)
                );
        }

        private void RepMatQualtiaPorCenCosNv3(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDetalladoEmple());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i == 9) ? "{0:c}" : string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Costo Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepMatQualtiaPorCenCosGridN3_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepMatQualtiaPorCenCosPnlN3_T", tituloGrid)
                );
        }
        #endregion Reporte Qualtia por Centro de costos

        #region Reporte llamadas perdidas por extension

        private void RepMatPerdidasPnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?nav=RepPerdidasPorExtN2&level=1";


            #region Grid

            DataTable dtReporte = DSODataAccess.Execute(RepMatPerdidasPorExtPorDia());
            List<string> formatoCols = new List<string>();

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
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                      DTIChartsAndControls.GridView("RepTabPerdidasPorExtGrid_T", dtReporte, false,
                      "", formatoCols.ToArray(), 0, true, false),
                      "RepMatPerdidasPnl", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.MatricialTa)
                      );


            #endregion Grid
        }

        private void RepPerdidasPorExtensionNv2(Control contenedor, string tituloGrid)
        {
            LlamPerd.Visible = true;
            DataTable dtReporte = DSODataAccess.Execute(RepMatPerdidasPorExtPorDia());
            List<string> formatoCols = new List<string>();

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
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepTabPerdidasPorExtGridN2_T", dtReporte, false,
                "", formatoCols.ToArray(), 0, true, false),
                                "RepTabPerdidasPorExtPnlN2_T", tituloGrid)
                );
        }

        private void RepPerdidasPorExtensionNv3(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDetalladoPorExt());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i == 10) ? "{0:c}" : string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepMatQualtiaPorCenCosGridN3_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepMatQualtiaPorCenCosPnlN3_T", tituloGrid)
                );
        }
        #endregion Reporte llamadas perdidas por extension
        /*REPORTES POR DESVIOS*/
        private void RepDesviosPorTipoDestino(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosTipoDestino());

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto Total Desc");
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepDesviosTipoDestino", ldt, true, "Totales",
                                      new string[] { "", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "" }, Request.Path + "?Nav=RepDesviosTipoDestinoN2&TDest={0}",
                                      new string[] { "Tipo de Destino" }, 0, new int[] { 7 }, new int[] { 1, 2, 3, 4, 5, 6 }, new int[] { 0 }),
                                      "RepDesviosTipoDestino", tituloGrid, Request.Path + "?Nav=RepDesviosTipoDestinoN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo de Destino", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de Destino"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTipoDestino",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepDesviosTipoDestino", "", "", "Tipo de Destino", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);


            #endregion Grafica
        }
        private void RepDesviosPorTipoDestino2pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosTipoDestino());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto Total Desc");
                ldt.AcceptChanges();
            }
            #region Grid

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosTipoDestino_T", ldt, true, "Totales",
                                new string[] { "", "{0:c}", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "" },
                              Request.Path + "?Nav=RepDesviosTipoDestinoN2&TDest={0}",
                                new string[] { "Tipo de Destino" }, 0, new int[] { 7 }, new int[] { 1, 2, 3, 4, 5, 6 }, new int[] { 0 }),
                                "RepDesviosTipoDestino_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo de Destino", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo de Destino"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosTipoDestinoGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTipoDestinoGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosTipoDestinoGraf_G", "", "", "Tipo de Destino", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepDesviosPorTipoDestinoN22pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid, string linkGraf, string linkGrid)
        {

            DataTable ldt = DSODataAccess.Execute(RepDesviosTipoDestinoN2(linkGraf));

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosTipoDestinoN2_T", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, linkGrid,
                                /*Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&Linea={2}&TDest="+ param["TDest"] + "&NumDesvios=1",*/
                                new string[] { "Emple", "Extensión" }, 1, new int[] { 0, 8 }, new int[] { 2, 3, 4, 5, 6, 7 }, new int[] { 1 }),
                                "RepDesviosTipoDestinoN2_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosTipoDestinoGrafN2_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTipoDestinoGrafN2_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosTipoDestinoGrafN2_G", "", "", "Empleado", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepDesviosPorTipoDestinoN3(Control contenedorGrid, string tituloGrid)
        {
            int numDesv = Convert.ToInt32(param["NumDesvios"]);
            DataTable dt = DSODataAccess.Execute(RepDetalladoDesvios(numDesv));

            if (dt != null && dt.Rows.Count > 0)
            {
                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepDesviosDetalleGrid_T", dt, true, "Totales",
                            new string[] { "", "", "", "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "", "" },
                             "", new string[] { }, 0,
                            new int[] { 17 }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, new int[] { }),
                            "RepDesviosDetalleGrid_T", tituloGrid));
            }
        }
        private void RepDesviosPorExtension(Control contenedor, string tituloGrid, int pestaniaActiva, string link)
        {
            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&Linea=''+CONVERT(VARCHAR,[Numero de Desvio])+''&NumDesvios=2'' '";
            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&Linea=''+CONVERT(VARCHAR,[Numero de Desvio])+''&NumDesvios=2'' '";
            DataTable ldt = DSODataAccess.Execute(RepDesviosTipoDestinoN2(link));
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedor.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("RepDesviosPorExtension", ldt, true, "Totales",
                          new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" },
                          Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&NumDesvios=2",
                          new string[] { "Emple", "Extensión" }, 1, new int[] { 0, 8 }, new int[] { 2, 3, 4, 5, 6, 7 }, new int[] { 1 }),
                          "RepDesviosPorExtension", tituloGrid, Request.Path + "?Nav=RepDesviosPorExtensionN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorExtension",
        FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
        "RepDesviosPorExtension", "", "", "Empleado", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica
        }
        private void RepDesviosPorExtension2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid, string link)
        {

            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&Linea=''+CONVERT(VARCHAR,[Numero de Desvio])+''&NumDesvios=2'' '";
            DataTable ldt = DSODataAccess.Execute(RepDesviosTipoDestinoN2(link));
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosPorExtension2Pnl_T", ldt, true, "Totales",
                                new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" },
                              Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple={0}&Extension={1}&NumDesvios=2",
                                new string[] { "Emple", "Extensión" }, 1, new int[] { 0, 8 }, new int[] { 2, 3, 4, 5, 6, 7 }, new int[] { 1 }),
                                "RepDesviosPorExtension2Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosPorExtension2PnlGrafN2_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorExtension2PnlGrafN2_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosPorExtension2PnlGrafN2_G", "", "", "Empleado", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepDesviosPorCencosto1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosPorCencos());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepDesviosCenCosto", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, Request.Path + "?Nav=RepDesviosCenCostoN2&CenCos={0}",
                                      new string[] { "icodCatalogo" }, 1, new int[] { 0, 7 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }),
                                      "RepDesviosCenCosto", tituloGrid, Request.Path + "?Nav=RepDesviosCenCostoN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosCenCosto",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepDesviosCenCosto", "", "", "Centro de Costos", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);


            #endregion Grafica
        }
        private void RepDesviosPorCencosto2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosPorCencos());
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosCenCosto_T", ldt, true, "Totales",
                                 new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, Request.Path + "?Nav=RepDesviosCenCostoN2&CenCos={0}",
                                 new string[] { "icodCatalogo" }, 1, new int[] { 0, 7 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }),
                                "RepDesviosCenCosto_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosCenCostoGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosCenCostoGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosCenCostoGraf_G", "", "", "Centro de Costos", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepDesviosPorCencostoN2(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosPorCencos());
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosCenCostoN2_T", ldt, true, "Totales",
                                 new string[] { "", "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, Request.Path + "?Nav=RepDesviosTipoDestinoN3&CenCos={0}&Emple={1}&NumDesvios=3",
                                 new string[] { "icodCatalogo", "Emple" }, 1, new int[] { 0, 2 }, new int[] { 3, 4, 5, 6, 7, 8 }, new int[] { 1 }),
                                "RepDesviosCenCostoN2_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosCenCostoGrafN2_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosCenCostoGrafN2_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosCenCostoGrafN2_G", "", "", "Empleado", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepDesviosPorSitios1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosPorSitio());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }
            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepDesviosSitio", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, Request.Path + "?Nav=RepDesviosSitioN2&Sitio={0}",
                                      new string[] { "icodSitio" }, 1, new int[] { 0, 7 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }),
                                      "RepDesviosSitio", tituloGrid, Request.Path + "?Nav=RepDesviosSitioN1", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosSitio",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepDesviosSitio", "", "", "Sitio", "Importe", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);


            #endregion Grafica

        }
        private void RepDesviosPorSitios2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepDesviosPorSitio());
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc");
                ldt.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("RepDesviosSitio2Pnl_T", ldt, true, "Totales",
                                      new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}", "{0:0,0}", "", "" }, Request.Path + "?Nav=RepDesviosSitioN2&Sitio={0}",
                                      new string[] { "icodSitio" }, 1, new int[] { 0, 7 }, new int[] { 2, 3, 4, 5, 6 }, new int[] { 1 }),
                                      "RepDesviosSitio2Pnl_T", tituloGrid)
                      );

            #endregion Grid
            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Gasto de Desvíos", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }

                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosSitioGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosSitioGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepDesviosSitioGraf_G", "", "", "Sitio", "Importe", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }

        #region Reportes desvios

        public void RepDesviosPorHoraDash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?Nav=RepDesviosPorHoraN1&Level=1";
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPorHora());

            #region Grid
            string[] formatoCols = new string[] { "", "", "{0:c}", "{0:N0}", "{0:N0}", "{0:N0}", "" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepDesviosPorHoraDashGrid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosPorHoraN2&Level=1&Hora={0}",
                                new string[] { "Hora" }, 1,
                                new int[] { 0 }, boundfieldCols.ToArray(), new int[] { 1 }
                                ),
                                "RepDesviosPorHoraDashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosPorHoraN2&Level=1&Hora="
                        + dtReporte.Rows[i]["hora"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Horario", "Gasto de desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Horario"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Horario"].ColumnName = "label";
                dtReporte.Columns["Gasto de desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorHoraDashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepDesviosPorHoraDashPnl_T", "", "", "Horario", "Gasto de desvíos", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica

        }

        public void RepDesviosPorHoraN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPorHora());

            #region Grid
            string[] formatoCols = new string[] { "", "", "{0:c}", "{0:N0}", "{0:N0}", "{0:N0}", "" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;


                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosPorHoraN1Grid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosPorHoraN2&Hora={0}",
                                new string[] { "Hora" }, 1,
                                new int[] { 0 }, boundfieldCols.ToArray(), new int[] { 1 }
                                ),
                                "RepDesviosPorHoraN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosPorHoraN2&Hora="
                        + dtReporte.Rows[i]["hora"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Horario", "Gasto de desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Horario"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Horario"].ColumnName = "label";
                dtReporte.Columns["Gasto de desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosPorHoraN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorHoraN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepDesviosPorHoraN1GPnl_T", "", "", "Horario", "Gasto de desvíos", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        public void RepDesviosPorHoraN2(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDetalladoDesvios());
            string[] formatoCols = new string[] { "", "", "", "", "", "", "", "", "", "{0:N0}", "{0:c}", "", "", "", "", "", "", "" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepDesviosPorHoraN2Grid_T", dtReporte, true, "Totales", formatoCols),
                                "RepDesviosPorHoraN2Pnl_T", tituloGrid)
                );
        }

        public void RepDesviosTop10LlamadasDash(Control contenedor, string tituloGrid, int pestaniaActiva, string by)
        {
            string linkURLNavegacion = "#";
            string ejeY = "";
            string prefijo = "";
            if (by == "Gasto")
            {
                linkURLNavegacion = Request.Path + "?Nav=RepDesviosTop10LlamadasGN1&Level=1";
                ejeY = "Gasto de desvíos";
                prefijo = "$";
            }
            else if (by == "Minutos")
            {
                linkURLNavegacion = Request.Path + "?Nav=RepDesviosTop10LlamadasMN1&Level=1";
                ejeY = "Minutos desvíos";
                prefijo = "";
            }
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosTop10Llamadas(by));

            #region Grid
            string[] formatoCols = new string[] { "", "", "", "{0:c}", "{0:N0}", "" };

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepDesviosTop10Llamadas" + by + "DashGrid_T", dtReporte, true, "Totales", formatoCols),
                               "RepDesviosTop10Llamadas" + by + "DashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "#";
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", ejeY, "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns[ejeY].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTop10Llamadas" + by + "DashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepDesviosTop10Llamadas" + by + "DashPnl_T", "", "", "Extensión", ejeY, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, prefijo), false);

            #endregion Grafica

        }

        public void RepDesviosTop10LlamadasN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid, string by)
        {
            string ejeY = "";
            string prefijo = "";
            if (by == "Gasto")
            {
                ejeY = "Gasto de desvíos";
                prefijo = "$";
            }
            else if (by == "Minutos")
            {
                ejeY = "Minutos desvíos";
                prefijo = "";
            }
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosTop10Llamadas(by));

            #region Grid
            string[] formatoCols = new string[] { "", "", "", "{0:c}", "{0:N0}", "" };

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosTop10LlamadasN1Grid_T", dtReporte, true, "Totales", formatoCols),
                                "RepDesviosTop10LlamadasN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "#";
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", ejeY, "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns[ejeY].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosTop10LlamadasN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTop10LlamadasN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepDesviosTop10LlamadasN1GPnl_T", "", "", "Horario", ejeY, grafActiva, FCGpoGraf.Tabular, prefijo), false);

            #endregion Grafica

        }

        public void RepDesviosTop10ExtDash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?Nav=RepDesviosTop10ExtN1&Level=1";
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosTop10Ext());

            #region Grid
            string[] formatoCols = new string[] { "", "", "{0:c}", "{0:N0}", "{0:N0}" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepDesviosTop10ExtDashGrid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosTop10ExtN2&Level=1&Extension={0}",
                                new string[] { "Extensión" }, 0,
                                new int[] { }, boundfieldCols.ToArray(), new int[] { 0 }
                                ),
                                "RepDesviosTop10ExtDashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosTop10ExtN2&Level=1&Extension="
                        + dtReporte.Rows[i]["Extensión"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Llamadas desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTop10ExtDashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepDesviosTop10ExtDashPnl_T", "", "", "Extensión", "Llamadas desvíos", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica

        }

        public void RepDesviosTop10ExtN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosTop10Ext());

            #region Grid
            string[] formatoCols = new string[] { "", "", "{0:c}", "{0:N0}", "{0:N0}" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;


                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosTop10ExtN1Grid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosTop10ExtN2&Extension={0}",
                                new string[] { "Extensión" }, 0,
                                new int[] { }, boundfieldCols.ToArray(), new int[] { 0 }
                                ),
                                "RepDesviosTop10ExtN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosTop10ExtN2&Extension="
                        + dtReporte.Rows[i]["Extensión"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Llamadas desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosTop10ExtN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosTop10ExtN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepDesviosTop10ExtN1GPnl_T", "", "", "Extensión", "Llamadas desvíos", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        public void RepDesviosTop10ExtN2(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDetalladoDesvios());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i == 10) ? "{0:c}" : string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepDesviosTop10ExtN2Grid_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepDesviosTop10ExtN2Pnl_T", tituloGrid)
                );
        }

        public void RepDesviosPerdidosPorEmpleDash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?Nav=RepDesviosPerdidosPorEmpleN1&Level=1";
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPerdidosPorEmple());

            #region Grid
            string[] formatoCols = new string[] { "", "{0:N0}", "{0:N0}", "{0:N0}" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    if (i > 0)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepDesviosPerdidosPorEmpleDashGrid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosPerdidosPorEmpleN2&Level=1&Extension={0}",
                                new string[] { "Extensión" }, 0,
                                new int[] { }, boundfieldCols.ToArray(), new int[] { 0 }
                                ),
                                "RepDesviosPerdidosPorEmpleDashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosPerdidosPorEmpleN2&Level=1&Extension="
                        + dtReporte.Rows[i]["Extensión"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas no contestadas", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Llamadas no contestadas"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPerdidosPorEmpleDashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepDesviosPerdidosPorEmpleDashPnl_T", "", "", "Extensión", "Llamadas no contestadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica

        }

        public void RepDesviosPerdidosPorEmpleN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPerdidosPorEmple());

            #region Grid
            string[] formatoCols = new string[] { "", "{0:N0}", "{0:N0}", "{0:N0}" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    if (i > 0)
                    {
                        boundfieldCols.Add(i);
                    }
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosPerdidosPorEmpleN1Grid_T", dtReporte, true, "Totales",
                                formatoCols,
                                Request.Path + "?Nav=RepDesviosPerdidosPorEmpleN2&Extension={0}",
                                new string[] { "Extensión" }, 0,
                                new int[] { }, boundfieldCols.ToArray(), new int[] { 0 }
                                ),
                                "RepDesviosPerdidosPorEmpleN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepDesviosPerdidosPorEmpleN2&Extension="
                        + dtReporte.Rows[i]["Extensión"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas no contestadas", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Llamadas no contestadas"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosPerdidosPorEmpleN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPerdidosPorEmpleN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepDesviosPerdidosPorEmpleN1GPnl_T", "", "", "Extensión", "Llamadas no contestadas", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }

        public void RepDesviosPerdidosPorEmpleN2(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDetalladoDesvios());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i == 10) ? "{0:c}" : string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepDesviosPerdidosPorEmpleN2Grid_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepDesviosPerdidosPorEmpleN2Pnl_T", tituloGrid)
                );
        }

        public void RepDesviosPorCarrierDash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?Nav=RepDesviosPorCarrierN1&Level=1";
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPorCarrier());

            #region Grid
            string[] formatoCols = new string[] { "", "{0:c}", "{0:c}", "{0:N0}", "{0:N0}", "{0:N0}", "" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    boundfieldCols.Add(i);
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                               DTIChartsAndControls.GridView("RepDesviosPorCarrierDashGrid_T", dtReporte, true, "Totales", formatoCols),
                               "RepDesviosPorCarrierDashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "#";
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Carrier", "Gasto de desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Carrier"].ColumnName = "label";
                dtReporte.Columns["Gasto de desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorCarrierDashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepDesviosPorCarrierDashPnl_T", "", "", "Carrier", "Gasto de desvíos", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa), false);

            #endregion Grafica

        }

        public void RepDesviosPorCarrierN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepDesviosPorCarrier());

            #region Grid
            string[] formatoCols = new string[] { "", "{0:c}", "{0:c}", "{0:N0}", "{0:N0}", "{0:N0}", "" };
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;

                    boundfieldCols.Add(i);
                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepDesviosPorCarrierN1Grid_T", dtReporte, true, "Totales", formatoCols),
                                "RepDesviosPorCarrierN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "#";
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Carrier", "Gasto de desvíos", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Carrier"].ColumnName = "label";
                dtReporte.Columns["Gasto de desvíos"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepDesviosPorCarrierN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepDesviosPorCarrierN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepDesviosPorCarrierN1GPnl_T", "", "", "Carrier", "Gasto de desvíos", grafActiva, FCGpoGraf.Tabular), false);

            #endregion Grafica

        }

        public void RepExtensionessinActVSBD1pnl(Control contenedor, string tituloGrid)
        {

            DataTable dt = DSODataAccess.Execute(RepExtensionessinActVSBD());

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepExtensionessinActGrid_T", dt, false, "", new string[] { "", "", "" }),
                                "RepExtensionessinActGrid_T", tituloGrid)
                );

        }
        public void RepTraficoExtensiones1pnl(Control contenedor, string tituloGrid)
        {

            DataTable ldt = DSODataAccess.Execute(RepTraficoExtensiones());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "CANTIDAD LLAMADAS Desc");
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepTraficoExtensiones1pnl_T", ldt, true, "Totales", new string[] { "", "", "", "{0:0,0}", "{0:0,0}" }),
                                "RepTraficoExtensiones1pnl_T", tituloGrid)
                );

        }
        #endregion Reportes desvios

        private void ConsEmpmasLlam1Dash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?nav=ConsEmpmasLlam&level=1";
            DataTable dtReporte = DSODataAccess.Execute(ConsEmpmasLlam());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);


                dtReporte = dvldt.ToTable(false, new string[] { "Nombre Completo", "Codigo Empleado", "Numero", "Duracion" });


                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                dtReporte.Columns["Numero"].ColumnName = "Llamadas";


                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Llamadas Desc");
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                      DTIChartsAndControls.GridView("ConsEmpmasLlamGrid_T", dtReporte, true, "Totales",
                                new string[] { "", "", "", "" },
                                Request.Path + "?Nav=ConsEmpmasLlamN2&Emple={0}",
                                new string[] { "Codigo Empleado" }, 0,
                                new int[] { 1 }, new int[] { 2, 3 }, new int[] { 0 }),
                      "RepMatPerdidasPnl", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.MatricialTa)
                      );
        }

        #region Reporte por dispositivo IKUSI
        public void RepLlamadasPorDispositivoDash(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            string linkURLNavegacion = Request.Path + "?Nav=RepLlamadasPorDispositivoN1&Level=1";
            DataTable dtReporte = DSODataAccess.Execute(RepLlamadasPorDispositivo());

            #region Grid
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
                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }

                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["TipoDispositivo"].ColumnName = "Tipo dispositivo";
                dtReporte.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                dtReporte.Columns["CantidadMinutos"].ColumnName = "Cantidad minutos";
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("RepLlamadasPorDispositivoDashGrid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepLlamadasPorDispositivoN2&Level=1&Dispositivo={0}&TipoDisp={1}",
                                new string[] { "iCodCatTipoDispositivo", "Tipo dispositivo" }, 1,
                                new int[] { 0 }, boundfieldCols.ToArray(), new int[] { 1 }
                                ),
                                "RepLlamadasPorDispositivoDashPnl_T", tituloGrid, linkURLNavegacion, pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );
            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepLlamadasPorDispositivoN2&Level=1&Dispositivo="
                        + dtReporte.Rows[i]["iCodCatTipoDispositivo"]
                        + "&TipoDisp="
                        + dtReporte.Rows[i]["Tipo dispositivo"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Tipo dispositivo", "Cantidad llamadas", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Tipo dispositivo"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Tipo dispositivo"].ColumnName = "label";
                dtReporte.Columns["Cantidad llamadas"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepLlamadasPorDispositivoDashPnl_T",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte),
            "RepLlamadasPorDispositivoDashPnl_T", "", "", "Tipo dispositivo", "Llamadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica

        }
        public void RepLlamadasPorDispositivoN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepLlamadasPorDispositivo());

            #region Grid
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
                    if (i > 1)
                    {
                        boundfieldCols.Add(i);
                    }

                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["TipoDispositivo"].ColumnName = "Tipo dispositivo";
                dtReporte.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                dtReporte.Columns["CantidadMinutos"].ColumnName = "Cantidad minutos";
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamadasPorDispositivoDashGrid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepLlamadasPorDispositivoN2&Level=1&Dispositivo={0}&TipoDisp={1}",
                                new string[] { "iCodCatTipoDispositivo", "Tipo dispositivo" }, 1,
                                new int[] { 0 }, boundfieldCols.ToArray(), new int[] { 1 }
                                ),
                                "RepLlamadasPorDispositivoN1Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepLlamadasPorDispositivoN2&Level=1&Dispositivo="
                        + dtReporte.Rows[i]["iCodCatTipoDispositivo"]
                        + "&TipoDisp="
                        + dtReporte.Rows[1]["Tipo dispositivo"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Tipo dispositivo", "Cantidad llamadas", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Tipo dispositivo"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Tipo dispositivo"].ColumnName = "label";
                dtReporte.Columns["Cantidad llamadas"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepLlamadasPorDispositivoN1GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepLlamadasPorDispositivoN1GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepLlamadasPorDispositivoN1GPnl_T", "", "", "Tipo dispositivo", "Llamadas", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica

        }
        public void RepLlamadasPorDispositivoN2(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepLlamadasPorDispositivoExt());

            #region Grid
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
                    if (i > 0)
                    {
                        boundfieldCols.Add(i);
                    }

                    i++;
                }


                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                dtReporte.Columns["CantidadMinutos"].ColumnName = "Cantidad minutos";
                dtReporte.AcceptChanges();
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepLlamadasPorDispositivoN2Grid_T", dtReporte, true, "Totales",
                                formatoCols.ToArray(),
                                Request.Path + "?Nav=RepLlamadasPorDispositivoN3&Extension={0}&Dispositivo=" + param["TipoDisp"],
                                new string[] { "Extensión" }, 0,
                                new int[] { }, boundfieldCols.ToArray(), new int[] { 0 }
                                ),
                                "RepLlamadasPorDispositivoN2Pnl_T", tituloGrid)
                );

            #endregion Grid

            #region Grafica

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                dtReporte.Columns.Add("link");
                for (int i = 0; i < dtReporte.Rows.Count; i++)
                {
                    dtReporte.Rows[i]["link"] = "?Nav=RepLlamadasPorDispositivoN3&Extension="
                        + dtReporte.Rows[i]["Extensión"]
                        + "&Dispositivo = "
                        + param["TipoDisp"];
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Cantidad llamadas", "link" });
                if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                }
                dtReporte.Columns["Extensión"].ColumnName = "label";
                dtReporte.Columns["Cantidad llamadas"].ColumnName = "value";

                dtReporte.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepLlamadasPorDispositivoN2GPnl_T", tituloGrafica, grafActiva, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepLlamadasPorDispositivoN2GPnl_T",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtReporte), "RepLlamadasPorDispositivoN2GPnl_T", "", "", "Extensión", "Llamadas", grafActiva, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        public void RepLlamadasPorDispositivoN3(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepLlamadasPorDispositivoDet());
            List<string> formatoCols = new List<string>();
            List<int> boundfieldCols = new List<int>();

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                var nombresCols = new string[dtReporte.Columns.Count];

                int i = 0;
                foreach (DataColumn dc in dtReporte.Columns)
                {
                    nombresCols[i] = dc.ColumnName;
                    formatoCols.Add((i == 10) ? "{0:c}" : string.Empty);
                    i++;
                }

                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, nombresCols);
                dtReporte.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(DTIChartsAndControls.GridView("RepLlamadasPorDispositivoN3Grid_T", dtReporte, true, "Totales", formatoCols.ToArray()),
                                "RepLlamadasPorDispositivoN3Pnl_T", tituloGrid)
                );
        }
        #endregion Reporte por dispositivo IKUSI
        #region ReportesLlamadasPerdidas
        private void RepTabPorSitioPerdidas1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {

            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasSitio("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio=''+ convert(varchar,[Codigo Sitio])", omitirInfoCDR, omitirInfoSiana));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Numero", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorSitioPerdidasTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={0}",
                                      new string[] { "Codigo Sitio" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "RepTabPorSitioPerdidas1Pnl", tituloGrid, Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorSitioPerdidasTab2",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepTabPorSitioPerdidas1Pnl", "", "", "Sitio", "Cantidad llamadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica
        }
        private void RepTabPorSitioPerdidas2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
            DSODataAccess.Execute(
               RepLlamadasPerdidasSitio("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN2&Sitio=''+ convert(varchar,[Codigo Sitio])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Sitio", "Nombre Sitio", "Numero", "link" });
                ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("LlamadasPerdidasSitioN2Pnl_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN2&Sitio={0}",
                          new string[] { "Codigo Sitio" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                          "LlamadasPerdidasSitioN2Pnl_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("LlamadasPerdidasSitioN2Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LlamadasPerdidasSitioN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "LlamadasPerdidasSitioN2Graf_G", "", "", "Sitio", "Cantidad llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepTabPorTdestPerdidas1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            //Se obtiene el DataSource del reporte
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasTdest("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest='' + convert(varchar,[Codigo Tipo Destino])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Numero", "link" });
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorTdestPerdidasTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&TDest={0}",
                                      new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "RepTabPorTdestPerdidas1Pnl", tituloGrid, Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo Destino"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorTdestPerdidasTab2",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepTabPorTdestPerdidas1Pnl", "", "", "Tipo Destino", "Cantidad llamadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica
        }
        private void RepTabPorTdestPerdidas2Pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
            DSODataAccess.Execute(
                RepLlamadasPerdidasTdest("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN2&TDest='' + convert(varchar,[Codigo Tipo Destino])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Numero", "link" });
                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("LlamadasPerdidasTDestN12Pnl_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN2&TDest={0}",
                          new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                          "LlamadasPerdidasTDestN12Pnl_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Tipo Destino"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("LlamadasPerdidasTDestN1Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LlamadasPerdidasTDestN1Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "LlamadasPerdidasTDestN1Graf_G", "", "", "Tipo Destino", "Cantidad llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepTabPorCenCosPerdidas1pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasCencos(" '''" + Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN2&CenCos='''"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo CenCos", "Cencos", "TotalLlamadas", "link" });
                ldt.Columns["Cencos"].ColumnName = "Centro de Costos";
                ldt.Columns["TotalLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorCenCosPerdidasTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN2&CenCos={0}",
                                      new string[] { "Codigo CenCos" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "RepTabPorCenCosPerdidas1Pnl", tituloGrid, Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorCenCosPerdidasTab2",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepTabPorCenCosPerdidas1Pnl", "", "", "Centro de Costos", "Cantidad llamadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica
        }
        private void RepTabPorCenCosPerdidas2pnlN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasCencos(" '''" + Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN2&CenCos='''"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo CenCos", "Cencos", "TotalLlamadas", "link" });
                ldt.Columns["Cencos"].ColumnName = "Centro de Costos";
                ldt.Columns["TotalLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("LlamadasPerdidasCencosN12Pnl_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN2&CenCos={0}",
                          new string[] { "Codigo CenCos" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                          "LlamadasPerdidasCencosN12Pnl_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Centro de Costos"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("LlamadasPerdidasCencostoN1Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LlamadasPerdidasCencostoN1Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "LlamadasPerdidasCencostoN1Graf_G", "", "", "Centro de Costos", "Cantidad llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepTabPorCenCosPerdidas2pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
            DSODataAccess.Execute(
            RepLlamadasPerdidasCencosN2("'''" + Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN3&Emple='''"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Emple", "Nombre", "TotalLlamadas", "link" });
                ldt.Columns["Nombre"].ColumnName = "Empleado";
                ldt.Columns["TotalLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("LlamadasPerdidasCencosN2Pnl_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN3&Emple={0}",
                          new string[] { "Codigo Emple" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                          "LlamadasPerdidasCencosN2Pnl_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("LlamadasPerdidasCencosN2Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LlamadasPerdidasCencosN2Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "LlamadasPerdidasCencosN2Graf_G", "", "", "Empleado", "Cantidad llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        private void RepTabTop10EmplePerdidas(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasTopEmple("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Emple='' + convert(varchar,[Codigo Empleado])"));

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Empleado", "Nombre Completo", "Numero", "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GrafConsPorEmplePerdidasTab1", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Emple={0}",
                                      new string[] { "Codigo Empleado" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                                      "RepTabPorEmplePerdidas1Pnl", tituloGrid, Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), pestaniaActiva, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsPorEmplePerdidasTab2",
              FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
              "RepTabPorEmplePerdidas1Pnl", "", "", "Empleado", "Cantidad llamadas", pestaniaActiva, FCGpoGraf.TabularBaCoDoTa, ""), false);

            #endregion Grafica
        }
        private void RepTabTop10EmplePerdidas2pnl(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
                DSODataAccess.Execute(
                RepLlamadasPerdidasTopEmple("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN2&Emple='' + convert(varchar,[Codigo Empleado])"));
            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Empleado", "Nombre Completo", "Numero", "link" });
                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("RepTabPorEmplePerdidas2Pnl_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "" }, Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN2&Emple={0}",
                          new string[] { "Codigo Empleado" }, 1, new int[] { 0, 3 }, new int[] { 2 }, new int[] { 1 }),
                          "RepTabPorEmplePerdidas2Pnl_T", tituloGrid)
          );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Cantidad llamadas", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("RepTabPorEmplePerdidas2PnlGraf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTabPorEmplePerdidas2PnlGraf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorEmplePerdidas2PnlGraf_G", "", "", "Empleado", "Cantidad llamadas", 2, FCGpoGraf.Tabular, ""), false);

            #endregion Grafica
        }
        #endregion RepportesLlamadasPerdidas
        private void ReptabLlamadasAgrupEmpleN1(Control contenedorGrafica, int grafActiva, string tituloGrafica, Control contenedorGrid, string tituloGrid)
        {
            DataTable ldt =
                DSODataAccess.Execute(
               RepLlamadasAgrupEmpleN1("[link]=''" + Request.Path + "?Nav=RepLlamadasAgrupadasEmpleN2&Emple=''+convert(varchar,[Codigo Empleado]),"));
            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Codigo Empleado", "Nombre Completo", "Numero", "link", "Duracion", "Total" });
                ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                ldt.AcceptChanges();
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
            }

            contenedorGrid.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("ReptabLlamadasAgrupEmpleN1_T", ldt, true, "Totales",
                          new string[] { "", "", "{0:0,0}", "", "{0:0,0}", "{0:c}" }, Request.Path + "?Nav=RepLlamadasAgrupadasEmpleN2&Emple={0}",
                          new string[] { "Codigo Empleado" }, 1, new int[] { 0, 3 }, new int[] { 2, 4, 5 }, new int[] { 1 }),
                          "ReptabLlamadasAgrupEmpleN1_T", tituloGrid)
          );
            #endregion Grid

            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Empleado", "Total", "link" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Empleado"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ReptabLlamadasAgrupEmpleN1Graf_G", tituloGrafica, 1, FCGpoGraf.Tabular));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReptabLlamadasAgrupEmpleN1Graf_G",
                FCAndControls.Grafica1Serie(
                FCAndControls.ConvertDataTabletoJSONString(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "ReptabLlamadasAgrupEmpleN1Graf_G", "", "", "Empleado", "Total", 2, FCGpoGraf.Tabular, "$"), false);

            #endregion


        }

        private void ReptabLlamadasAgrupEmpleN2(Control contenedorGrid, string tituloGrid)
        {

            DataTable dt = DSODataAccess.Execute(RepLlamadasAgrupEmpleN2());

            if (dt != null && dt.Rows.Count > 0)
            {


                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepLlamadasAgrupDetalleGrid_T", dt, true, "Totales",
                            new string[] { "", "", "", "", "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "", "" },
                             "", new string[] { }, 0,
                            new int[] { 17 }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, new int[] { 1 }),
                            "RepLlamasAgrupDetalleGrid", tituloGrid));
            }
        }


        #region ConsultaTipoDestino
        private void ReptabConsultaPorTipoDestinoPeEm(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable RepPorTDestPeEm = DSODataAccess.Execute(ConsultaPorTipoDestinoPeEm());

            if (RepPorTDestPeEm.Rows.Count > 0 && RepPorTDestPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepPorTDestPeEm);
                RepPorTDestPeEm = dvGrafConsHist.ToTable(false, new string[] { "Codigo Tipo Destino", "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                RepPorTDestPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo destino";
                RepPorTDestPeEm.Columns["Total"].ColumnName = "Total";
                RepPorTDestPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                RepPorTDestPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
            }


            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepPorTDestPeEmGrid_T", RepPorTDestPeEm, true, "Totales",
                                new string[] { "", "", "{0:c}", "", "" }, Request.Path + "?Nav=TDestPeEmN2&TDest={0}",
                                new string[] { "Codigo Tipo Destino" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                "RepPorTDestPeEmGridRep01_T", tituloGrid, DictTooltips.ReptabConsultaPorTipoDestinoPeEm, Request.Path + "?Nav=TDestPeEmN1")
                );

        }
        #endregion ConsultaTipoDestino

        #region Reporte por tipo de llamada
        private void ReptabconsultaPorTipoLlamPeEmDetalleBanorte(Control contenedor, string tituloGrid, int PestanaActiva)
        {
            if (DSODataContext.Schema != "bimbo")
            {
                DataTable GrafPorTipoLlamPeEm = null;

                #region //20160722 NZ Se agrega esta seccion para cuando entren los esquemas de Evox, Banorte y K5Banorte
                if (DSODataContext.Schema.ToString().ToLower() == "evox"
                                                        || DSODataContext.Schema.ToString().ToLower() == "banorte"
                                                        || DSODataContext.Schema.ToString().ToLower() == "k5banorte")
                {
                    //NZ 20160921
                    GrafPorTipoLlamPeEm = DSODataAccess.Execute(consultaPorTipoLlamPeEmDetalleBanorte(
                    "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));

                    if (GrafPorTipoLlamPeEm.Rows.Count > 0)
                    {
                        string gpoNoIdentificado = DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = '0NoIdent'").ToString();
                        foreach (DataRow row in GrafPorTipoLlamPeEm.Rows)
                        {
                            if (row["Clave Tipo Llamada"].ToString() == gpoNoIdentificado)
                            {
                                row["link"] = Request.ApplicationPath + "/UserInterface/Historicos/Etiquetacion/EtiquetacionEmple.aspx";
                                break;
                            }
                        }
                    }
                } //20160722 NZ 
                else
                {
                    GrafPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaPorTipoLlamPeEm(
                    "[link] = ''" + Request.Path + "?Nav=PorTipoLlamN2&TipoLlam=''+convert(varchar,[Clave Tipo Llamada])"));

                }
                #endregion

                if (GrafPorTipoLlamPeEm.Rows.Count > 0 && GrafPorTipoLlamPeEm.Columns.Count > 0)
                {
                    DataView dvGrafConsHist = new DataView(GrafPorTipoLlamPeEm);
                    GrafPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Tipo Llamada", "Total", "link" });
                    GrafPorTipoLlamPeEm.Columns["Tipo Llamada"].ColumnName = "label";
                    GrafPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";
                    GrafPorTipoLlamPeEm.Columns["link"].ColumnName = "link";
                }

                contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafPorTipoLlamPeEm_G", "Consumo por tipo de llamada", PestanaActiva, FCGpoGraf.Tabular, Request.Path + "?Nav=PorTipoLlamN1"));


                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafPorTipoLlamPeEm_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafPorTipoLlamPeEm),
                    "GrafPorTipoLlamPeEm_G", "Consumo por tipo de llamada", "", "Tipo de llamada", "Total", PestanaActiva, FCGpoGraf.Tabular), false);


            }
        }
        #endregion

        #region llamadas mas caras
        private void ReptabConsultaNumerosMasCarosPeEm(Control contenedor, string tituloGrid, int PestanaActiva)
        {
            //Se obtiene el DataSource del reporte
            DataTable RepNumMasCarosPeEm = DSODataAccess.Execute(ConsultaNumerosMasCarosPeEm());
            int[] colNoVisibles = new int[] { };
            int[] colNav = new int[] { };
            if (RepNumMasCarosPeEm.Rows.Count > 0 && RepNumMasCarosPeEm.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepNumMasCarosPeEm);
                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Etiqueta", "Tipo Llamada", "Total", "Duracion", "Numero" });
                    RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                    colNoVisibles = new int[] { 0 };
                    colNav = new int[] { 2, 3, 4, 5, 6 };

                }
                else
                {
                    RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "CodNumMarcado", "Numero Marcado", "Nombre Localidad",
                                                                                                               "Tipo Llamada", "Total", "Duracion","Numero" });
                    RepNumMasCarosPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                }

                RepNumMasCarosPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";

                if (DSODataContext.Schema.ToLower() != "bimbo")
                {
                    RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                    colNoVisibles = new int[] { 0 };
                    colNav = new int[] { 2, 3, 4, 5, 6 };
                }
                else
                {
                    colNoVisibles = new int[] { 0, 3 };
                    colNav = new int[] { 2, 4, 5, 6 };
                }

                RepNumMasCarosPeEm.Columns["Total"].ColumnName = "Total";
                RepNumMasCarosPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                RepNumMasCarosPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";       //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
            }


            contenedor.Controls.Add(
                     DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                     DTIChartsAndControls.GridView("RepNumMasCarosPeEmGrid_T", RepNumMasCarosPeEm, true, "Totales",
                                     new string[] { "", "", "", "", "{0:c}", "", "" }, Request.Path + "?Nav=NumMasCarosN2&NumMarc={0}",
                                     new string[] { "CodNumMarcado" }, 1, colNoVisibles, colNav, new int[] { 1 }),
                                     "RepNumMasCarosPeEmGridRep03_T", tituloGrid, DictTooltips.ReptabConsultaNumerosMasCarosPeEm, Request.Path + "?Nav=NumMasCarosN1")
                     );
        }
        #endregion
        #region Reporte Historico Bimbo
        private void tabConsultaHistoricoPeEm(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            System.Data.DataTable GrafConsHist = DSODataAccess.Execute(ConsultaHistoricoPeEm());
            if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(GrafConsHist);
                GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                GrafConsHist.Columns["Nombre Mes"].ColumnName = "label";
                GrafConsHist.Columns["Total"].ColumnName = "value";
                GrafConsHist.AcceptChanges();
            }

            contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConsHist_G", tituloGrid, pestaniaActiva, FCGpoGraf.Tabular, DictTooltips.tabConsultaHistoricoPeEm, Request.Path + "?Nav=HistoricoPeEmN1"));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHist_G",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConsHist), "GrafConsHist_G", "Consumo histórico a  12 meses", "", "Mes", "Importe", 0, FCGpoGraf.Tabular), false);
        }

        #endregion Reporte Historico Bimbo

        #region Reporte Extensiones Bimbo
        private void tabConsultaExtensionesEnLasQueSeUsoElCodAuto(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            DataTable RepExtenDondeUtilizoCodAut = DSODataAccess.Execute(ConsultaExtensionesEnLasQueSeUsoElCodAuto());

            if (RepExtenDondeUtilizoCodAut.Rows.Count > 0 && RepExtenDondeUtilizoCodAut.Columns.Count > 0)
            {
                DataView dvGrafConsHist = new DataView(RepExtenDondeUtilizoCodAut);
                RepExtenDondeUtilizoCodAut = dvGrafConsHist.ToTable(false, new string[] { "Codigo Autorizacion", "Extension", "Llamadas" });
                RepExtenDondeUtilizoCodAut.Columns["Codigo Autorizacion"].ColumnName = "Código de autorización";
                RepExtenDondeUtilizoCodAut.Columns["Extension"].ColumnName = "Extensión";
                RepExtenDondeUtilizoCodAut.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("RepExtenDondeUtilizoCodAutGrid_T", RepExtenDondeUtilizoCodAut, true, "Totales",
                                new string[] { "", "", "" }), "RepExtenDondeUtilizoCodAutGridRep05_T", tituloGrid, DictTooltips.tabConsultaExtensionesEnLasQueSeUsoElCodAuto, Request.Path + "?Nav=ExtenUsoCodAutN1")
                );
        }
        #endregion Reporte Extensiones Bimbo

        #region Reporte Consumo por tipo de llamada
        private void tabConsultaConsumoPorTipoLlamadaPeEm(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            if (DSODataContext.Schema.ToLower() == "bimbo")
            {
                System.Data.DataTable GrafConPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaConsumoPorTipoLlamadaPeEm());
                if (GrafConPorTipoLlamPeEm.Rows.Count > 0 && GrafConPorTipoLlamPeEm.Columns.Count > 0)
                {
                    DataView dvGrafConsHist = new DataView(GrafConPorTipoLlamPeEm);
                    GrafConPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "etiqueta", "Total" });
                    GrafConPorTipoLlamPeEm.Columns["etiqueta"].ColumnName = "label";
                    GrafConPorTipoLlamPeEm.Columns["Total"].ColumnName = "value";

                }

                contenedor.Controls.Add(DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafConPorTipoLlamPeEm_G", tituloGrid, pestaniaActiva, FCGpoGraf.Tabular, Request.Path + "?Nav=ExtenUsoCodAutN1"));


                Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConPorTipoLlamPeEm_G",
                    FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(GrafConPorTipoLlamPeEm),
                    "GrafConPorTipoLlamPeEm_G", "Consumo por tipo de llamada", "", "Tipo de llamada", "Total", 0, FCGpoGraf.Tabular), false);
            }
        }
        #endregion Reporte Consumo por tipo de llamada

        private void RepTabEmpleporCencos(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepConsumoColabDirectos());

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Emple", "NomCompleto", "CenCosDesc", "Costo", "Llams", "Dur" });
                ldt.Columns["NomCompleto"].ColumnName = "Colaborador";
                ldt.Columns["CenCosDesc"].ColumnName = "Centro Costos";
                ldt.Columns["Costo"].ColumnName = "Importe";
                ldt.Columns["Llams"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Dur"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }
            contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                    "RepTabEmplePorCencosGrid", ldt, true, "Totales",
                                    new string[] { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}&EmpleConJer=1",
                                    new string[] { "Emple" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabReporteEmplePorCencosGrid_T", tituloGrid, DictTooltips.RepTabEmpleporCencos));

            #endregion
        }
        private void RepTabConsumoporDeptosN1(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepConsumoporDeptoN1());

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Cencos", "CencosDesc", "Costo", "Llams", "Dur" });
                ldt.Columns["CencosDesc"].ColumnName = "Centro Costos";
                ldt.Columns["Costo"].ColumnName = "Importe";
                ldt.Columns["Llams"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Dur"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }
            contenedor.Controls.Add(
                            DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView(
                                    "RepTabEmplePorCencosGrid", ldt, true, "Totales",
                                    new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=RepConsumoporDeptosN2&CenCos={0}",
                                    new string[] { "Cencos" }, 1, new int[] { 0 },
                                    new int[] { 2, 3, 4, 5 }, new int[] { 1 }), "RepTabReportePorCencosGrid_T", tituloGrid, DictTooltips.RepTabConsumoporDeptosN1));

            #endregion

        }
        private void RepTabConsumoporDeptosN2(Control contenedor, string tituloGrid)
        {
            DataTable ldt = DSODataAccess.Execute(RepConsumoporDeptoN2());

            #region Grid
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvRepConsCenCos = new DataView(ldt);
                ldt = dvRepConsCenCos.ToTable(false,
                    new string[] { "Emple", "CencosDesc", "Colaborador", "Costo", "Llams", "Dur" });
                ldt.Columns["CencosDesc"].ColumnName = "Centro de Costos";
                ldt.Columns["Costo"].ColumnName = "Importe";
                ldt.Columns["Llams"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                ldt.Columns["Dur"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                ldt.AcceptChanges();
            }

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepLlamadasAgrupDetalleGrid_T", ldt, true, "Totales",
                            new string[] { "", "", "", "{0:c}", "{0:0}", "{0:0,0}" }, Request.Path + "?Nav=EmpleMCN2&Emple={0}&EmpleConJer=1",
                            new string[] { "Emple" }, 1,
                            new int[] { 0 }, new int[] { 1, 3, 4, 5 }, new int[] { 2 }),
                            "RepLlamasAgrupDetalleGrid", tituloGrid));


            #endregion

        }
        private void RepConsLlamsMasCaras1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsLlamsMasCaras());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(
                    false,
                    new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Sitio", "Nombre Localidad" });

                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Nombre Sitio"].ColumnName = "Sitio";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.AcceptChanges();
            }
            #region Grid

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView("RepConsLlamsMasCarasGrid", dtReporte, true, "Totales",
                        new string[] { "", "", "", "", "", "", "{0:c}", "", "" }, "",
                        new string[] { }, 0, new int[] { }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, new int[] { }),
                    "RepConsLlamsMasCaras1Pnl_T", tituloGrid, DictTooltips.RepConsLlamsMasCaras1Pnl, Request.Path + "?Nav=ConsLlamsMasCaras")
                );
            #endregion
        }
        private void RepConsNumerosMasMarcadas1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(ConsNumerosMasMarcadas());

            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(
                    false,
                    new string[] { "Numero Marcado", "Llamadas", "Nombre Localidad", "Codigo Localidad", "Total", "Cant Emp", "Duracion" });
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                dtReporte.Columns["Cant Emp"].ColumnName = "Número  de Empleados";
                dtReporte.Columns["Duracion"].ColumnName = "Minutos";

                dtReporte.AcceptChanges();
            }

            #region Grid

            contenedor.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                    DTIChartsAndControls.GridView(
                        "ConsLlamsMasCarasGrid", dtReporte, true, "Totales",
                        new string[] { "", "", "", "", "{0:c}", "", "", "", "" }, Request.Path + "?Nav=ConsNumerosMasMarcadasN2&NumMarc={0}&locali={1}",
                        new string[] { "Número Marcado", "Codigo Localidad" }, 0,
                        new int[] { 3 }, new int[] { 1, 2, 4, 5, 6 }, new int[] { 0 }),
                        "ConsNumerosMasMarcadas1Pnl_T", tituloGrid, DictTooltips.RepConsNumerosMasMarcadas1Pnl, Request.Path + "?Nav=ConsNumerosMasMarcadas")
                );
            #endregion
        }
        private void RepConsMenuConsLLamadasMasTiempo1Pnl(Control contenedor, string tituloGrid)
        {
            DataTable dtReporte = DSODataAccess.Execute(RepTabMenuConsLLamadasMasTiempo());
            string[] cols = new string[] { };
            if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
            {
                if (DSODataContext.Schema.ToUpper() != "BIMBO")
                {
                    cols = new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Duracion Minutos", "Costo", "Nombre Localidad", "Tipo Llamada" };
                    dtReporte.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                }
                else
                {
                    cols = new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Duracion Minutos", "Costo", "Nombre Localidad" };
                }
                DataView dvldt = new DataView(dtReporte);
                dtReporte = dvldt.ToTable(false, cols);
                dtReporte.Columns["Extension"].ColumnName = "Extensión";
                dtReporte.Columns["Nombre Completo"].ColumnName = "Empleado";
                dtReporte.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                dtReporte.Columns["Duracion Minutos"].ColumnName = "Minutos";
                dtReporte.Columns["Costo"].ColumnName = "Total";
                dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";


                dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Minutos Desc");
                dtReporte.AcceptChanges();
            }


            contenedor.Controls.Add(
          DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("RepTabMenuConsLLamadasMasTiempoGrid_T", dtReporte, true, "Total",
                          new string[] { "", "", "", "", "{0:0,0}", "{0:c}", "", "" }),
                          "RepTabMenuConsLLamadasMasTiempo1Pnl_T", tituloGrid, DictTooltips.RepConsMenuConsLLamadasMasTiempo1Pnl, Request.Path + "?Nav=RepTabMenuConsLLamadasMasTiempo")
          );
        }


        //2022-01-04
        private void RepTabPorExtensionPI1Pnl(Control contenedor, string tituloGrid, int pestaniaActiva, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            campoOrdenamiento = "Cantidad llamadas desc";
            numberPrefix = " ";

            DataTable ldt = DSODataAccess.Execute(RepTraficoPorExtensionPI());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, 
                    new string[] { "Extension", "iCodCatCenCos", "CentroCostos", "iCodCatSitio", "Sitio", "CantidadLlamadas", "Minutos" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["CentroCostos"].ColumnName = "Centro de costos";
                ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, campoOrdenamiento);
            }



            contenedor.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                      DTIChartsAndControls.GridView("GridTraficoPorExtPITab1", ldt, true, "Totales",
                                      new string[] { "", "", "", "", "", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=DetallePorExtPI&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Extension={0}&CenCos={1}&Sitio={2}",
                                      new string[] { "Extensión", "iCodCatCenCos", "iCodCatSitio" }, 0, new int[] { 1, 3 }, new int[] { 2, 4, 5, 6 }, new int[] { 0 }),
                                      "RepTabPorExtensionPI1Pnl", tituloGrid, Request.Path + "?Nav=PorExtPIN1&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString(), 3, FCGpoGraf.TabularBaCoDoTa)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Cantidad llamadas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTraficoPorExtPITab2",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabPorExtensionPI1Pnl", "", "", "Extensión", "Cantidad llamadas", 3, FCGpoGraf.TabularBaCoDoTa, numberPrefix), false);
            

            #endregion Grafica
        }

        private void RepTabPorExtensionPI2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(RepTraficoPorExtensionPI());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Extension", "iCodCatCenCos", "CentroCostos", "iCodCatSitio", "Sitio", "CantidadLlamadas", "Minutos" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["CentroCostos"].ColumnName = "Centro de costos";
                ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas desc");
            }

            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&NumDesvios=2'' '";

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GridTraficoPorExtPITab1", ldt, true, "Totales",
                                      new string[] { "", "", "", "", "", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=DetallePorExtPI&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Extension={0}&CenCos={1}&Sitio={2}",
                                      new string[] { "Extensión", "iCodCatCenCos", "iCodCatSitio" }, 0, new int[] { 1, 3 }, new int[] { 2, 4, 5, 6 }, new int[] { 0 }),
                                      "RepTabPorExtensionPI1Pnl", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Cantidad llamadas" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTraficoPorExtPI_G", tituloGrafica, 1, FCGpoGraf.Tabular, " "));



            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTraficoPorExtPI_G",
                FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(
                        DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafTraficoPorExtPI_G", "", "", "Extensión", "Cantidad llamadas", 1, FCGpoGraf.Tabular, " "), false);



            #endregion Grafica
        }


        private void RepDetalleDesdePorExtension(Control contenedorGrid, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(RepDetalladoDesdePorExtension());

            if (dt != null && dt.Rows.Count > 0)
            {
                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepDetalleDesdePorExtGrid_T", dt, true, "Totales",
                            new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "", "", ""  },
                             "", new string[] { }, 0,
                            new int[] { 17 }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, new int[] { }),
                            "RepDetalleDesdePorExtGrid_T", tituloGrid));
            }
        }


        private void RepDetalleHospitales(Control contenedorGrid, string tituloGrid)
        {
            DataTable dt = DSODataAccess.Execute(RepDetalladoHospitales());

            if (dt != null && dt.Rows.Count > 0)
            {
                contenedorGrid.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                            DTIChartsAndControls.GridView("RepDetalleDesdePorExtGrid_T", dt, true, "Totales",
                            new string[] { "", "", "", "", "", "{0:0,0}", "{0:c}", "", "", "", "", "", "", "", "" },
                             "", new string[] { }, 0,
                            new int[] { 17 }, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, new int[] { }),
                            "RepDetalleDesdePorExtGrid_T", tituloGrid));
            }
        }


        private void RepTabPorExtension2Pnls(Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ObtieneTraficoLlamadasPorExtension());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Extension", "Sitio", "Total", "CantidadLlamadas", "Minutos" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total desc, Cantidad llamadas desc, Cantidad minutos desc");
            }

            contenedorGrid.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                DTIChartsAndControls.GridView("GridTraficoPorExtTab1", ldt, true, "Totales", new string[] { "", "", "{0:c}", "{0:0,0}", "{0:0,0}" }),
                                "GridTraficoPorExtTab1", tituloGrid));

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTraficoPorExt_G", tituloGrafica, 1, FCGpoGraf.Tabular, " "));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTraficoPorExt_G",
                FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(
                        DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafTraficoPorExt_G", "", "", "Extensión", "Total", 1, FCGpoGraf.Tabular, " "), false);


            #endregion Grafica
        }


        private void RepTabTraficoContestadasYNoPorSitio2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(RepTabCantLlamsContestadasYNoPorSitio());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "iCodCatSitio", "Sitio", "LlamadasNoContestadas", "LlamadasContestadas", "Total" });
                ldt.Columns["LlamadasContestadas"].ColumnName = "Llamadas contestadas";
                ldt.Columns["LlamadasNoContestadas"].ColumnName = "Llamadas no contestadas";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total desc");
            }

            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&NumDesvios=2'' '";

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GridTraficoContestadasYNo", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=RepTabContestadasYNoUnSitioPorExten&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={0}",
                                      new string[] { "iCodCatSitio" }, 1, new int[] { 0 }, new int[] { 2, 3, 4 }, new int[] { 1 }),
                                      "RepTabPorExtensionPI1Pnl", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Sitio"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTraficoPorSitio_G", tituloGrafica, 1, FCGpoGraf.Tabular, " "));



            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTraficoPorSitio_G",
                FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(
                        DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafTraficoPorSitio_G", "", "", "Sitio", "Total", 1, FCGpoGraf.Tabular, " "), false);



            #endregion Grafica
        }


        private void RepTabTraficoContestadasYNoUnSitioPorExt2Pnls(string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,
                                                              Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0)
        {
            DataTable ldt = DSODataAccess.Execute(ObtieneCantidadLlamadasContestadasyNoUnSitioPorExt());

            #region Grid

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Extension", "iCodCatSitio", "LlamadasNoContestadas", "LlamadasContestadas", "Total" });
                ldt.Columns["Extension"].ColumnName = "Extensión";
                ldt.Columns["LlamadasContestadas"].ColumnName = "Llamadas contestadas";
                ldt.Columns["LlamadasNoContestadas"].ColumnName = "Llamadas no contestadas";
                ldt.AcceptChanges();

                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total desc");
            }

            //string link = " '''" + Request.Path + "?Nav=RepDesviosTipoDestinoN3&Emple=''+CONVERT(VARCHAR,A.Emple)+''&Extension=''+CONVERT(VARCHAR,Extension) +''&NumDesvios=2'' '";

            contenedorGrid.Controls.Add(
                      DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                                      DTIChartsAndControls.GridView("GridTraficoContestadasYNo", ldt, true, "Totales",
                                      new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }, Request.Path + "?Nav=DetallePorExtPI&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&Sitio={1}&Extension={0}",
                                      new string[] { "Extensión", "iCodCatSitio" }, 0, new int[] { 1 }, new int[] { 2, 3, 4 }, new int[] { 0 }),
                                      "RepTabPorExtensionPI1Pnl", tituloGrid)
                      );

            #endregion Grid

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Extensión", "Total" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Extensión"].ColumnName = "label";
                ldt.Columns["Total"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTraficoUnSitio_G", tituloGrafica, 1, FCGpoGraf.Tabular, " "));



            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTraficoUnSitio_G",
                FCAndControls.Grafica1Serie(
                    FCAndControls.ConvertDataTabletoJSONString(
                        DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)), "GrafTraficoUnSitio_G", "", "", "Extensión", "Total", 1, FCGpoGraf.Tabular, " "), false);



            #endregion Grafica
        }

        //RepTabParticipantesvsReunionesMes(linkGrid, linkGraf, Rep2, 1, TituloNavegacion, Rep1, TituloNavegacion)
        //string linkGrid, string linkGrafica, Control contenedorGrafica, int grafActiva, string tituloGrafica,Control contenedorGrid, string tituloGrid, int omitirInfoCDR = 0, int omitirInfoSiana = 0
        private void RepTabParticipantesvsReunionesMes(Control contenedor,  string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            //fecha NumeroReuniones PromedioParticipante    maximoParticipante
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneConsultaMes());

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fecha", "NumeroReuniones", "PromedioParticipante", "maximoParticipante" });
                ldt.Columns["fecha"].ColumnName = "Fecha";
                ldt.Columns["NumeroReuniones"].ColumnName = "No. Reuniones";
                ldt.Columns["PromedioParticipante"].ColumnName = "Promedio participantes";
                ldt.Columns["maximoParticipante"].ColumnName = "Max participantes";
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridparticipantesMeet", ldt, false, "Totales",
                          new string[] { "", "", "{0:0,0}", "{0:0,0}", "{0:0,0}" }, Request.Path,
                          new string[] { "Fecha" }, 4, new int[] { 1 }, new int[] { 0, 1, 2, 3 }, new int[] {}),
                          "RepTabParticipantesvsReunionesMes", tituloGrid)
                );
            }
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                //dvldt.Sort = "Fecha";
                ldt = dvldt.ToTable(false, new string[] { "Fecha", "No. Reuniones" });
               
                ldt.Columns["Fecha"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }
            contenedorGrafica.Controls.Add(
                DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafparticipantesMeet", tituloGrafica, 1, FCGpoGraf.Tabular, " "));


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafparticipantesMeet",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "", 10)),
                "GrafparticipantesMeet", "", "", "fecha", "No. Reuniones", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }

        private void RepTabParticipantesvsReunionesMes1(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneConsultaMes());

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "fecha", "NumeroReuniones", "PromedioParticipante", "maximoParticipante" });
                ldt.Columns["fecha"].ColumnName = "Fecha";
                ldt.Columns["NumeroReuniones"].ColumnName = "No. Reuniones";
                ldt.Columns["PromedioParticipante"].ColumnName = "Promedio participantes";
                ldt.Columns["maximoParticipante"].ColumnName = "Max participantes";
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("GridparticipantesMeet", ldt, false, "Totales",
                          new string[] { "", "", "{0:0,0}", "{0:0,0}" }, Request.Path,
                          new string[] { "fecha" }, 0, new int[] { 1 }, new int[] {0,1, 2, 3 }, new int[] { 2, 3 }, 0),
                          "RepTabParticipantesvsReunionesMes1", tituloGrid,"", 3, FCGpoGraf.TabularBaCoDoTa)
                );
            }
            #endregion
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "fecha", "No. Reuniones" });
                if (ldt.Rows[ldt.Rows.Count - 1]["fecha"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["fecha"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafparticipantesMeet",
            FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
            "RepTabParticipantesvsReunionesMes1", "", "", "fecha", "No. Reuniones", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
        }

        private void RepTabParticipantesvsHorasMes(Control contenedor, string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneParticipantesHoras());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre",   "NumeroEventos",   "Horasdedicadasareuniones",    "PromedioEvento" });
                ldt.Columns["Nombre"].ColumnName = "Nombre";
                ldt.Columns["NumeroEventos"].ColumnName = "No. Reuniones del mes";
                ldt.Columns["Horasdedicadasareuniones"].ColumnName = "Horas  dedicadas a reuniones(suma)";
                ldt.Columns["PromedioEvento"].ColumnName = "Tiempo promedio / reunión (horas/no. reuniones)";
                ldt.AcceptChanges();


                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridTParticipantesvsHoras", ldt, false, "Totales",
                          new string[] { "", "", "", "", "" }, Request.Path,
                          new string[] { "Nombre"}, 0, new int[] { 1 }, new int[] {1, 2, 3, 4 }, new int[] { 0 }),
                          "GridTParticipantesvsHoras", tituloGrid)
                );
            }
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "No. Reuniones del mes" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre"].ColumnName = "label";
                ldt.Columns["No. Reuniones del mes"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTParticipantesvsHoras", tituloGrafica, 1, FCGpoGraf.Tabular, " "));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTParticipantesvsHoras",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "GrafTParticipantesvsHoras", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);


            #endregion
        }

        private void RepTabParticipantesvsHorasMes1(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneParticipantesHoras());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre", "NumeroEventos", "Horasdedicadasareuniones", "PromedioEvento" });
                ldt.Columns["Nombre"].ColumnName = "Nombre";
                ldt.Columns["NumeroEventos"].ColumnName = "No. Reuniones del mes";
                ldt.Columns["Horasdedicadasareuniones"].ColumnName = "Horas  dedicadas a reuniones(suma)";
                ldt.Columns["PromedioEvento"].ColumnName = "Tiempo promedio / reunión (horas/no. reuniones)";
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("GridTParticipantesvsHoras", ldt, false, "Totales",
                          new string[] { "", "", "", ""}, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 1 }, new int[] { 0, 2, 3,}, new int[] {},0),
                          "RepTabParticipantesvsHorasMes1", tituloGrid, "", 3, FCGpoGraf.TabularBaCoDoTa)
                ) ;
            }
            #endregion
            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "No. Reuniones del mes" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre"].ColumnName = "label";
                ldt.Columns["No. Reuniones del mes"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTParticipantesvsHoras",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabParticipantesvsHorasMes1", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }
        private void RepTabReunionesvsSemana(Control contenedor, string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneNumeroReuniones());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "DiaSemana", "mreuniones" });
                ldt.Columns["DiaSemana"].ColumnName = "Horario pico";
                ldt.Columns["mreuniones"].ColumnName = "No. reuniones";
                ldt.AcceptChanges();


                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridTParticipantesvsHoras", ldt, false, "Totales",
                          new string[] { "", "", "", "", "" }, Request.Path,
                          new string[] { "Horario pico" }, 0, new int[] { 1 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                          "GridTParticipantesvsHoras", tituloGrid)
                );
            }
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Horario pico", "No. Reuniones" });
                ldt.Columns["Horario pico"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTReunionesSemana", tituloGrafica, 1, FCGpoGraf.Tabular, " "));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTReunionesSemana",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "", 10)),
                "GrafTReunionesSemana", "", "", "label", "value", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);


            #endregion
        }
        private void RepTabReunionesvsSemana1(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneNumeroReuniones());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "DiaSemana", "mreuniones" });
                ldt.Columns["DiaSemana"].ColumnName = "Horario pico";
                ldt.Columns["mreuniones"].ColumnName = "No. reuniones";
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("GridTParticipantesvsHoras", ldt, false, "Totales",
                          new string[] { "", "", "", "" }, Request.Path,
                          new string[] { "Horario pico" }, 0, new int[] { 1 }, new int[] { 0, 1}, new int[] { }, 0),
                          "RepTabParticipantesvsHorasMes1", tituloGrid, "", 3, FCGpoGraf.TabularBaCoDoTa)
                );
            }
            #endregion

            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Horario pico", "No. Reuniones" });
                ldt.Columns["Horario pico"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTParticipantesvsHoras",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabParticipantesvsHorasMes1", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }
        private void RepTabUsuariossinutilizacion(Control contenedor, string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            DataTable ldt = DSODataAccess.Execute(ObtieneNoParticipantes());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre"});
                ldt.AcceptChanges();


                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridTNousuarios", ldt, false, "Totales",
                          new string[] { "", "", "", "", "" }, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 1 }, new int[] {1, 2, 3, 4 }, new int[] { 0 }),
                          "GridTNousuarios", tituloGrid)
                );
            }


        }

        private void RepTabUsuariossinutilizacion1(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneNoParticipantes());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre" });
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("GridTNousuarios", ldt, false, "Totales",
                          new string[] { "", "", "", "" }, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 0 }, new int[] { 0, }, new int[] { }, 0),
                          "GridTNousuarios", tituloGrid, "", 3, FCGpoGraf.TabularBaCoDoTa)
                );
            }
            #endregion
            #region Grafica
            


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTNousuarios",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabNousuarios", "", "", "Nombre","", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }

        private void RepTabPlataformaMeet(Control contenedor, string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(Plataformameet());
            string linkURLNavegacion = Request.Path + "?Nav=";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre", "No. Reuniones", "Minutos totales"});

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridTPlataformaMeet", ldt, false, "Totales",
                          new string[] { "", "", "", "", "" }, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 1 }, new int[] {1, 2, 3, 4 }, new int[] { 0 }),
                          "RepTabPlataformaMeetPersonas_T", tituloGrid,"Nombre","")
                );
            }
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "No. Reuniones" });
                ldt.Columns["Nombre"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTPlataforma", tituloGrafica, 1, FCGpoGraf.Tabular, " "));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTPlataforma",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "", 10)),
                "GrafTPlataforma", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }

        private void RepTabPlataformaMeet1(Control contenedor, string tituloGrid, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(ObtieneParticipantesHoras());
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre", "NumeroEventos", "Horasdedicadasareuniones", "PromedioEvento" });
                ldt.Columns["Nombre"].ColumnName = "Nombre";
                ldt.Columns["NumeroEventos"].ColumnName = "No. Reuniones del mes";
                ldt.Columns["Horasdedicadasareuniones"].ColumnName = "Horas  dedicadas a reuniones(suma)";
                ldt.Columns["PromedioEvento"].ColumnName = "Tiempo promedio / reunión (horas/no. reuniones)";
                ldt.AcceptChanges();

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                          DTIChartsAndControls.GridView("GridTParticipantesvsHoras", ldt, false, "Totales",
                          new string[] { "", "", "", "" }, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 1 }, new int[] { 0, 2, 3, }, new int[] { }, 0),
                          "RepTabParticipantesvsHorasMes1", tituloGrid, "", 3, FCGpoGraf.TabularBaCoDoTa)
                );
            }
            #endregion
            #region Grafica
            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "No. Reuniones del mes" });
                if (ldt.Rows[ldt.Rows.Count - 1]["Nombre"].ToString() == "Totales")
                {
                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                }
                ldt.Columns["Nombre"].ColumnName = "label";
                ldt.Columns["No. Reuniones del mes"].ColumnName = "value";
                ldt.AcceptChanges();
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTParticipantesvsHoras",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10)),
                "RepTabParticipantesvsHorasMes1", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }

        private void RepTabPlataformaMeetPersonas(Control contenedor, string tituloGrid, Control contenedorGrafica, string tituloGrafica, int pestaniaActiva)
        {
            #region Grid
            DataTable ldt = DSODataAccess.Execute(Plataformameetdetail(""));
            string linkURLNavegacion = Request.Path + "?Nav=";

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false,
                    new string[] { "Nombre", "No. Reuniones", "Minutos totales" });

                contenedor.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRepNNvlTabla(
                          DTIChartsAndControls.GridView("GridTPlataformaMeet", ldt, false, "Totales",
                          new string[] { "", "", "", "", "" }, Request.Path,
                          new string[] { "Nombre" }, 0, new int[] { 1 }, new int[] { 1, 2, 3, 4 }, new int[] { 0 }),
                          "GridTPlataformaMeet", tituloGrid, "Nombre", "")
                );
            }
            #endregion

            #region Grafica

            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(ldt);
                ldt = dvldt.ToTable(false, new string[] { "Nombre", "No. Reuniones" });
                ldt.Columns["Nombre"].ColumnName = "label";
                ldt.Columns["No. Reuniones"].ColumnName = "value";
                ldt.AcceptChanges();
            }

            contenedorGrafica.Controls.Add(
             DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("GrafTPlataforma", tituloGrafica, 1, FCGpoGraf.Tabular, " "));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafTPlataforma",
                FCAndControls.Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(
                    DTIChartsAndControls.selectTopNTabla(ldt, "", 10)),
                "GrafTPlataforma", "", "", "Nombre", "NumeroEventos", 3, FCGpoGraf.TabularBaCoDoTa, ""), false);
            #endregion
        }

        // TODO : DO Paso 2 - Metodo que crea reporte
        /*Aqui podemos copiar uno de los metodos que estan ya hechos y hacer los ajustes necesarios para mostrar un nuevo reporte*/

        /******************************************************************************************************************************/


        #endregion Reportes

    }
}