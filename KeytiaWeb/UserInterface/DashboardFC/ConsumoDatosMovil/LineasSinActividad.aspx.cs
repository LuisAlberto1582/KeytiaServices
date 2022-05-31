using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Drawing;

namespace KeytiaWeb.UserInterface.DashboardFC.ConsumoDatosMovil
{
    public partial class LineasSinActividad : System.Web.UI.Page
    {
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        string fechaI = string.Empty;
        string fechaF = string.Empty;
        string rutaLogos = AppDomain.CurrentDomain.BaseDirectory;


        bool ValidaConsultaFechasBD()
        {
            return true;
        }

        public string ConsultaFechaMaximaDeDetallFactCDR()
        {
            StringBuilder lsb = new StringBuilder();
            //RM 20161214 Se modifico la consulta para que regrese una fecha por default, el primero del mes actual en dado caso de no encontrar una 
            lsb.Append("select isNull(Max(FechaInicio),'" + DateTime.Now.ToString("yyyy-MM-01 00:00:00") + "') as FechaInicio \n ");
            lsb.Append("from " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleFacturaCDR','Español')]  \n ");
            lsb.Append("where Carrier = 373 \n ");
            return lsb.ToString();
        }

        private void CalculaFechasDeDashboard()
        {
            DateTime ldtFechaInicio = new DateTime();
            string lsfechaInicio = DSODataAccess.ExecuteScalar(ConsultaFechaMaximaDeDetallFactCDR()).ToString();
            if (DateTime.TryParse(lsfechaInicio, out ldtFechaInicio))
            {
                //NZ 20150319 Se establecio que siempre se mostrara el primer dia de cada mes.
                fechaInicio = new DateTime(ldtFechaInicio.Year, ldtFechaInicio.Month, 1);
                fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);
            }

            Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
            Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;


            if (!Page.IsPostBack && (ValidaConsultaFechasBD() || (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")))
            {
                if (Session["FechaInicio"].ToString() == "" && Session["FechaFin"].ToString() == "")
                {
                    try
                    {
                        CalculaFechasDeDashboard();
                    }
                    catch (Exception ex)
                    {
                        throw new KeytiaWebException(
                            "Ocurrio un error al darle valores default a los campos de fecha en '" + Request.Path
                            + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                    }
                }
            }
            else
            {
                //En este caso que es Moviles, aquí se ven meses completos, así que si se selecciona una fecha 
                //diferente el primer mes, se mueven internamente las fechas para que siempre sea mes completo.
                if (Session["FechaInicio"] != null && Session["FechaInicio"] != "")
                {
                    DateTime fechaAux = Convert.ToDateTime(Session["FechaInicio"].ToString());

                    fechaInicio = new DateTime(fechaAux.Year, fechaAux.Month, 1);
                    fechaFinal = fechaInicio.AddMonths(1).AddDays(-1);

                    Session["FechaInicio"] = fechaInicio.ToString("yyyy-MM-dd");
                    Session["FechaFin"] = fechaFinal.ToString("yyyy-MM-dd");
                }
            }


            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
            fechaI = HttpContext.Current.Session["FechaInicio"].ToString();
            fechaF = HttpContext.Current.Session["FechaFin"].ToString();
            (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();
            (Master as KeytiaOH).ChangeCalendar(false);   //Hace que la pagina tenga un solo calendario.
            ObtieneDatos();
        }
        private void ObtieneDatos()
        {
            RepLineasInactivasCantidad(Rep1);
            RepLineasInactivasSinImportes(Rep2);
        }
        private void RepLineasInactivasCantidad(Control contenedor)
        {

            string tituloReporte = "Total mensual de lineas inactivas";

            System.Data.DataTable dt = DSODataAccess.Execute(ConsultaTablaMesesInactivosCantidad());

            if (dt != null && dt.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("ReportePrincipal", dt, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dt.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));
            }
        }
        private void RepLineasInactivasSinImportes(Control contenedor)
        {

            System.Data.DataTable dt = DSODataAccess.Execute(ConsultaTablaMesesInactivosSinImporte());

            string tituloReporte = "Consumo Lineas Inactivas";

            if (dt != null && dt.Rows.Count > 0)
            {
                rowBusqueda.Visible = true;
                int[] camposBoundField = new int[] { 0, 1, 2, 3, 4, 5, 6 };
                int[] camposNavegacion = new int[] { };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("RepConsumoMovil_t", dt, false, "",
                      formatoColumnas, "", new string[] { " " }, 0,
                      camposNavegacion, camposBoundField, new int[] { }, 2);

                #region Marcar celdas inactivas
                foreach (GridViewRow row in grid.Rows)
                {
                    for (int i = 1; i < grid.Columns.Count; i++)
                    {
                        if (row.Cells[i].Text == "SIN ACTIVIDAD")
                        {
                            row.Cells[i].CssClass = "textColorTable";
                        }
                    }
                }
                #endregion
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "RepConsumoMovil", tituloReporte, string.Empty));
            }
            else
            {
                rowBusqueda.Visible = false;
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "RepConsumoMovil", tituloReporte, string.Empty));
            }
        }
        private string ConsultaTablaMesesInactivosSinImporte()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC [dbo].[ObtieneLineasSinActividad]");
            query.AppendLine(" @Esquema =  '" + DSODataContext.Schema + "',");
            query.AppendLine(" @Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine(" @Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine(" @FechaFin =  '" + Session["FechaInicio"].ToString() + "'");
            return query.ToString();
        }

