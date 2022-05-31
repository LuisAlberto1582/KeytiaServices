using KeytiaServiceBL;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public class ExportaFiles
    {

            string rutaLogos = AppDomain.CurrentDomain.BaseDirectory + "\\images\\";
            public string GeneraArchivoExcel(string nameFiles, System.Data.DataTable dt1)
            {

                var path = Util.AppSettings("FolderTemp");
                string pathFile = "";
                pathFile = Path.Combine(path, nameFiles + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Application xlApp;
                Workbook xlWorkBook;
                Worksheet xlWorkSheet;
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
                    CreaHojas(xlWorkSheet, formatRange);
                    EstiloEncabezadosTabla(xlWorkSheet, 11, "A");
                    CreaRow(numeroCelda, dt1, xlWorkSheet);

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
            private void CreaHojas(Worksheet xlWorkSheet, Range formatRange)
            {

                formatRange.NumberFormat = "@";
                formatRange.Font.Size = 12;
                formatRange.WrapText = true;
                formatRange.ColumnWidth = 33;
                formatRange = xlWorkSheet.Range["A11:A11", "B11:B11"];
                if (File.Exists(rutaLogos + "LogoKeytiaRep.png"))
                {
                    xlWorkSheet.Shapes.AddPicture(rutaLogos + "LogoKeytiaRep.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 280, 50);
                }

            }
            private void CreaRow(int numCelda, System.Data.DataTable dt, Worksheet xlWorkSheet)
            {

                int indiceCol = 0;
                Range formatRange1;
                int cantCol = dt.Columns.Count;
                formatRange1 = xlWorkSheet.Range[xlWorkSheet.Cells[16, 1], xlWorkSheet.Cells[16, cantCol]];
                formatRange1.NumberFormat = "@";
                EstiloEncabezadosTabla2(xlWorkSheet, 16, cantCol);

                foreach (DataColumn col in dt.Columns)  //Columnas
                {
                    indiceCol++;
                    xlWorkSheet.Cells[16, indiceCol] = col.ColumnName;
                }

                GeneraDatosTable(xlWorkSheet, dt, 17);
            }
            private void EstiloEncabezadosTabla(Worksheet xlWorkSheet, int r, string col)
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
            private void EstiloEncabezadosTabla2(Worksheet xlWorkSheet, int r, int col)
            {
                Range formatRange;
                formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[r, 1], xlWorkSheet.Cells[16, col]];
                //formatRange = xlWorkSheet.Range["A" + r, col + r];
                formatRange.Font.Bold = true;
                formatRange.NumberFormat = "@";
                formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                formatRange.Font.Size = 11;
                formatRange.Font.Name = "Arial";
                formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            }
            private void GeneraDatosTable(Worksheet xlWorkSheet, System.Data.DataTable dt, int renglon)
            {
                int totalRows = dt.Rows.Count;
                int totalCols = dt.Columns.Count;
                int rowDatos = renglon;
                Range formatRange;
                var color1 = Color.FromArgb(220, 225, 231);
                var color2 = Color.White;

                int numeroColum = dt.Columns.Count;
                int indiceColumn = 0;

                foreach (DataRow row in dt.Rows)  //Filas
                {
                    indiceColumn = 0;

                    foreach (DataColumn col in dt.Columns)  //Columnas
                    {
                        indiceColumn++;
                        double value;
                        var valor = row[col.ColumnName];
                        bool suc = double.TryParse(valor.ToString(), out value);
                        formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[rowDatos, indiceColumn], xlWorkSheet.Cells[rowDatos, indiceColumn]];
                        formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        xlWorkSheet.Cells[rowDatos, indiceColumn] = row[col.ColumnName].ToString().Replace("'", "");

                    }

                    formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[rowDatos, 1], xlWorkSheet.Cells[rowDatos, numeroColum]];
                    var colorf = rowDatos % 2 == 0 ? color2 : color1;
                    AllBorders(formatRange);
                    formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                    rowDatos++;
                }

                formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[16, 1], xlWorkSheet.Cells[(totalRows + 17), numeroColum]];
                AllBorders(formatRange);

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