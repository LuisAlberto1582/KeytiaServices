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

        #region Exportacion

        public void ExportXLS(string tipoExtensionArchivo)
        {
            CrearXLS(tipoExtensionArchivo);
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();

            int omitirInfoCDR = 0;
            int omitirInfoSiana = 0;

            int.TryParse(param["omitirInfoCDR"].ToString(), out omitirInfoCDR);
            int.TryParse(param["omitirInfoSiana"].ToString(), out omitirInfoSiana);

            try
            {
                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla y grafica) 1

                #region Exportar reportes con tabla y grafica

                if (param["Nav"] == "HistoricoDashN1" || param["Nav"] == "HistoricoN1" || param["Nav"] == "HistoricoN2" || param["Nav"] == "HistoricoN3" || param["Nav"] == "HistoricoN4" ||
                    param["Nav"] == "SitioN1" || param["Nav"] == "SitioN2" ||
                    param["Nav"] == "TDestDashN1" || param["Nav"] == "TDestN1" || param["Nav"] == "TDestN2" ||
                    param["Nav"] == "CenCosJerN1" || param["Nav"] == "CenCosJerN2" ||
                    param["Nav"] == "EmpleMCConSitioN1" ||
                    param["Nav"] == "EmpleMCDashN1" || param["Nav"] == "EmpleMCN1" || param["Nav"] == "EmpleCel10DigsN1" || param["Nav"] == "EmpleMCN2" ||
                    param["Nav"] == "TpLlamN1" || param["Nav"] == "TpLlamN2" ||
                    param["Nav"] == "SitioMatN1" ||
                    param["Nav"] == "CenCosN1" || param["Nav"] == "CenCosN2" ||
                    param["Nav"] == "RepPorCarrierN1" || param["Nav"] == "RepPorCarrierN2" || param["Nav"] == "RepPorCarrierN3" || param["Nav"] == "RepPorCarrierN4" ||
                    param["Nav"] == "RepHistPorDiaSemN1" ||
                    param["Nav"] == "RepLlamRedQN1" || param["Nav"] == "RepLlamRedQN2" ||
                    param["Nav"] == "Hist3AniosN1" ||
                    param["Nav"] == "HistoricoPeEmN1" ||
                    param["Nav"] == "ConColaboradoresN1" ||
                    param["Nav"] == "RepTDestPrsN1" || param["Nav"] == "RepTDestPrsN2" || param["Nav"] == "RepTDestPrsN3" || param["Nav"] == "RepTDestPrsN4" ||
                    param["Nav"] == "HistoricoN1Prs" ||
                    param["Nav"] == "RepTDestPrsLDN3" ||
                    param["Nav"] == "TopAreasN1" || param["Nav"] == "TopAreasN2" ||
                    param["Nav"] == "CenCosJerarquicoN3" || param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoSoloCDRN1" || param["Nav"] == "CenCosJerarquicoN2" ||
                    param["Nav"] == "RepPrsConsumosPorCampaniaN1" ||
                    param["Nav"] == "RepTabResumenSpeedDialN1" ||
                    param["Nav"] == "RepTabTraficoPorHora2Pnl" ||
                    param["Nav"] == "RepEstTabCodEnMasDeNExtensiones" || param["Nav"] == "RepTabCodigosNoAsignados" ||
                    param["Nav"] == "RepTabExtensionesNoAsignadas" || param["Nav"] == "RepMatConsumoPorCarrierN5" ||
                    param["Nav"] == "RepMatConsumoPorEmpleN3" || param["Nav"] == "RepMatConsumoPorSitioN4" ||
                    param["Nav"] == "RepMatConsumoPorCenCosSJN4" || param["Nav"] == "ConsEmpsMasCarosN3" || param["Nav"] == "ConsumoEmpsMasTiempoN2" ||
                    param["Nav"] == "RepEstTabExtAbiertas" || param["Nav"] == "RepTabConsPorEmpleado" || param["Nav"] == "RepTabConsPorEmpleadoN2" ||
                    param["Nav"] == "RepTabConsPorEmpleadoN3" ||
                    param["Nav"] == "RepTabConsPobmasMindeConvN2" || param["Nav"] == "RepTabConsPobmasMindeConvN3" ||
                    param["Nav"] == "ConsEmpmasLlamN2" || param["Nav"] == "ConsNumerosMasMarcadasN2" || param["Nav"] == "ConsLocalidMasMarcadasN2" ||
                    param["Nav"] == "ConsumoEmpsMasTiempo" || param["Nav"] == "RepTabLlamadasEntreSitios" || param["Nav"] == "RepTabLlamadasEntreSitiosN2" ||
                    param["Nav"] == "RepTabLlamadasEntreSitiosN3" ||
                    param["Nav"] == "ConsLugmasCost" || param["Nav"] == "ConsLugmasCostN2" || param["Nav"] == "ConsEmpmasLlamN2" ||
                    param["Nav"] == "RepPorSucursalv2N1" || param["Nav"] == "RepPorSucursalv2N2" ||
                    param["Nav"] == "RepIndConcentracionGastoN2" || param["Nav"] == "RepIndConcentracionGastoN3" || param["Nav"] == "RepIndCodAutoNuevosN2" ||
                    param["Nav"] == "RepIndExtenNuevasN2" ||
                    param["Nav"] == "ConRepTabConsHistAnioActualVsAnterior2" || param["Nav"] == "RepTabIndicadorCodigosNoAsignadosN2" || param["Nav"] == "RepTabIndicadorExtensionesNoAsignadasN2" ||
                    param["Nav"] == "RepLineasExcedentesTelcel2pnl" || param["Nav"] == "PorConceptoN2" ||
                    param["Nav"] == "RepMinPorCarrier" || param["Nav"] == "RepLlamPerdidas2pnl" || param["Nav"] == "RepLlamPerdidasN22Pnl" || param["Nav"] == "RepTabSeeYouOnUtilCliente2pnl" ||
                    param["Nav"] == "RepTabSeeYouOnUtilSistema2pnl" || param["Nav"] == "RepTabSeeYouOnUtilSistemaHist2pnl" || param["Nav"] == "RepTabSeeYouOnCencosHoras2Pnl" || param["Nav"] == "RepBolsaConsumoN1" ||
                    param["Nav"] == "RepDesviosTipoDestinoN1" || param["Nav"] == "RepDesviosTipoDestinoN2" || param["Nav"] == "RepDesviosPorExtensionN1" || param["Nav"] == "RepDesviosCenCostoN1" ||
                    param["Nav"] == "RepDesviosCenCostoN2" || param["Nav"] == "RepDesviosSitioN2" ||
                    param["Nav"] == "HistoricoLlamadasN1" || param["Nav"] == "HistoricoLlamadasN2" || param["Nav"] == "HistoricoLlamadasN3" || param["Nav"] == "HistoricoLlamadasN4" ||
                    param["Nav"] == "RepDesviosPorHoraN1" || param["Nav"] == "RepDesviosTop10LlamadasMN1" || param["Nav"] == "RepDesviosTop10LlamadasGN1" || param["Nav"] == "RepDesviosTop10ExtN1" || param["Nav"] == "RepDesviosPerdidosPorEmpleN1" || param["Nav"] == "RepDesviosPorCarrierN1" ||
                    param["Nav"] == "RepLlamadasPorDispositivoN1" || param["Nav"] == "RepLlamadasPorDispositivoN2" || param["Nav"] == "RepTraficoExtensiones1pnl" || param["Nav"] == "RepLlamadasPerdidasPorTDestN1" ||
                    param["Nav"] == "RepLlamadasPerdidasPorSitioN1" || param["Nav"] == "RepLlamadasPerdidasPorTopEmpleN1" || param["Nav"] == "RepLlamadasPerdidasPorCencosN1" || param["Nav"] == "RepLlamadasPerdidasPorCencosN2"
                    || param["Nav"] == "RepLlamadasAgrupadasEmple" || param["Nav"] == "RepTabVariacionRentaLineas"
                    || param["Nav"] == "RepTabPorExtensionPI" || param["Nav"] == "RepTabPorExtension"
                    || param["Nav"] == "RepTabContestadasYNoPorSitio" || param["Nav"] == "RepTabContestadasYNoUnSitioPorExten" ||
                    param["Nav"] == "RepTabParticipantesvsReunionesMes" || param["Nav"] == "RepTabParticipantesvsReunionesMes1" ||
                    param["Nav"] == "RepTabParticipantesvsHorasMes" || param["Nav"] == "RepTabParticipantesvsHorasMes"
                    )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGrafico" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                    if (param["Nav"] == "HistoricoDashN1" || param["Nav"] == "HistoricoN1")
                    {
                        DataTable GrafConsHist = new DataTable();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte historico");

                        #region Reporte historico

                        if (param["Nav"] != "HistoricoDashN1")
                        {
                            GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistorico("", omitirInfoCDR, omitirInfoSiana));
                        }
                        else
                        {
                            GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistoricoDashboard("", ""));
                        }


                        if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["Total"].ColumnName = "Total";
                            GrafConsHist = DTIChartsAndControls.ordenaTabla(GrafConsHist, "Orden asc");
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo historico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafConsHist.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "HistoricoN1Prs")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte historico");

                        #region Reporte historico

                        DataTable GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistoricoPrs(""));

                        if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            RemoveColHerencia(ref GrafConsHist);
                            GrafConsHist.Columns.Remove("Orden");

                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "TotalSimulado",
                            "TotalReal", "CostoSimulado", "CostoReal", "SM" });

                            if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                            {
                                GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "TotalSimulado" });
                                GrafConsHist.Columns["TotalSimulado"].ColumnName = "Total";
                                GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            }
                            else
                            {
                                GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "TotalReal" });
                                GrafConsHist.Columns["TotalReal"].ColumnName = "Total";
                                GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            }

                            GrafConsHist.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo historico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        /* TABLA */
                        DataTable TablaConsHist = DSODataAccess.Execute(ConsultaConsumoHistoricoPrs(""));

                        if (TablaConsHist.Rows.Count > 0 && TablaConsHist.Columns.Count > 0)
                        {
                            DataView dvTablaConsHist = new DataView(TablaConsHist);
                            RemoveColHerencia(ref TablaConsHist);
                            TablaConsHist.Columns.Remove("Orden");

                            TablaConsHist = dvTablaConsHist.ToTable(false, new string[] { "Mes Anio", "Nombre Mes", "TotalSimulado",
                            "TotalReal", "CostoSimulado", "CostoReal", "SM" });

                            TablaConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            TablaConsHist.Columns.Remove("Mes Anio");

                            TablaConsHist = EliminarColumnasDeAcuerdoABanderas(TablaConsHist);

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(TablaConsHist, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "HistoricoN2" || param["Nav"] == "RepPorCarrierN3")
                    {
                        if(DSODataContext.Schema.ToUpper() == "FCA" && param["Nav"] == "HistoricoN2")
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Sitios");
                            DataTable RepSitio = DSODataAccess.Execute(ConsultaPorSitio(""));
                            if(RepSitio != null && RepSitio.Rows.Count >0)
                            {
                                DataView dvRepConsCenCos = new DataView(RepSitio);
                                //RepSitio = dvRepConsCenCos.ToTable(false,
                                //    new string[] { "Sitio", "Total", "Llamadas", "Minutos", "link" });
                                RepSitio = dvRepConsCenCos.ToTable(false,
                                    new string[] { "Sitio", "Total", "Llamadas", "Minutos"});

                                RepSitio.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                RepSitio.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                RepSitio.AcceptChanges();

                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepSitio, 0, "Totales"), "Reporte", "Tabla");

                                DataView dvRepSitio = new DataView(RepSitio);
                                RepSitio = dvRepConsCenCos.ToTable(false,
                                    new string[] { "Sitio", "Total" });
                                if (RepSitio.Rows[RepSitio.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                                {
                                    RepSitio.Rows[RepSitio.Rows.Count - 1].Delete();
                                }
                                RepSitio.AcceptChanges();

                                //ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepSitio, "Total desc", 10), "Consumo por Sitio", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de Costos", "", "", "Total", "$#,0.00",
                                //            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                                ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepSitio, "Total desc", 10), "Consumo por Sitio", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Sitio", "", "", "Total", "$#,0.00",
                                            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                            }

                        }
                        else
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costos");

                            #region Reporte por CenCos

                            DataTable RepConsCenCos = DSODataAccess.Execute(ConsultaPorCenCos(""));
                            if (RepConsCenCos.Rows.Count > 0 && RepConsCenCos.Columns.Count > 0)
                            {
                                DataView dvRepConsCenCos = new DataView(RepConsCenCos);
                                RepConsCenCos = dvRepConsCenCos.ToTable(false,
                                    new string[] { "Centro de Costos", "Total", "Llamadas", "Minutos" });
                                RepConsCenCos.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                RepConsCenCos.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                RepConsCenCos.AcceptChanges();
                            }

                            if (RepConsCenCos.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsCenCos, 0, "Totales"), "Reporte", "Tabla");
                            }

                            if (RepConsCenCos.Rows.Count > 0 && RepConsCenCos.Columns.Count > 0)
                            {
                                DataView dvRepConsCenCos = new DataView(RepConsCenCos);
                                RepConsCenCos = dvRepConsCenCos.ToTable(false,
                                    new string[] { "Centro de Costos", "Total" });
                                if (RepConsCenCos.Rows[RepConsCenCos.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                                {
                                    RepConsCenCos.Rows[RepConsCenCos.Rows.Count - 1].Delete();
                                }
                                RepConsCenCos.AcceptChanges();
                            }

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsCenCos, "Total desc", 10), "Consumo por centro de costos", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de Costos", "", "", "Total", "$#,0.00",
                                             true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Reporte por CenCos
                        }

                    }
                    else if (param["Nav"] == "EmpleMCConSitioN1")
                    {
                        #region Reporte TOP empleado con sitio

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



                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por " + nombreColumnaColaborador);



                        DataTable RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaPorEmpleMasCarosConSitio(""));
                        if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                        {
                            DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                            //NZ 20160823
                            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                            {
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                new string[] { "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion", "Puesto" });
                            }
                            else
                            {
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                new string[] { "ExtensionEmple", "Nombre Completo", "Nombre Centro de Costos", "Sitio", "Total", "Numero", "Duracion" });
                            }

                            RepConsEmpleMasCaros.Columns["ExtensionEmple"].ColumnName = "Extensión";
                            RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                            RepConsEmpleMasCaros.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                            RepConsEmpleMasCaros.Columns["Sitio"].ColumnName = "Sitio";
                            RepConsEmpleMasCaros.Columns["Total"].ColumnName = "Total";
                            RepConsEmpleMasCaros.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            RepConsEmpleMasCaros.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepConsEmpleMasCaros.AcceptChanges();
                        }

                        if (RepConsEmpleMasCaros.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsEmpleMasCaros, 1, "Totales"), "Reporte", "Tabla");
                        }

                        if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                        {
                            DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                new string[] { nombreColumnaColaborador, "Total" });
                            if (RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                            {
                                RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1].Delete();
                            }
                            RepConsEmpleMasCaros.AcceptChanges();
                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsEmpleMasCaros, "Total desc", 10), "Consumo por " + nombreColumnaColaborador, new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, nombreColumnaColaborador, "", "", "Total", "$#,0.00",
                                        true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Top Empleado con sitio
                    }
                    else if (param["Nav"] == "HistoricoN3" || param["Nav"] == "SitioN2" || param["Nav"] == "TDestN2" ||
                        param["Nav"] == "EmpleMCDashN1" || param["Nav"] == "EmpleMCN1" || param["Nav"] == "EmpleCel10DigsN1" || param["Nav"] == "CenCosN2" || param["Nav"] == "RepPorCarrierN4" ||
                        param["Nav"] == "TopAreasN2" || param["Nav"] == "CenCosJerarquicoN3"
                        )
                    {
                        // JF.2021-06-03 Al alterarse la navegacion del reporte por tipo destino, existe un caso donde su nivel 3 tiene el parametro de navegacion Tdest = ###
                        // Para este caso es necesario exportar una información distinta al proceso normal que ejecuta este metodo. 
                        // Si se entra con esquema FCA y se cuenta con Parametro Tdest se ejecuta un proceso distinto. En caso de no, se ejecutara normalmente. 
                        if(DSODataContext.Schema.ToUpper() == "FCA" && !string.IsNullOrEmpty(param["TDest"]) && Request.QueryString["Sitio"] == null)
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por sitio");

                            #region Reporte por sitio

                            DataTable GrafConsPorSitio = DSODataAccess.Execute(ConsultaConsumoPorSitio("", omitirInfoCDR, omitirInfoSiana));

                            if (GrafConsPorSitio.Rows.Count > 0 && GrafConsPorSitio.Columns.Count > 0)
                            {

                                DataView dvGrafConsHist = new DataView(GrafConsPorSitio);
                                if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                                {
                                    GrafConsPorSitio = dvGrafConsHist.ToTable(false, new string[] { "Nombre Sitio", "Total", "Numero", "Duracion" });
                                    campoOrdenamiento = "Total desc";
                                    campoAGraficar = "Total";
                                    numberFormat = "$#,0.00";
                                    GrafConsPorSitio.Columns["Total"].ColumnName = "Total";
                                }
                                else
                                {
                                    GrafConsPorSitio = dvGrafConsHist.ToTable(false, new string[] { "Nombre Sitio", "Numero", "Duracion" });
                                    campoOrdenamiento = "Cantidad llamadas desc";
                                    campoAGraficar = "Cantidad llamadas";
                                    numberFormat = "#";
                                }
                                GrafConsPorSitio.Columns["Nombre Sitio"].ColumnName = "Sitio";
                                GrafConsPorSitio.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                GrafConsPorSitio.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"


                                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                                if (DSODataContext.Schema.ToLower() == "k5banorte")
                                {
                                    GrafConsPorSitio = DTIChartsAndControls.ordenaTabla(GrafConsPorSitio, "Total DESC");
                                }
                            }

                            //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10
                            if (DSODataContext.Schema.ToLower() == "ula")
                            {
                                ExportacionExcelRep.CrearGrafico(GrafConsPorSitio, "Consumo por sitio", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Sitio", "", "", "Total", "$#,0.00",
                                true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                            }
                            else
                            {
                                ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafConsPorSitio, campoOrdenamiento, 10), "Consumo por sitio", new string[] { campoAGraficar }, new string[] { campoAGraficar }, new string[] { campoAGraficar }, "Sitio", "", "", campoAGraficar, numberFormat,
                                true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                            }



                            if (GrafConsPorSitio.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsPorSitio, 0, "Totales"), "Reporte", "Tabla");
                            }

                            #endregion Reporte por sitio
                        }
                        else
                        {
                            DataTable RepConsEmpleMasCaros = new DataTable();

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

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por " + nombreColumnaColaborador);

                            #region Reporte por empleado

                            if (param["Nav"] != "EmpleMCDashN1")
                            {
                                RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaPorEmpleMasCaros("", int.MaxValue, omitirInfoCDR, omitirInfoSiana));
                            }
                            else
                            {
                                //Reporte Dashboard
                                RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaReporteTopNEmpleDashboard("", "", 10));
                                RepConsEmpleMasCaros = DTIChartsAndControls.ordenaTabla(RepConsEmpleMasCaros, "Total DESC");
                            }

                            if (DSODataContext.Schema.ToUpper() == "BIMBO")
                            {
                                if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                                {
                                    DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);

                                    RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                            new string[] { "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion", "CostoFija", "CostoMovil", "Total" });
                                    RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                                    RepConsEmpleMasCaros.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                                    RepConsEmpleMasCaros.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                    RepConsEmpleMasCaros.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                    RepConsEmpleMasCaros.Columns["CostoFija"].ColumnName = "Consumo Total de Telefonía Fija";
                                    RepConsEmpleMasCaros.Columns["CostoMovil"].ColumnName = "Consumo Total de Telefonía Móvil";
                                    RepConsEmpleMasCaros.Columns["Total"].ColumnName = "Total";
                                    RepConsEmpleMasCaros.AcceptChanges();
                                }
                            }
                            else
                            {
                                if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                                {
                                    DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                                    if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                                    {
                                        RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                            new string[] { "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "Puesto" });

                                        RepConsEmpleMasCaros.Columns["No Nomina"].ColumnName = "Nomina";
                                    }
                                    else
                                    {

                                        if (DSODataContext.Schema.ToLower() == "institutomora")
                                        {
                                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                            new string[] { "Extension", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" });
                                        }
                                        // JF.2021-06-03 Se requiere quitar columna centro de costos de tabla ingresada a excel
                                        if (DSODataContext.Schema.ToUpper() == "FCA")
                                        {
                                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                           new string[] { "Nombre Completo", "Total", "Numero", "Duracion" });
                                        }
                                        else
                                        {
                                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                           new string[] { "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" });
                                        }
                                    }


                                    RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;

                                    // JF.2021-06-03 Para cliente FCA la columna Nombre Centro de Costos no se agrega. Se agrega condicional.
                                    if (DSODataContext.Schema.ToUpper() != "FCA")
                                    {
                                        RepConsEmpleMasCaros.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                                    }

                                    RepConsEmpleMasCaros.Columns["Total"].ColumnName = "Total";
                                    RepConsEmpleMasCaros.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                    RepConsEmpleMasCaros.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                    RepConsEmpleMasCaros.AcceptChanges();

                                    //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                                    if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                                    {
                                        RepConsEmpleMasCaros = DTIChartsAndControls.ordenaTabla(RepConsEmpleMasCaros, "Total DESC");
                                    }
                                }
                            }

                            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                            {
                                campoOrdenamiento = "Total desc";
                                campoAGraficar = "Total";
                                numberFormat = "$#,0.00";

                            }
                            else
                            {
                                RepConsEmpleMasCaros.Columns.Remove("Total");
                                campoOrdenamiento = "Cantidad llamadas desc";
                                campoAGraficar = "Cantidad llamadas";
                                numberFormat = "#,0";
                            }

                            if (RepConsEmpleMasCaros.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsEmpleMasCaros, 0, "Totales"), "Reporte", "Tabla");
                            }

                            if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                            {
                                DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { nombreColumnaColaborador, campoAGraficar });
                                if (RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                                {
                                    RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1].Delete();
                                }
                                RepConsEmpleMasCaros.AcceptChanges();
                            }
                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsEmpleMasCaros, campoOrdenamiento, 10), "Consumo por " + nombreColumnaColaborador, new string[] { campoAGraficar }, new string[] { campoAGraficar }, new string[] { campoAGraficar }, nombreColumnaColaborador, "", "", campoAGraficar, numberFormat,
                                            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Reporte por empleado
                        }
                    }
                    else if (param["Nav"] == "SitioN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por sitio");

                        #region Reporte por sitio

                        DataTable GrafConsPorSitio = DSODataAccess.Execute(ConsultaConsumoPorSitio("", omitirInfoCDR, omitirInfoSiana));

                        if (GrafConsPorSitio.Rows.Count > 0 && GrafConsPorSitio.Columns.Count > 0)
                        {

                            DataView dvGrafConsHist = new DataView(GrafConsPorSitio);
                            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                            {
                                GrafConsPorSitio = dvGrafConsHist.ToTable(false, new string[] { "Nombre Sitio", "Total", "Numero", "Duracion" });
                                campoOrdenamiento = "Total desc";
                                campoAGraficar = "Total";
                                numberFormat = "$#,0.00";
                                GrafConsPorSitio.Columns["Total"].ColumnName = "Total";
                            }
                            else
                            {
                                GrafConsPorSitio = dvGrafConsHist.ToTable(false, new string[] { "Nombre Sitio", "Numero", "Duracion" });
                                campoOrdenamiento = "Cantidad llamadas desc";
                                campoAGraficar = "Cantidad llamadas";
                                numberFormat = "#";
                            }
                            GrafConsPorSitio.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            GrafConsPorSitio.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            GrafConsPorSitio.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"


                            //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                            if (DSODataContext.Schema.ToLower() == "k5banorte")
                            {
                                GrafConsPorSitio = DTIChartsAndControls.ordenaTabla(GrafConsPorSitio, "Total DESC");
                            }
                        }

                        //BG.20170726 Si el cliente es ULA la grafica no lleva el TOP 10
                        if (DSODataContext.Schema.ToLower() == "ula")
                        {
                            ExportacionExcelRep.CrearGrafico(GrafConsPorSitio, "Consumo por sitio", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Sitio", "", "", "Total", "$#,0.00",
                            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                        else
                        {
                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafConsPorSitio, campoOrdenamiento, 10), "Consumo por sitio", new string[] { campoAGraficar }, new string[] { campoAGraficar }, new string[] { campoAGraficar }, "Sitio", "", "", campoAGraficar, numberFormat,
                            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }



                        if (GrafConsPorSitio.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsPorSitio, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por sitio
                    }
                    else if (param["Nav"] == "TDestDashN1" || param["Nav"] == "TDestN1" || param["Nav"] == "EmpleMCN2" ||
                        param["Nav"] == "TpLlamN2" || param["Nav"] == "HistoricoN4" || param["Nav"] == "RepMatConsumoPorCarrierN5" ||
                        param["Nav"] == "RepMatConsumoPorEmpleN3" || param["Nav"] == "RepMatConsumoPorSitioN4" ||
                        param["Nav"] == "RepMatConsumoPorCenCosSJN4" || param["Nav"] == "ConsEmpsMasCarosN3" || param["Nav"] == "ConsumoEmpsMasTiempoN2" ||
                        param["Nav"] == "RepIndConcentracionGastoN3")
                    {
                        DataTable GrafConsPorTDest = new DataTable();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo destino");

                        #region Reporte por tipo destino

                        if (param["Nav"] != "TDestDashN1")
                        {
                            GrafConsPorTDest = DSODataAccess.Execute(ConsultaConsumoPorTipoDestino("", omitirInfoCDR, omitirInfoSiana));
                        }
                        else
                        {
                            //Reporte con sp optimizado
                            GrafConsPorTDest = DSODataAccess.Execute(ConsultaReportePorTDestDashboard(string.Empty, string.Empty));
                        }

                        /*bandera que identifica si se desglosara el costo de la llamada*/
                        int desglosaCosto = 0;
                        if (Session["DesgloseCosto"] != null)
                        {
                            desglosaCosto = Convert.ToInt32(Session["DesgloseCosto"]);
                        }


                        if (GrafConsPorTDest.Rows.Count > 0 && GrafConsPorTDest.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsPorTDest);

                            if (GrafConsPorTDest.Columns.Contains("Costo"))
                            {
                                GrafConsPorTDest = dvGrafConsHist.ToTable(false, new string[] { "Nombre Tipo Destino", "Costo", "CostoSM", "Total", "Numero", "Duracion" });
                            }
                            else
                            {
                                GrafConsPorTDest = dvGrafConsHist.ToTable(false, new string[] { "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                            }


                            GrafConsPorTDest.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            GrafConsPorTDest.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            GrafConsPorTDest.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            if (desglosaCosto == 1)
                            {
                                if (GrafConsPorTDest.Columns.Contains("Costo"))
                                {
                                    GrafConsPorTDest.Columns["Costo"].ColumnName = "Costo Llamada";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                }

                                if (GrafConsPorTDest.Columns.Contains("CostoSM"))
                                {
                                    GrafConsPorTDest.Columns["CostoSM"].ColumnName = "Costo Servicio Medido";
                                }


                            }
                            GrafConsPorTDest.Columns["Total"].ColumnName = "Total";
                            GrafConsPorTDest = DTIChartsAndControls.ordenaTabla(GrafConsPorTDest, "Total DESC");

                            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                            {
                                campoOrdenamiento = "Total desc";
                                campoAGraficar = "Total";
                                numberFormat = "$#,0.00";

                            }
                            else
                            {
                                GrafConsPorTDest.Columns.Remove("Total");
                                campoOrdenamiento = "Cantidad llamadas desc";
                                campoAGraficar = "Cantidad llamadas";
                                numberFormat = "#,0";
                            }

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafConsPorTDest, campoOrdenamiento, 10), "Consumo por tipo destino", new string[] { campoAGraficar }, new string[] { campoAGraficar },
                            new string[] { campoAGraficar }, "Tipo Destino", "", "", campoAGraficar, numberFormat,
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 550, 300);

                        if (GrafConsPorTDest.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsPorTDest, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por tipo destino
                    }
                    else if (param["Nav"] == "CenCosJerN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costos Jerarquico");

                        #region Reporte por cencos jerarquico

                        DataTable RepConsCenCosJerar = DSODataAccess.Execute(ConsultaConsumoPorCenCosJerarquico(""));
                        if (RepConsCenCosJerar.Rows.Count > 0 && RepConsCenCosJerar.Columns.Count > 0)
                        {
                            DataView dvRepConsCenCosJerar = new DataView(RepConsCenCosJerar);
                            RepConsCenCosJerar = dvRepConsCenCosJerar.ToTable(false,
                                new string[] { "Nombre Centro de Costos", "Total", "TotLlamadas", "Duracion Minutos" });
                            RepConsCenCosJerar.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                            RepConsCenCosJerar.Columns["Total"].ColumnName = "Total";
                            RepConsCenCosJerar.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            RepConsCenCosJerar.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";  //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepConsCenCosJerar.AcceptChanges();
                        }

                        if (RepConsCenCosJerar.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsCenCosJerar, 0, "Totales"), "Reporte", "Tabla");
                        }

                        if (RepConsCenCosJerar.Rows.Count > 0 && RepConsCenCosJerar.Columns.Count > 0)
                        {
                            DataView dvRepConsCenCosJerar = new DataView(RepConsCenCosJerar);
                            RepConsCenCosJerar = dvRepConsCenCosJerar.ToTable(false,
                                new string[] { "Centro de costos", "Total" });
                            if (RepConsCenCosJerar.Rows[RepConsCenCosJerar.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                            {
                                RepConsCenCosJerar.Rows[RepConsCenCosJerar.Rows.Count - 1].Delete();
                            }
                            RepConsCenCosJerar.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsCenCosJerar, "Total desc", 10), "Consumo por centro de costos jerarquico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de costos", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte por cencos jerarquico
                    }
                    else if (param["Nav"] == "CenCosJerN2")
                    {
                        if (TieneCenCosHijos(param["CenCos"]))
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costos Jerarquico");

                            #region Reporte por cencos jerarquico

                            DataTable RepConsCenCosJerar = DSODataAccess.Execute(ConsultaConsumoPorCenCosJerarquico(""));
                            if (RepConsCenCosJerar.Rows.Count > 0 && RepConsCenCosJerar.Columns.Count > 0)
                            {
                                DataView dvRepConsCenCosJerar = new DataView(RepConsCenCosJerar);
                                RepConsCenCosJerar = dvRepConsCenCosJerar.ToTable(false,
                                    new string[] { "Nombre Centro de Costos", "Total", "TotLlamadas", "Duracion Minutos" });
                                RepConsCenCosJerar.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                                RepConsCenCosJerar.Columns["Total"].ColumnName = "Total";
                                RepConsCenCosJerar.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";         //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                RepConsCenCosJerar.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                RepConsCenCosJerar.AcceptChanges();
                            }

                            if (RepConsCenCosJerar.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsCenCosJerar, 0, "Totales"), "Reporte", "Tabla");
                            }

                            if (RepConsCenCosJerar.Rows.Count > 0 && RepConsCenCosJerar.Columns.Count > 0)
                            {
                                DataView dvRepConsCenCosJerar = new DataView(RepConsCenCosJerar);
                                RepConsCenCosJerar = dvRepConsCenCosJerar.ToTable(false,
                                    new string[] { "Centro de costos", "Total" });
                                if (RepConsCenCosJerar.Rows[RepConsCenCosJerar.Rows.Count - 1]["Centro de costos"].ToString() == "Totales")
                                {
                                    RepConsCenCosJerar.Rows[RepConsCenCosJerar.Rows.Count - 1].Delete();
                                }
                                RepConsCenCosJerar.AcceptChanges();
                            }

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsCenCosJerar, "Total desc", 10), "Consumo por centro de costos jerarquico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de costos", "", "", "Total", "$#,0.00",
                                           true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Reporte por cencos jerarquico
                        }
                        else
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por colaborador");

                            #region Reporte por empleado

                            DataTable RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaPorEmpleMasCaros("", int.MaxValue));
                            if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                            {
                                DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                                if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                                {
                                    RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "Puesto" });
                                }
                                else
                                {
                                    RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" });
                                }

                                if (RepConsEmpleMasCaros.Columns.Contains("No Nomina"))
                                {
                                    //BG.20161113 se agrega columna de nomina
                                    RepConsEmpleMasCaros.Columns["No Nomina"].ColumnName = "Nomina";
                                }

                                RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = "Colaborador";
                                RepConsEmpleMasCaros.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                                RepConsEmpleMasCaros.Columns["Total"].ColumnName = "Total";
                                RepConsEmpleMasCaros.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                RepConsEmpleMasCaros.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                RepConsEmpleMasCaros.AcceptChanges();

                                //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                                {
                                    RepConsEmpleMasCaros = DTIChartsAndControls.ordenaTabla(RepConsEmpleMasCaros, "Total DESC");
                                }
                            }

                            if (RepConsEmpleMasCaros.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsEmpleMasCaros, 0, "Totales"), "Reporte", "Tabla");
                            }

                            if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                            {
                                DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { "Colaborador", "Total" });
                                if (RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                                {
                                    RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1].Delete();
                                }
                                RepConsEmpleMasCaros.AcceptChanges();
                            }
                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsEmpleMasCaros, "Total desc", 10), "Consumo por colaborador", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                            true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Reporte por empleado
                        }
                    }
                    else if (param["Nav"] == "TpLlamN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo de llamada");

                        #region Reporte por tipo de llamada

                        DataTable ldt = null;

                        //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                        ldt = DSODataAccess.Execute(GeneraConsultaPorTpLlamada1Pnl(""));


                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Tipo Llamada", "Total" });
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();

                            //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                            {
                                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");
                            }
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por tipo de llamada", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Tipo de llamada", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por tipo de llamada
                    }
                    else if (param["Nav"] == "SitioMatN1" || param["Nav"] == "RepPorCarrierN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo por sitio");

                        #region Reporte por sitio

                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorSitioMat(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            ldt.Columns.Remove("Codigo Sitio");
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt.Columns["TotalCosto"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por sitio", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Sitio", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por sitio
                    }
                    else if (param["Nav"] == "CenCosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costos");

                        #region Reporte por centro de costos

                        DataTable ldt = DSODataAccess.Execute(ConsultaPorCenCos(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsCenCos = new DataView(ldt);
                            ldt = dvRepConsCenCos.ToTable(false,
                                new string[] { "Centro de Costos", "Total", "Llamadas", "Minutos" });
                            ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por centro de costos", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de Costos", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por centro de costos
                    }
                    else if (param["Nav"] == "RepPorCarrierN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por carrier");

                        #region Reporte por carrier

                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorCarrierMat(""));
                        if (ldt.Columns.Contains("Codigo Carrier"))
                        {
                            ldt.Columns.Remove("Codigo Carrier");
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            ldt.Columns["TotalCosto"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por Carrier", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Carrier", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por carrier
                    }
                    else if (param["Nav"] == "RepHistPorDiaSemN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte consumo por día de la semana");

                        #region Reporte consumo por día de la semana

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepHistPorDiaDeLaSemana());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "DiaSemana", "Total" });
                            ldt.Columns["DiaSemana"].ColumnName = "Día";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por día de la semana", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Día", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte consumo por día de la semana
                    }
                    else if (param["Nav"] == "RepLlamRedQN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas por la red Q");

                        #region Reporte Llamadas por la red Q

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoSimuladoSitiosDestino(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Nombre Sitio Origen", "Total2", "Duracion", "Llamadas" });
                            ldt.Columns["Nombre Sitio Origen"].ColumnName = "Sitio";
                            ldt.Columns["Total2"].ColumnName = "Total";
                            ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";    //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Llamadas por la red Q", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Sitio", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas por la red Q
                    }
                    else if (param["Nav"] == "RepLlamRedQN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas por la red Q");

                        #region Reporte Llamadas por la red Q N2

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoSimuladoDestino(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Nombre Sitio Destino", "Total", "Duracion Minutos", "TotLlamadas", });
                            ldt.Columns["Nombre Sitio Destino"].ColumnName = "Destino";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.Columns["TotLlamadas"].ColumnName = "Cantidad llamadas";         //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Llamadas por la red Q", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Destino", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas por la red Q N2
                    }
                    else if (param["Nav"] == "Hist3AniosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Historico de 3 años");

                        #region Reporte Historico de 3 años

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepHistorico3Anios());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Mes", "2 Años atras", "Año anterior", "Año actual" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Historico de 3 años", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Historico de 3 años
                    }
                    else if (param["Nav"] == "HistoricoPeEmN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo histórico");

                        #region Historico de empleado

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

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo historico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafConsHist.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Historico de empleado
                    }
                    else if (param["Nav"] == "ConColaboradoresN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Colaboradores Directos");

                        #region Reporte Colaboradores

                        DataTable ldt = DSODataAccess.Execute(ConsultaReporteColaboradores());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Colaborador", "Total" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Reporte Colaboradores", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Colaboradores
                    }
                    else if (param["Nav"] == "RepTDestPrsN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Carrier");

                        #region Reporte

                        DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestPrsN2(""));

                        //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            RemoveColHerencia(ref ldt);

                            ldt = dvldt.ToTable(false, new string[] { "Nombre Carrier", "TotalSimulado",
                                "TotalReal", "CostoSimulado", "CostoReal", "SM" , "Numero", "Duracion" });

                            ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            //ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();

                            ldt = EliminarColumnasDeAcuerdoABanderas(ldt);
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica


                        DataTable dtTablaCarrier = DSODataAccess.Execute(ConsultaPorTDestPrsN2(""));

                        if (dtTablaCarrier.Rows.Count > 0 && dtTablaCarrier.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtTablaCarrier);
                            RemoveColHerencia(ref dtTablaCarrier);

                            dtTablaCarrier.Columns["Nombre Carrier"].ColumnName = "Carrier";

                            dtTablaCarrier = dvldt.ToTable(false, new string[] { "Carrier", "TotalSimulado",
                                "TotalReal", "CostoSimulado", "CostoReal", "SM" });

                            if (dtTablaCarrier.Rows[dtTablaCarrier.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                            {
                                dtTablaCarrier.Rows[dtTablaCarrier.Rows.Count - 1].Delete();
                            }

                            if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                            {
                                dtTablaCarrier = dvldt.ToTable(false, new string[] { "Carrier", "TotalSimulado" });
                                dtTablaCarrier.Columns["TotalSimulado"].ColumnName = "Total";

                            }
                            else
                            {
                                dtTablaCarrier = dvldt.ToTable(false, new string[] { "Carrier", "TotalReal" });
                                dtTablaCarrier.Columns["TotalReal"].ColumnName = "Total";

                            }


                            dtTablaCarrier.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtTablaCarrier, "Total desc", 10),
                            "Reporte por Carrier", new string[] { "Total" },
                            new string[] { "Total" }, new string[] { "Total" },
                            "Carrier", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                            lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion
                    }
                    else if (param["Nav"] == "RepTDestPrsN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo de destino");

                        #region Reporte

                        DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestPrs(""));

                        //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            RemoveColHerencia(ref ldt);

                            ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "TotalSimulado",
                            "TotalReal", "CostoSimulado", "CostoReal", "SM" , "Numero", "Duracion" });

                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                            //ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();

                            ldt = EliminarColumnasDeAcuerdoABanderas(ldt);
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica


                        DataTable dtTablaTDest = DSODataAccess.Execute(ConsultaPorTDestPrs(""));

                        if (dtTablaTDest.Rows.Count > 0 && dtTablaTDest.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtTablaTDest);
                            RemoveColHerencia(ref ldt);

                            dtTablaTDest.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";

                            dtTablaTDest = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalSimulado",
                            "TotalReal", "CostoSimulado", "CostoReal", "SM" });

                            if (dtTablaTDest.Rows[dtTablaTDest.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                            {
                                dtTablaTDest.Rows[dtTablaTDest.Rows.Count - 1].Delete();
                            }


                            if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                            {
                                dtTablaTDest = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalSimulado" });
                                dtTablaTDest.Columns["TotalSimulado"].ColumnName = "Total";
                            }
                            else
                            {
                                dtTablaTDest = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalReal" });
                                dtTablaTDest.Columns["TotalReal"].ColumnName = "Total";
                            }

                            dtTablaTDest.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtTablaTDest, "Total desc", 10),
                            "Reporte por tipo de destino", new string[] { "Total" },
                            new string[] { "Total" }, new string[] { "Total" },
                            "Tipo de destino", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                            lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion
                    }
                    else if (param["Nav"] == "RepTDestPrsN3")
                    {
                        #region reporte
                        if (param["TDest"] == "238135")
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Tipo De Destino");

                            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestLDPrsN2(""));

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                RemoveColHerencia(ref ldt);

                                ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "TotalSimulado",
                                "TotalReal", "CostoSimulado", "CostoReal", "SM" , "Numero", "Duracion" });

                                ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de Destino";
                                //ldt.Columns["Total"].ColumnName = "Total";
                                ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                ldt.AcceptChanges();

                                ldt = EliminarColumnasDeAcuerdoABanderas(ldt);
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                            #region Grafica

                            DataTable dtTablaTDestLD = DSODataAccess.Execute(ConsultaPorTDestLDPrsN2(""));

                            if (dtTablaTDestLD.Rows.Count > 0 && dtTablaTDestLD.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(dtTablaTDestLD);
                                RemoveColHerencia(ref ldt);

                                dtTablaTDestLD.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";

                                dtTablaTDestLD = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalSimulado",
                                "TotalReal", "CostoSimulado", "CostoReal", "SM" });


                                if (dtTablaTDestLD.Rows[dtTablaTDestLD.Rows.Count - 1]["Tipo de destino"].ToString() == "Totales")
                                {
                                    dtTablaTDestLD.Rows[dtTablaTDestLD.Rows.Count - 1].Delete();
                                }

                                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                                {
                                    dtTablaTDestLD = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalSimulado" });
                                    dtTablaTDestLD.Columns["TotalSimulado"].ColumnName = "Total";

                                }
                                else
                                {
                                    dtTablaTDestLD = dvldt.ToTable(false, new string[] { "Tipo de destino", "TotalReal" });
                                    dtTablaTDestLD.Columns["TotalReal"].ColumnName = "Total";

                                }


                                dtTablaTDestLD.AcceptChanges();
                            }

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtTablaTDestLD, "Total desc", 10),
                                "Reporte por tipo de destino", new string[] { "Total" },
                                new string[] { "Total" }, new string[] { "Total" },
                                "Tipo de destino", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                                lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Grafica

                        }
                        else if (param["TDest"] == "83283") //BG.20160912 Rentas Telmex
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Rentas Telmex");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleRentasPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "CtaTelmex", "CenCos", "Descripcion", "Folio", "Tipo", "Costo" });

                                ldt.Columns["CtaTelmex"].ColumnName = "Cta Telmex";
                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                        }
                        else if (param["TDest"] == "255748") //BG.20160912 Enlaces Telmex
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Enlaces Telmex");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlacesPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "CtaTelmex", "CenCos", "Descripcion", "Folio", "Comercio", "Costo" });

                                ldt.Columns["CtaTelmex"].ColumnName = "Cta Telmex";

                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else if (param["TDest"] == "83492") //BG.20160912 Uninet Telmex
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Servicio Uninet Telmex");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleUninetPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "Identificacion de Servicios", "LeyendaCobro", "Importe", "Comercio" });

                                //ldt.Columns["PorcentajeCobro"].ColumnName = "Porcentaje Cobro";
                                ldt.Columns["LeyendaCobro"].ColumnName = "Leyenda Cobro";

                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                        }
                        else if (param["TDest"] == "383" && param["Carrier"] == "374") //BG.20160912 800Entrada Telmex
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte 800 Entrada Telmex");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalle800EntPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "Cuenta", "LadaTelefono", "No800", "Comercio",
                                    "Cencos", "Llamadas", "Minutos", "Costo",  });

                                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else if (param["TDest"] == "279390") //BG.20161004 ENLACES AXTEL
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Enlaces Axtel");

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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                        }
                        else if (param["TDest"] == "279391") //BG.20161004 ENLACES AVANTEL
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Enlaces Avantel");

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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else if (param["TDest"] == "383" && param["Carrier"] == "200413") //BG.20170116 800Entrada Avantel
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte 800 Entrada Avantel");

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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                        }
                        else if (param["TDest"] == "83283" && param["Carrier"] == "291794") //BG.20170216 RENTAS TELNOR
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Rentas Telnor");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleRentasTelnorPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });
                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else if (param["TDest"] == "238134" && param["Carrier"] == "291794") //BG.20170216 SM TELNOR
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte SM Telnor");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleSMTelnorPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });
                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else if (param["TDest"] == "292895" && param["Carrier"] == "291794") //BG.20170216 ENLACES TELNOR
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Enlaces Telnor");

                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlTelnorPrs());

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false, new string[] { "Descripcion", "CenCos", "Importe" });
                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid
                        }
                        else
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Centro de Costos");

                            DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestCencosPrsN3(""));

                            //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                    new string[] { "Nombre Centro de Costos", "TotImporte", "LLamadas", "TotMin" });
                                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                                ldt.Columns["TotImporte"].ColumnName = "Total";
                                ldt.Columns["LLamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                ldt.Columns["TotMin"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                ldt.AcceptChanges();
                            }

                            #region Grid

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                            #endregion Grid

                            #region Grafica

                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                    new string[] { "Centro de Costos", "Total" });
                                if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                                {
                                    ldt.Rows[ldt.Rows.Count - 1].Delete();
                                }
                                ldt.AcceptChanges();
                            }

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10),
                                "Reporte por Centro de Costos", new string[] { "Total" },
                                new string[] { "Total" }, new string[] { "Total" },
                                "Centro de Costos", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                                lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            #endregion Grafica
                        }

                        #endregion
                    }
                    else if (param["Nav"] == "RepTDestPrsN4")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Colaborador");

                        #region Reporte

                        RepTDestPrsN4 rep = new RepTDestPrsN4(Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59", Request.Path, listadoParametros);

                        DataTable ldt = DSODataAccess.Execute(rep.ConsultaPorTDestEmplePrsN4(""));

                        //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "NombreEmple", "TotImporte", "LLamadas", "TotMin" });
                            ldt.Columns["NombreEmple"].ColumnName = "Colaborador";
                            ldt.Columns["TotImporte"].ColumnName = "Total";
                            ldt.Columns["LLamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["TotMin"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10),
                            "Reporte por Colaborador", new string[] { "Total" },
                            new string[] { "Total" }, new string[] { "Total" },
                            "Colaborador", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                            lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion
                    }
                    else if (param["Nav"] == "RepTDestPrsLDN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por Centro de Costos");

                        #region Reporte

                        DataTable ldt = DSODataAccess.Execute(ConsultaPorTDestCencosPrsN3(""));

                        //Editas el DataTable para seleccionar las columnas necesarias y sus nombres.
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Nombre Centro de Costos", "TotImporte", "LLamadas", "TotMin" });
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                            ldt.Columns["TotImporte"].ColumnName = "Total";
                            ldt.Columns["LLamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["TotMin"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Centro de Costos", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10),
                            "Reporte por Centro de Costos", new string[] { "Total" },
                            new string[] { "Total" }, new string[] { "Total" },
                            "Centro de Costos", "", "", "Total", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                            lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion
                    }
                    else if (param["Nav"] == "TopAreasN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Áreas de Mayor Consumo");

                        #region Reporte Áreas de Mayor Consumo

                        DataTable ldt = DSODataAccess.Execute(ConsultaAreasMayorConsumo(""));



                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvTopAreas = new DataView(ldt);
                            ldt = dvTopAreas.ToTable(false,
                                new string[] { "Area", "Importe", "Porcentaje" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Area"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Area"].ColumnName = "Area";
                            ldt.Columns["Importe"].ColumnName = "Total";
                            ldt.Columns["Porcentaje"].ColumnName = "PorcentajeOrig";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Áreas de Mayor Consumo", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Area", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        ldt.Columns.Add("Porcentaje", typeof(string));

                        if (ldt.Rows.Count > 0)
                        {
                            foreach (DataRow row in ldt.Rows)
                            {
                                row["Porcentaje"] = Convert.ToDecimal(row["PorcentajeOrig"]).ToString("##,0.00 %");
                            }

                            ldt.Columns.Remove("PorcentajeOrig");

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Áreas de Mayor Consumo
                    }
                    else if (param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costros jerárquico");

                        #region Reporte Jerarquico V2

                        int idCenCos;

                        if (param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoSoloCDRN1")
                        {
                            DataTable dtCenCos = DSODataAccess.Execute("SELECT CenCos FROM " + DSODataContext.Schema +
                                               ".vUsuario WHERE dtfinvigencia >= getdate() AND iCodCatalogo=" + Session["iCodUsuario"].ToString() + " AND CenCos IS NOT NULL");

                            idCenCos = (dtCenCos.Rows.Count == 0) ? 0 : Convert.ToInt32(dtCenCos.Rows[0][0]);
                        }
                        else { idCenCos = Convert.ToInt32(param["CenCos"]); }

                        if (idCenCos != 0)
                        {
                            DataTable ldtTabla = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", Request.Path + "?Nav=CenCosJerarquicoN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", idCenCos, omitirInfoCDR, omitirInfoSiana));

                            if (ldtTabla.Rows.Count > 0 && ldtTabla.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldtTabla);
                                ldtTabla = dvldt.ToTable(false,
                                    new string[] { "Descripcion", "TotalSimulado", "TotalReal", "CostoSimulado", "CostoReal",
                                                    "SM", "Llamadas", "Minutos"});
                                ldtTabla.Columns["Descripcion"].ColumnName = "Centro de costos";
                                ldtTabla.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                ldtTabla.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                ldtTabla.AcceptChanges();
                            }

                            Session["MuestraSM"] = 0;  //Para que nunca muestre el desglose del Servicio Medido

                            EliminarColumnasDeAcuerdoABanderas(ldtTabla);

                            //20161018 NZ: Se Agrega ordenamiento desde codigo para cuando es Banorte.
                            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                            {
                                ldtTabla = DTIChartsAndControls.ordenaTabla(ldtTabla, "Total DESC");
                            }

                            if (ldtTabla.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldtTabla, 0, "Totales"), "Reporte", "Tabla");
                            }

                            //Grafica
                            DataTable ldtGrafica = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico(Request.Path + "?Nav=CenCosJerarquicoN2&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", Request.Path + "?Nav=CenCosJerarquicoN3&omitirInfoCDR=" + omitirInfoCDR.ToString() + "&omitirInfoSiana=" + omitirInfoSiana.ToString() + "&CenCos=", idCenCos, omitirInfoCDR, omitirInfoSiana));

                            if (ldtGrafica.Rows.Count > 0 && ldtGrafica.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldtGrafica);

                                if (Convert.ToInt32(Session["MuestraCostoSimulado"]) == 1)
                                {
                                    ldtGrafica = dvldt.ToTable(false,
                                    new string[] { "Descripcion", "TotalSimulado" });
                                    ldtGrafica.Columns["TotalSimulado"].ColumnName = "Total";
                                    ldtGrafica.Columns["Descripcion"].ColumnName = "Centro de costos";
                                }
                                else
                                {
                                    ldtGrafica = dvldt.ToTable(false,
                                    new string[] { "Descripcion", "TotalReal" });
                                    ldtGrafica.Columns["TotalReal"].ColumnName = "Total";
                                    ldtGrafica.Columns["Descripcion"].ColumnName = "Centro de costos";
                                }

                                ldtGrafica.AcceptChanges();
                            }

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldtGrafica, "Total desc", 10), "Gráfica consumo por centro de costos jerárquico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de costos", "Centro de costos", "", "Total", "$#,0.00",
                                             true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        }

                        EstablecerBanderasClientePerfil(); //regresar a los valores originales a las variables de session.

                        #endregion Reporte Jerarquico V2
                    }
                    else if (param["Nav"] == "RepPrsConsumosPorCampaniaN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo Por Campaña");

                        #region Reporte Consumo por Campaña Prosa

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepPrsConsumoPorCampania());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepConsumoTI = new DataView(ldt);
                            ldt = dvRepConsumoTI.ToTable(false, new string[] { "Campaña", "Importe", "Llamadas", "Minutos" });

                            ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Consumo por Campaña", new string[] { "Importe" }, new string[] { "Importe" }, new string[] { "Total" }, "Campaña", "", "", "Importe", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xl3DColumn, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Consumo por Campaña Prosa
                    }
                    else if (param["Nav"] == "RepTabResumenSpeedDialN1")//NZ 20170719
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Resumen Speed Dials");

                        #region Reporte Resumen Speed Dials

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepTabResumenSpeedDial("''"));
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);

                            ldt = dvldt.ToTable(false,
                            new string[] { "SitioDesc", "Numero Marcado", "Costo", "Llamadas", "Minutos" });

                            ldt.Columns["SitioDesc"].ColumnName = "Sitio";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                            ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC, Cantidad llamadas DESC ");
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Número Marcado", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Número Marcado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total DESC", 10), "Resumen Speed Dials", new string[] { "Total" }, new string[] { "Total" },
                            new string[] { "Total" }, "Número Marcado", "", "", "Importe", "#,0",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Resumen Speed Dials
                    }
                    else if (param["Nav"] == "RepTabTraficoPorHora2Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Trafico por Hora");

                        #region Reporte

                        DataTable original = DSODataAccess.Execute(ConsultaReporteTraficoPorHora());
                        DataTable ldt = original.Clone();
                        ldt.Columns[1].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "SitioDesc", "Hora", "CantLlamadas" });

                            ldt.Columns["SitioDesc"].ColumnName = "Sitios";
                            ldt.Columns["Hora"].ColumnName = "Hora";
                            ldt.Columns["CantLlamadas"].ColumnName = "Cantidad Llamadas";

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Desc"),
                                0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad Llamadas desc", 10),
                             "Trafíco por Hora", new string[] { "value" },
                             new string[] { "Cantidad Llamadas" }, new string[] { "Cantidad Llamadas" },
                             "label", "Hora", "", "Cantidad llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion
                    }
                    else if (param["Nav"] == "RepEstTabCodEnMasDeNExtensiones")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Códigos en más de N extensiones");

                        DataTable original = DSODataAccess.Execute(RepEstTabCodEnMasDeNExtensiones());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Nombre Completo", "No Nomina", "Total", "Llamadas", "Duracion Minutos", "CantCod", "Nombre Centro de Costos", "Nombre Sitio" });

                            ldt.Columns["Codigo Autorizacion"].ColumnName = "Código";
                            ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                            ldt.Columns["No Nomina"].ColumnName = "Nómina";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["CantCod"].ColumnName = "Núm. de Extensiones";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }

                    else if (param["Nav"] == "RepTabCodigosNoAsignados")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Códigos no asignados");

                        DataTable original = DSODataAccess.Execute(RepTabCodigosNoAsignados());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Codigo Autorizacion", "Total", "Numero", "Duracion", "Nombre Sitio" });

                            ldt.Columns["Codigo Autorizacion"].ColumnName = "Código de Autorización";
                            ldt.Columns["Total"].ColumnName = "Importe";
                            ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc"),
                                0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Códigos no asignados", new string[] { "value" },
                             new string[] { "Importe" }, new string[] { "Importe" },
                             "label", "Hora", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabExtensionesNoAsignadas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones no asignadas");

                        DataTable original = DSODataAccess.Execute(RepTabExtensionesNoAsignadas());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Total"].ColumnName = "Importe";
                            ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc"),
                                0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Extensiones no asignadas", new string[] { "value" },
                             new string[] { "Importe" }, new string[] { "Importe" },
                             "label", "Hora", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepEstTabExtAbiertas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones Abiertas");

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
                        DataTable dt = new DataTable();
                        dt = ldt.Copy();

                        if (dt.Columns.Contains("ExtSitio"))
                        {
                            dt.Columns.Remove("ExtSitio");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dt, "Llamadas Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas", "ExtSitio" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Llamadas desc", 10),
                             "Cantidad de Llamadas", new string[] { "Llamadas" },
                             new string[] { "Llamadas" }, new string[] { "Llamadas" },
                             "ExtSitio", "Extensión", "", "Llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabConsPorEmpleado")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consolidado Por Empleado");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleado());

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Centro de Costos", "Nombre Completo", "CostoFija", "CostoMovil", "Total" });
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["CostoFija"].ColumnName = "Consumo Total de Telefonía Fija";
                            ldt.Columns["CostoMovil"].ColumnName = "Consumo Total de Telefonía Móvil";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

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
                            dtGraf.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtGraf, "Total desc", 10),
                             "Consumo Por Categoría De Llamada", new string[] { "Total" },
                             new string[] { "Importe Total" }, new string[] { "Nombre Categoria Tipo Destino" },
                             "Nombre Categoria Tipo Destino", "Categoria", "", "Tipo Destino", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabVariacionRentaLineas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Variacion por Renta Lineas");

                        DataTable ldt = DSODataAccess.Execute(Consulta12MesesVariacionRenta());
                        List<string> nombresColumna = new List<string>();

                        foreach (DataColumn column in ldt.Columns)
                        {
                            nombresColumna.Add(column.ColumnName);
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { nombresColumna[0].ToString(), nombresColumna[1].ToString(), nombresColumna[2].ToString(), nombresColumna[3].ToString(), nombresColumna[4].ToString(), nombresColumna[5].ToString(), nombresColumna[6].ToString(), nombresColumna[7].ToString(), nombresColumna[8].ToString(), nombresColumna[9].ToString(), nombresColumna[10].ToString(), nombresColumna[11].ToString(), nombresColumna[12].ToString(), nombresColumna[13].ToString(), nombresColumna[14].ToString(), nombresColumna[15].ToString(), nombresColumna[16].ToString(), nombresColumna[17].ToString(), nombresColumna[18].ToString(), nombresColumna[19].ToString(), nombresColumna[20].ToString(), nombresColumna[21].ToString(), nombresColumna[22].ToString(), nombresColumna[23].ToString(), nombresColumna[24].ToString(), nombresColumna[25].ToString(), nombresColumna[26], nombresColumna[27].ToString() });
                            
                            //ldt = DTIChartsAndControls.ordenaTabla(ldt, "Lineas Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt,
                                 "Reporte", "Tabla");

                        #endregion Grid

                    }
                    else if (param["Nav"] == "RepTabConsPorEmpleadoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por Tipo Destino");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleadoN2("[link] = ''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN3&Emple=" + param["Emple"] + "&CenCos=" + param["CenCos"] + "&TDest='' + convert(varchar,[Codigo Tipo Destino])'"));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Importe Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Importe Total desc", 10),
                             "Consumo Por Tipo Destino", new string[] { "Importe Total" },
                             new string[] { "Importe Total" }, new string[] { "Tipo Destino" },
                             "Tipo Destino", "Tipo Destino", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabConsPorEmpleadoN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por Tipo Destino");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsPorEmpleadoN3("[link] =''" + Request.Path + "?Nav=RepTabConsPorEmpleadoN4&Emple=" + param["Emple"] + "&Cencos=" + param["CenCos"] + "&NumMarc =''+ Convert(varchar,[Numero Marcado]) + ''&TipoEtiqueta ='' + convert(varchar,[Clave Tipo Llamada]) + ''&TDest ='' + convert(varchar,[Codigo Tipo Destino])'"));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "NumMarcado", "Total", "Numero", "Duracion", "Tipo Llamada" });
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Número Marcado", "Importe Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Número Marcado"].ToString() == "Importe Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Importe Total desc", 10),
                             "Consumo Tabular por Número Marcaddo", new string[] { "Importe Total" },
                             new string[] { "Importe Total" }, new string[] { "Número Marcado" },
                             "Número Marcado", "Número Marcado", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 500, 400);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabConsPobmasMindeConvN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por Empleado");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsPobmasMindeConvN2("[link] = ''" + Request.Path + "?Nav=RepTabConsPobmasMindeConvN3&Emple=''+ Convert(varchar,[Codigo Empleado]) + ''&Locali=''+Convert(varchar,[Codigo Localidad])'"));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "TotImporte", "LLamadas", "TotMin" });
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["TotImporte"].ColumnName = "Total de Importe";
                            ldt.Columns["LLamadas"].ColumnName = "Total de Llamadas";
                            ldt.Columns["TotMin"].ColumnName = "Total de Minutos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Importe Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total de Importe Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Total de Importe" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total de Importe desc", 10),
                             "Consumo Por Empleado", new string[] { "Total de Importe" },
                             new string[] { "Total de Importe" }, new string[] { "Empleado" },
                             "Empleado", "Empleado", "", "Total de Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabConsPobmasMindeConvN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por Empleado");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsPobmasMindeConvN3("[link] = ''" + Request.Path + "?Nav=RepTabConsPobmasMindeConvN3&Emple=''+ Convert(varchar,[Codigo Empleado]) + ''&Locali=''+Convert(varchar,[Codigo Localidad]) +''&TipoEtiqueta=''+Convert(varchar,[Clave Tipo Llamada])'"));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo Llamada", "TotImporte", "TotLlam", "TotMin" });
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt.Columns["TotImporte"].ColumnName = "Total";
                            ldt.Columns["TotLlam"].ColumnName = " Llamadas";
                            ldt.Columns["TotMin"].ColumnName = "Minutos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo de Llamada", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Llamada"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10),
                         "Consumo Por Tipo Llamada", new string[] { "Total" },
                         new string[] { "Total" }, new string[] { "Tipo de Llamada" },
                         "Tipo de Llamada", "Tipo de Llamada", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                         lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsEmpmasLlamN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Empleados");


                        DataTable ldt = DSODataAccess.Execute(ConsEmpmasLlamN2(""));


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion" });

                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";


                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe total Desc");
                            ldt.AcceptChanges();
                        }

                        if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                        {
                            campoAGraficar = "Importe Total";
                            numberFormat = "$#,0.00";

                        }
                        else
                        {
                            ldt.Columns.Remove("Importe Total");
                            campoAGraficar = "Cantidad llamadas";
                            numberFormat = "#,0";
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", campoAGraficar });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Tipo Destino"].ColumnName = "label";
                            ldt.Columns[campoAGraficar].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Top Destinos/ " + param["Emple"] + "", new string[] { "value" },
                             new string[] { campoAGraficar }, new string[] { campoAGraficar },
                             "label", "Tipo Destino", "", campoAGraficar, numberFormat, true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsEmpmasLlamN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top colaboradores con más llamadas");


                        DataTable ldt = DSODataAccess.Execute(ConsEmpmasLlamN2(""));


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Codigo Tipo Destino", "Total", "Numero", "Duracion" });

                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";


                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Importe Total" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Tipo Destino"].ColumnName = "label";
                            ldt.Columns["Importe Total"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Top Destinos/ " + param["Emple"] + "", new string[] { "value" },
                             new string[] { "Importe Total" }, new string[] { "Importe Total" },
                             "label", "Tipo Destino", "", "Importe Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsNumerosMasMarcadasN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Agrupado por Empleado ");


                        DataTable ldt = DSODataAccess.Execute(ConsNumerosMasMarcadasN2(""));
                        DataTable ldtGraf = DSODataAccess.Execute(ConsNumerosMasMarcadasN2Graf());
                        string[] cols = new string[] { };
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            if (DSODataContext.Schema.ToUpper() == "FCA")
                            {
                                cols = new string[] { "Nombre Completo", "Nombre Sitio", "Total", "Numero", "Duracion" };
                            }
                            else
                            {
                                cols = new string[] { "Nombre Completo", "Centro de Costos", "Nombre Sitio", "Total", "Numero", "Duracion"};
                                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                            }

                            ldt = dvldt.ToTable(false, cols);
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Nombre del Sitio";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total Minutos";
                            ldt.AcceptChanges();
                        }

                        if (ldtGraf.Rows.Count > 0 && ldtGraf.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldtGraf);
                            ldtGraf = dvldt.ToTable(false, new string[] { "Nombre Sitio", "Codigo Sitio", "Total" });
                            ldtGraf.Columns["Nombre Sitio"].ColumnName = "label";
                            ldtGraf.Columns["Total"].ColumnName = "value";
                            ldtGraf.AcceptChanges();
                        }

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldtGraf, "value desc", 10),
                             "Consumo Tabular Empleado", new string[] { "value" },
                             new string[] { "Importe Total" }, new string[] { "Importe Total" },
                             "label", "Nombre del Sitio", "", "Importe Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsLocalidMasMarcadasN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado");

                        DataTable ldt = DSODataAccess.Execute(ConsLocalidMasMarcadasN2());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "TotImporte", "LLamadas", "TotMin" });

                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["TotImporte"].ColumnName = "Total Importe";
                            ldt.Columns["LLamadas"].ColumnName = "Total de Llamadas";
                            ldt.Columns["TotMin"].ColumnName = "Total de Minutos";
                            ldt.AcceptChanges();
                        }


                        #region Grid


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");



                        #endregion Grid

                        #region Grafica


                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Total Importe" });


                            ldt.Columns["Empleado"].ColumnName = "label";
                            ldt.Columns["Total Importe"].ColumnName = "value";

                            if (ldt.Rows[ldt.Rows.Count - 1]["label"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Consumo Tabular Empleado", new string[] { "value" },
                             new string[] { "Empleado" }, new string[] { "Importe Total" },
                             "label", "Empleado", "", "Importe Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsumoEmpsMasTiempo")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado");


                        DataTable ldt = DSODataAccess.Execute(ConsumoEmpsMasTiempo(string.Empty));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Nombre Completo", "Duracion" });

                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Minutos Desc");
                            ldt.AcceptChanges();
                        }

                        #region Grid


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");



                        #endregion Grid

                        #region Grafica


                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Total de Minutos" });


                            ldt.Columns["Colaborador"].ColumnName = "label";
                            ldt.Columns["Total de Minutos"].ColumnName = "value";

                            if (ldt.Rows[ldt.Rows.Count - 1]["label"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Empleados con más tiempo al tel.", new string[] { "value" },
                             new string[] { "Colaborador" }, new string[] { "Total de Minutos" },
                             "label", "Colaborador", "", "Total de Minutos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                    }
                    else if (param["Nav"] == "ConsLugmasCost")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Localidades con mayor consumo");

                        DataTable dtReporte = DSODataAccess.Execute(ConsLugmasCost(1));
                        DataTable ldt = dtReporte.Copy();

                        #region Grid
                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false,
                                new string[] { "Nombre Localidad", "Costo", "Duracion", "TotLlam", });
                            dtReporte.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            dtReporte.Columns["Costo"].ColumnName = "Total Importe";
                            dtReporte.Columns["Duracion"].ColumnName = "Minutos";
                            dtReporte.Columns["TotLlam"].ColumnName = "Llamadas";

                            dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Importe Desc");
                            dtReporte.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtReporte, 0, "Totales"), "Reporte", "Tabla");

                        #endregion

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Costo" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Nombre Localidad"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Nombre Localidad"].ColumnName = "label";
                            ldt.Columns["Costo"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Localidades con mayor consumo", new string[] { "value" },
                             new string[] { "Nombre Localidad" }, new string[] { "Total" },
                             "label", "Localidad", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsLugmasCostN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Localidades con mayor consumo");

                        DataTable dtReporte = DSODataAccess.Execute(ConsLugmasCostN2(string.Empty));
                        DataTable ldt = dtReporte.Copy();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Sitio", "Total", "Numero", "Duracion" });
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Cantidad minutos";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total DESC");

                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Consumo por sitio", new string[] { "value" },
                             new string[] { "Sitio" }, new string[] { "Total" },
                             "label", "Sitio", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }

                    else if (param["Nav"] == "RepTabLlamadasEntreSitios")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas de Entrada por Sitio");

                        DataTable ldt = DSODataAccess.Execute(RepObtieneLlamEntradasEnlace(""));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Descripcion", "Llamadas" });
                            ldt.Columns["Descripcion"].ColumnName = "Sitio";
                            ldt.Columns["Llamadas"].ColumnName = "Llamadas de entrada";

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc");
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(ldt);
                            ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Sitio", "Llamadas de entrada" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Llamadas de entrada desc", 10),
                             "Llamadas de Entrada", new string[] { "Llamadas de entrada" },
                             new string[] { "Llamadas de entrada" }, new string[] { "Sitio" },
                             "Sitio", "Sitios", "", "Llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabLlamadasEntreSitiosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas de Entrada por Sitio");

                        DataTable ldt = DSODataAccess.Execute(RepTabLlamadasEntreSitiosN2(""));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "vchDescripcion", "TotalLlam" });
                            ldt.Columns["vchDescripcion"].ColumnName = "Sitio";
                            ldt.Columns["TotalLlam"].ColumnName = "Llamadas de entrada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc");
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Llamadas de entrada Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(ldt);
                            ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Sitio", "Llamadas de entrada" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Llamadas de entrada desc", 10),
                             "Llamadas de Entrada", new string[] { "Llamadas de entrada" },
                             new string[] { "Llamadas de entrada" }, new string[] { "Sitio" },
                             "Sitio", "Sitios", "", "Llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabLlamadasEntreSitiosN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas de Entrada por Extensión");

                        DataTable ldt = DSODataAccess.Execute(RepTabLlamadasEntreSitiosN3(""));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Teldest", "TotalLlam" });
                            ldt.Columns["Teldest"].ColumnName = "Extensión Destino";
                            ldt.Columns["TotalLlam"].ColumnName = "Llamadas Recibidas";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Recibidas Desc");
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Recibidas Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(ldt);
                            ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Extensión Destino", "Llamadas Recibidas" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Extensión Destino"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Llamadas Recibidas desc", 10),
                             "Llamadas de Entrada por Extensión", new string[] { "Llamadas Recibidas" },
                             new string[] { "Llamadas Recibidas" }, new string[] { "Extensión Destino" },
                             "Extensión Destino", "Extensión", "", "Llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepPorSucursalv2N1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por sucursal");

                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorSucursalBimbo(""));

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Sitio", "Importe Telmex", "Total Llams Telmex", "Importe Axtel", "Total Llams Axtel", "Total Gral" });
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt.Columns["Total Llams Telmex"].ColumnName = "Llamadas Telmex";
                            ldt.Columns["Total Llams Axtel"].ColumnName = "Llamadas Axtel";

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Gral Desc");
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Gral Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(ldt);
                            ldt = dvRepMinsPorEtiq.ToTable(false, new string[] { "Sitio", "Total Gral" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total Gral desc", 10),
                             "Consumo por sucursal", new string[] { "Total Gral" },
                             new string[] { "Total Gral" }, new string[] { "Sitio" },
                             "Sitio", "", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepPorSucursalv2N2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por linea");

                        DataTable dtReporte = DSODataAccess.Execute(ConsultaConsumoPorSucursalBimboN2(""));

                        #region Grid

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false, new string[] { "Telefono", "Total Llams Telmex", "Importe Telmex", "Importe Axtel", "Total Llams Axtel", "Total Gral" });
                            dtReporte.Columns["Total Llams Telmex"].ColumnName = "Llamadas Telmex";
                            dtReporte.Columns["Total Llams Axtel"].ColumnName = "Llamadas Axtel";

                            dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Total Gral Desc");
                            dtReporte.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtReporte, "Total Gral Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #region Grafica

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(dtReporte);
                            dtReporte = dvRepMinsPorEtiq.ToTable(false, new string[] { "Telefono", "Total Gral" });

                            if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Telefono"].ToString() == "Total")
                            {
                                dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                            }
                            dtReporte.AcceptChanges();
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtReporte, "Total Gral desc", 10),
                             "Consumo por telefono", new string[] { "Total Gral" },
                             new string[] { "Total Gral" }, new string[] { "Telefono" },
                             "Telefono", "", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepIndConcentracionGastoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Empleados que concentran el 15% del gasto total");

                        #region Grid

                        DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndConcentracionGasto2Nvl());

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false, new string[] { "Colaborador", "Centro de Costos", "Llamadas", "Duracion", "Importe" });
                            dtReporte.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                            dtReporte.Columns["Duracion"].ColumnName = "Cantidad minutos";
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtReporte, 0, "Total"), "Reporte", "Tabla");
                        }

                        #endregion Grid

                        #region Grafica

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(dtReporte);
                            dtReporte = dvRepMinsPorEtiq.ToTable(false, new string[] { "Colaborador", "Importe" });

                            if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Colaborador"].ToString() == "Total")
                            {
                                dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                            }
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtReporte, "Importe desc", 10),
                                 "Empleados que concentran el 15% del gasto total", new string[] { "Importe" },
                                 new string[] { "Colaborador" }, new string[] { "Importe" },
                                 "Colaborador", "Colaborador", "", "Importe", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                                 lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepIndCodAutoNuevosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Códigos nuevos");

                        #region Grid
                        DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndCodAutoNuevos2Nvl());

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false, new string[] { "Sitio", "Asignados", "No Asignados", "Importe" });
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtReporte, 0, "Total"), "Reporte", "Tabla");
                        }
                        #endregion

                        #region Grafica

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(dtReporte);
                            dtReporte = dvRepMinsPorEtiq.ToTable(false, new string[] { "Sitio", "Importe" });

                            if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Sitio"].ToString() == "Total")
                            {
                                dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                            }
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtReporte, "Importe desc", 10),
                                 "Códigos Nuevos", new string[] { "Importe" },
                                 new string[] { "Sitio" }, new string[] { "Importe" },
                                 "Sitio", "Sitio", "", "Importe", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                                 lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepIndExtenNuevasN2")
                    {

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones nuevas");

                        #region Grid
                        DataTable dtReporte = DSODataAccess.Execute(ConsultaRepIndExtenNuevas2Nvl());

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false, new string[] { "Extensión", "Sitio", "Asignadas", "No Asignadas", "Importe" });
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtReporte, 0, "Total"), "Reporte", "Tabla");
                        }
                        #endregion

                        #region Grafica

                        if (dtReporte.Rows.Count > 0 && dtReporte.Columns.Count > 0)
                        {
                            DataView dvRepMinsPorEtiq = new DataView(dtReporte);
                            dtReporte = dvRepMinsPorEtiq.ToTable(false, new string[] { "Extensión", "Importe" });

                            if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Extensión"].ToString() == "Total")
                            {
                                dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                            }
                            dtReporte.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtReporte, "Importe desc", 10),
                                 "Extensiones Nuevas", new string[] { "Importe" },
                                 new string[] { "Extensión" }, new string[] { "Importe" },
                                 "Extensión", "Extensión", "", "Importe", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                                 lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConRepTabConsHistAnioActualVsAnterior2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Año actual vs año anterior");

                        #region Año Actual vs Anterior

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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtReporte, 0, "Totales"), "Reporte", "Tabla");

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
                            List<string> listaSeriesSinMeses = new List<string>();

                            foreach (string serie in series)
                            {
                                if (serie != "Mes")
                                {
                                    listaSeriesSinMeses.Add(serie);
                                }
                            }
                            //20180718 Grafica de stack NZ

                            ExportacionExcelRep.CrearGrafico(dtOriginal,
                                     "", new string[] { "Total Año Anterior", "Total Año Actual" },
                                     listaSeriesSinMeses.ToArray(), listaSeriesSinMeses.ToArray(),
                                     "Mes", "Mes", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked,
                                     lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }

                        #endregion

                        #endregion

                    }
                    else if (param["Nav"] == "RepTabIndicadorCodigosNoAsignadosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Codigos por Identificar");
                        DataTable original = DSODataAccess.Execute(RepIndicadorCodigosNoAsignados());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Total"].ColumnName = "Importe";
                            ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc"),
                                0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Extensiones no asignadas", new string[] { "value" },
                             new string[] { "Importe" }, new string[] { "Importe" },
                             "label", "Hora", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabIndicadorExtensionesNoAsignadasN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones por Identificar");

                        DataTable original = DSODataAccess.Execute(RepIndicadorExtensionesNoAsignadas());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();


                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Total", "Numero", "Duracion", "Nombre Sitio" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Total"].ColumnName = "Importe";
                            ldt.Columns["Numero"].ColumnName = "Total de llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Desc"),
                                0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Extensiones no asignadas", new string[] { "value" },
                             new string[] { "Importe" }, new string[] { "Importe" },
                             "label", "Hora", "", "Importe", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepLineasExcedentesTelcel2pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Líneas Telcel que Exceden la Renta");
                        DataTable original = DSODataAccess.Execute(ConsultaExcedenteLineasTelcel(""));
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Linea", "Carrier", "Nombre Completo", "Nombre Centro de Costos", "Renta", "Excedente", "Total" });

                            ldt.Columns["Linea"].ColumnName = "Línea";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"),
                                0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Línea", "Total" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Línea"].ColumnName = "label";
                            ldt.Columns["Total"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Líneas que Exceden Renta", new string[] { "value" },
                             new string[] { "Total" }, new string[] { "Total" },
                             "label", "Línea", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "PorConceptoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desglose de Concepto");
                        DataTable original = DSODataAccess.Execute(ConsultaRepPorCategoria(""));
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Concepto", "Total" });

                            ldt.Columns["Concepto"].ColumnName = "Concepto";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Concepto", "Total" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Concepto"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Concepto"].ColumnName = "label";
                            ldt.Columns["Total"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Desgloce por Concepto", new string[] { "value" },
                             new string[] { "Total" }, new string[] { "Total" },
                             "label", "Concepto", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepMinPorCarrier")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Minutos por Carrier");
                        DataTable dt = new DataTable();
                        dt = DSODataAccess.Execute(ConsultaRepMinutosPorCarrier("[link] = ''''"));


                        if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                        {
                            /*Elimina Col innecesarias*/


                            if (dt.Columns.Contains("Codigo Carrier"))
                            {
                                dt.Columns.Remove("Codigo Carrier");
                            }

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
                            dt = dv.ToTable(false, new string[] { "Nombre Carrier", "Duracion", "Total", "Numero" });
                            dtGraf = dv.ToTable(false, new string[] { "Nombre Carrier", "Duracion" });

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


                            ExportacionExcelRep.CreaTablaEnExcel(lExcel,
                            DTIChartsAndControls.agregaTotales(
                                    DTIChartsAndControls.ordenaTabla(dt, "Total Desc"),
                                                                    0,
                                                                    "Total"),
                                                                "Reporte",
                                                                "Tabla"
                                                                );


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

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtGraf, "value desc", 10),
                             "Minutos por Carrier", new string[] { "value" },
                             new string[] { "Total" }, new string[] { "Total" },
                             "label", "Minutos", "", "Carrier", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                    }
                    else if (param["Nav"] == "RepLlamPerdidas2pnl")
                    {
                        /**/
                        int exportDetall = 1;
                        string tituloReporte = "";
                        if (Session["ExporDetall"] != null)
                        {
                            exportDetall = Convert.ToInt32(Session["ExporDetall"]);
                        }

                        tituloReporte = "Detalle de Llamadas Perdidas";
                        if (exportDetall == 2 && LlamPerd.Visible == true)
                        {
                            param["Emple"] = "0";
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, tituloReporte);
                            DataTable dt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadasN3(exportDetall));
                            #region Grid1
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
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                            }



                            #endregion
                        }

                        /**/

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Total de Llamadas Perdidas por Sitio");
                        DataTable original = DSODataAccess.Execute(ConsultaReporteLlamNoContestadas());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "SitioDesc", "Total" });

                            ldt.Columns["SitioDesc"].ColumnName = "Sitio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Sitio", "Total" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Sitio"].ColumnName = "label";
                            ldt.Columns["Total"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Total de Llamadas Perdidas por Sitio", new string[] { "value" },
                             new string[] { "Total" }, new string[] { "Total" },
                             "label", "Sitio", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepLlamPerdidasN22Pnl")
                    {
                        /**/
                        int exportDetall = 1;
                        string tituloReporte = "";
                        if (Session["ExporDetall"] != null)
                        {
                            exportDetall = Convert.ToInt32(Session["ExporDetall"]);
                        }

                        tituloReporte = "Detalle de Llamadas Perdidas";
                        if (exportDetall == 2 && LlamPerd.Visible == true)
                        {
                            param["Emple"] = "0";
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, tituloReporte);
                            DataTable dt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadasN3(exportDetall));
                            #region Grid1
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
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                            }



                            #endregion
                        }

                        /**/
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Total de Llamadas Perdidas por Empleado");
                        DataTable original = DSODataAccess.Execute(ConsutaReporteLlamNoContestadasN2());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "NomCompleto", "Descripcion", "Total" });
                            ldt.Columns["NomCompleto"].ColumnName = "Colaborador";
                            ldt.Columns["Descripcion"].ColumnName = "Centro de Costos";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Colaborador", "Centro de Costos", "Total" });

                            if (ldt.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Total")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Colaborador"].ColumnName = "label";
                            ldt.Columns["Total"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Total de Llamadas Perdidas por Empleado", new string[] { "value" },
                             new string[] { "Total" }, new string[] { "Total" },
                             "label", "Colaborador", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabSeeYouOnUtilCliente2pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Utilización Sistemas Cliente");
                        DataTable original = DSODataAccess.Execute(ConsultaUtilSistemasCliente());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Client", "Cliente", "Utilizacion", "Cantidad" });
                            ldt.Columns["Cliente"].ColumnName = "Cliente";
                            ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                            ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Número de Eventos Asc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Número de Eventos Desc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        DataTable ldt1 = DSODataAccess.Execute(ConsultaUtilSistemasCliente());
                        if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt1);
                            ldt1 = dvldt.ToTable(false, new string[] { "Cliente", "UtilizacionGraf" });
                            if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                            {
                                ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                            }

                            ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "UtilizacionGraf Desc");
                            ldt1.AcceptChanges();
                            ldt1.Columns["Cliente"].ColumnName = "label";
                            ldt1.Columns["UtilizacionGraf"].ColumnName = "value";

                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10),
                             "Utilización Sistemas Cliente", new string[] { "value" },
                             new string[] { "UtilizacionGraf" }, new string[] { "UtilizacionGraf" },
                             "label", "Cliente", "", "Utilizacion", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabSeeYouOnUtilSistema2pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Utilización Sistema");
                        DataTable original = DSODataAccess.Execute(ConsultaUtilSistema());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "Utilizacion", "Cantidad" });
                            ldt.Columns["Cliente"].ColumnName = "Cliente";
                            ldt.Columns["Sistema"].ColumnName = "Sistema";
                            ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                            ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Cliente Asc"),
                                0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        DataTable ldt1 = ldt;
                        if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt1);
                            ldt1 = dvldt.ToTable(false, new string[] { "Cliente", "Sistema", "Número de Eventos" });
                            if (ldt1.Rows[ldt1.Rows.Count - 1]["Cliente"].ToString() == "Totales")
                            {
                                ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                            }
                            ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "Número de Eventos Desc");
                            ldt1.Columns["Sistema"].ColumnName = "label";
                            ldt1.Columns["Número de Eventos"].ColumnName = "value";
                            ldt1.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10),
                             "Utilización Sistemas", new string[] { "value" },
                             new string[] { "Número de Eventos" }, new string[] { "Número de Eventos" },
                             "label", "Sistema", "", "Número de Eventos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabSeeYouOnUtilSistemaHist2pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Histórico de Utilización de Sistema");
                        DataTable original = DSODataAccess.Execute(ConsultaUtilSistemaHistorico());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Mes", "Utilizacion" });
                            ldt.Columns["Nombre Mes"].ColumnName = "Mes";
                            ldt.Columns["Utilizacion"].ColumnName = "Utilización";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

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
                            ldt.AcceptChanges();

                            ldt.Columns["Mes"].ColumnName = "label";
                            ldt.Columns["Utilización"].ColumnName = "value";

                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                             "Histórico de Utilización de Sistema", new string[] { "value" },
                             new string[] { "Utilización" }, new string[] { "Utilización" },
                             "label", "Mes", "", "NUtilización", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepTabSeeYouOnCencosHoras2Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Horas Hombre por Centro de Costos");
                        DataTable original = DSODataAccess.Execute(ConsultaCencosHorasHombre());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        DataTable ldt1 = original.Clone();
                        ldt1.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }
                        foreach (DataRow row in original.Rows)
                        {
                            ldt1.ImportRow(row);
                        }
                        original.Clear();
                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Cencosdesc", "TiempoTotal", "Cantidad", "Promedio" });
                            ldt.Columns["Cencosdesc"].ColumnName = "Centro de Costos";
                            ldt.Columns["TiempoTotal"].ColumnName = "Utilización";
                            ldt.Columns["Cantidad"].ColumnName = "Número de Eventos";
                            ldt.Columns["Promedio"].ColumnName = "Tiempo Promedio";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "TiempoTotalOrig Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                         DTIChartsAndControls.ordenaTabla(ldt, "Número de Eventos Desc"),
                         0, "Total"), "Reporte", "Tabla");

                        #endregion Grid
                        #region Grafica

                        if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
                        {
                            DataView dvldt1 = new DataView(ldt1);
                            ldt1 = dvldt1.ToTable(false, new string[] { "Cencosdesc", "TiempoTotalOrig" });
                            if (ldt1.Rows[ldt1.Rows.Count - 1]["Cencosdesc"].ToString() == "Totales")
                            {
                                ldt1.Rows[ldt1.Rows.Count - 1].Delete();
                            }
                            ldt1 = DTIChartsAndControls.ordenaTabla(ldt1, "TiempoTotalOrig Desc");
                            ldt1.AcceptChanges();
                            ldt1.Columns["Cencosdesc"].ColumnName = "label";
                            ldt1.Columns["TiempoTotalOrig"].ColumnName = "value";

                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt1, "value desc", 10),
                             "Horas Hombre por Centro de Costos", new string[] { "value" },
                             new string[] { "TiempoTotalOrig" }, new string[] { "TiempoTotalOrig" },
                             "label", "Centro de Costos", "", "Total", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                             lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "RepBolsaConsumoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo de Bolsas");
                        DataTable ldt1 = DSODataAccess.Execute(ConsultaConsumoBolsaDiaria());
                        if (ldt1.Rows.Count > 0 && ldt1.Columns.Count > 0)
                        {
                            DataView dvRepConsumoDiario = new DataView(ldt1);
                            ldt1 = dvRepConsumoDiario.ToTable(false,
                                new string[] { "Fecha", "Llamadas Locales", "Llamadas a Celular", "Minutos USA", "Minutos 800 Nacional" });

                            ldt1.AcceptChanges();
                        }
                        ExportacionExcelRep.CrearGrafico(ldt1, "Consumo de Bolsa Diario", new string[] { "Llamadas Locales", "Llamadas a Celular", "Minutos USA", "Minutos 800 Nacional" },
                                        new string[] { "Llamadas Locales", "Llamadas a Celular", "Minutos USA", "Minutos 800 Nacional" }, new string[] { "Llamadas Locales", "Llamadas a Celular", "Minutos USA", "Minutos 800 Nacional" }, "Fecha", "Dia", "", "Bolsa Consumida", "{0:0,0}",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 200);






                        DataTable original = DSODataAccess.Execute(ConsultaConsumoBolsa());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }
                        original.Clear();

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Unidad", "cantidad", "Unidad de Cobro", "Bolsa Consumida", "% de Consumo", "Bolsa Disponible", "%  de Consumo Disponible", "Fuera de Bolsa", "% Excedente", "Tarifa" });
                            ldt.AcceptChanges();
                        }
                        #region Grid
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        #endregion Grid


                    }
                    #endregion Reporte por CenCos
                    /*Reportes DESVIOS*/
                    else if (param["Nav"] == "RepDesviosTipoDestinoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por tipo de Destino");
                        DataTable original = DSODataAccess.Execute(RepDesviosTipoDestino());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo de Destino", "Gasto Total", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto Total Desc"), 0, "Totales"), "Reporte", "Tabla");
                        //ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo de Destino", "Gasto de Desvíos" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo de Destino"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Tipo de Destino"].ColumnName = "label";
                            ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                     "Desvíos por tipo de Destino", new string[] { "value" },
                     new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvíos" },
                     "label", "Tipo de Destino", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                     lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                    }
                    else if (param["Nav"] == "RepDesviosTipoDestinoN2" || param["Nav"] == "RepDesviosSitioN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por Empleado");

                        DataTable original = DSODataAccess.Execute(RepDesviosTipoDestinoN2(""));
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto Total Desc"), 0, "Totales"), "Reporte", "Tabla");
                        //ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Empleado"].ColumnName = "label";
                            ldt.Columns["Gasto de Desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                     "Desvíos por Empleado", new string[] { "value" },
                     new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvios" },
                     "label", "Empleado", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                     lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosPorExtensionN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por Extensión");
                        DataTable original = DSODataAccess.Execute(RepDesviosTipoDestinoN2(""));
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Empleado", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc"), 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos" });
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Desvíos por Extensión", new string[] { "value" },
                        new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvíos" },
                        "label", "Empleado", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosCenCostoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por Centro de Costos");
                        DataTable original = DSODataAccess.Execute(RepDesviosPorCencos());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc"), 0, "Totales"), "Reporte", "Tabla");
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Gasto de Desvíos" });
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Desvíos por Extensión", new string[] { "value" },
                        new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvíos" },
                        "label", "Centro de Costos", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosCenCostoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por Centro de Costos");
                        DataTable original = DSODataAccess.Execute(RepDesviosPorCencos());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Centro de Costos", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc"), 0, "Totales"), "Reporte", "Tabla");
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Gasto de Desvíos" });
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Desvíos por Extensión", new string[] { "value" },
                        new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvíos" },
                        "label", "Empleado", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosSitioN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desvíos por Sitio");
                        DataTable original = DSODataAccess.Execute(RepDesviosPorCencos());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);
                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Sitio", "Gasto de Desvíos", "Minutos Desvíos", "Llamadas Desvíos", "Llamadas no contestadas", "Porcentaje no contestadas" });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             DTIChartsAndControls.ordenaTabla(ldt, "Gasto de Desvíos Desc"), 0, "Totales"), "Reporte", "Tabla");
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Sitio", "Gasto de Desvíos" });
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Desvíos por Sitio", new string[] { "value" },
                        new string[] { "Gasto de Desvíos" }, new string[] { "Gasto de Desvíos" },
                        "label", "Sitio", "", "Gasto de Desvíos", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "HistoricoLlamadasN1")
                    {
                        DataTable GrafConsHist = new DataTable();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte historico");

                        #region Reporte historico


                        GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistorico("", omitirInfoCDR, omitirInfoSiana, 1));



                        if (GrafConsHist.Rows.Count > 0 && GrafConsHist.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["Total"].ColumnName = "Total";
                            GrafConsHist = DTIChartsAndControls.ordenaTabla(GrafConsHist, "Orden asc");
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo historico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "0",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        if (GrafConsHist.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "HistoricoLlamadasN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costos");

                        #region Reporte por CenCos

                        DataTable RepConsCenCos = DSODataAccess.Execute(ConsultaPorCenCos(""));
                        if (RepConsCenCos.Rows.Count > 0 && RepConsCenCos.Columns.Count > 0)
                        {
                            DataView dvRepConsCenCos = new DataView(RepConsCenCos);
                            RepConsCenCos = dvRepConsCenCos.ToTable(false,
                                new string[] { "Centro de Costos", "Llamadas", "Minutos" });
                            RepConsCenCos.Columns["Llamadas"].ColumnName = "Cantidad llamadas";   //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            RepConsCenCos.Columns["Minutos"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepConsCenCos.AcceptChanges();
                        }

                        if (RepConsCenCos.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsCenCos, 0, "Totales"), "Reporte", "Tabla");
                        }

                        if (RepConsCenCos.Rows.Count > 0 && RepConsCenCos.Columns.Count > 0)
                        {
                            DataView dvRepConsCenCos = new DataView(RepConsCenCos);
                            RepConsCenCos = dvRepConsCenCos.ToTable(false,
                                new string[] { "Centro de Costos", "Cantidad llamadas" });
                            if (RepConsCenCos.Rows[RepConsCenCos.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                            {
                                RepConsCenCos.Rows[RepConsCenCos.Rows.Count - 1].Delete();
                            }
                            RepConsCenCos.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsCenCos, "Cantidad llamadas desc", 10), "Consumo por centro de costos", new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" }, "Centro de Costos", "", "", "Cantidad llamadas", "0",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                    }
                    else if (param["Nav"] == "HistoricoLlamadasN3")
                    {
                        DataTable RepConsEmpleMasCaros = new DataTable();
                        string nombreColumnaColaborador = "Colaborador";

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por " + nombreColumnaColaborador);

                        #region Reporte por empleado


                        RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaReporteTopNEmpleDashboard("", "", 10));
                        RepConsEmpleMasCaros = DTIChartsAndControls.ordenaTabla(RepConsEmpleMasCaros, "Total DESC");



                        if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                        {
                            DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);

                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                            new string[] { "Nombre Completo", "Nombre Centro de Costos", "Numero", "Duracion" });

                        }


                        RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
                        RepConsEmpleMasCaros.Columns["Nombre Centro de Costos"].ColumnName = "Centro de costos";
                        RepConsEmpleMasCaros.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                        RepConsEmpleMasCaros.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                        RepConsEmpleMasCaros.AcceptChanges();






                        if (RepConsEmpleMasCaros.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConsEmpleMasCaros, 0, "Totales"), "Reporte", "Tabla");
                        }

                        if (RepConsEmpleMasCaros.Rows.Count > 0 && RepConsEmpleMasCaros.Columns.Count > 0)
                        {
                            DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                            RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                new string[] { nombreColumnaColaborador, "Cantidad llamadas" });
                            if (RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1][nombreColumnaColaborador].ToString() == "Totales")
                            {
                                RepConsEmpleMasCaros.Rows[RepConsEmpleMasCaros.Rows.Count - 1].Delete();
                            }
                            RepConsEmpleMasCaros.AcceptChanges();
                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(RepConsEmpleMasCaros, "Cantidad llamadas desc", 10), "Consumo por " + nombreColumnaColaborador, new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" }, nombreColumnaColaborador, "", "", "Cantidad llamadas", "0",
                                        true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte por empleado
                    }
                    else if (param["Nav"] == "HistoricoLlamadasN4")
                    {
                        DataTable GrafConsPorTDest = new DataTable();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo destino");

                        #region Reporte por tipo destino

                        GrafConsPorTDest = DSODataAccess.Execute(ConsultaReportePorTDestDashboard(string.Empty, string.Empty));



                        if (GrafConsPorTDest.Rows.Count > 0 && GrafConsPorTDest.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsPorTDest);


                            GrafConsPorTDest = dvGrafConsHist.ToTable(false, new string[] { "Nombre Tipo Destino", "Numero", "Duracion" });



                            GrafConsPorTDest.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            GrafConsPorTDest.Columns["Numero"].ColumnName = "Cantidad llamadas";     //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            GrafConsPorTDest.Columns["Duracion"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            GrafConsPorTDest = DTIChartsAndControls.ordenaTabla(GrafConsPorTDest, "Cantidad llamadas DESC");

                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(GrafConsPorTDest, "Cantidad llamadas desc", 10), "Consumo por tipo destino", new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                            new string[] { "Cantidad llamadas" }, "Tipo Destino", "", "", "Cantidad llamadas", "0",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 550, 300);

                        if (GrafConsPorTDest.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsPorTDest, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por tipo destino
                    }
                    else if (param["Nav"] == "RepLlamadasPorDispositivoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de llamadas por dispositivo");
                        DataTable ldt = DSODataAccess.Execute(RepLlamadasPorDispositivo());
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView ldtv = new DataView(ldt);

                            ldt = ldtv.ToTable(false, new string[] { "TipoDispositivo", "CantidadLlamadad", "CantidadMinutos" });

                            ldt.Columns["TipoDispositivo"].ColumnName = "Tipo dispositivo";
                            ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                            ldt.Columns["CantidadMinutos"].ColumnName = "Cantidad minutos";
                            ldt.AcceptChanges();

                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo dispositivo", "Cantidad llamdas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo dispositivo"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo dispositivo"].ToString() == "")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Tipo dispositivo"].ColumnName = "label";
                            ldt.Columns["Cantidad llamdas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Llamadas por dispositivo", new string[] { "value" },
                        new string[] { "Cantidad llamdas" }, new string[] { "Cantidad llamdas" },
                        "label", "Tipo dispositivo", "", "Cantidad llamdas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlPie,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPorDispositivoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de llamadas por dispositivo");

                        DataTable ldt = DSODataAccess.Execute(RepLlamadasPorDispositivoExt());
                        ldt.Columns["Extension"].ColumnName = "Extensión";
                        ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                        ldt.Columns["CantidadMinutos"].ColumnName = "Cantidad minutos";
                        ldt.AcceptChanges();


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            if (ldt.Rows[ldt.Rows.Count - 1]["Extensión"].ToString() == "")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Extensión"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Llamadas por extensión", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Extensión", "", "Cantidad llamadas", "##", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }

                    if (param["Nav"] == "RepTraficoExtensiones1pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Tráfico de Extensiones");
                        DataTable ldt = DSODataAccess.Execute(RepTraficoExtensiones());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                        DataTable ldt1 = DSODataAccess.Execute(RepExtensionessinActVSBD());
                        if (ldt1.Rows.Count > 0)
                        {
                            ldt1.Columns["EXTENSION"].ColumnName = "EXTENSION SIN ACTIVIDAD";
                            ldt1.AcceptChanges();
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt1, "Reporte", "{Grafica}");
                        }
                    }
                    else if (param["Nav"] == "RepDesviosPorHoraN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de desvíos por hora");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosPorHora());
                        ldt.Columns.Remove("Hora");
                        ldt.AcceptChanges();

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Horario", "Gasto de desvíos" });
                            ldt.Columns["Horario"].ColumnName = "label";
                            ldt.Columns["Gasto de desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Desvíos por hora", new string[] { "value" },
                        new string[] { "Gasto de desvíos" }, new string[] { "Gasto de desvíos" },
                        "label", "Horario", "", "Gasto de desvíos", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosTop10LlamadasGN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top 10 llamadas de desvío con mayor gasto");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosTop10Llamadas("Gasto"));


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Gasto de desvíos" });
                            ldt.Columns["Extensión"].ColumnName = "label";
                            ldt.Columns["Gasto de desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Top 10", new string[] { "value" },
                        new string[] { "Gasto de desvíos" }, new string[] { "Gasto de desvíos" },
                        "label", "Extensión", "", "Gasto de desvíos", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosTop10LlamadasMN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top 10 llamadas de desvío con mayor duración");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosTop10Llamadas("Minutos"));


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Minutos desvíos" });
                            ldt.Columns["Extensión"].ColumnName = "label";
                            ldt.Columns["Minutos desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Top 10", new string[] { "value" },
                        new string[] { "Minutos desvíos" }, new string[] { "Minutos desvíos" },
                        "label", "Extensión", "", "Minutos desvíos", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosTop10ExtN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones con mayor cantidad de llamadas de desvíos");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosTop10Ext());


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas desvíos" });
                            ldt.Columns["Extensión"].ColumnName = "label";
                            ldt.Columns["Llamadas desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Top 10", new string[] { "value" },
                        new string[] { "Llamadas desvíos" }, new string[] { "Llamadas desvíos" },
                        "label", "Extensión", "", "Llamadas desvíos", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosPerdidosPorEmpleN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones con llamadas de desvío no contestadas");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosTop10Ext());


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extensión", "Llamadas no contestadas" });
                            ldt.Columns["Extensión"].ColumnName = "label";
                            ldt.Columns["Llamadas no contestadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Llamadas no contestadas", new string[] { "value" },
                        new string[] { "Llamadas no contestadas" }, new string[] { "Llamadas no contestadas" },
                        "label", "Extensión", "", "Llamadas no contestadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepDesviosPorCarrierN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de desvíos por carrier");

                        DataTable ldt = DSODataAccess.Execute(RepDesviosPorHora());

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                             ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Carrier", "Gasto de desvíos" });
                            ldt.Columns["Carrier"].ColumnName = "label";
                            ldt.Columns["Gasto de desvíos"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt,
                        "Desvíos por carrier", new string[] { "value" },
                        new string[] { "Gasto de desvíos" }, new string[] { "Gasto de desvíos" },
                        "label", "Carrier", "", "Gasto de desvíos", "$#,0.00", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPerdidasPorTDestN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por tipo destino");

                        DataTable ldt =
                        DSODataAccess.Execute(
                        RepLlamadasPerdidasTdest("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTDestN2&TDest='' + convert(varchar,[Codigo Tipo Destino])"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Tipo Destino", "Numero" });
                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Tipo Destino", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo Destino"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Tipo Destino"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad llamadas desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Tipo Destino", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPerdidasPorSitioN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por sitio");
                        DataTable ldt =
                        DSODataAccess.Execute(
                           RepLlamadasPerdidasSitio("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorSitioN2&Sitio=''+ convert(varchar,[Codigo Sitio])"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Sitio", "Numero" });
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Sitio", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Sitio"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Sitio"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad llamadas desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Sitio", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPerdidasPorTopEmpleN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por Empleado");
                        DataTable ldt =
                            DSODataAccess.Execute(
                            RepLlamadasPerdidasTopEmple("[link] = ''" + Request.Path + "?Nav=RepLlamadasPerdidasPorTopEmpleN2&Emple='' + convert(varchar,[Codigo Empleado])"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Numero" });
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["Numero"].ColumnName = "Cantidad llamadas";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Empleado"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad llamadas desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Empleado", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPerdidasPorCencosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por Centro de Costos");
                        DataTable ldt =
                            DSODataAccess.Execute(
                            RepLlamadasPerdidasCencos(" '''" + Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN2&CenCos='''"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Cencos", "TotalLlamadas" });
                            ldt.Columns["Cencos"].ColumnName = "Centro de Costos";
                            ldt.Columns["TotalLlamadas"].ColumnName = "Cantidad llamadas";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Centro de Costos", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Centro de Costos"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad llamadas desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Centro de Costos", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasPerdidasPorCencosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por Centro de Empleados");
                        DataTable ldt =
                        DSODataAccess.Execute(
                        RepLlamadasPerdidasCencosN2("'''" + Request.Path + "?Nav=RepLlamadasPerdidasPorCencosN3&Emple='''"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre", "TotalLlamadas" });
                            ldt.Columns["Nombre"].ColumnName = "Empleado";
                            ldt.Columns["TotalLlamadas"].ColumnName = "Cantidad llamadas";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            //ldt = dvldt.ToTable(false, new string[] { "Nombre", "Cantidad llamadas" });
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "Cantidad llamadas" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Empleado"].ColumnName = "label";
                            ldt.Columns["Cantidad llamadas"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Cantidad llamadas desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad llamadas" },
                        "label", "Empleado", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepLlamadasAgrupadasEmple")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "");
                        DataTable ldt =
                        DSODataAccess.Execute(
                        RepLlamadasPerdidasCencosN2("'''" + Request.Path + "?Nav=RepLlamadasAgrupadasEmpleN2&Emple='''"));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre", "Numero", "Duracion", "Total" });
                            ldt.Columns["Nombre"].ColumnName = "Empleado";
                            ldt.Columns["Numero"].ColumnName = "Cantidad de llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Cantidad de Minutos";
                            ldt.Columns["Total"].ColumnName = "importeTotal";
                            ldt.AcceptChanges();
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "importeTotal Desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                        ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Empleado", "importeTotal" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Empleado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Empleado"].ColumnName = "label";
                            ldt.Columns["importeTotal"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "importeTotal desc", 10),
                        "importeTotal", new string[] { "value" },
                        new string[] { "importeTotal" }, new string[] { "importeTotal" },
                        "label", "Empleado", "", "importeTotal", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarStacked,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                    }
                    else if (param["Nav"] == "RepTabPorExtensionPI")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "");
                        DataTable ldt =
                        DSODataAccess.Execute(RepTraficoPorExtensionPI());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Extension", "CentroCostos", "Sitio", "CantidadLlamadas", "Minutos" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["CentroCostos"].ColumnName = "Centro de costos";
                            ldt.Columns["CantidadLlamadas"].ColumnName = "Cantidad llamadas";
                            ldt.Columns["Minutos"].ColumnName = "Cantidad minutos";
                            ldt.AcceptChanges();

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Cantidad llamadas desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Cantidad llamadas", new string[] { "value" },
                        new string[] { "Cantidad llamadas" }, new string[] { "Cantidad Llamadas" },
                        "label", "Extensión", "", "Cantidad llamadas", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepTabPorExtension")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "");
                        DataTable ldt =
                        DSODataAccess.Execute(ObtieneTraficoLlamadasPorExtension());

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

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Total", new string[] { "value" },
                        new string[] { "Total" }, new string[] { "Total" },
                        "label", "Extensión", "", "Total", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepTabContestadasYNoPorSitio")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "");
                        DataTable ldt =
                        DSODataAccess.Execute(RepTabCantLlamsContestadasYNoPorSitio());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Sitio", "LlamadasNoContestadas", "LlamadasContestadas", "Total" });
                            ldt.Columns["LlamadasContestadas"].ColumnName = "Llamadas contestadas";
                            ldt.Columns["LlamadasNoContestadas"].ColumnName = "Llamadas no contestadas";
                            ldt.AcceptChanges();

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Total", new string[] { "value" },
                        new string[] { "Total" }, new string[] { "Total" },
                        "label", "Sitio", "", "Total", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepTabContestadasYNoUnSitioPorExten")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "");
                        DataTable ldt =
                        DSODataAccess.Execute(ObtieneCantidadLlamadasContestadasyNoUnSitioPorExt());

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

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "Total", new string[] { "value" },
                        new string[] { "Total" }, new string[] { "Total" },
                        "label", "Extensión", "", "Total", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepTabParticipantesvsReunionesMes" || param["Nav"] == "RepTabParticipantesvsReunionesMes1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Participantes / reunión");
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
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Fecha", "No. Reuniones" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Fecha"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Fecha"].ColumnName = "label";
                            ldt.Columns["No. Reuniones"].ColumnName = "value";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "No. Reuniones", new string[] { "value" },
                        new string[] { "No. Reuniones" }, new string[] { "No. Reuniones" },
                        "label", "Nombre", "", "No. Reuniones", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                    else if (param["Nav"] == "RepTabParticipantesvsHorasMes" || param["Nav"] == "RepTabParticipantesvsHorasMes")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Tiempo dedicado a reuniones");
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
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(
                            lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "value desc", 10),
                        "No. Reuniones del mes", new string[] { "value" },
                        new string[] { "No. Reuniones del mes" }, new string[] { "No. Reuniones del mes" },
                        "label", "Nombre", "", "No. Reuniones del mes", "#,0", true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered,
                        lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                    }
                }
                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla y grafica) 2
                #endregion Exportar reportes con tabla y grafica

                // TODO : DO Paso 5 - Agregar la exportacion del reporte (tabla) 1

                #region Exportar Reportes solo con tabla

                if (param["Nav"] == "CenCosJerN3" || param["Nav"] == "TpLlamN3" ||
                    param["Nav"] == "CenCosN3" ||
                    param["Nav"] == "RepPorCarrierN5" ||
                    param["Nav"] == "RepLlamRedQN3" ||
                    param["Nav"] == "TDestPeEmN1" || param["Nav"] == "TDestPeEmN2" || param["Nav"] == "TDestPeEmN3" || param["Nav"] == "TDestPeEmN4" ||
                    param["Nav"] == "PorTipoLlamN1" || param["Nav"] == "PorTipoLlamN2" || param["Nav"] == "PorTipoLlamN3" ||
                    param["Nav"] == "NumMasCarosN1" || param["Nav"] == "NumMasCarosN2" || param["Nav"] == "ExtenUsoCodAutN1" ||
                    param["Nav"] == "DetalleMiConsumo" ||
                    param["Nav"] == "ConPorTipoLlamN1" ||
                    param["Nav"] == "HistoricoN5" || param["Nav"] == "HistoricoN6" || param["Nav"] == "HistoricoN7" || param["Nav"] == "HistoricoLlamadasN5" ||
                    param["Nav"] == "SitioN3" || param["Nav"] == "SitioN4" || param["Nav"] == "SitioN5" ||
                    param["Nav"] == "TDestN3" || param["Nav"] == "TDestN4" || param["Nav"] == "TDestN5" ||
                    param["Nav"] == "EmpleMCN3" || param["Nav"] == "EmpleMCN4" || param["Nav"] == "EmpleMCN5" ||
                    param["Nav"] == "RepTabDetalleParaFinanzas" ||
                    param["Nav"] == "RepTDestPrsN5" ||
                    param["Nav"] == "TopAreasN3" || param["Nav"] == "CenCosJerarquicoN4" ||
                    param["Nav"] == "RepDetalladoLlamsTelcelF4" ||
                    param["Nav"] == "RepTDestPrsOrange" || param["Nav"] == "RepTDestPrsUninet" || param["Nav"] == "RepTDestPrsBestel" ||
                    param["Nav"] == "RepTabAccesosAgrupadoN1" || param["Nav"] == "RepTabAccesosAgrupadoN2" ||
                    param["Nav"] == "RepTabResumenSpeedDialN2" || param["Nav"] == "ExportDirSpeedDial" ||
                    param["Nav"] == "RepMatConsumoCampaniaTDest" ||
                    param["Nav"] == "RepLlamsBuzonVoz" || param["Nav"] == "RepDetalleLlamsBuzonVoz" ||
                    param["Nav"] == "RepTabExtensionesNoAsignadasN2" || param["Nav"] == "RepTabCodigosNoAsignadosN2" ||
                    param["Nav"] == "RepEstTabCodEnMasDeNExtensionesN2" || param["Nav"] == "RepMatDiasProcesados" ||
                    param["Nav"] == "RepMatConsumoPorCarrier" || param["Nav"] == "RepMatConsumoPorCarrierN2" || param["Nav"] == "RepMatConsumoPorCarrierN3" ||
                    param["Nav"] == "RepMatConsumoPorCarrierN4" || param["Nav"] == "RepMatConsumoPorEmple" || param["Nav"] == "RepMatConsumoPorEmpleN2" ||
                    param["Nav"] == "RepMatConsumoPorSitio" ||
                    param["Nav"] == "RepMatConsumoPorSitioN2" || param["Nav"] == "RepMatConsumoPorSitioN3" ||
                    param["Nav"] == "RepMatConsumoPorCenCosSJ" || param["Nav"] == "RepMatConsumoPorCenCosSJN2" || param["Nav"] == "RepMatConsumoPorCenCosSJN3" ||
                    param["Nav"] == "RepMatConsumoPorCarrierN6" ||
                    param["Nav"] == "RepMatConsumoPorEmpleN4" || param["Nav"] == "RepMatConsumoPorSitioN5" || param["Nav"] == "RepMatConsumoPorCenCosSJN5" ||
                    param["Nav"] == "ReporteDetalladoCinvestav" ||
                    param["Nav"] == "RepTabMenuConsLLamadasMasTiempo" ||
                    param["Nav"] == "RepTabConsPorEmpleadoN4" || param["Nav"] == "RepTabConsPobmasMindeConv" || param["Nav"] == "RepTabConsPobmasMindeConvN4" ||
                    param["Nav"] == "RepTabLLamsFueraHoraLaboral" || param["Nav"] == "ConsEmpmasLlam" ||
                    param["Nav"] == "ConsEmpmasLlamN3" || param["Nav"] == "ConsEmpsMasCaros" ||
                    param["Nav"] == "RepTabConsumosPorNumMarcTipoLlamada" || param["Nav"] == "RepTabConsumosPorNumMarcTipoLlamadaN2" ||
                    param["Nav"] == "ConsEmpsMasCarosN7" || param["Nav"] == "ConsLlamsMasCaras" || param["Nav"] == "ConsNumerosMasMarcadas" ||
                    param["Nav"] == "ConsNumerosMasMarcadasN3" || param["Nav"] == "ConsLocalidMasMarcadas" ||
                    param["Nav"] == "ConsEmpsMasCarosN2" || param["Nav"] == "ConsEmpsMasCarosN4" || param["Nav"] == "ConsumoEmpsMasTiempoN4" ||
                    param["Nav"] == "RepTabLlamadasEntreSitiosN4" ||
                    param["Nav"] == "ConsLugmasCostN3" || param["Nav"] == "ConsLugmasCostN4" || param["Nav"] == "ConsLugmasCostN5" ||
                    param["Nav"] == "RepIndLlamMayorDuracionN2" ||
                    param["Nav"] == "RepIndLlamMayorDuracionN2LDM" ||
                    param["Nav"] == "RepTepBuscaLineasAVencer" ||
                    param["Nav"] == "ReporteMatricialDiasProcesados" ||
                    param["Nav"] == "RepTabReporteLlamadasEntradasFiltroExten" ||
                    param["Nav"] == "RepTabReporteLlamadasEntradasFiltroExtenN2" || param["Nav"] == "PorConceptoN3" ||
                    param["Nav"] == "ReportePorEmpleConJer" || param["Nav"] == "RepLlamPerdidasN3" ||
                    param["Nav"] == "RepTabSeeYouOnUtilClienteN2" || param["Nav"] == "RepTabSeeYouOnUtilClienteN3" ||
                    param["Nav"] == "RepTabImporteEmplePorTpLlam" || param["Nav"] == "RepTeleMarketing" || param["Nav"] == "RepTabBolsaDiariaN2" ||
                    param["Nav"] == "RepMatQualtiaPorCenCosN1" || param["Nav"] == "RepMatQualtiaPorCenCosN2" || param["Nav"] == "RepMatQualtiaPorCenCosN3" ||
                    param["Nav"] == "RepPerdidasPorExtN2" || param["Nav"] == "RepPerdidasPorExtN3" ||
                    param["Dash"] == "Desvios" || param["Nav"] == "RepDesviosTipoDestinoN3" || param["Nav"] == "RepExtensionessinActBD" ||
                    param["Nav"] == "RepLlamadasPorDispositivoN3" ||
                    param["Nav"] == "RepDesviosPorHoraN2" || param["Nav"] == "RepDesviosTop10ExtN2" || param["Nav"] == "RepDesviosPerdidosPorEmpleN2" ||
                    param["Nav"] == "RepLlamadasPerdidasPorTDestN2" || param["Nav"] == "RepLlamadasPerdidasPorSitioN2" ||
                    param["Nav"] == "RepLlamadasPerdidasPorTopEmpleN2" || param["Nav"] == "RepLlamadasPerdidasPorCencosN3" ||
                    param["Nav"] == "RepLlamadasAgrupadasEmpleN2" || param["Nav"] == "RepColabDirectos"
                    || param["Nav"] == "RepConsumoporDeptosN1" || param["Nav"] == "RepConsumoporDeptosN2" 
                    || param["Nav"] == "RepCantContestadasYNoContestadas" || param["Nav"] == "DetallePorExtPI" || param["Nav"] == "DetalleHospitales" ||
                    param["Nav"] == "RepTabParticipantesvsReunionesMes1"
                    )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "CenCosJerN3" || param["Nav"] == "EmpleMCN3" || param["Nav"] == "TpLlamN3" ||
                        param["Nav"] == "CenCosN3" ||
                        param["Nav"] == "RepPorCarrierN5" || param["Nav"] == "RepTDestPrsN5" || param["Nav"] == "TopAreasN3" || param["Nav"] == "CenCosJerarquicoN4" ||
                        param["Nav"] == "RepTabResumenSpeedDialN2" || param["Nav"] == "ConsEmpsMasCarosN4" ||
                        param["Nav"] == "ConsumoEmpsMasTiempoN4" || param["Nav"] == "ConsLugmasCostN5" || param["Nav"] == "RepMatConsumoPorEmpleN4")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

                        //BG. 20151104 agrego validacion del empleado, si es enlaces telmex exportara el reporte Detallado de Telmex 
                        string descEmple = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"] });
                        if (descEmple.ToLower().Contains("enlaces telmex"))
                        {

                            DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalladoEnlacesTelmex());
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                                            new string[] { "Folio", "PuntaA", "PuntaB", "Total" });
                                ldt.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado Enlaces Telmex");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }
                        else
                        {
                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());
                            RemoveColHerencia(ref RepDetallado);

                            RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                            //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                            #region Cambiar nombre de campos particulares
                            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                            if (nombresCamposParticulares.Rows.Count > 0)
                            {
                                foreach (DataRow row in nombresCamposParticulares.Rows)
                                {
                                    if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                    {
                                        RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                    }
                                }
                            }
                            #endregion Cambiar nombre de campos particulares
                            //20150703 NZ

                            //NZ 20161005
                            if (RepDetallado.Columns.Contains("Duracion"))
                            {
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                            }

                            //BG 20170814
                            if (RepDetallado.Columns.Contains("Llamadas"))
                            {
                                RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";     //BG 20170814 se cambia "Llamadas" por "Cantidad Llamadas"
                            }

                            EliminarColumnasDeAcuerdoABanderas(RepDetallado);



                            if (RepDetallado.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Dash"] == "Desvios" || param["Nav"] == "RepDesviosPorHoraN2" || param["Nav"] == "RepDesviosTop10ExtN2" || param["Nav"] == "RepDesviosPerdidosPorEmpleN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de desvíos");

                        #region Reporte Detallado

                        DataTable RepDetallado = DSODataAccess.Execute(RepDetalladoDesvios());

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, RepDetallado, "Reporte", "Tabla");
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "RepPerdidasPorExtN2")
                    {
                        int exportDetall = 1;
                        if (Session["ExporDetall"] != null)
                        {
                            exportDetall = Convert.ToInt32(Session["ExporDetall"]);
                        }
                        if (exportDetall == 2 && LlamPerd.Visible == true)
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas perdidas");

                            #region Reporte Detallado

                            DataTable RepDetallado = DSODataAccess.Execute(RepDetalladoPerdidas());

                            if (RepDetallado.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, RepDetallado, "Reporte", "Tabla");
                            }

                            #endregion Reporte Detallado
                        }

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas perdidas por extensión");

                        #region Reporte

                        DataTable Rep = DSODataAccess.Execute(RepTabPerdidasPorExt());

                        if (Rep.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, Rep, "Reporte", "Tabla");
                        }

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepPerdidasPorExtN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas perdidas por extensión");

                        #region Reporte Detallado

                        DataTable RepDetallado = DSODataAccess.Execute(RepDetalladoPorExt());

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, RepDetallado, "Reporte", "Tabla");
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "RepLlamRedQN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

                        DataTable RepDetallado = DSODataAccess.Execute(ConsultaRepConsumoSimuladoDestinoDetalle());
                        RemoveColHerencia(ref RepDetallado);

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "RepLlamadasPorDispositivoN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

                        DataTable RepDetallado = DSODataAccess.Execute(RepLlamadasPorDispositivoDet());
                        RemoveColHerencia(ref RepDetallado);

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "RepMatQualtiaPorCenCosN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

                        DataTable RepDetallado = DSODataAccess.Execute(RepDetalladoEmple());

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "TDestPeEmN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo de servicio");

                        #region Reporte por tipo de servicio

                        //Se obtiene el DataSource del reporte
                        DataTable RepPorTDestPeEm = DSODataAccess.Execute(ConsultaPorTipoDestinoPeEm());

                        if (RepPorTDestPeEm.Rows.Count > 0 && RepPorTDestPeEm.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(RepPorTDestPeEm);
                            RepPorTDestPeEm = dvGrafConsHist.ToTable(false, new string[] { "Nombre Tipo Destino", "Total", "Numero", "Duracion" });
                            RepPorTDestPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo destino";
                            RepPorTDestPeEm.Columns["Total"].ColumnName = "Total";
                            RepPorTDestPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";      //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            RepPorTDestPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                        }

                        if (RepPorTDestPeEm.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepPorTDestPeEm, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por tipo de servicio
                    }
                    else if (param["Nav"] == "TDestPeEmN2")
                    {
                        #region Seleccion de reporte

                        string descTipoDestino = FCAndControls.AgregaEtiqueta(new string[] { param["TDest"] });
                        if (descTipoDestino.ToLower().Contains("telcel"))
                        {
                            param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                            if (param["Emple"] != "-1")
                            {
                                //20150115 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                                int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                                if (numeroDeLineasTelcel > 1)
                                {
                                    DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "373"));//Telcel
                                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                    {
                                        DataView dvldt = new DataView(ldt);
                                        ldt = dvldt.ToTable(false,
                                                                                    new string[] { "Extension", "NombreCompleto", "Total" });
                                        ldt.Columns["Extension"].ColumnName = "Línea";
                                        ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                        ldt.Columns["Total"].ColumnName = "Total";
                                        ldt.AcceptChanges();

                                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                                    }
                                }
                                else
                                {
                                    ConsultaLineaPorCarrier("telcel");
                                    DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                                    if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                                    {
                                        DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                        RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                                    new string[] { "Concepto", "Total" });
                                        RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                        RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                        RepDetalleXConcepto.AcceptChanges();

                                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                                    }
                                }
                            }
                        }
                        else if (descTipoDestino.ToLower().Contains("nextel"))
                        {
                            param["Emple"] = DSODataAccess.ExecuteScalar(ConsultaEmpleadoLigadoAlUsuario()).ToString();
                            if (param["Emple"] != "-1")
                            {
                                //20150115 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                                int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                                if (numeroDeLineasTelcel > 1)
                                {
                                    DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "78019"));//Nextel
                                    if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                    {
                                        DataView dvldt = new DataView(ldt);
                                        ldt = dvldt.ToTable(false,
                                                                                    new string[] { "Extension", "NombreCompleto", "Total" });
                                        ldt.Columns["Extension"].ColumnName = "Línea";
                                        ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                        ldt.Columns["Total"].ColumnName = "Total";
                                        ldt.AcceptChanges();

                                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                                    }
                                }
                                else
                                {
                                    DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                                    if (datosFactura.Rows.Count > 0)
                                    {
                                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                                    }
                                }

                            }
                        }
                        else
                        {
                            DataTable RepPorNumMarcPeEm = DSODataAccess.Execute(ConsultaPorNumMarcPeEm());

                            if (RepPorNumMarcPeEm.Rows.Count > 0 && RepPorNumMarcPeEm.Columns.Count > 0)
                            {
                                DataView dvGrafConsHist = new DataView(RepPorNumMarcPeEm);

                                if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                                {
                                    RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Etiqueta", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                                }
                                else
                                {
                                    if (DSODataContext.Schema.ToLower() != "bimbo")
                                    {
                                        RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Nombre Localidad", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                                        RepPorNumMarcPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                                    }
                                    else
                                    {
                                        RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Nombre Localidad", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion" });

                                    }
                                    RepPorNumMarcPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                                }


                                RepPorNumMarcPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                                RepPorNumMarcPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                                RepPorNumMarcPeEm.Columns["Total"].ColumnName = "Total";
                                RepPorNumMarcPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";        //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                                RepPorNumMarcPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";       //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"

                                RepPorNumMarcPeEm.AcceptChanges();

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepPorNumMarcPeEm, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "TDestPeEmN3")
                    {
                        #region Seleccion de reporte

                        string descTipoDestino = FCAndControls.AgregaEtiqueta(param["TDest"]);
                        if (descTipoDestino.ToLower().Contains("telcel"))
                        {
                            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                            if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                            {
                                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                    new string[] { "Concepto", "Total" });
                                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                RepDetalleXConcepto.AcceptChanges();

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else if (descTipoDestino.ToLower().Contains("nextel"))
                        {
                            DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                            if (datosFactura.Rows.Count > 0)
                            {
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else
                        {
                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetallePeEm());

                            if (RepDetallado.Rows.Count > 0 && RepDetallado.Columns.Count > 0)
                            {
                                DataView dvGrafConsHist = new DataView(RepDetallado);
                                RepDetallado = dvGrafConsHist.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado",
                                                                                                  "Tipo Llamada", "Etiqueta" });
                                RepDetallado.Columns["Extension"].ColumnName = "Extensión";
                                RepDetallado.Columns["Fecha"].ColumnName = "Fecha";
                                RepDetallado.Columns["Hora"].ColumnName = "Hora";
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";        //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                RepDetallado.Columns["Total"].ColumnName = "Total";
                                RepDetallado.Columns["Nombre Localidad"].ColumnName = "Localidad";
                                RepDetallado.Columns["Numero Marcado"].ColumnName = "Número marcado";
                                RepDetallado.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                                RepDetallado.Columns["Etiqueta"].ColumnName = "Etiqueta";


                                //20180710 RM Se agregga apostrofe a la columna del numero marcado para la exportacion

                                if (RepDetallado.Columns.Contains("Número marcado"))
                                {
                                    foreach (DataRow row in RepDetallado.Rows)
                                    {

                                        if (!row["Número marcado"].ToString().StartsWith("'"))
                                        {
                                            row["Número marcado"] = "'" + row["Número marcado"];
                                        }

                                    }
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "TDestPeEmN4")
                    {
                        #region Seleccion del reporte

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
                            if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                            {
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel,
                                    DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                        0, "Totales"), "Reporte", "Tabla");
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

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel,
                                        DTIChartsAndControls.agregaTotales(
                                             DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                                             0, "Totales"), "Reporte", "Tabla");
                                }
                            }
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "PorTipoLlamN3" || param["Nav"] == "NumMasCarosN2" || param["Nav"] == "DetalleMiConsumo")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte detallado

                        DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetallePeEm());

                        if (RepDetallado.Rows.Count > 0 && RepDetallado.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(RepDetallado);
                            RepDetallado = dvGrafConsHist.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado",
                                                                                                  "Tipo Llamada", "Etiqueta" });
                            RepDetallado.Columns["Extension"].ColumnName = "Extensión";
                            RepDetallado.Columns["Fecha"].ColumnName = "Fecha";
                            RepDetallado.Columns["Hora"].ColumnName = "Hora";
                            RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";        //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepDetallado.Columns["Total"].ColumnName = "Total";
                            RepDetallado.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            RepDetallado.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepDetallado.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                            RepDetallado.Columns["Etiqueta"].ColumnName = "Etiqueta";

                        }



                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte detallado
                    }
                    else if (param["Nav"] == "PorTipoLlamN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo de llamada");

                        #region Reporte por tipo de llamada

                        DataTable GrafPorTipoLlamPeEm = null;
                        //NZ 20160921
                        if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                        {
                            GrafPorTipoLlamPeEm = DSODataAccess.Execute(consultaPorTipoLlamPeEmDetalleBanorte(""));
                        }
                        else { GrafPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaPorTipoLlamPeEm("")); }

                        if (GrafPorTipoLlamPeEm.Rows.Count > 0 && GrafPorTipoLlamPeEm.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafPorTipoLlamPeEm);
                            GrafPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Tipo Llamada", "Total" });
                        }

                        ExportacionExcelRep.CrearGrafico(GrafPorTipoLlamPeEm, "Consumo por tipo de llamada", new string[] { "Total" },
                            new string[] { "Total" }, new string[] { "Total" }, "Tipo Llamada", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlPie, lExcel, "{Tabla}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte por tipo de llamada
                    }
                    else if (param["Nav"] == "PorTipoLlamN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por número marcado");

                        #region Reporte por numero marcado

                        //Se obtiene el DataSource del reporte
                        DataTable RepPorNumMarcPeEm = DSODataAccess.Execute(ConsultaPorNumMarcPeEm());

                        if (RepPorNumMarcPeEm.Rows.Count > 0 && RepPorNumMarcPeEm.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(RepPorNumMarcPeEm);

                            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                            {
                                RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Etiqueta", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                            }
                            else
                            {
                                RepPorNumMarcPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Nombre Localidad", "Nombre Tipo Destino",
                                                                                                             "Total", "Numero", "Duracion","Tipo Llamada" });
                                RepPorNumMarcPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            }

                            RepPorNumMarcPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepPorNumMarcPeEm.Columns["Nombre Tipo Destino"].ColumnName = "Tipo de destino";
                            RepPorNumMarcPeEm.Columns["Total"].ColumnName = "Total";
                            RepPorNumMarcPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";       //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                            RepPorNumMarcPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepPorNumMarcPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";

                        }

                        if (RepPorNumMarcPeEm.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepPorNumMarcPeEm, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por numero marcado
                    }
                    else if (param["Nav"] == "NumMasCarosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte números más caros");

                        #region Reporte Numeros mas caros

                        //Se obtiene el DataSource del reporte
                        DataTable RepNumMasCarosPeEm = DSODataAccess.Execute(ConsultaNumerosMasCarosPeEm());

                        if (RepNumMasCarosPeEm.Rows.Count > 0 && RepNumMasCarosPeEm.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(RepNumMasCarosPeEm);
                            if (DSODataContext.Schema.ToLower() == "k5banorte" || DSODataContext.Schema.ToLower() == "evox")
                            {
                                RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Etiqueta", "Tipo Llamada", "Total", "Duracion", "Numero" });
                            }
                            else
                            {
                                RepNumMasCarosPeEm = dvGrafConsHist.ToTable(false, new string[] { "Numero Marcado", "Nombre Localidad",
                                                                                                               "Tipo Llamada", "Total", "Duracion","Numero" });

                                RepNumMasCarosPeEm.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            }

                            RepNumMasCarosPeEm.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepNumMasCarosPeEm.Columns["Tipo Llamada"].ColumnName = "Tipo de llamada";
                            RepNumMasCarosPeEm.Columns["Total"].ColumnName = "Total";
                            RepNumMasCarosPeEm.Columns["Duracion"].ColumnName = "Cantidad minutos";      //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                            RepNumMasCarosPeEm.Columns["Numero"].ColumnName = "Cantidad llamadas";       //NZ 20161005 se cambia "Llamadas" por "Cantidad llamadas"
                        }
                        if (RepNumMasCarosPeEm.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepNumMasCarosPeEm, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Numeros mas caros
                    }
                    else if (param["Nav"] == "ExtenUsoCodAutN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones en las que se usó el código de llamadas");

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
                        if (RepExtenDondeUtilizoCodAut.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepExtenDondeUtilizoCodAut, 0, "Totales"), "Reporte", "Tabla");
                        }
                        #endregion Reporte Extensiones en las que se utilizo el codigo de llamadas
                    }
                    else if (param["Nav"] == "ConPorTipoLlamN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por tipo de llamada");

                        #region Reporte por tipo de llamada

                        DataTable GrafConPorTipoLlamPeEm = DSODataAccess.Execute(ConsultaConsumoPorTipoLlamada());
                        if (GrafConPorTipoLlamPeEm.Rows.Count > 0 && GrafConPorTipoLlamPeEm.Columns.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConPorTipoLlamPeEm);
                            GrafConPorTipoLlamPeEm = dvGrafConsHist.ToTable(false, new string[] { "Etiqueta", "total" });
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConPorTipoLlamPeEm, "Consumo por tipo de llamada", new string[] { "total" },
                            new string[] { "total" }, new string[] { "total" }, "Etiqueta", "", "", "total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlPie, lExcel, "{Tabla}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte por tipo de llamada
                    }
                    else if (param["Nav"] == "HistoricoN5" || param["Nav"] == "TDestN3" || param["Nav"] == "EmpleMCN3" || param["Nav"] == "RepMatConsumoPorEmpleN4" || param["Nav"] == "HistoricoLlamadasN5")
                    {
                        #region Seleccion de reporte

                        string descTipoDestino = FCAndControls.AgregaEtiqueta(new string[] { param["TDest"] });
                        if (descTipoDestino.ToLower().Contains("telcel"))
                        {
                            //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "373")); //Telcel
                                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                {
                                    DataView dvldt = new DataView(ldt);
                                    ldt = dvldt.ToTable(false,
                                                                            new string[] { "Extension", "NombreCompleto", "Total" });
                                    ldt.Columns["Extension"].ColumnName = "Línea";
                                    ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                    ldt.Columns["Total"].ColumnName = "Total";
                                    ldt.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("telcel");

                                DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                                if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                                {
                                    DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                    RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                            new string[] { "Concepto", "Total" });
                                    RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                    RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                    RepDetalleXConcepto.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura telcel");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else if (descTipoDestino.ToLower().Contains("nextel"))
                        {
                            //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "78019")); //Nextel
                                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                {
                                    DataView dvldt = new DataView(ldt);
                                    ldt = dvldt.ToTable(false,
                                                                            new string[] { "Extension", "NombreCompleto", "Total" });
                                    ldt.Columns["Extension"].ColumnName = "Línea";
                                    ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                    ldt.Columns["Total"].ColumnName = "Total";
                                    ldt.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("nextel");

                                DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura AT&T");  //NZ 20151012 Se cambia "nextel" por "AT&T".
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else if (descTipoDestino.ToLower().Contains("enlaces telmex")) //BG.20151104 se agrega validacion del Tdest "enlaces Telmex"
                        {

                            DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalladoEnlacesTelmex());
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                                                    new string[] { "Folio", "PuntaA", "PuntaB", "Total" });
                                ldt.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado Enlaces Telmex");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        }
                        else
                        {
                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());
                            RemoveColHerencia(ref RepDetallado);

                            RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                            //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                            #region Cambiar nombre de campos particulares
                            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                            if (nombresCamposParticulares.Rows.Count > 0)
                            {
                                foreach (DataRow row in nombresCamposParticulares.Rows)
                                {
                                    if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                    {
                                        RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                    }
                                }
                            }
                            #endregion Cambiar nombre de campos particulares
                            //20150703 NZ

                            //NZ 20161005
                            if (RepDetallado.Columns.Contains("Duracion"))
                            {
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                            }

                            EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "HistoricoN6" || param["Nav"] == "TDestN4" || param["Nav"] == "EmpleMCN4")
                    {
                        #region Seleccion de reporte dependiendo carrier
                        string descTipoDestino = FCAndControls.AgregaEtiqueta(new string[] { param["TDest"] });
                        if (descTipoDestino.ToLower().Contains("telcel"))
                        {
                            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                            if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                            {
                                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                    new string[] { "Concepto", "Total" });
                                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                RepDetalleXConcepto.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura telcel");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");

                        }
                        if (descTipoDestino.ToLower().Contains("nextel"))
                        {
                            DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura AT&T"); //NZ 20151012 Se cambia "nextel" por "AT&T".
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                        }
                        #endregion Seleccion de reporte dependiendo carrier
                    }
                    else if (param["Nav"] == "HistoricoN7" || param["Nav"] == "TDestN5" || param["Nav"] == "EmpleMCN5")
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
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por concepto");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDesgloseFacturaPorConcepto, 0, "Totales"), "Reporte", "Tabla");
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

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por llamada");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDesgloseFacturaPorLlamada, 0, "Totales"), "Reporte", "Tabla");

                            }

                            #endregion Seleccion de reporte
                        }

                        #endregion Reporte Costo por Empleado
                    }
                    else if (param["Nav"] == "SitioN3")
                    {
                        #region Seleccion de reporte

                        string descSitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                        if (descSitio.ToLower().Contains("telcel"))
                        {
                            //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "373")); //Telcel
                                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                {
                                    DataView dvldt = new DataView(ldt);
                                    ldt = dvldt.ToTable(false,
                                                                            new string[] { "Extension", "NombreCompleto", "Total" });
                                    ldt.Columns["Extension"].ColumnName = "Línea";
                                    ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                    ldt.Columns["Total"].ColumnName = "Total";
                                    ldt.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("telcel");

                                DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                                if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                                {
                                    DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                    RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                            new string[] { "Concepto", "Total" });
                                    RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                    RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                    RepDetalleXConcepto.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura telcel");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else if (descSitio.ToLower().Contains("nextel"))
                        {
                            //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("nextel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "78019")); //Nextel
                                if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                                {
                                    DataView dvldt = new DataView(ldt);
                                    ldt = dvldt.ToTable(false,
                                                                            new string[] { "Extension", "NombreCompleto", "Total" });
                                    ldt.Columns["Extension"].ColumnName = "Línea";
                                    ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                    ldt.Columns["Total"].ColumnName = "Total";
                                    ldt.AcceptChanges();
                                }
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
                                ConsultaLineaPorCarrier("nextel");

                                DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura AT&T"); //NZ 20151012 Se cambia "nextel" por "AT&T".
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else if (descSitio.ToLower().Contains("enlaces telmex"))
                        {

                            DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalladoEnlacesTelmex());
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                                                    new string[] { "Folio", "PuntaA", "PuntaB", "Total" });
                                ldt.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado Enlaces Telmex");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        }
                        else
                        {
                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());
                            RemoveColHerencia(ref RepDetallado);

                            RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                            //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                            #region Cambiar nombre de campos particulares
                            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                            if (nombresCamposParticulares.Rows.Count > 0)
                            {
                                foreach (DataRow row in nombresCamposParticulares.Rows)
                                {
                                    if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                    {
                                        RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                    }
                                }
                            }
                            #endregion Cambiar nombre de campos particulares
                            //20150703 NZ

                            //NZ 20161005
                            if (RepDetallado.Columns.Contains("Duracion"))
                            {
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                            }

                            RepDetallado = EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "SitioN4")
                    {
                        #region Seleccion de reporte dependiendo carrier
                        string descSitio = FCAndControls.AgregaEtiqueta(new string[] { param["Sitio"] });
                        if (descSitio.ToLower().Contains("telcel"))
                        {
                            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFacturaTelcel());
                            if (RepDetalleXConcepto.Rows.Count > 0 && RepDetalleXConcepto.Columns.Count > 0)
                            {
                                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                                                    new string[] { "Concepto", "Total" });
                                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                RepDetalleXConcepto.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura telcel");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");

                        }
                        if (descSitio.ToLower().Contains("nextel"))
                        {
                            DataTable datosFactura = DSODataAccess.Execute(ConsultaDetalleFacturaNextel());
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle factura AT&T"); //NZ 20151012 Se cambia "nextel" por "AT&T".
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(datosFactura, 0, "Totales"), "Reporte", "Tabla");
                        }
                        #endregion Seleccion de reporte dependiendo carrier
                    }
                    else if (param["Nav"] == "SitioN5")
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
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por concepto");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDesgloseFacturaPorConcepto, 0, "Totales"), "Reporte", "Tabla");
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
                                    RepDesgloseFacturaPorLlamada.Columns["Duracion Minutos"].ColumnName = "Cantidad minutos";    //NZ 20161005 se cambia "Minutos" por "Cantidad minutos"
                                    RepDesgloseFacturaPorLlamada.Columns["Costo"].ColumnName = "Total";
                                    RepDesgloseFacturaPorLlamada.Columns["Punta A"].ColumnName = "Localidad Origen";
                                    RepDesgloseFacturaPorLlamada.Columns["Dir Llamada"].ColumnName = "Tipo";
                                    RepDesgloseFacturaPorLlamada.Columns["Punta B"].ColumnName = "Localidad Destino";
                                    RepDesgloseFacturaPorLlamada.AcceptChanges();
                                }

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por llamada");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDesgloseFacturaPorLlamada, 0, "Totales"), "Reporte", "Tabla");

                            }

                            #endregion Seleccion de reporte
                        }

                        #endregion Reporte Costo por Empleado
                    }
                    else if (param["Nav"] == "RepTabDetalleParaFinanzas")
                    {
                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleParaFinanzas());

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

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Detalle Para Finanzas");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

                        //20150425 RJ.Agrego instruccion para renombrar la hoja de "Reporte" a "Llamados"
                        lExcel.Renombrar("Reporte", "Llamados");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepDetalladoLlamsTelcelF4")
                    {
                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleLlamsTelcelF4());

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

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Detallados de Llamadas Telcel");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTDestPrsOrange")
                    {
                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlOrangePrs());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                                            new string[] { "Empleado", "CenCos", "Importe" });

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Factura Orange");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTDestPrsUninet")
                    {
                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlUninetCOLPrs());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                                            new string[] { "Cia", "Cuenta", "C.C", "Proyecto", "Producto", "Temporal", "Debito Valor", "Credito Valor", "Descripcion" });

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Factura Uninet Colombia");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTDestPrsBestel")
                    {
                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleEnlBestelPrs());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                                            new string[] { "Concepto", "Importe" });

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Factura Bestel");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTabAccesosAgrupadoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Accesos por Usuario");

                        #region Reporte AccesosAgrupado

                        DataTable ldt = DSODataAccess.Execute(ConsultaAccesosAgrupado());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                                           new string[] { "Usuario", "Descripcion Usuario", "Cantidad Accesos" });
                            ldt.Columns["Usuario"].ColumnName = "CuentaUsuario";
                            ldt.Columns["Descripcion Usuario"].ColumnName = "Descripción";
                            ldt.Columns["Cantidad Accesos"].ColumnName = "Cantidad de Accesos";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                        #endregion Reporte AccesosAgrupado
                    }
                    else if (param["Nav"] == "RepTabAccesosAgrupadoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Bítacora Accesos");

                        #region Reporte AccesosAgrupado

                        DataTable ldt = DSODataAccess.Execute(ConsultaAccesosAgrupadoN2(param["Usuario"]));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                                           new string[] { "Usuario", "Descripcion Usuario", "Fecha Acceso" });
                            ldt.Columns["Usuario"].ColumnName = "CuentaUsuario";
                            ldt.Columns["Descripcion Usuario"].ColumnName = "Descripción";
                            ldt.Columns["Fecha Acceso"].ColumnName = "Fecha Acceso";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                        #endregion Reporte AccesosAgrupado
                    }
                    else if (param["Nav"] == "ExportDirSpeedDial")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Directorio Speed Dials");

                        #region Directorio Speed Dials

                        DataTable ldt = DSODataAccess.Execute(ConsultaGetDirectorioSpeedDials());

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                        #endregion Directorio Speed Dials
                    }
                    else if (param["Nav"] == "RepMatConsumoCampaniaTDest")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Campaña");
                        DataTable ldt = DSODataAccess.Execute(ConsultaRepMatConsumoPorCampañaTipoDest());

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt.Columns.Remove("id");
                            ldt.Columns.Remove("idSitio");
                            ldt.Columns.Remove("idCarrier");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepLlamsBuzonVoz")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas en Buzón de Voz");
                        DataTable ldt = DSODataAccess.Execute(RepTabLlamadasBuzonDeVoz());

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Fecha", "Segundos", "Minutos", "LlamsBuzon" });
                            ldt.Columns["LlamsBuzon"].ColumnName = "Llamadas en Buzón";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "[Fecha] asc, [Segundos] Desc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepDetalleLlamsBuzonVoz")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas en Buzón de Voz");
                        DataTable ldt = DSODataAccess.Execute(RepTabDetalleLlamsBuzonDeVoz());

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);


                            string cliente = DSODataContext.Schema;

                            if (cliente.ToUpper() == "SWISSHOSPITAL")
                            {
                                ldt = dvldt.ToTable(false, new string[] { "Fecha", "Hora", "Duracion", "NumMarcado", "Extension", "ExtensionIntermedia", "ExtensionInicial", "Buzon" });

                                ldt.Columns["ExtensionIntermedia"].ColumnName = "Extension Intermedia";
                                ldt.Columns["ExtensionInicial"].ColumnName = "Extension Inicial";
                                ldt.Columns["Extension"].ColumnName = "Extensión Final";
                            }
                            else
                            {
                                ldt = dvldt.ToTable(false, new string[] { "Fecha", "Hora", "Duracion", "NumMarcado", "Extension", "Buzon" });
                                ldt.Columns["Extension"].ColumnName = "Extensión";
                            }


                            ldt.Columns["Duracion"].ColumnName = "Duración";
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Buzon"].ColumnName = "Buzón";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "[Fecha] asc, [Hora] asc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTabExtensionesNoAsignadasN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones no asignadas");

                        #region Reporte

                        DataTable original = DSODataAccess.Execute(RepTabExtensionesNoAsignadasN2());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Codigo Autorizacion", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepTabCodigosNoAsignadosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Códigos no asignados");

                        #region Reporte

                        DataTable original = DSODataAccess.Execute(RepTabCodigosNoAsignadosN2());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Codigo Autorizacion", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Codigo Autorizacion"].ColumnName = "Código Autorización";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total Desc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepEstTabCodEnMasDeNExtensionesN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Códigos en más de N extensiones");

                        #region Reporte

                        DataTable original = DSODataAccess.Execute(RepEstTabCodEnMasDeNExtensionesN2());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total de minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepMatDiasProcesados")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Días procesados");

                        #region Reporte

                        DataTable original = DSODataAccess.Execute(RepMatDiasProcesados());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                formatoCols.Add((i > 0) ? "{0:0,0}" : string.Empty);

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);


                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Fecha ASC");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Fecha Asc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepMatConsumoPorCarrier")
                    {
                        #region Reporte
                        List<string> formatoCols = new List<string>();
                        List<int> boundfieldCols = new List<int>();
                        List<string> nombresCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por carrier");

                        DataTable original = DSODataAccess.Execute(RepMatConsumoPorCarrier());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                if (i > 1)
                                {
                                    boundfieldCols.Add(i);
                                    nombresCols.Add(dc.ColumnName);
                                    formatoCols.Add((i == 2) ? string.Empty : "{0:c}");
                                }
                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols.ToArray());

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total DESC"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepMatConsumoPorCarrierN2" || param["Nav"] == "RepMatConsumoPorEmpleN2" || param["Nav"] == "RepMatConsumoPorSitio"
                        || param["Nav"] == "RepMatConsumoPorCenCosSJN3" || param["Nav"] == "ConsEmpsMasCarosN2")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        List<int> boundfieldCols = new List<int>();
                        List<string> nombresCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por sitio");

                        DataTable original = new DataTable();

                        switch (param["Nav"])
                        {
                            case "RepMatConsumoPorCarrierN2":
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN2());
                                break;
                            case "RepMatConsumoPorEmpleN2":
                                original = DSODataAccess.Execute(RepMatConsumoPorEmpleN2());
                                break;
                            case "RepMatConsumoPorSitio":
                                original = DSODataAccess.Execute(RepMatConsumoPorSitio());
                                break;
                            case "RepMatConsumoPorCenCosSJN3":
                                original = DSODataAccess.Execute(RepMatConsumoPorCenCosSJN3());
                                break;
                            case "ConsEmpsMasCarosN2":
                                original = DSODataAccess.Execute(ConsEmpsMasCarosN2());
                                break;
                            default:
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN2());
                                break;
                        }

                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                if (i > 1)
                                {
                                    boundfieldCols.Add(i);
                                    nombresCols.Add(dc.ColumnName);
                                    formatoCols.Add((i == 2) ? string.Empty : "{0:c}");
                                }
                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols.ToArray());

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total DESC"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepMatConsumoPorCarrierN3" || param["Nav"] == "RepMatConsumoPorSitioN2" || param["Nav"] == "RepMatConsumoPorCenCosSJ" || param["Nav"] == "ConsLugmasCostN3" || param["Nav"] == "RepMatQualtiaPorCenCosN1")
                    {
                        #region Reporte
                        List<string> formatoCols = new List<string>();
                        List<int> boundfieldCols = new List<int>();
                        List<string> nombresCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por centro de costos");

                        DataTable original = new DataTable();

                        switch (param["Nav"])
                        {
                            case "RepMatConsumoPorCarrierN3":
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN3());
                                break;
                            case "RepMatConsumoPorSitioN2":
                                original = DSODataAccess.Execute(RepMatConsumoPorSitioN2());
                                break;
                            case "RepMatConsumoPorCenCosSJ":
                                original = DSODataAccess.Execute(RepMatConsumoPorCenCosSJ());
                                break;
                            case "ConsLugmasCostN3":
                                original = DSODataAccess.Execute(ConsultaConsLugmasCostN3());
                                break;
                            case "RepMatQualtiaPorCenCosN1":
                                original = DSODataAccess.Execute(RepMatQualtiaPorCenCos());
                                original.Columns["Total"].SetOrdinal(5);
                                break;
                            default:
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN3());
                                break;
                        }

                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                if (i > 3)
                                {
                                    boundfieldCols.Add(i);
                                    nombresCols.Add(dc.ColumnName);
                                    formatoCols.Add((i == 4) ? string.Empty : "{0:c}");
                                }
                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols.ToArray());
                            ldt.Columns["CenCosDesc"].ColumnName = "Centro de costos";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total DESC"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepMatConsumoPorCarrierN4" || param["Nav"] == "RepMatConsumoPorEmple" || param["Nav"] == "RepMatConsumoPorSitioN3"
                        || param["Nav"] == "RepMatConsumoPorCenCosSJN2" || param["Nav"] == "ConsLugmasCostN4" || param["Nav"] == "RepMatQualtiaPorCenCosN2")
                    {

                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        List<int> boundfieldCols = new List<int>();
                        List<string> nombresCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por empleado");

                        DataTable original = new DataTable();
                        switch (param["Nav"])
                        {
                            case "RepMatConsumoPorCarrierN4":
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN4());
                                break;
                            case "RepMatConsumoPorEmple":
                                original = DSODataAccess.Execute(RepMatConsumoPorEmple());
                                break;
                            case "RepMatConsumoPorSitioN3":
                                original = DSODataAccess.Execute(RepMatConsumoPorSitioN3());
                                break;
                            case "RepMatConsumoPorCenCosSJN2":
                                original = DSODataAccess.Execute(RepMatConsumoPorCenCosSJN2());
                                break;
                            case "ConsLugmasCostN4":
                                original = DSODataAccess.Execute(ConsLugmasCostN4());
                                break;
                            case "RepMatQualtiaPorCenCosN2":
                                original = DSODataAccess.Execute(RepMatQualtiaPorEmple());
                                original.Columns["Total"].SetOrdinal(5);
                                break;
                            default:
                                original = DSODataAccess.Execute(RepMatConsumoPorCarrierN4());
                                break;
                        }

                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                if (DSODataContext.Schema.ToString().ToLower() == "institutomora")
                                {
                                    if (i == 1)
                                    {
                                        boundfieldCols.Add(i);
                                        nombresCols.Add(dc.ColumnName);
                                        formatoCols.Add(string.Empty);
                                    }
                                    if (i > 4)
                                    {
                                        boundfieldCols.Add(i);
                                        nombresCols.Add(dc.ColumnName);
                                        formatoCols.Add((i == 5) ? string.Empty : "{0:c}");
                                    }
                                }
                                else
                                {
                                    if (i > 3)
                                    {
                                        boundfieldCols.Add(i);
                                        nombresCols.Add(dc.ColumnName);
                                        formatoCols.Add((i == 4) ? string.Empty : "{0:c}");
                                    }
                                }


                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols.ToArray());
                            ldt.Columns["NomCompleto"].ColumnName = "Nombre";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total DESC"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "ReporteDetalladoCinvestav")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        List<int> boundfieldCols = new List<int>();
                        List<string> nombresCols = new List<string>();
                        int[] columnasNoVisibles = null;
                        int[] columnasVisibles = null;
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas");

                        DataTable original = DSODataAccess.Execute(ConsultaDetalle());

                        DataTable ldt = original.Clone();

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            if (ldt.Columns.Contains("RID"))
                                ldt.Columns.Remove("RID");
                            if (ldt.Columns.Contains("RowNumber"))
                                ldt.Columns.Remove("RowNumber");
                            if (ldt.Columns.Contains("TopRID"))
                                ldt.Columns.Remove("TopRID");
                            if (ldt.Columns.Contains("Centro de costos"))
                                ldt.Columns.Remove("Centro de costos");
                            if (ldt.Columns.Contains("Llamadas"))
                                ldt.Columns.Remove("Llamadas");
                            if (ldt.Columns.Contains("Carrier"))
                                ldt.Columns.Remove("Carrier");
                            if (ldt.Columns.Contains("Codigo Autorizacion"))
                                ldt.Columns.Remove("Codigo Autorizacion");
                            if (ldt.Columns.Contains("Localidad"))
                                ldt.Columns.Remove("Localidad");
                            if (ldt.Columns.Contains("TotalSimulado"))
                                ldt.Columns.Remove("TotalSimulado");
                            if (ldt.Columns.Contains("CostoSimulado"))
                                ldt.Columns.Remove("CostoSimulado");
                            if (ldt.Columns.Contains("CostoReal"))
                                ldt.Columns.Remove("CostoReal");
                            if (ldt.Columns.Contains("SM"))
                                ldt.Columns.Remove("SM");
                            if (ldt.Columns.Contains("Sitio"))
                                ldt.Columns.Remove("Sitio");

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "[TotalSimulado] desc");

                            columnasVisibles = new int[] { 1, 0, 2, 3, 4, 5, 7, 12, 13 };
                            columnasNoVisibles = new int[] { 6, 8, 9, 10, 11 };

                            if (ldt.Columns.Contains("Colaborador"))
                            {
                                ldt.Columns["Colaborador"].ColumnName = "Nombre";
                            }

                            if (ldt.Columns.Contains("Tipo llamada"))
                            {
                                ldt.Columns["Tipo llamada"].ColumnName = "Tipo";
                            }

                            if (ldt.Columns.Contains("Duracion"))
                            {
                                ldt.Columns["Duracion"].ColumnName = "Duración";
                            }

                            if (ldt.Columns.Contains("Llamadas"))
                            {
                                ldt.Columns["Llamadas"].ColumnName = "Cantidad llamadas";
                            }

                            if (ldt.Columns.Contains("Numero Marcado"))
                            {
                                ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            }

                            if (ldt.Columns.Contains("Tipo de destino"))
                            {
                                ldt.Columns["Tipo de destino"].ColumnName = "Localización";
                            }

                            if (ldt.Columns.Contains("TotalReal"))
                            {
                                ldt.Columns["TotalReal"].ColumnName = "Cargo";
                            }
                        }



                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total DESC"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepTabMenuConsLLamadasMasTiempo")
                    {
                        #region Reporte
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Llamadas");

                        DataTable original = DSODataAccess.Execute(RepTabMenuConsLLamadasMasTiempo());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "NumMarcado", "Fecha", "Duracion Minutos", "Costo", "Nombre Localidad", "Tipo Llamada" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";

                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Minutos desc"), 0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabConsPorEmpleadoN4")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado");

                        DataTable original = DSODataAccess.Execute(RepTabConsPorEmpleadoN4());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "NumMarcado", "Tipo Llamada", "Etiqueta" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc "), 0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabConsPobmasMindeConv")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Destinos");

                        DataTable original = DSODataAccess.Execute(RepTabConsPobmasMindeConv());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Costo", "Duracion", "TotLlam" });
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";
                            ldt.Columns["TotLlam"].ColumnName = "Llamadas";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Minutos Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Minutos Desc "), 0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabConsPobmasMindeConvN4")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Destinos");

                        DataTable original = DSODataAccess.Execute(RepTabConsPobmasMindeConvN4());
                        DataTable ldt = original.Clone();
                        ldt.Columns[0].DataType = typeof(string);

                        foreach (DataRow row in original.Rows)
                        {
                            ldt.ImportRow(row);
                        }

                        original.Clear();

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "NumMarcado", "Tipo Llamada", "Etiqueta" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["NumMarcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Tipo Llamada"].ColumnName = "Tipo de Llamada";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc "), 0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabLLamsFueraHoraLaboral")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas Fuera del Horario Laboral");

                        string WhereHorario = BuscaHorario();
                        DataTable ldt = DSODataAccess.Execute(RepTabLLamsFueraHoraLaboral(WhereHorario));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Centro de Costos" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Fecha"].ColumnName = "Fecha";
                            ldt.Columns["Hora"].ColumnName = "Hora";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";

                            ldt.AcceptChanges();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "ConsLugmasCost")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Destinos");

                        DataTable ldt = DSODataAccess.Execute(ConsLugmasCost(1));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Costo", "Duracion", "TotLlam", });
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Costo"].ColumnName = "Total Importe";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";
                            ldt.Columns["TotLlam"].ColumnName = "Llamadas";


                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Importe Desc");
                            ldt.AcceptChanges();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Total Importe")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "ConsEmpmasLlamN3")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Empleados");

                        DataTable ldt = DSODataAccess.Execute(ConsEmpmasLlamN3(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            if (Convert.ToString(Session["OcultarColumnImporte"]) == "1")
                            {
                                ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });



                            }
                            else
                            {
                                ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta" });
                                ldt.Columns["Total"].ColumnName = "Importe Total";
                                ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            }

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total Miutos";

                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";




                            ldt.AcceptChanges();

                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "ConsEmpsMasCaros")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Empleados ");

                        DataTable ldt = DSODataAccess.Execute(ConsEmpsMasCaros(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            string[] cols  = new string[] { };
                            if (DSODataContext.Schema.ToUpper() == "FCA")
                            {
                                cols = new string[] { "Nombre Completo", "Codigo Empleado", "Total", "Numero", "Duracion" };
                            }
                            else
                            {
                                cols = new string[] { "Nombre Completo", "Codigo Empleado", "Centro de Costos", "Codigo Centro de Costos", "Total", "Numero", "Duracion" };
                                ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costos";
                            }

                            ldt = dvldt.ToTable(false, cols);
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Numero"].ColumnName = "Total Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total Minutos";

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabConsumosPorNumMarcTipoLlamada")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por Tipo de Llamada");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsumosPorNumMarcTipoLlamada());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Numero Marcado", "Llamadas", "Duracion Minutos", "Total", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });

                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Etiqueta"].ColumnName = "Localidad";

                            //ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");

                            ldt.AcceptChanges();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            foreach (DataRow row in ldt.Rows)
                            {
                                row["Número Marcado"] = "'" + row["Número Marcado"].ToString();
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "RepTabConsumosPorNumMarcTipoLlamadaN2")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado ");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsumosPorNumMarcTipoLlamadaN2());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });


                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";

                            //ldt = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        foreach (DataRow row in ldt.Rows)
                        {
                            row["Número Marcado"] = "'" + row["Número Marcado"].ToString();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "RepTabConsumosPorNumMarcTipoLlamadaN2")
                    {
                        #region Reporte

                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Empleados ");

                        DataTable ldt = DSODataAccess.Execute(RepTabConsumosPorNumMarcTipoLlamadaN2());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });


                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";

                            //ldt = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }

                        foreach (DataRow row in ldt.Rows)
                        {
                            row["Número Marcado"] = "'" + row["Número Marcado"].ToString();
                        }

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "ConsEmpsMasCarosN7")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado ");

                        #region Grid
                        DataTable ldt = DSODataAccess.Execute(ConsEmpsMasCarosN7(""));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Clave Tipo Llamada", "Etiqueta" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";


                            //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:c}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }


                            foreach (DataRow row in ldt.Rows)
                            {
                                row["Número Marcado"] = "'" + row["Número Marcado"];
                            }


                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "ConsLlamsMasCaras")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas más caras ");

                        #region Grid
                        DataTable ldt = DSODataAccess.Execute(ConsLlamsMasCaras());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(
                                                            false,
                                                            new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Sitio", "Nombre Localidad" });
                            //new string[] { "Extension", "Nombre Completo", "Codigo Empleado", "Numero Marcado", "Fecha", "Hora", "Duracion Minutos", "Costo", "Nombre Sitio", "Codigo Sitio", "Nombre Localidad", "Codigo Localidad" });

                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Nombre Completo"].ColumnName = "Empleado";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Nombre Sitio"].ColumnName = "Sitio";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";


                            //dtReporte = DTIChartsAndControls.ordenaTabla(dtReporte, "Importe Total Desc");
                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:c}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }


                        foreach (DataRow row in ldt.Rows)
                        {
                            row["Número Marcado"] = "'" + row["Número Marcado"];
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "ConsNumerosMasMarcadas")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Números más Marcados ");

                        #region Grid
                        DataTable ldt = DSODataAccess.Execute(ConsNumerosMasMarcadas());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(
                                                            false,
                                                            new string[] { "Numero Marcado", "Llamadas", "Nombre Localidad", "Total", "Cant Emp", "Duracion" });


                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Cant Emp"].ColumnName = "Número  de Empleados";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";

                            ldt.AcceptChanges();
                        }



                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:c}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }


                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }


                        foreach (DataRow row in ldt.Rows)
                        {
                            row["Número Marcado"] = "'" + row["Número Marcado"];
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "ConsNumerosMasMarcadasN3")
                    {
                        #region Reporte
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado ");

                        DataTable ldt = DSODataAccess.Execute(ConsNumerosMasMarcadasN3());

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            string[] cols = (DSODataContext.Schema.ToUpper() == "FCA") ? new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta", "Nombre Empleado", "Nombre del Sitio" } : new string[] { "Extension", "Fecha", "Hora", "Duracion", "Total", "Nombre Localidad", "Numero Marcado", "Tipo Llamada", "Etiqueta", "Nombre Empleado", "Centro de Costos", "Nombre del Sitio" };
                            ldt = dvldt.ToTable(false, cols);


                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Duracion"].ColumnName = "Total Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";
                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";

                            ldt.AcceptChanges();
                        }

                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:c}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }


                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }


                        foreach (DataRow row in ldt.Rows)
                        {
                            row["Número Marcado"] = "'" + row["Número Marcado"];
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion Reporte
                    }
                    else if (param["Nav"] == "ConsLocalidMasMarcadas")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top Destinos");

                        #region Grid

                        DataTable ldt = DSODataAccess.Execute(ConsLocalidMasMarcadas(1));

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Nombre Localidad", "Nombre Tipo Destino", "Numero", "Duracion", "Total", });

                            ldt.Columns["Nombre Localidad"].ColumnName = "Localidad";
                            ldt.Columns["Nombre Tipo Destino"].ColumnName = "Tipo Destino";
                            ldt.Columns["Numero"].ColumnName = "Total de Llamadas";
                            ldt.Columns["Duracion"].ColumnName = "Total de Minutos";
                            ldt.Columns["Total"].ColumnName = "Importe Total";


                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total de Llamadas Desc");
                            ldt.AcceptChanges();
                        }


                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            var nombresCols = new string[ldt.Columns.Count];

                            int i = 0;
                            foreach (DataColumn dc in ldt.Columns)
                            {
                                nombresCols[i] = dc.ColumnName;
                                if (dc.ColumnName == "Importe Total")
                                {
                                    formatoCols.Add("{0:0,0}");
                                }
                                else
                                {
                                    formatoCols.Add(string.Empty);
                                }

                                i++;
                            }

                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, nombresCols);
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
                    else if (param["Nav"] == "RepTabLlamadasEntreSitiosN4")
                    {
                        List<string> formatoCols = new List<string>();
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado ");

                        #region Reporte

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

                        #region Grid

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                                       DTIChartsAndControls.ordenaTabla(dtReporte, "Cantidad de Minutos Desc "), 0, "Total"), "Reporte", "Tabla");

                        #endregion Grid

                        #endregion
                    }
                    else if (param["Nav"] == "ConsEmpmasLlam")
                    {

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top colaboradores con más llamadas");
                        DataTable ldt = DSODataAccess.Execute(ConsEmpmasLlam());

                        #region Grid
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);

                            if (Convert.ToString(Session["OcultarColumnImporte"]) != "1")
                            {

                                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Total", "Duracion", "Numero" });
                            }
                            else
                            {
                                ldt = dvldt.ToTable(false, new string[] { "Nombre Completo", "Duracion", "Numero" });
                            }


                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Duracion"].ColumnName = "Minutos";
                            ldt.Columns["Numero"].ColumnName = "Llamadas";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Llamadas Desc");
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion
                    }
                    else if (param["Nav"] == "RepIndLlamMayorDuracionN2" || param["Nav"] == "RepIndLlamMayorDuracionN2LDM")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                        int opc = 0;
                        if (param["Nav"] == "RepIndLlamMayorDuracionN2LDM")
                        {
                            opc = 1;
                        }
                        #region Reporte
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
                        //if (DSODataContext.Schema.ToUpper() != "SCHINDLER" && DSODataContext.Schema.ToUpper() != "K5SCHINDLER")
                        //{
                        //    query.AppendLine(" AND vchCodigo IN('CelLoc','CelNac')");
                        //}
                        //else
                        //{
                        //    query.AppendLine(" AND vchCodigo IN('LDM')");
                        //}

                        query.AppendLine("SELECT @valores");

                        string tiposDestino = DSODataAccess.ExecuteScalar(query.ToString()).ToString();

                        if (!string.IsNullOrEmpty(valor) && !string.IsNullOrEmpty(tiposDestino))
                        {
                            WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";

                            //if (!string.IsNullOrEmpty(valor) && !string.IsNullOrEmpty(tiposDestino) && (DSODataContext.Schema.ToUpper() != "SCHINDLER" && DSODataContext.Schema.ToUpper() != "K5SCHINDLER"))
                            //{
                            //    WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
                            //}
                            //else
                            //{
                            //    WhereAdicional = "[Duracion] = " + valor + " AND [iCodTDest] IN (" + tiposDestino + ")";
                            //}




                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());
                            RemoveColHerencia(ref RepDetallado);

                            RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                            //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                            #region Cambiar nombre de campos particulares
                            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                            if (nombresCamposParticulares.Rows.Count > 0)
                            {
                                foreach (DataRow row in nombresCamposParticulares.Rows)
                                {
                                    if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                    {
                                        RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                    }
                                }
                            }
                            #endregion Cambiar nombre de campos particulares
                            //20150703 NZ

                            //NZ 20161005
                            if (RepDetallado.Columns.Contains("Duracion"))
                            {
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                            }

                            //BG 20170814
                            if (RepDetallado.Columns.Contains("Llamadas"))
                            {
                                RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";     //BG 20170814 se cambia "Llamadas" por "Cantidad Llamadas"
                            }

                            EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                            if (RepDetallado.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        #endregion
                    }
                    else if (param["Nav"] == "RepTepBuscaLineasAVencer")
                    {
                        DataTable dtReporte = DSODataAccess.Execute(ConsultaBuscaLineasAVencer(4));
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


                        //GridView grdvdtRep = new GridView();
                        //grdvdtRep = DTIChartsAndControls.GridView("RepTabBuscaLineasAVencerGrid", dtCloneRep, false, "Totales",
                        //                    new string[] { "", "", "", "", "" });

                        //grdvdtRep.RowDataBound += (sender, e) => CambiaColorLetra_RowDataBound(sender, e, "Meses", "Fecha Fin");
                        //grdvdtRep.DataBind();

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Renovación de planes");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtCloneRep, "Reporte", "Tabla");
                        #endregion
                    }
                    else if (param["Nav"] == "ReporteMatricialDiasProcesados")
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

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Dias Procesados");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtRep, "Reporte", "Tabla");
                            #endregion

                        }

                    }


                    else if (param["Nav"] == "RepTabReporteLlamadasEntradasFiltroExten")
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
                        if (dtRep.Columns.Contains("Tdest")) { dtRep.Columns.Remove("Tdest"); }


                        #region Reporte
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte llamadas de entrada CC");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtRep, "Reporte", "Tabla");
                        #endregion

                    }
                    else if (param["Nav"] == "RepTabReporteLlamadasEntradasFiltroExtenN2")
                    {
                        WhereAdicional = " Extension = ''" + param["Extension"].ToString() + "''";

                        DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalle());
                        RemoveColHerencia(ref RepDetallado);

                        RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                        //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                        #region Cambiar nombre de campos particulares
                        DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                        if (nombresCamposParticulares.Rows.Count > 0)
                        {
                            foreach (DataRow row in nombresCamposParticulares.Rows)
                            {
                                if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                {
                                    RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                }
                            }
                        }
                        #endregion Cambiar nombre de campos particulares
                        //20150703 NZ

                        //NZ 20161005
                        if (RepDetallado.Columns.Contains("Duracion"))
                        {
                            RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                        }

                        RepDetallado = EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                    }
                    else if (param["Nav"] == "PorConceptoN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desgloce de llamadas");

                        #region Reporte

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
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(DTIChartsAndControls.ordenaTabla(dt, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
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

                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(DTIChartsAndControls.ordenaTabla(dtLlams, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
                            }

                            #endregion Seleccion de reporte
                        }

                        #endregion
                    }
                    else if (param["Nav"] == "ReportePorEmpleConJer")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Comsumo por colaborador jerarquico");

                        #region Reporte
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

                                if (dt.Columns.Contains("Emple"))
                                {
                                    dt.Columns.Remove("Emple");
                                }

                                if (dt.Columns.Contains("CenCos"))
                                {
                                    dt.Columns.Remove("CenCos");
                                }

                                dt.AcceptChanges();

                                /*Renombrar Columnas*/
                                if (dt.Columns.Contains("NominaA"))
                                {
                                    dt.Columns["NominaA"].ColumnName = "Nómina";
                                }

                                if (dt.Columns.Contains("Jefe"))
                                {
                                    dt.Columns["Jefe"].ColumnName = "Jefe DIrecto";
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

                                dt.AcceptChanges();
                                /*GRID*/

                                //List<string> FormatosCol = new List<string>() { "", "", "", "{0:c}", "{0:0,0}", "{0:0,0}" };
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");

                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion


                    }
                    else if (param["Nav"] == "RepLlamPerdidasN3")
                    {
                        int exportDetall = 1;
                        string tituloReporte = "";
                        if (Session["ExporDetall"] != null)
                        {
                            exportDetall = Convert.ToInt32(Session["ExporDetall"]);
                        }
                        if (exportDetall == 1)
                        {
                            tituloReporte = "Detallado de Llamadas Perdidas";
                        }
                        else
                        {
                            tituloReporte = "Detalle de Llamadas Perdidas";
                        }
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, tituloReporte);
                        DataTable dt = DSODataAccess.Execute(ConsultaReporteLlamNoContestadasN3(exportDetall));
                        #region Grid
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
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");

                        #endregion

                    }

                    else if (param["Nav"] == "EmpleMCNSiana3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

                        //BG. 20151104 agrego validacion del empleado, si es enlaces telmex exportara el reporte Detallado de Telmex 
                        string descEmple = FCAndControls.AgregaEtiqueta(new string[] { param["Emple"] });
                        if (descEmple.ToLower().Contains("enlaces telmex"))
                        {

                            DataTable ldt = DSODataAccess.Execute(ConsultaRepDetalladoEnlacesTelmex());
                            if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                            {
                                DataView dvldt = new DataView(ldt);
                                ldt = dvldt.ToTable(false,
                                                                    new string[] { "Folio", "PuntaA", "PuntaB", "Total" });
                                ldt.AcceptChanges();
                            }
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado Enlaces Telmex");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }
                        else
                        {
                            DataTable RepDetallado = DSODataAccess.Execute(ConsultaDetalleSiana());
                            RemoveColHerencia(ref RepDetallado);

                            RepDetallado = DTIChartsAndControls.ordenaTabla(RepDetallado, "[TotalSimulado] desc");

                            //20150703 NZ Validar si se cambiara el nombre de alguna columna particularmente por cliente.
                            #region Cambiar nombre de campos particulares
                            DataTable nombresCamposParticulares = ObtenerCamposConNombreParticular("ConsultaDetalle");
                            if (nombresCamposParticulares.Rows.Count > 0)
                            {
                                foreach (DataRow row in nombresCamposParticulares.Rows)
                                {
                                    if (RepDetallado.Columns.Contains(row["NombreOrigCampo"].ToString()))
                                    {
                                        RepDetallado.Columns[row["NombreOrigCampo"].ToString()].ColumnName = row["NombreNuevoCampo"].ToString();
                                    }
                                }
                            }
                            #endregion Cambiar nombre de campos particulares
                            //20150703 NZ

                            //NZ 20161005
                            if (RepDetallado.Columns.Contains("Duracion"))
                            {
                                RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                            }

                            //BG 20170814
                            if (RepDetallado.Columns.Contains("Llamadas"))
                            {
                                RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";     //BG 20170814 se cambia "Llamadas" por "Cantidad Llamadas"
                            }

                            EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                            if (RepDetallado.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        #endregion
                    }

                    else if (param["Nav"] == "RepTabSeeYouOnUtilClienteN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Utilización Sistema Cliente");
                        DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasClienteN2());
                        #region Grid
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
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion
                    }
                    else if (param["Nav"] == "RepTabSeeYouOnUtilClienteN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Utilización Sistemas");
                        DataTable ldt = DSODataAccess.Execute(ConsultaUtilSistemasClienteN3());
                        #region Grid
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
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");

                        #endregion
                    }
                    else if (param["Nav"] == "RepTabImporteEmplePorTpLlam")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Importe por Tipo de Llamada");

                        DataTable ldt = DSODataAccess.Execute(ConsultaLlamPorTipoDestinoN2());
                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "nominaEmple", "nombreEmple", "cencosDesc", "Total" });
                            ldt.Columns["nominaEmple"].ColumnName = "Nomina";
                            ldt.Columns["nombreEmple"].ColumnName = "Colaborador";
                            ldt.Columns["cencosDesc"].ColumnName = "Centro de Costos";
                            ldt.Columns["Total"].ColumnName = "Importe";

                            ldt.AcceptChanges();

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Importe desc");
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                    }
                    else if (param["Nav"] == "RepTeleMarketing")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Comunicacion e Imagen Telemarketing");
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
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                    }
                    else if (param["Nav"] == "RepTabBolsaDiariaN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Consumo de Bolsa");
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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }
                    }
                    else if (param["Nav"] == "RepDesviosTipoDestinoN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle por Desvíos");
                        DataTable ldt = DSODataAccess.Execute(RepDetalladoDesvios(1));
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if (param["Nav"] == "RepExtensionessinActBD")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Extensiones sin Actividad vs Base de Datos");
                        DataTable ldt = DSODataAccess.Execute(RepExtensionessinActVSBD());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }
                    }
                    else if (param["Nav"] == "RepTraficoExtensiones1pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Tráfico de Extensiones");
                        DataTable ldt = DSODataAccess.Execute(RepTraficoExtensiones());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }
                    }

                    else if (param["Nav"] == "RepLlamadasPerdidasPorTDestN2" || param["Nav"] == "RepLlamadasPerdidasPorSitioN2" ||
                        param["Nav"] == "RepLlamadasPerdidasPorTopEmpleN2" || param["Nav"] == "RepLlamadasPerdidasPorCencosN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalle());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }
                    }
                    else if (param["Nav"] == "RepLlamadasAgrupadasEmpleN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalle());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if (param["Nav"] == "RepColabDirectos")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = DSODataAccess.Execute(RepConsumoColabDirectos());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if (param["Nav"] == "RepConsumoporDeptosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = DSODataAccess.Execute(RepConsumoporDeptoN1());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if (param["Nav"] == "RepConsumoporDeptosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = DSODataAccess.Execute(RepConsumoporDeptoN2());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if (param["Nav"] == "RepCantContestadasYNoContestadas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas que fueron recibidas, no fueron contestadas, o fueron devueltas");
                        DataTable ldt = DSODataAccess.Execute(RepCantLineasContestadasYNoContestadas());
                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        }

                    }
                    else if ((param["Nav"] == "RepMatConsumoPorCarrierN6" || param["Nav"] == "RepMatConsumoPorSitioN5")
                        && Request.QueryString["Sitio"] != null 
                        && Request.QueryString["Emple"] != null 
                        && Request.QueryString["TDest"] != null)
                    {
                        if(DSODataContext.Schema.ToLower() == "fca")
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas");
                            DataTable ldt = DSODataAccess.Execute(ConsultaDetalle());
                            if (ldt.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                            }
                        } 
                    }
                    else if (param["Nav"] == "DetallePorExtPI" || param["Nav"] == "DetalleHospitales")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");
                        DataTable ldt = new DataTable();
                        
                        if (param["Nav"] == "DetallePorExtPI")
                            ldt = DSODataAccess.Execute(RepDetalladoDesdePorExtension());
                        else if (param["Nav"] == "DetalleHospitales")
                            ldt = DSODataAccess.Execute(RepDetalladoHospitales());


                        #region Grid

                        if (ldt.Rows.Count > 0 && ldt.Columns.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);

                            string cliente = DSODataContext.Schema;

                            ldt = 
                                dvldt.ToTable(false, new string[] { "Fecha", "Hora", "Extensión", "Numero Marcado", "Localidad", "Duracion", "Total", "Timbrado", "Tipo de destino", "Resultado", "OrigReason", "DestReason", "Centro de costos", "Colaborador", "Sitio" });

                            ldt.Columns["Numero Marcado"].ColumnName = "NÚmero Marcado";
                            ldt.Columns["Duracion"].ColumnName = "Duración";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "[Total] asc, [Duración] asc"), 0, "Totales"), "Reporte", "Tabla");

                        #endregion Grid
                    }
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
                    if (Tabla.Columns.Contains("SM")) Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalReal"].ColumnName = "Total";
                }
            }

            if (Convert.ToString(Session["OcultarColumnImporte"]) == "1")
            {
                Tabla.Columns.Remove("Total");
            }

            return Tabla;
        }

        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }


        #endregion

    }
}