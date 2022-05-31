using System;
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
    public class ExportFiles
    {
        string rutaLogos = AppDomain.CurrentDomain.BaseDirectory + "\\images\\";
        public string GeneraArchivoExcel(string nameFiles,string fechaInicio1, string fechaFinal1, System.Data.DataTable dt1, System.Data.DataTable dt,int opc,int opc2)
        {

            var path = Util.AppSettings("FolderTemp");
            string pathFile = "";
            pathFile = Path.Combine(path, nameFiles+"_"+DateTime.Now.ToString("yyyyMMdd") + ".xls");
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
                //EstiloEncabezadosTabla(xlWorkSheet, 11, "D");
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
        private void CreaHojas(Worksheet xlWorkSheet, Range formatRange,string fechaInicio1,string fechaFinal1)
        {
            
            formatRange.NumberFormat = "@";
            formatRange.Font.Size = 12;
            formatRange.WrapText = true;
            formatRange.ColumnWidth = 33;
            if(fechaInicio1 != "" && fechaInicio1 != null)
            {
                string fechaIni = Convert.ToDateTime(fechaInicio1).ToString("MMMM").ToUpper() + " DE " + Convert.ToDateTime(fechaInicio1).ToString("yyyy");
                xlWorkSheet.Cells[11, 1] = "Periodo de Facturación: ";
                xlWorkSheet.Cells[11, 2] = fechaIni;
            }
            formatRange = xlWorkSheet.Range["A11:A11", "B11:B11"];
            if(File.Exists(rutaLogos + "LogoKeytiaRep.png"))
            {
                xlWorkSheet.Shapes.AddPicture(rutaLogos + "LogoKeytiaRep.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 280, 50);               
            }

        }
        private void CreaRow(int numCelda,int numRep, System.Data.DataTable dt1, System.Data.DataTable dt, Worksheet xlWorkSheet,int opc2)
        {
            switch (numRep)
            {
                case 1:

                    string titu = "Centro de Costos";
                    if (dt.Columns.Contains("Plazas"))
                    {
                        titu = "Plazas";
                    }
                    else if (dt.Columns.Contains("Mercados"))
                    {
                        titu = "Mercados";
                    }
                    else if (dt.Columns.Contains("Tiendas"))
                    {
                        titu = "Tiendas";
                    }
                    else if (dt.Columns.Contains("Conceptos"))
                    {
                        titu = "Conceptos";
                    }
                    if (opc2 == 3)
                    {
                        titu = "Tipo de Destino";
                    }
                    xlWorkSheet.Cells[16, 1] = titu;

                    xlWorkSheet.Cells[16, 2] = "Total";
                    xlWorkSheet.Cells[16, 3] = "Cantidad de Llamadas";
                    xlWorkSheet.Cells[16, 4] = "Cantidad de Minutos";
                    if (opc2 ==2)
                    {
                        EstiloEncabezadosTabla(xlWorkSheet, 16, "D");                       
                    }
                    else if(opc2 == 1)
                    {
                        xlWorkSheet.Cells[16, 5] = "Cantidad de Sitios";
                        EstiloEncabezadosTabla(xlWorkSheet, 16, "E");

                    }
                    

                    GeneraDatosTable(xlWorkSheet, dt, 17, 1, opc2);

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
                    GeneraDatosTable(xlWorkSheet, dt1, 17, 2, opc2);
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
                    GeneraDatosTable(xlWorkSheet, dt, 17, 3, opc2);
                    break;
                case 4:
                    int indiceColumna = 0;
                    Range formatRange;
                    int cantCols = dt.Columns.Count;
                    string colFin="";

                    if(cantCols == 13)
                    {
                        colFin = "M";
                    }
                    else
                    {
                        colFin = "H";
                    }

                    formatRange = xlWorkSheet.Range["A16", colFin +"16"];
                    formatRange.NumberFormat = "@";
                    EstiloEncabezadosTabla(xlWorkSheet, 16, colFin);

                    foreach (DataColumn col in dt.Columns)  //Columnas
                    {
                        indiceColumna++;
                        xlWorkSheet.Cells[16, indiceColumna] = col.ColumnName;
                    }
                   
                    GeneraDatosTable(xlWorkSheet, dt, 17, 4, opc2);

                    break;
                case 5:
                        int indiceCol = 0;
                        Range formatRange1;
                        int cantCol = dt.Columns.Count;
                        string colFin1 = "";
                        int rowFin = dt.Rows.Count;
                        if (cantCol == 13)
                        {
                            colFin = "M";
                        }
                        else
                        {
                            colFin = "H";
                        }

                        formatRange1 = xlWorkSheet.Range[xlWorkSheet.Cells[16, 1], xlWorkSheet.Cells[16, cantCol]];
                    //formatRange1 = xlWorkSheet.Range[1, colFin ];
                        formatRange1.NumberFormat = "@";
                        EstiloEncabezadosTabla2(xlWorkSheet, 16, cantCol);

                        foreach (DataColumn col in dt.Columns)  //Columnas
                        {
                            indiceCol++;
                            xlWorkSheet.Cells[16, indiceCol] = col.ColumnName;
                        }

                        GeneraDatosTable(xlWorkSheet, dt, 17, 5, opc2);
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
        private void GeneraDatosTable(Worksheet xlWorkSheet, System.Data.DataTable dt,int renglon,int opcion,int opc2)
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
                    if(opc2 ==2)
                    {
                        letra = "D";
                    }
                    else
                    {
                        letra = "E";
                    }
                    
                    foreach (DataRow dr in dt.Rows)
                    {

                        if(dt.Columns.Contains("Plazas"))
                        { 
                            xlWorkSheet.Cells[rowDatos, 1] = dr["Plazas"].ToString(); 
                        }
                        else if(dt.Columns.Contains("Mercados"))
                        {
                            xlWorkSheet.Cells[rowDatos, 1] = dr["Mercados"].ToString();
                        }
                        else if(dt.Columns.Contains("Tiendas"))
                        {
                            xlWorkSheet.Cells[rowDatos, 1] = dr["Tiendas"].ToString();
                        }
                        else if (dt.Columns.Contains("Conceptos"))
                        {
                            xlWorkSheet.Cells[rowDatos, 1] = dr["Conceptos"].ToString();
                        }

                        xlWorkSheet.Cells[rowDatos, 2] = dr["TOTALREAL"].ToString();
                        xlWorkSheet.Cells[rowDatos, 3] = Convert.ToInt32(dr["LLAMADAS"]);
                        xlWorkSheet.Cells[rowDatos, 4] = Convert.ToInt32(dr["MINUTOS"]);
                        if (opc2 == 1)
                        {
                            xlWorkSheet.Cells[rowDatos, 5] = Convert.ToInt32(dr["TOTALSITIOS"]);
                            totExcedente += Convert.ToDecimal(dr["TOTALSITIOS"]);
                        }

                        
                        formatRange = xlWorkSheet.Range["A" + rowDatos, letra + rowDatos];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        cantEmples += Convert.ToInt32(dr["LLAMADAS"]);
                        totRenta += Convert.ToDecimal(dr["MINUTOS"]);                      
                        totTotal += Convert.ToDecimal(dr["TOTALREAL"]);
                        rowDatos++;
                    }

                    formatRange = xlWorkSheet.Range["A16", "E16"];
                    formatRange.AutoFilter(1);

                    xlWorkSheet.Cells[(totalRows + 17), 1] = "Total";
                    xlWorkSheet.Cells[(totalRows + 17), 2] = totTotal;
                    xlWorkSheet.Cells[(totalRows + 17), 3] = cantEmples;
                    xlWorkSheet.Cells[(totalRows + 17), 4] = totRenta;
                    if (opc2 == 1)
                    {
                        xlWorkSheet.Cells[(totalRows + 17), 5] = totExcedente;
                    }
                    
                   
                    formatRange = xlWorkSheet.Range["B17" + ":" + "B" + (totalRows + 17),"B17" + ":" + "B" + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";

                    formatRange = xlWorkSheet.Range["A" + (totalRows + 17), letra + (totalRows + 17)];
                    formatRange.Font.Bold = true;
                    formatRange.Font.Size = 11;
                    formatRange.Font.Name = "Arial";

                    formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows + 17), letra + "16" + ":" + letra + (totalRows + 17)];
                    formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    AllBorders(formatRange);
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
                    formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows + 17), letra + "16" + ":" + letra + (totalRows + 17)];
                    formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    AllBorders(formatRange);
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
                    formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows + 17), letra + "16" + ":" + letra + (totalRows + 17)];
                    formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    AllBorders(formatRange);
                    break;
                case 4:
                    letra = "";
                    int numColum = dt.Columns.Count;
                    string colFin = "";
                    if (numColum == 13)
                    {
                        letra = "M";
                    }
                    else
                    {
                        letra = "H";
                    }
                    
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

                    formatRange = xlWorkSheet.Range["L17" + ":" + "L" + (totalRows + 17), "L17" + ":" + "L" + (totalRows + 17)];
                    formatRange.NumberFormat = "$#,##0.00";
                    formatRange = xlWorkSheet.Range["A16" + ":" + "A" + (totalRows + 17), letra + "16" + ":" + letra + (totalRows + 17)];
                    formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    AllBorders(formatRange);
                    break;
                case 5:
                    int numeroColum = dt.Columns.Count;
                    int indiceColumn = 0;
                    double suma = 0;
                    foreach (DataRow row in dt.Rows)  //Filas
                    {
                        indiceColumn = 0;

                        foreach (DataColumn col in dt.Columns)  //Columnas
                        {
                            indiceColumn++;
                            double value;
                            var valor = row[col.ColumnName];
                            bool suc = double.TryParse(valor.ToString().Replace("'","").Trim(),out value);
                            if (suc)
                            {
                                formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[rowDatos, indiceColumn], xlWorkSheet.Cells[rowDatos, indiceColumn]];                                
                                formatRange.NumberFormat = "$#,##0.00";
                                formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                            }
                            xlWorkSheet.Cells[rowDatos, indiceColumn] = row[col.ColumnName];
                            if(col.ColumnName == "Total" || col.ColumnName == "IMPORTE")
                            {
                                suma += Convert.ToDouble(row[col.ColumnName]);
                            }
                        }

                        formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[rowDatos, 1], xlWorkSheet.Cells[rowDatos, numeroColum]];
                        var colorf = rowDatos % 2 == 0 ? color2 : color1;
                        AllBorders(formatRange);
                        formatRange.Interior.Color = ColorTranslator.ToOle(colorf);

                        rowDatos++;
                    }

                    xlWorkSheet.Cells[(totalRows + 17), 1]= "Total";
                    xlWorkSheet.Cells[(totalRows + 17), numeroColum] = suma;
                    formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[(totalRows + 17), numeroColum], xlWorkSheet.Cells[(totalRows + 17), numeroColum]];
                    formatRange.NumberFormat = "$#,##0.00";
                    formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[(totalRows + 17), 1], xlWorkSheet.Cells[(totalRows + 17), numeroColum]];
                    formatRange.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                   
                    formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[16, 1], xlWorkSheet.Cells[(totalRows + 17), numeroColum]];
                    AllBorders(formatRange);
                    break;
                default:
                    break;
            }

            if(opcion != 5)
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