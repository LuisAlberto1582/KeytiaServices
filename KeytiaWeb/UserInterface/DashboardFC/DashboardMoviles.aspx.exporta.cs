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
        #region Exportacion

        public void ExportXLS()
        {
            CrearXLS(".xlsx");
        }

        protected void CrearXLS(string lsExt)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                // TODO : DM Paso 7 - Agregar la exportacion del reporte 
                string exportar = param["Nav"].ToString();
                #region Exportar reportes con tabla y grafica

                if (param["Nav"] == "HistoricoN1" || param["Nav"] == "CenCosN3" || param["Nav"] == "DetFacPEmN1" ||
                    param["Nav"] == "LlamACelRedN1" || param["Nav"] == "CenCosN4" || param["Nav"] == "ResumenPorPlanesN1" ||
                    param["Nav"] == "TopLineasMasCarasN1" || param["Nav"] == "TopLineasMasCarasN3" || param["Nav"] == "ResumPorEquipoN1" ||
                    param["Nav"] == "CenCosN1" ||
                    param["Nav"] == "ResumenPorConcepTelcelN1" ||
                    param["Nav"] == "CenCosN2" ||
                    param["Nav"] == "EmpleN1" ||
                    param["Nav"] == "HistoricoAnioActVsAntN1" ||
                    param["Nav"] == "HistoricoPeEmN1" ||
                    param["Nav"] == "EmpleAcumN1" || param["Nav"] == "RepEmpleAcum" ||
                    param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoN2" || param["Nav"] == "CenCosJerarquicoN3" ||
                    param["Nav"] == "Direccion" || param["Nav"] == "SubDireccion" || /*param["Nav"] == "EmpleadosN2" ||*/
                    param["Nav"] == "ConsumoPorCtaMaestra" ||
                    param["Nav"] == "ConsumoMovilPorTecno" ||
                    param["Nav"] == "CarrierCCN2" || param["Nav"] == "HistoricoConCantLineasN1" ||
                    param["Nav"] == "ConsumoLineasNoIdent" || param["Nav"] == "ConsumoCantLineasNuevas" ||
                    param["Nav"] == "RepTabConsumoLineasSinAct2Pnls" ||
                    param["Nav"] == "ConsumoLineasEnElMes" || param["Nav"] == "ConsumoPorCentroDeCostos2Pnl" ||
                    param["Nav"] == "DireccionJerarq" || param["Nav"] == "LineasNuevasTelcel2pPnl" || param["Nav"] == "DistConsumoIntPorAppN1" || param["Nav"] == "HistGastoInternetN1" || param["Nav"] == "RepUsoInternetTabs"
                    )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGraficoMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "HistoricoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo histórico");

                        #region Reporte historico

                        //Se obtiene el DataSource del reporte
                        DataTable GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistoricoMov());

                        if (GrafConsHist.Rows.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total", "CantidadLineas" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["Total"].ColumnName = "Total";
                            GrafConsHist.Columns["CantidadLineas"].ColumnName = "Cantidad líneas";
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "HistoricoConCantLineasN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Comportamiento histórico móviles");

                        #region Histórico con cantidad de líneas

                        DataTable GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistoricoMoviles());

                        if (GrafConsHist.Rows.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "TotalLineas", "TotalSinIVA", "TotalConIVA" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["TotalLineas"].ColumnName = "Cant. Líneas";
                            GrafConsHist.Columns["TotalSinIVA"].ColumnName = "Total sin IVA";
                            GrafConsHist.Columns["TotalConIVA"].ColumnName = "Total con IVA";
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Cantidad líneas",
                            new string[] { "Cant. Líneas" }, new string[] { "Cant. Líneas" }, new string[] { "Cant. Líneas" }, "Mes", "", "", "Cant. Líneas", "#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(GrafConsHist, 0, "Totales"), "Reporte", "Tabla");

                        #endregion
                    }
                    else if (param["Nav"] == "CenCosN3" || param["Nav"] == "CenCosN4")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura / " + FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"], param["Emple"] }));

                        #region Reporte por concepto

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
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte por concepto

                        #region Reporte historico

                        DataTable GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistorico());
                        if (GrafConsHist.Rows.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["Total"].ColumnName = "Total";
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "DetFacPEmN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura telcel");

                        #region Reporte Detalle de factura telcel
                        DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());

                        //NZ 20150401  Se remueve el concepto de Rentas de este reporte.
                        RepDetalleXConcepto = CostoXConceptoRemoverRentas(RepDetalleXConcepto);

                        if (RepDetalleXConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                            RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                new string[] { "Concepto", "Total" });
                            RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                            RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                            RepDetalleXConcepto.AcceptChanges();
                        }


                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Detalle de factura telcel

                        #region Reporte historico

                        DataTable GrafConsHistDetFacPEmN1 = DSODataAccess.Execute(ConsultaConsumoHistoricoPerfilEmpleado());
                        if (GrafConsHistDetFacPEmN1.Rows.Count > 0)
                        {
                            DataView dvGrafConsHistDetFacPEmN1 = new DataView(GrafConsHistDetFacPEmN1);
                            GrafConsHistDetFacPEmN1 = dvGrafConsHistDetFacPEmN1.ToTable(false, new string[] { "Nombre Mes", "Total" });
                            GrafConsHistDetFacPEmN1.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHistDetFacPEmN1.Columns["Total"].ColumnName = "Total";
                            GrafConsHistDetFacPEmN1.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHistDetFacPEmN1, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte historico
                    }
                    else if (param["Nav"] == "LlamACelRedN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas a celulares de la red");

                        #region Reporte Llamadas a celulares de red

                        DataTable RepLlamACeluRed = DSODataAccess.Execute(ConsultaLlamACelularesDeLaRed(""));
                        if (RepLlamACeluRed.Rows.Count > 0)
                        {
                            DataView dvRepLlamACeluRed = new DataView(RepLlamACeluRed);
                            RepLlamACeluRed = dvRepLlamACeluRed.ToTable(false,
                                new string[] { "Extension", "Nombre Completo", "Total", "Duracion", "Numero" });
                            RepLlamACeluRed.Columns["Extension"].ColumnName = "Línea";
                            RepLlamACeluRed.Columns["Nombre Completo"].ColumnName = "Responsable";
                            RepLlamACeluRed.Columns["Total"].ColumnName = "Total";
                            RepLlamACeluRed.Columns["Duracion"].ColumnName = "Minutos";
                            RepLlamACeluRed.Columns["Numero"].ColumnName = "Llamadas";
                            RepLlamACeluRed.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepLlamACeluRed, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Llamadas a celulares de red

                        #region Reporte Llamadas a celulares de red (grafica)

                        if (RepLlamACeluRed.Rows.Count > 0)
                        {
                            DataView dvRepLlamACeluRed = new DataView(RepLlamACeluRed);
                            RepLlamACeluRed = dvRepLlamACeluRed.ToTable(false,
                                new string[] { "Línea", "Total" });
                            if (RepLlamACeluRed.Rows[RepLlamACeluRed.Rows.Count - 1]["Línea"].ToString() == "Totales")
                            {
                                RepLlamACeluRed.Rows[RepLlamACeluRed.Rows.Count - 1].Delete();
                            }
                            RepLlamACeluRed.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(RepLlamACeluRed, "Llamadas a celulares de la red", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Línea", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Llamadas a celulares de red (grafica)
                    }
                    else if (param["Nav"] == "ResumenPorPlanesN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Resumen por planes");

                        #region Reporte Resumen por planes

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepResumenPorPlanes(""));
                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Plan", "Total", "Cantidad" });
                            ldt.Columns["Plan"].ColumnName = "Plan";
                            ldt.Columns["Total"].ColumnName = "Renta";
                            ldt.Columns["Cantidad"].ColumnName = "Cantidad";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Resumen por planes

                        #region Reporte Resumen por planes (Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Plan", "Renta" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Plan"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Resumen por planes", new string[] { "Renta" }, new string[] { "Renta" },
                                         new string[] { "Total" }, "Plan", "", "", "Renta", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Resumen por planes (Grafica)
                    }
                    else if (param["Nav"] == "TopLineasMasCarasN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Top líneas más caras");

                        #region Reporte Top líneas más caras

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras(""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Extension", "NombreCompleto", "Total" });
                            ldt.Columns["Extension"].ColumnName = "Línea";
                            ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Top líneas más caras

                        #region Reporte Top líneas más caras (Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Línea", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Top líneas más caras", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Línea", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Top líneas más caras (Grafica)
                    }
                    else if (param["Nav"] == "TopLineasMasCarasN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Por número marcado");

                        #region Reporte Por número marcado

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepPorNumeroMarcado(""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Numero Marcado", "Costo", "Duracion Minutos", "Llamadas", "TpoProm", "Punta B" });
                            ldt.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Llamadas"].ColumnName = "Llamadas";
                            ldt.Columns["TpoProm"].ColumnName = "Tiempo promedio";
                            ldt.Columns["Punta B"].ColumnName = "Localidad";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Por número marcado

                        #region Reporte Por número marcado (Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Número marcado", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Número marcado"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.Columns["Número marcado"].ColumnName = "Numero marcado";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Por número marcado", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Numero marcado", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Por número marcado (Grafica)
                    }
                    else if (param["Nav"] == "ResumPorEquipoN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Resumen por equipos");

                        #region Resumen por equipos

                        DataTable ldt = DSODataAccess.Execute(ConsultaResumenEquiposTelcel(""));

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

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Resumen por equipos

                        #region Resumen por equipos (Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Tipo", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tipo"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Resumen por equipos ", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Tipo", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Resumen por equipos (Grafica)
                    }
                    else if (param["Nav"] == "CenCosN1") //20141024 AM. Se agrega exportacion de reporte 
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Costo por Área");

                        #region Reporte Costo por Área

                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoPorCenCos(""));
                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Nombre Centro de Costos", "Total", "Renta", "Diferencia" });
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Renta"].ColumnName = "Renta";
                            ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Costo por Área

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
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Costo por Área", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Área", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlDoughnut, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ResumenPorConcepTelcelN1") //20141024 AM. Se agrega exportacion de reporte 
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Resumen por conceptos telcel");

                        #region Reporte Resumen por conceptos telcel

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepResumenPorConceptosTelcel(""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Servicio", "Descripcion", "Mes Actual", "Mes Anterior", "Mes Anterior 2" });
                            ldt.Columns["Servicio"].ColumnName = "Conceptos";
                            ldt.Columns["Descripcion"].ColumnName = "Descripcion";
                            ldt.Columns["Mes Actual"].ColumnName = "Mes seleccionado";
                            ldt.Columns["Mes Anterior"].ColumnName = "Mes anterior";
                            ldt.Columns["Mes Anterior 2"].ColumnName = "2 meses atrás";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Resumen por conceptos telcel

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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Mes seleccionado desc", 10), "Resumen por conceptos telcel",
                            new string[] { "Mes seleccionado", "Mes anterior", "2 meses atrás" },
                            new string[] { "Mes seleccionado", "Mes anterior", "2 meses atrás" },
                            new string[] { "Mes seleccionado", "Mes anterior", "2 meses atrás" },
                             "Conceptos", "", "", "Total", "$#,0.00",
                             true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "CenCosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Costo por colaborador / " + FCAndControls.AgregaEtiqueta(new string[] { param["CenCos"] }));

                        #region Reporte por empleado

                        DataTable RepConsXEmple = DSODataAccess.Execute(ConsultaCostoPorEmpleado(""));
                        if (RepConsXEmple.Rows.Count > 0)
                        {
                            DataView dvRepConsXEmple = new DataView(RepConsXEmple);
                            RepConsXEmple = dvRepConsXEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                            RepConsXEmple.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            RepConsXEmple.Columns["Total"].ColumnName = "Total";
                            RepConsXEmple.Columns["Renta"].ColumnName = "Renta";
                            RepConsXEmple.Columns["Diferencia"].ColumnName = "Monto excedido";
                            RepConsXEmple.AcceptChanges();
                        }

                        if (RepConsXEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(RepConsXEmple, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado

                        #region Grafico

                        DataTable GrafConsHist = DSODataAccess.Execute(ConsultaConsumoHistorico());
                        if (GrafConsHist.Rows.Count > 0)
                        {
                            DataView dvGrafConsHist = new DataView(GrafConsHist);
                            GrafConsHist = dvGrafConsHist.ToTable(false, new string[] { "Nombre Mes", "Total" });
                            GrafConsHist.Columns["Nombre Mes"].ColumnName = "Mes";
                            GrafConsHist.Columns["Total"].ColumnName = "Total";
                            GrafConsHist.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHist, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" },
                                       new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                       true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafico

                    }
                    else if (param["Nav"] == "EmpleN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Costo por colaborador");

                        #region Reporte por empleado

                        DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleado(""));
                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvRepConsXEmple = new DataView(ldt);
                            ldt = dvRepConsXEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Renta"].ColumnName = "Renta";
                            ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado

                        #region Grafica

                        if (ldt.Rows.Count > 0)
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Costo por colaborador", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "HistoricoAnioActVsAntN1" || param["Nav"] == "HistoricoPeEmN1") //20150204 AM. Se agrega exportacion de reporte historico de perfil empleado
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
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo histórico móviles año actual vs anterior");

                        #region Reporte historico anio actual vs anterior

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepHistoricoAnioActualVsAnteriorMoviles());

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "", new string[] { dosAnioAtras, anioAnterior, anioActual }, new string[] { dosAnioAtras, anioAnterior, anioActual },
                                        new string[] { dosAnioAtras, anioAnterior, anioActual }, "Mes", "", "", "Total", "$#,0.00",
                                        true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt,
                                0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte historico anio actual vs anterior
                    }
                    else if (param["Nav"] == "EmpleAcumN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Colaborador Acumulado");

                        #region Reporte por empleado

                        DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleAcumulado());
                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvRepConsXEmple = new DataView(ldt);
                            ldt = dvRepConsXEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Renta"].ColumnName = "Renta";
                            ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado

                        #region Grafica

                        if (ldt.Rows.Count > 0)
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Consumo Por Colaborador Acumulado", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "CenCosJerarquicoN1" || param["Nav"] == "CenCosJerarquicoN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por centro de costros jerárquico");

                        #region Reporte Jerarquico V2

                        int idCenCos;

                        if (param["Nav"] == "CenCosJerarquicoN1")
                        {
                            DataTable dtCenCos = DSODataAccess.Execute("SELECT CenCos FROM " + DSODataContext.Schema +
                                               ".vUsuario WHERE dtfinvigencia >= getdate() AND iCodCatalogo=" + Session["iCodUsuario"].ToString() + " AND CenCos IS NOT NULL");

                            idCenCos = (dtCenCos.Rows.Count == 0) ? 0 : Convert.ToInt32(dtCenCos.Rows[0][0]);
                        }
                        else { idCenCos = Convert.ToInt32(param["CenCos"]); }

                        if (idCenCos != 0)
                        {
                            DataTable ldtTabla = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico("", "", idCenCos));

                            if (ldtTabla.Rows.Count > 0)
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
                            DataTable ldtGrafica = DSODataAccess.Execute(ObtenerConsumoPorCenCosJerarquico("", "", idCenCos));

                            if (ldtGrafica.Rows.Count > 0)
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

                            ExportacionExcelRep.CrearGrafico(ldtGrafica, "Gráfica consumo por centro de costos jerárquico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Centro de costos", "Centro de costos", "", "Total", "$#,0.00",
                                             true, Microsoft.Office.Interop.Excel.XlChartType.xlBarClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        }

                        EstablecerBanderasClientePerfil(); //regresar a los valores originales a las variables de session.

                        #endregion Reporte Jerarquico V2
                    }
                    else if (param["Nav"] == "CenCosJerarquicoN3")
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
                            RepConsEmpleMasCaros = DSODataAccess.Execute(consultaPorEmpleMasCaros("", int.MaxValue));
                        }
                        else
                        {
                            //Reporte Dashboard
                            RepConsEmpleMasCaros = DSODataAccess.Execute(ConsultaReporteTopNEmpleDashboard("", "", 10));
                            RepConsEmpleMasCaros = DTIChartsAndControls.ordenaTabla(RepConsEmpleMasCaros, "Total DESC");
                        }

                        if (RepConsEmpleMasCaros.Rows.Count > 0)
                        {
                            DataView dvRepConsEmpleMasCaros = new DataView(RepConsEmpleMasCaros);
                            //NZ 20160823
                            //BG.20161113 se agrega columna de nomina
                            if (DSODataContext.Schema.ToLower() == "k5banorte" /*|| DSODataContext.Schema.ToLower() == "evox"*/)
                            {
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { "No Nomina", "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion", "Puesto" });

                                RepConsEmpleMasCaros.Columns["No Nomina"].ColumnName = "Nomina";
                            }
                            else
                            {
                                RepConsEmpleMasCaros = dvRepConsEmpleMasCaros.ToTable(false,
                                    new string[] { "Nombre Completo", "Nombre Centro de Costos", "Total", "Numero", "Duracion" });
                            }


                            RepConsEmpleMasCaros.Columns["Nombre Completo"].ColumnName = nombreColumnaColaborador;
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

                        if (RepConsEmpleMasCaros.Rows.Count > 0)
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

                        #endregion Reporte por empleado
                    }
                    else if (param["Nav"] == "Direccion")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Costo Por Área (Direcciones)");

                        #region Reporte Costo Por Area (Direcciones)

                        DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorAreaDireccion()); //""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "DescDireccion", "Importe", "Renta", "MontoExc" });
                            ldt.Columns["DescDireccion"].ColumnName = "Área";
                            ldt.Columns["Importe"].ColumnName = "Total";
                            ldt.Columns["MontoExc"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Costo Por Area (Direcciones)

                        #region Reporte Costo Por Area (Direcciones)(Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Área", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Costo Por Área (Direcciones)", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Área", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Costo Por Area (Direcciones)(Grafica)
                    }
                    else if (param["Nav"] == "SubDireccion")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Costo Por Área (SubDirecciones)");

                        #region Reporte Costo Por Area (SubDirecciones)

                        DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorAreaSubDireccion(""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "DescSubDireccion", "Importe", "Renta", "MontoExc" });
                            ldt.Columns["DescSubDireccion"].ColumnName = "Área";
                            ldt.Columns["Importe"].ColumnName = "Total";
                            ldt.Columns["MontoExc"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Costo Por Area (SubDirecciones)

                        #region Reporte Costo Por Area (SubDirecciones)(Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Área", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Área"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Costo Por Área (SubDirecciones)", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Área", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Costo Por Area (SubDirecciones)(Grafica)
                    }
                    else if (param["Nav"] == "EmpleadosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Colaborador");
                        string anioActual = DateTime.Now.Year.ToString();
                        string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
                        string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();

                        #region Reporte por empleado

                        DataTable RepConsumoEmpleFiltroCC = DSODataAccess.Execute(ConsultaConsumoEmpleFiltroCarrier(""));
                        if (RepConsumoEmpleFiltroCC.Rows.Count > 0)
                        {
                            DataView dvRepConsumoPorEmple = new DataView(RepConsumoEmpleFiltroCC);
                            RepConsumoEmpleFiltroCC = dvRepConsumoPorEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Nombre Carrier", "Linea", "Total", "Renta", "Diferencia" });
                            RepConsumoEmpleFiltroCC.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            RepConsumoEmpleFiltroCC.Columns["Total"].ColumnName = "Total";
                            RepConsumoEmpleFiltroCC.Columns["Renta"].ColumnName = "Renta";
                            RepConsumoEmpleFiltroCC.Columns["Diferencia"].ColumnName = "Monto excedido";
                            RepConsumoEmpleFiltroCC.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            RepConsumoEmpleFiltroCC.AcceptChanges();
                        }

                        if (RepConsumoEmpleFiltroCC.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(RepConsumoEmpleFiltroCC, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado

                        #region Grafico Consumo Historico Empleados

                        DataTable GrafConsHistEmple = DSODataAccess.Execute(ConsultaRepHistAnioActualVsAnteriorMovFiltroCC());
                        if (GrafConsHistEmple.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(GrafConsHistEmple);
                            GrafConsHistEmple = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                            if (GrafConsHistEmple.Rows[GrafConsHistEmple.Rows.Count - 1]["Mes"].ToString() == "Totales")
                            {
                                GrafConsHistEmple.Rows[GrafConsHistEmple.Rows.Count - 1].Delete();
                            }

                            GrafConsHistEmple.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHistEmple, "Consumo Histórico", new string[] { dosAnioAtras, anioAnterior, anioActual }, new string[] { dosAnioAtras, anioAnterior, anioActual },
                                    new string[] { dosAnioAtras, anioAnterior, anioActual }, "Mes", "", "", "Total", "$#,0.00",
                                    true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);


                        #endregion Grafico Consumo Historico Empleados
                    }
                    else if (param["Nav"] == "ConsumoPorCtaMaestra")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Cargos Telcel");

                        #region Reporte Consumo Agrupado por Cuenta Maestra

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoPorCtaMaestra(""));

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "CuentaPadre", "Total" });
                            ldt.Columns["CuentaPadre"].ColumnName = "Cuenta Padre";
                            ldt.Columns["Total"].ColumnName = "Importe";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");

                        #endregion Reporte Consumo Agrupado por Cuenta Maestra

                        #region Reporte Consumo Agrupado por Cuenta Maestra(Grafica)

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Cuenta Padre", "Importe" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Cuenta Padre"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(ldt, "Cargos Telcel", new string[] { "Importe" }, new string[] { "Importe" },
                                         new string[] { "Importe" }, "Cuenta Padre", "", "", "Importe", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Reporte Consumo Agrupado por Cuenta Maestra(Grafica)
                    }
                    else if (param["Nav"] == "ConsumoMovilPorTecno")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo De Moviles Por Tecnología");

                        #region Reporte

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoMovilesPorTecnologia(""));
                        RemoveColHerencia(ref ldt);

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvRepConsMovTecno = new DataView(ldt);
                            ldt = dvRepConsMovTecno.ToTable(false,
                                new string[] { "Tecnologia", "Total", "Renta", "Excedente" });
                            ldt.Columns["Tecnologia"].ColumnName = "Tecnologias";
                            ldt.Columns["Excedente"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte

                        #region Grafica

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Tecnologias", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Tecnologias"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Consumo De Moviles Por Tecnologia", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Tecnologias", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "CarrierCCN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Carrier");

                        #region Reporte

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepTabConsumoPorCarrierFiltroCC(""));
                        RemoveColHerencia(ref ldt);

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvRepConsMovTecno = new DataView(ldt);
                            ldt = dvRepConsMovTecno.ToTable(false,
                                new string[] { "Descripcion", "Total", "Renta", "Diferencia" });
                            ldt.Columns["Descripcion"].ColumnName = "Carrier";
                            ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte

                        #region Grafica

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false,
                                new string[] { "Carrier", "Total" });
                            if (ldt.Rows[ldt.Rows.Count - 1]["Carrier"].ToString() == "Totales")
                            {
                                ldt.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Consumo Por Carrier", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Carrier", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                    }
                    else if (param["Nav"] == "ConsumoLineasNoIdent")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo lineas no identificadas");

                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasNoIdent(""));

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


                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }


                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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

                            dtRepOriginal.AcceptChanges();
                        }


                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "Total desc", 10), "Consumo lineas no identificadas", new string[] { "Total" }, new string[] { "Total" },
                                          new string[] { "Total" }, "linea", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion


                    }

                    else if (param["Nav"] == "ConsumoCantLineasNuevas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Lineas Nuevas");

                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoCantLineasNuevas(""));

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
                                new string[] { "Linea", "Total", "Renta", "Diferencia" });
                            dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                            dtRepOriginal.Columns["Total"].ColumnName = "Total";
                            dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                            dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                            dtRepOriginal.AcceptChanges();
                        }

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10), "Consumo lineas Nuevas", new string[] { "value" }, new string[] { "Total" },
                                          new string[] { "Total" }, "label", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion


                    }

                    else if (param["Nav"] == "RepTabConsumoLineasSinAct2Pnls")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Lineas Sin Actividad");

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
                                new string[] { "Linea", "Total", "Renta", "Diferencia" });
                            dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                            dtRepOriginal.Columns["Total"].ColumnName = "Total";
                            dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                            dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                            dtRepOriginal.AcceptChanges();
                        }

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10), "Consumo lineas sin Actividad", new string[] { "value" }, new string[] { "Total" },
                                          new string[] { "Total" }, "label", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion


                    }
                    else if (param["Nav"] == "ConsumoLineasNoIdent")
                    {
                        ///20180829 RM 15.
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Lineas No Identificadas");
                        ///20180829 RM 15.

                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasNoIdent(""));

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


                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }


                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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

                            dtRepOriginal.AcceptChanges();
                        }


                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "Total desc", 10), "Consumo lineas no identificadas", new string[] { "Total" }, new string[] { "Total" },
                                          new string[] { "Total" }, "linea", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion


                    }

                    else if (param["Nav"] == "ConsumoCantLineasNuevas")
                    {
                        ///20180829 RM 17.
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Lineas Nuevas");
                        ///20180829 RM 17.
                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoCantLineasNuevas(""));

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
                                new string[] { "Linea", "Total", "Renta", "Diferencia" });
                            dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                            dtRepOriginal.Columns["Total"].ColumnName = "Total";
                            dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                            dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                            dtRepOriginal.AcceptChanges();
                        }

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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

                        ///20180829 RM 18.
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10), "Consumo lineas Nuevas", new string[] { "value" }, new string[] { "Total" },
                                          new string[] { "Total" }, "label", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        ///20180829 RM 18.
                        #endregion


                    }
                    ///20180829 RM 19
                    else if (param["Nav"] == "ConsumoLineasEnElMes")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Lineas Mes Anterior");

                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoLineasEnElMes(""));

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
                            ///20180829 RM 20.
                            dtRepOriginal = dvldt.ToTable(false,
                                new string[] { "Linea", "Total", "Renta", "Diferencia", "link", "iCodCatCarrier" });
                            ///20180829 RM 20.
                            dtRepOriginal.Columns["Linea"].ColumnName = "Linea";
                            dtRepOriginal.Columns["Total"].ColumnName = "Total";
                            dtRepOriginal.Columns["Renta"].ColumnName = "Renta";
                            dtRepOriginal.Columns["Diferencia"].ColumnName = "Monto excedido";
                            dtRepOriginal.AcceptChanges();
                        }

                        ///20180829 RM 21.
                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }
                        ///20180829 RM 21.


                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

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

                            dtRepOriginal.AcceptChanges();
                        }

                        ///20180829 RM 22.
                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        if (dtRepOriginal.Columns.Contains("iCodCatCarrier"))
                        {
                            dtRepOriginal.Columns.Remove("iCodCatCarrier");
                        }


                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "Total desc", 10), "Grafica consumo lineas en el mes", new string[] { "Total" }, new string[] { "Total" },
                                          new string[] { "Total" }, "linea", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        ///20180829 RM 22.
                        #endregion


                    }
                    else if (param["Nav"] == "ConsumoPorCentroDeCostos2Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por Centro de Costos");

                        DataTable dtReporte = new DataTable();
                        //dtReporte = DSODataAccess.Execute(ConsultaRepTabConsumoPorCentroDeCostos1Pnl());
                        dtReporte = BuscadtPruebaConsumoPorCentroDeCostos("");

                        #region Reporte 1Pnl

                        #region elimina Columnas para Exportacion
                        if (dtReporte.Columns.Contains("iCodCatCenCos"))
                        {
                            dtReporte.Columns.Remove("iCodCatCenCos");
                        }

                        if (dtReporte.Columns.Contains("cenCosNivel"))
                        {
                            dtReporte.Columns.Remove("cenCosNivel");
                        }

                        if (dtReporte.Columns.Contains("link"))
                        {
                            dtReporte.Columns.Remove("link");
                        }
                        #endregion

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

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                            DTIChartsAndControls.ordenaTabla(dtReporte, "Total desc"),
                            0, "Totales"), "Reporte", "Tabla");
                        }





                        #endregion Grid

                        #region Grafica

                        if (dtReporte.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dtReporte);
                            dtReporte = dvldt.ToTable(false, new string[] { "Centro de Costos", "Total" });
                            if (dtReporte.Rows[dtReporte.Rows.Count - 1]["Centro de Costos"].ToString() == "Totales")
                            {
                                dtReporte.Rows[dtReporte.Rows.Count - 1].Delete();
                            }
                            dtReporte.Columns["Centro de Costos"].ColumnName = "label";
                            dtReporte.Columns["Total"].ColumnName = "value";

                            dtReporte.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtReporte, "value desc", 10), "Consumo por Centro de Costos", new string[] { "value" }, new string[] { "Total" },
                                          new string[] { "Total" }, "label", "", "", "Total", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica

                        #endregion


                    }
                    else if (param["Nav"] == "DireccionJerarq")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por área (Direcciones)");

                        DataTable dtRepOriginal = DSODataAccess.Execute(ConsultaCostoPorAreaDireccion());

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
                        if (dtRepOriginal.Columns.Contains("idDireccion"))
                        {
                            dtRepOriginal.Columns.Remove("idDireccion");
                        }
                        #endregion

                        #region Reporte
                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dtRepOriginal);
                            dtRepOriginal = dvldt.ToTable(false,
                            new string[] { "DescDireccion", "Importe", "Renta", "MontoExc" });
                            dtRepOriginal.Columns["DescDireccion"].ColumnName = "Área";
                            dtRepOriginal.Columns["Importe"].ColumnName = "Total";
                            dtRepOriginal.Columns["MontoExc"].ColumnName = "Monto excedido";
                            dtRepOriginal.AcceptChanges();
                        }

                        ///20180829 RM 21.
                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }

                        ///20180829 RM 21.


                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dtRepOriginal, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion

                        #region Grafica
                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dtRepOriginal);
                            dtRepOriginal = dvldt.ToTable(false, new string[] { "Área", "Total" });
                            if (dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1]["Área"].ToString() == "Totales")
                            {
                                dtRepOriginal.Rows[dtRepOriginal.Rows.Count - 1].Delete();
                            }
                            dtRepOriginal.Columns["Área"].ColumnName = "label";
                            dtRepOriginal.Columns["Total"].ColumnName = "value";
                            dtRepOriginal.AcceptChanges();
                        }

                        ///20180829 RM 22.
                        if (dtRepOriginal.Columns.Contains("link"))
                        {
                            dtRepOriginal.Columns.Remove("link");
                        }
                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dtRepOriginal, "value desc", 10), "Grafica consumo lineas en el mes", new string[] { "value" }, new string[] { "Total" },
                                          new string[] { "value" }, "label", "", "", "value", "$#,0.00",
                                          true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        ///20180829 RM 22.
                        #endregion

                    }
                    else if (param["Nav"] == "LineasNuevasTelcel2pPnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Lineas Nuevas Telcel");
                        DataTable dt = DSODataAccess.Execute(ConsultaLineasNuevasTelcel());
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dt);
                            dt = dvldt.ToTable(false,
                            new string[] { "Extension", "Nombre Completo", "Total" });
                            dt.Columns["Extension"].ColumnName = "Línea";
                            dt.Columns["Nombre Completo"].ColumnName = "Responsable";

                            dt.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(dt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dt);
                            dt = dvldt.ToTable(false, new string[] { "Línea", "Total" });
                            if (dt.Rows[dt.Rows.Count - 1]["Línea"].ToString() == "Totales")
                            {
                                dt.Rows[dt.Rows.Count - 1].Delete();
                            }

                            dt.Columns["Línea"].ColumnName = "label";
                            dt.Columns["Total"].ColumnName = "value";
                            dt.AcceptChanges();


                            if (dt.Columns.Contains("link"))
                            {
                                dt.Columns.Remove("link");
                            }
                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10), "Lineas Nuevas Telcel", new string[] { "value" }, new string[] { "Total" },
                                              new string[] { "value" }, "label", "", "", "value", "$#,0.00",
                                              true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                    }
                    else if (param["Nav"] == "DistConsumoIntPorAppN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Distribución consumo internet por app");
                        DataTable dt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataView ldv = new DataView(dt);
                            ldv.Sort = "Importe desc, GB desc";
                            dt = ldv.ToTable(false, new string[] { "APP", "Importe", "GB" });
                            dt.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, 
                                DTIChartsAndControls.agregaTotales(
                                    DTIChartsAndControls.ordenaTabla(dt, "Importe desc"),0, "Totales"), "Reporte", "Tabla");

                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dt);
                            dt = dvldt.ToTable(false, new string[] { "APP", "Importe" });

                            dt.Columns["APP"].ColumnName = "label";
                            dt.Columns["Importe"].ColumnName = "value";
                            dt.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(dt, "value desc", 10), "Distribución consumo internet por app", new string[] { "value" }, new string[] { "Total" },
                                              new string[] { "value" }, "label", "", "", "value", "$#,0.00",
                                              true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);
                        }
                    }
                    else if(param["Nav"] == "HistGastoInternetN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Histórico gasto internet");
                        DataTable dt = DSODataAccess.Execute(ConsultaConsumoHistorico12Meses());
                        if(dt != null && dt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dt);
                            dt.Columns["Fecha"].ColumnName = "MES";
                            dt.Columns["GastoNacional"].ColumnName = "Gasto internet nal";
                            dt.Columns["GastoInternac"].ColumnName = "Gasto internet intl";
                            dt.Columns["GastoPaquetes"].ColumnName = "Gasto en paquetes";
                            dt.AcceptChanges();

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel,
                               DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");

                            if (dt.Rows[dt.Rows.Count - 1]["MES"].ToString() == "Totales")
                            {
                                dt.Rows[dt.Rows.Count - 1].Delete();
                            }
                            dt.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(dt, "Histórico gasto internet", new string[] { "Gasto internet nal", "Gasto internet intl", "Gasto en paquetes" }, new string[] { "Gasto internet nal", "Gasto internet intl", "Gasto en paquetes" }, new string[] { "Gasto internet nal", "Gasto internet intl","Gasto en paquetes" }, "MES", "", "", "Importes", "$#,0.00",
                                             true, Microsoft.Office.Interop.Excel.XlChartType.xlLineMarkers, lExcel, "{Grafica}", "Reporte", "DatosGr1", 450, 380);

                        }
                    }
                    else if(param["Nav"] == "RepUsoInternetTabs")
                    {
                        string label = (param["TipoConsumo"] == "Nac") ? "Consumo Internet Nacional" : "Consumo Internet Internacional";
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, label);
                        DataTable dt = DSODataAccess.Execute(ConsultaConsolidadoAplicacion());
                        if( dt!= null && dt.Rows.Count > 0)
                        {
                            DataTable dtCloned = dt.Clone();
                            dtCloned.Columns[2].DataType = typeof(string);
                            foreach (DataRow row in dt.Rows)
                            {
                                dtCloned.ImportRow(row);
                            }

                            DataView dvldt = new DataView(dtCloned);
                            dtCloned = DTIChartsAndControls.ordenaTabla(dtCloned, "IMPORTE Desc");
                            //dt.Columns["GB"].DataType= typeof(string);
                            dtCloned.AcceptChanges();
                            //new string[] { "", "{0:c}", "{0:0,0}" }
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel,
                               DTIChartsAndControls.agregaTotales(dtCloned, 0, "Totales"), "Reporte", "Tabla");

                            if (dtCloned.Rows[dtCloned.Rows.Count - 1]["App"].ToString() == "Totales")
                            {
                                dtCloned.Rows[dtCloned.Rows.Count - 1].Delete();
                            }
                            dtCloned.AcceptChanges();

                            ExportacionExcelRep.CrearGrafico(dtCloned, label, new string[] { "IMPORTE","GB" }, new string[] { "IMPORTE","GB"}, new string[] { "IMPORTE","GB"}
                                ,"APP", "", "", "GB", "",true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnStacked, lExcel, "{Grafica}", "Reporte", "DatosGr1", 450, 380);
                        }
                    }
                }

                #endregion Exportar reportes con tabla y grafica

                #region Exportar Reportes solo con tabla

                if (param["Nav"] == "EmpleN2" || param["Nav"] == "HistXEmpleN1" ||
                    param["Nav"] == "HistXCenCosN1" || param["Nav"] == "CostoXEmpleN1" || param["Nav"] == "DetFacPEmN2" || param["Nav"] == "LlamMasLargN1" ||
                    param["Nav"] == "LlamMasCostN1" || param["Nav"] == "LlamACelRedN2" || param["Nav"] == "LlamACelRedN3" || param["Nav"] == "CenCosN5" ||
                    param["Nav"] == "TopLineasMasCarasN2" || param["Nav"] == "TopLineasMasCarasN3" ||
                    param["Nav"] == "EmpleN3" || param["Nav"] == "EmpleN4" ||
                    param["Nav"] == "DetConsCol" || param["Nav"] == "MiConsumoInternet" || param["Nav"] == "ConsultaConcepto" || /* NZ 20150401 Reportes: DetConsCol, MiConsumoInternet, ConsultaConcepto */
                    param["Nav"] == "MinsUtilizadosN1" || /* BG.20150918 */
                    param["Nav"] == "RepDetalladoLlamsTelcelF4" /*BG*/   ||
                    param["Nav"] == "RepTabDetalladoFacturaTelcel" /*RM 20161209*/  ||
                    param["Nav"] == "RepDetalleLlamsMinsUtil" /* BG.20170224 */ ||
                    param["Nav"] == "EmpleMCN2" || param["Nav"] == "LineaTelcelCtaMaeN2" ||
                    param["Nav"] == "MiConsumoInternet2Pnl" || param["Nav"] == "ReporteGlobalTelMovil" ||
                    param["Nav"] == "ConsumoPorConcepto1Linea1Pnl" || param["Nav"] == "RepTabConsumoPorConcepto1LineaNueva1Pnl" ||
                    param["Nav"] == "ConsumoPorConcepto1Linea1PnlMesAnterior" || param["Nav"] == "RepUsoDeMinEnLineas" ||
                    param["Nav"] == "RepMatConsumoAcumHistPorSubdireccion" ||
                    param["Nav"] == "RepTabUsoDeMinEnLineas3Meses" ||
                    param["Nav"] == "RepTabConsumoDatosLineas1Mes" || param["Nav"] == "LlamadasMasCostosas1Pnl" || param["Nav"] == "LlamMasLargas1Pnl" ||
                    param["Nav"] == "LineasTelcelDadasDeBaja1pnl" || param["Nav"] == "LineasConGastoMensaje1Pnl" || param["Nav"] == "DetallaFactTelcel1Pnl" ||
                    param["Nav"] == "RepTabConsumoRegion" ||
                    param["Nav"] == "RepTabConsumoCuentaConcentradora" ||
                    param["Nav"] == "RepTabConsumoBranch" ||
                    param["Nav"] == "RepTabConsumoDepto" ||
                    param["Nav"] == "ReporteConsumoPorLinea" ||
                    param["Nav"] == "RepLineasTelcelSinActividad" || param["Nav"] == "RepLineasSinAct3MesesN2"|| param["Nav"] == "RepLineasExcedenteInternetN2"||
                    param["Nav"] == "RepLineasExcedenPlanBaseN2"|| param["Nav"] == "RepDesgloseTipoExcedenteLineasN3" ||
                    param["Nav"] == "RepInventarioLineasMoviles" ||
                    param["Nav"] == "LineasInactivas" || param["Nav"] == "PlanBaseLinea" || param["Nav"] == "LineasInactivasSinImporte"
                    )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "EmpleN2")
                    {
                        #region Seleccion de reporte

                        if (lsCvePerfil == "SupervisorDet" || lsCvePerfil == "Config" ||
                            //20150113 AM. Se agrega validacion para bimbo, el perfil administrador ve el reporte por concepto con nav a detalle
                            (DSODataContext.Schema.ToLower().Trim() == "bimbo" && Session["iCodPerfil"].ToString() == "369"))
                        {
                            //20150113 AM. Se agrego validacion para mandar directo a detalle si el empleado solo tiene una linea
                            int numeroDeLineasTelcel = ConsultaNumeroLineas("telcel");
                            if (numeroDeLineasTelcel > 1)
                            {
                                DataTable ldt = DSODataAccess.Execute(ConsultaRepTopLineasMasCaras("", "373")); //Telcel
                                if (ldt.Rows.Count > 0)
                                {
                                    DataView dvldt = new DataView(ldt);
                                    ldt = dvldt.ToTable(false,
                                        new string[] { "Extension", "NombreCompleto", "Total" });
                                    ldt.Columns["Extension"].ColumnName = "Línea";
                                    ldt.Columns["NombreCompleto"].ColumnName = "Colaborador";
                                    ldt.Columns["Total"].ColumnName = "Total";
                                    ldt.AcceptChanges();

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por línea");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"),
                                        "Reporte", "Tabla");
                                }
                            }
                            else
                            {
                                ConsultaLineaTelcel();

                                DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
                                if (RepDetalleXConcepto.Rows.Count > 0)
                                {
                                    DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                    RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                        new string[] { "Concepto", "Total" });
                                    RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                    RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                    RepDetalleXConcepto.AcceptChanges();

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"),
                                        "Reporte", "Tabla");
                                }
                            }
                        }
                        else
                        {
                            /*20150121 AM. Se agrega reporte historico*/
                            lExcel.Cerrar(true);
                            lExcel.Dispose();
                            lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTablaYGrafico" + lsExt);
                            lExcel.Abrir();

                            lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                            DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
                            if (RepDetalleXConcepto.Rows.Count > 0)
                            {
                                DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                                RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                    new string[] { "Concepto", "Total" });
                                RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                                RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                                RepDetalleXConcepto.AcceptChanges();

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"),
                                    "Reporte", "Tabla");
                            }

                            /*20150121 AM. Se agrega reporte historico*/
                            #region Reporte historico de empleado

                            DataTable ldtRepHistEmple = DSODataAccess.Execute(ConsultaConsumoHistoricoDelEmpleado(param["Emple"]));

                            if (ldtRepHistEmple.Rows.Count > 0)
                            {
                                DataView dvGrafConsHistDetFacPEmN1 = new DataView(ldtRepHistEmple);
                                ldtRepHistEmple = dvGrafConsHistDetFacPEmN1.ToTable(false, new string[] { "Nombre Mes", "Total" });
                                ldtRepHistEmple.Columns["Nombre Mes"].ColumnName = "Mes";
                                ldtRepHistEmple.Columns["Total"].ColumnName = "Total";
                                ldtRepHistEmple.AcceptChanges();

                                ExportacionExcelRep.CrearGrafico(ldtRepHistEmple, "Consumo histórico", new string[] { "Total" }, new string[] { "Total" }, new string[] { "Total" }, "Mes", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                            }

                            #endregion Reporte historico de empleado
                        }

                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "EmpleN3")
                    {
                        #region Reporte por concepto

                        DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
                        if (RepDetalleXConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                            RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                new string[] { "Concepto", "Total" });
                            RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                            RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                            RepDetalleXConcepto.AcceptChanges();

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por concepto
                    }
                    else if (param["Nav"] == "EmpleN4")
                    {
                        /*BG.20160317 se agrega validacion de esquema. Si se trata de nemak, agregara un nivel mas a su navegacion. */
                        if (DSODataContext.Schema.ToLower().Trim() != "nemak")
                        {
                            #region Seleccion de reporte

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

                                if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                                {
                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte por concepto");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                        0, "Totales"), "Reporte", "Tabla");
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

                                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                            DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                                            0, "Totales"), "Reporte", "Tabla");
                                    }
                                }
                            }

                            #endregion Seleccion de reporte

                        }
                        else
                        {

                            #region Seleccion de reporte desglose roaming

                            DataTable RepDesgloseFacturaPorConcepto = DSODataAccess.Execute(ConsultaConsumoDesgloseRoaming());

                            //20141205 AM. Se agrega condicion para validar que el datatable tenga datos
                            if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                            {

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

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                                        0, "Totales"), "Reporte", "Tabla");

                                    //return;
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

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por concepto");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                        0, "Totales"), "Reporte", "Tabla");


                                }
                                else if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() == "VER DESGLOSE")
                                {

                                    DataTable RepDesgloseConceptosRoaming = DSODataAccess.Execute(ConsultaDetalleFacturaRoamInternVoz());

                                    if (RepDesgloseConceptosRoaming.Rows.Count > 0)
                                    {
                                        DataView dvRepDesgloseConceptosRoaming = new DataView(RepDesgloseConceptosRoaming);
                                        RepDesgloseConceptosRoaming = dvRepDesgloseConceptosRoaming.ToTable(false,
                                            new string[] { "Concepto", "Total" });
                                        RepDesgloseConceptosRoaming.Columns["Total"].ColumnName = "Importe";
                                        RepDesgloseConceptosRoaming.AcceptChanges();
                                    }


                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Roaming Internacional Voz");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(RepDesgloseConceptosRoaming, "Importe desc"),
                                        0, "Totales"), "Reporte", "Tabla");

                                }
                            }

                            #endregion Seleccion de reporte desglose roaming
                        }
                    }
                    else if (param["Nav"] == "HistXEmpleN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo histórico por colaborador");

                        #region Reporte Consumo histórico por empleado

                        DataTable RepHistXEmple = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMat());
                        RemoveColHerencia(ref RepHistXEmple);

                        if (RepHistXEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(RepHistXEmple, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Consumo histórico por empleado
                    }
                    else if (param["Nav"] == "HistXCenCosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo histórico por área");

                        #region Reporte Consumo histórico por área

                        DataTable RepHistXCenCos = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMat());

                        #region Elimina columnas no necesarias en el gridview
                        RemoveColHerencia(ref RepHistXCenCos);
                        if (RepHistXCenCos.Columns.Contains("Codigo Centro de Costos"))
                            RepHistXCenCos.Columns.Remove("Codigo Centro de Costos");
                        #endregion // Elimina columnas no necesarias en el gridview

                        if (RepHistXCenCos.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepHistXCenCos, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Consumo histórico por área
                    }
                    else if (param["Nav"] == "CostoXEmpleN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por colaborador");

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



                            //RepCostoXEmple.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            RepCostoXEmple.Columns["Renta"].ColumnName = "Renta";
                            RepCostoXEmple.Columns["Diferencia"].ColumnName = "Monto excedido";
                            RepCostoXEmple.AcceptChanges();
                        }

                        if (RepCostoXEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepCostoXEmple, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado
                    }
                    else if (param["Nav"] == "DetFacPEmN2" || param["Nav"] == "CenCosN5")  //20141024 AM. Se agrega exportacion de reporte 
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo por colaborador");

                        #region Reporte Costo por Empleado

                        DataTable RepDesgloseFacturaPorConcepto = DSODataAccess.Execute(ConsultaConsumoDesglosadoPorConcepto());

                        if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDesgloseFacturaPorConcepto = new DataView(RepDesgloseFacturaPorConcepto);
                            RepDesgloseFacturaPorConcepto = dvRepDesgloseFacturaPorConcepto.ToTable(false,
                                new string[] { "Concepto", "Descripcion", "Total" });
                            RepDesgloseFacturaPorConcepto.Columns["Total"].ColumnName = "Importe";
                            RepDesgloseFacturaPorConcepto.AcceptChanges();
                        }
                        if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                        {
                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo desglosado por concepto");
                            if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                    DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                    0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        else
                        {
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

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas Telcel sin filtrar claves cargo");
                            if (RepDesgloseFacturaPorConcepto.Rows.Count > 0)
                            {
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"), 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        #endregion Reporte Costo por Empleado
                    }
                    else if (param["Nav"] == "LlamMasLargN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas mas largas");

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
                        if (RepLlamMasLarg.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepLlamMasLarg, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas mas largas
                    }
                    else if (param["Nav"] == "LlamMasCostN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas mas costosas");

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

                        if (RepLlamMasCostosas.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepLlamMasCostosas, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Llamadas mas costosas
                    }
                    else if (param["Nav"] == "LlamACelRedN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas a celulares de la red");

                        #region Reporte llamadas a celulares de la red

                        DataTable RepNumerosLlamACeluRed = DSODataAccess.Execute(ConsultaNumerosMarcadosACelRed(""));
                        if (RepNumerosLlamACeluRed.Rows.Count > 0)
                        {
                            DataView dvRepNumerosLlamACeluRed = new DataView(RepNumerosLlamACeluRed);
                            RepNumerosLlamACeluRed = dvRepNumerosLlamACeluRed.ToTable(false,
                                new string[] { "Numero Marcado", "Etiqueta", "Total", "Duracion", "Numero" });
                            RepNumerosLlamACeluRed.Columns["Numero Marcado"].ColumnName = "Número marcado";
                            RepNumerosLlamACeluRed.Columns["Etiqueta"].ColumnName = "Etiqueta";
                            RepNumerosLlamACeluRed.Columns["Total"].ColumnName = "Total";
                            RepNumerosLlamACeluRed.Columns["Duracion"].ColumnName = "Minutos";
                            RepNumerosLlamACeluRed.Columns["Numero"].ColumnName = "Llamadas";
                            RepNumerosLlamACeluRed.AcceptChanges();
                        }
                        if (RepNumerosLlamACeluRed.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepNumerosLlamACeluRed, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte llamadas a celulares de la red
                    }
                    else if (param["Nav"] == "LlamACelRedN3")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de llamadas");

                        #region Reporte detalle llamadas

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

                        if (RepDetalleLlamPerfEmple.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleLlamPerfEmple, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte detalle llamadas
                    }

                    else if (param["Nav"] == "TopLineasMasCarasN2")
                    {
                        #region Reporte Detalle

                        StringBuilder consultaEmpleadoEnBaseALinea = new StringBuilder();
                        consultaEmpleadoEnBaseALinea.Append("select isNull(Emple, 0) as Emple  \r");
                        consultaEmpleadoEnBaseALinea.Append("from  " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Lineas  \r");
                        consultaEmpleadoEnBaseALinea.Append("where dtinivigencia <> dtfinvigencia  \r");
                        consultaEmpleadoEnBaseALinea.Append("and dtfinvigencia >= getdate()  \r");
                        consultaEmpleadoEnBaseALinea.Append("and vchCodigo = '" + param["Linea"] + "' \r");
                        param["Emple"] = DSODataAccess.ExecuteScalar(consultaEmpleadoEnBaseALinea.ToString()).ToString();

                        DataTable RepDetalleXConcepto = DSODataAccess.Execute(ConsultaDetalleFactura());
                        if (RepDetalleXConcepto.Rows.Count > 0)
                        {
                            DataView dvRepDetalleXConcepto = new DataView(RepDetalleXConcepto);
                            RepDetalleXConcepto = dvRepDetalleXConcepto.ToTable(false,
                                new string[] { "Concepto", "Total" });
                            RepDetalleXConcepto.Columns["Concepto"].ColumnName = "Concepto";
                            RepDetalleXConcepto.Columns["Total"].ColumnName = "Total";
                            RepDetalleXConcepto.AcceptChanges();

                            ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura / " + param["Linea"]);
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleXConcepto, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Resumen Detalle
                    }

                    else if (param["Nav"] == "TopLineasMasCarasN3")
                    {
                        #region Seleccion de reporte

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


                            if (RepDesgloseFacturaPorConcepto.Rows[0]["Concepto"].ToString() != "VER DETALLE")
                            {
                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                     DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorConcepto, "Importe desc"),
                                     0, "Totales"), "Reporte", "Tabla");
                            }
                            else
                            {
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

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");
                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                         DTIChartsAndControls.ordenaTabla(RepDesgloseFacturaPorLlamada, "Total desc"),
                                         0, "Totales"), "Reporte", "Tabla");
                                }
                            }


                        }
                        #endregion Seleccion de reporte
                    }
                    else if (param["Nav"] == "MiConsumoInternet" || param["Nav"] == "MiConsumoInternet2Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle del consumo de Internet");

                        #region Reporte Detalle del consumo de Internet

                        int filtroLinea;
                        bool resultConvert = Int32.TryParse(param["Exten"], out filtroLinea);

                        DataTable RepConIntCol = DSODataAccess.Execute(ConsultaConsumoInternetPorColaborador(Convert.ToInt32(Session["iCodUsuario"]), (resultConvert) ? filtroLinea : 0));

                        // 20150504 Se manda llamar metodo para el calculo de las columnas.
                        bool banderaInternetIlimitado = CalculoDeColumnas(RepConIntCol);

                        if (RepConIntCol.Rows.Count > 0)
                        {
                            #region Acomodo de los indices de las columnas

                            //Se especifica que columnas se mostraran en el segun nivel dependiendo de esta bandera.
                            RepConIntCol.Columns.RemoveAt(9);
                            if (banderaInternetIlimitado)
                            {
                                RepConIntCol.Columns.RemoveAt(6);
                                RepConIntCol.Columns.RemoveAt(5);
                                RepConIntCol.Columns.RemoveAt(2);
                            }
                            #endregion Acomodo de los indices de las columnas

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConIntCol, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detalle del consumo de Internet
                    }
                    else if (param["Nav"] == "DetConsCol") //BG.20170301
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle del Consumo Histórico de Internet");

                        #region Reporte Detalle del Consumo Histórico de Internet

                        int filtroLinea;
                        bool resultConvert = Int32.TryParse(param["Exten"], out filtroLinea);

                        DataTable RepConHistIntCol = DSODataAccess.Execute(ConsultaConsumoHistoricoInternet(Convert.ToInt32(Session["iCodUsuario"]), (resultConvert) ? filtroLinea : 0));

                        // 20150504 Se manda llamar metodo para el calculo de las columnas.
                        bool banderaInternetIlimitado = CalculoDeColumnasRepHist(RepConHistIntCol);


                        DataView dvRepConHistIntCol = new DataView(RepConHistIntCol);

                        RepConHistIntCol.Columns[4].ColumnName = "Internet MB";
                        RepConHistIntCol = dvRepConHistIntCol.ToTable(false,
                            new string[] { "Fecha", "% Utilizado", "Internet Disponible (MB) ", "Consumo (MB)", "Excedio (MB)", "% Excedido", "Gasto Local", "Gasto Internacional" });

                        if (RepConHistIntCol.Rows.Count > 0)
                        {

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepConHistIntCol, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detalle del Consumo Histórico de Internet
                    }
                    else if (param["Nav"] == "ConsultaConcepto") //NZ 20150401   
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Lineas por concepto: " + string.Format("{0} - {1}", (string.IsNullOrEmpty(txtConcepto.Text)) ? txtConceptoDescripcion.ToolTip : txtConcepto.Text, txtConceptoDescripcion.Text));

                        #region Reporte de Lineas por concepto

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

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte de Lineas por concepto
                    }
                    else if (param["Nav"] == "RepDetalladoLlamsTelcelF4")
                    {
                        DataTable ldt = DSODataAccess.Execute(ConsultaDetalleLlamsTelcelF4());

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

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Detallados de Llamadas Telcel");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        #endregion Grid
                    }
                    else if (param["Nav"] == "MinsUtilizadosN1")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Minutos Utilizados");

                        #region Reporte Minutos Utilizados

                        DataTable RepMinsUtilizados = DSODataAccess.Execute(ConsultaRepMinutosUtilizados());
                        if (RepMinsUtilizados.Rows.Count > 0)
                        {
                            DataView dvRepMinsUtilizados = new DataView(RepMinsUtilizados);
                            RepMinsUtilizados = dvRepMinsUtilizados.ToTable(false,
                                new string[] { "Linea", "MinsDisp" });    //new string[] { "Linea", "MinsDisp", "MinsUtil" });
                            RepMinsUtilizados.Columns["Linea"].ColumnName = "Línea";
                            RepMinsUtilizados.Columns["MinsDisp"].ColumnName = "Minutos Disponibles";
                            //RepMinsUtilizados.Columns["MinsUtil"].ColumnName = "Minutos Utilizados";
                            RepMinsUtilizados.AcceptChanges();
                        }
                        if (RepMinsUtilizados.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepMinsUtilizados, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Minutos Utilizados
                    }
                    else if (param["Nav"] == "RepDetalleLlamsMinsUtil")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de Llamadas");

                        #region Detalle Llamadas (Minutos Utilizados)

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

                        if (RepDetalleMinsUtil.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleMinsUtil, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Detalle Llamadas (Minutos Utilizados)
                    }
                    else if (param["Nav"] == "RepTabDetalladoFacturaTelcel")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle Factura Telcel");

                        #region Reporte Detalle Factuta Telcel

                        DataTable RepDetalleFacturaTelcel = DSODataAccess.Execute(ConsultaDetalladoFacturaTelcel());
                        if (RepDetalleFacturaTelcel.Rows.Count > 0)
                        {
                            DataView dvRepDetalleFacturaTelcel = new DataView(RepDetalleFacturaTelcel);
                            RepDetalleFacturaTelcel = dvRepDetalleFacturaTelcel.ToTable(false,
                                new string[] { "Tel","Gente de Prosa","Centro de Costos","ID","No Centro de Costos","Nomina", "Servicios", "Servicios Adicionales",
                                    "Tiempo Aire Nacional", "Larga Distancia Nacional" ,"Tiempo Aire Roaming Nacional", "Larga Distancia Roaming Nacional",
                                    "Tiempo Aire Roaming Internacional","Larga Distancia Roaming Internacional", "Subtotal Excedente",
                                    "IVA Excedente", "Total Excedente","Total" });

                            RepDetalleFacturaTelcel.AcceptChanges();
                        }
                        if (RepDetalleFacturaTelcel.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetalleFacturaTelcel, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Detalle Factura Telcel
                    }
                    else if (param["Nav"] == "EmpleMCN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte detallado");

                        #region Reporte Detallado

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

                        if (RepDetallado.Columns.Contains("Duracion"))
                        {
                            RepDetallado.Columns["Duracion"].ColumnName = "Cantidad minutos";     //NZ 20161005 se cambia "Duracion" por "Cantidad minutos"
                        }
                        if (RepDetallado.Columns.Contains("Llamadas"))
                        {
                            RepDetallado.Columns["Llamadas"].ColumnName = "Cantidad llamadas";     //BG 20170814 se cambia "Llamadas" por "Cantidad Llamadas"
                        }

                        EliminarColumnasDeAcuerdoABanderas(RepDetallado);

                        if (RepDetallado.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepDetallado, 0, "Totales"), "Reporte", "Tabla");
                        }
                        #endregion Reporte Detallado
                    }
                    else if (param["Nav"] == "LineaTelcelCtaMaeN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Linea");

                        DataTable ldt = DSODataAccess.Execute(ConsultaRepConsumoLineaTelcelCtaMaestra());

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "CuentaPadre", "Linea", "Nomina", "Empleado", "CenCos",
                                                                      "Centro de Costo", "NoEmpresa", "Empresa", "Importe"});
                            ldt.Columns["CuentaPadre"].ColumnName = "Cuenta Padre";
                            ldt.Columns["Nomina"].ColumnName = "No Nomina";
                            ldt.Columns["CenCos"].ColumnName = "No CeCos";
                            ldt.Columns["NoEmpresa"].ColumnName = "No Empresa";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                    }
                    else if (param["Nav"] == "ReporteGlobalTelMovil")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Global Telefonía Móvil");

                        #region Reporte Global Telefonía Móvil

                        DataTable RepGlobalTelMov = DSODataAccess.Execute(ConsultaConsumoGlobalTelefoniaMovil());
                        if (RepGlobalTelMov.Rows.Count > 0)
                        {
                            DataView dvRepGlobalTelMov = new DataView(RepGlobalTelMov);
                            RepGlobalTelMov = dvRepGlobalTelMov.ToTable(false,
                                new string[] { "Direccion", "SubDireccion", "Nombre Completo", "Nombre Carrier", "Linea", "Total", "Renta", "Diferencia" });

                            RepGlobalTelMov.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            RepGlobalTelMov.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            RepGlobalTelMov.Columns["Diferencia"].ColumnName = "Monto excedido";
                            RepGlobalTelMov.AcceptChanges();
                        }

                        if (RepGlobalTelMov.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(RepGlobalTelMov, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Global Telefonía Móvil
                    }
                    else if (param["Nav"] == "ConsumoPorConcepto1Linea1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");

                        DataTable dtRepOriginal = new DataTable();
                        dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1Linea1Pnl(param["Tel"], param["Carrier"]));

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

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepOriginal, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion

                    }

                    else if (param["Nav"] == "RepTabConsumoPorConcepto1LineaNueva1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");

                        DataTable dtRepOriginal = new DataTable();
                        dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1LineaNueva1Pnl(param["Tel"], param["Carrier"]));

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

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepOriginal, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion

                    }
                    else if (param["Nav"] == "ConsumoPorConcepto1Linea1PnlMesAnterior")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detalle de factura");

                        DataTable dtRepOriginal = new DataTable();
                        dtRepOriginal = DSODataAccess.Execute(ConsultaConsumoPorConcepto1Linea1PnlMesAnterior(param["Tel"], param["Carrier"]));

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

                        if (dtRepOriginal.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepOriginal, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion

                    }
                    else if (param["Nav"] == "RepUsoDeMinEnLineas")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo de Minutos");

                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(GetMinLineas());

                        if (ldt.Rows.Count > 0)
                        {
                            RemoveColHerencia(ref ldt);
                            DataView dvldt = new DataView(ldt);
                            ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion
                    }
                    else if (param["Nav"] == "RepMatConsumoAcumHistPorSubdireccion")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Histórico por Subdirección");

                        #region Reporte Consumo Histórico
                        DataTable dtRepAñoActual = DSODataAccess.Execute(ConsultaRepMatConsumoAcumHistPorSubdireccion(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year));

                        if (dtRepAñoActual.Rows.Count > 0)
                        {
                            #region Elimina columnas no necesarias en el gridview
                            RemoveColHerencia(ref dtRepAñoActual);
                            if (dtRepAñoActual.Columns.Contains("CenCos"))
                                dtRepAñoActual.Columns.Remove("CenCos");

                            if (dtRepAñoActual.Columns.Contains("iCodCatDireccion"))
                            {
                                dtRepAñoActual.Columns.Remove("iCodCatDireccion");
                            }
                            if (dtRepAñoActual.Columns.Contains("iCodCatSubdireccion"))
                            {
                                dtRepAñoActual.Columns.Remove("iCodCatSubdireccion");
                            }
                            #endregion // Elimina columnas no necesarias en el gridview

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoActual, 0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte Consumo Histórico por dirección

                    }
                    else if (param["Nav"] == "RepTabUsoDeMinEnLineas3Meses")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo de Minutos");

                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(GetMinLineas3Meses());

                        if (ldt.Rows.Count > 0)
                        {
                            RemoveColHerencia(ref ldt);
                            DataView dvldt = new DataView(ldt);
                            ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Área";
                            ldt.AcceptChanges();
                        }

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion
                    }
                    else if (param["Nav"] == "RepTabConsumoDatosLineas1Mes")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo de Internet");

                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoDatosLineas1Mes());

                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            RemoveColHerencia(ref ldt);


                            if (ldt.Columns.Contains("FechaPub"))
                            {
                                ldt.Columns.Remove("FechaPub");
                            }


                            if (ldt.Columns.Contains("Nombre Carrier"))
                                ldt.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            if (ldt.Columns.Contains("Empleado"))
                                ldt.Columns["Empleado"].ColumnName = "Responsable";
                            if (ldt.Columns.Contains("CentroCostos"))
                                ldt.Columns["CentroCostos"].ColumnName = "Área";
                        }

                        DataTable dtCloned = ldt.Clone();

                        int i = 0;
                        foreach (DataColumn col in dtCloned.Columns)
                        {
                            if (col.ColumnName.Contains("%"))
                            {
                                dtCloned.Columns[i].DataType = typeof(string);
                            }
                            i++;
                        }


                        //foreach (DataRow row in ldt.Rows)
                        //{
                        //    dtCloned.ImportRow(row);
                        //}

                        //foreach (DataRow row in dtCloned.Rows)
                        //{
                        //    foreach (DataColumn col in dtCloned.Columns)
                        //    {
                        //        if (col.ColumnName.Contains("%"))
                        //        {
                        //            row[col] = (Convert.ToDouble(row[col].ToString()) * 100).ToString() + " %";
                        //        }
                        //    }

                        //}

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion
                    }
                    else if (param["Nav"] == "LlamadasMasCostosas1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas Mas Costosas");
                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(ConsultaLLamadasMasCostosas());
                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos", "Costo", "Dir Llamada", "Punta A", "Punta B" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Fecha Llamada"].ColumnName = "Fecha";
                            ldt.Columns["Hora Llamada"].ColumnName = "Hora";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Dir Llamada"].ColumnName = "Tipo";
                            ldt.Columns["Punta A"].ColumnName = "Localidad Origen";
                            ldt.Columns["Punta B"].ColumnName = "Localidad Destino";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion 
                    }
                    else if (param["Nav"] == "LlamMasLargas1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Llamadas Más Largas");
                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(ConsultaLlamMasLargas());
                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Numero Marcado", "Fecha Llamada", "Hora Llamada", "Duracion Minutos", "Costo", "Dir Llamada", "Punta A", "Punta B" });
                            ldt.Columns["Extension"].ColumnName = "Extensión";
                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Numero Marcado"].ColumnName = "Número Marcado";
                            ldt.Columns["Fecha Llamada"].ColumnName = "Fecha";
                            ldt.Columns["Hora Llamada"].ColumnName = "Hora";
                            ldt.Columns["Duracion Minutos"].ColumnName = "Minutos";
                            ldt.Columns["Costo"].ColumnName = "Total";
                            ldt.Columns["Dir Llamada"].ColumnName = "Tipo";
                            ldt.Columns["Punta A"].ColumnName = "Localidad Origen";
                            ldt.Columns["Punta B"].ColumnName = "Localidad Destino";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Minutos Desc");
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion 
                    }
                    else if (param["Nav"] == "LineasTelcelDadasDeBaja1pnl")
                    {

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Líneas dadas de Baja");
                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(ConsultaLineasTelcelBajas());
                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Nombre Centro de Costos" });
                            ldt.Columns["Extension"].ColumnName = "Línea";
                            ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costo";
                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Responsable Desc");
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
                        #endregion 
                    }
                    else if (param["Nav"] == "LineasConGastoMensaje1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Líneas dadas de Baja");
                        #region Reporte
                        DataTable ldt = DSODataAccess.Execute(ConsultaLineasConGastoMensaje());
                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldt);
                            ldt = dvldt.ToTable(false, new string[] { "Extension", "Nombre Completo", "Nombre Centro de Costos", "CantMens", "Total" });
                            ldt.Columns["Extension"].ColumnName = "Línea";
                            ldt.Columns["Nombre Completo"].ColumnName = "Responsable";
                            ldt.Columns["Nombre Centro de Costos"].ColumnName = "Centro de Costo";
                            ldt.Columns["CantMens"].ColumnName = "Cantidad de Mensajes";

                            ldt = DTIChartsAndControls.ordenaTabla(ldt, "Total Desc");
                            ldt.AcceptChanges();
                        }
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(ldt, 0, "Totales"), "Reporte", "Tabla");
                        #endregion 
                    }
                    else if (param["Nav"] == "DetallaFactTelcel1Pnl")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Detallado de Factura Telcel");

                        DataTable dt = DSODataAccess.Execute(ConsultaDetalladoFacTelcel());
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(dt);
                            dt = dvldt.ToTable(false, new string[] { "Linea", "PlanLinea", "Min Libres Pico", "Min Facturables Pico", "Min Libres No Pico", "Min Facturables No Pico", "Tiempo Aire Nacional"
                                                ,"Tiempo Aire Roaming Nac","Tiempo Aire Roaming Int","Larga Distancia Nac","Larga Distancia Roam Nac","Larga Distancia Roam Int","Servicios Adicionales","Desc Tiempo Aire","Desc Tiempo Aire Roam",
                                                    "Otros Desc","Cargos Creditos","Otros Serv","Otros Servicios","Roaming GPRS Internacional","Costo"});
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

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                    }
                    else if (param["Nav"] == "RepTabConsumoRegion")
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

                                dt = DTIChartsAndControls.ordenaTabla(dt, "Total Desc");
                                dt.AcceptChanges();

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Facturación consolidada");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "RepTabConsumoCuentaConcentradora")
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

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Distribución por cuenta concentradora");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "RepTabConsumoBranch")
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

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Distribución por Centro de Costos");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "RepTabConsumoDepto")
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

                                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Distribución por departamento");
                                ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "RepLineasTelcelSinActividad")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de lineas sin actividad Telcel");

                        #region Reporte

                        DataTable Rep = DSODataAccess.Execute(ConsRepLineasTelcelSinActividad());

                        if (Rep.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, Rep, "Reporte", "Tabla");
                        }

                        #endregion Reporte

                    }
                    else if (param["Nav"] == "RepInventarioLineasMoviles")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Inventario Lineas Moviles");

                        #region Reporte

                        DataTable Rep = DSODataAccess.Execute(ConsultaInventarioLineasMoviles());

                        if (Rep.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, Rep, "Reporte", "Tabla");
                        }

                        #endregion Reporte

                    }
                    else if (param["Nav"] == "LineasInactivas")
                    {
                        try
                        {
                            DataTable dt = DSODataAccess.Execute(ConsultaTablaMesesInactivos());

                            if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                            {
                                #region Reporte
                                DataView dataViewInactivos = dt.DefaultView;
                                dataViewInactivos.Sort = "Inactivos desc";
                                dt = dataViewInactivos.ToTable();

                                DataTable newDt = new DataTable();
                                newDt = dt.Clone();

                                // Prueba
                                newDt.Columns.Remove("TuvoActividadMesSelecMenos5");
                                newDt.Columns.Remove("TuvoActividadMesSelecMenos4");
                                newDt.Columns.Remove("TuvoActividadMesSelecMenos3");
                                newDt.Columns.Remove("TuvoActividadMesSelecMenos2");
                                newDt.Columns.Remove("TuvoActividadMesSelecMenos1");
                                newDt.Columns.Remove("TuvoActividadMesSelec");

                                newDt.Columns[0].DataType = typeof(String);

                                foreach (DataRow row in dt.Rows)
                                {
                                    newDt.ImportRow(row);
                                }

                                List<string> columnasTabla = new List<string>();
                                columnasTabla.Add("Linea");
                                columnasTabla.Add("Meses Inactivos");

                                for (int i = 5; i >= 0; i--)
                                {
                                    columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-i)));
                                }

                                string[] nuevoNombreColumnas = columnasTabla.ToArray();

                                for (int i = 0; i < newDt.Columns.Count; i++)
                                {
                                    newDt.Columns[i].ColumnName = nuevoNombreColumnas[i];
                                }

                                string[] campos = null;

                                if (newDt.Rows.Count > 0 && newDt.Columns.Count > 0)
                                {
                                    campos = nuevoNombreColumnas;
                                    DataView dvldt = new DataView(newDt);
                                    newDt = dvldt.ToTable(false, campos);
                                    newDt.AcceptChanges();

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de consumo para lineas con inactividad");

                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(newDt, "Total desc"),
                                        0, "Totales"), "Reporte", "Tabla");
                                }
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "PlanBaseLinea")
                    {
                        try
                        {
                            DataTable dt = DSODataAccess.Execute(ConsultaLineasPorPlanBase());

                            if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                            {
                                #region Reporte
                                if (dt.Rows.Count > 0)
                                {
                                    DataView dvldt = new DataView(dt);
                                    dt = dvldt.ToTable(false,
                                        new string[] { "Descripcion", "iCodCatalogo", "RentaTelefonia", "TotalMes3", "TotalMes2", "TotalMes1" });
                                    dt.Columns["Descripcion"].ColumnName = "Plan";
                                    dt.Columns["iCodCatalogo"].ColumnName = "Codigo Plan";
                                    dt.Columns["RentaTelefonia"].ColumnName = "Renta Base";
                                    dt.Columns["TotalMes3"].ColumnName = String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-2));
                                    dt.Columns["TotalMes2"].ColumnName = String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-1));
                                    dt.Columns["TotalMes1"].ColumnName = String.Format("{0:yyyy/MM}", fechaFinal);
                                    dt.AcceptChanges();
                                }

                                List<string> columnasTabla = new List<string>();
                                columnasTabla.Add("Plan");
                                columnasTabla.Add("Codigo Plan");
                                columnasTabla.Add("Renta Base");
                                columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-2)));
                                columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-1)));
                                columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal));

                                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                                {
                                    string[] campos = columnasTabla.ToArray();
                                    DataView dvldt = new DataView(dt);

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de consumo para lineas por plan base");

                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                        DTIChartsAndControls.ordenaTabla(dt, "Total desc"),
                                        0, "Totales"), "Reporte", "Tabla");
                                }
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "LineasInactivasSinImporte")
                    {
                        try
                        {
                            DataTable dt = DSODataAccess.Execute(ConsultaTablaMesesInactivosSinImporte());
                            List<string> columnasTabla = new List<string>();
                            string[] nuevoNombreColumnas;
                            string[] campos = null;

                            columnasTabla.Add("Linea");
                            //columnasTabla.Add("Meses Inactivos");

                            if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                            {
                                #region Reporte
                                DataView dataViewInactivos = dt.DefaultView;
                                //dataViewInactivos.Sort = "Inactivos desc";
                                dt = dataViewInactivos.ToTable();

                                DataTable newDt = new DataTable();
                                newDt = dt.Clone();

                                newDt.Columns[0].DataType = typeof(String);

                                foreach (DataRow row in dt.Rows)
                                {
                                    newDt.ImportRow(row);
                                }

                                if (fechaFinal.Day != 1 || fechaFinal.Day != 2)
                                {
                                    fechaFinal = fechaFinal.AddMonths(-1);
                                }

                                for (int i = 5; i >= 0; i--)
                                {
                                    columnasTabla.Add(String.Format("{0:yyyy/MM}", fechaFinal.AddMonths(-i)));
                                }

                                nuevoNombreColumnas = columnasTabla.ToArray();

                                for (int i = 0; i < newDt.Columns.Count; i++)
                                {
                                    newDt.Columns[i].ColumnName = nuevoNombreColumnas[i];
                                }

                                if (newDt.Rows.Count > 0 && newDt.Columns.Count > 0)
                                {
                                    campos = nuevoNombreColumnas;
                                    DataView dvldt = new DataView(newDt);
                                    newDt = dvldt.ToTable(false, campos);
                                    newDt.AcceptChanges();

                                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de lineas con inactividad mensual");

                                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.ordenaTabla(newDt, "Total desc"), "Reporte", "Tabla");
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (param["Nav"] == "ReporteConsumoPorLinea")
                    {

                        DataTable dtRepConsumo = DSODataAccess.Execute(ConsultaReporteConsumoFPorLinea());
                        DataView dtvRepConsumo = new DataView(dtRepConsumo);

                        string[] columnasTable = { "Tel", "NomCompleto", "Descripcion", "RentaLinea", "PlanLineaDesc", "MbIncluidosPlan", "MbConsumo", "LlamsConsumo" };
                        dtRepConsumo = dtvRepConsumo.ToTable(false, columnasTable);

                        #region Eliminar Columnas innecesarias

                        if (dtRepConsumo.Columns.Contains("Id")) { dtRepConsumo.Columns.Remove("Id"); }
                        if (dtRepConsumo.Columns.Contains("Linea")) { dtRepConsumo.Columns.Remove("Linea"); }
                        if (dtRepConsumo.Columns.Contains("Carrier")) { dtRepConsumo.Columns.Remove("Carrier"); }
                        if (dtRepConsumo.Columns.Contains("CarrierDesc")) { dtRepConsumo.Columns.Remove("CarrierDesc"); }
                        if (dtRepConsumo.Columns.Contains("PlanLinea")) { dtRepConsumo.Columns.Remove("PlanLinea"); }
                        if (dtRepConsumo.Columns.Contains("DatosNac")) { dtRepConsumo.Columns.Remove("DatosNac"); }
                        if (dtRepConsumo.Columns.Contains("TotalNac")) { dtRepConsumo.Columns.Remove("TotalNac"); }
                        if (dtRepConsumo.Columns.Contains("DatosInt")) { dtRepConsumo.Columns.Remove("DatosInt"); }
                        if (dtRepConsumo.Columns.Contains("TotalInt")) { dtRepConsumo.Columns.Remove("TotalInt"); }
                        if (dtRepConsumo.Columns.Contains("CantLlam")) { dtRepConsumo.Columns.Remove("CantLlam"); }
                        if (dtRepConsumo.Columns.Contains("TotalLlams")) { dtRepConsumo.Columns.Remove("TotalLlams"); }
                        if (dtRepConsumo.Columns.Contains("MbTolerancia")) { dtRepConsumo.Columns.Remove("MbTolerancia"); }
                        if (dtRepConsumo.Columns.Contains("LlamsTolerancia")) { dtRepConsumo.Columns.Remove("LlamsTolerancia"); }
                        if (dtRepConsumo.Columns.Contains("MbCriterio")) { dtRepConsumo.Columns.Remove("MbCriterio"); }
                        if (dtRepConsumo.Columns.Contains("LlamsCriterio")) { dtRepConsumo.Columns.Remove("LlamsCriterio"); }
                        if (dtRepConsumo.Columns.Contains("iCodCatEmple")) { dtRepConsumo.Columns.Remove("iCodCatEmple"); }
                        if (dtRepConsumo.Columns.Contains("iCodcatCenCos")) { dtRepConsumo.Columns.Remove("iCodcatCenCos"); }

                        #endregion

                        #region Cambia nombres Columnas

                        if (dtRepConsumo.Columns.Contains("Tel")) { dtRepConsumo.Columns["Tel"].ColumnName = "Linea"; }
                        if (dtRepConsumo.Columns.Contains("PlanLineaDesc")) { dtRepConsumo.Columns["PlanLineaDesc"].ColumnName = "Plan celular"; }
                        if (dtRepConsumo.Columns.Contains("RentaLinea")) { dtRepConsumo.Columns["RentaLinea"].ColumnName = "Renta"; }
                        if (dtRepConsumo.Columns.Contains("MbIncluidosPlan")) { dtRepConsumo.Columns["MbIncluidosPlan"].ColumnName = "Megas incluidos"; }
                        if (dtRepConsumo.Columns.Contains("MbConsumo")) { dtRepConsumo.Columns["MbConsumo"].ColumnName = "Megas consumidos"; }
                        if (dtRepConsumo.Columns.Contains("LlamsConsumo")) { dtRepConsumo.Columns["LlamsConsumo"].ColumnName = "Cantidad llamadas"; }
                        if (dtRepConsumo.Columns.Contains("NomCompleto")) { dtRepConsumo.Columns["NomCompleto"].ColumnName = "Responsable"; }
                        if (dtRepConsumo.Columns.Contains("Descripcion")) { dtRepConsumo.Columns["Descripcion"].ColumnName = "CC."; }

                        #endregion

                        List<string> listaformatos = new List<string>();

                        if (dtRepConsumo.Rows.Count > 0)
                        {



                            listaformatos = new List<string>();

                            for (int i = 1; i <= dtRepConsumo.Columns.Count; i++)
                            {
                                if (dtRepConsumo.Columns[i - 1].ColumnName.ToLower() == "renta")
                                {
                                    listaformatos.Add("{0:c}");
                                }
                                else if (dtRepConsumo.Columns[i - 1].ColumnName.ToLower() == "megas incluidos" || dtRepConsumo.Columns[i - 1].ColumnName.ToLower() == "megas consumidos" || dtRepConsumo.Columns[i - 1].ColumnName.ToLower() == "cantidad llamadas")
                                {
                                    listaformatos.Add("{0:0,0}");
                                }
                                else
                                {
                                    listaformatos.Add("");
                                }
                            }

                            if (dtRepConsumo.Rows.Count > 0 && dtRepConsumo.Columns.Count > 0)
                            {
                                //Elimina Nulos lo cambia por ceros
                                foreach (DataRow row in dtRepConsumo.Rows)
                                {

                                    foreach (DataColumn column in dtRepConsumo.Columns)
                                    {
                                        if (row[column] == DBNull.Value)
                                        {
                                            row[column] = 0;
                                        }
                                    }
                                }
                            }
                        }
                        #region Reporte

                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Consumo de datos por linea");
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dtRepConsumo, "Reporte", "Tabla");

                        #endregion

                    }
                }


             
                else if (param["Nav"] == "RepLineasSinAct3MesesN2")
                {
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte de Líneas sin actividad en los últimos 3 meses");
                    DataTable dt = DSODataAccess.Execute(ConsultaRepLineasSinAct3MesesN2());

                    if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                    {
                        //DataView dvldt = new DataView(dt);
                        //dt = dvldt.ToTable(false, new string[] { "DepartamentoDesc", "CenCosDesc", "SubTotal", "iva", "total" });

                        //dt.Columns["DepartamentoDesc"].ColumnName = "Línea";
                        //dt.Columns["CenCosDesc"].ColumnName = "Empleado ";
                        //dt.Columns["SubTotal"].ColumnName = "Centro de costos";
                        //dt.AcceptChanges();

                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                    }
                }
                else if(param["Nav"] == "RepLineasExcedenteInternetN2")
                {
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Líneas que excedieron su límite de internet");
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
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                    }
                }
                else if(param["Nav"]== "RepLineasExcedenPlanBaseN2")
                {
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Líneas que exceden su plan base");
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
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                    }
                }
                else if(param["Nav"] == "RepDesgloseTipoExcedenteLineasN3")
                {
                    ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Desglose por tipo de excedente");
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
                        ExportacionExcelRep.CreaTablaEnExcel(lExcel, dt, "Reporte", "Tabla");
                    }

                }
                #endregion Exportar Reportes solo con tabla

                #region Exportar Reportes con 2 Tablas
                //NZ 20150713 
                if (param["Nav"] == "RepMat" || param["Nav"] == "RepMatConsumoAcumHistPorDireccion")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteDosTablas" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "RepMat")   //NZ 20150713   
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Histórico por Área");

                        #region Reporte Consumo Histórico por Área
                        DataTable dtRepAñoActual = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAct());

                        if (dtRepAñoActual.Rows.Count > 0)
                        {
                            #region Elimina columnas no necesarias en el gridview
                            RemoveColHerencia(ref dtRepAñoActual);
                            if (dtRepAñoActual.Columns.Contains("Codigo Centro de Costos"))
                                dtRepAñoActual.Columns.Remove("Codigo Centro de Costos");
                            #endregion // Elimina columnas no necesarias en el gridview

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoActual, 0, "Totales"), "Reporte", "Tabla1");
                        }

                        DataTable dtRepAñoAnterior = DSODataAccess.Execute(ConsultaConsumoHistPorCenCosMatAnioAnt());

                        if (dtRepAñoAnterior.Rows.Count > 0)
                        {
                            #region Elimina columnas no necesarias en el gridview
                            RemoveColHerencia(ref dtRepAñoAnterior);
                            if (dtRepAñoAnterior.Columns.Contains("Codigo Centro de Costos"))
                                dtRepAñoAnterior.Columns.Remove("Codigo Centro de Costos");
                            #endregion // Elimina columnas no necesarias en el gridview

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoAnterior, 0, "Totales"), "Reporte", "Tabla2");
                        }

                        #endregion Reporte Consumo Histórico por Área
                    }
                    if (param["Nav"] == "RepMatConsumoAcumHistPorDireccion")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Histórico por Dirección");

                        #region Reporte Consumo Histórico
                        DataTable dtRepAñoActual = DSODataAccess.Execute(GetConsumoHistAcumPorDireccionUnAño(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year));

                        if (dtRepAñoActual.Rows.Count > 0)
                        {
                            #region Elimina columnas no necesarias en el gridview
                            RemoveColHerencia(ref dtRepAñoActual);
                            if (dtRepAñoActual.Columns.Contains("CenCos"))
                                dtRepAñoActual.Columns.Remove("CenCos");
                            #endregion // Elimina columnas no necesarias en el gridview

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoActual, 0, "Totales"), "Reporte", "Tabla1");
                        }

                        DataTable dtRepAñoAnterior = DSODataAccess.Execute(GetConsumoHistAcumPorDireccionUnAño(Convert.ToDateTime(Session["FechaInicio"].ToString()).Year - 1));

                        if (dtRepAñoAnterior.Rows.Count > 0)
                        {
                            #region Elimina columnas no necesarias en el gridview
                            RemoveColHerencia(ref dtRepAñoAnterior);
                            if (dtRepAñoAnterior.Columns.Contains("CenCos"))
                                dtRepAñoAnterior.Columns.Remove("CenCos");
                            #endregion // Elimina columnas no necesarias en el gridview

                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dtRepAñoAnterior, 0, "Totales"), "Reporte", "Tabla2");
                        }

                        #endregion Reporte Consumo Histórico por dirección
                    }
                }

                #endregion Exportar Reportes con 2 Tablas

                #region Exportacion 2 Reportes y Grafia
                if (param["Nav"] == "EmpleadosN2"
                   )
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte2TablaYGraficoMovil" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                    if (param["Nav"] == "EmpleadosN2")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Colaborador");

                        string anioActual = DateTime.Now.Year.ToString();
                        string anioAnterior = DateTime.Now.AddYears(-1).Year.ToString();
                        string dosAnioAtras = DateTime.Now.AddYears(-2).Year.ToString();

                        #region Reporte por empleado

                        DataTable RepConsumoEmpleFiltroCC = DSODataAccess.Execute(ConsultaConsumoEmpleFiltroCarrier(""));
                        if (RepConsumoEmpleFiltroCC.Rows.Count > 0)
                        {
                            DataView dvRepConsumoPorEmple = new DataView(RepConsumoEmpleFiltroCC);
                            RepConsumoEmpleFiltroCC = dvRepConsumoPorEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Nombre Carrier", "Total", "Renta", "Diferencia" });
                            RepConsumoEmpleFiltroCC.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            RepConsumoEmpleFiltroCC.Columns["Total"].ColumnName = "Total";
                            RepConsumoEmpleFiltroCC.Columns["Renta"].ColumnName = "Renta";
                            RepConsumoEmpleFiltroCC.Columns["Diferencia"].ColumnName = "Monto excedido";
                            RepConsumoEmpleFiltroCC.Columns["Nombre Carrier"].ColumnName = "Carrier";
                            RepConsumoEmpleFiltroCC.AcceptChanges();
                        }

                        if (RepConsumoEmpleFiltroCC.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(RepConsumoEmpleFiltroCC, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla1");
                        }

                        #endregion Reporte por empleado

                        #region Grafico Consumo Historico Empleados

                        DataTable GrafConsHistEmple = DSODataAccess.Execute(ConsultaRepHistAnioActualVsAnteriorMovFiltroCC());
                        if (GrafConsHistEmple.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(GrafConsHistEmple);
                            GrafConsHistEmple = dvldt.ToTable(false, new string[] { "Mes", dosAnioAtras, anioAnterior, anioActual });
                            if (GrafConsHistEmple.Rows[GrafConsHistEmple.Rows.Count - 1]["Mes"].ToString() == "Totales")
                            {
                                GrafConsHistEmple.Rows[GrafConsHistEmple.Rows.Count - 1].Delete();
                            }

                            GrafConsHistEmple.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(GrafConsHistEmple, "Consumo Histórico", new string[] { dosAnioAtras, anioAnterior, anioActual }, new string[] { dosAnioAtras, anioAnterior, anioActual },
                                    new string[] { dosAnioAtras, anioAnterior, anioActual }, "Mes", "", "", "Total", "$#,0.00",
                                    true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);


                        #endregion Grafico Consumo Historico Empleados


                        #region Tabla Hist por  colaborador
                        DataTable ldt = DSODataAccess.Execute(ConsultaConsumoHistoricoPorEmpleadoMatAnioAct());

                        if (ldt.Columns.Contains("RID"))
                            ldt.Columns.Remove("RID");
                        if (ldt.Columns.Contains("RowNumber"))
                            ldt.Columns.Remove("RowNumber");
                        if (ldt.Columns.Contains("TopRID"))
                            ldt.Columns.Remove("TopRID");

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla2");
                        }
                        #endregion



                    }





                }

                #endregion

                #region Exportación 2 Tablas y  3 Graficas
                if (param["Nav"] == "RepEmpleAcum")
                {
                    string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                    lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\Reporte2Tablasy3Graficas" + lsExt);
                    lExcel.Abrir();

                    lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                    if (param["Nav"] == "RepEmpleAcum")
                    {
                        ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Consumo Por Colaborador Acumulado");
                        #region Consumo x Emple Acum
                        #region Reporte por empleado

                        DataTable ldt = DSODataAccess.Execute(ConsultaCostoPorEmpleAcumulado());
                        if (ldt.Rows.Count > 0)
                        {
                            DataView dvRepConsXEmple = new DataView(ldt);
                            ldt = dvRepConsXEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                            ldt.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldt.Columns["Total"].ColumnName = "Total";
                            ldt.Columns["Renta"].ColumnName = "Renta";
                            ldt.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldt.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldt, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla");
                        }

                        #endregion Reporte por empleado

                        #region Grafica

                        if (ldt.Rows.Count > 0)
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

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldt, "Total desc", 10), "Consumo Por Colaborador Acumulado", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica}", "Reporte", "DatosGr1", 350, 300);

                        #endregion Grafica
                        #endregion
                        #region Consumo x Emple
                        #region Reporte por empleado

                        DataTable ldtxEmple = DSODataAccess.Execute(ConsultaCostoPorEmple());
                        if (ldtxEmple.Rows.Count > 0)
                        {
                            DataView dvRepConsXEmple = new DataView(ldtxEmple);
                            ldtxEmple = dvRepConsXEmple.ToTable(false,
                                new string[] { "Nombre Completo", "Linea", "Total", "Renta", "Diferencia" });
                            ldtxEmple.Columns["Nombre Completo"].ColumnName = "Colaborador";
                            ldtxEmple.Columns["Total"].ColumnName = "Total";
                            ldtxEmple.Columns["Renta"].ColumnName = "Renta";
                            ldtxEmple.Columns["Diferencia"].ColumnName = "Monto excedido";
                            ldtxEmple.AcceptChanges();
                        }

                        if (ldt.Rows.Count > 0)
                        {
                            ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(
                                DTIChartsAndControls.ordenaTabla(ldtxEmple, "Total desc"),
                                0, "Totales"), "Reporte", "Tabla2");
                        }

                        #endregion Reporte por empleado

                        #region Grafica

                        if (ldtxEmple.Rows.Count > 0)
                        {
                            DataView dvldt = new DataView(ldtxEmple);
                            ldtxEmple = dvldt.ToTable(false,
                                new string[] { "Colaborador", "Total" });
                            if (ldtxEmple.Rows[ldt.Rows.Count - 1]["Colaborador"].ToString() == "Totales")
                            {
                                ldtxEmple.Rows[ldt.Rows.Count - 1].Delete();
                            }
                            ldtxEmple.AcceptChanges();
                        }

                        ExportacionExcelRep.CrearGrafico(DTIChartsAndControls.selectTopNTabla(ldtxEmple, "Total desc", 10), "Consumo Por Colaborador Acumulado", new string[] { "Total" }, new string[] { "Total" },
                                         new string[] { "Total" }, "Colaborador", "", "", "Total", "$#,0.00",
                                         true, Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered, lExcel, "{Grafica2}", "Reporte", "DatosGr2", 350, 300);

                        #endregion Grafica
                        #endregion
                        #region Consumo x Emple por Mes

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

                            ExportacionExcelRep.CrearGrafico(dt, "", listaCol.ToArray(), listaCol.ToArray(),
                                            listaCol.ToArray(), "Mes", "", "", "Total", "$#,0.00",
                                            true, Microsoft.Office.Interop.Excel.XlChartType.xlLine, lExcel, "{Grafica3}", "Reporte", "DatosGr3", 650, 300);


                        }


                        #endregion
                    }

                }
                #endregion
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
                    Tabla.Columns.Remove("SM");
                    Tabla.Columns["TotalReal"].ColumnName = "Total";
                }
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