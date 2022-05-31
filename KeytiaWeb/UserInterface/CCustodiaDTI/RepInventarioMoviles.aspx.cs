using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class RepInventarioMoviles : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
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

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                string sp = "EXEC [ObtieneLayoutLineas] @Schema = '" + esquema + "'";
                // System.Data.DataTable dt = DSODataAccess.Execute(sp, connStr);
                //string file = excelFile.GeneraArchivoExcel("RepLayout_" + DateTime.Now.ToString("hhmmss"), dt);
                //ExportFiles(file);
                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Inventario Lineas");
                DataTable ldt = DSODataAccess.Execute(sp);
                if (ldt.Rows.Count > 0)
                {
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, ldt, "Reporte", "Tabla");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "InventarioLineas");
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


        protected void ExportarArchivo(string lsExt, string psFileKey, string psTempPath, string nombreArchivo)
        {
            string lsTitulo = HttpUtility.UrlEncode(nombreArchivo + DateTime.Today.ToString("dd-MM-yyyy"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
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
    }
}