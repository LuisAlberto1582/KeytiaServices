﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Excel;
using System.Data;
using KeytiaServiceBL;
using System.Drawing;

namespace KeytiaWeb.UserInterface
{
    public class ExportFile
    {
        string rutaLogos = AppDomain.CurrentDomain.BaseDirectory;
        public string GeneraArchivoExcel(string nameFiles,string fechaInicio1, string fechaFinal1, System.Data.DataTable dt1, System.Data.DataTable dt,int opc)
        {

            var path = Util.AppSettings("FolderTemp");
            string pathFile = "";
            pathFile = Path.Combine(path, nameFiles+"_"+DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Worksheet xlWorkSheet1;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            Range formatRange;
            int numeroCelda = 16;
            try
            {
                if (File.Exists(pathFile))
                    File.Delete(pathFile);
                formatRange = xlWorkSheet.Range["A11:A11", "E11:E11"];
                CreaHojas(xlWorkSheet, formatRange, fechaInicio1);
                EstiloEncabezadosTabla(xlWorkSheet, 11, "A");               
                CreaRow(numeroCelda, opc, dt1, dt, xlWorkSheet);
                if (opc == 2)
                {
                    xlWorkBook.Worksheets.Add();
                    xlWorkSheet1 = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
                    formatRange = xlWorkSheet1.Range["A11:A11", "O11:O11"];
                    CreaHojas(xlWorkSheet1, formatRange, fechaInicio1);
                    EstiloEncabezadosTabla(xlWorkSheet1, 11, "A");
                    CreaRow(numeroCelda, 4, dt1, dt, xlWorkSheet1);

                    /*CREA LA GRAFICA*/
                    ChartObjects chartObjs = (ChartObjects)xlWorkSheet1.ChartObjects();
                    ChartObject chartObj = chartObjs.Add(10, 310, 500, 300);
                    Chart xlChart = chartObj.Chart;
                    Range xValues = xlWorkSheet1.Range["B16", "N16"];
                    Range values = xlWorkSheet1.Range["B18", "N18"];
                    SeriesCollection seriesCollection = (SeriesCollection)xlChart.SeriesCollection();
                    Series series1 = seriesCollection.NewSeries();
                    series1.XValues = xValues;
                    series1.Values = values;
                    series1.Name = "Excedente";
                    xlChart.ChartType = XlChartType.xlLine;
                    xlChart.ChartWizard(Title: "Consumo Histórico");
                    releaseObject(xlWorkSheet1);
                }


                xlApp.DisplayAlerts = false;
                //xlApp.ActiveWindow.DisplayGridlines = false;
                xlWorkBook.SaveAs(pathFile, XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pathFile;
        }
        private void CreaHojas(Worksheet xlWorkSheet, Range formatRange,string fechaInicio1)
        {
            
            formatRange.NumberFormat = "@";
            formatRange.Font.Size = 12;
            formatRange.WrapText = true;
            formatRange.ColumnWidth = 33;
            string fechaIni = Convert.ToDateTime(fechaInicio1).ToString("dd") + " de " + Convert.ToDateTime(fechaInicio1).ToString("MMMM").ToUpper() + " de " + Convert.ToDateTime(fechaInicio1).ToString("yyyy");
            xlWorkSheet.Cells[11, 1] = "Fecha: ";
            xlWorkSheet.Cells[11, 2] = fechaIni;
            formatRange = xlWorkSheet.Range["A11:A11", "B11:B11"];

            DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] WITH(NOLOCK)" +
                                                " where Esquema = '" + DSODataContext.Schema + "'" +
                                                " and dtinivigencia <> dtfinVigencia " +
                                                " and dtfinVigencia>getdate()");
            // pRowCliente["LogoExportacion"].ToString().Replace("~/", "")
            //var pathLogo = rutaLogos+"bat.jpg";
            var lsImg = pRowCliente["LogoExportacion"].ToString().Replace("~/", "").Replace("/", "\\");
            if (lsImg.StartsWith("\\"))
            {
                lsImg = lsImg.Substring(1);
            }
            var pathLogo =  lsImg;
            //xlWorkSheet.Shapes.AddPicture(rutaLogos + "LogoKeytiaRep.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,0, 0, 280,50);
            //xlWorkSheet.Shapes.AddPicture(rutaLogos + "Komatsu.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue,300, 0, 250, 45);

            xlWorkSheet.Shapes.AddPicture(rutaLogos + "\\images\\LogoKeytiaRep.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 280, 75);
            xlWorkSheet.Shapes.AddPicture(rutaLogos + pathLogo, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 300, 0, 250, 70);

        }
        private void CreaRow(int numCelda,int numRep, System.Data.DataTable dt1, System.Data.DataTable dt, Worksheet xlWorkSheet)
        {
            switch (numRep)
            {
                case 1:
                    xlWorkSheet.Cells[16, 1] = "Departamento";
                    xlWorkSheet.Cells[16, 2] = "Cantidad de empleados";
                    xlWorkSheet.Cells[16, 3] = "Renta";
                    xlWorkSheet.Cells[16, 4] = "Excedente";
                    xlWorkSheet.Cells[16, 5] = "Total";
                    EstiloEncabezadosTabla(xlWorkSheet, 16, "E");
                    GeneraDatosTable(xlWorkSheet, dt, 17,1);
                    break;
                case 2:
                    /*agregar otra hoja para el historico del segundo nivel*/
                    xlWorkSheet.Cells[16, 1] = "Departamento";
                    xlWorkSheet.Cells[16, 2] = "Puesto";
                    xlWorkSheet.Cells[16, 3] = "Cantidad de empleados";
                    xlWorkSheet.Cells[16, 4] = "Renta";
                    xlWorkSheet.Cells[16, 5] = "Excedente";
                    xlWorkSheet.Cells[16, 6] = "Total";
                    EstiloEncabezadosTabla(xlWorkSheet, 16, "F");
                    GeneraDatosTable(xlWorkSheet, dt1, 17, 2);
                    break;
                case 3:
                    xlWorkSheet.Cells[16, 1] = "Departamento";
                    xlWorkSheet.Cells[16, 2] = "Puesto";
                    xlWorkSheet.Cells[16, 3] = "Línea";
                    xlWorkSheet.Cells[16, 4] = "Responsable";
                    xlWorkSheet.Cells[16, 5] = "Renta";
                    xlWorkSheet.Cells[16, 6] = "Excedente";
                    xlWorkSheet.Cells[16, 7] = "Total";
                    EstiloEncabezadosTabla(xlWorkSheet, 16, "G");
                    GeneraDatosTable(xlWorkSheet, dt, 17, 3);
                    break;
                case 4:
                    int indiceColumna = 0;
                    Range formatRange;
                    formatRange = xlWorkSheet.Range["A16","O16"];
                    formatRange.NumberFormat = "@";
                    foreach (DataColumn col in dt.Columns)  //Columnas
                    {
                        indiceColumna++;
                        xlWorkSheet.Cells[16, indiceColumna] = col.ColumnName;
                    }
                    EstiloEncabezadosTabla(xlWorkSheet, 16, "O");
                    GeneraDatosTable(xlWorkSheet, dt, 17, 4);

                    break;

            }

        }
        private void EstiloEncabezadosTabla(Worksheet xlWorkSheet,int r,string col)
        {
            Range formatRange;

            formatRange = xlWorkSheet.Range["A" + r, col + r];
            formatRange.Font.Bold = true;
            formatRange.NumberFormat = "@";
            formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
            formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            formatRange.Font.Size = 11;
            formatRange.Font.Name = "Arial";
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
        }
        private void GeneraDatosTable(Worksheet xlWorkSheet, System.Data.DataTable dt,int renglon,int opcion)
        {
            int totalRows = dt.Rows.Count;
            int totalCols = dt.Columns.Count;
            int rowDatos = renglon;
            Range formatRange;
            string letra="";
            int cantEmples=0;
            decimal totRenta=0;
            decimal totExcedente=0;
            decimal totTotal=0;
            var color1 = Color.FromArgb(220, 225, 231);
            var color2 = Color.White;
            switch (opcion)
            {
                case 1:
                    letra = "E";
                    foreach (DataRow dr in dt.Rows)
                    {
                        xlWorkSheet.Cells[rowDatos, 1] = dr["Cencos"].ToString();
                        xlWorkSheet.Cells[rowDatos, 2] = Convert.ToInt32(dr["CantEmples"]);
                        xlWorkSheet.Cells[rowDatos, 3] = dr["Renta"].ToString();
                        xlWorkSheet.Cells[rowDatos, 4] = dr["Excedente"].ToString();
                        xlWorkSheet.Cells[rowDatos, 5] = dr["Total"].ToString();

                        formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        cantEmples += Convert.ToInt32(dr["CantEmples"]);
                        totRenta += Convert.ToDecimal(dr["Renta"]);
                        totExcedente += Convert.ToDecimal(dr["Excedente"]);
                        totTotal += Convert.ToDecimal(dr["Total"]);
                        rowDatos++;
                    }

                    formatRange = xlWorkSheet.Range["A16", "E16"];
                    formatRange.AutoFilter(1);

                    xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
                    xlWorkSheet.Cells[(totalRows + 17), 2] = cantEmples;
                    xlWorkSheet.Cells[(totalRows + 17), 3] = totRenta;
                    xlWorkSheet.Cells[(totalRows + 17), 4] = totExcedente;
                    xlWorkSheet.Cells[(totalRows + 17), 5] = totTotal;

                    formatRange = xlWorkSheet.Range["C17" + ":" + "C" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";

                    formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
                    formatRange.Font.Bold = true;
                    formatRange.Font.Size = 11;
                    formatRange.Font.Name = "Arial";
                    break;
                case 2:
                    letra = "F";
                    foreach (DataRow dr in dt.Rows)
                    {
                        xlWorkSheet.Cells[rowDatos, 1] = dr["CenCos"].ToString();
                        xlWorkSheet.Cells[rowDatos, 2] = dr["Puesto"].ToString();
                        xlWorkSheet.Cells[rowDatos, 3] = Convert.ToInt32(dr["CantEmples"]);
                        xlWorkSheet.Cells[rowDatos, 4] = dr["Renta"].ToString();
                        xlWorkSheet.Cells[rowDatos, 5] = dr["Excedente"].ToString();
                        xlWorkSheet.Cells[rowDatos, 6] = dr["Total"].ToString();

                        formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        cantEmples += Convert.ToInt32(dr["CantEmples"]);
                        totRenta += Convert.ToDecimal(dr["Renta"]);
                        totExcedente += Convert.ToDecimal(dr["Excedente"]);
                        totTotal += Convert.ToDecimal(dr["Total"]);
                        rowDatos++;
                    }

                    formatRange = xlWorkSheet.Range["A16", "E16"];
                    formatRange.AutoFilter(1);

                    xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
                    xlWorkSheet.Cells[(totalRows + 17), 3] = cantEmples;
                    xlWorkSheet.Cells[(totalRows + 17), 4] = totRenta;
                    xlWorkSheet.Cells[(totalRows + 17), 5] = totExcedente;
                    xlWorkSheet.Cells[(totalRows + 17), 6] = totTotal;

                    formatRange = xlWorkSheet.Range["D17" + ":" + "D" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";

                    formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
                    formatRange.Font.Bold = true;
                    formatRange.Font.Size = 11;
                    formatRange.Font.Name = "Arial";
                    break;
                case 3:
                    letra = "G";
                    foreach (DataRow dr in dt.Rows)
                    {
                        xlWorkSheet.Cells[rowDatos, 1] = dr["Cencos"].ToString();
                        xlWorkSheet.Cells[rowDatos, 2] = dr["Puesto"].ToString();
                        xlWorkSheet.Cells[rowDatos, 3] = dr["Linea"].ToString();
                        xlWorkSheet.Cells[rowDatos, 4] = dr["NomCompleto"].ToString();
                        xlWorkSheet.Cells[rowDatos, 5] = dr["Renta"].ToString();
                        xlWorkSheet.Cells[rowDatos, 6] = dr["Excedente"].ToString();
                        xlWorkSheet.Cells[rowDatos, 7] = dr["Total"].ToString();

                        formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        totRenta += Convert.ToDecimal(dr["Renta"]);
                        totExcedente += Convert.ToDecimal(dr["Excedente"]);
                        totTotal += Convert.ToDecimal(dr["Total"]);
                        rowDatos++;
                    }

                    formatRange = xlWorkSheet.Range["A16", "G16"];
                    formatRange.AutoFilter(1);

                    xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
                    xlWorkSheet.Cells[(totalRows + 17), 5] = totRenta;
                    xlWorkSheet.Cells[(totalRows + 17), 6] = totExcedente;
                    xlWorkSheet.Cells[(totalRows + 17), 7] = totTotal;

                    formatRange = xlWorkSheet.Range["E17" + ":" + "E" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";

                    formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
                    formatRange.Font.Bold = true;
                    formatRange.Font.Size = 11;
                    formatRange.Font.Name = "Arial";
                    break;
                case 4:
                    letra = "O";
                    int numColum = dt.Columns.Count;
                    int indiceColumna = 0;
                    foreach (DataRow row in dt.Rows)  //Filas
                    {
                        indiceColumna = 0;

                        foreach (DataColumn col in dt.Columns)  //Columnas
                        {
                            indiceColumna++;
                            xlWorkSheet.Cells[rowDatos, indiceColumna] = row[col.ColumnName];
                        }

                        formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        rowDatos++;
                    }

                    formatRange = xlWorkSheet.Range["B17" + ":" + "E" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";  
                    
                    break;
                default:
                    break;
            }

            formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows+17), letra + "16" + ":" + letra + (totalRows+17)];
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            AllBorders(formatRange);

            if(opcion != 4)
            {
                formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
                formatRange.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            }
           
        }
        private void AllBorders(Range formatRange)
        {
            formatRange.Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            formatRange.BorderAround(XlLineStyle.xlContinuous,
               XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
               XlColorIndex.xlColorIndexAutomatic);
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}