        private string ConsultaTablaMesesInactivosCantidad()
        {
            DateTime fechaAux1 = Convert.ToDateTime(Session["FechaInicio"].ToString());
            DateTime fechaFinal1 = new DateTime(fechaAux1.Year, fechaAux1.Month, 1);
            DateTime fechaInicio1 = fechaFinal1.AddMonths(-5);

            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC [ObtieneLineasSinActividadCantidadNMeses]");
            query.AppendLine("@Schema =  '" + DSODataContext.Schema + "',");
            query.AppendLine("@Usuario =  '" + Session["iCodUsuario"] + "',");
            query.AppendLine("@Perfil =  '" + Session["iCodPerfil"] + "',");
            query.AppendLine("@FechaIni =  '" + fechaInicio1.ToString("yyyy-MM-dd") + "',");
            query.AppendLine("@FechaFin =  '" + fechaFinal1.ToString("yyyy-MM-dd") + "'");
            return query.ToString();
        }

        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            
            ExportFile(GeneraExcel());
        }
        public void ExportFile(string filePath)
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
        private string GeneraExcel()
        {
            var path = Util.AppSettings("FolderTemp");

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
            var pathLogo = rutaLogos + lsImg ;
            string pathFile = "";

            pathFile = Path.Combine(path, "Lineas Sin Actividad" + "_" + Session["FechaInicio"].ToString() + ".xls");
           
            Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            int numRow = 0;

            xlApp = new Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Range formatRange;

            System.Data.DataTable dt = DSODataAccess.Execute(ConsultaTablaMesesInactivosSinImporte());

            try
            {
                numRow = dt.Rows.Count + 16;
                if (File.Exists(pathFile))
                    File.Delete(pathFile);

                if (File.Exists(pathLogo))
                {
                    xlWorkSheet.Shapes.AddPicture(pathLogo, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 100, 50);
                }

                formatRange = xlWorkSheet.Range["A1:A"+ numRow, "G1:G"+ numRow];
                formatRange.NumberFormat = "@";
                formatRange.Font.Size = 12;

                xlWorkSheet.Cells[8, 1] = "Reporte Lineas Sin Actividad Mensual";

                xlWorkSheet.Cells[10, 1] = "Período:";
                /*FORMATEA ENCABEZADOS*/
                EstiloEncabezadosTabla(xlWorkSheet, 10, 10, 1, 1, true);

                xlWorkSheet.Cells[10, 2] = Convert.ToDateTime(Session["FechaInicio"]).ToString("MMMM yyyy").ToUpper();

                GeneraRow(dt, xlWorkSheet, 12, 1);
                // FormatoTitulos(xlWorkSheet, "A", "G", 16);
                xlApp.DisplayAlerts = false;
                xlWorkBook.SaveAs(pathFile, XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                xlWorkBook.Close(false, misValue, misValue);
                xlApp.Quit();
                throw new KeytiaWebException("Ocurrio un error en :" + ex);
            }
            return pathFile;
        }
        private void GeneraRow(System.Data.DataTable dt, Worksheet worksheet,int rowIni,int colIni)
        {
            int numeroColum = dt.Columns.Count;
            int rowFin = dt.Rows.Count + rowIni;
            int rowDatos = rowIni +1;
            int indiceColumna = colIni;
            Range formatRange;

            /*GENERA ENCABEZADOS*/
            foreach (DataColumn col in dt.Columns)  //Columnas
            {                
                worksheet.Cells[rowIni, indiceColumna] = col.ColumnName;
                indiceColumna++;
            }

            var color1 = Color.FromArgb(220, 225, 231);
            var color2 = Color.White;
            /*FORMATEA ENCABEZADOS*/
            EstiloEncabezadosTabla(worksheet, rowIni, rowIni, colIni, numeroColum, true);

            /*VACIA LA INFORMACION EN LA HOJA*/
            foreach (DataRow row in dt.Rows)//Filas
            {
                indiceColumna = colIni;

                foreach (DataColumn col in dt.Columns)  //Columnas
                {
                    var dato = row[col.ColumnName];
                    worksheet.Cells[rowDatos, indiceColumna] = dato;

                    var colorf = rowDatos % 2 == 0 ? color2 : color1;
                    Range formatRange1;
                    formatRange1 = worksheet.Range[worksheet.Cells[rowDatos, indiceColumna], worksheet.Cells[rowDatos, indiceColumna]];

                    if (dato.ToString().Trim() == "SIN ACTIVIDAD")
                    {

                        formatRange1.Font.Color = ColorTranslator.ToOle(Color.Red);
                    }

                    formatRange1.Interior.Color = ColorTranslator.ToOle(colorf);
                    indiceColumna++;
                }

                formatRange = worksheet.Range[worksheet.Cells[rowDatos, colIni], worksheet.Cells[rowDatos, indiceColumna-1]];
                formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                AllBorders(formatRange);

                rowDatos++;
            }

            worksheet.Columns.AutoFit();
        }
        private void EstiloEncabezadosTabla(Worksheet xlWorkSheet, int rowIni, int rowFin, int colIni, int colFin, bool autoFit)
        {
            Range formatRange;
            formatRange = xlWorkSheet.Range[xlWorkSheet.Cells[rowIni, colIni], xlWorkSheet.Cells[rowFin, colFin]];
            formatRange.Font.Bold = true;
            formatRange.NumberFormat = "@";
            formatRange.Interior.Color = ColorTranslator.ToOle(Color.Blue);
            formatRange.Font.Color = ColorTranslator.ToOle(Color.White);
            formatRange.Font.Size = 11;
            formatRange.Font.Name = "Calibri";
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

        }
        private void AllBorders(Range formatRange)
        {
            formatRange.Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            formatRange.BorderAround(XlLineStyle.xlContinuous,
               XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
               XlColorIndex.xlColorIndexAutomatic);
        }
        private void FormatoTitulos(Worksheet xlWorkSheet, string col1, string col2, int celda)
        {
            Range formatRange;
            formatRange = xlWorkSheet.Range[col1 + celda + ":" + col2 + celda];
            formatRange.NumberFormat = "@";
            formatRange.Font.Bold = true;
            //formatRange.WrapText = true;
            formatRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            formatRange.VerticalAlignment = XlVAlign.xlVAlignBottom;
          
            formatRange.Font.Size = 11;
            formatRange.BorderAround(XlLineStyle.xlContinuous,
            XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic,
            XlColorIndex.xlColorIndexAutomatic);
            formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

            formatRange.Font.Name = "Arial";
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
    }
}