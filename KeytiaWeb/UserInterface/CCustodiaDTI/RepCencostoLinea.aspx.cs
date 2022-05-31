using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class RepCencostoLinea : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            ExportaFiles excelFile = new ExportaFiles();

            string sp = "EXEC [ObtieneCencosLineaVSCencosEmple] @Schema = '" + esquema + "'";
            //System.Data.DataTable dt = DSODataAccess.Execute(sp, connStr);
            //string file = excelFile.GeneraArchivoExcel("RepCencostoLinea_" + DateTime.Now.ToString("hhmmss"),dt);
            //ExportFiles(file);
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "CenCosto Linea VS Cencosto Emple");
                System.Data.DataTable ldt = DSODataAccess.Execute(sp);
                if (ldt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel,ldt, "Reporte", "Tabla");
                }
                string psFileKey;
                string psTempPath;

                psFileKey = Guid.NewGuid().ToString();
                psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
                System.IO.Directory.CreateDirectory(psTempPath);

                string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + ".xlsx");
                Session[psFileKey] = lsFileName;

                lExcel.FilePath = lsFileName;
                lExcel.SalvarComo();

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "CenCostoLineavsCencostoEmple");
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrExportTo", ex, ".xlsx");
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
        public void ExportFiles(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var buffer = File.ReadAllBytes(filePath);
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Type", "application/octet-stream");
                    Response.AddHeader("Content-disposition", "attachment; filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.BinaryWrite(buffer);
                    Response.ContentType = "application/octet-stream";
                    Response.Flush();
                    buffer = null;

                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                throw new KeytiaWebException("Ocurrio un error en : " + ex);

            }
        }
        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }
    }

    public class ExportFile
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
                CreaRow(numeroCelda,dt1, xlWorkSheet);

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