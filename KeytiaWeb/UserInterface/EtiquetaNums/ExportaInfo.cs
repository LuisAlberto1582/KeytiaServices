using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Excel;
namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public class ExportaInfo
    {     
        public string GeneraArchivoExcel(string nameFiles,string[] datosEmple,string fechaEtiq, GridView gridViewLocal, GridView gridViewTotLoc,GridView grvLlamCobrar,GridView grdTotCobrar, GridView grdMoviles, GridView grdTotMoviles,string[] totales,int page)
        {
            var nombre = datosEmple[0];
            var departamento = datosEmple[1];
            var localidad = datosEmple[2];
            var cencos = datosEmple[3];
            var numEmple = datosEmple[4];
            var numDepto = datosEmple[5];
            var numLocalidad = datosEmple[6];
            var numCencos = datosEmple[7];

            var path= KeytiaServiceBL.Util.AppSettings("FolderTemp");
            string pathFile = "";
            pathFile = System.IO.Path.Combine(path, nameFiles+"_" + nombre + "_" + fechaEtiq + ".xls");
            Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            int numeroCelda = 5;

            xlApp = new Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Range formatRange;
            try
            {

                if (File.Exists(pathFile))
                    File.Delete(pathFile);
                formatRange = xlWorkSheet.Range["A1:A3", "G1:G3"];
                formatRange.NumberFormat = "@";
                formatRange.Font.Size = 12;
                formatRange.WrapText = true;
                formatRange.ColumnWidth = 33;

                xlWorkSheet.Cells[1, 1] = "Nombre del Empleado: ";
                xlWorkSheet.Cells[1, 2] = nombre;
                xlWorkSheet.Cells[1, 3] = "Número de empleado: ";
                xlWorkSheet.Cells[1, 4] = numEmple;
                xlWorkSheet.Cells[2, 1] = "Departamento: ";
                xlWorkSheet.Cells[2, 2] = departamento;
                xlWorkSheet.Cells[2, 3] = "Número del Departamento: ";
                xlWorkSheet.Cells[2, 4] = numDepto;
                xlWorkSheet.Cells[3, 1] = "Localidad: ";
                xlWorkSheet.Cells[3, 2] = localidad;
                xlWorkSheet.Cells[3, 3] = "Número de Localidad: ";
                xlWorkSheet.Cells[3, 4] = numLocalidad;

                /*validar si existen llamadas locales*/
                if (gridViewLocal != null && gridViewLocal.Rows.Count > 0)
                {                   
                    xlWorkSheet.Cells[numeroCelda, 4] = "Detalle de llamadas locales - " + fechaEtiq;
                    FormatoTitulos(xlWorkSheet, "A", "I", numeroCelda,1);
                    numeroCelda = GeneraRows(gridViewLocal, gridViewTotLoc, 5, xlWorkSheet);                   
                    xlWorkSheet.Cells[numeroCelda - 2, 1] = "Las llamadas locales por el momento no se cobrarán.";
                }
                //else
                //{numeroCelda = 5;}              
                /*Se obtiene la Informacion del detalle de llamadas LD Y CEL que se cobran*/
                if(grvLlamCobrar != null && grvLlamCobrar.Rows.Count > 0)
                {
                    xlWorkSheet.Cells[numeroCelda, 4] = "Detalle de llamadas de Larga Distancia y a Celular - " + fechaEtiq;
                    FormatoTitulos(xlWorkSheet, "A", "I", numeroCelda,1);
                    numeroCelda = GeneraRows(grvLlamCobrar, grdTotCobrar, numeroCelda, xlWorkSheet);
                }
                //else
                //{numeroCelda = 5;}

                /*SE OBTIENE LA INFORMACION DE LAS LLAMADAS DE MOVILES*/
                if( grdTotMoviles != null && grdMoviles.Rows.Count > 0)
                {
                    xlWorkSheet.Cells[numeroCelda, 4] = "Detalle de llamadas de Telefonía Móvil - " + fechaEtiq;
                    FormatoTitulos(xlWorkSheet, "A", "I", numeroCelda,1);
                    numeroCelda = GeneraRows(grdMoviles, grdTotMoviles, numeroCelda, xlWorkSheet);                    
                }
                //else
                //{numeroCelda = 5;}

                if(page == 1)
                {
                    /*Se registran los totales*/
                    int r = numeroCelda;
                    xlWorkSheet.Cells[r, 1] = "Total de llamadas Cel/LD: " + totales[0];
                    xlWorkSheet.Cells[r + 1, 1] = "";
                    xlWorkSheet.Cells[r + 2, 1] = "Tiempo total: " + totales[1];

                    xlWorkSheet.Cells[r, 2] = "  Importe de Llamadas Personales Aceptadas: $" + totales[3];
                    xlWorkSheet.Cells[r + 1, 2] = "Importe Negocio: $" + totales[4];
                    xlWorkSheet.Cells[r + 2, 2] = "Importe Total: $" + totales[2];

                    formatRange = xlWorkSheet.Range["A" + r + ":A" + (r + 2), "C" + r + ":C" + (r + 2)];
                    formatRange.NumberFormat = "@";
                    formatRange.Font.Bold = true;
                    formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    formatRange.HorizontalAlignment = XlHAlign.xlHAlignRight;
                    formatRange.VerticalAlignment = XlVAlign.xlVAlignBottom;
                    formatRange.BorderAround(XlLineStyle.xlContinuous,
                                                XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
                                                XlColorIndex.xlColorIndexAutomatic);
                    formatRange.Font.Size = 10;
                    formatRange.Font.Name = "Arial";
                    formatRange = xlWorkSheet.Range["B" + r + ":C" + r];
                    formatRange.Merge(false);
                    formatRange = xlWorkSheet.Range["B" + (r + 1) + ":C" + (r + 1)];
                    formatRange.Merge(false);
                    formatRange = xlWorkSheet.Range["B" + (r + 2) + ":C" + (r + 2)];
                    formatRange.Merge(false);

                    formatRange = xlWorkSheet.Range["B" + r + ":C" + r];
                    formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
                }
               
                
                #region formato excel
                xlApp.DisplayAlerts = false;
                xlApp.ActiveWindow.DisplayGridlines = false;



                formatRange = xlWorkSheet.Range["A1:A3"];
                formatRange.Font.Bold = true;
                formatRange.HorizontalAlignment = XlHAlign.xlHAlignRight;
                formatRange.VerticalAlignment = XlVAlign.xlVAlignBottom;

                formatRange = xlWorkSheet.Range["C1:C3"];
                formatRange.Font.Bold = true;
                formatRange.HorizontalAlignment = XlHAlign.xlHAlignRight;
                formatRange.VerticalAlignment = XlVAlign.xlVAlignBottom;

                formatRange = xlWorkSheet.Range["A1:A3", "D1:D3"];
                formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                formatRange.BorderAround(XlLineStyle.xlContinuous,
                XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
                XlColorIndex.xlColorIndexAutomatic);


                xlWorkBook.SaveAs(pathFile, XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                #endregion formato excel

                return pathFile;

            }
            catch (Exception ex)
            {
                xlWorkBook.Close(false, misValue, misValue);
                xlApp.Quit();
                return "";
                throw new KeytiaWebException("Ocurrio un error en :" + ex);
            }
        }
        private int GeneraRows(GridView detalle,GridView totales,int numRow, Worksheet xlWorkSheet)
        {
            int renglon = numRow;

            var headerRow = detalle.HeaderRow;
            var totalCols = detalle.Rows[0].Cells.Count - 1;
            var totalRows = detalle.Rows.Count;
            int r = renglon + 2;
            Range formatRange;

            formatRange = xlWorkSheet.Range["A" + r, "H" + r];
            formatRange.Font.Bold = true;
            formatRange.NumberFormat = "@";
            formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
            formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            formatRange.Font.Size = 11;
            formatRange.Font.Name = "Arial";
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

            /*Obtiene los nombres de la columnas*/
            for (var i = 1; i <= totalCols; i++)
            {
                xlWorkSheet.Cells[renglon + 2, i] = HttpUtility.HtmlDecode(headerRow.Cells[i - 1].Text);
            }

            /*Obtiene la info del Gridview*/
            int renglon1 = renglon + 3;
            for (var row = 1; row <= totalRows; row++)
            {
                for (var col = 0; col < totalCols; col++)
                {
                    var nomcol = HttpUtility.HtmlDecode(headerRow.Cells[col].Text);
                    string dato = "";

                    var controls = detalle.Rows[row - 1].Cells[col].Controls.Count;
                    if(controls > 0)
                    {
                        var control = detalle.Rows[row - 1].Cells[col].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                        var localidad = detalle.Rows[row - 1].Cells[col].FindControl("txtRferencia") as System.Web.UI.WebControls.TextBox;
                        if (control.ID == "chkRow" && nomcol == "Negocio")
                        {
                            if (control.Checked == true)
                            {
                                dato = "X";
                            }
                        }
                        else if (localidad.ID == "txtRferencia" && nomcol == "Referencia")
                        {
                            dato = localidad.Text;
                        }

                    }
                    else
                    {
                        dato = HttpUtility.HtmlDecode(detalle.Rows[row - 1].Cells[col].Text.Replace("&nbsp;", " ").ToString());
                    }

                    if (nomcol == "Número")
                    {
                        formatRange = xlWorkSheet.Range["C" + renglon1, "C" + renglon1];
                        formatRange.NumberFormat = "@";
                    }
                    else if(nomcol == "Número Marcado" || nomcol == "Fecha(dd/mm/aaaa)")
                    {
                        formatRange = xlWorkSheet.Range["B" + renglon1, "B" + renglon1];
                        formatRange.NumberFormat = "@";
                        formatRange = xlWorkSheet.Range["D" + renglon1, "D" + renglon1];
                        formatRange.NumberFormat = "@";
                    }

                    xlWorkSheet.Cells[renglon1, col + 1] = dato.ToString();
                }

                renglon1++;
            }

            formatRange = xlWorkSheet.Range["A"+(renglon + 3) +":"+"A"+renglon1, "H" + (renglon + 3) + ":" + "H" + renglon1];
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            formatRange.BorderAround(XlLineStyle.xlContinuous,
               XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
               XlColorIndex.xlColorIndexAutomatic);

            /*agrega los totales*/
            if(totales != null && totales.Rows.Count > 0)
            {
                xlWorkSheet.Cells[renglon1, 3] = totales.Rows[0].Cells[0].Text;
                xlWorkSheet.Cells[renglon1, 5] = totales.Rows[0].Cells[1].Text;
                xlWorkSheet.Cells[renglon1, 6] = totales.Rows[0].Cells[2].Text;
                FormatoTitulos(xlWorkSheet, "A", "H", renglon1, 2);
            }



            renglon = renglon1 + 3;


            return renglon;

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
                //Console.WriteLine(ex.ToString());
                //Console.ReadKey();
            }
            finally
            {
                GC.Collect();
            }
        }
        private void FormatoTitulos(Worksheet xlWorkSheet,string col1, string col2,int celda,int opc)
        {
            Range formatRange;
            formatRange = xlWorkSheet.Range[col1 + celda + ":"+ col2 + celda];
            formatRange.NumberFormat = "@";
            formatRange.Font.Bold = true;
            //formatRange.WrapText = true;
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            formatRange.VerticalAlignment = XlVAlign.xlVAlignBottom;
            if (opc == 1)
            {
                formatRange.Font.Size = 12;
                formatRange.Merge(false);               
            }
            else
            {
                formatRange.Font.Size = 11;
                formatRange.BorderAround(XlLineStyle.xlContinuous,
                XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
                XlColorIndex.xlColorIndexAutomatic);
                formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            }
            
            formatRange.Font.Name = "Arial";
        }
        
    }
}