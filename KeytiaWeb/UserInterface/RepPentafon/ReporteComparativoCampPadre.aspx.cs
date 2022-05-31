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

namespace KeytiaWeb.UserInterface.RepPentafon
{
    public partial class ReporteComparativoCampañaPadre : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        /*Se inician el valor de las fechas del primer dia del mes en curso al dia actual del mes en curso*/
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
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            try
            {
                //NZ Este codigo se debe incluir para que el backend se entere de que las fechas del frente cambiaron.
                fechaI = HttpContext.Current.Session["FechaInicio"].ToString();
                fechaF = HttpContext.Current.Session["FechaFin"].ToString();
                (Master as KeytiaOH).ExtraerFechasRangeFrontToBack();


                if (!Page.IsPostBack)
                {
                    LlenaTipoConsumoList();
                    IniciaProcess();
                }

                if (Session["FechaInicio"].ToString() != fechaI || Session["FechaFin"].ToString() != fechaF)
                {
                    IniciaProcess();
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private void IniciaProcess()
        {
            var dato = cboTipo.SelectedItem.Value;
            var opc = cboTipo.SelectedItem.Text;
            DataTable dt = DSODataAccess.Execute(ObtieneDatos(dato),connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                pnl1.Visible = true;
                DataView dvldt = new DataView(dt);
                dt.Columns["Descripcion"].ColumnName = "Campaña";
                dt = DTIChartsAndControls.ordenaTabla(dt, "TotalGeneral Desc");
                dt.AcceptChanges();
                List<string> list = new List<string>();
                list.Add("");
                var cantCols = dt.Columns.Count;
                var tipoDato = (opc == "Costo") ? "{0:c}" : "";
                for (int i = 1; i <= cantCols; i++)
                {                    
                    list.Add(tipoDato);
                }
                string[] formatos = list.ToArray();
                Control Controles = DTIChartsAndControls.GridView("RepComparaCampCarrier", dt, true, "Totales",formatos);
                pnl1.Controls.Add(Controles);
                //pnl1.Controls.Add(DTIChartsAndControls.TituloY2RowsTablaGrafica(Controles, "RepComparaCampCarrier", "", "", true));
            }
            else
            {
                lblMensajeInfo.Text = "¡No se encontro información para mostrar!";
                pnlInfo.Visible = true;
            }

        }
        private string ObtieneDatos(string tipo)
        {
           
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC dbo.ObtieneReportePorCampañaPadre");
            query.AppendLine(" @Esquema = '"+esquema+"',");
            query.AppendLine(" @FechaIni = '"+ Session["FechaInicio"].ToString() + "',");
            query.AppendLine(" @FechaFin = '" + Session["FechaFin"].ToString() + "',");
            query.AppendLine(" @TipoTotal = '"+ tipo + "',");
            query.AppendLine(" @Usuario = " + iCodUsuario + ",");
            query.AppendLine(" @Perfil = " + iCodPerfil + "");
            return query.ToString();
        }
        private void LlenaTipoConsumoList()
        {
            cboTipo.Items.Clear();
            ListItem i;
            i = new ListItem("Costo", "(Costo + CostoSM)");
            cboTipo.Items.Add(i);
            i = new ListItem("Llamadas", "TotalLlamadas");
            cboTipo.Items.Add(i);
            i = new ListItem("Minutos", "DuracionMin");
            cboTipo.Items.Add(i);
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            IniciaProcess();
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
                ExportacionExcelRep.ProcesarTituloExcel(lExcel, "Reporte comparativo por campañas padre");
                var dato = cboTipo.SelectedItem.Value;
                DataTable dt = DSODataAccess.Execute(ObtieneDatos(dato), connStr);
                if(dt != null && dt.Rows.Count > 0)
                {
                    DataView dvldt = new DataView(dt);
                    dt.Columns["Descripcion"].ColumnName = "Campaña";
                    dt = DTIChartsAndControls.ordenaTabla(dt, "TotalGeneral Desc");
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

                ExportarArchivo(".xlsx", psFileKey, psTempPath, "Reportes" + "_" + "Reporte comparativo por campañas padre" + "_");
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