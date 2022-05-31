using KeytiaServiceBL;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public class ExportExcel
    {
        string rutaLogos = AppDomain.CurrentDomain.BaseDirectory + "\\images\\";
        public string GeneraArchivoExcel(string nameFiles, string fechaInicio1, string fechaFinal1, System.Data.DataTable dt1, System.Data.DataTable dt, int opc, int opc2)
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
                CreaHojas(xlWorkSheet, formatRange, fechaInicio1, fechaFinal1);
                EstiloEncabezadosTabla(xlWorkSheet, 11, "A");
                EstiloEncabezadosTabla(xlWorkSheet, 11, "D");
                CreaRow(numeroCelda, opc, dt1, dt, xlWorkSheet, opc2);

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
        private void CreaHojas(Worksheet xlWorkSheet, Range formatRange, string fechaInicio1, string fechaFinal1)
        {

            formatRange.NumberFormat = "@";
            formatRange.Font.Size = 12;
            formatRange.WrapText = true;
            formatRange.ColumnWidth = 33;
            string fechaIni = Convert.ToDateTime(fechaInicio1).ToString("dd") + " de " + Convert.ToDateTime(fechaInicio1).ToString("MMMM").ToUpper() + " de " + Convert.ToDateTime(fechaInicio1).ToString("yyyy");
            string fechaFin = Convert.ToDateTime(fechaFinal1).ToString("dd") + " de " + Convert.ToDateTime(fechaFinal1).ToString("MMMM").ToUpper() + " de " + Convert.ToDateTime(fechaFinal1).ToString("yyyy");
            xlWorkSheet.Cells[11, 1] = "Fecha Inicio: ";
            xlWorkSheet.Cells[11, 2] = fechaIni;
            xlWorkSheet.Cells[11, 3] = "Fecha Fin: ";
            xlWorkSheet.Cells[11, 4] = fechaFin;

            formatRange = xlWorkSheet.Range["A11:A11", "B11:B11"];
            if (File.Exists(rutaLogos + "LogoKeytiaRep.png"))
            {
                xlWorkSheet.Shapes.AddPicture(rutaLogos + "LogoKeytiaRep.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 280, 50);
            }
                         
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
        private void CreaRow(int numCelda, int numRep, System.Data.DataTable dt1, System.Data.DataTable dt, Worksheet xlWorkSheet, int opc2)
        {
            xlWorkSheet.Cells[16, 1] = "Sitio";
            xlWorkSheet.Cells[16, 2] = "Total";
            xlWorkSheet.Cells[16, 3] = "Costo Buzón";
            xlWorkSheet.Cells[16, 4] = "Cantidad Buzón";
            xlWorkSheet.Cells[16, 5] = "Cantidad Llamadas";
            xlWorkSheet.Cells[16, 6] = "Cantidad Minutos";

            EstiloEncabezadosTabla(xlWorkSheet, 16, "F");
            GeneraDatosTable(xlWorkSheet, dt, 17, 3, opc2);

        }
        private void GeneraDatosTable(Worksheet xlWorkSheet, System.Data.DataTable dt, int renglon, int opcion, int opc2)
        {
            object misValue = System.Reflection.Missing.Value;
            int totalRows = dt.Rows.Count;
            int totalCols = dt.Columns.Count;
            int rowDatos = renglon;
            Range formatRange;
            string letra = "";

            decimal totTotal = 0;
            decimal totalBuzon = 0;
            int totLlamBuzon = 0;
            int  totLlam= 0;
            int totMin = 0;
            
            var color1 = Color.FromArgb(220, 225, 231);
            var color2 = Color.White;
            letra = "F";
            foreach (DataRow dr in dt.Rows)
            {
                xlWorkSheet.Cells[rowDatos, 1] = dr["Descripcion"].ToString();
                xlWorkSheet.Cells[rowDatos, 2] = Convert.ToDecimal(dr["TotalGral"]);
                xlWorkSheet.Cells[rowDatos, 3] = Convert.ToDecimal(dr["TotSmart"]);
                xlWorkSheet.Cells[rowDatos, 4] = Convert.ToInt32(dr["TotLlamSmart"]);
                xlWorkSheet.Cells[rowDatos, 5] = Convert.ToInt32(dr["TotalLlamGral"]);
                xlWorkSheet.Cells[rowDatos, 6] = Convert.ToInt32(dr["TotMinGrla"]);

                formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                //var colorf = rowDatos % 2 == 0 ? color2 : color1;
                AllBorders(formatRange);
                //formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                totTotal += Convert.ToDecimal(dr["TotalGral"]);
                totalBuzon +=  Convert.ToDecimal(dr["TotSmart"]);
                totLlamBuzon += Convert.ToInt32(dr["TotLlamSmart"]);
                totLlam += Convert.ToInt32(dr["TotalLlamGral"]);
                totMin += Convert.ToInt32(dr["TotMinGrla"]);

                rowDatos++;
            }

            formatRange = xlWorkSheet.Range["A16", "F16"];
            formatRange.AutoFilter(1);

            xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
            xlWorkSheet.Cells[(totalRows + 17), 2] = totTotal;
            xlWorkSheet.Cells[(totalRows + 17), 3] = totalBuzon;
            xlWorkSheet.Cells[(totalRows + 17), 4] = totLlamBuzon;
            xlWorkSheet.Cells[(totalRows + 17), 5] = totLlam;
            xlWorkSheet.Cells[(totalRows + 17), 6] = totMin;

            formatRange = xlWorkSheet.Range["B17" + ":" + "B" + (totalRows + 17), "C17" + ":" + "C" + (totalRows + 17)];
            formatRange.NumberFormat = "$#,##0.00";

            formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
            formatRange.Font.Bold = true;
            formatRange.Font.Size = 11;
            formatRange.Font.Name = "Arial";

            formatRange = xlWorkSheet.Range["A16:F"+ (totalRows + 16)];
            formatRange.Sort(xlWorkSheet.Range["B16:B"+ (totalRows + 16)],
             XlSortOrder.xlDescending, misValue, misValue,
             XlSortOrder.xlDescending, misValue,
             XlSortOrder.xlDescending, XlYesNoGuess.xlNo,
             misValue, misValue, XlSortOrientation.xlSortColumns,
             XlSortMethod.xlStroke,
             XlSortDataOption.xlSortNormal,
             XlSortDataOption.xlSortNormal,
             XlSortDataOption.xlSortNormal
            );

            rowDatos = renglon;
            foreach (DataRow dr in dt.Rows)
            {
                formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                var colorf = rowDatos % 2 == 0 ? color2 : color1;
                AllBorders(formatRange);
                formatRange.Interior.Color = ColorTranslator.ToOle(colorf);
                rowDatos++;
            }
            //switch (opcion)
            //{
            //    case 1:
            //        if (opc2 == 2)
            //        {
            //            letra = "D";
            //        }
            //        else
            //        {
            //            letra = "E";
            //        }

            //        foreach (DataRow dr in dt.Rows)
            //        {

            //            xlWorkSheet.Cells[rowDatos, 1] = dr["DESCRIPCION"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 2] = dr["TOTALREAL"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 3] = Convert.ToInt32(dr["LLAMADAS"]);
            //            xlWorkSheet.Cells[rowDatos, 4] = Convert.ToInt32(dr["MINUTOS"]);
            //            if (opc2 == 1)
            //            {
            //                xlWorkSheet.Cells[rowDatos, 5] = Convert.ToInt32(dr["TOTALSITIOS"]);
            //                totExcedente += Convert.ToDecimal(dr["TOTALSITIOS"]);
            //            }


            //            formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
            //            var colorf = rowDatos % 2 == 0 ? color2 : color1;
            //            AllBorders(formatRange);
            //            formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

            //            cantEmples += Convert.ToInt32(dr["LLAMADAS"]);
            //            totRenta += Convert.ToDecimal(dr["MINUTOS"]);
            //            totTotal += Convert.ToDecimal(dr["TOTALREAL"]);
            //            rowDatos++;
            //        }

            //        formatRange = xlWorkSheet.Range["A16", "E16"];
            //        formatRange.AutoFilter(1);

            //        xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
            //        xlWorkSheet.Cells[(totalRows + 17), 2] = totTotal;
            //        xlWorkSheet.Cells[(totalRows + 17), 3] = cantEmples;
            //        xlWorkSheet.Cells[(totalRows + 17), 4] = totRenta;
            //        if (opc2 == 1)
            //        {
            //            xlWorkSheet.Cells[(totalRows + 17), 5] = totExcedente;
            //        }


            //        formatRange = xlWorkSheet.Range["B17" + ":" + "B" + (totalRows + 17), "B17" + ":" + "B" + (totalRows + 17)];
            //        formatRange.NumberFormat = "$#,##0.00";

            //        formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
            //        formatRange.Font.Bold = true;
            //        formatRange.Font.Size = 11;
            //        formatRange.Font.Name = "Arial";
            //        break;
            //    case 2:
            //        letra = "F";
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            xlWorkSheet.Cells[rowDatos, 1] = dr["CenCos"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 2] = dr["Puesto"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 3] = Convert.ToInt32(dr["CantEmples"]);
            //            xlWorkSheet.Cells[rowDatos, 4] = dr["Renta"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 5] = dr["Excedente"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 6] = dr["Total"].ToString();

            //            formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
            //            var colorf = rowDatos % 2 == 0 ? color2 : color1;
            //            AllBorders(formatRange);
            //            formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

            //            cantEmples += Convert.ToInt32(dr["CantEmples"]);
            //            totRenta += Convert.ToDecimal(dr["Renta"]);
            //            totExcedente += Convert.ToDecimal(dr["Excedente"]);
            //            totTotal += Convert.ToDecimal(dr["Total"]);
            //            rowDatos++;
            //        }

            //        formatRange = xlWorkSheet.Range["A16", "E16"];
            //        formatRange.AutoFilter(1);

            //        xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
            //        xlWorkSheet.Cells[(totalRows + 17), 3] = cantEmples;
            //        xlWorkSheet.Cells[(totalRows + 17), 4] = totRenta;
            //        xlWorkSheet.Cells[(totalRows + 17), 5] = totExcedente;
            //        xlWorkSheet.Cells[(totalRows + 17), 6] = totTotal;

            //        formatRange = xlWorkSheet.Range["D17" + ":" + "D" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
            //        formatRange.NumberFormat = "$#,##0.00";

            //        formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
            //        formatRange.Font.Bold = true;
            //        formatRange.Font.Size = 11;
            //        formatRange.Font.Name = "Arial";
            //        break;
            //    case 3:
            //        letra = "G";
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            xlWorkSheet.Cells[rowDatos, 1] = dr["Cencos"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 2] = dr["Puesto"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 3] = dr["Linea"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 4] = dr["NomCompleto"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 5] = dr["Renta"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 6] = dr["Excedente"].ToString();
            //            xlWorkSheet.Cells[rowDatos, 7] = dr["Total"].ToString();

            //            formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
            //            var colorf = rowDatos % 2 == 0 ? color2 : color1;
            //            AllBorders(formatRange);
            //            formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

            //            totRenta += Convert.ToDecimal(dr["Renta"]);
            //            totExcedente += Convert.ToDecimal(dr["Excedente"]);
            //            totTotal += Convert.ToDecimal(dr["Total"]);
            //            rowDatos++;
            //        }

            //        formatRange = xlWorkSheet.Range["A16", "G16"];
            //        formatRange.AutoFilter(1);

            //        xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
            //        xlWorkSheet.Cells[(totalRows + 17), 5] = totRenta;
            //        xlWorkSheet.Cells[(totalRows + 17), 6] = totExcedente;
            //        xlWorkSheet.Cells[(totalRows + 17), 7] = totTotal;

            //        formatRange = xlWorkSheet.Range["E17" + ":" + "E" + (totalRows + 17), letra + "17" + ":" + letra + (totalRows + 17)];
            //        formatRange.NumberFormat = "$#,##0.00";

            //        formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
            //        formatRange.Font.Bold = true;
            //        formatRange.Font.Size = 11;
            //        formatRange.Font.Name = "Arial";
            //        break;
            //    case 4:
            //        letra = "R";
            //        int numColum = dt.Columns.Count;
            //        int indiceColumna = 0;
            //        foreach (DataRow row in dt.Rows)  //Filas
            //        {
            //            indiceColumna = 0;

            //            foreach (DataColumn col in dt.Columns)  //Columnas
            //            {
            //                indiceColumna++;
            //                xlWorkSheet.Cells[rowDatos, indiceColumna] = row[col.ColumnName];
            //            }

            //            formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
            //            var colorf = rowDatos % 2 == 0 ? color2 : color1;
            //            AllBorders(formatRange);
            //            formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

            //            rowDatos++;
            //        }

            //        formatRange = xlWorkSheet.Range["L17" + ":" + "L" + (totalRows + 17), "L17" + ":" + "L" + (totalRows + 17)];
            //        formatRange.NumberFormat = "$#,##0.00";

            //        break;
            //    default:
            //        break;
            //}

            formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows + 17), letra + "16" + ":" + letra + (totalRows + 17)];
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            AllBorders(formatRange);

            if (opcion != 4)
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