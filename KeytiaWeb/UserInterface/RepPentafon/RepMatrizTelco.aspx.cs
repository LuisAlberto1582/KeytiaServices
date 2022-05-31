using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public partial class RepMatrizTelco : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        protected DateTime fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        protected DateTime fechaFinal = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        string fechaI = string.Empty;
        string fechaF = string.Empty;
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
            fechaI = HttpContext.Current.Session["FechaInicio"].ToString();
            fechaF = HttpContext.Current.Session["FechaFin"].ToString();
            (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();

            //if (!Page.IsPostBack)
            //{
            //   
            //}

            ObtieneDatos();
        }
        private void ObtieneDatos()
        {

            DataTable dt = DSODataAccess.Execute(ObtieneInformacion(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                rowBuscarText.Visible = true;
                pnlInfo.Visible = false;
                pnlGrid.Visible = true;
                GeneraReporte(dt);
            }
            else
            {
                rowBuscarText.Visible = false;
                pnlInfo.Visible = true;
                pnlGrid.Visible = false;
                lblMensajeInfo.Text = "¡No se encontro información para mostrar!";
            }
        }
        private void GeneraReporte(DataTable dt)
        {
            try
            {

                string[] formatos = { "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" };
                Control Controles = DTIChartsAndControls.GridView("RepMatrizTelco", dt, true, "Totales", formatos);
                repMatriz.Controls.Add(Controles);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ObtieneInformacion()
        {
            string query = "EXEC dbo.GeneraMatrizTelco  @FechaIni = '{0}', @FechaFin = '{1}', @Usuario = {2},@Perfil = {3}";
            string sp = string.Format(query, Session["FechaInicio"].ToString() + " 00:00:00", Session["FechaFin"].ToString() + " 23:59:59", Session["iCodUsuario"], Session["iCodPerfil"].ToString());
            return sp;
        }
        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            ExcelAccess lExcel = new ExcelAccess();
            try
            {
                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());
                lExcel.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\DashBoardFC\ReporteTabla.xlsx");
                lExcel.Abrir();
                lExcel.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");
                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte Matriz Telco");


                DataTable dt = DSODataAccess.Execute(ObtieneInformacion(), connStr);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);

                    //dt = DTIChartsAndControls.ordenaTabla(dt, "Cantidad de llamadas perdidas  Desc");
                    dt.AcceptChanges();
                    ExportacionExcelRep.CreaTablaEnExcel(lExcel, DTIChartsAndControls.agregaTotales(dt, 0, "Totales"), "Reporte", "Tabla");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + "Reporte Matriz Telco" + "_");
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

    }
}