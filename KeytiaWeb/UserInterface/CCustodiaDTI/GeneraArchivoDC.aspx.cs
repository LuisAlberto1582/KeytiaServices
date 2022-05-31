using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class GeneraArchivoDC : System.Web.UI.Page
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

                DataTable dt = DSODataAccess.Execute(ObtieneFechaFactura(), connStr);
                DataRow dr = dt.Rows[0];
                DateTime fecha = Convert.ToDateTime(dr["Fecha"].ToString());

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla" + ".xlsx");
                lExcel.Abrir();

                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");


                string sp = "EXEC dbo.ObtieneInfoTelmex @FECHAPUB = '" + fecha.ToString("yyyy-MM-dd") + "'";

                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Archivo DC Todos los Carriers");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Archivo DC Todos los Carriers");
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
        private string ObtieneFechaFactura()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT");
            query.AppendLine(" MAX(fechafacturacion) AS Fecha");
            query.AppendLine(" FROM(");
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(DATE, FechaFacturacion + '01') AS fechafacturacion");
            query.AppendLine(" FROM SIANA.bat.TelmexLD WITH(NOLOCK)");
            query.AppendLine(" GROUP BY fechafacturacion");
            query.AppendLine(" UNION");
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(DATE, FechaFacturacion + '01') AS fechafacturacion");
            query.AppendLine(" FROM SIANA.bat.TelmexSM WITH(NOLOCK)");
            query.AppendLine(" GROUP BY fechafacturacion");
            query.AppendLine(" UNION");
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(DATE, FechaFacturacion + '01') AS FechaFacturacion");
            query.AppendLine(" FROM SIANA.bat.TelmexRentas WITH(NOLOCK)");
            query.AppendLine(" GROUP BY FechaFacturacion");
            query.AppendLine(" UNION");
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(DATE, FechaFacturacion + '01') AS FechaFacturacion");
            query.AppendLine(" FROM SIANA.bat.TelmexCP WITH(NOLOCK)");
            query.AppendLine(" GROUP BY FechaFacturacion");
            query.AppendLine(" UNION");
            query.AppendLine(" SELECT");
            query.AppendLine(" CONVERT(DATE, FechaFacturacion + '01') AS FechaFacturacion");
            query.AppendLine(" FROM SIANA.bat.TelmexUninet WITH(NOLOCK)");
            query.AppendLine(" GROUP BY FechaFacturacion");
            query.AppendLine(" ) AS TELMEX");
            return query.ToString();
        }
    }
}